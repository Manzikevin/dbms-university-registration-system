using Google.Protobuf.WellKnownTypes;
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

namespace UniRegstrationSys
{
    public partial class StudentForm : Form
    {
        // Initializes the form and its designer-generated visual components.
        public StudentForm()
        {
            InitializeComponent();
        }

        string connString = "Server=localhost;Database=universityreg;Uid=root;Pwd=;";

        // Placeholder for filtering student records when the text in the search box changes.
        private void SearchBox_TextChanged(object sender, EventArgs e)
        {

        }

        // Placeholder for executing logic when label2 is clicked.
        private void label2_Click(object sender, EventArgs e)
        {

        }

        // Placeholder for executing logic when the text inside textBox2 is modified.
        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        // Triggers initial data loading to populate the grid as soon as the form opens.
        private void StudentForm_Load(object sender, EventArgs e)
        {
            RefreshStudentGrid();
        }

        // Clears all entry fields on the form and refocuses the cursor onto the registration field.
        private void ClearFormFields()
        {
            RegNo.Clear();
            FirstName.Clear();
            LastName.Clear();
            Program.Text = "";
            Year.Clear();
            RegNo.Focus();
        }

        // Executes an INSERT command using safe parameters to save a new student record to MySQL.
        public bool SaveStudent(string regNum, string firstName, string lastName, string program, int year)
        {
            string query = @"INSERT INTO Students (RegistrationNumber, FirstName, LastName, Program, Year) 
                             VALUES (@RegNum, @FirstName, @LastName, @Program, @Year);";

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RegNum", regNum);
                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.Parameters.AddWithValue("@LastName", lastName);
                    cmd.Parameters.AddWithValue("@Program", program);
                    cmd.Parameters.AddWithValue("@Year", year);

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
                            MessageBox.Show("This Registration Number already exists.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        // Queries the database to retrieve all registered student columns into a structured DataTable object.
        public DataTable GetStudentList()
        {
            string query = "SELECT StudentID, RegistrationNumber, FirstName, LastName, Program, Year FROM Students";
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
        private void RefreshStudentGrid()
        {
            DataTable dt = GetStudentList();
            dgvStudents.DataSource = dt;

            dgvStudents.Columns["StudentID"].HeaderText = "ID";
            dgvStudents.Columns["RegistrationNumber"].HeaderText = "Reg Number";
            dgvStudents.Columns["FirstName"].HeaderText = "First Name";
            dgvStudents.Columns["LastName"].HeaderText = "Last Name";
            dgvStudents.Columns["Program"].HeaderText = "Program";
            dgvStudents.Columns["Year"].HeaderText = "Year";
        }

        // Validates form input data, extracts text, and attempts to save a new student on button click.
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(RegNo.Text) ||
                string.IsNullOrWhiteSpace(FirstName.Text) ||
                string.IsNullOrWhiteSpace(LastName.Text))
            {
                MessageBox.Show("Please fill in all required fields (Reg Number, First Name, and Last Name).",
                                "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(Year.Text, out int schoolYear))
            {
                MessageBox.Show("Please enter a valid numeric value for the Year.",
                                "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string regNum = RegNo.Text.Trim();
            string firstName = FirstName.Text.Trim();
            string lastName = LastName.Text.Trim();
            string program = Program.Text.Trim();

            bool isSuccess = SaveStudent(regNum, firstName, lastName, program, schoolYear);

            if (isSuccess)
            {
                MessageBox.Show("Student registered successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFormFields();
                RefreshStudentGrid();
            }
        }

        // Captures selected grid row field values and mapping data back to the entry form when a cell is clicked.
        private void dgvStudents_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvStudents.Rows[e.RowIndex];

                RegNo.Text = row.Cells["RegistrationNumber"].Value?.ToString();
                FirstName.Text = row.Cells["FirstName"].Value?.ToString();
                LastName.Text = row.Cells["LastName"].Value?.ToString();
                Program.Text = row.Cells["Program"].Value?.ToString();
                Year.Text = row.Cells["Year"].Value?.ToString();
            }
        }

        // Executes an UPDATE database transaction to modify existing row values matching the unique registration number.
        public bool UpdateStudent(string regNum, string firstName, string lastName, string program, int year)
        {
            string query = @"UPDATE Students 
                             SET FirstName = @FirstName, LastName = @LastName, Program = @Program, Year = @Year 
                             WHERE RegistrationNumber = @RegNum;";

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RegNum", regNum);
                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.Parameters.AddWithValue("@LastName", lastName);
                    cmd.Parameters.AddWithValue("@Program", program);
                    cmd.Parameters.AddWithValue("@Year", year);

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

        // Validates selected values, parses numeric data, and processes edits to a student row on button click.
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(RegNo.Text))
            {
                MessageBox.Show("Please select a student from the grid to update.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(Year.Text, out int schoolYear))
            {
                MessageBox.Show("Please enter a valid numeric value for the Year.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string regNum = RegNo.Text.Trim();
            string firstName = FirstName.Text.Trim();
            string lastName = LastName.Text.Trim();
            string program = Program.Text.Trim();

            if (UpdateStudent(regNum, firstName, lastName, program, schoolYear))
            {
                MessageBox.Show("Student updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFormFields();
                RefreshStudentGrid();
            }
        }

        // Executes a DELETE database transaction targeting a single entry using its registration parameter.
        public bool DeleteStudent(string regNum)
        {
            string query = "DELETE FROM Students WHERE RegistrationNumber = @RegNum;";

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RegNum", regNum);

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

        // Confirms intent with a dialog box before processing deletion operations against the designated student row.
        private void button3_Click(object sender, EventArgs e)
        {
            string regNum = RegNo.Text.Trim();

            if (string.IsNullOrWhiteSpace(regNum))
            {
                MessageBox.Show("Please select a student from the grid to delete.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult dialogResult = MessageBox.Show($"Are you sure you want to delete student with Reg No: {regNum}?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                if (DeleteStudent(regNum))
                {
                    MessageBox.Show("Student deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFormFields();
                    RefreshStudentGrid();
                }
            }
        }

        // Resets all input controls back to their default empty states on clear button click.
        private void button4_Click(object sender, EventArgs e)
        {
            ClearFormFields();
        }
    }
}