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
            btnCancel = new Button();
            btnAccept = new Button();
            tabSettings = new TabControl();
            tabPlot = new TabPage();
            lblEntropy = new Label();
            lblCumulative = new Label();
            grpFFT = new GroupBox();
            radDown = new RadioButton();
            radUp = new RadioButton();
            chkBoxplot = new CheckBox();
            chkCrossHair = new CheckBox();
            chkEntropy = new CheckBox();
            chkCumulative = new CheckBox();
            chkPower = new CheckBox();
            grpAxis = new GroupBox();
            radTime = new RadioButton();
            radSeconds = new RadioButton();
            radPoints = new RadioButton();
            txtEnd = new TextBox();
            txtStart = new TextBox();
            lblEnd = new Label();
            lblStart = new Label();
            tabDerivative = new TabPage();
            lblAlgorithms = new Label();
            cboAlgorithms = new ComboBox();
            chkComputeDerivative = new CheckBox();
            chkExportDerivative = new CheckBox();
            tabIntegration = new TabPage();
            chkAbsolute = new CheckBox();
            cboIntegration = new ComboBox();
            lblIntegration = new Label();
            chkExportIntegration = new CheckBox();
            chkComputeIntegration = new CheckBox();
            tabEntropy = new TabPage();
            cboEntropyAlgorithm = new ComboBox();
            lblEntropyAlgorithm = new Label();
            txtFactorR = new TextBox();
            txtFactorM = new TextBox();
            lblFactorM = new Label();
            lblFactorR = new Label();
            tabGUI = new TabPage();
            chkWindowPos = new CheckBox();
            txtDataFormat = new TextBox();
            lblDataFormat = new Label();
            grpCulture = new GroupBox();
            cboAllCultures = new ComboBox();
            radUserCulture = new RadioButton();
            radInvariantCulture = new RadioButton();
            radCurrentCulture = new RadioButton();
            chkDlgPath = new CheckBox();
            btnReset = new Button();
            tabSettings.SuspendLayout();
            tabPlot.SuspendLayout();
            grpFFT.SuspendLayout();
            grpAxis.SuspendLayout();
            tabDerivative.SuspendLayout();
            tabIntegration.SuspendLayout();
            tabEntropy.SuspendLayout();
            tabGUI.SuspendLayout();
            grpCulture.SuspendLayout();
            SuspendLayout();
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(282, 304);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(100, 30);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "&Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += Cancel_Click;
            // 
            // btnAccept
            // 
            btnAccept.Location = new Point(388, 304);
            btnAccept.Name = "btnAccept";
            btnAccept.Size = new Size(100, 30);
            btnAccept.TabIndex = 3;
            btnAccept.Text = "&Accept";
            btnAccept.UseVisualStyleBackColor = true;
            btnAccept.Click += Accept_Click;
            // 
            // tabSettings
            // 
            tabSettings.Controls.Add(tabPlot);
            tabSettings.Controls.Add(tabDerivative);
            tabSettings.Controls.Add(tabIntegration);
            tabSettings.Controls.Add(tabEntropy);
            tabSettings.Controls.Add(tabGUI);
            tabSettings.Location = new Point(12, 14);
            tabSettings.Name = "tabSettings";
            tabSettings.SelectedIndex = 0;
            tabSettings.Size = new Size(480, 284);
            tabSettings.TabIndex = 0;
            // 
            // tabPlot
            // 
            tabPlot.Controls.Add(lblEntropy);
            tabPlot.Controls.Add(lblCumulative);
            tabPlot.Controls.Add(grpFFT);
            tabPlot.Controls.Add(chkBoxplot);
            tabPlot.Controls.Add(chkCrossHair);
            tabPlot.Controls.Add(chkEntropy);
            tabPlot.Controls.Add(chkCumulative);
            tabPlot.Controls.Add(chkPower);
            tabPlot.Controls.Add(grpAxis);
            tabPlot.Controls.Add(txtEnd);
            tabPlot.Controls.Add(txtStart);
            tabPlot.Controls.Add(lblEnd);
            tabPlot.Controls.Add(lblStart);
            tabPlot.Location = new Point(4, 26);
            tabPlot.Name = "tabPlot";
            tabPlot.Padding = new Padding(3);
            tabPlot.Size = new Size(472, 254);
            tabPlot.TabIndex = 0;
            tabPlot.Text = "Plotting";
            tabPlot.UseVisualStyleBackColor = true;
            // 
            // lblEntropy
            // 
            lblEntropy.AutoSize = true;
            lblEntropy.Location = new Point(30, 190);
            lblEntropy.MaximumSize = new Size(240, 0);
            lblEntropy.Name = "lblEntropy";
            lblEntropy.Size = new Size(207, 19);
            lblEntropy.TabIndex = 18;
            lblEntropy.Text = "Entropy (approximate && sample)";
            // 
            // lblCumulative
            // 
            lblCumulative.AutoSize = true;
            lblCumulative.Location = new Point(30, 160);
            lblCumulative.MaximumSize = new Size(257, 0);
            lblCumulative.Name = "lblCumulative";
            lblCumulative.Size = new Size(186, 19);
            lblCumulative.TabIndex = 17;
            lblCumulative.Text = "Cumulative fractal dimension";
            // 
            // grpFFT
            // 
            grpFFT.Controls.Add(radDown);
            grpFFT.Controls.Add(radUp);
            grpFFT.Location = new Point(287, 143);
            grpFFT.Name = "grpFFT";
            grpFFT.Size = new Size(175, 95);
            grpFFT.TabIndex = 16;
            grpFFT.TabStop = false;
            grpFFT.Text = "FFT points";
            // 
            // radDown
            // 
            radDown.AutoSize = true;
            radDown.Location = new Point(13, 59);
            radDown.Name = "radDown";
            radDown.Size = new Size(142, 23);
            radDown.TabIndex = 1;
            radDown.TabStop = true;
            radDown.Text = "Round down (trim)";
            radDown.UseVisualStyleBackColor = true;
            // 
            // radUp
            // 
            radUp.AutoSize = true;
            radUp.Location = new Point(13, 27);
            radUp.Name = "radUp";
            radUp.Size = new Size(158, 23);
            radUp.TabIndex = 0;
            radUp.TabStop = true;
            radUp.Text = "Round up (add zeros)";
            radUp.UseVisualStyleBackColor = true;
            // 
            // chkBoxplot
            // 
            chkBoxplot.AutoSize = true;
            chkBoxplot.Location = new Point(14, 99);
            chkBoxplot.Name = "chkBoxplot";
            chkBoxplot.Size = new Size(78, 23);
            chkBoxplot.TabIndex = 2;
            chkBoxplot.Text = "Box plot";
            chkBoxplot.UseVisualStyleBackColor = true;
            // 
            // chkCrossHair
            // 
            chkCrossHair.AutoSize = true;
            chkCrossHair.Location = new Point(14, 219);
            chkCrossHair.Name = "chkCrossHair";
            chkCrossHair.Size = new Size(156, 23);
            chkCrossHair.TabIndex = 6;
            chkCrossHair.Text = "Show plot's crosshair";
            chkCrossHair.UseVisualStyleBackColor = true;
            // 
            // chkEntropy
            // 
            chkEntropy.AutoSize = true;
            chkEntropy.Location = new Point(14, 189);
            chkEntropy.Name = "chkEntropy";
            chkEntropy.Size = new Size(35, 23);
            chkEntropy.TabIndex = 5;
            chkEntropy.Text = "E";
            chkEntropy.UseVisualStyleBackColor = true;
            // 
            // chkCumulative
            // 
            chkCumulative.AutoSize = true;
            chkCumulative.Location = new Point(14, 159);
            chkCumulative.Name = "chkCumulative";
            chkCumulative.Size = new Size(37, 23);
            chkCumulative.TabIndex = 4;
            chkCumulative.Text = "C";
            chkCumulative.UseVisualStyleBackColor = true;
            // 
            // chkPower
            // 
            chkPower.AutoSize = true;
            chkPower.Location = new Point(14, 129);
            chkPower.Name = "chkPower";
            chkPower.Size = new Size(93, 23);
            chkPower.TabIndex = 3;
            chkPower.Text = "Power (dB)";
            chkPower.UseVisualStyleBackColor = true;
            // 
            // grpAxis
            // 
            grpAxis.Controls.Add(radTime);
            grpAxis.Controls.Add(radSeconds);
            grpAxis.Controls.Add(radPoints);
            grpAxis.Location = new Point(287, 12);
            grpAxis.Name = "grpAxis";
            grpAxis.Size = new Size(175, 125);
            grpAxis.TabIndex = 15;
            grpAxis.TabStop = false;
            grpAxis.Text = "Abscissa axis";
            // 
            // radTime
            // 
            radTime.AutoSize = true;
            radTime.Location = new Point(22, 88);
            radTime.Name = "radTime";
            radTime.Size = new Size(114, 23);
            radTime.TabIndex = 2;
            radTime.TabStop = true;
            radTime.Text = "Date and time";
            radTime.UseVisualStyleBackColor = true;
            // 
            // radSeconds
            // 
            radSeconds.AutoSize = true;
            radSeconds.Location = new Point(22, 56);
            radSeconds.Name = "radSeconds";
            radSeconds.Size = new Size(77, 23);
            radSeconds.TabIndex = 1;
            radSeconds.TabStop = true;
            radSeconds.Text = "Seconds";
            radSeconds.UseVisualStyleBackColor = true;
            // 
            // radPoints
            // 
            radPoints.AutoSize = true;
            radPoints.Location = new Point(22, 25);
            radPoints.Name = "radPoints";
            radPoints.Size = new Size(98, 23);
            radPoints.TabIndex = 0;
            radPoints.TabStop = true;
            radPoints.Text = "Data points";
            radPoints.UseVisualStyleBackColor = true;
            // 
            // txtEnd
            // 
            txtEnd.Location = new Point(171, 56);
            txtEnd.Name = "txtEnd";
            txtEnd.Size = new Size(71, 25);
            txtEnd.TabIndex = 1;
            // 
            // txtStart
            // 
            txtStart.Location = new Point(171, 17);
            txtStart.Name = "txtStart";
            txtStart.Size = new Size(71, 25);
            txtStart.TabIndex = 0;
            // 
            // lblEnd
            // 
            lblEnd.AutoSize = true;
            lblEnd.Location = new Point(10, 59);
            lblEnd.MaximumSize = new Size(170, 0);
            lblEnd.Name = "lblEnd";
            lblEnd.Size = new Size(108, 19);
            lblEnd.TabIndex = 12;
            lblEnd.Text = "Array index end:";
            // 
            // lblStart
            // 
            lblStart.AutoSize = true;
            lblStart.Location = new Point(10, 20);
            lblStart.MaximumSize = new Size(170, 0);
            lblStart.Name = "lblStart";
            lblStart.Size = new Size(113, 19);
            lblStart.TabIndex = 11;
            lblStart.Text = "Array index start:";
            // 
            // tabDerivative
            // 
            tabDerivative.Controls.Add(lblAlgorithms);
            tabDerivative.Controls.Add(cboAlgorithms);
            tabDerivative.Controls.Add(chkComputeDerivative);
            tabDerivative.Controls.Add(chkExportDerivative);
            tabDerivative.Location = new Point(4, 24);
            tabDerivative.Name = "tabDerivative";
            tabDerivative.Padding = new Padding(3);
            tabDerivative.Size = new Size(472, 256);
            tabDerivative.TabIndex = 2;
            tabDerivative.Text = "Derivative";
            tabDerivative.UseVisualStyleBackColor = true;
            // 
            // lblAlgorithms
            // 
            lblAlgorithms.AutoSize = true;
            lblAlgorithms.Location = new Point(63, 122);
            lblAlgorithms.Name = "lblAlgorithms";
            lblAlgorithms.Size = new Size(174, 19);
            lblAlgorithms.TabIndex = 3;
            lblAlgorithms.Text = "Finite difference algorithms";
            // 
            // cboAlgorithms
            // 
            cboAlgorithms.FormattingEnabled = true;
            cboAlgorithms.Location = new Point(63, 150);
            cboAlgorithms.Name = "cboAlgorithms";
            cboAlgorithms.Size = new Size(223, 25);
            cboAlgorithms.TabIndex = 2;
            cboAlgorithms.SelectionChangeCommitted += Differentiation_SelectionChangeCommitted;
            // 
            // chkComputeDerivative
            // 
            chkComputeDerivative.AutoSize = true;
            chkComputeDerivative.Location = new Point(27, 38);
            chkComputeDerivative.Name = "chkComputeDerivative";
            chkComputeDerivative.Size = new Size(217, 23);
            chkComputeDerivative.TabIndex = 0;
            chkComputeDerivative.Text = "Compute numerical derivative?";
            chkComputeDerivative.UseVisualStyleBackColor = true;
            chkComputeDerivative.CheckedChanged += ComputeDerivative_CheckedChanged;
            // 
            // chkExportDerivative
            // 
            chkExportDerivative.AutoSize = true;
            chkExportDerivative.Location = new Point(27, 78);
            chkExportDerivative.Name = "chkExportDerivative";
            chkExportDerivative.Size = new Size(167, 23);
            chkExportDerivative.TabIndex = 1;
            chkExportDerivative.Text = "Export derivative data?";
            chkExportDerivative.UseVisualStyleBackColor = true;
            // 
            // tabIntegration
            // 
            tabIntegration.Controls.Add(chkAbsolute);
            tabIntegration.Controls.Add(cboIntegration);
            tabIntegration.Controls.Add(lblIntegration);
            tabIntegration.Controls.Add(chkExportIntegration);
            tabIntegration.Controls.Add(chkComputeIntegration);
            tabIntegration.Location = new Point(4, 24);
            tabIntegration.Name = "tabIntegration";
            tabIntegration.Padding = new Padding(3);
            tabIntegration.Size = new Size(472, 256);
            tabIntegration.TabIndex = 3;
            tabIntegration.Text = "Integration";
            tabIntegration.UseVisualStyleBackColor = true;
            // 
            // chkAbsolute
            // 
            chkAbsolute.AutoSize = true;
            chkAbsolute.Location = new Point(27, 70);
            chkAbsolute.Name = "chkAbsolute";
            chkAbsolute.Size = new Size(259, 23);
            chkAbsolute.TabIndex = 1;
            chkAbsolute.Text = "Compute the absolute-value integral?";
            chkAbsolute.UseVisualStyleBackColor = true;
            // 
            // cboIntegration
            // 
            cboIntegration.FormattingEnabled = true;
            cboIntegration.Location = new Point(63, 170);
            cboIntegration.Name = "cboIntegration";
            cboIntegration.Size = new Size(223, 25);
            cboIntegration.TabIndex = 3;
            cboIntegration.SelectionChangeCommitted += Integration_SelectionChangeCommitted;
            // 
            // lblIntegration
            // 
            lblIntegration.AutoSize = true;
            lblIntegration.Location = new Point(63, 142);
            lblIntegration.Name = "lblIntegration";
            lblIntegration.Size = new Size(149, 19);
            lblIntegration.TabIndex = 4;
            lblIntegration.Text = "Quadrature algorithms";
            // 
            // chkExportIntegration
            // 
            chkExportIntegration.AutoSize = true;
            chkExportIntegration.Location = new Point(27, 104);
            chkExportIntegration.Name = "chkExportIntegration";
            chkExportIntegration.Size = new Size(207, 23);
            chkExportIntegration.TabIndex = 2;
            chkExportIntegration.Text = "Export numerical integration?";
            chkExportIntegration.UseVisualStyleBackColor = true;
            // 
            // chkComputeIntegration
            // 
            chkComputeIntegration.AutoSize = true;
            chkComputeIntegration.Location = new Point(27, 36);
            chkComputeIntegration.Name = "chkComputeIntegration";
            chkComputeIntegration.Size = new Size(225, 23);
            chkComputeIntegration.TabIndex = 0;
            chkComputeIntegration.Text = "Compute numerical integration?";
            chkComputeIntegration.UseVisualStyleBackColor = true;
            chkComputeIntegration.CheckedChanged += ComputeIntegration_CheckedChanged;
            // 
            // tabEntropy
            // 
            tabEntropy.Controls.Add(cboEntropyAlgorithm);
            tabEntropy.Controls.Add(lblEntropyAlgorithm);
            tabEntropy.Controls.Add(txtFactorR);
            tabEntropy.Controls.Add(txtFactorM);
            tabEntropy.Controls.Add(lblFactorM);
            tabEntropy.Controls.Add(lblFactorR);
            tabEntropy.Location = new Point(4, 24);
            tabEntropy.Name = "tabEntropy";
            tabEntropy.Padding = new Padding(3);
            tabEntropy.Size = new Size(472, 256);
            tabEntropy.TabIndex = 4;
            tabEntropy.Text = "Entropy";
            tabEntropy.UseVisualStyleBackColor = true;
            // 
            // cboEntropyAlgorithm
            // 
            cboEntropyAlgorithm.FormattingEnabled = true;
            cboEntropyAlgorithm.Location = new Point(131, 47);
            cboEntropyAlgorithm.Name = "cboEntropyAlgorithm";
            cboEntropyAlgorithm.Size = new Size(157, 25);
            cboEntropyAlgorithm.TabIndex = 5;
            cboEntropyAlgorithm.SelectionChangeCommitted += Entropy_SelectionChangeCommitted;
            // 
            // lblEntropyAlgorithm
            // 
            lblEntropyAlgorithm.AutoSize = true;
            lblEntropyAlgorithm.Location = new Point(54, 50);
            lblEntropyAlgorithm.Name = "lblEntropyAlgorithm";
            lblEntropyAlgorithm.Size = new Size(70, 19);
            lblEntropyAlgorithm.TabIndex = 4;
            lblEntropyAlgorithm.Text = "Algorithm";
            // 
            // txtFactorR
            // 
            txtFactorR.Location = new Point(131, 138);
            txtFactorR.Name = "txtFactorR";
            txtFactorR.Size = new Size(100, 25);
            txtFactorR.TabIndex = 3;
            // 
            // txtFactorM
            // 
            txtFactorM.Location = new Point(131, 94);
            txtFactorM.Name = "txtFactorM";
            txtFactorM.Size = new Size(100, 25);
            txtFactorM.TabIndex = 2;
            // 
            // lblFactorM
            // 
            lblFactorM.AutoSize = true;
            lblFactorM.Location = new Point(54, 97);
            lblFactorM.Name = "lblFactorM";
            lblFactorM.Size = new Size(61, 19);
            lblFactorM.TabIndex = 1;
            lblFactorM.Text = "M factor";
            // 
            // lblFactorR
            // 
            lblFactorR.AutoSize = true;
            lblFactorR.Location = new Point(54, 141);
            lblFactorR.Name = "lblFactorR";
            lblFactorR.Size = new Size(56, 19);
            lblFactorR.TabIndex = 0;
            lblFactorR.Text = "R factor";
            // 
            // tabGUI
            // 
            tabGUI.Controls.Add(chkWindowPos);
            tabGUI.Controls.Add(txtDataFormat);
            tabGUI.Controls.Add(lblDataFormat);
            tabGUI.Controls.Add(grpCulture);
            tabGUI.Controls.Add(chkDlgPath);
            tabGUI.Location = new Point(4, 26);
            tabGUI.Name = "tabGUI";
            tabGUI.Padding = new Padding(3);
            tabGUI.Size = new Size(472, 254);
            tabGUI.TabIndex = 1;
            tabGUI.Text = "User interface";
            tabGUI.UseVisualStyleBackColor = true;
            // 
            // chkWindowPos
            // 
            chkWindowPos.AutoSize = true;
            chkWindowPos.Location = new Point(23, 154);
            chkWindowPos.Name = "chkWindowPos";
            chkWindowPos.Size = new Size(319, 23);
            chkWindowPos.TabIndex = 4;
            chkWindowPos.Text = "Remember window size and position on startup";
            chkWindowPos.UseVisualStyleBackColor = true;
            // 
            // txtDataFormat
            // 
            txtDataFormat.Location = new Point(260, 214);
            txtDataFormat.Name = "txtDataFormat";
            txtDataFormat.Size = new Size(75, 25);
            txtDataFormat.TabIndex = 3;
            // 
            // lblDataFormat
            // 
            lblDataFormat.AutoSize = true;
            lblDataFormat.Location = new Point(19, 217);
            lblDataFormat.MaximumSize = new Size(320, 0);
            lblDataFormat.Name = "lblDataFormat";
            lblDataFormat.Size = new Size(201, 19);
            lblDataFormat.TabIndex = 2;
            lblDataFormat.Text = "Numeric data-formatting string";
            // 
            // grpCulture
            // 
            grpCulture.Controls.Add(cboAllCultures);
            grpCulture.Controls.Add(radUserCulture);
            grpCulture.Controls.Add(radInvariantCulture);
            grpCulture.Controls.Add(radCurrentCulture);
            grpCulture.Location = new Point(24, 14);
            grpCulture.Name = "grpCulture";
            grpCulture.Size = new Size(400, 130);
            grpCulture.TabIndex = 0;
            grpCulture.TabStop = false;
            grpCulture.Text = "UI and data format";
            // 
            // cboAllCultures
            // 
            cboAllCultures.AutoCompleteMode = AutoCompleteMode.Suggest;
            cboAllCultures.Enabled = false;
            cboAllCultures.FormattingEnabled = true;
            cboAllCultures.Location = new Point(171, 93);
            cboAllCultures.Name = "cboAllCultures";
            cboAllCultures.Size = new Size(190, 25);
            cboAllCultures.TabIndex = 2;
            cboAllCultures.SelectionChangeCommitted += AllCultures_SelectionChangeCommitted;
            // 
            // radUserCulture
            // 
            radUserCulture.AutoSize = true;
            radUserCulture.Location = new Point(18, 93);
            radUserCulture.Name = "radUserCulture";
            radUserCulture.Size = new Size(108, 23);
            radUserCulture.TabIndex = 3;
            radUserCulture.TabStop = true;
            radUserCulture.Text = "Select culture";
            radUserCulture.UseVisualStyleBackColor = true;
            radUserCulture.CheckedChanged += UserCulture_CheckedChanged;
            // 
            // radInvariantCulture
            // 
            radInvariantCulture.AutoSize = true;
            radInvariantCulture.Location = new Point(18, 60);
            radInvariantCulture.Name = "radInvariantCulture";
            radInvariantCulture.Size = new Size(196, 23);
            radInvariantCulture.TabIndex = 1;
            radInvariantCulture.TabStop = true;
            radInvariantCulture.Text = "Invariant culture formatting";
            radInvariantCulture.UseVisualStyleBackColor = true;
            radInvariantCulture.CheckedChanged += InvariantCulture_CheckedChanged;
            // 
            // radCurrentCulture
            // 
            radCurrentCulture.AutoSize = true;
            radCurrentCulture.Location = new Point(18, 28);
            radCurrentCulture.Name = "radCurrentCulture";
            radCurrentCulture.Size = new Size(189, 23);
            radCurrentCulture.TabIndex = 0;
            radCurrentCulture.TabStop = true;
            radCurrentCulture.Text = "Current culture formatting";
            radCurrentCulture.UseVisualStyleBackColor = true;
            radCurrentCulture.CheckedChanged += CurrentCulture_CheckedChanged;
            // 
            // chkDlgPath
            // 
            chkDlgPath.AutoSize = true;
            chkDlgPath.Location = new Point(23, 183);
            chkDlgPath.Name = "chkDlgPath";
            chkDlgPath.Size = new Size(290, 23);
            chkDlgPath.TabIndex = 1;
            chkDlgPath.Text = "Remember open/save dialog previous path";
            chkDlgPath.UseVisualStyleBackColor = true;
            // 
            // btnReset
            // 
            btnReset.Location = new Point(19, 304);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(100, 30);
            btnReset.TabIndex = 1;
            btnReset.Text = "&Reset";
            btnReset.UseVisualStyleBackColor = true;
            btnReset.Click += Reset_Click;
            // 
            // FrmSettings
            // 
            AcceptButton = btnAccept;
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(504, 346);
            Controls.Add(btnReset);
            Controls.Add(tabSettings);
            Controls.Add(btnAccept);
            Controls.Add(btnCancel);
            Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmSettings";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Settings";
            tabSettings.ResumeLayout(false);
            tabPlot.ResumeLayout(false);
            tabPlot.PerformLayout();
            grpFFT.ResumeLayout(false);
            grpFFT.PerformLayout();
            grpAxis.ResumeLayout(false);
            grpAxis.PerformLayout();
            tabDerivative.ResumeLayout(false);
            tabDerivative.PerformLayout();
            tabIntegration.ResumeLayout(false);
            tabIntegration.PerformLayout();
            tabEntropy.ResumeLayout(false);
            tabEntropy.PerformLayout();
            tabGUI.ResumeLayout(false);
            tabGUI.PerformLayout();
            grpCulture.ResumeLayout(false);
            grpCulture.PerformLayout();
            ResumeLayout(false);
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
        private TabPage tabDerivative;
        private CheckBox chkComputeDerivative;
        private CheckBox chkExportDerivative;
        private Label lblAlgorithms;
        private ComboBox cboAlgorithms;
        private TabPage tabIntegration;
        private ComboBox cboIntegration;
        private Label lblIntegration;
        private CheckBox chkExportIntegration;
        private CheckBox chkComputeIntegration;
        private CheckBox chkAbsolute;
        private CheckBox chkBoxplot;
        private GroupBox grpFFT;
        private RadioButton radDown;
        private RadioButton radUp;
        private Label lblEntropy;
        private Label lblCumulative;
        private TabPage tabEntropy;
        private Label lblFactorM;
        private Label lblFactorR;
        private ComboBox cboEntropyAlgorithm;
        private Label lblEntropyAlgorithm;
        private TextBox txtFactorR;
        private TextBox txtFactorM;
        private CheckBox chkWindowPos;
    }
}
