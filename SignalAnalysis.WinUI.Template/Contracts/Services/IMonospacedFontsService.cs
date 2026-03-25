namespace SignalAnalysis.Template.Contracts.Services;

public interface IMonospacedFontsService
{
    IReadOnlyList<double> FontSizes { get; }
    IReadOnlyList<SignalAnalysis.Template.Models.FontItem> MonospacedFonts { get; }
}
