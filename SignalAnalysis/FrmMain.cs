namespace SignalAnalysis;

public partial class FrmMain : Form
{
    // Program data
    ClassSettings _settings = new();
    SignalStats Results = new();
    SignalData Signal = new();

    // Task variables
    Task statsTask = Task.CompletedTask;
    private CancellationTokenSource tokenSource = new();
    private CancellationToken token = CancellationToken.None;

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
                                                    StringResources.MsgBoxExit,
                                                    StringResources.MsgBoxExitTitle,
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


    private void PopulateComboWindow()
    {
        IWindow[] windows = Window.GetWindows();
        stripComboWindows.Items.AddRange(windows);
        stripComboWindows.SelectedIndex = windows.ToList().FindIndex(x => x.Name == "Hanning");
        //stripComboWindows.Text = windows[5].Name;

        // Move the focus away in order to deselect the text
        this.tableLayoutPanel1.Focus();
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
    /// Shows the measuring time in the StatusStrip control
    /// </summary>
    private void UpdateUI_MeasuringTime()
    {
        double seconds = Signal.IndexEnd - Signal.IndexStart;
        seconds /= Signal.SampleFrequency == 0 ? 1 : Signal.SampleFrequency;
        TimeSpan TimeSlice = TimeSpan.FromSeconds(seconds);
        //TimeSlice = Signal.MeasuringTime;

        if (TimeSlice == TimeSpan.Zero)
        {
            statusStripLabelEmpty.Text = string.Empty;
            statusStripLabelEmpty.ToolTipText = string.Empty;
        }
        else
        {
            statusStripLabelEmpty.Text = $"{TimeSlice.Days} {GetTimeString(StringResources.FileHeader22, TimeSlice.Days)}, " +
                $"{TimeSlice.Hours} {GetTimeString(StringResources.FileHeader23, TimeSlice.Hours)}, " +
                $"{TimeSlice.Minutes} {GetTimeString(StringResources.FileHeader24, TimeSlice.Minutes)}, " +
                $"{TimeSlice.Seconds} {GetTimeString(StringResources.FileHeader25, TimeSlice.Seconds)} " +
                $"{StringResources.FileHeader26} " +
                $"{TimeSlice.Milliseconds} {GetTimeString(StringResources.FileHeader27, TimeSlice.Milliseconds)}";
            statusStripLabelEmpty.ToolTipText = statusStripLabelEmpty.Text;
        }
    }

    /// <summary>
    /// Extracts the given substring from a string with multiple values delimited by <paramref name="strSplit"/>
    /// </summary>
    /// <param name="strValues">String with multiples values</param>
    /// <param name="time">Index from wich the substring will be returned.
    /// If this is bigger that the number of values in <paramref name="strValues"/>, then the last one is returned</param>
    /// <param name="strSplit">String used as delimiter</param>
    /// <returns>Substring at (array)position determined by <paramref name="time"/></returns>
    private string GetTimeString(string strValues, int time = 0, string strSplit = ", ")
    {
        string[] arrValues = strValues.Split(strSplit);
        int uBound = arrValues.GetUpperBound(0);
        if (time >= uBound)
            return arrValues[uBound];
        else
            return arrValues[time % uBound];
    }

    /// <summary>
    /// Updates the UI language of all controls
    /// </summary>
    private void UpdateUI_Language()
    {
        this.SuspendLayout();

        // Testing string resources
        StringResources.Culture = _settings.AppCulture;

        // Update the form's tittle
        SetFormTitle(this, String.Empty);

        UpdateUI_MeasuringTime();

        // Update ToolStrip
        toolStripMain_Exit.Text = StringResources.ToolStripExit;
        toolStripMain_Exit.ToolTipText = StringResources.ToolTipExit;
        toolStripMain_Open.Text = StringResources.ToolStripOpen;
        toolStripMain_Open.ToolTipText = StringResources.ToolTipOpen;
        toolStripMain_Export.Text = StringResources.ToolStripExport;
        toolStripMain_Export.ToolTipText = StringResources.ToolTipExport;
        stripComboSeries.ToolTipText = StringResources.ToolTipCboSeries;
        stripComboWindows.ToolTipText = StringResources.ToolTipCboWindows;
        toolStripMain_Settings.Text = StringResources.ToolStripSettings;
        toolStripMain_Settings.ToolTipText = StringResources.ToolTipSettings;
        toolStripMain_About.Text = StringResources.ToolStripAbout;
        toolStripMain_About.ToolTipText = StringResources.ToolTipAbout;

        // Update StatusStrip
        statusStripLabelCulture.Text = _settings.AppCulture.Name == String.Empty ? "Invariant" : _settings.AppCulture.Name;
        statusStripLabelCulture.ToolTipText = StringResources.ToolTipUILanguage + ":" + Environment.NewLine + _settings.AppCulture.NativeName;
        statusStripLabelExPower.ToolTipText = StringResources.StatusTipPower;
        statusStripLabelExCumulative.ToolTipText = StringResources.StatusTipFractal;
        statusStripLabelExEntropy.ToolTipText = StringResources.StatusTipEntropy;
        statusStripLabelExCrossHair.ToolTipText = StringResources.StatusTipCrossHair;
        statusStripLabelExDerivative.ToolTipText = StringResources.StatusTipDerivative;
        statusStripLabelExIntegration.ToolTipText = StringResources.StatusTipIntegration;

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
        //plotFractal.Plot.Title(StringResources.PlotFractalTitle1 +
        //    " " +
        //    (_settings.CumulativeDimension ? StringResources.PlotFractalTitle2 : String.Empty) +
        //    " (H = " + (double.IsNaN(FractalDimension.DimensionSingle) ? Results.FractalDimension : FractalDimension.DimensionSingle).ToString("0.00####", _settings.AppCulture) +
        //    " — Var(H) = " + (double.IsNaN(FractalDimension.VarianceH) ? Results.FractalVariance : FractalDimension.VarianceH).ToString("0.00####", _settings.AppCulture) + ")");

        plotFractal.Plot.Title(String.Format(_settings.AppCulture,
            "{0} {1} (H = {2:0.00####} — Var(H) = {3:0.00####})",
            StringResources.PlotFractalTitle1,
            _settings.CumulativeDimension ? StringResources.PlotFractalTitle2 : String.Empty,
            double.IsNaN(FractalDimension.DimensionSingle) ? Results.FractalDimension : FractalDimension.DimensionSingle,
            double.IsNaN(FractalDimension.VarianceH) ? Results.FractalVariance : FractalDimension.VarianceH)
            );

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

        plotDerivative.CultureUI = _settings.AppCulture;
        plotDerivative.Plot.Title(StringResources.PlotDerivativeTitle);
        plotDerivative.Plot.XAxis.Label(StringResources.PlotDerivativeXLabel);
        plotDerivative.Plot.YAxis.Label(StringResources.PlotDerivativeYLabel1);
        plotDerivative.Plot.YAxis2.Label(StringResources.PlotDerivativeYLabel2);
        plotDerivative.Plot.YAxis2.Ticks(true);
        plotDerivative.Refresh();

        // Update the results text
        if (txtStats.Text.Length > 0)
            txtStats.Text = Results.ToString(
                _settings.AppCulture,
                _settings.ComputeIntegration,
                _settings.ComputeIntegration ? StringResources.IntegrationAlgorithms.Split(", ")[(int)_settings.IntegrationAlgorithm] : string.Empty);

        this.ResumeLayout();
    }

}
