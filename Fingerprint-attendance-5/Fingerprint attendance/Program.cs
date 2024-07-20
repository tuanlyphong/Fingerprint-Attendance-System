using System;
using System.Windows.Forms;
using OfficeOpenXml;

namespace Fingerprint_attendance
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Set the EPPlus LicenseContext property
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Set the connection string here
            string connectionString = "Data Source=NTRLORD; Initial Catalog=FingerprintAttendanceDB;Integrated Security=True;";


            Application.Run(new LoginForm(connectionString)); // Pass the connection string to your initial form
        }
    }
}
