using SignalAnalysis.Template.Contracts.Services;
using SignalAnalysis.Template.Models;
using Microsoft.UI.Xaml.Media;

namespace SignalAnalysis.Template.Services;

public class MonospacedFontsService : IMonospacedFontsService
{
    public IReadOnlyList<double> FontSizes { get; } = new List<double> { 8, 9, 10, 11, 12, 14, 16, 18, 20, 24, 28, 36, 48, 72 };
    public IReadOnlyList<Models.FontItem> MonospacedFonts { get; }
    public MonospacedFontsService()
    {
        var candidates = new List<FontItem> {
            new ("Arial", new FontFamily("Arial")),
            new ("Comic Sans MS", new FontFamily("Comic Sans MS")),
            new ("Consolas", new FontFamily("Consolas")),
            new ("Courier New", new FontFamily("Courier New")),
            new ("Lucida Console", new FontFamily("Lucida Console")),
            new ("Cascadia Code", new FontFamily("Cascadia Code")),
            new ("Fira Code", new FontFamily("Fira Code")),
            new ("Segoe UI", new FontFamily("Segoe UI")),
            new ("Times New Roman", new FontFamily("Times New Roman"))
        };

        // Optional: filtering logic to ensure the font is actually monospaced could be added here.
        MonospacedFonts = candidates.ToList();
    }
}