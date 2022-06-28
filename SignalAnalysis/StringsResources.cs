namespace SignalAnalysis;

public static class StringResources
{
    /// <summary>
    /// Represents a resource manager that provides convinient access to culture-specific resources at run time
    /// </summary>
    public static System.Resources.ResourceManager StringRM { get; set; } = new("SignalAnalysis.localization.strings", typeof(FrmMain).Assembly);
    
    /// <summary>
    /// Specific culture from which the string resources will be retrieved
    /// </summary>
    public static System.Globalization.CultureInfo Culture { get; set; } = System.Globalization.CultureInfo.CurrentCulture;


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
    public static string FileHeader21 => StringRM.GetString("strFileHeader21", Culture) ?? "Time";
    public static string FileHeader22 => StringRM.GetString("strFileHeader22", Culture) ?? "days";
    public static string FileHeader23 => StringRM.GetString("strFileHeader23", Culture) ?? "hours";
    public static string FileHeader24 => StringRM.GetString("strFileHeader24", Culture) ?? "minutes";
    public static string FileHeader25 => StringRM.GetString("strFileHeader25", Culture) ?? "seconds";
    public static string FileHeader26 => StringRM.GetString("strFileHeader26", Culture) ?? "and";
    public static string FileHeader27 => StringRM.GetString("strFileHeader27", Culture) ?? "milliseconds";


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


    //StringsRM.GetString("strPlotFFTXLabel", Culture) ?? "Frequency (Hz)";
    //    StringsRM.GetString("strPlotFFTYLabelPow", Culture) ?? "Power (dB)";

    //StringsRM.GetString("strMsgBoxErrorSaveDataTitle", Culture) ?? "Error saving data";
    //    StringsRM.GetString("strPlotFFTYLabelMag", Culture) ?? "Magnitude (RMS²)";

    //    String.Format(StringsRM.GetString("strFileHeaderSection", Culture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader00", Culture) ?? "ErgoLux data");
}
