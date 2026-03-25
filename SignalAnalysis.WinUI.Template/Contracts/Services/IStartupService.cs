namespace SignalAnalysis.Template.Contracts.Services;
public interface IStartupService
{
    /// <summary>
    /// Set the registry key to enable or disable the application launching at system startup.
    /// </summary>
    /// <param name="enabled"><see langword="True"/> if the app should be launched, <see langword="false"/> otherwise</param>
    void SetStartupEnabled(bool enabled);

    /// <summary>
    /// Check if the application is set, on the registry, to launch at system startup.
    /// </summary>
    /// <returns><see langword="True"/> if the registry key exists, <see langword="false"/> otherwise</returns>
    bool IsStartupEnabled();

    /// <summary>
    /// Gets a value indicating whether the process was launched with the --startup argument.
    /// </summary>
    bool IsAutoStart { get; }
}
