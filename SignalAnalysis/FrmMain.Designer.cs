namespace SignalAnalysis
{
    partial class FrmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnData = new System.Windows.Forms.Button();
            this.lblData = new System.Windows.Forms.Label();
            this.formsPlotCrossHair1 = new ScottPlot.FormsPlotCrossHair();
            this.formsPlotCrossHair2 = new ScottPlot.FormsPlotCrossHair();
            this.SuspendLayout();
            // 
            // btnData
            // 
            this.btnData.Location = new System.Drawing.Point(12, 12);
            this.btnData.Name = "btnData";
            this.btnData.Size = new System.Drawing.Size(87, 28);
            this.btnData.TabIndex = 0;
            this.btnData.Text = "Select data";
            this.btnData.UseVisualStyleBackColor = true;
            this.btnData.Click += new System.EventHandler(this.btnData_Click);
            // 
            // lblData
            // 
            this.lblData.AutoSize = true;
            this.lblData.Location = new System.Drawing.Point(114, 18);
            this.lblData.Name = "lblData";
            this.lblData.Size = new System.Drawing.Size(87, 19);
            this.lblData.TabIndex = 1;
            this.lblData.Text = "Data filepath";
            // 
            // formsPlotCrossHair1
            // 
            this.formsPlotCrossHair1.BackColor = System.Drawing.Color.Transparent;
            this.formsPlotCrossHair1.Constrict = false;
            this.formsPlotCrossHair1.CrossHairColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.formsPlotCrossHair1.HorizontalLine = false;
            this.formsPlotCrossHair1.Location = new System.Drawing.Point(13, 46);
            this.formsPlotCrossHair1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.formsPlotCrossHair1.Name = "formsPlotCrossHair1";
            this.formsPlotCrossHair1.Size = new System.Drawing.Size(758, 240);
            this.formsPlotCrossHair1.TabIndex = 2;
            this.formsPlotCrossHair1.VerticalLine = false;
            // 
            // formsPlotCrossHair2
            // 
            this.formsPlotCrossHair2.BackColor = System.Drawing.Color.Transparent;
            this.formsPlotCrossHair2.Constrict = false;
            this.formsPlotCrossHair2.CrossHairColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.formsPlotCrossHair2.HorizontalLine = false;
            this.formsPlotCrossHair2.Location = new System.Drawing.Point(13, 292);
            this.formsPlotCrossHair2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.formsPlotCrossHair2.Name = "formsPlotCrossHair2";
            this.formsPlotCrossHair2.Size = new System.Drawing.Size(758, 214);
            this.formsPlotCrossHair2.TabIndex = 3;
            this.formsPlotCrossHair2.VerticalLine = false;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 518);
            this.Controls.Add(this.formsPlotCrossHair2);
            this.Controls.Add(this.formsPlotCrossHair1);
            this.Controls.Add(this.lblData);
            this.Controls.Add(this.btnData);
            this.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button btnData;
        private Label lblData;
        private ScottPlot.FormsPlotCrossHair formsPlotCrossHair1;
        private ScottPlot.FormsPlotCrossHair formsPlotCrossHair2;
    }
}