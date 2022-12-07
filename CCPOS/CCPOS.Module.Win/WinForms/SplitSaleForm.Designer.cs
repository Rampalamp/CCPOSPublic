namespace CCPOS.Module.Win.WinForms
{
    partial class SplitSaleForm
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
            this.partySize = new System.Windows.Forms.NumericUpDown();
            this.btnSplitSale = new MaterialSkin.Controls.MaterialRaisedButton();
            this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
            ((System.ComponentModel.ISupportInitialize)(this.partySize)).BeginInit();
            this.SuspendLayout();
            // 
            // partySize
            // 
            this.partySize.Location = new System.Drawing.Point(70, 133);
            this.partySize.Name = "partySize";
            this.partySize.Size = new System.Drawing.Size(76, 22);
            this.partySize.TabIndex = 0;
            // 
            // btnSplitSale
            // 
            this.btnSplitSale.AutoSize = true;
            this.btnSplitSale.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSplitSale.Depth = 0;
            this.btnSplitSale.Icon = null;
            this.btnSplitSale.Location = new System.Drawing.Point(52, 162);
            this.btnSplitSale.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnSplitSale.Name = "btnSplitSale";
            this.btnSplitSale.Primary = true;
            this.btnSplitSale.Size = new System.Drawing.Size(113, 36);
            this.btnSplitSale.TabIndex = 1;
            this.btnSplitSale.Text = "SPLIT SALE";
            this.btnSplitSale.UseVisualStyleBackColor = true;
            this.btnSplitSale.Click += new System.EventHandler(this.btnSplitSale_Click);
            // 
            // materialLabel1
            // 
            this.materialLabel1.AutoSize = true;
            this.materialLabel1.BackColor = System.Drawing.SystemColors.Window;
            this.materialLabel1.Depth = 0;
            this.materialLabel1.Font = new System.Drawing.Font("Roboto", 11F);
            this.materialLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialLabel1.Location = new System.Drawing.Point(62, 99);
            this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel1.Name = "materialLabel1";
            this.materialLabel1.Size = new System.Drawing.Size(93, 24);
            this.materialLabel1.TabIndex = 2;
            this.materialLabel1.Text = "Party Size";
            // 
            // SplitSaleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(217, 232);
            this.Controls.Add(this.materialLabel1);
            this.Controls.Add(this.btnSplitSale);
            this.Controls.Add(this.partySize);
            this.Name = "SplitSaleForm";
            this.Text = "Split Sale Even";
            ((System.ComponentModel.ISupportInitialize)(this.partySize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown partySize;
        private MaterialSkin.Controls.MaterialRaisedButton btnSplitSale;
        private MaterialSkin.Controls.MaterialLabel materialLabel1;
    }
}