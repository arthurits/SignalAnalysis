using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ScottPlot;
using ScottPlot.Plottables;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SignalAnalysis.Controls;


public partial class ScatterSeries : ObservableObject
{
    [ObservableProperty]
    public partial ObservableCollection<double>? Xs { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<double>? Ys { get; set; }

    // OPCIONAL: para uso futuro
    [ObservableProperty]
    public partial ObservableCollection<(double X, double Y)>? Points { get; set; }
}


internal sealed class ScatterHandle
{
    public List<double> Xs { get; } = new();
    public List<double> Ys { get; } = new();
    public Scatter Scatter { get; }

    public ScatterHandle(Scatter scatter)
    {
        Scatter = scatter;
    }
}



public sealed partial class ScatterPlotView : UserControl
{
    #region Series DependencyProperty
    public static readonly DependencyProperty SeriesProperty =
        DependencyProperty.Register(
            nameof(Series),
            typeof(ObservableCollection<ScatterSeries>),
            typeof(ScatterPlotView),
            new PropertyMetadata(null, OnSeriesChanged));

    public ObservableCollection<ScatterSeries>? Series
    {
        get => (ObservableCollection<ScatterSeries>?)GetValue(SeriesProperty);
        set => SetValue(SeriesProperty, value);
    }
    #endregion


    #region Titles DependencyProperties

    public static readonly DependencyProperty PlotTitleProperty =
        DependencyProperty.Register(
            nameof(PlotTitle),
            typeof(string),
            typeof(ScatterPlotView),
            new PropertyMetadata(string.Empty, OnTitlesChanged));

    public static readonly DependencyProperty XAxisTitleProperty =
        DependencyProperty.Register(
            nameof(XAxisTitle),
            typeof(string),
            typeof(ScatterPlotView),
            new PropertyMetadata(string.Empty, OnTitlesChanged));

    public static readonly DependencyProperty YAxisTitleProperty =
        DependencyProperty.Register(
            nameof(YAxisTitle),
            typeof(string),
            typeof(ScatterPlotView),
            new PropertyMetadata(string.Empty, OnTitlesChanged));

    public string PlotTitle
    {
        get => (string)GetValue(PlotTitleProperty);
        set => SetValue(PlotTitleProperty, value);
    }

    public string XAxisTitle
    {
        get => (string)GetValue(XAxisTitleProperty);
        set => SetValue(XAxisTitleProperty, value);
    }

    public string YAxisTitle
    {
        get => (string)GetValue(YAxisTitleProperty);
        set => SetValue(YAxisTitleProperty, value);
    }
    #endregion


    // Método público para que la vista que contiene este control pueda añadir el host visual
    // y enlazar el Plot creado aquí al host concreto de ScottPlot WinUI.
    public Plot GetPlot() => _plot;

    // Internal property to hold the reference to the ScottPlot host control, which is used to call Refresh() when needed.
    // This should be set by the parent view after adding the host control to the visual tree.
    //private readonly Lock _sync = new();
    //private readonly DebounceDispatcher _debouncer;
    private readonly ScottPlot.WinUI.WinUIPlot _plotHost;
    private readonly Plot _plot;
    //private readonly Scatter _scatter;

    // Relación 1:1 Serie ↔ Scatter
    private readonly Dictionary<ScatterSeries, ScottPlot.Plottables.Scatter> _scatters = [];    // This is valid for ScottPlot version 5.0.0 and later, where Scatter is a plottable type that can be updated with new data.
    private readonly Dictionary<ScatterSeries, ScatterHandle> _seriesMap = [];


    //// Internal Lists to hold the actual data points for the plot.
    //// These are updated based on changes to the Xs and Ys collections, and are what the SignalXY plottable uses as its data source.
    //// We maintain these internal lists to ensure that we only add complete pairs of X and Y values to the plot,
    //// and to handle cases where X and Y values may arrive in an interleaved manner.
    //private readonly List<double> _xsList = [];
    //private readonly List<double> _ysList = [];

    //// Internal buffers to hold pending X or Y values when they arrive without their corresponding pair,
    //// to ensure that we only add complete pairs to the plot. This handles the case where X and Y are added in an interleaved manner.
    //private double? _pendingX = null;
    //private double? _pendingY = null;

    public ScatterPlotView()
    {
        InitializeComponent();

        //_debouncer = new DebounceDispatcher(DispatcherQueue.GetForCurrentThread(), TimeSpan.FromMilliseconds(80));

        // Create the ScottPlot Plot and Scatter plottable, and set up the host control for WinUI.
        _plotHost = new();
        _plot = _plotHost.Plot;
        //_scatter = _plot.Add.Scatter(_xsList, _ysList);

        // Insert the ScottPlot host control into the visual tree of this UserControl.
        // In this case, we assume there is a Grid named "RootGrid" in the XAML where we want to place the plot.
        RootGrid.Children.Add(_plotHost);
        //WinUIPlotHost = _winUIPlot;
    }


    #region Series handling

    private static void OnSeriesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctrl = (ScatterPlotView)d;

        if (e.OldValue is ObservableCollection<ScatterSeries> oldSeries)
            oldSeries.CollectionChanged -= ctrl.Series_CollectionChanged;

        if (e.NewValue is ObservableCollection<ScatterSeries> newSeries)
            newSeries.CollectionChanged += ctrl.Series_CollectionChanged;

        ctrl.RebuildAllSeries();
    }

    private void Series_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RebuildAllSeries();
    }

    #endregion

    #region Rebuild / Update logic

    private void RebuildAllSeries()
    {
        _plot.Clear();
        _scatters.Clear();

        if (Series is null)
            return;

        foreach (var serie in Series)
        {
            serie.PropertyChanged += OnSeriePropertyChanged;
            AddOrUpdateSeries(serie);
        }

        _plot.Axes.AutoScale();
        _plotHost.Refresh();
    }

    private void OnSeriePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not ScatterSeries serie)
            return;

        AddOrUpdateSeries(serie);
        _plot.Axes.AutoScale();
        _plotHost.Refresh();
    }

    private void AddOrUpdateSeries(ScatterSeries serie)
    {
        if (!TryGetData(serie, out var xs, out var ys))
            return;

        if (!_seriesMap.TryGetValue(serie, out var handle))
        {
            // Create data lists for the new series so that they can be updated later without needing to replace the entire plottable.
            var xsList = new List<double>(xs);
            var ysList = new List<double>(ys);
            var scatter = _plot.Add.Scatter(xsList, ysList);

            // Store the Scatter plottable and its associated data lists in the handle, so that we can update them later when the series changes.
            handle = new ScatterHandle(scatter);
            handle.Xs.AddRange(xsList);
            handle.Ys.AddRange(ysList);
            _seriesMap[serie] = handle;
        }
        else
        {
            // Modify the existing data lists for the Scatter plottable associated with this series.
            // This is the only way to update in ScottPlot version 4.x. In version 5.0.0 and later, we can directly use the .Update() method.
            handle.Xs.Clear();
            handle.Ys.Clear();

            handle.Xs.AddRange(xs);
            handle.Ys.AddRange(ys);
        }
    }

    private static bool TryGetData(ScatterSeries serie, out double[] xs, out double[] ys)
    {
        xs = [];
        ys = [];

        // PRIORIDAD 1: Points
        if (serie.Points is not null && serie.Points.Count > 0)
        {
            xs = serie.Points.Select(p => p.X).ToArray();
            ys = serie.Points.Select(p => p.Y).ToArray();
            return true;
        }

        // PRIORIDAD 2: Xs + Ys
        if (serie.Xs is null || serie.Ys is null)
            return false;

        int count = Math.Min(serie.Xs.Count, serie.Ys.Count);
        if (count == 0)
            return false;

        xs = serie.Xs.Take(count).ToArray();
        ys = serie.Ys.Take(count).ToArray();
        return true;
    }

    #endregion

    #region Titles handling
    private static void OnTitlesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctrl = (ScatterPlotView)d;
        ctrl.ApplyTitles();
    }

    private void ApplyTitles()
    {
        _plot.Title(PlotTitle ?? string.Empty);
        _plot.XLabel(XAxisTitle ?? string.Empty);
        _plot.YLabel(YAxisTitle ?? string.Empty);

        _plotHost.Refresh();
    }

    #endregion
}
