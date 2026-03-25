namespace SignalAnalysis.Contracts.Services;
public interface IIconLoaderFactory
{
    /// <summary>
    /// Returns the appropriate icon loader based on the specified type.
    /// </summary>
    IIconLoader GetLoader(IconLoaderType type);
}
