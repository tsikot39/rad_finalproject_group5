namespace WindowsFormsApp1
{
    partial class LowStockAlert
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblStoreManager = new System.Windows.Forms.Label();
            this.dgvLowStockAlerts = new System.Windows.Forms.DataGridView();
            this.btnExportLowStock = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLowStockAlerts)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(376, 53);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(431, 32);
            this.lblTitle.TabIndex = 3;
            this.lblTitle.Text = "INVENTORY MANAGEMENT SYSTEM";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblStoreManager
            // 
            this.lblStoreManager.AutoSize = true;
            this.lblStoreManager.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStoreManager.Location = new System.Drawing.Point(506, 146);
            this.lblStoreManager.Name = "lblStoreManager";
            this.lblStoreManager.Size = new System.Drawing.Size(173, 28);
            this.lblStoreManager.TabIndex = 8;
            this.lblStoreManager.Text = "Low-Stock Alerts";
            this.lblStoreManager.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dgvLowStockAlerts
            // 
            this.dgvLowStockAlerts.AllowUserToAddRows = false;
            this.dgvLowStockAlerts.AllowUserToDeleteRows = false;
            this.dgvLowStockAlerts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLowStockAlerts.Location = new System.Drawing.Point(12, 192);
            this.dgvLowStockAlerts.Name = "dgvLowStockAlerts";
            this.dgvLowStockAlerts.ReadOnly = true;
            this.dgvLowStockAlerts.RowHeadersWidth = 51;
            this.dgvLowStockAlerts.RowTemplate.Height = 24;
            this.dgvLowStockAlerts.Size = new System.Drawing.Size(1158, 465);
            this.dgvLowStockAlerts.TabIndex = 9;
            // 
            // btnExportLowStock
            // 
            this.btnExportLowStock.Location = new System.Drawing.Point(484, 680);
            this.btnExportLowStock.Name = "btnExportLowStock";
            this.btnExportLowStock.Size = new System.Drawing.Size(140, 50);
            this.btnExportLowStock.TabIndex = 11;
            this.btnExportLowStock.Text = "Export Report";
            this.btnExportLowStock.UseVisualStyleBackColor = true;
            this.btnExportLowStock.Click += new System.EventHandler(this.btnExportLowStock_Click);
            // 
            // LowStockAlert
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.ClientSize = new System.Drawing.Size(1182, 753);
            this.Controls.Add(this.btnExportLowStock);
            this.Controls.Add(this.dgvLowStockAlerts);
            this.Controls.Add(this.lblStoreManager);
            this.Controls.Add(this.lblTitle);
            this.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "LowStockAlert";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Low-Stock Alerts";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLowStockAlerts)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblStoreManager;
        private System.Windows.Forms.DataGridView dgvLowStockAlerts;
        private System.Windows.Forms.Button btnExportLowStock;
    }
}