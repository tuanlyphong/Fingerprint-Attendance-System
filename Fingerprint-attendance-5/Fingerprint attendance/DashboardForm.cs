using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Fingerprint_attendance
{
    public partial class DashboardForm : Form
    {
        private string username;
        private string _connectionString;

        public DashboardForm(string username, string connectionString)
        {
            InitializeComponent();
            this.username = username;
            this._connectionString = connectionString;
            UpdateWelcomeMessage(username);
            PopulateClassesGrid();
        }

        private void UpdateWelcomeMessage(string username)
        {
            lblWelcome.Text = "Welcome, " + username;
        }

        private void PopulateClassesGrid()
        {
            DataTable classesTable = GetClassesData();
            dgvClasses.DataSource = classesTable;
        }

        private DataTable GetClassesData()
        {
            DataTable classesTable = new DataTable();
            string query = "SELECT ClassName FROM Classes WHERE UserID = (SELECT UserID FROM Users WHERE Username = @Username)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    classesTable.Load(reader);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return classesTable;
        }

        private int GetCurrentUserID(string username)
        {
            int userID = 0;
            string query = "SELECT UserID FROM Users WHERE Username = @Username";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);

                connection.Open();
                object result = command.ExecuteScalar();

                if (result != null)
                {
                    userID = Convert.ToInt32(result);
                }
            }

            return userID;
        }

        private void BtnAddClass_Click(object sender, EventArgs e)
        {
            string className = txtClassName.Text;

            if (string.IsNullOrWhiteSpace(className))
            {
                MessageBox.Show("Please enter a class name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                int userID = GetCurrentUserID(username);
                string query = "INSERT INTO Classes (ClassName, UserID) VALUES (@ClassName, @UserID)";

                using (SqlConnection connection = new SqlConnection(_connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClassName", className);
                    command.Parameters.AddWithValue("@UserID", userID);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Class added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        PopulateClassesGrid();
                        txtClassName.Clear();
                    }
                    else
                    {
                        MessageBox.Show("Failed to add class.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRemoveClass_Click(object sender, EventArgs e)
        {
            string className = txtClassName.Text;

            if (string.IsNullOrWhiteSpace(className))
            {
                MessageBox.Show("Please enter a class name to remove.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult result = MessageBox.Show($"Are you sure you want to remove the class '{className}'?", "Remove Class", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    string query = "DELETE FROM Classes WHERE ClassName = @ClassName";

                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ClassName", className);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Class removed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            PopulateClassesGrid();
                            txtClassName.Clear();
                        }
                        else
                        {
                            MessageBox.Show("Class not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnAttendance_Click(object sender, EventArgs e)
        {
            if (dgvClasses.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvClasses.SelectedRows[0];
                string? className = selectedRow.Cells["ClassName"].Value?.ToString();

                if (className != null)
                {
                    try
                    {
                        AttendanceForm attendanceForm = new AttendanceForm(className, _connectionString);
                        attendanceForm.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while opening the serial port: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Class name is null.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a class to take attendance.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNewSemester_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to reset the classes table? This action cannot be undone.", "Reset Classes Table", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                ResetClassesTable();
            }
        }

        private void ResetClassesTable()
        {
            try
            {
                string query = "DELETE FROM Classes";

                using (SqlConnection connection = new SqlConnection(_connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Classes table reset successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        PopulateClassesGrid();
                    }
                    else
                    {
                        MessageBox.Show("No classes found to reset.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAttendance2_Click(object sender, EventArgs e)
        {
            if (dgvClasses.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvClasses.SelectedRows[0];
                string? className = selectedRow.Cells["ClassName"].Value?.ToString();

                if (className != null)
                {
                    try
                    {
                        wifi wifi = new wifi(className,_connectionString);
                        wifi.InitializeNetworkStreamHandler("192.168.1.133", 80);
                        wifi.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while opening the serial port: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Class name is null.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a class to take attendance.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
