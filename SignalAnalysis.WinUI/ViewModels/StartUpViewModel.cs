using CommunityToolkit.Mvvm.ComponentModel;
using SignalAnalysis.Contracts.Services;
using SignalAnalysis.Helpers;
using SignalAnalysis.Models;
using System.Collections.ObjectModel;

namespace SignalAnalysis.ViewModels;

public partial class StartUpViewModel: ObservableRecipient
{
    private readonly ILocalizationService _localizationService;

    public ObservableCollection<PlotSeries> PlotSeries = [];

    public ObservableCollection<double> Xs { get; } = [];
    public ObservableCollection<double> Ys { get; } = [];

    public ObservableCollection<double> Derivative_Xs { get; } = [];
    public ObservableCollection<double> Derivative_Ys { get; } = [];

    [ObservableProperty]
    public partial bool PlotSaveEnabled { get; set; } = true;
    [ObservableProperty]
    public partial bool PlotLegendChecked { get; set; } = true;
    [ObservableProperty]
    public partial string PlotPalette { get; set; } = (new ScottPlot.Palettes.Nord()).Name;

    [ObservableProperty]
    public partial int SelectedPlotSeriesIndex { get; set; } = -1;

    [ObservableProperty]
    public partial List<string> DerivativeMethods { get; set; } = [];
    [ObservableProperty]
    public partial int SelectedDerivativeIndex { get; set; } = 1;

    [ObservableProperty]
    public partial bool UpdatePlotSimpleTasksChecked { get; set; } = false;
    [ObservableProperty]
    public partial bool UpdatePlotCompositeTasksChecked { get; set; } = false;

    [ObservableProperty]
    public partial DocumentBase DocumentDto { get; set; }

    public StartUpViewModel(ILocalizationService localizationService)
    {
        // Subscribe to localization service events
        _localizationService = localizationService;
        _localizationService.LanguageChanged -= OnLanguageChanged;
        _localizationService.LanguageChanged += OnLanguageChanged;

        // Load string resources into binding variables for the UI
        OnLanguageChanged(null, EventArgs.Empty);

        //// For testing purposes, add some dummy data to the plot series collection
        //for (int i = 0; i < 200; i++)
        //{
        //    Xs.Add(i * 0.1);
        //    Ys.Add(System.Math.Cos(i * 0.1));
        //}
    }

    public void Dispose()
    {
        _localizationService.LanguageChanged -= OnLanguageChanged;
    }

    partial void OnDocumentDtoChanged(DocumentBase oldValue, DocumentBase newValue)
    {
        PlotSeries.Clear();
        if (DocumentDto.SeriesNames.Count > 0)
        {
            // Añadir cada nombre como PlotSeries, conservando el índice
            int index = 0;
            foreach (var name in DocumentDto.SeriesNames)
            {
                PlotSeries.Add(new PlotSeries(name, index));
                index++;
            }
            //// LINQ alternative
            //foreach (var item in docElux.SeriesNames.Select((name, i) => new PlotSeries(name, i)))
            //{
            //    ViewModel.PlotSeries.Add(item);
            //}
        }

        // Select the first series by default if there are any
        if (PlotSeries.Count >= 1)
        {
            SelectedPlotSeriesIndex = 0;
        }
    }

    partial void OnSelectedPlotSeriesIndexChanged(int oldValue, int newValue)
    {
        if (newValue >= 0 && newValue < DocumentDto.SeriesNames.Count)
        {
            var selectedSeriesName = DocumentDto.SeriesNames[newValue];
            
            // Clear existing data points. We need to keep the references to the ObservableCollections for the plot to update correctly.
            Xs.Clear();
            Ys.Clear();
            
            var period = 1 / DocumentDto.SamplingFrequency;
            var data = DocumentDto.SeriesData[newValue];
            for (int i = 0; i < data.Count; i++)
            {
                Xs.Add(i * period); // Ejemplo de eje X
                Ys.Add(data[i]); // Datos de la serie seleccionada
            }
            
        }
    }

    partial void OnSelectedDerivativeIndexChanged(int oldValue, int newValue)
    {
        _ = OnSelectedDerivativeMethodIndexAsync(oldValue, newValue);
    }

    private async Task OnSelectedDerivativeMethodIndexAsync(int oldValue, int newValue)
    {
        await Compute.ComputeAsync(DocumentDto, SelectedPlotSeriesIndex, derivative: true);
    }

}
