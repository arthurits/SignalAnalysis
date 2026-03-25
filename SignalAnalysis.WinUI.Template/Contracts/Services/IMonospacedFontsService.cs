namespace SignalAnalysis.Template.Contracts.Services;

public interface IMonospacedFontsService
{
    IReadOnlyList<double> FontSizes { get; }
    IReadOnlyList<SignalAnalysis.Models.FontItem> MonospacedFonts { get; }
}
