namespace Fingerprint_attendance
{
    partial class wifi
    {




        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(wifi));
            dgvStudents = new DataGridView();
            statusColumn = new DataGridViewTextBoxColumn();
            btnAddStudent = new Button();
            btnStartAttendance = new Button();
            btnFinishAttendance = new Button();
            txtStudentName = new TextBox();
            txtStudentNumber = new TextBox();
            lblStudentName = new Label();
            lblStudentNumber = new Label();
            btnEnrollFingerprint = new Button();
            lblEnrollStatus = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvStudents).BeginInit();
            SuspendLayout();
            // 
            // dgvStudents
            // 
            dgvStudents.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvStudents.ColumnHeadersHeight = 29;
            dgvStudents.Columns.AddRange(new DataGridViewColumn[] { statusColumn });
            dgvStudents.Location = new Point(20, 20);
            dgvStudents.Name = "dgvStudents";
            dgvStudents.RowHeadersWidth = 51;
            dgvStudents.Size = new Size(681, 201);
            dgvStudents.TabIndex = 0;
            // 
            // statusColumn
            // 
            statusColumn.FillWeight = 20F;
            statusColumn.HeaderText = "Status";
            statusColumn.MinimumWidth = 6;
            statusColumn.Name = "statusColumn";
            // 
            // btnAddStudent
            // 
            btnAddStudent.Location = new Point(20, 240);
            btnAddStudent.Name = "btnAddStudent";
            btnAddStudent.Size = new Size(150, 30);
            btnAddStudent.TabIndex = 1;
            btnAddStudent.Text = "Add Student";
            btnAddStudent.Click += btnAddStudent_Click;
            // 
            // btnStartAttendance
            // 
            btnStartAttendance.Location = new Point(200, 240);
            btnStartAttendance.Name = "btnStartAttendance";
            btnStartAttendance.Size = new Size(200, 30);
            btnStartAttendance.TabIndex = 2;
            btnStartAttendance.Text = "Start Attendance Mode";
            btnStartAttendance.Click += btnStartAttendance_Click;
            // 
            // btnFinishAttendance
            // 
            btnFinishAttendance.Location = new Point(420, 240);
            btnFinishAttendance.Name = "btnFinishAttendance";
            btnFinishAttendance.Size = new Size(150, 30);
            btnFinishAttendance.TabIndex = 3;
            btnFinishAttendance.Text = "Finish Attendance";
            btnFinishAttendance.Click += btnFinishAttendance_Click_1;
            // 
            // txtStudentName
            // 
            txtStudentName.Location = new Point(155, 347);
            txtStudentName.Name = "txtStudentName";
            txtStudentName.Size = new Size(178, 27);
            txtStudentName.TabIndex = 7;
            // 
            // txtStudentNumber
            // 
            txtStudentNumber.Location = new Point(155, 297);
            txtStudentNumber.Name = "txtStudentNumber";
            txtStudentNumber.Size = new Size(178, 27);
            txtStudentNumber.TabIndex = 6;
            // 
            // lblStudentName
            // 
            lblStudentName.AutoSize = true;
            lblStudentName.Location = new Point(42, 350);
            lblStudentName.Name = "lblStudentName";
            lblStudentName.Size = new Size(107, 20);
            lblStudentName.TabIndex = 5;
            lblStudentName.Text = "Student Name:";
            // 
            // lblStudentNumber
            // 
            lblStudentNumber.AutoSize = true;
            lblStudentNumber.Location = new Point(28, 300);
            lblStudentNumber.Name = "lblStudentNumber";
            lblStudentNumber.Size = new Size(121, 20);
            lblStudentNumber.TabIndex = 4;
            lblStudentNumber.Text = "Student Number:";
            // 
            // btnEnrollFingerprint
            // 
            btnEnrollFingerprint.Location = new Point(363, 297);
            btnEnrollFingerprint.Name = "btnEnrollFingerprint";
            btnEnrollFingerprint.Size = new Size(178, 30);
            btnEnrollFingerprint.TabIndex = 8;
            btnEnrollFingerprint.Text = "Enroll Fingerprint";
            btnEnrollFingerprint.UseVisualStyleBackColor = true;
            btnEnrollFingerprint.Click += btnEnrollFingerprint_Click;
            // 
            // lblEnrollStatus
            // 
            lblEnrollStatus.AutoSize = true;
            lblEnrollStatus.Location = new Point(363, 354);
            lblEnrollStatus.Name = "lblEnrollStatus";
            lblEnrollStatus.Size = new Size(181, 20);
            lblEnrollStatus.TabIndex = 9;
            lblEnrollStatus.Text = "Enrollment Status: Default";
            // 
            // wifi
            // 
            ClientSize = new Size(713, 408);
            Controls.Add(lblEnrollStatus);
            Controls.Add(btnEnrollFingerprint);
            Controls.Add(txtStudentName);
            Controls.Add(txtStudentNumber);
            Controls.Add(lblStudentName);
            Controls.Add(lblStudentNumber);
            Controls.Add(dgvStudents);
            Controls.Add(btnAddStudent);
            Controls.Add(btnStartAttendance);
            Controls.Add(btnFinishAttendance);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "wifi";
            Text = "Attendance Form";
            ((System.ComponentModel.ISupportInitialize)dgvStudents).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private DataGridView dgvStudents;
        private Button btnAddStudent;
        private Button btnStartAttendance;
        private Button btnFinishAttendance;
        private TextBox txtStudentName;
        private TextBox txtStudentNumber;
        private Label lblStudentName;
        private Label lblStudentNumber;
        private Button btnEnrollFingerprint;
        private DataGridViewTextBoxColumn statusColumn;
        private Label lblEnrollStatus;
    }
}