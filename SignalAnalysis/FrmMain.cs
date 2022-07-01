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
        InitializeComponent();
        InitializeToolStripPanel();
        InitializeStatusStrip();
        InitializeMenu();

        PopulateComboWindow();

        this.plotOriginal.SnapToPoint = true;
        this.plotWindow.SnapToPoint = true;
        this.plotApplied.SnapToPoint = true;
        this.plotFractal.SnapToPoint = true;
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
                                                    StringResources.MsgBoxExitTitle,
                                                    StringResources.MsgBoxExit,
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

        //statusStripLabelExEntropy.Checked = false;
        //_settings.Entropy = false;

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
        string strSep = StringResources.FormTitleUnion;
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
        frm.Text = StringResources.FormTitle + strText;
    }

    /// <summary>
    /// Updates the UI language of all controls
    /// </summary>
    private void UpdateUI_Language()
    {
        this.SuspendLayout();

        // Testing string resources
        //StringResources.StringRM = StringsRM;
        StringResources.Culture = _settings.AppCulture;


        // Update the form's tittle
        SetFormTitle(this, String.Empty);

        // Update ToolStrip
        this.toolStripMain_Exit.Text = StringResources.ToolStripExit;
        this.toolStripMain_Exit.ToolTipText = StringResources.ToolTipExit;
        this.toolStripMain_Open.Text = StringResources.ToolStripOpen;
        this.toolStripMain_Open.ToolTipText = StringResources.ToolTipOpen;
        this.toolStripMain_Export.Text = StringResources.ToolStripExport;
        this.toolStripMain_Export.ToolTipText = StringResources.ToolTipExport;
        stripComboSeries.ToolTipText = StringResources.ToolTipCboSeries;
        stripComboWindows.ToolTipText = StringResources.ToolTipCboWindows;
        this.toolStripMain_Settings.Text = StringResources.ToolStripSettings;
        this.toolStripMain_Settings.ToolTipText = StringResources.ToolTipSettings;
        this.toolStripMain_About.Text = StringResources.ToolStripAbout;
        this.toolStripMain_About.ToolTipText = StringResources.ToolTipAbout;

        // Update StatusStrip
        statusStripLabelCulture.Text = _settings.AppCulture.Name == String.Empty ? "Invariant" : _settings.AppCulture.Name;
        statusStripLabelCulture.ToolTipText = StringResources.ToolTipUILanguage;
        statusStripLabelExPower.ToolTipText = StringResources.StatusTipPower;
        statusStripLabelExCumulative.ToolTipText = StringResources.StatusTipFractal;
        statusStripLabelExEntropy.ToolTipText = StringResources.StatusTipEntropy;
        statusStripLabelExCrossHair.ToolTipText = StringResources.StatusTipCrossHair;

        // Update plots
        plotOriginal.CultureUI = _settings.AppCulture;
        plotOriginal.Plot.Title(StringResources.PlotOriginalTitle);
        plotOriginal.Plot.YLabel(StringResources.PlotOriginalYLabel);
        plotOriginal.Plot.XLabel(StringResources.PlotOriginalXLabel);
        plotOriginal.Refresh();

        plotWindow.CultureUI = _settings.AppCulture;
        IWindow window = (IWindow)stripComboWindows.SelectedItem;
        if (window is not null)
            plotWindow.Plot.Title(String.Format(StringResources.PlotWindowTitle, window.Name));
        plotWindow.Plot.YLabel(StringResources.PlotWindowYLabel);
        plotWindow.Plot.XLabel(StringResources.PlotWindowXLabel);
        plotWindow.Refresh();

        plotApplied.CultureUI = _settings.AppCulture;
        plotApplied.Plot.Title(StringResources.PlotAppliedTitle);
        plotApplied.Plot.YLabel(StringResources.PlotAppliedYLabel);
        plotApplied.Plot.XLabel(StringResources.PlotAppliedXLabel);
        plotApplied.Refresh();

        plotFractal.CultureUI = _settings.AppCulture;
        plotFractal.Plot.Title(StringResources.PlotFractalTitle1 +
            " " +
            (_settings.CumulativeDimension ? StringResources.PlotFractalTitle2 : String.Empty) +
            " (H = " + (double.IsNaN(FractalDimension.DimensionSingle) ? Results.FractalDimension : FractalDimension.DimensionSingle).ToString("0.00####", _settings.AppCulture) +
            " — Var(H) = " + (double.IsNaN(FractalDimension.VarianceH) ? Results.FractalVariance : FractalDimension.VarianceH).ToString("0.00####", _settings.AppCulture) + ")");
        plotFractal.Plot.YLabel(StringResources.PlotFractalYLabel);
        plotFractal.Plot.XLabel(StringResources.PlotFractalXLabel);
        plotFractal.Refresh();

        plotFractalDistribution.CultureUI = _settings.AppCulture;
        plotFractalDistribution.Plot.Title(StringResources.PlotFractalDistributionTitle);
        plotFractalDistribution.Plot.XLabel(StringResources.PlotFractalDistributionXLabel);
        plotFractalDistribution.Plot.YLabel(StringResources.PlotFractalDistributionYLabel);
        plotFractalDistribution.Refresh();

        plotFFT.CultureUI = _settings.AppCulture;
        plotFFT.Plot.Title(StringResources.PlotFFTTitle);
        plotFFT.Plot.YLabel(_settings.PowerSpectra ? StringResources.PlotFFTYLabelPow : StringResources.PlotFFTYLabelMag);
        plotFFT.Plot.XLabel(StringResources.PlotFFTXLabel);
        plotFFT.Refresh();

        // Update the results text
        if (txtStats.Text.Length > 0)
            txtStats.Text = Results.ToString(_settings.AppCulture);

        this.ResumeLayout();
    }
}
