using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class ChangePassword : Form
    {
        public ChangePassword()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string newPassword = txtNewPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();

            // Validate input fields
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Please enter your username.", "Validation Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(newPassword))
            {
                MessageBox.Show("New password cannot be empty.", "Validation Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Please confirm your new password.", "Validation Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("The new password and confirmation password do not match. Please try again.", "Password Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Hash the new password
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

            string connectionString = ConfigurationManager.ConnectionStrings["InventoryManagementDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    // Check if the username exists
                    string queryCheck = "SELECT COUNT(*) FROM Users WHERE Username = @username";
                    SqlCommand cmdCheck = new SqlCommand(queryCheck, conn);
                    cmdCheck.Parameters.AddWithValue("@username", username);

                    conn.Open();
                    int userExists = (int)cmdCheck.ExecuteScalar();

                    if (userExists == 0)
                    {
                        MessageBox.Show("The username does not exist. Please check and try again.", "User Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Update the password
                    string queryUpdate = "UPDATE Users SET Password = @password WHERE Username = @username";
                    SqlCommand cmdUpdate = new SqlCommand(queryUpdate, conn);
                    cmdUpdate.Parameters.AddWithValue("@password", hashedPassword);
                    cmdUpdate.Parameters.AddWithValue("@username", username);

                    cmdUpdate.ExecuteNonQuery();
                    MessageBox.Show("Your password has been successfully updated!", "Password Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Redirect to the Main form
                    Main mainForm = new Main();
                    mainForm.Show();
                    this.Close(); // Close the ChangePassword form
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while updating your password. Please try again.\n\nDetails: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ChangePassword_Load(object sender, EventArgs e)
        {

        }
    }
}
