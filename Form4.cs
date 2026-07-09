using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace UniRegstrationSys
{
    public partial class StudentForm : Form
    {
        public StudentForm()
        {
            InitializeComponent();
        }

        string connString = "Server=localhost;Database=universityreg;Uid=root;Pwd=;";

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void StudentForm_Load(object sender, EventArgs e)
        {
            RefreshStudentGrid();
        }

        private void ClearFormFields()
        {
            RegNo.Clear();
            FirstName.Clear();
            LastName.Clear();
            Program.Text = "";
            Year.Clear();
            RegNo.Focus();
        }

        public bool SaveStudent(string regNum, string firstName, string lastName, string program, int year)
        {
            string insertStudentQuery = @"INSERT INTO Students (RegistrationNumber, FirstName, LastName, Program, Year) 
                                         VALUES (@RegNum, @FirstName, @LastName, @Program, @Year);";

            string insertUserQuery = @"INSERT INTO Users (RegistrationNumber, PasswordHash, Role) 
                                      VALUES (@RegNum, @PasswordHash, 'Student');";

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                try
                {
                    conn.Open();

                    using (MySqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            using (MySqlCommand cmdStudent = new MySqlCommand(insertStudentQuery, conn, transaction))
                            {
                                cmdStudent.Parameters.AddWithValue("@RegNum", regNum);
                                cmdStudent.Parameters.AddWithValue("@FirstName", firstName);
                                cmdStudent.Parameters.AddWithValue("@LastName", lastName);
                                cmdStudent.Parameters.AddWithValue("@Program", program);
                                cmdStudent.Parameters.AddWithValue("@Year", year);

                                cmdStudent.ExecuteNonQuery();
                            }

                            using (MySqlCommand cmdUser = new MySqlCommand(insertUserQuery, conn, transaction))
                            {
                                cmdUser.Parameters.AddWithValue("@RegNum", regNum);
                                cmdUser.Parameters.AddWithValue("@PasswordHash", regNum);

                                cmdUser.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            return true;
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1062)
                    {
                        MessageBox.Show("This Registration Number already exists in the system.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show("Database Transaction Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return false;
                }
            }
        }

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
                MessageBox.Show($"Student registered and user login account created successfully!\n\nDefault Password is: {regNum}",
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFormFields();
                RefreshStudentGrid();
            }
        }

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

        private void button4_Click(object sender, EventArgs e)
        {
            ClearFormFields();
        }
    }
}