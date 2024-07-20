using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace Fingerprint_attendance
{
    public partial class LoginForm : Form
    {
        private string _connectionString;

        public LoginForm(string connectionString)
        {
            InitializeComponent();
            _connectionString = connectionString;
            SetDataDirectory();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (AuthenticateUser(username, password))
            {
                MessageBox.Show("Login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DashboardForm dashboard = new DashboardForm(username, _connectionString);
                dashboard.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Invalid username or password. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void lnkRegister_Click(object sender, EventArgs e)
        {
            // Open the registration form and pass the reference to the login form and connection string
            RegistrationForm registrationForm = new RegistrationForm(this, _connectionString);
            registrationForm.ShowDialog();
        }

        private bool AuthenticateUser(string username, string password)
        {
            bool isAuthenticated = false;
            string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND Password = @Password";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", password);

                try
                {
                    connection.Open();
                    int count = (int)command.ExecuteScalar();
                    isAuthenticated = count > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return isAuthenticated;
        }

        private void SetDataDirectory()
        {
            string dataDirectory = AppDomain.CurrentDomain.GetData("DataDirectory") as string;
            if (string.IsNullOrEmpty(dataDirectory))
            {
                dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
                AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectory);
            }
        }
    }
}
