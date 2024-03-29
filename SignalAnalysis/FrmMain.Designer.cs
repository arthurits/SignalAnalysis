﻿namespace SignalAnalysis;

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
        tspTop = new ToolStripPanel();
        tspBottom = new ToolStripPanel();
        toolStripMain = new ToolStrip();
        toolStripMain_Exit = new ToolStripButton();
        toolStripMain_Open = new ToolStripButton();
        toolStripMain_Export = new ToolStripButton();
        toolStripSeparator1 = new ToolStripSeparator();
        stripComboSeries = new ToolStripComboBox();
        stripComboWindows = new ToolStripComboBox();
        toolStripSeparator2 = new ToolStripSeparator();
        toolStripMain_Settings = new ToolStripButton();
        toolStripSeparator3 = new ToolStripSeparator();
        toolStripMain_About = new ToolStripButton();
        statusStrip = new StatusStrip();
        statusStripLabelEmpty = new ToolStripStatusLabel();
        statusStripLabelCulture = new ToolStripStatusLabel();
        statusStripLabelExBoxplot = new ToolStripStatusLabelEx();
        statusStripLabelExDerivative = new ToolStripStatusLabelEx();
        statusStripLabelExIntegration = new ToolStripStatusLabelEx();
        statusStripLabelExCumulative = new ToolStripStatusLabelEx();
        statusStripLabelExPower = new ToolStripStatusLabelEx();
        statusStripLabelExEntropy = new ToolStripStatusLabelEx();
        statusStripLabelExCrossHair = new ToolStripStatusLabelEx();
        layoutGlobal = new TableLayoutPanel();
        layoutData = new TableLayoutPanel();
        plotOriginal = new ScottPlot.FormsPlotCrossHair();
        plotBoxPlot = new ScottPlot.FormsPlotCrossHair();
        plotDerivative = new ScottPlot.FormsPlotCrossHair();
        layoutFractal = new TableLayoutPanel();
        plotFractal = new ScottPlot.FormsPlotCrossHair();
        plotFractalDistribution = new ScottPlot.FormsPlotCrossHair();
        layoutFFT = new TableLayoutPanel();
        plotFFT = new ScottPlot.FormsPlotCrossHair();
        plotWindow = new ScottPlot.FormsPlotCrossHair();
        plotApplied = new ScottPlot.FormsPlotCrossHair();
        txtStats = new TextBox();
        toolStripMain.SuspendLayout();
        statusStrip.SuspendLayout();
        layoutGlobal.SuspendLayout();
        layoutData.SuspendLayout();
        layoutFractal.SuspendLayout();
        layoutFFT.SuspendLayout();
        SuspendLayout();
        // 
        // tspTop
        // 
        tspTop.Dock = DockStyle.Top;
        tspTop.Location = new Point(0, 0);
        tspTop.Name = "tspTop";
        tspTop.Orientation = Orientation.Horizontal;
        tspTop.RowMargin = new Padding(3, 0, 0, 0);
        tspTop.Size = new Size(884, 0);
        // 
        // tspBottom
        // 
        tspBottom.Dock = DockStyle.Bottom;
        tspBottom.Location = new Point(0, 721);
        tspBottom.Name = "tspBottom";
        tspBottom.Orientation = Orientation.Horizontal;
        tspBottom.RowMargin = new Padding(3, 0, 0, 0);
        tspBottom.Size = new Size(884, 0);
        // 
        // toolStripMain
        // 
        toolStripMain.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        toolStripMain.ImageScalingSize = new Size(48, 48);
        toolStripMain.Items.AddRange(new ToolStripItem[] { toolStripMain_Exit, toolStripMain_Open, toolStripMain_Export, toolStripSeparator1, stripComboSeries, stripComboWindows, toolStripSeparator2, toolStripMain_Settings, toolStripSeparator3, toolStripMain_About });
        toolStripMain.Location = new Point(0, 0);
        toolStripMain.Name = "toolStripMain";
        toolStripMain.RenderMode = ToolStripRenderMode.Professional;
        toolStripMain.Size = new Size(884, 27);
        toolStripMain.TabIndex = 0;
        toolStripMain.TabStop = true;
        toolStripMain.Text = "Main toolbar";
        // 
        // toolStripMain_Exit
        // 
        toolStripMain_Exit.ImageTransparentColor = Color.Magenta;
        toolStripMain_Exit.Name = "toolStripMain_Exit";
        toolStripMain_Exit.Size = new Size(34, 24);
        toolStripMain_Exit.Text = "Exit";
        toolStripMain_Exit.TextImageRelation = TextImageRelation.ImageAboveText;
        toolStripMain_Exit.ToolTipText = "Export data and data analysis";
        toolStripMain_Exit.Click += Exit_Click;
        // 
        // toolStripMain_Open
        // 
        toolStripMain_Open.Name = "toolStripMain_Open";
        toolStripMain_Open.Size = new Size(47, 24);
        toolStripMain_Open.Text = "Open";
        toolStripMain_Open.TextImageRelation = TextImageRelation.ImageAboveText;
        toolStripMain_Open.Click += Open_Click;
        // 
        // toolStripMain_Export
        // 
        toolStripMain_Export.Name = "toolStripMain_Export";
        toolStripMain_Export.Size = new Size(52, 24);
        toolStripMain_Export.Text = "Export";
        toolStripMain_Export.TextImageRelation = TextImageRelation.ImageAboveText;
        toolStripMain_Export.Click += Export_Click;
        // 
        // toolStripSeparator1
        // 
        toolStripSeparator1.Name = "toolStripSeparator1";
        toolStripSeparator1.Size = new Size(6, 27);
        // 
        // stripComboSeries
        // 
        stripComboSeries.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        stripComboSeries.DropDownHeight = 110;
        stripComboSeries.DropDownWidth = 122;
        stripComboSeries.FlatStyle = FlatStyle.Standard;
        stripComboSeries.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        stripComboSeries.IntegralHeight = false;
        stripComboSeries.MaxDropDownItems = 9;
        stripComboSeries.MergeAction = MergeAction.MatchOnly;
        stripComboSeries.Name = "stripComboSeries";
        stripComboSeries.Size = new Size(120, 27);
        stripComboSeries.ToolTipText = "Select data series";
        // 
        // stripComboWindows
        // 
        stripComboWindows.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        stripComboWindows.DropDownHeight = 110;
        stripComboWindows.DropDownWidth = 122;
        stripComboWindows.FlatStyle = FlatStyle.Standard;
        stripComboWindows.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        stripComboWindows.IntegralHeight = false;
        stripComboWindows.MaxDropDownItems = 9;
        stripComboWindows.MergeAction = MergeAction.MatchOnly;
        stripComboWindows.Name = "stripComboWindows";
        stripComboWindows.Size = new Size(120, 27);
        stripComboWindows.ToolTipText = "Select FFT window";
        // 
        // toolStripSeparator2
        // 
        toolStripSeparator2.Name = "toolStripSeparator2";
        toolStripSeparator2.Size = new Size(6, 27);
        // 
        // toolStripMain_Settings
        // 
        toolStripMain_Settings.ImageTransparentColor = Color.Magenta;
        toolStripMain_Settings.Name = "toolStripMain_Settings";
        toolStripMain_Settings.Size = new Size(62, 24);
        toolStripMain_Settings.Text = "Settings";
        toolStripMain_Settings.TextImageRelation = TextImageRelation.ImageAboveText;
        toolStripMain_Settings.ToolTipText = "Settings for plots, data, and UI";
        toolStripMain_Settings.Click += Settings_Click;
        // 
        // toolStripSeparator3
        // 
        toolStripSeparator3.Name = "toolStripSeparator3";
        toolStripSeparator3.Size = new Size(6, 27);
        // 
        // toolStripMain_About
        // 
        toolStripMain_About.ImageTransparentColor = Color.Magenta;
        toolStripMain_About.Name = "toolStripMain_About";
        toolStripMain_About.Size = new Size(51, 24);
        toolStripMain_About.Text = "About";
        toolStripMain_About.TextImageRelation = TextImageRelation.ImageAboveText;
        toolStripMain_About.ToolTipText = "About this software";
        toolStripMain_About.Click += About_Click;
        // 
        // statusStrip
        // 
        statusStrip.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        statusStrip.Items.AddRange(new ToolStripItem[] { statusStripLabelEmpty, statusStripLabelCulture, statusStripLabelExBoxplot, statusStripLabelExDerivative, statusStripLabelExIntegration, statusStripLabelExCumulative, statusStripLabelExPower, statusStripLabelExEntropy, statusStripLabelExCrossHair });
        statusStrip.Location = new Point(0, 693);
        statusStrip.Name = "statusStrip";
        statusStrip.RenderMode = ToolStripRenderMode.Professional;
        statusStrip.ShowItemToolTips = true;
        statusStrip.Size = new Size(884, 28);
        statusStrip.SizingGrip = false;
        statusStrip.TabIndex = 1;
        statusStrip.Text = "Status bar";
        // 
        // statusStripLabelEmpty
        // 
        statusStripLabelEmpty.BorderSides = ToolStripStatusLabelBorderSides.Right;
        statusStripLabelEmpty.DisplayStyle = ToolStripItemDisplayStyle.Text;
        statusStripLabelEmpty.Name = "statusStripLabelEmpty";
        statusStripLabelEmpty.Size = new Size(572, 23);
        statusStripLabelEmpty.Spring = true;
        statusStripLabelEmpty.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // statusStripLabelCulture
        // 
        statusStripLabelCulture.AutoSize = false;
        statusStripLabelCulture.BorderSides = ToolStripStatusLabelBorderSides.Right;
        statusStripLabelCulture.DisplayStyle = ToolStripItemDisplayStyle.Text;
        statusStripLabelCulture.Name = "statusStripLabelCulture";
        statusStripLabelCulture.Size = new Size(70, 23);
        statusStripLabelCulture.ToolTipText = "User interface language";
        statusStripLabelCulture.Click += Language_Click;
        // 
        // statusStripLabelExBoxplot
        // 
        statusStripLabelExBoxplot.AutoSize = false;
        statusStripLabelExBoxplot.BackColor = Color.Transparent;
        statusStripLabelExBoxplot.Checked = false;
        statusStripLabelExBoxplot.DisplayStyle = ToolStripItemDisplayStyle.Text;
        statusStripLabelExBoxplot.ForeColor = Color.LightGray;
        statusStripLabelExBoxplot.ForeColorChecked = Color.Black;
        statusStripLabelExBoxplot.ForeColorUnchecked = Color.LightGray;
        statusStripLabelExBoxplot.Name = "statusStripLabelExBoxplot";
        statusStripLabelExBoxplot.Size = new Size(28, 23);
        statusStripLabelExBoxplot.Text = "B";
        statusStripLabelExBoxplot.ToolTipText = "Box plot";
        statusStripLabelExBoxplot.Click += LabelEx_Click;
        // 
        // statusStripLabelExDerivative
        // 
        statusStripLabelExDerivative.AutoSize = false;
        statusStripLabelExDerivative.BackColor = Color.Transparent;
        statusStripLabelExDerivative.Checked = false;
        statusStripLabelExDerivative.DisplayStyle = ToolStripItemDisplayStyle.Text;
        statusStripLabelExDerivative.ForeColor = Color.LightGray;
        statusStripLabelExDerivative.ForeColorChecked = Color.Black;
        statusStripLabelExDerivative.ForeColorUnchecked = Color.LightGray;
        statusStripLabelExDerivative.Name = "statusStripLabelExDerivative";
        statusStripLabelExDerivative.Size = new Size(28, 23);
        statusStripLabelExDerivative.Text = "D";
        statusStripLabelExDerivative.ToolTipText = "Numerical differentiation";
        statusStripLabelExDerivative.Click += LabelEx_Click;
        // 
        // statusStripLabelExIntegration
        // 
        statusStripLabelExIntegration.AutoSize = false;
        statusStripLabelExIntegration.BackColor = Color.Transparent;
        statusStripLabelExIntegration.Checked = false;
        statusStripLabelExIntegration.DisplayStyle = ToolStripItemDisplayStyle.Text;
        statusStripLabelExIntegration.ForeColor = Color.LightGray;
        statusStripLabelExIntegration.ForeColorChecked = Color.Black;
        statusStripLabelExIntegration.ForeColorUnchecked = Color.LightGray;
        statusStripLabelExIntegration.Name = "statusStripLabelExIntegration";
        statusStripLabelExIntegration.Size = new Size(28, 23);
        statusStripLabelExIntegration.Text = "I";
        statusStripLabelExIntegration.ToolTipText = "Numerical integration";
        statusStripLabelExIntegration.Click += LabelEx_Click;
        // 
        // statusStripLabelExCumulative
        // 
        statusStripLabelExCumulative.AutoSize = false;
        statusStripLabelExCumulative.BackColor = Color.Transparent;
        statusStripLabelExCumulative.Checked = false;
        statusStripLabelExCumulative.DisplayStyle = ToolStripItemDisplayStyle.Text;
        statusStripLabelExCumulative.ForeColor = Color.LightGray;
        statusStripLabelExCumulative.ForeColorChecked = Color.Black;
        statusStripLabelExCumulative.ForeColorUnchecked = Color.LightGray;
        statusStripLabelExCumulative.Name = "statusStripLabelExCumulative";
        statusStripLabelExCumulative.Size = new Size(28, 23);
        statusStripLabelExCumulative.Text = "F";
        statusStripLabelExCumulative.ToolTipText = "Cumulative fractal dimension";
        statusStripLabelExCumulative.Click += LabelEx_Click;
        // 
        // statusStripLabelExPower
        // 
        statusStripLabelExPower.AutoSize = false;
        statusStripLabelExPower.BackColor = Color.Transparent;
        statusStripLabelExPower.Checked = true;
        statusStripLabelExPower.DisplayStyle = ToolStripItemDisplayStyle.Text;
        statusStripLabelExPower.ForeColor = Color.Black;
        statusStripLabelExPower.ForeColorChecked = Color.Black;
        statusStripLabelExPower.ForeColorUnchecked = Color.LightGray;
        statusStripLabelExPower.Name = "statusStripLabelExPower";
        statusStripLabelExPower.Size = new Size(28, 23);
        statusStripLabelExPower.Text = "P";
        statusStripLabelExPower.ToolTipText = "Power spectra (dB)";
        statusStripLabelExPower.Click += LabelEx_Click;
        // 
        // statusStripLabelExEntropy
        // 
        statusStripLabelExEntropy.AutoSize = false;
        statusStripLabelExEntropy.BackColor = Color.Transparent;
        statusStripLabelExEntropy.Checked = false;
        statusStripLabelExEntropy.DisplayStyle = ToolStripItemDisplayStyle.Text;
        statusStripLabelExEntropy.ForeColor = Color.LightGray;
        statusStripLabelExEntropy.ForeColorChecked = Color.Black;
        statusStripLabelExEntropy.ForeColorUnchecked = Color.LightGray;
        statusStripLabelExEntropy.Name = "statusStripLabelExEntropy";
        statusStripLabelExEntropy.Size = new Size(28, 23);
        statusStripLabelExEntropy.Text = "E";
        statusStripLabelExEntropy.ToolTipText = "Approximate and sample entropy";
        statusStripLabelExEntropy.Click += LabelEx_Click;
        // 
        // statusStripLabelExCrossHair
        // 
        statusStripLabelExCrossHair.AutoSize = false;
        statusStripLabelExCrossHair.BackColor = Color.Transparent;
        statusStripLabelExCrossHair.Checked = false;
        statusStripLabelExCrossHair.DisplayStyle = ToolStripItemDisplayStyle.Text;
        statusStripLabelExCrossHair.ForeColor = Color.LightGray;
        statusStripLabelExCrossHair.ForeColorChecked = Color.Black;
        statusStripLabelExCrossHair.ForeColorUnchecked = Color.LightGray;
        statusStripLabelExCrossHair.Name = "statusStripLabelExCrossHair";
        statusStripLabelExCrossHair.Size = new Size(28, 23);
        statusStripLabelExCrossHair.Text = "C";
        statusStripLabelExCrossHair.ToolTipText = "Plot's crosshair mode";
        statusStripLabelExCrossHair.Click += LabelEx_Click;
        // 
        // layoutGlobal
        // 
        layoutGlobal.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        layoutGlobal.BackColor = SystemColors.Control;
        layoutGlobal.ColumnCount = 1;
        layoutGlobal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layoutGlobal.Controls.Add(layoutData, 0, 0);
        layoutGlobal.Controls.Add(layoutFractal, 0, 1);
        layoutGlobal.Controls.Add(layoutFFT, 0, 2);
        layoutGlobal.Controls.Add(txtStats, 0, 3);
        layoutGlobal.Location = new Point(12, 74);
        layoutGlobal.Margin = new Padding(0);
        layoutGlobal.Name = "layoutGlobal";
        layoutGlobal.RowCount = 4;
        layoutGlobal.RowStyles.Add(new RowStyle(SizeType.Percent, 28F));
        layoutGlobal.RowStyles.Add(new RowStyle(SizeType.Percent, 28F));
        layoutGlobal.RowStyles.Add(new RowStyle(SizeType.Percent, 28F));
        layoutGlobal.RowStyles.Add(new RowStyle(SizeType.Percent, 14F));
        layoutGlobal.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        layoutGlobal.Size = new Size(860, 619);
        layoutGlobal.TabIndex = 2;
        // 
        // layoutData
        // 
        layoutData.BackColor = SystemColors.Control;
        layoutData.ColumnCount = 3;
        layoutData.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
        layoutData.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
        layoutData.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
        layoutData.Controls.Add(plotOriginal, 0, 0);
        layoutData.Controls.Add(plotBoxPlot, 1, 0);
        layoutData.Controls.Add(plotDerivative, 2, 0);
        layoutData.Dock = DockStyle.Fill;
        layoutData.Location = new Point(0, 0);
        layoutData.Margin = new Padding(0);
        layoutData.Name = "layoutData";
        layoutData.RowCount = 1;
        layoutData.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layoutData.Size = new Size(860, 176);
        layoutData.TabIndex = 0;
        // 
        // plotOriginal
        // 
        plotOriginal.BackColor = Color.Transparent;
        plotOriginal.CrossHairColor = Color.Red;
        plotOriginal.CultureUI = new System.Globalization.CultureInfo("en-US");
        plotOriginal.Dock = DockStyle.Fill;
        plotOriginal.Location = new Point(4, 3);
        plotOriginal.Margin = new Padding(4, 3, 4, 3);
        plotOriginal.Name = "plotOriginal";
        plotOriginal.ShowCrossHair = false;
        plotOriginal.ShowCrossHairHorizontal = false;
        plotOriginal.ShowCrossHairVertical = false;
        plotOriginal.Size = new Size(336, 170);
        plotOriginal.SnapToPoint = false;
        plotOriginal.TabIndex = 0;
        // 
        // plotBoxPlot
        // 
        plotBoxPlot.CrossHairColor = Color.Red;
        plotBoxPlot.CultureUI = new System.Globalization.CultureInfo("en-US");
        plotBoxPlot.Dock = DockStyle.Fill;
        plotBoxPlot.Location = new Point(348, 3);
        plotBoxPlot.Margin = new Padding(4, 3, 4, 3);
        plotBoxPlot.Name = "plotBoxPlot";
        plotBoxPlot.ShowCrossHair = false;
        plotBoxPlot.ShowCrossHairHorizontal = false;
        plotBoxPlot.ShowCrossHairVertical = false;
        plotBoxPlot.Size = new Size(164, 170);
        plotBoxPlot.SnapToPoint = false;
        plotBoxPlot.TabIndex = 1;
        // 
        // plotDerivative
        // 
        plotDerivative.CrossHairColor = Color.Red;
        plotDerivative.CultureUI = new System.Globalization.CultureInfo("en-US");
        plotDerivative.Dock = DockStyle.Fill;
        plotDerivative.Location = new Point(520, 3);
        plotDerivative.Margin = new Padding(4, 3, 4, 3);
        plotDerivative.Name = "plotDerivative";
        plotDerivative.ShowCrossHair = false;
        plotDerivative.ShowCrossHairHorizontal = false;
        plotDerivative.ShowCrossHairVertical = false;
        plotDerivative.Size = new Size(336, 170);
        plotDerivative.SnapToPoint = false;
        plotDerivative.TabIndex = 2;
        // 
        // layoutFractal
        // 
        layoutFractal.ColumnCount = 2;
        layoutFractal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 66.66666F));
        layoutFractal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
        layoutFractal.Controls.Add(plotFractal, 0, 0);
        layoutFractal.Controls.Add(plotFractalDistribution, 1, 0);
        layoutFractal.Dock = DockStyle.Fill;
        layoutFractal.Location = new Point(0, 176);
        layoutFractal.Margin = new Padding(0);
        layoutFractal.Name = "layoutFractal";
        layoutFractal.RowCount = 1;
        layoutFractal.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layoutFractal.Size = new Size(860, 176);
        layoutFractal.TabIndex = 1;
        // 
        // plotFractal
        // 
        plotFractal.BackColor = Color.Transparent;
        plotFractal.CrossHairColor = Color.Red;
        plotFractal.CultureUI = new System.Globalization.CultureInfo("en-US");
        plotFractal.Dock = DockStyle.Fill;
        plotFractal.Location = new Point(4, 3);
        plotFractal.Margin = new Padding(4, 3, 4, 3);
        plotFractal.Name = "plotFractal";
        plotFractal.ShowCrossHair = false;
        plotFractal.ShowCrossHairHorizontal = false;
        plotFractal.ShowCrossHairVertical = false;
        plotFractal.Size = new Size(565, 170);
        plotFractal.SnapToPoint = false;
        plotFractal.TabIndex = 0;
        // 
        // plotFractalDistribution
        // 
        plotFractalDistribution.CrossHairColor = Color.Red;
        plotFractalDistribution.CultureUI = new System.Globalization.CultureInfo("en-US");
        plotFractalDistribution.Dock = DockStyle.Fill;
        plotFractalDistribution.Location = new Point(577, 3);
        plotFractalDistribution.Margin = new Padding(4, 3, 4, 3);
        plotFractalDistribution.Name = "plotFractalDistribution";
        plotFractalDistribution.ShowCrossHair = false;
        plotFractalDistribution.ShowCrossHairHorizontal = false;
        plotFractalDistribution.ShowCrossHairVertical = false;
        plotFractalDistribution.Size = new Size(279, 170);
        plotFractalDistribution.SnapToPoint = false;
        plotFractalDistribution.TabIndex = 1;
        // 
        // layoutFFT
        // 
        layoutFFT.ColumnCount = 3;
        layoutFFT.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        layoutFFT.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        layoutFFT.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        layoutFFT.Controls.Add(plotFFT, 0, 0);
        layoutFFT.Controls.Add(plotWindow, 1, 0);
        layoutFFT.Controls.Add(plotApplied, 2, 0);
        layoutFFT.Dock = DockStyle.Fill;
        layoutFFT.Location = new Point(0, 352);
        layoutFFT.Margin = new Padding(0);
        layoutFFT.Name = "layoutFFT";
        layoutFFT.RowCount = 1;
        layoutFFT.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layoutFFT.Size = new Size(860, 176);
        layoutFFT.TabIndex = 2;
        // 
        // plotFFT
        // 
        plotFFT.BackColor = Color.Transparent;
        plotFFT.CrossHairColor = Color.Red;
        plotFFT.CultureUI = new System.Globalization.CultureInfo("en-US");
        plotFFT.Dock = DockStyle.Fill;
        plotFFT.Location = new Point(4, 3);
        plotFFT.Margin = new Padding(4, 3, 4, 3);
        plotFFT.Name = "plotFFT";
        plotFFT.ShowCrossHair = false;
        plotFFT.ShowCrossHairHorizontal = false;
        plotFFT.ShowCrossHairVertical = false;
        plotFFT.Size = new Size(422, 170);
        plotFFT.SnapToPoint = false;
        plotFFT.TabIndex = 0;
        // 
        // plotWindow
        // 
        plotWindow.BackColor = Color.Transparent;
        plotWindow.CrossHairColor = Color.Red;
        plotWindow.CultureUI = new System.Globalization.CultureInfo("en-US");
        plotWindow.Dock = DockStyle.Fill;
        plotWindow.Location = new Point(434, 3);
        plotWindow.Margin = new Padding(4, 3, 4, 3);
        plotWindow.Name = "plotWindow";
        plotWindow.ShowCrossHair = false;
        plotWindow.ShowCrossHairHorizontal = false;
        plotWindow.ShowCrossHairVertical = false;
        plotWindow.Size = new Size(207, 170);
        plotWindow.SnapToPoint = false;
        plotWindow.TabIndex = 1;
        // 
        // plotApplied
        // 
        plotApplied.BackColor = Color.Transparent;
        plotApplied.CrossHairColor = Color.Red;
        plotApplied.CultureUI = new System.Globalization.CultureInfo("en-US");
        plotApplied.Dock = DockStyle.Fill;
        plotApplied.Location = new Point(649, 3);
        plotApplied.Margin = new Padding(4, 3, 4, 3);
        plotApplied.Name = "plotApplied";
        plotApplied.ShowCrossHair = false;
        plotApplied.ShowCrossHairHorizontal = false;
        plotApplied.ShowCrossHairVertical = false;
        plotApplied.Size = new Size(207, 170);
        plotApplied.SnapToPoint = false;
        plotApplied.TabIndex = 2;
        // 
        // txtStats
        // 
        txtStats.Dock = DockStyle.Fill;
        txtStats.Location = new Point(3, 528);
        txtStats.Margin = new Padding(3, 0, 3, 5);
        txtStats.Multiline = true;
        txtStats.Name = "txtStats";
        txtStats.ReadOnly = true;
        txtStats.ScrollBars = ScrollBars.Vertical;
        txtStats.Size = new Size(854, 86);
        txtStats.TabIndex = 3;
        // 
        // FrmMain
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(884, 721);
        Controls.Add(layoutGlobal);
        Controls.Add(toolStripMain);
        Controls.Add(statusStrip);
        Controls.Add(tspBottom);
        Controls.Add(tspTop);
        Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        KeyPreview = true;
        MinimumSize = new Size(900, 760);
        Name = "FrmMain";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Signal analysis";
        FormClosing += FrmMain_FormClosing;
        Shown += FrmMain_Shown;
        KeyPress += FrmMain_KeyPress;
        toolStripMain.ResumeLayout(false);
        toolStripMain.PerformLayout();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        layoutGlobal.ResumeLayout(false);
        layoutGlobal.PerformLayout();
        layoutData.ResumeLayout(false);
        layoutFractal.ResumeLayout(false);
        layoutFFT.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
    private System.Windows.Forms.StatusStrip statusStrip;
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
    private System.Windows.Forms.ToolStripStatusLabel statusStripLabelEmpty;
    private System.Windows.Forms.ToolStripStatusLabel statusStripLabelCulture;
    private System.Windows.Forms.ToolStripStatusLabelEx statusStripLabelExPower;
    private System.Windows.Forms.ToolStripStatusLabelEx statusStripLabelExCumulative;
    private System.Windows.Forms.ToolStripStatusLabelEx statusStripLabelExEntropy;
    private System.Windows.Forms.ToolStripStatusLabelEx statusStripLabelExCrossHair;
    private System.Windows.Forms.ToolStripStatusLabelEx statusStripLabelExDerivative;
    private System.Windows.Forms.ToolStripStatusLabelEx statusStripLabelExIntegration;
    private System.Windows.Forms.ToolStripStatusLabelEx statusStripLabelExBoxplot;
    private TableLayoutPanel layoutGlobal;
    private TextBox txtStats;
    private TableLayoutPanel layoutFFT;
    private ScottPlot.FormsPlotCrossHair plotFFT;
    private ScottPlot.FormsPlotCrossHair plotWindow;
    private ScottPlot.FormsPlotCrossHair plotApplied;
    private TableLayoutPanel layoutFractal;
    private ScottPlot.FormsPlotCrossHair plotFractal;
    private ScottPlot.FormsPlotCrossHair plotFractalDistribution;
    private TableLayoutPanel layoutData;
    private ScottPlot.FormsPlotCrossHair plotOriginal;
    private ScottPlot.FormsPlotCrossHair plotBoxPlot;
    private ScottPlot.FormsPlotCrossHair plotDerivative;
}
