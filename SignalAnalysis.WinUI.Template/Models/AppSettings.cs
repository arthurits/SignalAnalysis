using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;

namespace SignalAnalysis.Template.Models;
public partial class AppSettings : ObservableObject
{
    /// <summary>
    /// Stores the settings file name
    /// </summary>
    [JsonIgnore]
    public string FileName { get; set; } = "appsettings.json";
    [JsonIgnore]
    public string AppDataFolder { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Signal analysis", "ApplicationData");


    /// <summary>
    /// Remember window position on start up
    /// </summary>
    [JsonPropertyName("Window position")]
    [ObservableProperty]
    public partial bool WindowPosition { get; set; } = false;
    [JsonPropertyName("Window top")]
    public int WindowTop { get; set; } = 0;
    [JsonPropertyName("Window left")]
    public int WindowLeft { get; set; } = 0;
    [JsonPropertyName("Window width")]
    public int WindowWidth { get; set; } = 900;
    [JsonPropertyName("Window height")]
    public int WindowHeight { get; set; } = 760;

    /// <summary>
    /// App theme name
    /// </summary>
    [JsonPropertyName("App theme name")]
    public string ThemeName { get; set; } = ElementTheme.Default.ToString();

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
        get => AppCulture.Name;
        set => AppCulture = new System.Globalization.CultureInfo(value);
    }

    /// <summary>
    /// True if open/save dialogs should remember the user's previous path
    /// </summary>
    [JsonPropertyName("Remember path in FileDialog")]
    [ObservableProperty]
    public partial bool RememberFileDialogPath { get; set; } = true;
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
    public string DefaultOpenPath { get; set; } = $"{System.IO.Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)}\\examples";
    /// <summary>
    /// User-defined path for reading files from disk
    /// </summary>
    [JsonPropertyName("User open path")]
    public string UserOpenPath { get; set; } = $"{System.IO.Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)}\\examples";

    /// <summary>
    /// Gets or sets a value indicating whether the application should display an icon in the system tray.
    /// </summary>
    [JsonPropertyName("Show icon in tray")]
    public bool ShowTrayIcon { get; set; } = true;
    /// <summary>
    /// Gets or sets the file path for the tray icon.
    /// </summary>
    [JsonPropertyName("Tray icon file path")]
    public string TrayIconFile { get; set; } = "Assets\\AppLogo - V2.ico";
    /// <summary>
    /// Gets or sets the tooltip text for the tray icon.
    /// </summary>
    [JsonPropertyName("Tray icon tooltip text")]
    public string TrayIconTooltip { get; set; } = "Manual handling";

    /// <summary>
    /// Gets or sets a value indicating whether the application minimizes to the system tray instead of the system taskbar.
    /// </summary>
    [JsonPropertyName("Minimize to tray")]
    public bool MinimizeToTray { get; set; } = false;

    /// <summary>
    /// Launch the application at system startup.
    /// </summary>
    [JsonPropertyName("Launch app on startup")]
    public bool LaunchAtStartup { get; set; } = false;

    /// <summary>
    /// Maximum number of tasks or subtasks
    /// </summary>
    [ObservableProperty]
    [JsonPropertyName("Maximum number of tasks")]
    public partial int MaxTasks { get; set; } = 10;

    [ObservableProperty]
    [JsonPropertyName("Suplicate subtasks")]
    public partial bool DuplicateSubTasks { get; set; } = false;

    /// <summary>
    /// Maximum number of tasks for CLI. Above that, the VLI calculation will be used.
    /// </summary>
    [ObservableProperty]
    [JsonPropertyName("Maximum number of tasks for CLI")]
    public partial int MaxTasksCLI { get; set; } = 10;

    [JsonIgnore]
    public string? AppPath { get; set; } = Path.GetDirectoryName(Environment.ProcessPath);
    [JsonIgnore]
    public string DocumentPath { get; set; } = string.Empty;
    [JsonIgnore]
    public string DocumentExtension { get; set; } = string.Empty;

    public AppSettings()
    {
    }


}
