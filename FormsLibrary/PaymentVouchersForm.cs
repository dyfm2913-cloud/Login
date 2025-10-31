using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using Common.Models;
using DatabaseManager;
using Services;
using Utilities;

namespace FormsLibrary
{
    public partial class PaymentVouchersForm : Form
    {
        private SpendingService spendingService;
        private DatabaseService databaseService;
        private User currentUser;

        public PaymentVouchersForm()
        {
            InitializeComponent();
            InitializeServices();
        }

        private void InitializeServices()
        {
            var config = new DatabaseConfig();
            databaseService = new DatabaseService(config);
            spendingService = new SpendingService(databaseService);
        }

        private void PaymentVouchersForm_Load(object sender, EventArgs e)
        {
            try
            {
                LoadSpendings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSpendings()
        {
            try
            {
                var spendings = spendingService.GetAllSpendings();
                dataGridViewSpendings.DataSource = spendings;
            }
            catch (Exception ex)
            {
                Logger.LogError("Error loading spendings", ex);
                MessageBox.Show($"Error loading spendings: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddSpending_Click(object sender, EventArgs e)
        {
            try
            {
                // Add spending logic here
                var spending = new Spending
                {
                    UserId = currentUser.Id,
                    Amount = decimal.Parse(txtAmount.Text),
                    Description = txtDescription.Text,
                    Category = cmbCategory.Text,
                    SpendingDate = DateTime.Now
                };

                bool success = spendingService.AddSpending(spending);
                if (success)
                {
                    MessageBox.Show("Spending added successfully!", "Success",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadSpendings();
                }
                else
                {
                    MessageBox.Show("Failed to add spending.", "Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding spending: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Add other event handlers and methods as needed
    }
}