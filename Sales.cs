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
    public partial class Sales : Form
    {
        public Sales()
        {
            InitializeComponent();
            LoadProducts();
        }

        private void LoadProducts()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["InventoryManagementDB"].ConnectionString;
            string query = "SELECT ProductID, ProductName FROM Products ORDER BY ProductName ASC";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                cmbProduct.DataSource = dt;
                cmbProduct.DisplayMember = "ProductName";
                cmbProduct.ValueMember = "ProductID";
                cmbProduct.SelectedIndex = -1; // No default selection
            }
        }

        private void Sales_Load(object sender, EventArgs e)
        {
            lblTitle.Left = (this.ClientSize.Width - lblTitle.Width) / 2;
            lblSalesEntry.Left = (this.ClientSize.Width - lblSalesEntry.Width) / 2;

           cmbProduct.DropDownStyle = ComboBoxStyle.DropDownList;

            txtQuantitySold.KeyPress += txtQuantitySold_KeyPress;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbProduct.SelectedIndex == -1 || string.IsNullOrWhiteSpace(txtQuantitySold.Text))
            {
                MessageBox.Show("Please select a product and enter a quantity.");
                return;
            }

            int productId = Convert.ToInt32(cmbProduct.SelectedValue);
            int quantitySold;
            if (!int.TryParse(txtQuantitySold.Text, out quantitySold) || quantitySold <= 0)
            {
                MessageBox.Show("Please enter a valid numeric quantity.");
                return;
            }

            DateTime saleDate = dtpSaleDate.Value;
            string connectionString = ConfigurationManager.ConnectionStrings["InventoryManagementDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Check available stock
                    string stockQuery = "SELECT Quantity FROM Products WHERE ProductID = @ProductID";
                    SqlCommand stockCmd = new SqlCommand(stockQuery, conn);
                    stockCmd.Parameters.AddWithValue("@ProductID", productId);

                    int availableStock = (int)stockCmd.ExecuteScalar();

                    if (quantitySold > availableStock)
                    {
                        MessageBox.Show("Insufficient stock available. Please enter a quantity less than or equal to " + availableStock + ".");
                        return; // Exit if not enough stock
                    }

                    // Start a transaction to ensure data integrity
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        // Insert sale entry
                        string insertSaleQuery = "INSERT INTO Sales (ProductID, QuantitySold, SaleDate) VALUES (@ProductID, @QuantitySold, @SaleDate)";
                        SqlCommand insertSaleCmd = new SqlCommand(insertSaleQuery, conn, transaction);
                        insertSaleCmd.Parameters.AddWithValue("@ProductID", productId);
                        insertSaleCmd.Parameters.AddWithValue("@QuantitySold", quantitySold);
                        insertSaleCmd.Parameters.AddWithValue("@SaleDate", saleDate);
                        insertSaleCmd.ExecuteNonQuery();

                        // Update product quantity
                        string updateProductQuery = "UPDATE Products SET Quantity = Quantity - @QuantitySold WHERE ProductID = @ProductID";
                        SqlCommand updateProductCmd = new SqlCommand(updateProductQuery, conn, transaction);
                        updateProductCmd.Parameters.AddWithValue("@QuantitySold", quantitySold);
                        updateProductCmd.Parameters.AddWithValue("@ProductID", productId);
                        updateProductCmd.ExecuteNonQuery();

                        // Commit the transaction
                        transaction.Commit();

                        MessageBox.Show("Sale entry saved and inventory updated successfully.");

                        // Reset the form fields for a new entry
                        ResetForm();
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction if an error occurs
                        transaction.Rollback();
                        MessageBox.Show("An error occurred while processing the transaction. Please try again.");
                    }
                }
                catch (SqlException sqlEx)
                {
                    // Handle database connection issues
                    MessageBox.Show("Database connection error. Please check your network or contact the administrator.\nDetails: " + sqlEx.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An unexpected error occurred: " + ex.Message);
                }
            }
        }

        // Method to reset form fields
        private void ResetForm()
        {
            cmbProduct.SelectedIndex = -1; // Deselect product
            txtQuantitySold.Clear();       // Clear quantity field
            dtpSaleDate.Value = DateTime.Now; // Reset date to current
        }

        private void txtQuantitySold_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow control keys such as backspace
            if (!char.IsControl(e.KeyChar))
            {
                // Allow only digits
                if (!char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;  // Ignore non-digit input
                }
            }
        }
    }
}
