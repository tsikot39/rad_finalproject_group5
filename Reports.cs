using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

using System.Windows.Forms;
using System.Linq;

namespace WindowsFormsApp1
{
    public partial class Reports : Form
    {
        public Reports()
        {
            InitializeComponent();
            
            cmbReportType.Items.Add("Inventory Levels");
            cmbReportType.Items.Add("Sales History");
            cmbReportType.DropDownStyle = ComboBoxStyle.DropDownList;

            dgvReport.AllowUserToResizeRows = false;
            dgvReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvReport.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;

            // Add event handler for selection change
            cmbReportType.SelectedIndexChanged += cmbReportType_SelectedIndexChanged;
        }

        private void Reports_Load(object sender, EventArgs e)
        {
            lblTitle.Left = (this.ClientSize.Width - lblTitle.Width) / 2;
            lblReports.Left = (this.ClientSize.Width - lblReports.Width) / 2;

            dgvReport.ClearSelection();
            dgvReport.CurrentCell = null;

            // Set no selection after the form has loaded completely
            this.BeginInvoke((MethodInvoker)delegate
            {
                dgvReport.ClearSelection();
                dgvReport.CurrentCell = null;
            });
        }

        private void cmbReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedReportType = cmbReportType.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedReportType))
            {
                MessageBox.Show("Please select a report type.");
                return;
            }

            switch (selectedReportType)
            {
                case "Inventory Levels":
                    LoadInventoryLevels();
                    break;
                case "Sales History":
                    LoadSalesHistory();
                    break;
                default:
                    MessageBox.Show("Invalid report type selected.");
                    break;
            }
            dgvReport.ClearSelection();
            dgvReport.CurrentCell = null;
        }

        private void LoadInventoryLevels()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["InventoryManagementDB"].ConnectionString;
            string query = "SELECT ProductName, Quantity, ReorderLevel FROM Products";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvReport.DataSource = dt;

                dgvReport.Columns["ProductName"].HeaderText = "Product Name";
                dgvReport.Columns["ReorderLevel"].HeaderText = "Reorder Level";

                dgvReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvReport.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            }
        }

        private void LoadSalesHistory()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["InventoryManagementDB"].ConnectionString;
            string query = @"
                SELECT Products.ProductName, Sales.QuantitySold, Sales.SaleDate
                FROM Sales
                INNER JOIN Products ON Sales.ProductID = Products.ProductID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvReport.DataSource = dt;

                dgvReport.Columns["ProductName"].HeaderText = "Product Name";
                dgvReport.Columns["QuantitySold"].HeaderText = "Quantity Sold";
                dgvReport.Columns["SaleDate"].HeaderText = "Sale Date";
            }
        }

        private void btnExportReport_Click(object sender, EventArgs e)
        {
            if (dgvReport.Rows.Count > 0)
            {
                string defaultFileName = "Report"; // Default file name
                string reportTitle = "";

                // Determine the report title and default file name based on the selected report type
                if (cmbReportType.SelectedItem?.ToString() == "Sales History")
                {
                    reportTitle = "Sales Report";
                    defaultFileName = "SalesReport";
                }
                else if (cmbReportType.SelectedItem?.ToString() == "Inventory Levels")
                {
                    reportTitle = "Inventory Report";
                    defaultFileName = "InventoryReport";
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf|CSV files (*.csv)|*.csv",
                    FileName = defaultFileName // Set the default file name dynamically
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string extension = Path.GetExtension(saveFileDialog.FileName);

                    try
                    {
                        if (extension == ".pdf")
                        {
                            ExportToPdf(saveFileDialog.FileName, reportTitle);
                            MessageBox.Show(
                                $"{reportTitle} exported successfully as PDF.",
                                "Export Successful",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                        }
                        else if (extension == ".csv")
                        {
                            ExportToCsv(saveFileDialog.FileName);
                            MessageBox.Show(
                                $"{reportTitle} exported successfully as CSV.",
                                "Export Successful",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                        }
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
                }
            }
            else
            {
                MessageBox.Show(
                    "No data available to export. Please select a report with data.",
                    "Export Unsuccessful",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }



        private void ExportToPdf(string filePath, string reportTitle)
        {
            Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 20f, 20f);
            try
            {
                PdfWriter.GetInstance(pdfDoc, new FileStream(filePath, FileMode.Create));
                pdfDoc.Open();

                // Add title
                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                Paragraph title = new Paragraph(reportTitle, titleFont)
                {
                    Alignment = Element.ALIGN_CENTER
                };
                pdfDoc.Add(title);

                // Add date
                Font dateFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
                Paragraph date = new Paragraph($"Date: {DateTime.Now:MMMM dd, yyyy}", dateFont)
                {
                    Alignment = Element.ALIGN_RIGHT
                };
                pdfDoc.Add(date);

                // Add some spacing
                pdfDoc.Add(new Paragraph("\n"));

                // Define visible columns and dynamically calculate widths
                int visibleColumnCount = dgvReport.Columns.Cast<DataGridViewColumn>()
                    .Count(col => col.Visible);

                PdfPTable table = new PdfPTable(visibleColumnCount)
                {
                    WidthPercentage = 100
                };

                float[] columnWidths = dgvReport.Columns.Cast<DataGridViewColumn>()
                    .Where(col => col.Visible)
                    .Select(col =>
                    {
                        float maxWidth = col.HeaderText.Length;
                        foreach (DataGridViewRow row in dgvReport.Rows)
                        {
                            if (row.Cells[col.Index].Value != null)
                            {
                                maxWidth = Math.Max(maxWidth, row.Cells[col.Index].Value.ToString().Length);
                            }
                        }
                        return maxWidth * 0.5f; // Adjust scaling factor for appropriate fit
                    })
                    .ToArray();

                table.SetWidths(columnWidths);

                // Add headers
                foreach (DataGridViewColumn column in dgvReport.Columns)
                {
                    if (column.Visible)
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
                foreach (DataGridViewRow row in dgvReport.Rows)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.Visible)
                        {
                            table.AddCell(new Phrase(cell.Value?.ToString() ?? string.Empty, FontFactory.GetFont(FontFactory.HELVETICA, 10)));
                        }
                    }
                }

                // Add table to document
                pdfDoc.Add(table);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error exporting to PDF: " + ex.Message,
                    "Export Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                pdfDoc.Close();
            }
        }



        private void ExportToCsv(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Write headers
                for (int i = 0; i < dgvReport.Columns.Count; i++)
                {
                    writer.Write(dgvReport.Columns[i].HeaderText);
                    if (i < dgvReport.Columns.Count - 1) writer.Write(",");
                }
                writer.WriteLine();

                // Write data rows
                foreach (DataGridViewRow row in dgvReport.Rows)
                {
                    for (int i = 0; i < dgvReport.Columns.Count; i++)
                    {
                        writer.Write(row.Cells[i].Value?.ToString() ?? string.Empty);
                        if (i < dgvReport.Columns.Count - 1) writer.Write(",");
                    }
                    writer.WriteLine();
                }

                MessageBox.Show("Report exported successfully as CSV.");
            }
        }

        private void dgvReport_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
