using MySql.Data.MySqlClient; // Fixed: Added mandatory dependency reference
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UniRegstrationSys
{
    public partial class Form5 : Form
    {
        // Initializes the summary dashboard form and its designer visual components.
        public Form5()
        {
            InitializeComponent();
        }

        string connString = "Server=localhost;Database=universityreg;Uid=root;Pwd=;";

        // Triggers initial setup routines when the window window loads.
        private void Form5_Load(object sender, EventArgs e)
        {
            ClearDashboardLabels();
        }

        // Resets all display labels back to their default placeholder text states.
        private void ClearDashboardLabels()
        {
            lblStudentName.Text = "Name: --";
            lblReg.Text = "Reg No: --";
            lblProgram.Text = "Program: --"; // Updated placeholder for program field
            lblYear.Text = "Year: --";       // Updated placeholder for year field
            lblTotalCourses.Text = "0";
        }

        // Queries the database using a JOIN aggregation to pull student details and count matching registration rows.
        private void FetchStudentSummary(string regNum)
        {
            // Fixed: Standardized query projection to group only by structural columns existing in your schema
            string query = @"
                SELECT 
                    s.FirstName, 
                    s.LastName, 
                    s.RegistrationNumber, 
                    s.Program, 
                    s.Year, 
                    COUNT(r.RegistrationID) AS TotalCourses
                FROM Students s
                LEFT JOIN Registrations r ON s.StudentID = r.StudentID
                WHERE s.RegistrationNumber = @RegNum
                GROUP BY s.StudentID, s.FirstName, s.LastName, s.RegistrationNumber, s.Program, s.Year;";

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
                                string totalCourses = reader["TotalCourses"].ToString();

                                // Fixed: Successfully mapped real table strings to corresponding label controls
                                lblStudentName.Text = $"Name: {firstName} {lastName}";
                                lblReg.Text = $"Reg No: {actualRegNum}";
                                lblProgram.Text = $"Program: {program}";
                                lblYear.Text = $"Year: {year}";
                                lblTotalCourses.Text = $" Total Courses Enrolled : { totalCourses}"
                                ;
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



        // Validates search input text strings and triggers the profile summary loading query on button click.
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
    }
}