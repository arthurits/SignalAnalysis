namespace SignalAnalysis.Contracts.Services;

public interface IMonospacedFontsService
{
    IReadOnlyList<double> FontSizes { get; }
    IReadOnlyList<SignalAnalysis.Models.FontItem> MonospacedFonts { get; }
}
