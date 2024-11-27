using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace WindowsFormsApp1
{
    public partial class LowStockAlert : Form
    {
        public LowStockAlert()
        {
            InitializeComponent();
            LoadLowStockAlerts();
        }

        // Method to load low-stock items into the DataGridView
        private void LoadLowStockAlerts()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["InventoryManagementDB"].ConnectionString;
            string query = @"
                SELECT Products.ProductID, Products.ProductName, Products.Quantity, Products.ReorderLevel, Suppliers.SupplierName, Suppliers.SupplierEmail
                FROM Products
                INNER JOIN Suppliers ON Products.SupplierID = Suppliers.SupplierID
                WHERE Products.Quantity <= Products.ReorderLevel
                ORDER BY Products.ProductID DESC";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvLowStockAlerts.DataSource = dt;

                dgvLowStockAlerts.Columns["ProductName"].HeaderText = "Product Name";
                dgvLowStockAlerts.Columns["ReorderLevel"].HeaderText = "Reorder Level";
                dgvLowStockAlerts.Columns["SupplierName"].HeaderText = "Supplier";
                dgvLowStockAlerts.Columns["SupplierEmail"].HeaderText = "Supplier Email";

                // Hide the ProductID column
                if (dgvLowStockAlerts.Columns["ProductID"] != null)
                {
                    dgvLowStockAlerts.Columns["ProductID"].Visible = false;
                }

                dgvLowStockAlerts.AllowUserToResizeRows = false;
                dgvLowStockAlerts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvLowStockAlerts.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            }

            dgvLowStockAlerts.ClearSelection();
            dgvLowStockAlerts.CurrentCell = null;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblTitle.Left = (this.ClientSize.Width - lblTitle.Width) / 2;
            lblStoreManager.Left = (this.ClientSize.Width - lblStoreManager.Width) / 2;

            dgvLowStockAlerts.ClearSelection();
            dgvLowStockAlerts.CurrentCell = null;

            // Set no selection after the form has loaded completely
            this.BeginInvoke((MethodInvoker)delegate
            {
                dgvLowStockAlerts.ClearSelection();
                dgvLowStockAlerts.CurrentCell = null;
            });
        }

        private void btnExportLowStock_Click(object sender, EventArgs e)
        {
            if (dgvLowStockAlerts.Rows.Count > 0)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    FileName = "LowStockReport.pdf"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 20f, 20f); // Match margins and styling

                    try
                    {
                        PdfWriter.GetInstance(pdfDoc, new FileStream(saveFileDialog.FileName, FileMode.Create));
                        pdfDoc.Open();

                        // Add title
                        Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                        Paragraph title = new Paragraph("Low Stock Report", titleFont);
                        title.Alignment = Element.ALIGN_CENTER;
                        pdfDoc.Add(title);

                        // Add date
                        Font dateFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
                        Paragraph date = new Paragraph($"Date: {DateTime.Now:MMMM dd, yyyy}", dateFont);
                        date.Alignment = Element.ALIGN_RIGHT;
                        pdfDoc.Add(date);

                        // Add some spacing
                        pdfDoc.Add(new Paragraph("\n"));

                        // Define visible columns and calculate dynamic widths
                        int visibleColumnCount = dgvLowStockAlerts.Columns.Cast<DataGridViewColumn>()
                            .Count(col => col.Visible && col.Name != "ProductID");

                        PdfPTable table = new PdfPTable(visibleColumnCount)
                        {
                            WidthPercentage = 100
                        };

                        float[] columnWidths = dgvLowStockAlerts.Columns.Cast<DataGridViewColumn>()
                            .Where(col => col.Visible && col.Name != "ProductID")
                            .Select(col =>
                            {
                                float maxWidth = col.HeaderText.Length;
                                foreach (DataGridViewRow row in dgvLowStockAlerts.Rows)
                                {
                                    if (row.Cells[col.Index].Value != null)
                                    {
                                        maxWidth = Math.Max(maxWidth, row.Cells[col.Index].Value.ToString().Length);
                                    }
                                }
                                return maxWidth * 0.5f; // Adjust scaling factor for better fit
                            })
                            .ToArray();

                        table.SetWidths(columnWidths);

                        // Add headers
                        foreach (DataGridViewColumn column in dgvLowStockAlerts.Columns)
                        {
                            if (column.Name != "ProductID")  // Skip ProductID
                            {
                                PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)))
                                {
                                    BackgroundColor = BaseColor.LIGHT_GRAY,
                                    HorizontalAlignment = Element.ALIGN_CENTER
                                };
                                table.AddCell(cell);
                            }
                        }

                        // Add data rows
                        foreach (DataGridViewRow row in dgvLowStockAlerts.Rows)
                        {
                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                if (dgvLowStockAlerts.Columns[cell.ColumnIndex].Name != "ProductID")  // Skip ProductID
                                {
                                    table.AddCell(new Phrase(cell.Value?.ToString() ?? string.Empty, FontFactory.GetFont(FontFactory.HELVETICA, 10)));
                                }
                            }
                        }

                        pdfDoc.Add(table);

                        // Add signatory section
                        Paragraph signatory = new Paragraph("\n\nCindy Leochico\nStore Manager", FontFactory.GetFont(FontFactory.HELVETICA, 12))
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        pdfDoc.Add(signatory);

                        MessageBox.Show(
                            "Low-Stock Report exported successfully as PDF.",
                            "Export Successful",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"An error occurred while exporting the report: {ex.Message}",
                            "Export Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                    finally
                    {
                        pdfDoc.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show(
                    "No data is available to export. Please ensure the low-stock report has data.",
                    "Export Unsuccessful",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

    }
}
