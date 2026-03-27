using Microsoft.UI.Xaml.Controls;
using SignalAnalysis.Contracts.Services;
using SignalAnalysis.ViewModels;

namespace SignalAnalysis.Views;

/// <summary>
/// StartUpPage to be used as a template.
/// </summary>
public sealed partial class StartUpPage : Page, IDisposable
{
    public StartUpViewModel ViewModel { get; }
    private readonly ILocalizationService _localizationService;

    private List<string> PlotPalettes { get; set; } = [];

    public StartUpPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<StartUpViewModel>();
        DataContext = ViewModel;
        _localizationService = App.GetService<ILocalizationService>();

        // Initialize the plot palettes
        ScottPlot.Palette.GetPalettes().ToList().ForEach(x => PlotPalettes.Add(x.Name));
    }

    public void Dispose()
    {
    }
}
