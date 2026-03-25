using Microsoft.Windows.ApplicationModel.Resources;
using SignalAnalysis.Contracts.Services;
using SignalAnalysis.Helpers;
using SignalAnalysis.Models;
using System.Globalization;
using System.Reflection;
using Windows.ApplicationModel;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.Storage;

namespace SignalAnalysis.Template.Services;

public class LocalizationService : ILocalizationService
{
    //private readonly ResourceLoader _resourceLoader;
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
        //return _resourceLoader.GetString(key);

        // Obtener el ResourceMap principal (puedes usar GetSubtree para mapas concretos)
        var mainMap = _resourceManager.MainResourceMap;

        // La clave suele ser "Resources/<resourceName>" o "<FileName>/<resourceName>"
        // Ajusta el path según la ubicación de tu .resw
        var entry = mainMap.TryGetValue(key);

        return entry?.ValueAsString ?? string.Empty;
    }

    public string GetString(string key, string resourceMap)
    {
        var subTree = _resourceManager.MainResourceMap.TryGetSubtree(resourceMap);
        var result = subTree?.TryGetValue(key);
        return result != null ? result.ValueAsString : string.Empty;
    }

    //private void DumpResourceMaps()
    //{
    //    var rm = new ResourceManager();
    //    var mainMap = rm.MainResourceMap;

    //    foreach (var key in mainMap.Keys)
    //    {
    //        Debug.WriteLine(key);
    //    }

    //    // Para ver las claves dentro de un subtree
    //    var subtree = mainMap.GetSubtree("Resources");
    //    foreach (var k in subtree.Keys)
    //    {
    //        Debug.WriteLine(k);
    //    }
    //}

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
            //Windows.ApplicationModel.Resources.Core.ResourceContext.GetForViewIndependentUse().Reset();

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
        string resourceFileName = "Shell",
        bool invariantCulture = true,
        bool loopAllKeys = false,
        CultureInfo? targetCulture = null)
    {
        var languages = new List<CultureOption>();
        //var resourceManager = Windows.ApplicationModel.Resources.Core.ResourceManager.Current;
        //var resourceMap = resourceManager.MainResourceMap;
        var resourceMap = _resourceManager.MainResourceMap.TryGetSubtree("About");
        var uniqueLanguages = new HashSet<string>();
        
        // Use the current UI culture if the parameter is not set
        targetCulture ??= Thread.CurrentThread.CurrentUICulture;
        Thread.CurrentThread.CurrentUICulture = targetCulture; // Set the current thread's UI culture to the target culture
        //Thread.CurrentThread.CurrentCulture = targetCulture;

        // Inspect available resources in folder
        //var resourceKeys = resourceMap.Keys.Where(k => k.StartsWith(resourceFileName)).ToList();
        var resourceKeys = new List<string>();
        for (uint i = 0; i < resourceMap.ResourceCount; i++)
        {
            var kvp = resourceMap.GetValueByIndex(i); // KeyValuePair<TKey, TValue>
            if (kvp.Key.StartsWith(resourceFileName, StringComparison.OrdinalIgnoreCase))
            {
                resourceKeys.Add(kvp.Key);
            }
        }

        for (uint i = 0; i < resourceMap.ResourceCount; i++)
        { 
            var kvp = resourceMap.GetValueByIndex(i);
            var named = resourceMap.TryGetValue(kvp.Key);
            foreach (var candidate in named.QualifierValues)
            {
                    if (string.Equals(candidate.Key, "Language", StringComparison.OrdinalIgnoreCase))
                    {
                        var tag = candidate.Value; // p.ej. "es-ES"
                                                   // normalizar y añadir a langs
                    var cultureOption = createCultureOption(tag);
                    if (cultureOption is not null)
                    {
                        languages.Add(cultureOption);
                    }
                }
            }
        }

        if (RuntimeHelper.IsMSIX)
        {
            // Si la app se ejecuta como MSIX, se pueden obtener los idiomas disponibles inspeccionando las carpetas de recursos
            var test = GetLanguagesFromPackageAsync();
            //result.AddRange(languages);
        }
        else
        {
            // Si no es MSIX, se puede usar el método tradicional (aunque no siempre es fiable)
            var test = GetAvailableLanguagesUnpackaged();
            //result.AddRange(languages);
        }
        

        if (invariantCulture)
        {
            var cultureOption = createCultureOption(string.Empty);
            if (cultureOption is not null) { languages.Add(cultureOption); }

            //var culture = CultureInfo.InvariantCulture;
            //if (uniqueLanguages.Add(culture.Name))
            //{
            //    languages.Add(new CultureOption
            //    {
            //        DisplayName = culture.DisplayName,  // This will be "Invariant Language (Invariant Country)" and does not change with the UI culture
            //        LanguageTag = culture.Name
            //    });
            //}
        }

        //if (loopAllKeys)
        //{
        //    // This option should be used when keys might contain differente languages or qualifiers.
        //    foreach (var key in resourceKeys)
        //    {
        //        var qualifiers = resourceMap[key].Candidates
        //            .SelectMany(c => c.Qualifiers)
        //            .Where(q => q.QualifierName == "Language")
        //            .Select(q => CultureInfo.GetCultureInfo(q.QualifierValue).Name)   // Normalizes the name to CultureInfo format (especially regarding upper and lower case)
        //            .Distinct()
        //            .ToList();

        //        foreach (var language in qualifiers)
        //        {
        //            var cultureOption = createCultureOption(language);
        //            if (cultureOption is not null)
        //            {
        //                languages.Add(cultureOption);
        //            }
        //            //if (TryGetCultureInfo(language, out var culture))
        //            //{
        //            //    if (uniqueLanguages.Add(language))
        //            //    {
        //            //        languages.Add(new CultureOption
        //            //        {
        //            //            DisplayName = culture?.DisplayName ?? string.Empty,
        //            //            LanguageTag = language
        //            //        });
        //            //    }
        //            //}
        //        }
        //    }
        //}
        //else
        //{    // This is much faster than iterating through all keys, but it assumes that the first key is representative of all the available languages.
        //    var firstKey = resourceKeys.FirstOrDefault();
        //    if (!string.IsNullOrEmpty(firstKey))
        //    {
        //        var qualifiers = resourceMap[firstKey].Candidates
        //            .SelectMany(c => c.Qualifiers)
        //            .Where(q => q.QualifierName == "Language")
        //            .Select(q => CultureInfo.GetCultureInfo(q.QualifierValue).Name) // Normalizes the name to CultureInfo format (especially regarding upper and lower case)
        //            .Distinct()
        //            .ToList();

        //        foreach (var language in qualifiers)
        //        {
        //            var cultureOption = createCultureOption(language);
        //            if (cultureOption is not null)
        //            {
        //                languages.Add(cultureOption);
        //            }
        //            //if (TryGetCultureInfo(language, out var culture))
        //            //{
        //            //    if (uniqueLanguages.Add(language))
        //            //    {
        //            //        languages.Add(new CultureOption
        //            //        {
        //            //            DisplayName = culture?.DisplayName ?? string.Empty,
        //            //            LanguageTag = language
        //            //        });
        //            //    }
        //            //}
        //        }
        //    }
        //}

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

    public async Task<IEnumerable<CultureOption>> GetLanguagesFromPackageAsync(string resourceFolderName = "Strings")
    {
        var result = new List<CultureOption>();
        var unique = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Carpeta raíz del paquete
        StorageFolder installed = Package.Current.InstalledLocation;

        // Intentar abrir la carpeta "Strings" (puede variar según tu estructura)
        StorageFolder? stringsFolder = null;
        try
        {
            stringsFolder = await installed.GetFolderAsync(resourceFolderName);
        }
        catch
        {
            // No existe la carpeta Strings; devolver vacío o usar otra ruta
            return result;
        }

        // Enumerar subcarpetas (cada una suele ser un language tag)
        var subfolders = await stringsFolder.GetFoldersAsync();
        foreach (var folder in subfolders)
        {
            var tag = folder.Name; // p.ej. "es-ES" o "en"
            if (string.IsNullOrWhiteSpace(tag)) continue;

            try
            {
                var ci = CultureInfo.GetCultureInfo(tag);
                if (unique.Add(ci.Name))
                {
                    result.Add(new CultureOption
                    {
                        LanguageTag = ci.Name,
                        DisplayName = ci.DisplayName
                    });
                }
            }
            catch (CultureNotFoundException)
            {
                // Si no es un tag de cultura válido, puedes ignorarlo o añadirlo crudo
                if (unique.Add(tag))
                {
                    result.Add(new CultureOption
                    {
                        LanguageTag = tag,
                        DisplayName = tag
                    });
                }
            }
        }

        // Orden opcional
        return result.OrderBy(r => r.DisplayName).ToList();
    }

    private IEnumerable<CultureOption> GetAvailableLanguagesUnpackaged(string resourceFolderName = "Strings")
    {
        var result = new List<CultureOption>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // 2) Fallback unpackaged: usar AppContext.BaseDirectory o la ubicación del ensamblado
        string baseDir = AppContext.BaseDirectory;
        if (string.IsNullOrEmpty(baseDir))
        {
            baseDir = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location ?? Assembly.GetExecutingAssembly().Location) ?? Environment.CurrentDirectory;
        }

        // Buscar carpeta "Strings" bajo baseDir (estructura típica de .resw)
        var stringsPath = Path.Combine(baseDir, resourceFolderName);
        if (Directory.Exists(stringsPath))
        {
            foreach (var dir in Directory.GetDirectories(stringsPath))
            {
                var folderName = Path.GetFileName(dir);
                AddCultureFromFolderName(folderName, result, seen);
            }
        }

        // 3) Complemento: buscar carpetas de satellite assemblies (p.ej. "es-ES\MyApp.resources.dll")
        // Recorremos subcarpetas directas de baseDir y comprobamos si contienen *.resources.dll
        foreach (var dir in Directory.GetDirectories(baseDir))
        {
            var folderName = Path.GetFileName(dir);
            try
            {
                // validar si folderName es un culture tag
                var ci = CultureInfo.GetCultureInfo(folderName);
                // comprobar si hay algún archivo *.resources.dll dentro
                var hasResourcesDll = Directory.EnumerateFiles(dir, "*.resources.dll", SearchOption.TopDirectoryOnly).Any();
                if (hasResourcesDll)
                {
                    AddCultureFromFolderName(folderName, result, seen);
                }
            }
            catch (CultureNotFoundException)
            {
                // no es un culture tag, ignorar
            }
        }

        // 4) Si no se ha encontrado nada, como último recurso devolver la cultura actual
        if (result.Count == 0)
        {
            var cur = CultureInfo.CurrentUICulture;
            result.Add(new CultureOption { LanguageTag = cur.Name, DisplayName = cur.DisplayName });
        }

        return result.OrderBy(r => r.DisplayName).ToList();
    }

    private void AddCultureFromFolderName(string folderName, List<CultureOption> list, HashSet<string> seen)
    {
        if (string.IsNullOrWhiteSpace(folderName)) return;
        try
        {
            var ci = CultureInfo.GetCultureInfo(folderName);
            if (seen.Add(ci.Name))
            {
                list.Add(new CultureOption { LanguageTag = ci.Name, DisplayName = ci.DisplayName });
            }
        }
        catch (CultureNotFoundException)
        {
            // Si no es una cultura válida, puedes optar por ignorarla o añadirla cruda:
            if (seen.Add(folderName))
            {
                list.Add(new CultureOption { LanguageTag = folderName, DisplayName = folderName });
            }
        }
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

