namespace MifareOneTool
{
    partial class FormCardInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCardInfo));
            this.labelUID = new System.Windows.Forms.Label();
            this.labelBCC = new System.Windows.Forms.Label();
            this.labelBCCStatus = new System.Windows.Forms.Label();
            this.dataGridViewSectors = new System.Windows.Forms.DataGridView();
            this.richTextBoxDetail = new System.Windows.Forms.RichTextBox();
            this.buttonClose = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSectors)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelUID
            // 
            resources.ApplyResources(this.labelUID, "labelUID");
            this.labelUID.Name = "labelUID";
            // 
            // labelBCC
            // 
            resources.ApplyResources(this.labelBCC, "labelBCC");
            this.labelBCC.Name = "labelBCC";
            // 
            // labelBCCStatus
            // 
            resources.ApplyResources(this.labelBCCStatus, "labelBCCStatus");
            this.labelBCCStatus.Name = "labelBCCStatus";
            // 
            // dataGridViewSectors
            // 
            resources.ApplyResources(this.dataGridViewSectors, "dataGridViewSectors");
            this.dataGridViewSectors.AllowUserToAddRows = false;
            this.dataGridViewSectors.AllowUserToDeleteRows = false;
            this.dataGridViewSectors.AllowUserToResizeRows = false;
            this.dataGridViewSectors.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewSectors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewSectors.Name = "dataGridViewSectors";
            this.dataGridViewSectors.ReadOnly = true;
            this.dataGridViewSectors.RowHeadersVisible = false;
            this.dataGridViewSectors.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewSectors.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewSectors_CellClick);
            // 
            // richTextBoxDetail
            // 
            resources.ApplyResources(this.richTextBoxDetail, "richTextBoxDetail");
            this.richTextBoxDetail.BackColor = System.Drawing.Color.White;
            this.richTextBoxDetail.Name = "richTextBoxDetail";
            this.richTextBoxDetail.ReadOnly = true;
            // 
            // buttonClose
            // 
            resources.ApplyResources(this.buttonClose, "buttonClose");
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.dataGridViewSectors, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.richTextBoxDetail, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.buttonClose, 1, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Controls.Add(this.labelUID);
            this.flowLayoutPanel1.Controls.Add(this.labelBCC);
            this.flowLayoutPanel1.Controls.Add(this.labelBCCStatus);
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 2);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // FormCardInfo
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormCardInfo";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.FormCardInfo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSectors)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelUID;
        private System.Windows.Forms.Label labelBCC;
        private System.Windows.Forms.Label labelBCCStatus;
        private System.Windows.Forms.DataGridView dataGridViewSectors;
        private System.Windows.Forms.RichTextBox richTextBoxDetail;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}
