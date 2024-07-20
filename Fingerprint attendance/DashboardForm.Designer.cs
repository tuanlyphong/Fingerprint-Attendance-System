using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Fingerprint_attendance
{
    partial class DashboardForm : Form
    {
        private Label lblWelcome;
        private Button btnAddClass;
        private Button btnRemoveClass;
        private Button btnAttendance;
        private DataGridView dgvClasses;

        Dictionary<string, string> attendanceStatus = new Dictionary<string, string>();

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DashboardForm));
            lblWelcome = new Label();
            btnAddClass = new Button();
            btnRemoveClass = new Button();
            btnAttendance = new Button();
            dgvClasses = new DataGridView();
            btnNewSemester = new Button();
            txtClassName = new TextBox();
            btnAttendance2 = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvClasses).BeginInit();
            SuspendLayout();
            // 
            // lblWelcome
            // 
            lblWelcome.AutoSize = true;
            lblWelcome.Location = new Point(10, 10);
            lblWelcome.Name = "lblWelcome";
            lblWelcome.Size = new Size(78, 20);
            lblWelcome.TabIndex = 0;
            lblWelcome.Text = "Welcome, ";
            // 
            // btnAddClass
            // 
            btnAddClass.Location = new Point(10, 40);
            btnAddClass.Name = "btnAddClass";
            btnAddClass.Size = new Size(143, 34);
            btnAddClass.TabIndex = 1;
            btnAddClass.Text = "Add Class";
            btnAddClass.Click += BtnAddClass_Click;
            // 
            // btnRemoveClass
            // 
            btnRemoveClass.Location = new Point(290, 40);
            btnRemoveClass.Name = "btnRemoveClass";
            btnRemoveClass.Size = new Size(126, 34);
            btnRemoveClass.TabIndex = 2;
            btnRemoveClass.Text = "Remove Class";
            btnRemoveClass.Click += BtnRemoveClass_Click;
            // 
            // btnAttendance
            // 
            btnAttendance.BackColor = Color.Orange;
            btnAttendance.Location = new Point(566, 3);
            btnAttendance.Name = "btnAttendance";
            btnAttendance.Size = new Size(204, 31);
            btnAttendance.TabIndex = 3;
            btnAttendance.Text = "Take Attendance (Direct)";
            btnAttendance.UseVisualStyleBackColor = false;
            btnAttendance.Click += BtnAttendance_Click;
            // 
            // dgvClasses
            // 
            dgvClasses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvClasses.ColumnHeadersHeight = 29;
            dgvClasses.Location = new Point(10, 80);
            dgvClasses.Name = "dgvClasses";
            dgvClasses.RowHeadersWidth = 51;
            dgvClasses.Size = new Size(760, 300);
            dgvClasses.TabIndex = 4;
            // 
            // btnNewSemester
            // 
            btnNewSemester.BackColor = Color.Red;
            btnNewSemester.Location = new Point(429, 3);
            btnNewSemester.Name = "btnNewSemester";
            btnNewSemester.Size = new Size(119, 31);
            btnNewSemester.TabIndex = 5;
            btnNewSemester.Text = "New Semester";
            btnNewSemester.UseVisualStyleBackColor = false;
            btnNewSemester.Click += btnNewSemester_Click;
            // 
            // txtClassName
            // 
            txtClassName.Location = new Point(159, 44);
            txtClassName.Name = "txtClassName";
            txtClassName.Size = new Size(125, 27);
            txtClassName.TabIndex = 6;
            // 
            // btnAttendance2
            // 
            btnAttendance2.BackColor = Color.Chartreuse;
            btnAttendance2.Location = new Point(566, 40);
            btnAttendance2.Name = "btnAttendance2";
            btnAttendance2.Size = new Size(204, 31);
            btnAttendance2.TabIndex = 7;
            btnAttendance2.Text = "Take Attendance (Wifi)";
            btnAttendance2.UseVisualStyleBackColor = false;
            btnAttendance2.Click += btnAttendance2_Click;
            // 
            // DashboardForm
            // 
            ClientSize = new Size(782, 453);
            Controls.Add(btnAttendance2);
            Controls.Add(txtClassName);
            Controls.Add(btnNewSemester);
            Controls.Add(lblWelcome);
            Controls.Add(btnAddClass);
            Controls.Add(btnRemoveClass);
            Controls.Add(btnAttendance);
            Controls.Add(dgvClasses);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "DashboardForm";
            Text = "Dashboard";
            ((System.ComponentModel.ISupportInitialize)dgvClasses).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private Button btnNewSemester;
        private TextBox txtClassName;
        private Button btnAttendance2;
    }
}