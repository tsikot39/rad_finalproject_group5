using System;
using System.Data.SqlClient;  // Required for SQL interaction
using System.Configuration;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        // Form Load event to center labels and button
        private void Form1_Load(object sender, EventArgs e)
        {
            lblTitle.Left = (this.ClientSize.Width - lblTitle.Width) / 2;
            lblUserLogin.Left = (this.ClientSize.Width - lblUserLogin.Width) / 2;
            btnLogin.Left = (this.ClientSize.Width - btnLogin.Width) / 2;
            linkChangePassword.Left = (this.ClientSize.Width - linkChangePassword.Width) / 2;
        }

        // Login Button Click event for validating user login
        private void btnLogin_Click_1(object sender, EventArgs e)
        {
            // Get the connection string from app.config
            string connectionString = ConfigurationManager.ConnectionStrings["InventoryManagementDB"].ConnectionString;

            // Get the input from the text boxes
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // Check if username or password is empty
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please fill in both Username and Password fields.", "Validation Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;  // Stop further processing
            }

            // SQL query to fetch the hashed password for the given username
            string query = "SELECT Password FROM Users WHERE Username=@username AND Role='Store Manager'";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows) // If the username exists
                    {
                        reader.Read();
                        string hashedPassword = reader["Password"].ToString(); // Get the hashed password from the database

                        // Verify the user-provided password against the hashed password
                        if (BCrypt.Net.BCrypt.Verify(password, hashedPassword))
                        {
                            MessageBox.Show("Welcome back! Login successful.", "Login Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Open Store Manager Dashboard (redirect)
                            Dashboard dashboard = new Dashboard(username); // Assuming DashboardForm is the name of your form
                            dashboard.Show();

                            this.Hide(); // Hide the login form
                        }
                        else
                        {
                            MessageBox.Show("The password you entered is incorrect. Please try again.", "Invalid Credentials", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show("The username you entered does not exist. Please check your credentials and try again.", "User Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An unexpected error occurred while processing your login: {ex.Message}", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void linkChangePassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Open the Change Password form
            ChangePassword changePasswordForm = new ChangePassword();
            changePasswordForm.Show();
            this.Hide();  // Optionally hide the login form
        }
    }
}
