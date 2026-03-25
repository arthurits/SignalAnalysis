using SignalAnalysis.Contracts.Services;
using SignalAnalysis.ViewModels;

using Microsoft.UI.Xaml.Controls;
using Windows.System.UserProfile;

namespace SignalAnalysis.Template.Views;

public sealed partial class SettingsPage : Page, IDisposable
{
    public SettingsViewModel ViewModel { get; }
    private readonly ILocalizationService _localizationService;

    public SettingsPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<SettingsViewModel>();
        _localizationService = App.GetService<ILocalizationService>();
        DataContext = ViewModel;
    }

    public void Dispose()
    {

    }
    /// <summary>
    /// Load available cultures into the ComboBox. Sets the default culture based on the app's primary language.
    /// </summary>
    private void LoadCultures()
    {
        // Get the list of available languages from the localization service
        var cultures = _localizationService.GetAvailableLanguages();
        //var languagesInGerman = _localizationService.GetAvailableLanguages(targetCulture: new CultureInfo("de"));

        // Asing the list to the ComboBox
        //LanguageComboBox.ItemsSource = cultures;

        // Set the default culture based on the app's primary language
        var language = Microsoft.Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride;
        var defaultCulture = !string.IsNullOrEmpty(language) ? language : GlobalizationPreferences.Languages[0];

        //// Search for the selected culture in the list
        //var selectedCulture = cultures.FirstOrDefault(lang => lang.LanguageTag == defaultCulture);

        //// Si no hay coincidencia exacta, buscar por prefijo (ej. "es" → "es-ES")
        //selectedCulture ??= cultures.FirstOrDefault(lang => defaultCulture.StartsWith(lang.LanguageTag + "-") || lang.LanguageTag.StartsWith(defaultCulture + "-"));


        // Search for the selected culture in the list
        var cultureList = cultures.ToList();

        // Look for an exact match first, then check for prefixes.
        var selectedCultureIndex = cultureList.FindIndex(lang =>
         lang.LanguageTag == defaultCulture ||
         defaultCulture.StartsWith(lang.LanguageTag + "-") ||
         lang.LanguageTag.StartsWith(defaultCulture + "-"));

        var selectedCulture = selectedCultureIndex >= 0 ? cultureList[selectedCultureIndex] : null;

        //if (selectedCulture is not null)
        //{
        //    //LanguageComboBox.SelectedItem = selectedCulture;
        //    LanguageComboBox.SelectedIndex = selectedCultureIndex;
        //}
        //else
        //{
        //    LanguageComboBox.SelectedIndex = 0; // Fallback to the first item if not found
        //}
    }
}
