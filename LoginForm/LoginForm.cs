using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using Common.Models;
using DatabaseManager;
using Security;
using Utilities;

namespace LoginForm
{
    public partial class LoginForm : Form
    {
        private DatabaseService dbService;

        public LoginForm()
        {
            InitializeComponent();
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            var config = new DatabaseConfig();
            dbService = new DatabaseService(config);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text;

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Please enter both username and password.", "Validation Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Hash the password for security
                string hashedPassword = SecurityManager.HashPassword(password);

                // Validate user credentials
                bool isValidUser = ValidateUserCredentials(username, hashedPassword);

                if (isValidUser)
                {
                    // Save login session
                    SessionManager.SaveSession(username);

                    MessageBox.Show("Login successful!", "Success",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Open main application form
                    OpenMainApplication();
                }
                else
                {
                    MessageBox.Show("Invalid username or password.", "Login Failed",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during login: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateUserCredentials(string username, string hashedPassword)
        {
            try
            {
                string query = @"SELECT COUNT(*) FROM Users 
                               WHERE Username = @Username AND PasswordHash = @PasswordHash 
                               AND IsActive = 1";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Username", username),
                    new SqlParameter("@PasswordHash", hashedPassword)
                };

                var result = dbService.ExecuteScalar<object>(query, parameters);
                int count = result != DBNull.Value ? Convert.ToInt32(result) : 0;
                return count > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database validation error: {ex.Message}");
            }
        }

        private void OpenMainApplication()
        {
            this.Hide();
            var mainForm = new MainForm();
            mainForm.FormClosed += (s, args) => this.Close();
            mainForm.Show();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkForgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Please contact administrator to reset your password.", "Forgot Password",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void linkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Registration feature coming soon.", "Register",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    public class MainForm : Form
    {
        public MainForm()
        {
            this.Text = "Main Application";
            this.Size = new System.Drawing.Size(800, 600);

            var label = new Label()
            {
                Text = "Welcome to Main Application!",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Font = new System.Drawing.Font("Arial", 16)
            };

            this.Controls.Add(label);
        }
    }
}