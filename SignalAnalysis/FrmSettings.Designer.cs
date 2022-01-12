namespace SignalAnalysis
{
    partial class FrmSettings
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
            this.lblStart = new System.Windows.Forms.Label();
            this.lblEnd = new System.Windows.Forms.Label();
            this.txtStart = new System.Windows.Forms.TextBox();
            this.txtEnd = new System.Windows.Forms.TextBox();
            this.grpAxis = new System.Windows.Forms.GroupBox();
            this.radTime = new System.Windows.Forms.RadioButton();
            this.radSeconds = new System.Windows.Forms.RadioButton();
            this.radPoints = new System.Windows.Forms.RadioButton();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAccept = new System.Windows.Forms.Button();
            this.chkPower = new System.Windows.Forms.CheckBox();
            this.chkCumulative = new System.Windows.Forms.CheckBox();
            this.chkEntropy = new System.Windows.Forms.CheckBox();
            this.chkCrossHair = new System.Windows.Forms.CheckBox();
            this.grpAxis.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblStart
            // 
            this.lblStart.AutoSize = true;
            this.lblStart.Location = new System.Drawing.Point(20, 21);
            this.lblStart.Name = "lblStart";
            this.lblStart.Size = new System.Drawing.Size(113, 19);
            this.lblStart.TabIndex = 0;
            this.lblStart.Text = "Array index start:";
            // 
            // lblEnd
            // 
            this.lblEnd.AutoSize = true;
            this.lblEnd.Location = new System.Drawing.Point(20, 55);
            this.lblEnd.Name = "lblEnd";
            this.lblEnd.Size = new System.Drawing.Size(108, 19);
            this.lblEnd.TabIndex = 1;
            this.lblEnd.Text = "Array index end:";
            // 
            // txtStart
            // 
            this.txtStart.Location = new System.Drawing.Point(135, 18);
            this.txtStart.Name = "txtStart";
            this.txtStart.Size = new System.Drawing.Size(84, 25);
            this.txtStart.TabIndex = 2;
            // 
            // txtEnd
            // 
            this.txtEnd.Location = new System.Drawing.Point(135, 52);
            this.txtEnd.Name = "txtEnd";
            this.txtEnd.Size = new System.Drawing.Size(84, 25);
            this.txtEnd.TabIndex = 3;
            // 
            // grpAxis
            // 
            this.grpAxis.Controls.Add(this.radTime);
            this.grpAxis.Controls.Add(this.radSeconds);
            this.grpAxis.Controls.Add(this.radPoints);
            this.grpAxis.Location = new System.Drawing.Point(252, 22);
            this.grpAxis.Name = "grpAxis";
            this.grpAxis.Size = new System.Drawing.Size(162, 146);
            this.grpAxis.TabIndex = 4;
            this.grpAxis.TabStop = false;
            this.grpAxis.Text = "Abscissa axis";
            // 
            // radTime
            // 
            this.radTime.AutoSize = true;
            this.radTime.Location = new System.Drawing.Point(27, 105);
            this.radTime.Name = "radTime";
            this.radTime.Size = new System.Drawing.Size(114, 23);
            this.radTime.TabIndex = 2;
            this.radTime.TabStop = true;
            this.radTime.Text = "Date and time";
            this.radTime.UseVisualStyleBackColor = true;
            // 
            // radSeconds
            // 
            this.radSeconds.AutoSize = true;
            this.radSeconds.Location = new System.Drawing.Point(27, 67);
            this.radSeconds.Name = "radSeconds";
            this.radSeconds.Size = new System.Drawing.Size(77, 23);
            this.radSeconds.TabIndex = 1;
            this.radSeconds.TabStop = true;
            this.radSeconds.Text = "Seconds";
            this.radSeconds.UseVisualStyleBackColor = true;
            // 
            // radPoints
            // 
            this.radPoints.AutoSize = true;
            this.radPoints.Location = new System.Drawing.Point(27, 30);
            this.radPoints.Name = "radPoints";
            this.radPoints.Size = new System.Drawing.Size(98, 23);
            this.radPoints.TabIndex = 0;
            this.radPoints.TabStop = true;
            this.radPoints.Text = "Data points";
            this.radPoints.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(219, 190);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnAccept
            // 
            this.btnAccept.Location = new System.Drawing.Point(325, 190);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(100, 30);
            this.btnAccept.TabIndex = 6;
            this.btnAccept.Text = "&Accept";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // chkPower
            // 
            this.chkPower.AutoSize = true;
            this.chkPower.Location = new System.Drawing.Point(20, 88);
            this.chkPower.Name = "chkPower";
            this.chkPower.Size = new System.Drawing.Size(93, 23);
            this.chkPower.TabIndex = 7;
            this.chkPower.Text = "Power (dB)";
            this.chkPower.UseVisualStyleBackColor = true;
            // 
            // chkCumulative
            // 
            this.chkCumulative.AutoSize = true;
            this.chkCumulative.Location = new System.Drawing.Point(20, 117);
            this.chkCumulative.Name = "chkCumulative";
            this.chkCumulative.Size = new System.Drawing.Size(205, 23);
            this.chkCumulative.TabIndex = 8;
            this.chkCumulative.Text = "Cumulative fractal dimension";
            this.chkCumulative.UseVisualStyleBackColor = true;
            // 
            // chkEntropy
            // 
            this.chkEntropy.AutoSize = true;
            this.chkEntropy.Location = new System.Drawing.Point(20, 146);
            this.chkEntropy.Name = "chkEntropy";
            this.chkEntropy.Size = new System.Drawing.Size(226, 23);
            this.chkEntropy.TabIndex = 9;
            this.chkEntropy.Text = "Entropy (approximate && sample)";
            this.chkEntropy.UseVisualStyleBackColor = true;
            // 
            // chkCrossHair
            // 
            this.chkCrossHair.AutoSize = true;
            this.chkCrossHair.Location = new System.Drawing.Point(20, 175);
            this.chkCrossHair.Name = "chkCrossHair";
            this.chkCrossHair.Size = new System.Drawing.Size(156, 23);
            this.chkCrossHair.TabIndex = 10;
            this.chkCrossHair.Text = "Show plot\'s crosshair";
            this.chkCrossHair.UseVisualStyleBackColor = true;
            // 
            // FrmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(437, 232);
            this.Controls.Add(this.chkCrossHair);
            this.Controls.Add(this.chkEntropy);
            this.Controls.Add(this.chkCumulative);
            this.Controls.Add(this.chkPower);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.grpAxis);
            this.Controls.Add(this.txtEnd);
            this.Controls.Add(this.txtStart);
            this.Controls.Add(this.lblEnd);
            this.Controls.Add(this.lblStart);
            this.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FrmSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.grpAxis.ResumeLayout(false);
            this.grpAxis.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label lblStart;
        private Label lblEnd;
        private TextBox txtStart;
        private TextBox txtEnd;
        private GroupBox grpAxis;
        private RadioButton radTime;
        private RadioButton radSeconds;
        private RadioButton radPoints;
        private Button btnCancel;
        private Button btnAccept;
        private CheckBox chkPower;
        private CheckBox chkCumulative;
        private CheckBox chkEntropy;
        private CheckBox chkCrossHair;
    }
}