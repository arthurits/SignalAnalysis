using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using $safeprojectname$.Contracts.Services;
using $safeprojectname$.Helpers;
using $safeprojectname$.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.System.UserProfile;

namespace $safeprojectname$.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    // Services
    private readonly IThemeSelectorService _themeSelectorService;
    private readonly ILocalizationService _localizationService;
    private readonly ILocalSettingsService<AppSettings> _settingsService;
    private readonly IMainWindowService _mainWindowService;
    private AppSettings _appSettings;
    private readonly HashSet<string> _pocoSettings;

    [ObservableProperty]
    public partial bool WindowPosition { get; set; }
    [ObservableProperty]
    public partial int WindowTop { get; set; }
    [ObservableProperty]
    public partial int WindowLeft { get; set; }
    [ObservableProperty]
    public partial int WindowWidth { get; set; }
    [ObservableProperty]
    public partial int WindowHeight { get; set; }


    [ObservableProperty]
    public partial int Theme { get; set; } = 0;
    public ObservableCollection<ComboBoxData> ColorModes { get; set; } = [];

    public ObservableCollection<CultureOption> AvailableLanguages { get; set; } = [];

    [ObservableProperty]
    public partial int SelectedLanguageIndex { get; set; } = -1;

    [ObservableProperty]
    public partial bool RememberFileDialogPath { get; set; }

    public string WindowSizeDescription => string.Format(StrWindowSize, _mainWindowService.WindowWidth, _mainWindowService.WindowHeight);

    public string WindowPositionDescription => string.Format(StrWindowPosition, _mainWindowService.WindowTop, _mainWindowService.WindowLeft);

    [ObservableProperty]
    public partial bool IsResetVisible { get; set; } = false;


    public SettingsViewModel(
        IThemeSelectorService themeSelectorService,
        ILocalSettingsService<AppSettings> settings,
        ILocalizationService localizationService,
        IMainWindowService mainWindowService)
    {
        // Settings service
        _settingsService = settings;
        _appSettings = settings.GetValues;

        // Get settings and update the observable properties
        WindowPosition = _appSettings.WindowPosition;
        RememberFileDialogPath = _appSettings.RememberFileDialogPath;

        // Theme service
        _themeSelectorService = themeSelectorService;
        Theme = (int)Enum.Parse<ElementTheme>(_appSettings.ThemeName);

        // Subscribe to localization service events
        _localizationService = localizationService;
        _localizationService.LanguageChanged += OnLanguageChanged;

        _mainWindowService = mainWindowService;
        _mainWindowService.PropertyChanged += MainWindow_Changed;

        // Populate the available languages
        var cultures = _localizationService.GetAvailableLanguages();
        var cultureList = cultures.ToList();
        AvailableLanguages = new ObservableCollection<CultureOption>(cultureList);

        var language = Microsoft.Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride;
        var currentLang = !string.IsNullOrEmpty(language) ? language : GlobalizationPreferences.Languages[0];

        // Look for an exact match first, then check for prefixes.
        var selectedCultureIndex = cultureList.FindIndex(lang =>
                 lang.LanguageTag == currentLang ||
                 currentLang.StartsWith(lang.LanguageTag + "-") ||
                 lang.LanguageTag.StartsWith(currentLang + "-"));

        SelectedLanguageIndex = cultureList.Count > 0 ? Math.Max(0, selectedCultureIndex) : -1;

        // Retrieve the properties from the AppSettings POCO
        _pocoSettings = typeof(AppSettings)
            .GetProperties()
            .Where(p => p.CanRead && p.CanWrite)
            .Select(p => p.Name)
            .ToHashSet();

        // Initialize the ViewModel properties with the POCO values
        foreach (var propName in _pocoSettings)
        {
            var vmProp = GetType().GetProperty(propName);
            var pocoProp = _appSettings.GetType().GetProperty(propName);

            if (vmProp is null || pocoProp is null)
            {
                continue; // Skip if the property does not exist in either the ViewModel or the POCO
            }
            vmProp!.SetValue(this, pocoProp!.GetValue(_appSettings));
        }
    }

    public void Dispose()
    {
        _mainWindowService.PropertyChanged   -= MainWindow_Changed;
        _localizationService.LanguageChanged -= OnLanguageChanged;
    }

    private void MainWindow_Changed(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(_mainWindowService.WindowWidth) or nameof(_mainWindowService.WindowHeight))
        {
            OnPropertyChanged(nameof(WindowSizeDescription));
        }
        if (e.PropertyName is nameof(_mainWindowService.WindowLeft) or nameof(_mainWindowService.WindowTop))
        {
            OnPropertyChanged(nameof(WindowPositionDescription));
        }
    }

    partial void OnWindowLeftChanged(int oldValue, int newValue)
    {
        OnPropertyChanged(nameof(WindowPositionDescription));
    }
    partial void OnWindowTopChanged(int oldValue, int newValue)
    {
        OnPropertyChanged(nameof(WindowPositionDescription));
    }
    partial void OnWindowWidthChanged(int oldValue, int newValue)
    {
        OnPropertyChanged(nameof(WindowSizeDescription));
    }
    partial void OnWindowHeightChanged(int oldValue, int newValue)
    {
        OnPropertyChanged(nameof(WindowSizeDescription));
    }


    partial void OnSelectedLanguageIndexChanged(int oldValue, int newValue)
    {
        if (newValue >= 0 && newValue < AvailableLanguages.Count)
        {
            var selected = AvailableLanguages[newValue];
            _localizationService.SetAppLanguage(selected.LanguageTag);

            // Refrescar lista y mantener selección
            var updated = _localizationService.GetAvailableLanguages();
            AvailableLanguages?.Clear();
            foreach (var language in updated)
            {
                AvailableLanguages?.Add(language);
            }
            //AvailableLanguages = new ObservableCollection<CultureOption>(updated.ToList());
            //SelectedLanguageIndex = updated.ToList().FindIndex(c => c.LanguageTag == selected.LanguageTag);
            //SelectedLanguageIndex = newValue; // Re-assign to trigger property change
        }
    }

    private void ThemeSelectorChanged(string? themeName)
    {
        if (Enum.TryParse(themeName, out ElementTheme theme) is true)
        {
            _themeSelectorService.SetTheme(theme);
        }
    }

    partial void OnThemeChanged(int value)
    {
        // Update the theme in the settings
        ThemeSelectorChanged(((ElementTheme)Theme).ToString());
        _appSettings.ThemeName = ((ElementTheme)Theme).ToString();
    }

    private void AppSettings_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_appSettings.WindowLeft) || e.PropertyName == nameof(_appSettings.WindowTop))
        {
            OnPropertyChanged(nameof(WindowPositionDescription));
        }
        else if (e.PropertyName == nameof(_appSettings.WindowWidth) || e.PropertyName == nameof(_appSettings.WindowHeight))
        {
            OnPropertyChanged(nameof(WindowSizeDescription));
        }
        else
        {
            if (IsResetVisible == false)
            {
                IsResetVisible = true;
            }
        }
    }

    /// <summary>
    /// Override OnPropertyChanged to handle custom behaviour
    /// </summary>
    /// <param name="e"></param>
    /// <see href="https://stackoverflow.com/questions/71857854/how-to-call-a-method-after-a-property-update"/>
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        // Call the base function
        base.OnPropertyChanged(e);

        // If the AppSettings POCO is not initialized or the property name is null, do nothing
        if (_pocoSettings is null || e.PropertyName is null)
        {
            return;
        }

        // We are only interested in properties that are part of the AppSettings POCO
        if (!_pocoSettings.Contains(e.PropertyName!))
        {
            return;
        }

        // Get the property value from the ViewModel
        var vmProp = GetType().GetProperty(e.PropertyName!);
        var newValue = vmProp?.GetValue(this);

        // Copy the value to the AppSettings POCO
        var pocoProp = _appSettings.GetType().GetProperty(e.PropertyName!);
        pocoProp?.SetValue(_appSettings, newValue);

        // Notify the settings service that a setting has changed
        _settingsService.NotifySettingChanged(e.PropertyName!, newValue);

        // Set the reset button visibility
        if (IsResetVisible == false)
        {
            IsResetVisible = true;
        }
    }

    [RelayCommand]
    private async Task ResetSettings()
    {
        var result = await MessageBox.Show(
            messageBoxText: "MsgBoxResetSettingsContent".GetLocalized("MessageBox"),
            caption: "MsgBoxResetSettingsTitle".GetLocalized("MessageBox"),
            primaryButtonText: "MsgBoxResetSettingsPrimary".GetLocalized("MessageBox"),
            closeButtonText: "MsgBoxResetSettingsClose".GetLocalized("MessageBox"),
            defaultButton: MessageBox.MessageBoxButtonDefault.CloseButton,
            icon: MessageBox.MessageBoxImage.Question);

        if (result == ContentDialogResult.Primary)
        {
            _appSettings = new AppSettings();
            Theme = (int)Enum.Parse<ElementTheme>(_appSettings.ThemeName);

            // Hide reset button until a setting has changed
            IsResetVisible = false;
        }
    }
}
