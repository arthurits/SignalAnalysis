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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.plotFFT = new ScottPlot.FormsPlotCrossHair();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.plotOriginal = new ScottPlot.FormsPlotCrossHair();
            this.plotWindow = new ScottPlot.FormsPlotCrossHair();
            this.plotApplied = new ScottPlot.FormsPlotCrossHair();
            this.txtStats = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.plotFractal = new ScottPlot.FormsPlotCrossHair();
            this.plotFractalDistribution = new ScottPlot.FormsPlotCrossHair();
            this.tspTop = new System.Windows.Forms.ToolStripPanel();
            this.tspBottom = new System.Windows.Forms.ToolStripPanel();
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.toolStripMain_Exit = new System.Windows.Forms.ToolStripButton();
            this.toolStripMain_Open = new System.Windows.Forms.ToolStripButton();
            this.toolStripMain_Export = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.stripComboSeries = new System.Windows.Forms.ToolStripComboBox();
            this.stripComboWindows = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMain_Settings = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMain_About = new System.Windows.Forms.ToolStripButton();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusStripLabelEmpty = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStripLabelCulture = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStripLabelExPower = new System.Windows.Forms.ToolStripStatusLabelEx();
            this.statusStripLabelExCumulative = new System.Windows.Forms.ToolStripStatusLabelEx();
            this.statusStripLabelExEntropy = new System.Windows.Forms.ToolStripStatusLabelEx();
            this.statusStripLabelExCrossHair = new System.Windows.Forms.ToolStripStatusLabelEx();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.toolStripMain.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.plotFFT, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtStats, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 74);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(860, 620);
            this.tableLayoutPanel1.TabIndex = 4;
            this.tableLayoutPanel1.TabStop = true;
            // 
            // plotFFT
            // 
            this.plotFFT.BackColor = System.Drawing.Color.Transparent;
            this.plotFFT.CrossHairColor = System.Drawing.Color.Red;
            this.plotFFT.CultureUI = new System.Globalization.CultureInfo("en-US");
            this.plotFFT.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotFFT.Location = new System.Drawing.Point(4, 349);
            this.plotFFT.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.plotFFT.Name = "plotFFT";
            this.plotFFT.ShowCrossHair = false;
            this.plotFFT.ShowCrossHairHorizontal = false;
            this.plotFFT.ShowCrossHairVertical = false;
            this.plotFFT.Size = new System.Drawing.Size(852, 167);
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
            this.tableLayoutPanel2.Size = new System.Drawing.Size(860, 173);
            this.tableLayoutPanel2.TabIndex = 4;
            // 
            // plotOriginal
            // 
            this.plotOriginal.BackColor = System.Drawing.Color.Transparent;
            this.plotOriginal.CrossHairColor = System.Drawing.Color.Red;
            this.plotOriginal.CultureUI = new System.Globalization.CultureInfo("en-US");
            this.plotOriginal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotOriginal.Location = new System.Drawing.Point(4, 3);
            this.plotOriginal.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.plotOriginal.Name = "plotOriginal";
            this.plotOriginal.ShowCrossHair = false;
            this.plotOriginal.ShowCrossHairHorizontal = false;
            this.plotOriginal.ShowCrossHairVertical = false;
            this.plotOriginal.Size = new System.Drawing.Size(278, 167);
            this.plotOriginal.SnapToPoint = false;
            this.plotOriginal.TabIndex = 2;
            // 
            // plotWindow
            // 
            this.plotWindow.BackColor = System.Drawing.Color.Transparent;
            this.plotWindow.CrossHairColor = System.Drawing.Color.Red;
            this.plotWindow.CultureUI = new System.Globalization.CultureInfo("en-US");
            this.plotWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotWindow.Location = new System.Drawing.Point(290, 3);
            this.plotWindow.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.plotWindow.Name = "plotWindow";
            this.plotWindow.ShowCrossHair = false;
            this.plotWindow.ShowCrossHairHorizontal = false;
            this.plotWindow.ShowCrossHairVertical = false;
            this.plotWindow.Size = new System.Drawing.Size(278, 167);
            this.plotWindow.SnapToPoint = false;
            this.plotWindow.TabIndex = 3;
            // 
            // plotApplied
            // 
            this.plotApplied.BackColor = System.Drawing.Color.Transparent;
            this.plotApplied.CrossHairColor = System.Drawing.Color.Red;
            this.plotApplied.CultureUI = new System.Globalization.CultureInfo("en-US");
            this.plotApplied.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotApplied.Location = new System.Drawing.Point(576, 3);
            this.plotApplied.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.plotApplied.Name = "plotApplied";
            this.plotApplied.ShowCrossHair = false;
            this.plotApplied.ShowCrossHairHorizontal = false;
            this.plotApplied.ShowCrossHairVertical = false;
            this.plotApplied.Size = new System.Drawing.Size(280, 167);
            this.plotApplied.SnapToPoint = false;
            this.plotApplied.TabIndex = 4;
            // 
            // txtStats
            // 
            this.txtStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtStats.Location = new System.Drawing.Point(3, 522);
            this.txtStats.Multiline = true;
            this.txtStats.Name = "txtStats";
            this.txtStats.ReadOnly = true;
            this.txtStats.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStats.Size = new System.Drawing.Size(854, 95);
            this.txtStats.TabIndex = 6;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66.66666F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.Controls.Add(this.plotFractal, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.plotFractalDistribution, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 173);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(860, 173);
            this.tableLayoutPanel3.TabIndex = 7;
            // 
            // plotFractal
            // 
            this.plotFractal.BackColor = System.Drawing.Color.Transparent;
            this.plotFractal.CrossHairColor = System.Drawing.Color.Red;
            this.plotFractal.CultureUI = new System.Globalization.CultureInfo("en-US");
            this.plotFractal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotFractal.Location = new System.Drawing.Point(4, 3);
            this.plotFractal.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.plotFractal.Name = "plotFractal";
            this.plotFractal.ShowCrossHair = false;
            this.plotFractal.ShowCrossHairHorizontal = false;
            this.plotFractal.ShowCrossHairVertical = false;
            this.plotFractal.Size = new System.Drawing.Size(565, 167);
            this.plotFractal.SnapToPoint = false;
            this.plotFractal.TabIndex = 4;
            // 
            // plotFractalDistribution
            // 
            this.plotFractalDistribution.CrossHairColor = System.Drawing.Color.Red;
            this.plotFractalDistribution.CultureUI = new System.Globalization.CultureInfo("en-US");
            this.plotFractalDistribution.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotFractalDistribution.Location = new System.Drawing.Point(577, 3);
            this.plotFractalDistribution.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.plotFractalDistribution.Name = "plotFractalDistribution";
            this.plotFractalDistribution.ShowCrossHair = false;
            this.plotFractalDistribution.ShowCrossHairHorizontal = false;
            this.plotFractalDistribution.ShowCrossHairVertical = false;
            this.plotFractalDistribution.Size = new System.Drawing.Size(279, 167);
            this.plotFractalDistribution.SnapToPoint = false;
            this.plotFractalDistribution.TabIndex = 5;
            // 
            // tspTop
            // 
            this.tspTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.tspTop.Location = new System.Drawing.Point(0, 0);
            this.tspTop.Name = "tspTop";
            this.tspTop.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.tspTop.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.tspTop.Size = new System.Drawing.Size(884, 0);
            // 
            // tspBottom
            // 
            this.tspBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tspBottom.Location = new System.Drawing.Point(0, 721);
            this.tspBottom.Name = "tspBottom";
            this.tspBottom.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.tspBottom.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.tspBottom.Size = new System.Drawing.Size(884, 0);
            // 
            // toolStripMain
            // 
            this.toolStripMain.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.toolStripMain.ImageScalingSize = new System.Drawing.Size(48, 48);
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMain_Exit,
            this.toolStripMain_Open,
            this.toolStripMain_Export,
            this.toolStripSeparator1,
            this.stripComboSeries,
            this.stripComboWindows,
            this.toolStripSeparator2,
            this.toolStripMain_Settings,
            this.toolStripSeparator3,
            this.toolStripMain_About});
            this.toolStripMain.Location = new System.Drawing.Point(0, 0);
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStripMain.Size = new System.Drawing.Size(884, 27);
            this.toolStripMain.TabIndex = 2;
            this.toolStripMain.Text = "Main toolbar";
            // 
            // toolStripMain_Exit
            // 
            this.toolStripMain_Exit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripMain_Exit.Name = "toolStripMain_Exit";
            this.toolStripMain_Exit.Size = new System.Drawing.Size(34, 24);
            this.toolStripMain_Exit.Text = "Exit";
            this.toolStripMain_Exit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripMain_Exit.ToolTipText = "Export data and data analysis";
            this.toolStripMain_Exit.Click += new System.EventHandler(this.Exit_Click);
            // 
            // toolStripMain_Open
            // 
            this.toolStripMain_Open.Name = "toolStripMain_Open";
            this.toolStripMain_Open.Size = new System.Drawing.Size(47, 24);
            this.toolStripMain_Open.Text = "Open";
            this.toolStripMain_Open.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripMain_Open.Click += new System.EventHandler(this.Open_Click);
            // 
            // toolStripMain_Export
            // 
            this.toolStripMain_Export.Name = "toolStripMain_Export";
            this.toolStripMain_Export.Size = new System.Drawing.Size(52, 24);
            this.toolStripMain_Export.Text = "Export";
            this.toolStripMain_Export.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripMain_Export.Click += new System.EventHandler(this.Export_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // stripComboSeries
            // 
            this.stripComboSeries.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.stripComboSeries.DropDownHeight = 110;
            this.stripComboSeries.DropDownWidth = 122;
            this.stripComboSeries.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.stripComboSeries.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.stripComboSeries.IntegralHeight = false;
            this.stripComboSeries.MaxDropDownItems = 9;
            this.stripComboSeries.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this.stripComboSeries.Name = "stripComboSeries";
            this.stripComboSeries.Size = new System.Drawing.Size(120, 27);
            this.stripComboSeries.ToolTipText = "Select data series";
            this.stripComboSeries.SelectedIndexChanged += new System.EventHandler(this.ComboSeries_SelectedIndexChanged);
            // 
            // stripComboWindows
            // 
            this.stripComboWindows.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.stripComboWindows.DropDownHeight = 110;
            this.stripComboWindows.DropDownWidth = 122;
            this.stripComboWindows.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.stripComboWindows.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.stripComboWindows.IntegralHeight = false;
            this.stripComboWindows.MaxDropDownItems = 9;
            this.stripComboWindows.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this.stripComboWindows.Name = "stripComboWindows";
            this.stripComboWindows.Size = new System.Drawing.Size(120, 27);
            this.stripComboWindows.ToolTipText = "Select FFT window";
            this.stripComboWindows.SelectedIndexChanged += new System.EventHandler(this.ComboWindow_SelectedIndexChanged);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripMain_Settings
            // 
            this.toolStripMain_Settings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripMain_Settings.Name = "toolStripMain_Settings";
            this.toolStripMain_Settings.Size = new System.Drawing.Size(62, 24);
            this.toolStripMain_Settings.Text = "Settings";
            this.toolStripMain_Settings.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripMain_Settings.ToolTipText = "Settings for plots, data, and UI";
            this.toolStripMain_Settings.Click += new System.EventHandler(this.Settings_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripMain_About
            // 
            this.toolStripMain_About.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripMain_About.Name = "toolStripMain_About";
            this.toolStripMain_About.Size = new System.Drawing.Size(51, 24);
            this.toolStripMain_About.Text = "About";
            this.toolStripMain_About.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripMain_About.ToolTipText = "About this software";
            this.toolStripMain_About.Click += new System.EventHandler(this.About_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusStripLabelEmpty,
            this.statusStripLabelCulture,
            this.statusStripLabelExPower,
            this.statusStripLabelExCumulative,
            this.statusStripLabelExEntropy,
            this.statusStripLabelExCrossHair});
            this.statusStrip.Location = new System.Drawing.Point(0, 693);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.statusStrip.ShowItemToolTips = true;
            this.statusStrip.Size = new System.Drawing.Size(884, 28);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "Status bar";
            // 
            // statusStripLabelEmpty
            // 
            this.statusStripLabelEmpty.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.statusStripLabelEmpty.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.statusStripLabelEmpty.Name = "statusStripLabelEmpty";
            this.statusStripLabelEmpty.Size = new System.Drawing.Size(697, 23);
            this.statusStripLabelEmpty.Spring = true;
            this.statusStripLabelEmpty.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusStripLabelCulture
            // 
            this.statusStripLabelCulture.AutoSize = false;
            this.statusStripLabelCulture.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.statusStripLabelCulture.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.statusStripLabelCulture.Name = "statusStripLabelCulture";
            this.statusStripLabelCulture.Size = new System.Drawing.Size(60, 23);
            this.statusStripLabelCulture.ToolTipText = "User interface language";
            this.statusStripLabelCulture.Click += new System.EventHandler(this.Language_Click);
            // 
            // statusStripLabelExPower
            // 
            this.statusStripLabelExPower.AutoSize = false;
            this.statusStripLabelExPower.BackColor = System.Drawing.Color.Transparent;
            this.statusStripLabelExPower.Checked = false;
            this.statusStripLabelExPower.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.statusStripLabelExPower.Name = "LabelExPower";
            this.statusStripLabelExPower.Size = new System.Drawing.Size(28, 23);
            this.statusStripLabelExPower.Text = "P";
            this.statusStripLabelExPower.ToolTipText = "Power spectra (dB)";
            this.statusStripLabelExPower.Click += new System.EventHandler(this.LabelEx_Click);
            // 
            // statusStripLabelExCumulative
            // 
            this.statusStripLabelExCumulative.AutoSize = false;
            this.statusStripLabelExCumulative.BackColor = System.Drawing.Color.Transparent;
            this.statusStripLabelExCumulative.Checked = true;
            this.statusStripLabelExCumulative.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.statusStripLabelExCumulative.Name = "LabelExCumulative";
            this.statusStripLabelExCumulative.Size = new System.Drawing.Size(28, 23);
            this.statusStripLabelExCumulative.Text = "F";
            this.statusStripLabelExCumulative.ToolTipText = "Cumulative fractal dimension";
            this.statusStripLabelExCumulative.Click += new System.EventHandler(this.LabelEx_Click);
            // 
            // statusStripLabelExEntropy
            // 
            this.statusStripLabelExEntropy.AutoSize = false;
            this.statusStripLabelExEntropy.BackColor = System.Drawing.Color.Transparent;
            this.statusStripLabelExEntropy.Checked = true;
            this.statusStripLabelExEntropy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.statusStripLabelExEntropy.Name = "LabelExEntropy";
            this.statusStripLabelExEntropy.Size = new System.Drawing.Size(28, 23);
            this.statusStripLabelExEntropy.Text = "E";
            this.statusStripLabelExEntropy.ToolTipText = "Approximate and sample entropy";
            this.statusStripLabelExEntropy.Click += new System.EventHandler(this.LabelEx_Click);
            // 
            // statusStripLabelExCrossHair
            // 
            this.statusStripLabelExCrossHair.AutoSize = false;
            this.statusStripLabelExCrossHair.BackColor = System.Drawing.Color.Transparent;
            this.statusStripLabelExCrossHair.Checked = true;
            this.statusStripLabelExCrossHair.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.statusStripLabelExCrossHair.Name = "LabelExCrossHair";
            this.statusStripLabelExCrossHair.Size = new System.Drawing.Size(28, 23);
            this.statusStripLabelExCrossHair.Text = "C";
            this.statusStripLabelExCrossHair.ToolTipText = "Plot\'s crosshair mode";
            this.statusStripLabelExCrossHair.Click += new System.EventHandler(this.LabelEx_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 721);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.toolStripMain);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.tspBottom);
            this.Controls.Add(this.tspTop);
            this.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(900, 760);
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Signal analysis";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Shown += new System.EventHandler(this.FrmMain_Shown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FrmMain_KeyPress);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion
    private TableLayoutPanel tableLayoutPanel1;
    private ScottPlot.FormsPlotCrossHair plotOriginal;
    private TableLayoutPanel tableLayoutPanel2;
    private ScottPlot.FormsPlotCrossHair plotWindow;
    private ScottPlot.FormsPlotCrossHair plotApplied;
    private ScottPlot.FormsPlotCrossHair plotFFT;
    private TextBox txtStats;
    private TableLayoutPanel tableLayoutPanel3;
    private ScottPlot.FormsPlotCrossHair plotFractal;
    private ScottPlot.FormsPlotCrossHair plotFractalDistribution;
    private System.Windows.Forms.ToolStripPanel tspTop;
    private System.Windows.Forms.ToolStripPanel tspBottom;
    private System.Windows.Forms.ToolStrip toolStripMain;
    private System.Windows.Forms.ToolStripButton toolStripMain_Exit;
    private System.Windows.Forms.ToolStripButton toolStripMain_Open;
    private System.Windows.Forms.ToolStripButton toolStripMain_Export;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripComboBox stripComboSeries;
    private System.Windows.Forms.ToolStripComboBox stripComboWindows;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    private System.Windows.Forms.ToolStripButton toolStripMain_Settings;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    private System.Windows.Forms.ToolStripButton toolStripMain_About;
    private System.Windows.Forms.StatusStrip statusStrip;
    private System.Windows.Forms.ToolStripStatusLabel statusStripLabelEmpty;
    private System.Windows.Forms.ToolStripStatusLabel statusStripLabelCulture;
    private System.Windows.Forms.ToolStripStatusLabelEx statusStripLabelExPower;
    private System.Windows.Forms.ToolStripStatusLabelEx statusStripLabelExCumulative;
    private System.Windows.Forms.ToolStripStatusLabelEx statusStripLabelExEntropy;
    private System.Windows.Forms.ToolStripStatusLabelEx statusStripLabelExCrossHair;
}
