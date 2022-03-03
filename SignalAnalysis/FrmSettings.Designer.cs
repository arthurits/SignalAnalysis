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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAccept = new System.Windows.Forms.Button();
            this.tabSettings = new System.Windows.Forms.TabControl();
            this.tabPlot = new System.Windows.Forms.TabPage();
            this.chkCrossHair = new System.Windows.Forms.CheckBox();
            this.chkEntropy = new System.Windows.Forms.CheckBox();
            this.chkCumulative = new System.Windows.Forms.CheckBox();
            this.chkPower = new System.Windows.Forms.CheckBox();
            this.grpAxis = new System.Windows.Forms.GroupBox();
            this.radTime = new System.Windows.Forms.RadioButton();
            this.radSeconds = new System.Windows.Forms.RadioButton();
            this.radPoints = new System.Windows.Forms.RadioButton();
            this.txtEnd = new System.Windows.Forms.TextBox();
            this.txtStart = new System.Windows.Forms.TextBox();
            this.lblEnd = new System.Windows.Forms.Label();
            this.lblStart = new System.Windows.Forms.Label();
            this.tabGUI = new System.Windows.Forms.TabPage();
            this.txtDataFormat = new System.Windows.Forms.TextBox();
            this.lblDataFormat = new System.Windows.Forms.Label();
            this.grpCulture = new System.Windows.Forms.GroupBox();
            this.cboAllCultures = new System.Windows.Forms.ComboBox();
            this.radUserCulture = new System.Windows.Forms.RadioButton();
            this.radInvariantCulture = new System.Windows.Forms.RadioButton();
            this.radCurrentCulture = new System.Windows.Forms.RadioButton();
            this.chkDlgPath = new System.Windows.Forms.CheckBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.tabSettings.SuspendLayout();
            this.tabPlot.SuspendLayout();
            this.grpAxis.SuspendLayout();
            this.tabGUI.SuspendLayout();
            this.grpCulture.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(255, 263);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // btnAccept
            // 
            this.btnAccept.Location = new System.Drawing.Point(361, 263);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(100, 30);
            this.btnAccept.TabIndex = 6;
            this.btnAccept.Text = "&Accept";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.Accept_Click);
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.tabPlot);
            this.tabSettings.Controls.Add(this.tabGUI);
            this.tabSettings.Location = new System.Drawing.Point(12, 14);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.SelectedIndex = 0;
            this.tabSettings.Size = new System.Drawing.Size(449, 243);
            this.tabSettings.TabIndex = 11;
            // 
            // tabPlot
            // 
            this.tabPlot.Controls.Add(this.chkCrossHair);
            this.tabPlot.Controls.Add(this.chkEntropy);
            this.tabPlot.Controls.Add(this.chkCumulative);
            this.tabPlot.Controls.Add(this.chkPower);
            this.tabPlot.Controls.Add(this.grpAxis);
            this.tabPlot.Controls.Add(this.txtEnd);
            this.tabPlot.Controls.Add(this.txtStart);
            this.tabPlot.Controls.Add(this.lblEnd);
            this.tabPlot.Controls.Add(this.lblStart);
            this.tabPlot.Location = new System.Drawing.Point(4, 26);
            this.tabPlot.Name = "tabPlot";
            this.tabPlot.Padding = new System.Windows.Forms.Padding(3);
            this.tabPlot.Size = new System.Drawing.Size(441, 213);
            this.tabPlot.TabIndex = 0;
            this.tabPlot.Text = "Plotting";
            this.tabPlot.UseVisualStyleBackColor = true;
            // 
            // chkCrossHair
            // 
            this.chkCrossHair.AutoSize = true;
            this.chkCrossHair.Location = new System.Drawing.Point(17, 173);
            this.chkCrossHair.Name = "chkCrossHair";
            this.chkCrossHair.Size = new System.Drawing.Size(156, 23);
            this.chkCrossHair.TabIndex = 19;
            this.chkCrossHair.Text = "Show plot\'s crosshair";
            this.chkCrossHair.UseVisualStyleBackColor = true;
            // 
            // chkEntropy
            // 
            this.chkEntropy.AutoSize = true;
            this.chkEntropy.Location = new System.Drawing.Point(17, 144);
            this.chkEntropy.Name = "chkEntropy";
            this.chkEntropy.Size = new System.Drawing.Size(226, 23);
            this.chkEntropy.TabIndex = 18;
            this.chkEntropy.Text = "Entropy (approximate && sample)";
            this.chkEntropy.UseVisualStyleBackColor = true;
            // 
            // chkCumulative
            // 
            this.chkCumulative.AutoSize = true;
            this.chkCumulative.Location = new System.Drawing.Point(17, 115);
            this.chkCumulative.Name = "chkCumulative";
            this.chkCumulative.Size = new System.Drawing.Size(205, 23);
            this.chkCumulative.TabIndex = 17;
            this.chkCumulative.Text = "Cumulative fractal dimension";
            this.chkCumulative.UseVisualStyleBackColor = true;
            // 
            // chkPower
            // 
            this.chkPower.AutoSize = true;
            this.chkPower.Location = new System.Drawing.Point(17, 86);
            this.chkPower.Name = "chkPower";
            this.chkPower.Size = new System.Drawing.Size(93, 23);
            this.chkPower.TabIndex = 16;
            this.chkPower.Text = "Power (dB)";
            this.chkPower.UseVisualStyleBackColor = true;
            // 
            // grpAxis
            // 
            this.grpAxis.Controls.Add(this.radTime);
            this.grpAxis.Controls.Add(this.radSeconds);
            this.grpAxis.Controls.Add(this.radPoints);
            this.grpAxis.Location = new System.Drawing.Point(261, 12);
            this.grpAxis.Name = "grpAxis";
            this.grpAxis.Size = new System.Drawing.Size(162, 146);
            this.grpAxis.TabIndex = 15;
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
            // txtEnd
            // 
            this.txtEnd.Location = new System.Drawing.Point(132, 50);
            this.txtEnd.Name = "txtEnd";
            this.txtEnd.Size = new System.Drawing.Size(84, 25);
            this.txtEnd.TabIndex = 14;
            // 
            // txtStart
            // 
            this.txtStart.Location = new System.Drawing.Point(132, 16);
            this.txtStart.Name = "txtStart";
            this.txtStart.Size = new System.Drawing.Size(84, 25);
            this.txtStart.TabIndex = 13;
            // 
            // lblEnd
            // 
            this.lblEnd.AutoSize = true;
            this.lblEnd.Location = new System.Drawing.Point(17, 53);
            this.lblEnd.Name = "lblEnd";
            this.lblEnd.Size = new System.Drawing.Size(108, 19);
            this.lblEnd.TabIndex = 12;
            this.lblEnd.Text = "Array index end:";
            // 
            // lblStart
            // 
            this.lblStart.AutoSize = true;
            this.lblStart.Location = new System.Drawing.Point(17, 19);
            this.lblStart.Name = "lblStart";
            this.lblStart.Size = new System.Drawing.Size(113, 19);
            this.lblStart.TabIndex = 11;
            this.lblStart.Text = "Array index start:";
            // 
            // tabGUI
            // 
            this.tabGUI.Controls.Add(this.txtDataFormat);
            this.tabGUI.Controls.Add(this.lblDataFormat);
            this.tabGUI.Controls.Add(this.grpCulture);
            this.tabGUI.Controls.Add(this.chkDlgPath);
            this.tabGUI.Location = new System.Drawing.Point(4, 24);
            this.tabGUI.Name = "tabGUI";
            this.tabGUI.Padding = new System.Windows.Forms.Padding(3);
            this.tabGUI.Size = new System.Drawing.Size(441, 215);
            this.tabGUI.TabIndex = 1;
            this.tabGUI.Text = "User interface";
            this.tabGUI.UseVisualStyleBackColor = true;
            // 
            // txtDataFormat
            // 
            this.txtDataFormat.Location = new System.Drawing.Point(219, 174);
            this.txtDataFormat.Name = "txtDataFormat";
            this.txtDataFormat.Size = new System.Drawing.Size(89, 25);
            this.txtDataFormat.TabIndex = 3;
            // 
            // lblDataFormat
            // 
            this.lblDataFormat.AutoSize = true;
            this.lblDataFormat.Location = new System.Drawing.Point(18, 177);
            this.lblDataFormat.Name = "lblDataFormat";
            this.lblDataFormat.Size = new System.Drawing.Size(201, 19);
            this.lblDataFormat.TabIndex = 2;
            this.lblDataFormat.Text = "Numeric data-formatting string";
            // 
            // grpCulture
            // 
            this.grpCulture.Controls.Add(this.cboAllCultures);
            this.grpCulture.Controls.Add(this.radUserCulture);
            this.grpCulture.Controls.Add(this.radInvariantCulture);
            this.grpCulture.Controls.Add(this.radCurrentCulture);
            this.grpCulture.Location = new System.Drawing.Point(21, 14);
            this.grpCulture.Name = "grpCulture";
            this.grpCulture.Size = new System.Drawing.Size(288, 124);
            this.grpCulture.TabIndex = 1;
            this.grpCulture.TabStop = false;
            this.grpCulture.Text = "UI and data format";
            // 
            // cboAllCultures
            // 
            this.cboAllCultures.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboAllCultures.Enabled = false;
            this.cboAllCultures.FormattingEnabled = true;
            this.cboAllCultures.Location = new System.Drawing.Point(135, 84);
            this.cboAllCultures.Name = "cboAllCultures";
            this.cboAllCultures.Size = new System.Drawing.Size(136, 25);
            this.cboAllCultures.TabIndex = 3;
            this.cboAllCultures.SelectedValueChanged += new System.EventHandler(this.AllCultures_SelectedValueChanged);
            // 
            // radUserCulture
            // 
            this.radUserCulture.AutoSize = true;
            this.radUserCulture.Location = new System.Drawing.Point(21, 86);
            this.radUserCulture.Name = "radUserCulture";
            this.radUserCulture.Size = new System.Drawing.Size(108, 23);
            this.radUserCulture.TabIndex = 2;
            this.radUserCulture.TabStop = true;
            this.radUserCulture.Text = "Select culture";
            this.radUserCulture.UseVisualStyleBackColor = true;
            this.radUserCulture.CheckedChanged += new System.EventHandler(this.UserCulture_CheckedChanged);
            // 
            // radInvariantCulture
            // 
            this.radInvariantCulture.AutoSize = true;
            this.radInvariantCulture.Location = new System.Drawing.Point(21, 56);
            this.radInvariantCulture.Name = "radInvariantCulture";
            this.radInvariantCulture.Size = new System.Drawing.Size(196, 23);
            this.radInvariantCulture.TabIndex = 1;
            this.radInvariantCulture.TabStop = true;
            this.radInvariantCulture.Text = "Invariant culture formatting";
            this.radInvariantCulture.UseVisualStyleBackColor = true;
            this.radInvariantCulture.CheckedChanged += new System.EventHandler(this.InvariantCulture_CheckedChanged);
            // 
            // radCurrentCulture
            // 
            this.radCurrentCulture.AutoSize = true;
            this.radCurrentCulture.Location = new System.Drawing.Point(21, 26);
            this.radCurrentCulture.Name = "radCurrentCulture";
            this.radCurrentCulture.Size = new System.Drawing.Size(189, 23);
            this.radCurrentCulture.TabIndex = 0;
            this.radCurrentCulture.TabStop = true;
            this.radCurrentCulture.Text = "Current culture formatting";
            this.radCurrentCulture.UseVisualStyleBackColor = true;
            this.radCurrentCulture.CheckedChanged += new System.EventHandler(this.CurrentCulture_CheckedChanged);
            // 
            // chkDlgPath
            // 
            this.chkDlgPath.AutoSize = true;
            this.chkDlgPath.Location = new System.Drawing.Point(18, 144);
            this.chkDlgPath.Name = "chkDlgPath";
            this.chkDlgPath.Size = new System.Drawing.Size(290, 23);
            this.chkDlgPath.TabIndex = 0;
            this.chkDlgPath.Text = "Remember open/save dialog previous path";
            this.chkDlgPath.UseVisualStyleBackColor = true;
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(12, 263);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(100, 30);
            this.btnReset.TabIndex = 12;
            this.btnReset.Text = "&Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.Reset_Click);
            // 
            // FrmSettings
            // 
            this.AcceptButton = this.btnAccept;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(473, 305);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.tabSettings);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.btnCancel);
            this.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FrmSettings";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.tabSettings.ResumeLayout(false);
            this.tabPlot.ResumeLayout(false);
            this.tabPlot.PerformLayout();
            this.grpAxis.ResumeLayout(false);
            this.grpAxis.PerformLayout();
            this.tabGUI.ResumeLayout(false);
            this.tabGUI.PerformLayout();
            this.grpCulture.ResumeLayout(false);
            this.grpCulture.PerformLayout();
            this.ResumeLayout(false);

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
        private TabControl tabSettings;
        private TabPage tabPlot;
        private TabPage tabGUI;
        private GroupBox grpCulture;
        private RadioButton radInvariantCulture;
        private RadioButton radCurrentCulture;
        private CheckBox chkDlgPath;
        private TextBox txtDataFormat;
        private Label lblDataFormat;
        private Button btnReset;
        private ComboBox cboAllCultures;
        private RadioButton radUserCulture;
    }
}