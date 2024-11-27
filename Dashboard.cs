using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Dashboard : Form
    {
        private string _username;
        public Dashboard(string username)
        {
            InitializeComponent();
            _username = username;
            lblUsername.Text = $"Welcome, {_username}";
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            lblTitle.Left = (this.ClientSize.Width - lblTitle.Width) / 2;
            lblStoreManager.Left = (this.ClientSize.Width - lblStoreManager.Width) / 2;
            btnManageProducts.Left = (this.ClientSize.Width - btnManageProducts.Width) / 2;
            btnViewLowStock.Left = (this.ClientSize.Width - btnViewLowStock.Width) / 2;
            btnViewReports.Left = (this.ClientSize.Width - btnViewReports.Width) / 2;

            lblDateTime.Text = DateTime.Now.ToString("f");

            lblDateTime.Left = this.ClientSize.Width - lblDateTime.Width - 10;

            // Ensure lblUsername and lblSeparator auto-resize based on content
            lblUsername.AutoSize = true;
            lblSeparator.AutoSize = true;

            // Position lblUsername near the right edge
            lblUsername.Left = this.ClientSize.Width - lblUsername.Width - lblSeparator.Width - linkLogout.Width - 20;

            // Position lblSeparator right next to lblUsername
            lblSeparator.Left = lblUsername.Left + lblUsername.Width;
            lblSeparator.Top = lblUsername.Top; // Align vertically with lblUsername

            // Position linkLogout next to lblSeparator
            linkLogout.Left = lblSeparator.Left + lblSeparator.Width;
            linkLogout.Top = lblUsername.Top; // Align vertically with lblUsername

            LoadOverview();
        }

        private void LoadOverview()
        {
            // Retrieve connection string from app.config
            string connectionString = ConfigurationManager.ConnectionStrings["InventoryManagementDB"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string queryTotalProducts = "SELECT COUNT(*) FROM Products";
                    string queryLowStockProducts = "SELECT COUNT(*) FROM Products WHERE Quantity <= ReorderLevel";

                    SqlCommand cmdTotal = new SqlCommand(queryTotalProducts, conn);
                    SqlCommand cmdLowStock = new SqlCommand(queryLowStockProducts, conn);

                    int totalProducts = (int)cmdTotal.ExecuteScalar();
                    int lowStockProducts = (int)cmdLowStock.ExecuteScalar();

                    lblOverview.Text = $"Total Products: {totalProducts}, Low Stock Products: {lowStockProducts}";

                    // Now center the lblOverview based on the new width
                    lblOverview.Left = (this.ClientSize.Width - lblOverview.Width) / 2;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading overview: " + ex.Message);
            }
        }

        private void btnManageProducts_Click(object sender, EventArgs e)
        {
            txtSupplierEmail productForm = new txtSupplierEmail();
            productForm.Show();
        }

        private void btnViewLowStock_Click(object sender, EventArgs e)
        {
            // Open Low-Stock Alerts Form
            LowStockAlert lowStockForm = new LowStockAlert();
            lowStockForm.Show();
        }

        private void btnViewReports_Click(object sender, EventArgs e)
        {
            // Open Reports Form
            Reports reportsForm = new Reports();
            reportsForm.Show();
        }

        private void lblOverview_Click(object sender, EventArgs e)
        {

        }

        private void linkLogout_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Redirect to the Main form upon logout
            Main mainForm = new Main();
            mainForm.Show();
            this.Close(); // Close the current form
        }

        private void btnSalesEntry_Click(object sender, EventArgs e)
        {
            Sales salesForm = new Sales();
            salesForm.Show();
        }
    }
}
