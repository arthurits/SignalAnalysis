namespace SignalAnalysis;

public static class StringsResources
{
    public static System.Resources.ResourceManager StringsRM { get; set; } = new("SignalAnalysis.localization.strings", typeof(FrmMain).Assembly);
    public static System.Globalization.CultureInfo Culture { get; set; } = System.Globalization.CultureInfo.CurrentCulture;
    public static string FileHeader01 => StringsRM.GetString("strFileHeader01", Culture) ?? "SignalAnalysis data";
    public static string FileHeader02 => StringsRM.GetString("strFileHeader02", Culture) ?? "Start time";
    public static string FileHeader03 => StringsRM.GetString("strFileHeader03", Culture) ?? "End time";
    public static string FileHeader04 => StringsRM.GetString("strFileHeader04", Culture) ?? "Total measuring time";    
    public static string FileHeader05 => StringsRM.GetString("strFileHeader05", Culture) ?? "Number of data points";
    public static string FileHeader06 => StringsRM.GetString("strFileHeader06", Culture) ?? "Sampling frequency";
    public static string FileHeader07 => StringsRM.GetString("strFileHeader07", Culture) ?? "Average";
    public static string FileHeader08 => StringsRM.GetString("strFileHeader08", Culture) ?? "Maximum";
    public static string FileHeader09 => StringsRM.GetString("strFileHeader09", Culture) ?? "Minimum";
    public static string FileHeader10 => StringsRM.GetString("strFileHeader10", Culture) ?? "Fractal dimension";
    public static string FileHeader11 => StringsRM.GetString("strFileHeader11", Culture) ?? "Fractal variance";
    public static string FileHeader12 => StringsRM.GetString("strFileHeader12", Culture) ?? "Approximate entropy";
    public static string FileHeader13 => StringsRM.GetString("strFileHeader13", Culture) ?? "Sample entropy";
    public static string FileHeader14 => StringsRM.GetString("strFileHeader14", Culture) ?? "Shannon entropy";
    public static string FileHeader15 => StringsRM.GetString("strFileHeader15", Culture) ?? "Entropy bit";
    public static string FileHeader16 => StringsRM.GetString("strFileHeader16", Culture) ?? "Ideal entropy";
    public static string FileHeader17 => StringsRM.GetString("strFileHeader17", Culture) ?? "Number of data series";
    public static string FileHeader21 => StringsRM.GetString("strFileHeader21", Culture) ?? "Time";
    public static string FileHeader22 => StringsRM.GetString("strFileHeader22", Culture) ?? "days";
    public static string FileHeader23 => StringsRM.GetString("strFileHeader23", Culture) ?? "hours";
    public static string FileHeader24 => StringsRM.GetString("strFileHeader24", Culture) ?? "minutes";
    public static string FileHeader25 => StringsRM.GetString("strFileHeader25", Culture) ?? "seconds";
    public static string FileHeader26 => StringsRM.GetString("strFileHeader26", Culture) ?? "and";
    public static string FileHeader27 => StringsRM.GetString("strFileHeader27", Culture) ?? "milliseconds";

    //StringsRM.GetString("strPlotFFTXLabel", Culture) ?? "Frequency (Hz)";
    //    StringsRM.GetString("strPlotFFTYLabelPow", Culture) ?? "Power (dB)";

    //StringsRM.GetString("strMsgBoxErrorSaveDataTitle", Culture) ?? "Error saving data";
    //    StringsRM.GetString("strPlotFFTYLabelMag", Culture) ?? "Magnitude (RMS²)";

    //    String.Format(StringsRM.GetString("strFileHeaderSection", Culture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader00", Culture) ?? "ErgoLux data");
}
