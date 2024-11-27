namespace WindowsFormsApp1
{
    partial class Dashboard
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
            this.btnManageProducts = new System.Windows.Forms.Button();
            this.btnViewLowStock = new System.Windows.Forms.Button();
            this.btnViewReports = new System.Windows.Forms.Button();
            this.lblOverview = new System.Windows.Forms.Label();
            this.lblDateTime = new System.Windows.Forms.Label();
            this.lblUsername = new System.Windows.Forms.Label();
            this.linkLogout = new System.Windows.Forms.LinkLabel();
            this.lblSeparator = new System.Windows.Forms.Label();
            this.btnSalesEntry = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(176, 53);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(431, 32);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "INVENTORY MANAGEMENT SYSTEM";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblStoreManager
            // 
            this.lblStoreManager.AutoSize = true;
            this.lblStoreManager.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStoreManager.Location = new System.Drawing.Point(261, 146);
            this.lblStoreManager.Name = "lblStoreManager";
            this.lblStoreManager.Size = new System.Drawing.Size(260, 28);
            this.lblStoreManager.TabIndex = 6;
            this.lblStoreManager.Text = "Store Manager Dashboard";
            this.lblStoreManager.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnManageProducts
            // 
            this.btnManageProducts.Location = new System.Drawing.Point(294, 205);
            this.btnManageProducts.Name = "btnManageProducts";
            this.btnManageProducts.Size = new System.Drawing.Size(195, 50);
            this.btnManageProducts.TabIndex = 7;
            this.btnManageProducts.Text = "Product Management";
            this.btnManageProducts.UseVisualStyleBackColor = true;
            this.btnManageProducts.Click += new System.EventHandler(this.btnManageProducts_Click);
            // 
            // btnViewLowStock
            // 
            this.btnViewLowStock.Location = new System.Drawing.Point(294, 261);
            this.btnViewLowStock.Name = "btnViewLowStock";
            this.btnViewLowStock.Size = new System.Drawing.Size(195, 50);
            this.btnViewLowStock.TabIndex = 8;
            this.btnViewLowStock.Text = "Low-Stock Alerts";
            this.btnViewLowStock.UseVisualStyleBackColor = true;
            this.btnViewLowStock.Click += new System.EventHandler(this.btnViewLowStock_Click);
            // 
            // btnViewReports
            // 
            this.btnViewReports.Location = new System.Drawing.Point(294, 373);
            this.btnViewReports.Name = "btnViewReports";
            this.btnViewReports.Size = new System.Drawing.Size(195, 50);
            this.btnViewReports.TabIndex = 9;
            this.btnViewReports.Text = "Reports";
            this.btnViewReports.UseVisualStyleBackColor = true;
            this.btnViewReports.Click += new System.EventHandler(this.btnViewReports_Click);
            // 
            // lblOverview
            // 
            this.lblOverview.AutoSize = true;
            this.lblOverview.Location = new System.Drawing.Point(364, 453);
            this.lblOverview.Name = "lblOverview";
            this.lblOverview.Size = new System.Drawing.Size(55, 23);
            this.lblOverview.TabIndex = 10;
            this.lblOverview.Text = "label1";
            this.lblOverview.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDateTime
            // 
            this.lblDateTime.AutoSize = true;
            this.lblDateTime.Location = new System.Drawing.Point(715, 521);
            this.lblDateTime.Name = "lblDateTime";
            this.lblDateTime.Size = new System.Drawing.Size(55, 23);
            this.lblDateTime.TabIndex = 11;
            this.lblDateTime.Text = "label1";
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(625, 9);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(55, 23);
            this.lblUsername.TabIndex = 12;
            this.lblUsername.Text = "label1";
            // 
            // linkLogout
            // 
            this.linkLogout.AutoSize = true;
            this.linkLogout.Location = new System.Drawing.Point(706, 9);
            this.linkLogout.Name = "linkLogout";
            this.linkLogout.Size = new System.Drawing.Size(64, 23);
            this.linkLogout.TabIndex = 13;
            this.linkLogout.TabStop = true;
            this.linkLogout.Text = "Logout";
            this.linkLogout.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLogout_LinkClicked);
            // 
            // lblSeparator
            // 
            this.lblSeparator.AutoSize = true;
            this.lblSeparator.Location = new System.Drawing.Point(686, 9);
            this.lblSeparator.Name = "lblSeparator";
            this.lblSeparator.Size = new System.Drawing.Size(14, 23);
            this.lblSeparator.TabIndex = 14;
            this.lblSeparator.Text = "|";
            // 
            // btnSalesEntry
            // 
            this.btnSalesEntry.Location = new System.Drawing.Point(294, 317);
            this.btnSalesEntry.Name = "btnSalesEntry";
            this.btnSalesEntry.Size = new System.Drawing.Size(195, 50);
            this.btnSalesEntry.TabIndex = 15;
            this.btnSalesEntry.Text = "Sales Entry";
            this.btnSalesEntry.UseVisualStyleBackColor = true;
            this.btnSalesEntry.Click += new System.EventHandler(this.btnSalesEntry_Click);
            // 
            // Dashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.ClientSize = new System.Drawing.Size(782, 553);
            this.Controls.Add(this.btnSalesEntry);
            this.Controls.Add(this.lblSeparator);
            this.Controls.Add(this.linkLogout);
            this.Controls.Add(this.lblUsername);
            this.Controls.Add(this.lblDateTime);
            this.Controls.Add(this.lblOverview);
            this.Controls.Add(this.btnViewReports);
            this.Controls.Add(this.btnViewLowStock);
            this.Controls.Add(this.btnManageProducts);
            this.Controls.Add(this.lblStoreManager);
            this.Controls.Add(this.lblTitle);
            this.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "Dashboard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Inventory Management System";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblStoreManager;
        private System.Windows.Forms.Button btnManageProducts;
        private System.Windows.Forms.Button btnViewLowStock;
        private System.Windows.Forms.Button btnViewReports;
        private System.Windows.Forms.Label lblOverview;
        private System.Windows.Forms.Label lblDateTime;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.LinkLabel linkLogout;
        private System.Windows.Forms.Label lblSeparator;
        private System.Windows.Forms.Button btnSalesEntry;
    }
}