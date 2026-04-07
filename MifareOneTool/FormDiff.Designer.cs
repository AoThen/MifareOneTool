namespace MifareOneTool
{
    partial class FormDiff
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDiff));
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageText = new System.Windows.Forms.TabPage();
            this.tabPageVisual = new System.Windows.Forms.TabPage();
            this.dataGridViewDiff = new System.Windows.Forms.DataGridView();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPageText.SuspendLayout();
            this.tabPageVisual.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDiff)).BeginInit();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.Color.Black;
            resources.ApplyResources(this.richTextBox1, "richTextBox1");
            this.richTextBox1.ForeColor = System.Drawing.Color.Orange;
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.TextChanged += new System.EventHandler(this.RichTextBox1_TextChanged);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            resources.ApplyResources(this.button2, "button2");
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            resources.ApplyResources(this.button3, "button3");
            this.button3.Name = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.button3, 1, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Controls.Add(this.button1);
            this.flowLayoutPanel1.Controls.Add(this.button2);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // tabControl
            // 
            resources.ApplyResources(this.tabControl, "tabControl");
            this.tabControl.Controls.Add(this.tabPageText);
            this.tabControl.Controls.Add(this.tabPageVisual);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tabPageText
            // 
            this.tabPageText.Controls.Add(this.richTextBox1);
            resources.ApplyResources(this.tabPageText, "tabPageText");
            this.tabPageText.Name = "tabPageText";
            this.tabPageText.UseVisualStyleBackColor = true;
            // 
            // tabPageVisual
            // 
            this.tabPageVisual.Controls.Add(this.dataGridViewDiff);
            resources.ApplyResources(this.tabPageVisual, "tabPageVisual");
            this.tabPageVisual.Name = "tabPageVisual";
            this.tabPageVisual.UseVisualStyleBackColor = true;
            // 
            // dataGridViewDiff
            // 
            this.dataGridViewDiff.AllowUserToAddRows = false;
            this.dataGridViewDiff.AllowUserToDeleteRows = false;
            this.dataGridViewDiff.AllowUserToResizeRows = false;
            resources.ApplyResources(this.dataGridViewDiff, "dataGridViewDiff");
            this.dataGridViewDiff.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewDiff.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewDiff.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewDiff.Name = "dataGridViewDiff";
            this.dataGridViewDiff.ReadOnly = true;
            this.dataGridViewDiff.RowHeadersVisible = false;
            this.dataGridViewDiff.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridViewDiff_CellFormatting);
            // 
            // flowLayoutPanel2
            // 
            resources.ApplyResources(this.flowLayoutPanel2, "flowLayoutPanel2");
            this.flowLayoutPanel2.Controls.Add(this.tableLayoutPanel1);
            this.flowLayoutPanel2.Controls.Add(this.tabControl);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            // 
            // FormDiff
            // 
            this.AcceptButton = this.button3;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowLayoutPanel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "FormDiff";
            this.Load += new System.EventHandler(this.FormDiff_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabPageText.ResumeLayout(false);
            this.tabPageVisual.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDiff)).EndInit();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageText;
        private System.Windows.Forms.TabPage tabPageVisual;
        private System.Windows.Forms.DataGridView dataGridViewDiff;
    }
}