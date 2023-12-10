namespace SignalAnalysis;

public static class StringResources
{
    /// <summary>
    /// Represents a resource manager that provides convinient access to culture-specific resources at run time
    /// https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2010/y99d1cd3(v=vs.100)?WT.mc_id=DT-MVP-5003235
    /// https://stackoverflow.com/questions/32989100/how-to-make-multi-language-app-in-winforms
    /// </summary>
    public static System.Resources.ResourceManager StringRM { get; set; } = new("SignalAnalysis.localization.strings", typeof(FrmMain).Assembly);
    
    /// <summary>
    /// Specific culture from which the string resources will be retrieved
    /// </summary>
    public static System.Globalization.CultureInfo Culture { get; set; } = System.Globalization.CultureInfo.CurrentCulture;

    public static string GetString(string StringName, System.Globalization.CultureInfo Culture) => StringRM.GetString(StringName, Culture) ?? string.Empty;

    public static string FileHeaderColon => StringRM.GetString("strFileHeaderColon", Culture) ?? ": ";
    public static string FileHeader00 => StringRM.GetString("strFileHeader00", Culture) ?? "ErgoLux data";
    public static string FileHeader01 => StringRM.GetString("strFileHeader01", Culture) ?? "SignalAnalysis data";
    public static string FileHeader02 => StringRM.GetString("strFileHeader02", Culture) ?? "Start time";
    public static string FileHeader03 => StringRM.GetString("strFileHeader03", Culture) ?? "End time";
    public static string FileHeader04 => StringRM.GetString("strFileHeader04", Culture) ?? "Total measuring time";    
    public static string FileHeader05 => StringRM.GetString("strFileHeader05", Culture) ?? "Number of data points";
    public static string FileHeader06 => StringRM.GetString("strFileHeader06", Culture) ?? "Sampling frequency";
    public static string FileHeader07 => StringRM.GetString("strFileHeader07", Culture) ?? "Average";
    public static string FileHeader08 => StringRM.GetString("strFileHeader08", Culture) ?? "Maximum";
    public static string FileHeader09 => StringRM.GetString("strFileHeader09", Culture) ?? "Minimum";
    public static string FileHeader10 => StringRM.GetString("strFileHeader10", Culture) ?? "Fractal dimension";
    public static string FileHeader11 => StringRM.GetString("strFileHeader11", Culture) ?? "Fractal variance";
    public static string FileHeader12 => StringRM.GetString("strFileHeader12", Culture) ?? "Approximate entropy";
    public static string FileHeader13 => StringRM.GetString("strFileHeader13", Culture) ?? "Sample entropy";
    public static string FileHeader14 => StringRM.GetString("strFileHeader14", Culture) ?? "Shannon entropy";
    public static string FileHeader15 => StringRM.GetString("strFileHeader15", Culture) ?? "Entropy bit";
    public static string FileHeader16 => StringRM.GetString("strFileHeader16", Culture) ?? "Ideal entropy";
    public static string FileHeader17 => StringRM.GetString("strFileHeader17", Culture) ?? "Number of data series";
    public static string FileHeader18 => StringRM.GetString("strFileHeader18", Culture) ?? "Number of sensors";
    public static string FileHeader19 => StringRM.GetString("strFileHeader19", Culture) ?? "Missing an empty line.";
    public static string FileHeader20 => StringRM.GetString("strFileHeader20", Culture) ?? "Missing column headers (series names).";
    public static string FileHeader21 => StringRM.GetString("strFileHeader21", Culture) ?? "Time";
    public static string FileHeader22 => StringRM.GetString("strFileHeader22", Culture) ?? "days, day, days";
    public static string FileHeader23 => StringRM.GetString("strFileHeader23", Culture) ?? "hours, hour, hours";
    public static string FileHeader24 => StringRM.GetString("strFileHeader24", Culture) ?? "minutes, minute, minutes";
    public static string FileHeader25 => StringRM.GetString("strFileHeader25", Culture) ?? "seconds, second, seconds";
    public static string FileHeader26 => StringRM.GetString("strFileHeader26", Culture) ?? "and";
    public static string FileHeader27 => StringRM.GetString("strFileHeader27", Culture) ?? "milliseconds, millisecond, milliseconds";
    public static string FileHeader28 => StringRM.GetString("strFileHeader28", Culture) ?? "Derivative";
    public static string FileHeader29 => StringRM.GetString("strFileHeader29", Culture) ?? "Differentiation algorithm";
    public static string FileHeader30 => StringRM.GetString("strFileHeader30", Culture) ?? "Integration algorithm";
    public static string FileHeader31 => StringRM.GetString("strFileHeader31", Culture) ?? "Integral value";
    public static string FileHeader32 => StringRM.GetString("strFileHeader32", Culture) ?? "Variance";
    public static string FileHeader33 => StringRM.GetString("strFileHeader33", Culture) ?? "BoxPlot lower limit";
    public static string FileHeader34 => StringRM.GetString("strFileHeader34", Culture) ?? "BoxPlot upper limit";
    public static string FileHeader35 => StringRM.GetString("strFileHeader35", Culture) ?? "Quartile 1 (25%)";
    public static string FileHeader36 => StringRM.GetString("strFileHeader36", Culture) ?? "Quartile 2 (50%)";
    public static string FileHeader37 => StringRM.GetString("strFileHeader37", Culture) ?? "Quartile 3 (75%)";
    public static string FileHeader38 => StringRM.GetString("strFileHeader38", Culture) ?? "Shannon / Ideal";
    public static string FileHeader39 => StringRM.GetString("strFileHeader39", Culture) ?? "Entropy algorithm";
    public static string FileHeader40 => StringRM.GetString("strFileHeader40", Culture) ?? "Entropy tolerance factor";
    public static string FileHeader41 => StringRM.GetString("strFileHeader41", Culture) ?? "Entropy embedding dimension";

    public static string FileHeaderSection => StringRM.GetString("strFileHeaderSection", Culture) ?? "Section '{0}' is mis-formatted.";


    public static string FormTitle => StringRM.GetString("strFrmTitle", Culture) ?? "Signal analysis";
    public static string FrmLanguage => StringRM.GetString("strFrmLanguage", Culture) ?? "Select culture";
    public static string FrmSettings => StringRM.GetString("strFrmSettings", Culture) ?? "Settings";

    public static string FormTitleUnion => StringRM.GetString("strFrmTitleUnion", Culture) ?? " - ";


    public static string MsgBoxExitTitle => StringRM.GetString("strMsgBoxExitTitle", Culture) ?? "Exit?";
    public static string MsgBoxExit => StringRM.GetString("strMsgBoxExit", Culture) ?? "Are you sure you want to exit" + Environment.NewLine + "the application?";
    public static string MsgBoxErrorEntropy => StringRM.GetString("strMsgBoxErrorEntropy", Culture) ?? "Unexpected error while computing entropy values." + Environment.NewLine + "{0}";
    public static string MsgBoxErrorEntropyTitle => StringRM.GetString("strMsgBoxErrorEntropyTitle", Culture) ?? "Entropy error";
    public static string MsgBoxErrorDerivative => StringRM.GetString("strMsgBoxErrorDerivative", Culture) ?? "Unexpected error while computing numerical differentiation." + Environment.NewLine + "{0}";
    public static string MsgBoxErrorDerivativeTitle => StringRM.GetString("strMsgBoxErrorDerivativeTitle", Culture) ?? "Differentiation error";
    public static string MsgBoxErrorDescriptiveStats => StringRM.GetString("strMsgBoxErrorDescriptiveStats", Culture) ?? "Unexpected error while computing descriptive statistics values." + Environment.NewLine + "{0}";
    public static string MsgBoxErrorDescriptiveStatsTitle => StringRM.GetString("strMsgBoxErrorDescriptiveStatsTitle", Culture) ?? "Statistics error";
    public static string MsgBoxErrorFFT => StringRM.GetString("strMsgBoxErrorFFT", Culture) ?? "Unexpected error while computing the FFT." + Environment.NewLine + "{0}";
    public static string MsgBoxErrorFFTTitle => StringRM.GetString("strMsgBoxErrorFFTTitle", Culture) ?? "FFT error";
    public static string MsgBoxErrorFractal => StringRM.GetString("strMsgBoxErrorFractal", Culture) ?? "Unexpected error while computing the fractal dimension." + Environment.NewLine + "{0}";
    public static string MsgBoxErrorFractalTitle => StringRM.GetString("strMsgBoxErrorFractalTitle", Culture) ?? "Fractal error";
    public static string MsgBoxErrorOpenData => StringRM.GetString("strMsgBoxErrorOpenData", Culture) ?? "An unexpected error happened while opening file data." + Environment.NewLine + "Please try again later or contact the software engineer." + Environment.NewLine + "{0}";
    public static string MsgBoxErrorOpenDataTitle => StringRM.GetString("strMsgBoxErrorOpenDataTitle", Culture) ?? "Error opening data";
    public static string MsgBoxErrorSaveData => StringRM.GetString("strMsgBoxErrorSaveData", Culture) ?? "An unexpected error happened while saving file data." + Environment.NewLine + "Please try again later or contact the software engineer." + Environment.NewLine + "{0}";
    public static string MsgBoxErrorSaveDataTitle => StringRM.GetString("strMsgBoxErrorSaveDataTitle", Culture) ?? "Error saving data";
    public static string MsgBoxInitArray => StringRM.GetString("strMsgBoxInitArray", Culture) ?? "Unexpected error in 'InitializeDataArrays'." + Environment.NewLine + "{0}";
    public static string MsgBoxInitArrayTitle => StringRM.GetString("strMsgBoxInitArrayTitle", Culture) ?? "Error";
    public static string MsgBoxOKSaveData => StringRM.GetString("strMsgBoxOKSaveData", Culture) ?? "Data has been successfully saved to disk.";
    public static string MsgBoxOKSaveDataTitle => StringRM.GetString("strMsgBoxOKSaveDataTitle", Culture) ?? "Data saved";
    public static string MsgBoxNoData => StringRM.GetString("strMsgBoxNoData", Culture) ?? "There is no data available to be saved.";
    public static string MsgBoxNoDataTitle => StringRM.GetString("strMsgBoxNoDataTitle", Culture) ?? "No data";
    public static string MsgBoxTaskFractalCancel => StringRM.GetString("strMsgBoxTaskFractalCancel", Culture) ?? $"Computation of the Hausdorff-Besicovitch fractal{Environment.NewLine}dimension has been stopped.";
    public static string MsgBoxTaskFractalCancelTitle => StringRM.GetString("strMsgBoxTaskFractalCancelTitle", Culture) ?? "Cancel";
    public static string MsgBoxTaskEntropyCancel => StringRM.GetString("strMsgBoxTaskEntropyCancel", Culture) ?? $"Entropy computation has been stopped.";
    public static string MsgBoxTaskEntropyCancelTitle => StringRM.GetString("strMsgBoxTaskEntropyCancelTitle", Culture) ?? "Cancel";
    public static string MsgBoxTaskDerivativeCancel => StringRM.GetString("strMsgBoxTaskDerivativeCancel", Culture) ?? $"Numerical differentiation has been stopped.";
    public static string MsgBoxTaskDerivativeCancelTitle => StringRM.GetString("strMsgBoxTaskDerivativeCancelTitle", Culture) ?? "Cancel";

    public static string ErrorDeserialize => StringRM.GetString("strErrorDeserialize", Culture) ?? "Error loading settings file." +
            Environment.NewLine + Environment.NewLine + "{0}" +
            Environment.NewLine + Environment.NewLine + "Default values will be used instead.";
    public static string ErrorDeserializeTitle => StringRM.GetString("strErrorDeserializeTitle", Culture) ?? "Error";

    public static string OpenDlgFilter => StringRM.GetString("strOpenDlgFilter", Culture) ?? "ErgoLux files (*.elux)|*.elux|SignalAnalysis files (*.sig)|*.sig|Text files (*.txt)|*.txt|Binary files (*.bin)|*.bin|All files (*.*)|*.*";
    public static string OpenDlgTitle => StringRM.GetString("strOpenDlgTitle", Culture) ?? "Select data file";
    public static string SaveDlgFilter => StringRM.GetString("strSaveDlgFilter", Culture) ?? "Text file (*.txt)|*.txt|SignalAnalysis file (*.sig)|*.sig|Binary file (*.bin)|*.bin|Results file (*.results)|*.results|All files (*.*)|*.*";
    public static string SaveDlgTitle => StringRM.GetString("strSaveDlgTitle", Culture) ?? "Export data";

    public static string ReadDataErrorCulture => StringRM.GetString("strReadDataErrorCulture", Culture) ?? "The culture identifier string name is not valid." + Environment.NewLine + "{0}";
    public static string ReadDataErrorCultureTitle => StringRM.GetString("strReadDataErrorCultureTitle", Culture) ?? "Culture name error";
    public static string ReadDataError => StringRM.GetString("strReadDataError", Culture) ?? "Unable to read data from file." + Environment.NewLine + "{0}";
    public static string ReadDataErrorTitle => StringRM.GetString("strReadDataErrorTitle", Culture) ?? "Error";
    public static string ReadDataErrorNumber => StringRM.GetString("strReadDataErrorNumber", Culture) ?? "Invalid numeric value: {0}";
    public static string ReadDataErrorNumberTitle => StringRM.GetString("strReadDataErrorNumberTitle", Culture) ?? "Error parsing data";
    public static string ReadNotimplementedError => StringRM.GetString("strReadNotimplementedError", Culture) ?? "Unable to read data from a {0} file.";
    public static string ReadNotimplementedErrorTitle => StringRM.GetString("strReadNotimplementedErrorTitle", Culture) ?? "Not implemented";

    public static string ToolStripExit => StringRM.GetString("strToolStripExit", Culture) ?? "Exit";
    public static string ToolTipExit => StringRM.GetString("strToolTipExit", Culture) ?? "Exit the application";
    public static string ToolStripOpen => StringRM.GetString("strToolStripOpen", Culture) ?? "Open";
    public static string ToolTipOpen => StringRM.GetString("strToolTipOpen", Culture) ?? "Open data file from disk";
    public static string ToolStripExport => StringRM.GetString("strToolStripExport", Culture) ?? "Export";
    public static string ToolTipExport => StringRM.GetString("strToolTipExport", Culture) ?? "Export data and data analysis";
    public static string ToolTipCboSeries => StringRM.GetString("strToolTipCboSeries", Culture) ?? "Select data series";
    public static string ToolTipCboWindows => StringRM.GetString("strToolTipCboWindows", Culture) ?? "Select FFT window";
    public static string ToolStripSettings => StringRM.GetString("strToolStripSettings", Culture) ?? "Settings";
    public static string ToolTipSettings => StringRM.GetString("strToolTipSettings", Culture) ?? "Settings for plots, data, and UI";
    public static string ToolStripAbout => StringRM.GetString("strToolStripAbout", Culture) ?? "About";
    public static string ToolTipAbout => StringRM.GetString("strToolTipAbout", Culture) ?? "About this software";

    public static string ToolTipUILanguage => StringRM.GetString("strToolTipUILanguage", Culture) ?? "User interface language";
    public static string StatusTipPower => StringRM.GetString("strStatusTipPower", Culture) ?? "Power spectra(dB)";
    public static string StatusTipFractal => StringRM.GetString("strStatusTipFractal", Culture) ?? "Cumulative fractal dimension";
    public static string StatusTipEntropy => StringRM.GetString("strStatusTipEntropy", Culture) ?? "Approximate and sample entropy";
    public static string StatusTipCrossHair => StringRM.GetString("strStatusTipCrossHair", Culture) ?? "Plot's crosshair mode";
    public static string StatusTipDerivative => StringRM.GetString("strStatusTipDerivative", Culture) ?? "Numerical differentiation";
    public static string StatusTipIntegration => StringRM.GetString("strStatusTipIntegration", Culture) ?? "Numerical integration";

    public static string PlotOriginalTitle => StringRM.GetString("strPlotOriginalTitle", Culture) ?? "Input signal";
    public static string PlotOriginalYLabel => StringRM.GetString("strPlotOriginalYLabel", Culture) ?? "Amplitude";
    public static string PlotOriginalXLabel => StringRM.GetString("strPlotOriginalXLabel", Culture) ?? "Time (seconds)";
    public static string PlotBoxPlotTitle => StringRM.GetString("strPlotBoxPlotTitle", Culture) ?? "BoxPlot";
    public static string PlotBoxplotYLabel => StringRM.GetString("strPlotBoxplotYLabel", Culture) ?? "Amplitude";
    public static string PlotWindowTitle => StringRM.GetString("strPlotWindowTitle", Culture) ?? "{0} window";
    public static string PlotWindowYLabel => StringRM.GetString("strPlotWindowYLabel", Culture) ?? "Amplitude";
    public static string PlotWindowXLabel => StringRM.GetString("strPlotWindowXLabel", Culture) ?? "Time (seconds)";
    public static string PlotAppliedTitle => StringRM.GetString("strPlotAppliedTitle", Culture) ?? "Windowed signal";
    public static string PlotAppliedYLabel => StringRM.GetString("strPlotAppliedYLabel", Culture) ?? "Amplitude";
    public static string PlotAppliedXLabel => StringRM.GetString("strPlotAppliedXLabel", Culture) ?? "Time (seconds)";
    public static string PlotFractalDistributionTitle => StringRM.GetString("strPlotFractalDistributionTitle", Culture) ?? "Fractal dimension distribution";
    public static string PlotFractalDistributionXLabel => StringRM.GetString("strPlotFractalDisributionXLabel", Culture) ?? "Fractal dimension (H)";
    public static string PlotFractalDistributionYLabel => StringRM.GetString("strPlotFractalDisributionYLabel", Culture) ?? "Probability";
    public static string PlotFractalTitle1 => StringRM.GetString("strPlotFractalTitle1", Culture) ?? "Fractal dimension";
    public static string PlotFractalTitle2 => StringRM.GetString("strPlotFractalTitle2", Culture) ?? "(cumulative)";
    public static string PlotFractalYLabel => StringRM.GetString("strPlotFractalYLabel", Culture) ?? "Dimension (H)";
    public static string PlotFractalXLabel => StringRM.GetString("strPlotFractalXLabel", Culture) ?? "Time (seconds)";
    public static string PlotFFTTitle => StringRM.GetString("strPlotFFTTitle", Culture) ?? "Fast Fourier transform";
    public static string PlotFFTYLabelPow => StringRM.GetString("strPlotFFTYLabelPow", Culture) ?? "Power (dB)";
    public static string PlotFFTYLabelMag => StringRM.GetString("strPlotFFTYLabelMag", Culture) ?? "Magnitude (RMS²)";
    public static string PlotFFTXLabel => StringRM.GetString("strPlotFFTXLabel", Culture) ?? "Frequency (Hz)";
    public static string PlotDerivativeTitle => StringRM.GetString("strPlotDerivativeTitle", Culture) ?? "Derivative";
    public static string PlotDerivativeXLabel => StringRM.GetString("strPlotDerivativeXLabel", Culture) ?? "Time (seconds)";
    public static string PlotDerivativeYLabel1 => StringRM.GetString("strPlotDerivativeYLabel1", Culture) ?? "Amplitude / sec";
    public static string PlotDerivativeYLabel2 => StringRM.GetString("strPlotDerivativeYLabel2", Culture) ?? "Amplitude";


    public static string BtnAccept => StringRM.GetString("strBtnAccept", Culture) ?? "&Accept";
    public static string BtnCancel => StringRM.GetString("strBtnCancel", Culture) ?? "&Cancel";
    public static string BtnReset => StringRM.GetString("strBtnReset", Culture) ?? "&Reset";

    // Settings
    public static string TabPlot => StringRM.GetString("strTabPlot", Culture) ?? "Plotting";
    public static string TabGUI => StringRM.GetString("strTabGUI", Culture) ?? "User interface";
    public static string TabDerivative => StringRM.GetString("strTabDerivative", Culture) ?? "Differentiation";
    public static string TabIntegration => StringRM.GetString("strTabIntegration", Culture) ?? "Integration";
    public static string TabEntropy => StringRM.GetString("strTabEntropy", Culture) ?? "Entropy";

    public static string LblStart => StringRM.GetString("strLblStart", Culture) ?? "Array index start";
    public static string LblEnd => StringRM.GetString("strLblEnd", Culture) ?? "Array index end (max {0})";
    public static string GrpAxis => StringRM.GetString("strGrpAxis", Culture) ?? "Abscissa axis";   
    public static string RadPoints => StringRM.GetString("strRadPoints", Culture) ?? "Data points";
    public static string RadSeconds => StringRM.GetString("strRadSeconds", Culture) ?? "Seconds";
    public static string RadTime => StringRM.GetString("strRadTime", Culture) ?? "Date and time";
    public static string ChkBoxplot => StringRM.GetString("strChkBoxplot", Culture) ?? "Box plot";
    public static string ChkPower => StringRM.GetString("strChkPower", Culture) ?? "Power (dB)";
    public static string ChkCumulative => StringRM.GetString("strChkCumulative", Culture) ?? "Cumulative fractal dimension";
    public static string ChkEntropy => StringRM.GetString("strChkEntropy", Culture) ?? "Entropy (approximate && sample)";
    public static string ChkCrossHair => StringRM.GetString("strChkCrossHair", Culture) ?? "Show plots' crosshair";
    public static string GrpFFT => StringRM.GetString("strGrpFFT", Culture) ?? "FFT points";
    public static string RadRoundUp => StringRM.GetString("strRadRoundUp", Culture) ?? "Round up (add 0)";
    public static string RadRoundDown => StringRM.GetString("strRadRoundDown", Culture) ?? "Round down (trim)";

    public static string ChkComputeDerivative => StringRM.GetString("strChkComputeDerivative", Culture) ?? "Compute numerical differentiation?";
    public static string ChkExportDerivative => StringRM.GetString("strChkExportDerivative", Culture) ?? "Export derivative data?";
    public static string GrpAlgorithms => StringRM.GetString("strGrpAlgorithms", Culture) ?? "Algorithms";
    public static string DifferentiationAlgorithms => StringRM.GetString("strDifferentiationAlgorithms", Culture) ?? "Backward one-point difference, Forward one-point difference, Central three-point difference, Central five-point difference, Central seven-point difference, Central nine-point difference, Savitzky–Golay linear three point, Savitzky–Golay linear five point, Savitzky–Golay linear seven point, Savitzky–Golay linear nine point, Savitzky–Golay cubic five point, Savitzky–Golay cubic seven point, Savitzky–Golay cubic nine point";

    public static string ChkComputeIntegration => StringRM.GetString("strChkComputeIntegration", Culture) ?? "Compute numerical integration?";
    public static string ChkAbsoluteIntegral => StringRM.GetString("strChkAbsoluteIntegral", Culture) ?? "Compute the absolute-value integral?";
    public static string ChkExportIntegration => StringRM.GetString("strChkExportIntegration", Culture) ?? "Export numerical integration?";
    public static string LblIntegration => StringRM.GetString("strLblIntegration", Culture) ?? "Quadrature algorithms";
    public static string IntegrationAlgorithms => StringRM.GetString("strIntegrationAlgorithms", Culture) ?? "Left-point rule, Mid-point rule, Right-point rule, Trapezoid rule, Simpson's 1/3 rule, Simpson's 3/8 rule, Simpson's composite rule, Romberg's method";

    public static string LblFactorR => StringRM.GetString("strLblFactorR", Culture) ?? "Tolerance factor";
    public static string LblFactorM => StringRM.GetString("strLblFactorM", Culture) ?? "Embedding dimension";
    public static string LblEntropy => StringRM.GetString("strLblEntropy", Culture) ?? "Algorithms";
    public static string EntropyAlgorithms => StringRM.GetString("strEntropyAlgorithms", Culture) ?? "Brute force, Monte-Carlo";

    public static string GrpCulture => StringRM.GetString("strGrpCulture", Culture) ?? "UI and data format";
    public static string RadCurrentCulture => StringRM.GetString("strRadCurrentCulture", Culture) ?? "Current culture formatting";
    public static string RadInvariantCulture => StringRM.GetString("strRadInvariantCulture", Culture) ?? "Invariant culture formatting";
    public static string RadUserCulture => StringRM.GetString("strRadUserCulture", Culture) ?? "Select culture";
    public static string ChkWindowPos => StringRM.GetString("strChkWindowPos", Culture) ?? "Remember window position and size on startup";
    public static string ChkDlgPath => StringRM.GetString("strChkDlgPath", Culture) ?? "Remember open/save dialog previous path";
    public static string LblDataFormat => StringRM.GetString("strLblDataFormat", Culture) ?? "Numeric data-formatting string";

    public static string DlgReset => StringRM.GetString("strDlgReset", Culture) ?? "Do you want to reset all fields" +
            Environment.NewLine + "to their default values?";
    public static string DlgResetTitle => StringRM.GetString("strDlgResetTitle", Culture) ?? "Reset settings?";


    // Form about
    public static string AboutProductName => StringRM.GetString("strAboutProductName", Culture) ?? "Product name: {0}";
    public static string AboutVersion => StringRM.GetString("strAboutVersion", Culture) ?? "Version {0}";
    public static string AboutCopyright => StringRM.GetString("strAboutCopyright", Culture) ?? "Copyright {0}";
    public static string AboutCompanyName => StringRM.GetString("strAboutCompanyName", Culture) ?? "Developed by {0}";
    public static string AboutDescription => StringRM.GetString("strAboutDescription", Culture) ?? "Analysis tool featuring basic descriptive statistics, FFT (fast Fourier transform), fractal dimension, differentiation, integration, and entropy for signals (elux files as well as generic ones)." +
        Environment.NewLine + Environment.NewLine + "No commercial use allowed whatsoever.Contact the author for any inquires." +
        Environment.NewLine + Environment.NewLine + "If you find this software useful, please consider supporting it!";


    // Unused strings
    public static string LblData => StringRM.GetString("strLblData", Culture) ?? "Data-file path";
    public static string LblSeries => StringRM.GetString("strLblSeries", Culture) ?? "Select series";
    public static string LblWindow => StringRM.GetString("strLblWindow", Culture) ?? "Window";
    public static string BtnData => StringRM.GetString("strBtnData", Culture) ?? "Select data";
    public static string BtnExport => StringRM.GetString("strBtnExport", Culture) ?? "&Export";
    public static string BtnSettings => StringRM.GetString("strBtnSettings", Culture) ?? "&Settings";



}
