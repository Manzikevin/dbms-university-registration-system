using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Mysqlx.Datatypes.Scalar.Types;

namespace UniRegstrationSys
{
    public partial class Form3 : Form
    {
        // Initializes the form and its designer-generated visual components.
        public Form3()
        {
            InitializeComponent();
        }

        string connString = "Server=localhost;Database=universityreg;Uid=root;Pwd=;";

        // Clears all entry fields on the form and refocuses the cursor onto the course code field.
        private void ClearFormFields()
        {
            CourseCode.Clear();
            CourseName.Clear();
            Credits.Clear();
            CourseCode.Focus();
        }

        // Queries the database to retrieve all registered course columns into a structured DataTable object.
        public DataTable GetCourseList()
        {
            string query = "SELECT CourseID, CourseCode, CourseName, Credits FROM Courses";
            DataTable dataTable = new DataTable();

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("Error loading data: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            return dataTable;
        }

        // Binds the updated database list to the DataGridView control and configures clean user column names.
        private void RefreshCourseGrid()
        {
            DataTable dt = GetCourseList();
            dgvCourses.DataSource = dt;

            dgvCourses.Columns["CourseID"].HeaderText = "ID";
            dgvCourses.Columns["CourseCode"].HeaderText = "Course Code";
            dgvCourses.Columns["CourseName"].HeaderText = "Course Name";
            dgvCourses.Columns["Credits"].HeaderText = "Credits";
        }

        // Triggers initial data loading to populate the grid as soon as the Form3 runtime window opens.

        private void Form3_Load_1(object sender, EventArgs e)
        {
            RefreshCourseGrid();
        }

        // Executes an INSERT command using safe parameters to save a new course record to MySQL.
        public bool SaveCourse(string courseCode, string courseName, int credits)
        {
            string query = @"INSERT INTO Courses (CourseCode, CourseName, Credits) 
                             VALUES (@CourseCode, @CourseName, @Credits);";

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CourseCode", courseCode);
                    cmd.Parameters.AddWithValue("@CourseName", courseName);
                    cmd.Parameters.AddWithValue("@Credits", credits);

                    try
                    {
                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                    catch (MySqlException ex)
                    {
                        if (ex.Number == 1062)
                        {
                            MessageBox.Show("This Course Code already exists.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        return false;
                    }
                }
            }
        }

        // Captures selected grid row field values and maps data back to the entry form textboxes when a cell is clicked.
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvCourses.Rows[e.RowIndex];

                CourseCode.Text = row.Cells["CourseCode"].Value?.ToString();
                CourseName.Text = row.Cells["CourseName"].Value?.ToString();
                Credits.Text = row.Cells["Credits"].Value?.ToString();
            }
        }

        // Validates form input data, extracts text, and attempts to save a new course on button click.
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CourseCode.Text) || string.IsNullOrWhiteSpace(CourseName.Text))
            {
                MessageBox.Show("Please fill in all required fields (Course Code and Course Name).",
                                "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(Credits.Text, out int courseCredits))
            {
                MessageBox.Show("Please enter a valid numeric integer value for Credits.",
                                "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string code = CourseCode.Text.Trim();
            string name = CourseName.Text.Trim();

            if (SaveCourse(code, name, courseCredits))
            {
                MessageBox.Show("Course registered successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFormFields();
                RefreshCourseGrid();
            }
        }

        // Executes an UPDATE database transaction to modify existing course attributes matching the given code.
        public bool UpdateCourse(string courseCode, string courseName, int credits)
        {
            string query = @"UPDATE Courses 
                             SET CourseName = @CourseName, Credits = @Credits 
                             WHERE CourseCode = @CourseCode;";

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CourseCode", courseCode);
                    cmd.Parameters.AddWithValue("@CourseName", courseName);
                    cmd.Parameters.AddWithValue("@Credits", credits);

                    try
                    {
                        conn.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
        }

        // Validates updated textbox strings, parses numbers, and edits the target course record on button click.
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CourseCode.Text))
            {
                MessageBox.Show("Please select a course from the grid to update.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(Credits.Text, out int courseCredits))
            {
                MessageBox.Show("Please enter a valid numeric integer value for Credits.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string code = CourseCode.Text.Trim();
            string name = CourseName.Text.Trim();

            if (UpdateCourse(code, name, courseCredits))
            {
                MessageBox.Show("Course updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFormFields();
                RefreshCourseGrid();
            }
        }

        // Executes a DELETE database transaction targeting a single course record based on its unique code string.
        public bool DeleteCourse(string courseCode)
        {
            string query = "DELETE FROM Courses WHERE CourseCode = @CourseCode;";

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CourseCode", courseCode);

                    try
                    {
                        conn.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
        }
        // Verifies context via user confirmation prompt prior to executing the course elimination transaction.
        private void button3_Click(object sender, EventArgs e)
        {
            string code = CourseCode.Text.Trim();

            if (string.IsNullOrWhiteSpace(code))
            {
                MessageBox.Show("Please select a course from the grid to delete.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult dialogResult = MessageBox.Show($"Are you sure you want to delete course {code}?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                if (DeleteCourse(code))
                {
                    MessageBox.Show("Course deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFormFields();
                    RefreshCourseGrid();
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ClearFormFields() ;
        }
    }
}