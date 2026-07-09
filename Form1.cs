using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace UniRegstrationSys
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string connString = "Server=localhost;Database=universityreg;Uid=root;Pwd=;";

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string userInput = txtRegNo.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(userInput) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter your credentials (Email / Reg Number) and password to login.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "";

            if (userInput.Contains("@"))
            {
                query = "SELECT Role, RegistrationNumber FROM Users WHERE Email = @Input AND PasswordHash = @Password;";
            }
            else
            {
                query = "SELECT Role, RegistrationNumber FROM Users WHERE RegistrationNumber = @Input AND PasswordHash = @Password;";
            }

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Input", userInput);
                    cmd.Parameters.AddWithValue("@Password", password);

                    try
                    {
                        conn.Open();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string userRole = reader["Role"].ToString();
                                string studentRegNum = reader["RegistrationNumber"]?.ToString();

                                if (userRole == "Admin")
                                {
                                    MDIParent mdi = new MDIParent();
                                    mdi.FormClosed += (s, args) => this.Close();
                                    mdi.Show();
                                    this.Hide();
                                }
                                else if (userRole == "Student")
                                {
                                    Form5 studentDashboard = new Form5();

                                    studentDashboard.LoggedInStudentReg = studentRegNum;

                                    studentDashboard.FormClosed += (s, args) => this.Close();
                                    studentDashboard.Show();
                                    this.Hide();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Invalid identification credential or password combination.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("Database Connection Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}