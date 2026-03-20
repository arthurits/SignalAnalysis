using System.Globalization;
using Microsoft.Windows.ApplicationModel.Resources;

using SignalAnalysis.Contracts.Services;
using SignalAnalysis.Models;

namespace SignalAnalysis.Services;

public class LocalizationService : ILocalizationService
{
    private readonly ResourceLoader _resourceLoader;
    private readonly ResourceManager _resourceManager;
    //private readonly Windows.ApplicationModel.Resources.Core.ResourceContext _defaultContextForCurrentView;
    
    public string CurrentLanguage { get; private set; }

    public event EventHandler? LanguageChanged;

    public LocalizationService()
    {
        //_resourceLoader = new();
        _resourceManager = new();
        //_defaultContextForCurrentView = Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView();
        //CurrentLanguage = ApplicationLanguages.Languages[0];
        CurrentLanguage = Microsoft.Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride ?? Windows.System.UserProfile.GlobalizationPreferences.Languages[0];
    }

    public string GetString(string key)
    {
        return _resourceLoader.GetString(key);
    }

    public string GetString(string key, string resourceMap)
    {
        var subTree = _resourceManager.MainResourceMap.TryGetSubtree(resourceMap);
        var result = subTree?.TryGetValue(key);
        return result != null ? result.ValueAsString : string.Empty;
    }

    /// <summary>
    /// Sets the application language by updating the primary language override. Notifies subscribers of the language change by invoking the <see href="LanguageChanged"> event.
    /// </summary>
    /// <param name="languageCode">Two-lettern language code from <see href="CultureInfo.Name"></param>
    /// <param name="invariantCulture">Default value when the <paramref name="languageCode"/> passed is equal to <see href="String.Empty"/></param>
    /// <param name="notifyLanguageChanged"><see langword="True"/> if the <see href="LanguageChanged"> event is invoked after the language change</param>
    public void SetAppLanguage(string languageCode, string invariantCulture = "en", bool notifyLanguageChanged =  true)
    {
        if (languageCode != CurrentLanguage)
        {
            // Update the current language only if it has changed
            CurrentLanguage = languageCode;

            // Set the primary language. This value is used to access the app's resources.
            Microsoft.Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = languageCode != string.Empty ? languageCode : invariantCulture;
            //Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().Reset();
            Windows.ApplicationModel.Resources.Core.ResourceContext.GetForViewIndependentUse().Reset();

            // Set the current thread's UI culture to the new language
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(CurrentLanguage);
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;
        }

        // If requested, notify subscribers that the language has changed
        if (notifyLanguageChanged)
        {
            LanguageChanged?.Invoke(null, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Retrieves a list of available languages from the resource files
    /// </summary>
    /// <param name="resourceFileName">Resource file name to look for keys and the languages defined</param>
    /// <param name="invariantCulture"><see langword="True"/> if the invariant culture is added as the first element in the returned list</param>
    /// <param name="loopAllKeys"><see langword="True"/> if the looping should be done for all keys in <paramref name="resourceFileName"/>, <see langword="false"/> if only the first key in <paramref name="resourceFileName"/> is used to look for the defined languages </param>
    /// <param name="targetCulture">The culture used to create the returned list</param>
    /// <returns>List containing all the languages used in the <paramref name="resourceFileName"/></returns>
    public IEnumerable<CultureOption> GetAvailableLanguages(
        string resourceFileName = "Resources",
        bool invariantCulture = true,
        bool loopAllKeys = false,
        CultureInfo? targetCulture = null)
    {
        var languages = new List<CultureOption>();
        var resourceManager = Windows.ApplicationModel.Resources.Core.ResourceManager.Current;
        var resourceMap = resourceManager.MainResourceMap;
        var uniqueLanguages = new HashSet<string>();
        
        // Use the current UI culture if the parameter is not set
        targetCulture ??= Thread.CurrentThread.CurrentUICulture;
        Thread.CurrentThread.CurrentUICulture = targetCulture; // Set the current thread's UI culture to the target culture
        //Thread.CurrentThread.CurrentCulture = targetCulture;

        // Inspect available resources in folder
        var resourceKeys = resourceMap.Keys.Where(k => k.StartsWith(resourceFileName)).ToList();

        if (invariantCulture)
        {
            var culture = CultureInfo.InvariantCulture;
            if (uniqueLanguages.Add(culture.Name))
            {
                languages.Add(new CultureOption
                {
                    DisplayName = culture.DisplayName,  // This will be "Invariant Language (Invariant Country)" and does not change with the UI culture
                    LanguageTag = culture.Name
                });
            }
        }

        if (loopAllKeys)
        {
            // This option should be used when keys might contain differente languages or qualifiers.
            foreach (var key in resourceKeys)
            {
                var qualifiers = resourceMap[key].Candidates
                    .SelectMany(c => c.Qualifiers)
                    .Where(q => q.QualifierName == "Language")
                    .Select(q => CultureInfo.GetCultureInfo(q.QualifierValue).Name)   // Normalizes the name to CultureInfo format (especially regarding upper and lower case)
                    .Distinct()
                    .ToList();

                foreach (var language in qualifiers)
                {
                    var cultureOption = createCultureOption(language);
                    if (cultureOption is not null)
                    {
                        languages.Add(cultureOption);
                    }
                    //if (TryGetCultureInfo(language, out var culture))
                    //{
                    //    if (uniqueLanguages.Add(language))
                    //    {
                    //        languages.Add(new CultureOption
                    //        {
                    //            DisplayName = culture?.DisplayName ?? string.Empty,
                    //            LanguageTag = language
                    //        });
                    //    }
                    //}
                }
            }
        }
        else
        {    // This is much faster than iterating through all keys, but it assumes that the first key is representative of all the available languages.
            var firstKey = resourceKeys.FirstOrDefault();
            if (!string.IsNullOrEmpty(firstKey))
            {
                var qualifiers = resourceMap[firstKey].Candidates
                    .SelectMany(c => c.Qualifiers)
                    .Where(q => q.QualifierName == "Language")
                    .Select(q => CultureInfo.GetCultureInfo(q.QualifierValue).Name) // Normalizes the name to CultureInfo format (especially regarding upper and lower case)
                    .Distinct()
                    .ToList();

                foreach (var language in qualifiers)
                {
                    var cultureOption = createCultureOption(language);
                    if (cultureOption is not null)
                    {
                        languages.Add(cultureOption);
                    }
                    //if (TryGetCultureInfo(language, out var culture))
                    //{
                    //    if (uniqueLanguages.Add(language))
                    //    {
                    //        languages.Add(new CultureOption
                    //        {
                    //            DisplayName = culture?.DisplayName ?? string.Empty,
                    //            LanguageTag = language
                    //        });
                    //    }
                    //}
                }
            }
        }

        CultureOption? createCultureOption(string language)
        {
            if (TryGetCultureInfo(language, out var culture))
            {
                if (uniqueLanguages.Add(language))
                {
                    return new CultureOption
                    {
                        DisplayName = culture?.DisplayName ?? string.Empty,
                        LanguageTag = language
                    };
                }
            }
            return null;
        }

        return languages;
    }

    // This method is commented out because the return values are not unique.
    //public IEnumerable<LanguageOption> GetAvailableLanguages()
    //{
    //    var languages = new List<LanguageOption>();
    //    var uniqueLanguages = new HashSet<string>();  // Evita duplicados
    //    var resourceContext = new Windows.ApplicationModel.Resources.Core.ResourceContext();

    //    foreach (var language in resourceContext.Languages)
    //    {
    //        if (language.Contains("neutral"))
    //        {
    //            continue;  // Ignorar el idioma neutral
    //        }

    //        if (TryGetCultureInfo(language, out CultureInfo? culture))
    //        {
    //            var languageOption = new LanguageOption
    //            {
    //                DisplayName = culture?.DisplayName ?? string.Empty,
    //                LanguageTag = culture.Name
    //            };

    //            if (uniqueLanguages.Add(languageOption.LanguageTag))  // Solo añade si es único
    //            {
    //                languages.Add(languageOption);
    //            }
    //        }
    //    }

    //    return languages;
    //}

    private bool TryGetCultureInfo(string language, out CultureInfo? culture)
    {
        try
        {
            culture = new CultureInfo(language);
            return true;
        }
        catch (CultureNotFoundException)
        {
            culture = null;
            return false;
        }
    }

}

