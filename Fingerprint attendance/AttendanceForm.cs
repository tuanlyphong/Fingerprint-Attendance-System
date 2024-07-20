using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using System.ComponentModel;




namespace Fingerprint_attendance
{
    public partial class AttendanceForm : Form
    {
        private string _connectionString;
        private SerialPort serialPort;
        private readonly string className;
        private bool isEnrollmentInProgress = false;
        private bool isAttendanceInProgress = false;
        private StringBuilder previousData = new StringBuilder();

        public AttendanceForm(string className, string connectionString)
        {
            InitializeComponent();
            this.className = className;
            this._connectionString = connectionString;
            InitializeSerialPort();
            LoadStudentsForClass(className);
            this.FormClosing += AttendanceForm_FormClosing;
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            System.Threading.Thread.Sleep(10);
            serialPort.ReadTimeout = 5000;
            string latestLine = serialPort.ReadLine().Trim();
            BeginInvoke(new Action(() => HandleSerialData(latestLine)));
        }



        private void CompareFingerprintTemplate(byte[] templateData)
        {
            try
            {
                int classId = GetClassId(className);             
                string query = "SELECT StudentNumber, FingerprintTemplate FROM Students WHERE ClassID = @ClassID";

                double maxConfidence = 0.0;
                int studentWithMaxConfidence = -1;

                using (SqlConnection connection = new SqlConnection(_connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClassID", classId);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string studentNumberStr = reader.GetString(0);  // Read StudentNumber as string

                        if (int.TryParse(studentNumberStr, out int studentNumber))
                        {
                            if (!IsStudentAttended(studentNumber))
                            {
                                byte[] storedTemplate = reader["FingerprintTemplate"] as byte[]; // Read FingerprintTemplate as byte array

                                if (storedTemplate != null)
                                {
                                    double confidence = CalculateTemplateSimilarity(templateData, storedTemplate);
                                    if (confidence >= 0.54 && confidence > maxConfidence)
                                    {
                                        maxConfidence = confidence;
                                        studentWithMaxConfidence = studentNumber;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show($"Fingerprint template for student {studentNumber} is null.");
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Unable to parse StudentNumber: {studentNumberStr}");
                        }
                    }

                    if (studentWithMaxConfidence != -1)
                    {
                        UpdateStudentStatusToAttended(studentWithMaxConfidence);
                        SetEnrollStatus("Success");
                    }
                    else
                    {
                        SetEnrollStatus("No Match");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while comparing fingerprint templates: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private bool IsStudentAttended(int studentNumber)
        {
            // Iterate through DataGridView rows to check if the student has already attended
            foreach (DataGridViewRow row in dgvStudents.Rows)
            {
                if (row.Cells["StudentNumber"].Value != null && int.TryParse(row.Cells["StudentNumber"].Value.ToString(), out int gridStudentNumber))
                {
                    if (gridStudentNumber == studentNumber)
                    {
                        return row.Cells["statusColumn"].Value != null && row.Cells["statusColumn"].Value.ToString() == "Attended";
                    }
                }
            }
            return false;
        }

        private void UpdateStudentStatusToAttended(int studentNumber)
        {
            foreach (DataGridViewRow row in dgvStudents.Rows)
            {
                if (row.Cells["StudentNumber"].Value != null && Convert.ToInt32(row.Cells["StudentNumber"].Value) == studentNumber)
                {
                    row.Cells["StatusColumn"].Value = "Attended"; // Update the Status column to "Attended"
                    break;
                }
            }
        }

        private double CalculateTemplateSimilarity(byte[] templateData1, byte[] templateData2)
        {
            if (templateData1.Length != templateData2.Length)
            {
                throw new ArgumentException("Templates must be of the same length.");
            }

            int differingBits = 0;

            // Calculate Hamming distance by counting differing bits
            for (int i = 0; i < templateData1.Length; i++)
            {
                byte xorResult = (byte)(templateData1[i] ^ templateData2[i]);
                differingBits += CountBits(xorResult);
            }

            int totalBits = templateData1.Length * 8;
            double similarity = 1.0 - ((double)differingBits / totalBits);

            return similarity;
        }

        private int CountBits(byte b)
        {
            int count = 0;
            while (b != 0)
            {
                count++;
                b &= (byte)(b - 1); // Clear the least significant bit set
            }
            return count;
        }



        private void SaveFingerprintTemplate(int studentID, byte[] templateData)
        {
            try
            {
                
                string query = "UPDATE Students SET FingerprintTemplate = @TemplateData WHERE StudentNumber = @StudentID";

                using (SqlConnection connection = new SqlConnection(_connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TemplateData", templateData);
                    command.Parameters.AddWithValue("@StudentID", studentID);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        SetEnrollStatus("Success");
                    }
                    else
                    {
                        MessageBox.Show("Failed to save fingerprint template.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving fingerprint template: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEnrollFingerprint_Click(object sender, EventArgs e)
        {
            if (dgvStudents.SelectedRows.Count > 0)
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Write("StartEnroll\n");
                    SetEnrollStatus("Waiting");
                }
            }
            else
            {
                MessageBox.Show("Please select a student to enroll.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnStartAttendance_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Write("StartAttend\n");
                SetEnrollStatus("Waiting");
            }
        }

         private void HandleSerialData(string latestLine)
        {
            if (latestLine == "NewEnrollStart")
            {
                isEnrollmentInProgress = true;
                previousData.Clear();
            }
            else if (isEnrollmentInProgress)
            {
                previousData.Append(latestLine);
                if (previousData.Length >= 512 * 2)
                {
                    byte[] templateData = new byte[512];
                    for (int i = 0; i < 512; i++)
                    {
                        string hexByte = previousData.ToString().Substring(i * 2, 2);
                        templateData[i] = Convert.ToByte(hexByte, 16);
                    }

                    int studentID = Convert.ToInt32(dgvStudents.SelectedRows[0].Cells["StudentNumber"].Value);
                    SaveFingerprintTemplate(studentID, templateData);
                    isEnrollmentInProgress = false;
                }
            }
            else if (latestLine == "NewAttendStart")
            {
                isAttendanceInProgress = true;
                previousData.Clear();
            }
            else if (isAttendanceInProgress)
            {
                previousData.Append(latestLine);
                if (previousData.Length >= 512 * 2)
                {
                    byte[] templateData = new byte[512];
                    for (int i = 0; i < 512; i++)
                    {
                        string hexByte = previousData.ToString().Substring(i * 2, 2);
                        templateData[i] = Convert.ToByte(hexByte, 16);
                    }

                    CompareFingerprintTemplate(templateData);
                    isAttendanceInProgress = false;
                }
            }
            else
            {
                Console.WriteLine("Received data outside of enrollment or attendance: " + latestLine);
            }
        }

        private void EnrollStatusResetTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SetEnrollStatus("Default");
        }

        private void SetEnrollStatus(string status)
        {
            if (lblEnrollStatus.InvokeRequired)
            {
                lblEnrollStatus.Invoke(new MethodInvoker(() => SetEnrollStatus(status)));
            }
            else
            {
                lblEnrollStatus.Text = $"Enrollment Status: {status}";
                if (status == "Success")
                {
                    var enrollStatusResetTimer = new System.Timers.Timer
                    {
                        Interval = 2000,
                        AutoReset = false
                    };
                    enrollStatusResetTimer.Elapsed += EnrollStatusResetTimer_Elapsed;
                    enrollStatusResetTimer.Start();
                }
                else if (status == "No Match")
                {
                    var enrollStatusResetTimer = new System.Timers.Timer
                    {
                        Interval = 2000,
                        AutoReset = false
                    };
                    enrollStatusResetTimer.Elapsed += EnrollStatusResetTimer_Elapsed;
                    enrollStatusResetTimer.Start();
                }
            }
        }

        private void InitializeSerialPort()
        {
            serialPort = new SerialPort("COM5", 9600)
            {
                ReadTimeout = 1000
            };
            try
            {
                serialPort.Open();
                serialPort.DataReceived += SerialPort_DataReceived;
            }
            catch (TimeoutException)
            {
                MessageBox.Show("Timeout occurred while reading from serial port.", "Timeout", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening serial port: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadStudentsForClass(string className)
        {
            try
            {
                
                string query = "SELECT StudentNumber, StudentName FROM Students WHERE ClassID = (SELECT ClassID FROM Classes WHERE ClassName = @ClassName)";

                using (SqlConnection connection = new SqlConnection(_connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClassName", className);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    DataTable studentsTable = new DataTable();
                    studentsTable.Load(reader);
                    dgvStudents.DataSource = studentsTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading students: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void btnAddStudent_Click(object sender, EventArgs e)
        {
            int classId = GetClassId(className);

            if (string.IsNullOrWhiteSpace(txtStudentNumber.Text) || string.IsNullOrWhiteSpace(txtStudentName.Text))
            {
                MessageBox.Show("Please enter both student number and name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                
                string query = "INSERT INTO Students (StudentNumber, StudentName, ClassID) VALUES (@StudentNumber, @StudentName, @ClassID)";

                using (SqlConnection connection = new SqlConnection(_connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentNumber", txtStudentNumber.Text);
                    command.Parameters.AddWithValue("@StudentName", txtStudentName.Text);
                    command.Parameters.AddWithValue("@ClassID", classId);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show($"Student added successfully: Number - {txtStudentNumber.Text}, Name - {txtStudentName.Text}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtStudentNumber.Text = "";
                        txtStudentName.Text = "";
                        LoadStudentsForClass(className);
                    }
                    else
                    {
                        MessageBox.Show("Failed to add student.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while adding the student: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetClassId(string className)
        {
            int classId = 0;
            try
            {
                
                string query = "SELECT ClassID FROM Classes WHERE ClassName = @ClassName";

                using (SqlConnection connection = new SqlConnection(_connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClassName", className);
                    connection.Open();
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        classId = Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while retrieving class ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return classId;
        }

        
        private void btnFinishAttendance_Click_1(object sender, EventArgs e)
        {
            // Set blank status to "Absent"
            foreach (DataGridViewRow row in dgvStudents.Rows)
            {
                if (row.Cells["statusColumn"].Value == null || string.IsNullOrWhiteSpace(row.Cells["statusColumn"].Value.ToString()))
                {
                    row.Cells["statusColumn"].Value = "Attended";
                }
            }

            // Get the current date and class name for the file name
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            string className = GetClassName(); // Assuming you have a method to get the class name
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = Path.Combine(desktopPath, $"{currentDate}_{className}.xlsx");
            // Save DataGridView to Excel file
            SaveDataGridViewToExcel(dgvStudents, fileName);

            MessageBox.Show("Attendance has been saved to Excel file.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void AttendanceForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Perform cleanup tasks here, if necessary
            // For example, closing serial port, saving data, etc.

            // Ensure the serial port is closed
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }
        private string GetClassName()
        {
            return className;
        }
        private void SaveDataGridViewToExcel(DataGridView dgv, string filePath)
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Attendance");

                // Add column headers
                for (int i = 0; i < dgv.Columns.Count; i++)
                {
                    worksheet.Cells[1, i + 1].Value = dgv.Columns[i].HeaderText;
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                    worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Add rows
                for (int i = 0; i < dgv.Rows.Count; i++)
                {
                    for (int j = 0; j < dgv.Columns.Count; j++)
                    {
                        worksheet.Cells[i + 2, j + 1].Value = dgv.Rows[i].Cells[j].Value;
                    }
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Save to file
                FileInfo file = new FileInfo(filePath);
                package.SaveAs(file);
            }
        }
    }
}
