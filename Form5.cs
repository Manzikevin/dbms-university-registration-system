using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace UniRegstrationSys
{
    public partial class Form5 : Form
    {
        public string LoggedInStudentReg { get; set; }

        public Form5()
        {
            InitializeComponent();
        }

        string connString = "Server=localhost;Database=universityreg;Uid=root;Pwd=;";

        private void Form5_Load(object sender, EventArgs e)
        {
            ClearDashboardLabels();

            if (!string.IsNullOrEmpty(LoggedInStudentReg))
            {
                StudentRegNoInput.Visible = false;
                button1.Visible = false;
                label1.Visible = false;

                FetchStudentSummary(LoggedInStudentReg);
            }
            else
            {
                label2.Visible = false;
                logoutbtn.Visible = false;
                ClearDashboardLabels();
            }
        }

        private void ClearDashboardLabels()
        {
            lblStudentName.Text = "Name: --";
            lblReg.Text = "Reg No: --";
            lblProgram.Text = "Program: --";
            lblYear.Text = "Year: --";
            lblFaculty.Text = "Faculty: --";
            lblDepartment.Text = "Department: --";

            lblTotalCourses.Text = "0";
        }

        private void FetchStudentSummary(string regNum)
        {

            string query = @"
                SELECT 
                    s.FirstName, 
                    s.LastName, 
                    s.RegistrationNumber, 
                    s.Program, 
                    s.Year, 
                    s.Faculty,
                    s.Department,
                    COUNT(r.RegistrationID) AS TotalCourses
                FROM Students s
                LEFT JOIN Registrations r ON s.StudentID = r.StudentID
                WHERE s.RegistrationNumber = @RegNum
                GROUP BY s.StudentID, s.FirstName, s.LastName, s.RegistrationNumber, s.Program, s.Year, s.Faculty, s.Department;";

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RegNum", regNum);

                    try
                    {
                        conn.Open();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string firstName = reader["FirstName"].ToString();
                                string lastName = reader["LastName"].ToString();
                                string actualRegNum = reader["RegistrationNumber"].ToString();
                                string program = reader["Program"].ToString();
                                string year = reader["Year"].ToString();

                                string faculty = reader["Faculty"].ToString();
                                string department = reader["Department"].ToString();

                                string totalCourses = reader["TotalCourses"].ToString();

                                lblStudentName.Text = $"Name: {firstName} {lastName}";
                                lblReg.Text = $"Reg No: {actualRegNum}";
                                lblProgram.Text = $"Program: {program}";
                                lblYear.Text = $"Year: {year}";

                                lblFaculty.Text = $"Faculty: {faculty}";
                                lblDepartment.Text = $"Department: {department}";

                                lblTotalCourses.Text = $" Total Courses Enrolled : {totalCourses}";
                            }
                            else
                            {
                                MessageBox.Show("No student found with that Registration Number.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                ClearDashboardLabels();
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string regNum = StudentRegNoInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(regNum))
            {
                MessageBox.Show("Please type a valid Student Registration Number to search.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FetchStudentSummary(regNum);
        }

        private void logoutbtn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to Logout?", "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                Form1 loginForm = new Form1();
                loginForm.Show();
                this.Hide();
            }
        }
    }
}