using System.Diagnostics;
using System.Text.Json;
using SignalAnalysis.Contracts.Services;
using SignalAnalysis.Helpers;
using SignalAnalysis.Models;
using Microsoft.Extensions.Options;
using Windows.Storage;

namespace SignalAnalysis.Template.Services;

/// <summary>
/// Class that reads and stores application settings.
/// 
/// Properties that cannot be modified using Reflection:
/// - `readonly` → Immutable after initialization.
/// - `init` → Can only be set in the constructor (C# 9+).
/// - `private set` → The value can only be modified within the class.
/// </summary>
public class LocalSettingsService : ILocalSettingsService<AppSettings>
{
    //private const string _defaultApplicationDataFolder = "OpenXML/ApplicationData";
    //private const string _defaultLocalSettingsFile = "appsettings.json";
    //private readonly IFileService _fileService;
    //private readonly string _localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    //private readonly string _applicationDataFolder;
    //private readonly string _localsettingsFile;

    private readonly AppSettings _options;

    public AppSettings GetValues => _options;

    public event EventHandler<SettingChangedEventArgs>? SettingChanged;

    private Dictionary<string, object> _settings;

    private bool _isInitialized;

    public LocalSettingsService(IOptions<AppSettings> options)
    {
        //_fileService = fileService;
        _options = options.Value;

        //_applicationDataFolder = Path.Combine(_localApplicationData, _options.AppDataFolder ?? _defaultApplicationDataFolder);
        //_localsettingsFile = _options.FileName ?? _defaultLocalSettingsFile;

        _settings = [];
    }

    private async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        var loaded = await ReadSettingFileAsync<Dictionary<string, object>>(_options.AppDataFolder, _options.FileName);
        _settings = loaded ?? [];
        _isInitialized = true;

    }


    public void NotifySettingChanged(string propertyName, object? newValue)
    {
        SettingChanged?.Invoke(this, new SettingChangedEventArgs(propertyName, newValue));
    }

    /// <summary>
    /// Reads a given setting key
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<TValue?> ReadSettingKeyAsync<TValue>(string key)
    {
        if (RuntimeHelper.IsMSIX)
        {
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out var obj))
            {
                return await Json.DeserializeAsync<TValue>((string)obj);
            }
        }
        else
        {
            await InitializeAsync();

            if (_settings != null && _settings.TryGetValue(key, out var obj))
            {
                return await Json.DeserializeAsync<TValue>((string)obj);
            }
        }

        return default;
    }

    /// <summary>
    /// Saves a given setting key
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public async Task SaveSettingKeyAsync<TValue>(string key, TValue value)
    {
        if (RuntimeHelper.IsMSIX)
        {
            ApplicationData.Current.LocalSettings.Values[key] = await Json.SerializeAsync(value);
        }
        else
        {
            await InitializeAsync();

            _settings[key] = await Json.SerializeAsync(value);

            await SaveSettingFileAsync(_options.AppDataFolder, _options.FileName);
        }
    }

    public async Task<TValue?> ReadSettingFileAsync<TValue>() where TValue : class, new()
    {
        var fullPath = Path.Combine(_options.AppDataFolder ?? string.Empty, _options.FileName);
        return await ReadSettingFileAsync<TValue>(fullPath);

    }

    public async Task<TValue?> ReadSettingFileAsync<TValue>(string folderPath, string fileName) where TValue : class, new()
    {
        var fullPath = Path.Combine(folderPath, fileName);
        return await ReadSettingFileAsync<TValue>(fullPath);

    }

    /// <summary>
    /// Updates the existing instance of AppSettings without overwriting it.
    /// This ensures that all ViewModels continue sharing the same instance 
    /// throughout the application.
    ///
    /// Reflection is used to automatically copy all writable properties.
    /// Properties marked as readonly, init, or private set will not be modified.
    /// </summary>
    public async Task<TValue?> ReadSettingFileAsync<TValue>(string fullPath) where TValue : class, new()
    {
        var result = default(TValue);

        if (File.Exists(fullPath))
        {
            try
            {
                var json = await File.ReadAllTextAsync(fullPath);
                result = JsonSerializer.Deserialize<TValue>(json);

                if (result is AppSettings settings)
                {
                    //_options = settings; This changes the reference, and therefore we will get diferent instances of AppSettings via Dependency Injection
                    // Instead, we copy the properties via Reflection without modifying the _options instance itself
                    foreach (var property in typeof(AppSettings).GetProperties())
                    {
                        // We only copy properties that can be modified (i.e., have a setter).
                        // So we exclude read-only properties, private setters, and properties with init-only setters.
                        if (property.CanWrite)  // && property.GetCustomAttribute<JsonIgnoreAttribute>() == null if we also want to ignore properties with JsonIgnore
                        {
                            property.SetValue(_options, property.GetValue(settings));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading settings file: {ex.Message}");

                // This guarantees that a valid instance of TValue is returned
                result = new TValue();

                // If result is AppSettings, then the prefedined values are copied via Reflextion
                if (result is AppSettings settings)
                {
                    var defaultSettings = new AppSettings();

                    foreach (var property in typeof(AppSettings).GetProperties())
                    {
                        if (property.CanWrite && property.SetMethod?.IsPublic == true)
                        {
                            property.SetValue(settings, property.GetValue(defaultSettings));
                        }
                    }
                }
            }
        }

        //_settings = _options.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        //        .ToDictionary(
        //        propertyInfo => propertyInfo.Name,
        //        propertyInfo => propertyInfo.GetValue(_options));

        //var test = _options.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);

        return result;
    }

    public async Task SaveSettingFileAsync(string? jsonString = null)
    {
        var fullPath = Path.Combine(_options.AppDataFolder ?? string.Empty, _options.FileName);
        await SaveSettingFileAsync(fullPath, jsonString);

    }

    public async Task SaveSettingFileAsync(string folderPath, string fileName, string? jsonString = null)
    {
        var fullPath = Path.Combine(folderPath, fileName);
        await SaveSettingFileAsync(fullPath, jsonString);

    }

    public async Task SaveSettingFileAsync(string fullPath, string? jsonString = null)
    {
        try
        {
            var directory = Path.GetDirectoryName(fullPath);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory ?? string.Empty);
            }

            if (jsonString is null)
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                jsonString = JsonSerializer.Serialize(_options, options);
            }

            await File.WriteAllTextAsync(fullPath, jsonString);

            //var fileContent = JsonSerializer.Serialize(content); // JsonConvert.SerializeObject(content);
            //File.WriteAllText(Path.Combine(folderPath, fileName), fileContent, Encoding.UTF8);
        }
        catch
        {
            // Handle the exception as needed, e.g., log it
        }

    }

    public async Task DeleteSettingFileAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
        {
            try
            {
                await Task.Run(() => File.Delete(filePath));
            }
            catch
            {
                // Error handling can be added here if needed
            }

        }
    }


}