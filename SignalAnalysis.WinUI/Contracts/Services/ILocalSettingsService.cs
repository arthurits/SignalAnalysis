namespace SignalAnalysis.Contracts.Services;

// Interface for managing local settings in the application
public interface ILocalSettingsService<T>
{
    // Get the settings file name
    T GetValues { get; }

    // Event to notify when a setting has changed
    event EventHandler<SettingChangedEventArgs> SettingChanged;

    // Notify that a setting has changed
    void NotifySettingChanged(string propertyName, object? newValue);

    // Read and save settings using a key-value pair approach
    Task<TValue?> ReadSettingKeyAsync<TValue>(string key);
    Task SaveSettingKeyAsync<TValue>(string key, TValue value);

    // Read and save into a file in the local application data folder
    Task<TValue?> ReadSettingFileAsync<TValue>() where TValue : class, new();
    Task<TValue?> ReadSettingFileAsync<TValue>(string fullPath) where TValue : class, new();
    Task<TValue?> ReadSettingFileAsync<TValue>(string folderPath, string fileName) where TValue : class, new();

    // Save the settings file with an optional JSON string
    Task SaveSettingFileAsync(string? jsonString = null);
    Task SaveSettingFileAsync(string fullPath, string? jsonString = null);
    Task SaveSettingFileAsync(string folderPath, string fileName, string? jsonString = null);

    // Delete the settings file
    Task DeleteSettingFileAsync(string filePath);
}

public class SettingChangedEventArgs(string name, object? value) : EventArgs
{
    public string PropertyName { get; } = name;
    public object? NewValue { get; } = value;
}