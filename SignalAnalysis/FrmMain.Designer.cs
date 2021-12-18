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
            this.plotFractal = new ScottPlot.FormsPlotCrossHair();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.plotFFT = new ScottPlot.FormsPlotCrossHair();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.plotOriginal = new ScottPlot.FormsPlotCrossHair();
            this.plotWindow = new ScottPlot.FormsPlotCrossHair();
            this.plotApplied = new ScottPlot.FormsPlotCrossHair();
            this.cboSeries = new System.Windows.Forms.ComboBox();
            this.lblSeries = new System.Windows.Forms.Label();
            this.lblWindow = new System.Windows.Forms.Label();
            this.cboWindow = new System.Windows.Forms.ComboBox();
            this.cmdExport = new System.Windows.Forms.Button();
            this.lblStats = new System.Windows.Forms.Label();
            this.btnSettings = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnData
            // 
            this.btnData.Location = new System.Drawing.Point(12, 12);
            this.btnData.Name = "btnData";
            this.btnData.Size = new System.Drawing.Size(122, 28);
            this.btnData.TabIndex = 0;
            this.btnData.Text = "Select data";
            this.btnData.UseVisualStyleBackColor = true;
            this.btnData.Click += new System.EventHandler(this.btnData_Click);
            // 
            // plotFractal
            // 
            this.plotFractal.BackColor = System.Drawing.Color.Transparent;
            this.plotFractal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotFractal.Location = new System.Drawing.Point(4, 205);
            this.plotFractal.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.plotFractal.Name = "plotFractal";
            this.plotFractal.Size = new System.Drawing.Size(852, 196);
            this.plotFractal.SnapToPoint = false;
            this.plotFractal.TabIndex = 3;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.plotFFT, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.plotFractal, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 92);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33445F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33444F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33111F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(860, 608);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // plotFFT
            // 
            this.plotFFT.BackColor = System.Drawing.Color.Transparent;
            this.plotFFT.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotFFT.Location = new System.Drawing.Point(4, 407);
            this.plotFFT.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.plotFFT.Name = "plotFFT";
            this.plotFFT.Size = new System.Drawing.Size(852, 198);
            this.plotFFT.SnapToPoint = false;
            this.plotFFT.TabIndex = 5;
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
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 240F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(860, 202);
            this.tableLayoutPanel2.TabIndex = 4;
            // 
            // plotOriginal
            // 
            this.plotOriginal.BackColor = System.Drawing.Color.Transparent;
            this.plotOriginal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotOriginal.Location = new System.Drawing.Point(4, 3);
            this.plotOriginal.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.plotOriginal.Name = "plotOriginal";
            this.plotOriginal.Size = new System.Drawing.Size(278, 196);
            this.plotOriginal.SnapToPoint = false;
            this.plotOriginal.TabIndex = 2;
            // 
            // plotWindow
            // 
            this.plotWindow.BackColor = System.Drawing.Color.Transparent;
            this.plotWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotWindow.Location = new System.Drawing.Point(290, 3);
            this.plotWindow.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.plotWindow.Name = "plotWindow";
            this.plotWindow.Size = new System.Drawing.Size(278, 196);
            this.plotWindow.SnapToPoint = false;
            this.plotWindow.TabIndex = 3;
            // 
            // plotApplied
            // 
            this.plotApplied.BackColor = System.Drawing.Color.Transparent;
            this.plotApplied.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotApplied.Location = new System.Drawing.Point(576, 3);
            this.plotApplied.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.plotApplied.Name = "plotApplied";
            this.plotApplied.Size = new System.Drawing.Size(280, 196);
            this.plotApplied.SnapToPoint = false;
            this.plotApplied.TabIndex = 4;
            // 
            // cboSeries
            // 
            this.cboSeries.FormattingEnabled = true;
            this.cboSeries.Location = new System.Drawing.Point(140, 55);
            this.cboSeries.Name = "cboSeries";
            this.cboSeries.Size = new System.Drawing.Size(150, 25);
            this.cboSeries.TabIndex = 5;
            this.cboSeries.SelectedIndexChanged += new System.EventHandler(this.cboSeries_SelectedIndexChanged);
            // 
            // lblSeries
            // 
            this.lblSeries.AutoSize = true;
            this.lblSeries.Location = new System.Drawing.Point(15, 56);
            this.lblSeries.Name = "lblSeries";
            this.lblSeries.Size = new System.Drawing.Size(82, 19);
            this.lblSeries.TabIndex = 6;
            this.lblSeries.Text = "Select series";
            // 
            // lblWindow
            // 
            this.lblWindow.AutoSize = true;
            this.lblWindow.Location = new System.Drawing.Point(316, 58);
            this.lblWindow.Name = "lblWindow";
            this.lblWindow.Size = new System.Drawing.Size(59, 19);
            this.lblWindow.TabIndex = 7;
            this.lblWindow.Text = "Window";
            // 
            // cboWindow
            // 
            this.cboWindow.FormattingEnabled = true;
            this.cboWindow.Location = new System.Drawing.Point(381, 55);
            this.cboWindow.Name = "cboWindow";
            this.cboWindow.Size = new System.Drawing.Size(150, 25);
            this.cboWindow.TabIndex = 8;
            this.cboWindow.SelectedIndexChanged += new System.EventHandler(this.cboWindow_SelectedIndexChanged);
            // 
            // cmdExport
            // 
            this.cmdExport.Location = new System.Drawing.Point(146, 12);
            this.cmdExport.Name = "cmdExport";
            this.cmdExport.Size = new System.Drawing.Size(95, 28);
            this.cmdExport.TabIndex = 11;
            this.cmdExport.Text = "Export data";
            this.cmdExport.UseVisualStyleBackColor = true;
            this.cmdExport.Click += new System.EventHandler(this.cmdExport_Click);
            // 
            // lblStats
            // 
            this.lblStats.AutoSize = true;
            this.lblStats.Location = new System.Drawing.Point(588, 58);
            this.lblStats.Name = "lblStats";
            this.lblStats.Size = new System.Drawing.Size(132, 19);
            this.lblStats.TabIndex = 16;
            this.lblStats.Text = "Descriptive statistics";
            // 
            // btnSettings
            // 
            this.btnSettings.Location = new System.Drawing.Point(260, 12);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(78, 28);
            this.btnSettings.TabIndex = 17;
            this.btnSettings.Text = "Settings";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 712);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.lblStats);
            this.Controls.Add(this.cmdExport);
            this.Controls.Add(this.cboWindow);
            this.Controls.Add(this.lblWindow);
            this.Controls.Add(this.lblSeries);
            this.Controls.Add(this.cboSeries);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.btnData);
            this.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Signal analysis";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FrmMain_KeyPress);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private Button btnData;
    private ScottPlot.FormsPlotCrossHair plotFractal;
    private TableLayoutPanel tableLayoutPanel1;
    private ScottPlot.FormsPlotCrossHair plotOriginal;
    private ComboBox cboSeries;
    private Label lblSeries;
    private Label lblWindow;
    private ComboBox cboWindow;
    private TableLayoutPanel tableLayoutPanel2;
    private ScottPlot.FormsPlotCrossHair plotWindow;
    private ScottPlot.FormsPlotCrossHair plotApplied;
    private ScottPlot.FormsPlotCrossHair plotFFT;
    private Button cmdExport;
    private Label lblStats;
    private Button btnSettings;
}
