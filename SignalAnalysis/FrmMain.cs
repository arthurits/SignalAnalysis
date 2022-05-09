namespace SignalAnalysis;

public partial class FrmMain : Form
{
    private double[][] _signalData = Array.Empty<double[]>();
    private string[] seriesLabels = Array.Empty<string>();
    private int nSeries = 0;
    double nSampleFreq = 0.0;
    
    DateTime nStart;
    ClassSettings _settings = new();
    Stats Results = new();
    
    ToolStripPanel tspTop = new();
    ToolStripPanel tspBottom = new();
    ToolStripComboBox stripComboSeries = new();
    ToolStripComboBox stripComboWindows = new();
    ToolStripStatusLabel stripLblCulture = new();

    Task statsTask = Task.CompletedTask;
    private CancellationTokenSource tokenSource = new();
    private CancellationToken token = CancellationToken.None;

    /// <summary>
    /// https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2010/y99d1cd3(v=vs.100)?WT.mc_id=DT-MVP-5003235
    /// https://stackoverflow.com/questions/32989100/how-to-make-multi-language-app-in-winforms
    /// </summary>
    private readonly System.Resources.ResourceManager StringsRM = new("SignalAnalysis.localization.strings", typeof(FrmMain).Assembly);

    public FrmMain()
    {
        // Load settings
        LoadProgramSettingsJSON();

        // Set form icon
        this.Icon = GraphicsResources.Load<Icon>(GraphicsResources.AppLogo);

        // Controls initialization
        InitializeToolStripPanel();
        InitializeStatusStrip();
        InitializeMenu();
        InitializeComponent();

        PopulateComboWindow();

        this.plotOriginal.SnapToPoint = true;
        this.plotFFT.SnapToPoint = true;

        // Language initialization
        UpdateUI_Language();
    }

    private void FrmMain_Shown(object sender, EventArgs e)
    {
        // Signal the native process (that launched us) to close the splash screen
        using var closeSplashEvent = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.ManualReset, "CloseSplashScreenEvent");
        closeSplashEvent.Set();
        
        // Move the focus away in order to deselect the text
        this.tableLayoutPanel1.Focus();
    }

    private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
    {
        using (new CenterWinDialog(this))
        {
            if (DialogResult.No == MessageBox.Show(this,
                                                    StringsRM.GetString("strMsgBoxExitTitle", _settings.AppCulture) ?? "Exit?",
                                                    StringsRM.GetString("strMsgBoxExit", _settings.AppCulture) ?? "Are you sure you want to exit\nthe application?",
                                                    MessageBoxButtons.YesNo,
                                                    MessageBoxIcon.Question,
                                                    MessageBoxDefaultButton.Button2))
            {
                // Cancel
                e.Cancel = true;
            }
        }

        // Save settings data
        SaveProgramSettingsJSON();
    }

    private void FrmMain_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == (char)Keys.Escape && statsTask.Status == TaskStatus.Running)
            tokenSource.Cancel();
    }

    private void PopulateComboSeries(params string[] values)
    {
        stripComboSeries.Items.Clear();
        if (values.Length != 0)
        {
            stripComboSeries.Items.AddRange(values);
            stripComboSeries.Text = values[0];
        }
        else
        {
            stripComboSeries.Items.AddRange(seriesLabels);
            stripComboSeries.Text = seriesLabels[0];
        }

    }
    private void PopulateComboWindow()
    {
        IWindow[] windows = Window.GetWindows();
        stripComboWindows.Items.AddRange(windows);
        stripComboWindows.SelectedIndex = windows.ToList().FindIndex(x => x.Name == "Hanning");

        // Move the focus away in order to deselect the text
        this.tableLayoutPanel1.Focus();
    }

    private void ComboSeries_SelectedIndexChanged(object? sender, EventArgs e)
    {
        // Move the focus away in order to deselect the text
        this.tableLayoutPanel1.Focus();

        if (_signalData.Length == 0) return;

        ((ToolStripStatusLabelEx)((ToolStrip)tspBottom.Controls[0]).Items[4]).Checked = false;

        // Update stats and plots
        UpdateStatsPlots(stripComboSeries.SelectedIndex);
    }

    private void ComboWindow_SelectedIndexChanged(object? sender, EventArgs e)
    {
        // Move the focus away in order to deselect the text
        this.tableLayoutPanel1.Focus();

        if (stripComboSeries.SelectedIndex < 0) return;

        // Extract the values 
        var signal = _signalData[stripComboSeries.SelectedIndex][_settings.IndexStart..(_settings.IndexEnd + 1)];
        if (signal is null || signal.Length == 0) return;

        UpdateWindowPlots(signal);
    }

    /// <summary>
    /// Sets the form's title
    /// </summary>
    /// <param name="frm">Form which title is to be set</param>
    /// <param name="strFileName">String to be added at the default title in 'strFormTitle' string.
    /// If <see langword="null"/>, no string is added.
    /// If <see cref="String.Empty"/>, the current added text is mantained.
    /// Other values are added to the default title.</param>
    private void SetFormTitle(System.Windows.Forms.Form frm, string? strFileName = null)
    {
        string strText = String.Empty;
        string strSep = StringsRM.GetString("strFrmTitleUnion", _settings.AppCulture) ?? " - ";
        if (strFileName is not null)
        {
            if (strFileName != String.Empty)
                strText = $"{strSep}{strFileName}";
            else
            {
                int index = frm.Text.IndexOf(strSep) > -1 ? frm.Text.IndexOf(strSep) : frm.Text.Length;
                strText = frm.Text[index..];
            }
        }
        frm.Text = StringsRM.GetString("strFormTitle", _settings.AppCulture) ?? "Signal analysis" + strText;
    }

    /// <summary>
    /// Updates the UI language of all controls
    /// </summary>
    private void UpdateUI_Language()
    {
        // Update the form's tittle
        SetFormTitle(this, String.Empty);

        // Update ToolStrip
        ((ToolStrip)tspTop.Controls[0]).Items[0].Text = StringsRM.GetString("strToolStripExit", _settings.AppCulture) ?? "Exit";
        ((ToolStrip)tspTop.Controls[0]).Items[0].ToolTipText = StringsRM.GetString("strToolTipExit", _settings.AppCulture) ?? "Exit the application";
        ((ToolStrip)tspTop.Controls[0]).Items[1].Text = StringsRM.GetString("strToolStripOpen", _settings.AppCulture) ?? "Open";
        ((ToolStrip)tspTop.Controls[0]).Items[1].ToolTipText = StringsRM.GetString("strToolTipOpen", _settings.AppCulture) ?? "Open data file from disk";
        ((ToolStrip)tspTop.Controls[0]).Items[2].Text = StringsRM.GetString("strToolStripExport", _settings.AppCulture) ?? "Export";
        ((ToolStrip)tspTop.Controls[0]).Items[2].ToolTipText = StringsRM.GetString("strToolTipExport", _settings.AppCulture) ?? "Export data and data analysis";
        stripComboSeries.ToolTipText = StringsRM.GetString("strToolTipCboSeries", _settings.AppCulture) ?? "Select data series";
        stripComboWindows.ToolTipText = StringsRM.GetString("strToolTipCboWindows", _settings.AppCulture) ?? "Select FFT window";
        ((ToolStrip)tspTop.Controls[0]).Items[7].Text = StringsRM.GetString("strToolStripSettings", _settings.AppCulture) ?? "Settings";
        ((ToolStrip)tspTop.Controls[0]).Items[7].ToolTipText = StringsRM.GetString("strToolTipSettings", _settings.AppCulture) ?? "Settings for plots, data, and UI";
        ((ToolStrip)tspTop.Controls[0]).Items[9].Text = StringsRM.GetString("strToolStripAbout", _settings.AppCulture) ?? "About";
        ((ToolStrip)tspTop.Controls[0]).Items[9].ToolTipText = StringsRM.GetString("strToolTipAbout", _settings.AppCulture) ?? "About this software";

        // Update StatusStrip
        stripLblCulture.Text = _settings.AppCulture.Name == String.Empty ? "Invariant" : _settings.AppCulture.Name;
        stripLblCulture.ToolTipText = StringsRM.GetString("strToolTipUILanguage", _settings.AppCulture) ?? "User interface language";
        ((ToolStrip)tspBottom.Controls[0]).Items[2].ToolTipText = StringsRM.GetString("strStatusTipPower", _settings.AppCulture) ?? "Power spectra(dB)";
        ((ToolStrip)tspBottom.Controls[0]).Items[3].ToolTipText = StringsRM.GetString("strStatusTipFractal", _settings.AppCulture) ?? "Cumulative fractal dimension";
        ((ToolStrip)tspBottom.Controls[0]).Items[4].ToolTipText = StringsRM.GetString("strStatusTipEntropy", _settings.AppCulture) ?? "Approximate and sample entropy";
        ((ToolStrip)tspBottom.Controls[0]).Items[5].ToolTipText = StringsRM.GetString("strStatusTipCrossHair", _settings.AppCulture) ?? "Plot's crosshair mode";

        // Update plots if they contain series
        plotOriginal.CultureUI = _settings.AppCulture;
        if (plotOriginal.Plot.GetPlottables().Length > 0)
        {
            plotOriginal.Plot.Title(StringsRM.GetString("strPlotOriginalTitle", _settings.AppCulture));
            plotOriginal.Plot.YLabel(StringsRM.GetString("strPlotOriginalYLabel", _settings.AppCulture));
            plotOriginal.Plot.XLabel(StringsRM.GetString("strPlotOriginalXLabel", _settings.AppCulture));
            plotOriginal.Refresh();
        }

        plotWindow.CultureUI = _settings.AppCulture;
        if (plotWindow.Plot.GetPlottables().Length > 0)
        {
            plotWindow.Plot.Title(StringsRM.GetString("strPlotWindowTitle", _settings.AppCulture));
            plotWindow.Plot.YLabel(StringsRM.GetString("strPlotWindowYLabel", _settings.AppCulture));
            plotWindow.Plot.XLabel(StringsRM.GetString("strPlotWindowXLabel", _settings.AppCulture));
        }

        plotApplied.CultureUI = _settings.AppCulture;
        if (plotApplied.Plot.GetPlottables().Length > 0)
        {
            plotApplied.Plot.Title(StringsRM.GetString("strPlotAppliedTitle", _settings.AppCulture));
            plotApplied.Plot.YLabel(StringsRM.GetString("strPlotAppliedYLabel", _settings.AppCulture));
            plotApplied.Plot.XLabel(StringsRM.GetString("strPlotAppliedXLabel", _settings.AppCulture));
        }

        plotFractal.CultureUI = _settings.AppCulture;
        if (plotFractal.Plot.GetPlottables().Length > 0)
        {
            plotFractal.Plot.YLabel(StringsRM.GetString("strPlotFractalYLabel", _settings.AppCulture));
            plotFractal.Plot.XLabel(StringsRM.GetString("strPlotFractalXLabel", _settings.AppCulture));
        }

        plotFFT.CultureUI = _settings.AppCulture;
        if (plotFFT.Plot.GetPlottables().Length > 0)
        {
            plotFFT.Plot.Title(StringsRM.GetString("strPlotFFTTitle", _settings.AppCulture));
            plotFFT.Plot.YLabel(_settings.PowerSpectra ? StringsRM.GetString("strPlotFFTYLabelPow", _settings.AppCulture) : StringsRM.GetString("strPlotFFTXLabelMag", _settings.AppCulture));
            plotFFT.Plot.XLabel(StringsRM.GetString("strPlotFFTXLabel", _settings.AppCulture));
        }

        if (txtStats.Text.Length > 0)
            txtStats.Text = Results.ToString(StringsRM, _settings.AppCulture);

    }

}
