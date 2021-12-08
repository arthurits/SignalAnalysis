namespace SignalAnalysis;

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
            this.plotFractal = new ScottPlot.FormsPlotCrossHair();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.plotOriginal = new ScottPlot.FormsPlotCrossHair();
            this.plotWindow = new ScottPlot.FormsPlotCrossHair();
            this.plotApplied = new ScottPlot.FormsPlotCrossHair();
            this.plotFFT = new ScottPlot.FormsPlotCrossHair();
            this.cboSeries = new System.Windows.Forms.ComboBox();
            this.lblSeries = new System.Windows.Forms.Label();
            this.lblWindow = new System.Windows.Forms.Label();
            this.cboWindow = new System.Windows.Forms.ComboBox();
            this.chkProgressive = new System.Windows.Forms.CheckBox();
            this.chkLog = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnData
            // 
            this.btnData.Location = new System.Drawing.Point(12, 12);
            this.btnData.Name = "btnData";
            this.btnData.Size = new System.Drawing.Size(87, 33);
            this.btnData.TabIndex = 0;
            this.btnData.Text = "Select data";
            this.btnData.UseVisualStyleBackColor = true;
            this.btnData.Click += new System.EventHandler(this.btnData_Click);
            // 
            // lblData
            // 
            this.lblData.AutoSize = true;
            this.lblData.Location = new System.Drawing.Point(105, 20);
            this.lblData.Name = "lblData";
            this.lblData.Size = new System.Drawing.Size(87, 19);
            this.lblData.TabIndex = 1;
            this.lblData.Text = "Data filepath";
            // 
            // plotFractal
            // 
            this.plotFractal.BackColor = System.Drawing.Color.Transparent;
            this.plotFractal.CrossHairColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.plotFractal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotFractal.HorizontalLine = false;
            this.plotFractal.Location = new System.Drawing.Point(4, 258);
            this.plotFractal.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.plotFractal.Name = "plotFractal";
            this.plotFractal.Size = new System.Drawing.Size(852, 249);
            this.plotFractal.SnapToPoint = false;
            this.plotFractal.TabIndex = 3;
            this.plotFractal.VerticalLine = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.plotFractal, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.plotFFT, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 84);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(860, 765);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.Controls.Add(this.plotOriginal, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.plotWindow, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.plotApplied, 2, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(860, 255);
            this.tableLayoutPanel2.TabIndex = 4;
            // 
            // plotOriginal
            // 
            this.plotOriginal.BackColor = System.Drawing.Color.Transparent;
            this.plotOriginal.CrossHairColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.plotOriginal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotOriginal.HorizontalLine = false;
            this.plotOriginal.Location = new System.Drawing.Point(4, 3);
            this.plotOriginal.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.plotOriginal.Name = "plotOriginal";
            this.plotOriginal.Size = new System.Drawing.Size(278, 249);
            this.plotOriginal.SnapToPoint = false;
            this.plotOriginal.TabIndex = 2;
            this.plotOriginal.VerticalLine = false;
            // 
            // plotWindow
            // 
            this.plotWindow.BackColor = System.Drawing.Color.Transparent;
            this.plotWindow.CrossHairColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.plotWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotWindow.HorizontalLine = false;
            this.plotWindow.Location = new System.Drawing.Point(290, 3);
            this.plotWindow.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.plotWindow.Name = "plotWindow";
            this.plotWindow.Size = new System.Drawing.Size(278, 249);
            this.plotWindow.SnapToPoint = false;
            this.plotWindow.TabIndex = 3;
            this.plotWindow.VerticalLine = false;
            // 
            // plotApplied
            // 
            this.plotApplied.BackColor = System.Drawing.Color.Transparent;
            this.plotApplied.CrossHairColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.plotApplied.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotApplied.HorizontalLine = false;
            this.plotApplied.Location = new System.Drawing.Point(576, 3);
            this.plotApplied.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.plotApplied.Name = "plotApplied";
            this.plotApplied.Size = new System.Drawing.Size(280, 249);
            this.plotApplied.SnapToPoint = false;
            this.plotApplied.TabIndex = 4;
            this.plotApplied.VerticalLine = false;
            // 
            // plotFFT
            // 
            this.plotFFT.BackColor = System.Drawing.Color.Transparent;
            this.plotFFT.CrossHairColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.plotFFT.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotFFT.HorizontalLine = false;
            this.plotFFT.Location = new System.Drawing.Point(4, 513);
            this.plotFFT.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.plotFFT.Name = "plotFFT";
            this.plotFFT.Size = new System.Drawing.Size(852, 249);
            this.plotFFT.SnapToPoint = false;
            this.plotFFT.TabIndex = 5;
            this.plotFFT.VerticalLine = false;
            // 
            // cboSeries
            // 
            this.cboSeries.FormattingEnabled = true;
            this.cboSeries.Location = new System.Drawing.Point(105, 53);
            this.cboSeries.Name = "cboSeries";
            this.cboSeries.Size = new System.Drawing.Size(156, 25);
            this.cboSeries.TabIndex = 5;
            this.cboSeries.SelectedIndexChanged += new System.EventHandler(this.cboSeries_SelectedIndexChanged);
            // 
            // lblSeries
            // 
            this.lblSeries.AutoSize = true;
            this.lblSeries.Location = new System.Drawing.Point(12, 56);
            this.lblSeries.Name = "lblSeries";
            this.lblSeries.Size = new System.Drawing.Size(82, 19);
            this.lblSeries.TabIndex = 6;
            this.lblSeries.Text = "Select series";
            // 
            // lblWindow
            // 
            this.lblWindow.AutoSize = true;
            this.lblWindow.Location = new System.Drawing.Point(340, 56);
            this.lblWindow.Name = "lblWindow";
            this.lblWindow.Size = new System.Drawing.Size(59, 19);
            this.lblWindow.TabIndex = 7;
            this.lblWindow.Text = "Window";
            // 
            // cboWindow
            // 
            this.cboWindow.FormattingEnabled = true;
            this.cboWindow.Location = new System.Drawing.Point(405, 53);
            this.cboWindow.Name = "cboWindow";
            this.cboWindow.Size = new System.Drawing.Size(155, 25);
            this.cboWindow.TabIndex = 8;
            this.cboWindow.SelectedIndexChanged += new System.EventHandler(this.cboWindow_SelectedIndexChanged);
            // 
            // chkProgressive
            // 
            this.chkProgressive.AutoSize = true;
            this.chkProgressive.Location = new System.Drawing.Point(702, 52);
            this.chkProgressive.Name = "chkProgressive";
            this.chkProgressive.Size = new System.Drawing.Size(166, 23);
            this.chkProgressive.TabIndex = 9;
            this.chkProgressive.Text = "Progressive fractal dim";
            this.chkProgressive.UseVisualStyleBackColor = true;
            this.chkProgressive.CheckedChanged += new System.EventHandler(this.chkProgressive_CheckedChanged);
            // 
            // chkLog
            // 
            this.chkLog.AutoSize = true;
            this.chkLog.Location = new System.Drawing.Point(602, 52);
            this.chkLog.Name = "chkLog";
            this.chkLog.Size = new System.Drawing.Size(93, 23);
            this.chkLog.TabIndex = 10;
            this.chkLog.Text = "Power (dB)";
            this.chkLog.UseVisualStyleBackColor = true;
            this.chkLog.CheckedChanged += new System.EventHandler(this.chkLog_CheckedChanged);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 861);
            this.Controls.Add(this.chkLog);
            this.Controls.Add(this.chkProgressive);
            this.Controls.Add(this.cboWindow);
            this.Controls.Add(this.lblWindow);
            this.Controls.Add(this.lblSeries);
            this.Controls.Add(this.cboSeries);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.lblData);
            this.Controls.Add(this.btnData);
            this.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Signal analysis";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private Button btnData;
    private Label lblData;
    private ScottPlot.FormsPlotCrossHair plotFractal;
    private TableLayoutPanel tableLayoutPanel1;
    private ScottPlot.FormsPlotCrossHair plotOriginal;
    private ComboBox cboSeries;
    private Label lblSeries;
    private Label lblWindow;
    private ComboBox cboWindow;
    private CheckBox chkProgressive;
    private TableLayoutPanel tableLayoutPanel2;
    private ScottPlot.FormsPlotCrossHair plotWindow;
    private ScottPlot.FormsPlotCrossHair plotApplied;
    private ScottPlot.FormsPlotCrossHair plotFFT;
    private CheckBox chkLog;
}
