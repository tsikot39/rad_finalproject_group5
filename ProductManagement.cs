using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Configuration;
using System.Drawing;

namespace WindowsFormsApp1
{
    public partial class txtSupplierEmail : Form
    {
        string connectionString = ConfigurationManager.ConnectionStrings["InventoryManagementDB"].ConnectionString;

        // Flag to track whether we are adding a new product or updating an existing one
        bool isAddingNewProduct = false;

        public txtSupplierEmail()
        {
            InitializeComponent();

            // Set initial placeholder text
            txtSearch.Text = "Product Name";
            txtSearch.ForeColor = Color.Gray;

            // Attach events
            txtSearch.Enter += RemovePlaceholder;
            txtSearch.Leave += SetPlaceholder;
        }

        private void RemovePlaceholder(object sender, EventArgs e)
        {
            if (txtSearch.Text == "Product Name")
            {
                txtSearch.Text = "";
                txtSearch.ForeColor = Color.Black; // Set normal text color
            }
        }

        private void SetPlaceholder(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "Product Name";
                txtSearch.ForeColor = Color.Gray; // Set placeholder text color
            }
        }

        private void ProductManagement_Load_1(object sender, EventArgs e)
        {
            lblTitle.Left = (this.ClientSize.Width - lblTitle.Width) / 2;
            lblStoreManager.Left = (this.ClientSize.Width - lblStoreManager.Width) / 2;

            // Set ComboBox to DropDownList to disable typing
            cmbFilterCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFilterSupplier.DropDownStyle = ComboBoxStyle.DropDownList;

            dgvProducts.AllowUserToResizeRows = false;

            dgvProducts.ClearSelection();
            dgvProducts.CurrentCell = null;

            // Set no selection after the form has loaded completely
            this.BeginInvoke((MethodInvoker)delegate
            {
                dgvProducts.ClearSelection();
                dgvProducts.CurrentCell = null;
            });

            // Load categories and suppliers into ComboBoxes
            LoadCategories();
            LoadSuppliers();

            // Load products into DataGridView
            LoadCategoryFilter();
            LoadSupplierFilter();
            LoadProducts();

            // Initially disable the Save/Update, Delete, and Clear buttons
            btnUpdateProduct.Enabled = false;
            btnDeleteProduct.Enabled = false;
            btnClearFields.Enabled = false;  // Disable the Clear button initially

            // Add event handlers to detect changes in fields
            AddFieldChangeHandlers();

            dgvProducts.CellFormatting += dgvProducts_CellFormatting;

            dgvProducts.DataError += dgvProducts_DataError;

            txtPrice.KeyPress += txtPrice_KeyPress;

            txtQuantity.KeyPress += txtQuantity_KeyPress;

        }

        // Load categories into cmbCategory and ensure no default selection
        private void LoadCategories()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT CategoryID, CategoryName FROM Categories";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                cmbCategory.DataSource = dt;
                cmbCategory.DisplayMember = "CategoryName";
                cmbCategory.ValueMember = "CategoryID";

                // Set no default selection
                cmbCategory.SelectedIndex = -1;
            }
        }

        // Load suppliers into cmbSupplier and ensure no default selection
        private void LoadSuppliers()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT SupplierID, SupplierName FROM Suppliers";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                cmbSupplier.DataSource = dt;
                cmbSupplier.DisplayMember = "SupplierName";
                cmbSupplier.ValueMember = "SupplierID";

                // Set no default selection
                cmbSupplier.SelectedIndex = -1;
            }
        }

        // Method to Load Products into DataGridView
        private void LoadProducts(string filterQuery = "")
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                                SELECT Products.ProductID, Products.ProductName, Categories.CategoryID, Categories.CategoryName, 
                                       Products.Price, Products.Quantity, Suppliers.SupplierID, Suppliers.SupplierName, 
                                       Products.ReorderLevel, Suppliers.SupplierEmail
                                FROM Products
                                INNER JOIN Categories ON Products.CategoryID = Categories.CategoryID
                                INNER JOIN Suppliers ON Products.SupplierID = Suppliers.SupplierID";


                if (!string.IsNullOrWhiteSpace(filterQuery))
                {
                    query += $" WHERE ProductName LIKE '%{filterQuery}%' OR CategoryName LIKE '%{filterQuery}%'";
                }

                // Sort by ProductID in descending order to show the most recently added data first
                query += " ORDER BY Products.ProductID DESC";  // or use CreatedDate DESC if you have a timestamp column

                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvProducts.DataSource = dt;

                dgvProducts.Columns["ProductName"].HeaderText = "Product Name";
                dgvProducts.Columns["CategoryName"].HeaderText = "Category";
                dgvProducts.Columns["SupplierName"].HeaderText = "Supplier";
                dgvProducts.Columns["ReorderLevel"].HeaderText = "Reorder Level";
                dgvProducts.Columns["SupplierEmail"].HeaderText = "Supplier Email";

                // Format the Price column with commas and two decimal places
                foreach (DataGridViewRow row in dgvProducts.Rows)
                {
                    if (row.Cells["Price"].Value != null)
                    {
                        row.Cells["Price"].Value = Convert.ToDecimal(row.Cells["Price"].Value).ToString("N2");
                    }
                }

                // Hide the ProductID column
                if (dgvProducts.Columns["ProductID"] != null)
                {
                    dgvProducts.Columns["ProductID"].Visible = false;
                }

                // Hide the CategoryID column (it won't be selected in the query anymore, so it's not needed)
                if (dgvProducts.Columns["CategoryID"] != null)
                {
                    dgvProducts.Columns["CategoryID"].Visible = false;
                }

                if (dgvProducts.Columns["SupplierID"] != null)
                {
                    dgvProducts.Columns["SupplierID"].Visible = false;
                }

                // Auto-size the columns to fit the content
                dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                // dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvProducts.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;

                
            }
            dgvProducts.ClearSelection();
            dgvProducts.CurrentCell = null;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            //if (!string.IsNullOrWhiteSpace(txtSearch.Text) && txtSearch.Text != "Product Name")
            //{
                // Only load filtered data if there is actual search input
                // LoadProducts(txtSearch.Text.Trim());
            //}
            ApplyFilter();
        }

        // Add event handlers for detecting field changes
        private void AddFieldChangeHandlers()
        {
            txtProductName.TextChanged += FieldChanged;
            txtPrice.TextChanged += FieldChanged;
            txtQuantity.TextChanged += FieldChanged;
            cmbSupplier.SelectedIndexChanged += FieldChanged;
            txtReorderLevel.TextChanged += FieldChanged;
        }

        // Event handler that enables the Clear button when any field has data
        private void FieldChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtProductName.Text) ||
                cmbCategory.SelectedIndex != -1 ||
                !string.IsNullOrWhiteSpace(txtPrice.Text) ||
                !string.IsNullOrWhiteSpace(txtQuantity.Text) ||
                cmbSupplier.SelectedIndex != -1 ||
                !string.IsNullOrWhiteSpace(txtReorderLevel.Text))
            {
                btnClearFields.Enabled = true;
            }
            else
            {
                btnClearFields.Enabled = false;
            }
        }

        private void btnAddProduct_Click(object sender, EventArgs e)
        {
            // Clear the fields for new entry and disable the Add button
            ClearFields();
            isAddingNewProduct = true;  // Set flag to indicate adding new product
            btnAddProduct.Enabled = false;
            btnUpdateProduct.Enabled = true;  // Enable Save/Update button for inserting new data
            btnDeleteProduct.Enabled = false; // Disable Delete button when adding a new product
            btnClearFields.Enabled = true;   // Enable Clear button to allow canceling

            txtProductName.Focus();
        }

        private void btnDeleteProduct_Click(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count > 0)
            {
                // Confirm deletion
                DialogResult result = MessageBox.Show(
                    "Are you sure you want to delete the selected product? This action cannot be undone.",
                    "Confirm Deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        // First, delete related records in LowStockAlerts table
                        string deleteLowStockAlertsQuery = "DELETE FROM LowStockAlerts WHERE ProductID = @ProductID";
                        SqlCommand deleteLowStockAlertsCmd = new SqlCommand(deleteLowStockAlertsQuery, conn);
                        deleteLowStockAlertsCmd.Parameters.AddWithValue("@ProductID", dgvProducts.SelectedRows[0].Cells["ProductID"].Value);

                        // Delete related records in Sales table
                        string deleteSalesQuery = "DELETE FROM Sales WHERE ProductID = @ProductID";
                        SqlCommand deleteSalesCmd = new SqlCommand(deleteSalesQuery, conn);
                        deleteSalesCmd.Parameters.AddWithValue("@ProductID", dgvProducts.SelectedRows[0].Cells["ProductID"].Value);

                        conn.Open();
                        deleteLowStockAlertsCmd.ExecuteNonQuery(); // Delete from LowStockAlerts
                        deleteSalesCmd.ExecuteNonQuery();          // Delete from Sales

                        // Now delete the product from the Products table
                        string query = "DELETE FROM Products WHERE ProductID = @ProductID";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@ProductID", dgvProducts.SelectedRows[0].Cells["ProductID"].Value);

                        cmd.ExecuteNonQuery();
                        conn.Close();

                        MessageBox.Show(
                            "The product has been successfully deleted.",
                            "Product Deleted",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        // Reload products
                        LoadProducts();
                        ClearFields();
                    }

                    btnDeleteProduct.Enabled = false; // Disable after deletion
                }
            }
            else
            {
                MessageBox.Show(
                    "Please select a product to delete.",
                    "No Selection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }


        private void btnClearFields_Click(object sender, EventArgs e)
        {
            ClearFields();
            btnAddProduct.Enabled = true;
            btnUpdateProduct.Enabled = false;  // Disable Save/Update when fields are cleared
            btnDeleteProduct.Enabled = false;  // Disable Delete when fields are cleared
            isAddingNewProduct = false; // Reset flag when clearing fields
            btnClearFields.Enabled = false;  // Disable Clear button after clearing fields
        }

        // Method to clear input fields
        private void ClearFields()
        {
            txtProductName.Clear();
            cmbCategory.SelectedIndex = -1;
            txtPrice.Clear();
            txtQuantity.Clear();
            cmbSupplier.SelectedIndex = -1;
            txtReorderLevel.Clear();

            dgvProducts.ClearSelection(); // Clear selection in DataGridView

            btnDeleteProduct.Enabled = false; // Disable Delete when fields are cleared
        }

        // When a product is selected in DataGridView, populate the fields
        private void dgvProducts_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count > 0)
            {
                txtProductName.Text = dgvProducts.SelectedRows[0].Cells["ProductName"].Value.ToString();

                // Map the CategoryName from the DataGridView to cmbCategory
                string categoryName = dgvProducts.SelectedRows[0].Cells["CategoryName"].Value.ToString();
                cmbCategory.SelectedIndex = cmbCategory.FindStringExact(categoryName);

                txtPrice.Text = Convert.ToDecimal(dgvProducts.SelectedRows[0].Cells["Price"].Value).ToString("N2");
                txtQuantity.Text = Convert.ToInt32(dgvProducts.SelectedRows[0].Cells["Quantity"].Value).ToString("N0");

                // Map the SupplierName from the DataGridView to cmbSupplier
                string supplierName = dgvProducts.SelectedRows[0].Cells["SupplierName"].Value.ToString();
                cmbSupplier.SelectedIndex = cmbSupplier.FindStringExact(supplierName);

                txtReorderLevel.Text = dgvProducts.SelectedRows[0].Cells["ReorderLevel"].Value.ToString();

                isAddingNewProduct = false; // Set flag to false since we are updating an existing product
                btnUpdateProduct.Enabled = true; // Enable Save/Update when a row is selected for editing
                btnAddProduct.Enabled = true;  // Re-enable Add if fields are populated by selection
                btnDeleteProduct.Enabled = true; // Enable Delete when a row is selected
                btnClearFields.Enabled = true;   // Enable Clear button if a row is selected
            }
        }


        private void btnUpdateProduct_Click(object sender, EventArgs e)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                MessageBox.Show(
                    "Please provide the product name.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (cmbCategory.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Please select a category.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                MessageBox.Show(
                    "Please provide a valid, non-negative price.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtQuantity.Text))
            {
                MessageBox.Show(
                    "Please provide a valid, non-negative quantity.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (cmbSupplier.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Please select a supplier.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtReorderLevel.Text))
            {
                MessageBox.Show(
                    "Please provide a valid, non-negative reorder level.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // Parse the price and quantity values back from the formatted strings
            decimal price = decimal.Parse(txtPrice.Text.Replace(",", ""));
            int quantity = int.Parse(txtQuantity.Text.Replace(",", ""));

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query;
                bool isUpdateOperation = !isAddingNewProduct && dgvProducts.SelectedRows.Count > 0;

                if (isUpdateOperation)
                {
                    // Update existing product
                    query = "UPDATE Products SET ProductName = @ProductName, CategoryID = @CategoryID, Price = @Price, Quantity = @Quantity, SupplierID = @SupplierID, ReorderLevel = @ReorderLevel WHERE ProductID = @ProductID";
                }
                else
                {
                    // Insert new product
                    query = "INSERT INTO Products (ProductName, CategoryID, Price, Quantity, SupplierID, ReorderLevel) VALUES (@ProductName, @CategoryID, @Price, @Quantity, @SupplierID, @ReorderLevel)";
                }

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ProductName", txtProductName.Text);
                cmd.Parameters.AddWithValue("@CategoryID", cmbCategory.SelectedValue);
                cmd.Parameters.AddWithValue("@Price", price);
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.Parameters.AddWithValue("@SupplierID", cmbSupplier.SelectedValue);
                cmd.Parameters.AddWithValue("@ReorderLevel", int.Parse(txtReorderLevel.Text));

                if (isUpdateOperation)
                {
                    cmd.Parameters.AddWithValue("@ProductID", dgvProducts.SelectedRows[0].Cells["ProductID"].Value);
                }

                cmd.ExecuteNonQuery();
                conn.Close();

                // Show appropriate message based on the operation
                string message = isUpdateOperation
                    ? "The product information has been successfully updated."
                    : "The product has been successfully added.";

                string title = isUpdateOperation
                    ? "Product Updated"
                    : "Product Added";

                MessageBox.Show(
                    message,
                    title,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // Reload products and reset the form
                LoadProducts();
                ClearFields();

                btnUpdateProduct.Enabled = false;  // Disable Update button
                btnAddProduct.Enabled = true;      // Enable Add button
            }
        }



        // Load the available categories and suppliers into the filter ComboBoxes
        private void LoadCategoryFilter()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT CategoryID, CategoryName FROM Categories";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // Insert a "All" option at the top to show all products if no specific filter is applied
                DataRow allRow = dt.NewRow();
                allRow["CategoryID"] = DBNull.Value; // No specific CategoryID for "All"
                allRow["CategoryName"] = "All";
                dt.Rows.InsertAt(allRow, 0);

                cmbFilterCategory.DataSource = dt;
                cmbFilterCategory.DisplayMember = "CategoryName";
                cmbFilterCategory.ValueMember = "CategoryID";
            }
        }

        private void LoadSupplierFilter()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT SupplierID, SupplierName FROM Suppliers";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // Insert a "All" option at the top to show all products if no specific filter is applied
                DataRow allRow = dt.NewRow();
                allRow["SupplierID"] = DBNull.Value;
                allRow["SupplierName"] = "All";
                dt.Rows.InsertAt(allRow, 0);

                cmbFilterSupplier.DataSource = dt;
                cmbFilterSupplier.DisplayMember = "SupplierName";
                cmbFilterSupplier.ValueMember = "SupplierID";
            }
        }

        private void cmbFilterCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void cmbFilterSupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        // Step 3: Apply Filter logic based on selected values in the ComboBoxes
        private void ApplyFilter()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT Products.ProductID, Products.ProductName, Categories.CategoryName, 
                           Products.Price, Products.Quantity, Suppliers.SupplierName, 
                           Products.ReorderLevel, Suppliers.SupplierEmail
                    FROM Products
                    INNER JOIN Categories ON Products.CategoryID = Categories.CategoryID
                    INNER JOIN Suppliers ON Products.SupplierID = Suppliers.SupplierID
                    WHERE 1=1";  // Always true condition to simplify adding other conditions dynamically

                SqlCommand cmd = new SqlCommand(query, conn);

                // Apply Search filter if there is input in txtSearch
                if (!string.IsNullOrWhiteSpace(txtSearch.Text) && txtSearch.Text != "Product Name")
                {
                    query += " AND Products.ProductName LIKE @SearchTerm";
                    cmd.Parameters.AddWithValue("@SearchTerm", $"%{txtSearch.Text}%");
                }

                // Apply Category filter if a specific category is selected (not "All")
                if (cmbFilterCategory.SelectedValue != null && cmbFilterCategory.SelectedValue != DBNull.Value)
                {
                    DataRowView selectedCategory = cmbFilterCategory.SelectedItem as DataRowView;
                    if (selectedCategory != null)
                    {
                        query += " AND Products.CategoryID = @CategoryID";
                        cmd.Parameters.AddWithValue("@CategoryID", selectedCategory["CategoryID"]);
                    }
                }

                // Apply Supplier filter if a specific supplier is selected (not "All")
                if (cmbFilterSupplier.SelectedValue != null && cmbFilterSupplier.SelectedValue != DBNull.Value)
                {
                    DataRowView selectedSupplier = cmbFilterSupplier.SelectedItem as DataRowView;
                    if (selectedSupplier != null)
                    {
                        query += " AND Products.SupplierID = @SupplierID";
                        cmd.Parameters.AddWithValue("@SupplierID", selectedSupplier["SupplierID"]);
                    }
                }

                // Sort by ProductID in descending order to show the most recently added data first
                query += " ORDER BY Products.ProductID DESC";  // or use CreatedDate DESC if you have a timestamp column

                cmd.CommandText = query; // Update the command text with the modified query

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvProducts.DataSource = dt;

                dgvProducts.Columns["ProductName"].HeaderText = "Product Name";
                dgvProducts.Columns["CategoryName"].HeaderText = "Category";
                dgvProducts.Columns["SupplierName"].HeaderText = "Supplier";
                dgvProducts.Columns["ReorderLevel"].HeaderText = "Reorder Level";
                dgvProducts.Columns["SupplierEmail"].HeaderText = "Supplier Email";

                // Auto-size the columns to fit the content
                dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                // dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvProducts.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            }

            dgvProducts.ClearSelection();
            dgvProducts.CurrentCell = null;
        }

        private void dgvProducts_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Ensure the correct column is the "Price" column
            if (dgvProducts.Columns[e.ColumnIndex].Name == "Price" && e.Value != null)
            {
                // Apply comma formatting with two decimal places for Price
                e.Value = string.Format("{0:N2}", e.Value);
                e.FormattingApplied = true;  // Mark formatting as applied
            }
        }

        private void dgvProducts_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Handle the error silently to prevent the exception from being thrown
            e.ThrowException = false;
        }

        private void txtPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow control keys such as backspace
            if (!char.IsControl(e.KeyChar))
            {
                // Allow digits and only one decimal point
                if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;  // Ignore the input if it is not a digit or decimal point
                }

                // Check if the user already typed a decimal point
                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;  // Ignore the input if a decimal point already exists
                }
            }
        }

        private void txtQuantity_KeyPress(object sender, KeyPressEventArgs e)
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

        private void dgvProducts_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {

        }
    }
}
