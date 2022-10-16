using System.Text.Json.Serialization;

namespace SignalAnalysis;

public class ClassSettings
{
    /// <summary>
    /// Stores the settings file name
    /// </summary>
    [JsonIgnore]
    public string FileName { get; set; } = "configuration.json";

    /// <summary>
    /// Remember window position on start up
    /// </summary>
    [JsonPropertyName("Window position")]
    public bool WindowPosition { get; set; } = false;
    [JsonPropertyName("Window top")]
    public int WindowTop { get; set; } = 0;
    [JsonPropertyName("Window left")]
    public int WindowLeft { get; set; } = 0;
    [JsonPropertyName("Window width")]
    public int WindowWidth { get; set; } = 900;
    [JsonPropertyName("Window height")]
    public int WindowHeight { get; set; } = 760;

    /// <summary>
    /// Array starting index analysis
    /// </summary>
    [JsonPropertyName("Array index start")]
    public int IndexStart { get; set; } = 0;
    /// <summary>
    /// Array ending index analysis
    /// </summary>
    [JsonPropertyName("Array index end")]
    public int IndexEnd { get; set; } = 0;
    /// <summary>
    /// True if the power spectra is to be computed
    /// </summary>
    [JsonPropertyName("Compute power spectra")]
    public bool PowerSpectra { get; set; } = true;
    /// <summary>
    /// True if the cumulative (incremental) fractal dimension is to be computed
    /// This can be CPU-consuming for large datasets
    /// </summary>
    [JsonPropertyName("Compute cumulative fractal")]
    public bool CumulativeDimension { get; set; } = false;
    /// <summary>
    /// True if both the application and sample entropies are to be computed
    /// </summary>
    [JsonPropertyName("Compute entropies")]
    public bool Entropy { get; set; } = false;
    /// <summary>
    /// True if a crosshair is shown on the plots
    /// </summary>
    [JsonPropertyName("Show plots crosshair")]
    public bool CrossHair { get; set; } = false;
    /// <summary>
    /// Abscissa axis type
    /// </summary>
    [JsonPropertyName("Plots abscissa axis")]
    public AxisType AxisType { get; set; } = AxisType.Seconds;

    /// <summary>
    /// Culture used throughout the app
    /// </summary>
    [JsonIgnore]
    public System.Globalization.CultureInfo AppCulture { get; set; } = System.Globalization.CultureInfo.CurrentCulture;
    /// <summary>
    /// Define the culture used throughout the app by asigning a culture string name
    /// </summary>
    [JsonPropertyName("Culture name")]
    public string AppCultureName
    {
        get { return AppCulture.Name; }
        set { AppCulture = new System.Globalization.CultureInfo(value); }
    }
    /// <summary>
    /// Milliseconds format
    /// </summary>
    [JsonPropertyName("Millisecons formatting string")]
    public string MillisecondsFormat
    {
        //get { return $"$1{AppCulture.NumberFormat.NumberDecimalSeparator}fff"; }
        get { return GetMillisecondsFormat(AppCulture); }
    }

    /// <summary>
    /// Numeric data formatting string
    /// </summary>
    [JsonPropertyName("Numeric data-formatting string")]
    public string DataFormat { get; set; } = "#0.0##";

    /// <summary>
    /// True if open/save dialogs should remember the user's previous path
    /// </summary>
    [JsonPropertyName("Remember path in FileDialog")]
    public bool RememberFileDialogPath { get; set; } = true;
    /// <summary>
    /// Default path for saving files to disk
    /// </summary>
    [JsonPropertyName("Default save path")]
    public string DefaultSavePath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
    /// <summary>
    /// User-defined path for saving files to disk
    /// </summary>
    [JsonPropertyName("User save path")]
    public string UserSavePath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
    /// <summary>
    /// Default path for reading files from disk
    /// </summary>
    [JsonPropertyName("Default open path")]
    public string DefaultOpenPath { get; set; } = $"{System.IO.Path.GetDirectoryName(Application.ExecutablePath)}\\examples";
    /// <summary>
    /// User-defined path for reading files from disk
    /// </summary>
    [JsonPropertyName("User open path")]
    public string UserOpenPath { get; set; } = $"{System.IO.Path.GetDirectoryName(Application.ExecutablePath)}\\examples";

    [JsonIgnore]
    public string? AppPath { get; set; } = Path.GetDirectoryName(Environment.ProcessPath);

    public ClassSettings()
    {
    }

    public static string GetMillisecondsFormat(System.Globalization.CultureInfo culture)
    {
        return $"$1{culture.NumberFormat.NumberDecimalSeparator}fff";
    }
}

public enum AxisType
{
    Seconds,
    Points,
    DateTime
}

