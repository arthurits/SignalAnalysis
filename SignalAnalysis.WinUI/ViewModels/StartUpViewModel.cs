using CommunityToolkit.Mvvm.ComponentModel;
using SignalAnalysis.Contracts.Services;
using SignalAnalysis.Controls;
using SignalAnalysis.Enumerations;
using SignalAnalysis.Models;
using SignalAnalysis.NumericalAlgorithms;
using System.Collections.ObjectModel;

namespace SignalAnalysis.ViewModels;

public partial class StartUpViewModel: ObservableRecipient
{
    // Services
    private readonly AppSettings _appSettings;
    private readonly ILocalSettingsService<AppSettings> _settingsService;
    private readonly ILocalizationService _localizationService;

    public ObservableCollection<PlotSeries> PlotSeries = [];
    
    [ObservableProperty]
    public partial ObservableCollection<ScatterSeries> OriginalData { get; set; } = [];
    [ObservableProperty]
    public partial BoxPlotData BoxPlotData { get; set; } = new BoxPlotData();
    [ObservableProperty]
    public partial ObservableCollection<ScatterSeries> DerivativeData { get; set; } = [];
    [ObservableProperty]
    public partial ObservableCollection<ScatterSeries> FractalData { get; set; } = [];
    [ObservableProperty]
    public partial ObservableCollection<ScatterSeries> DistributionData { get; set; } = [];
    [ObservableProperty]
    public partial ObservableCollection<ScatterSeries> FourierData { get; set; } = [];
    [ObservableProperty]
    public partial ObservableCollection<ScatterSeries> WindowData { get; set; } = [];
    [ObservableProperty]
    public partial ObservableCollection<ScatterSeries> WindowedData { get; set; } = [];

    [ObservableProperty]
    public partial bool PlotSaveEnabled { get; set; } = true;
    [ObservableProperty]
    public partial bool PlotLegendChecked { get; set; } = true;
    [ObservableProperty]
    public partial string PlotPalette { get; set; } = (new ScottPlot.Palettes.Nord()).Name;
    [ObservableProperty]
    public partial ScottPlot.IPalette SelectedPlotPalette { get; set; } = new ScottPlot.Palettes.Nord();

    [ObservableProperty]
    public partial int SelectedPlotSeriesIndex { get; set; } = -1;

    [ObservableProperty]
    public partial List<string> DerivativeMethods { get; set; } = [];
    [ObservableProperty]
    public partial int SelectedDerivativeIndex { get; set; } = 2;

    [ObservableProperty]
    public partial bool UpdatePlotSimpleTasksChecked { get; set; } = false;
    [ObservableProperty]
    public partial bool UpdatePlotCompositeTasksChecked { get; set; } = false;


    [ObservableProperty]
    public partial bool VisibleResultsSection { get; set; } = true;
    [ObservableProperty]
    public partial string TextResults { get; set; } = string.Empty;
    public IReadOnlyList<double> FontSizes { get; }
    public IReadOnlyList<FontItem> MonospacedFonts { get; }
    [ObservableProperty]
    public partial bool ExportTextEnabled { get; set; } = true;

    [ObservableProperty]
    public partial DocumentBase DocumentDto { get; set; }

    private SignalStats _signalStats = new();

    public StartUpViewModel(
        ILocalSettingsService<AppSettings> settings,
        ILocalizationService localizationService,
        IMonospacedFontsService fontsService)
    {
        // Load settings
        _appSettings = settings.GetValues;
        _settingsService = settings;
        // _settingsService.SettingChanged += OnSettingChanged;
        SelectedDerivativeIndex = _appSettings.DefaultDerivativeMethod;

        // Subscribe to localization service events
        _localizationService = localizationService;
        _localizationService.LanguageChanged -= OnLanguageChanged;
        _localizationService.LanguageChanged += OnLanguageChanged;

        // Get font information
        FontSizes = fontsService.FontSizes;
        MonospacedFonts = fontsService.MonospacedFonts;

        // Load string resources into binding variables for the UI
        OnLanguageChanged(null, EventArgs.Empty);

        // Add plot series
        OriginalData.Clear();
        OriginalData.Add(new ScatterSeries());
        DerivativeData.Clear();
        DerivativeData.Add(new ScatterSeries());    // Derivative signal on the primary Y axis
        DerivativeData.Add(new ScatterSeries        // Original signal on the secondary Y axis
        {
            UseSecondaryYAxis = true
        });

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

        // Set up X values for the original data and derivative data. We need to keep the references to the ObservableCollections for the plot to update correctly.
        var dataAbscissa = new ObservableCollection<double>(Enumerable.Range(0, newValue.SeriesPoints).Select(i => 0 + i / newValue.SamplingFrequency));

        OriginalData[0].Xs = dataAbscissa;
        foreach (var dataSerie in DerivativeData)
            dataSerie.Xs = dataAbscissa;

        // Select the first series by default if there are any
        if (PlotSeries.Count >= 1)
        {
            SelectedPlotSeriesIndex = 0;
        }

        //Derivative_Xs = dataAbscissa;

        //// Si quieres minimizar notificaciones, podrías implementar una colección con suspensión de notificaciones.
        //Derivative_Xs.Clear();
        //foreach (var v in dataAbscissa) Derivative_Xs.Add(v);
    }

    partial void OnSelectedPlotSeriesIndexChanged(int oldValue, int newValue)
    {
        if (newValue >= 0 && newValue < DocumentDto.SeriesNames.Count)
        {
            var selectedSeriesName = DocumentDto.SeriesNames[newValue];
            //var period = 1 / DocumentDto.SamplingFrequency;
            var data = DocumentDto.SeriesData[newValue];

            OriginalData[0].Ys = new ObservableCollection<double>(data);
            DerivativeData[1].Ys = OriginalData[0].Ys;
            _ = OnSelectedDerivativeMethodIndexAsync(-1, SelectedDerivativeIndex);

            (_signalStats.Average, _signalStats.Variance, _signalStats.Maximum, _signalStats.Minimum) = DescriptiveStatistics.ComputeAverage(data.ToArray());

            (_signalStats.BoxplotMin, _signalStats.BoxplotQ1, _signalStats.BoxplotQ2, _signalStats.BoxplotQ3, _signalStats.BoxplotMax) = BoxPlot.ComputeBoxPlotValues(data.ToArray(), false);
            BoxPlotData = new BoxPlotData()
            {
                Position = 0,
                BoxMin = _signalStats.BoxplotMin,
                BoxMiddle = _signalStats.BoxplotQ2,
                BoxMax = _signalStats.BoxplotMax,
                WhiskerMin = _signalStats.BoxplotMin - 1.5 * (_signalStats.BoxplotMax - _signalStats.BoxplotMin),
                WhiskerMax = _signalStats.BoxplotMax + 1.5 * (_signalStats.BoxplotMax - _signalStats.BoxplotMin),
            };
            StrBoxPlotXAxisTitle = selectedSeriesName;

            TextResults = _signalStats.ToString(
                _appSettings.AppCulture,
                boxplot:true);
        }
    }

    partial void OnSelectedDerivativeIndexChanged(int oldValue, int newValue)
    {
        bool derivative = true;

        if (derivative)
        {
            _ = OnSelectedDerivativeMethodIndexAsync(oldValue, newValue);
        }
    }

    private async Task OnSelectedDerivativeMethodIndexAsync(int oldValue, int newValue)
    {
        DerivativeMethod method = DerivativeMethod.CenteredThreePoint; // Default method
        if (Enum.IsDefined(typeof(DerivativeMethod), newValue))
        {
            method = (DerivativeMethod)newValue;
        }

        _signalStats.Derivative = await Task.Run(() => Derivative.Derivate(DocumentDto.SeriesData[SelectedPlotSeriesIndex].ToArray(),
            method: method,
            lowerIndex: 0,
            upperIndex: DocumentDto.SeriesData[SelectedPlotSeriesIndex].Count - 1,
            samplingFrequency: DocumentDto.SamplingFrequency));

        DerivativeData[0].Ys = new ObservableCollection<double>(_signalStats.Derivative);
    }

    partial void OnPlotPaletteChanged(string oldValue, string newValue)
    {
        var selectedPalette = ScottPlot.Palette.GetPalettes().Cast<ScottPlot.IPalette>().Where(x => x.Name == PlotPalette).First();
        if(selectedPalette is not null)
        {
            SelectedPlotPalette = selectedPalette;
        }
    }
}
