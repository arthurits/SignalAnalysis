using CommunityToolkit.Mvvm.ComponentModel;
using SignalAnalysis.Contracts.Services;
using SignalAnalysis.Models;
using System.Collections.ObjectModel;

namespace SignalAnalysis.ViewModels;

public partial class StartUpViewModel: ObservableRecipient
{
    private readonly ILocalizationService _localizationService;

    public ObservableCollection<PlotSeries> PlotSeries = [];

    [ObservableProperty]
    public partial bool PlotSaveEnabled { get; set; } = true;
    [ObservableProperty]
    public partial bool PlotLegendChecked { get; set; } = true;
    [ObservableProperty]
    public partial string PlotPalette { get; set; } = (new ScottPlot.Palettes.Nord()).Name;
    [ObservableProperty]
    public partial bool UpdatePlotSimpleTasksChecked { get; set; } = false;
    [ObservableProperty]
    public partial bool UpdatePlotCompositeTasksChecked { get; set; } = false;

    public StartUpViewModel(ILocalizationService localizationService)
    {
        // Subscribe to localization service events
        _localizationService = localizationService;
        _localizationService.LanguageChanged -= OnLanguageChanged;
        _localizationService.LanguageChanged += OnLanguageChanged;

        // Load string resources into binding variables for the UI
        OnLanguageChanged(null, EventArgs.Empty);
    }

    public void Dispose()
    {
        _localizationService.LanguageChanged -= OnLanguageChanged;
    }
}
