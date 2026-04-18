using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ScottPlot;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SignalAnalysis.Controls;

public partial class BoxPlotData : ObservableObject
{
    [ObservableProperty]
    public partial double Position { get; set; }

    [ObservableProperty]
    public partial double BoxMin { get; set; }

    [ObservableProperty]
    public partial double BoxMax { get; set; }

    [ObservableProperty]
    public partial double WhiskerMin { get; set; }

    [ObservableProperty]
    public partial double WhiskerMax { get; set; }

    [ObservableProperty]
    public partial double BoxMiddle { get; set; }
}

public sealed partial class BoxPlotView : UserControl
{
    #region BoxPlotData DependencyProperty
    public static readonly DependencyProperty BoxProperty =
        DependencyProperty.Register(
            nameof(Box),
            typeof(BoxPlotData),
            typeof(BoxPlotView),
            new PropertyMetadata(null, OnBoxChanged));

    public BoxPlotData? Box
    {
        get => (BoxPlotData?)GetValue(BoxProperty);
        set => SetValue(BoxProperty, value);
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

    // Public method for the parent view to add the visual host and bind the Plot created here to the specific ScottPlot WinUI host.
    public Plot GetPlot() => _plot;

    // Internal property to hold the reference to the ScottPlot host control, which is used to call Refresh() when needed.
    // This should be set by the parent view after adding the host control to the visual tree.
    private readonly ScottPlot.WinUI.WinUIPlot _plotHost;
    private readonly Plot _plot;

    public BoxPlotView()
    {
        InitializeComponent();

        // Create the ScottPlot Plot and Scatter plottable, and set up the host control for WinUI.
        _plotHost = new();
        _plot = _plotHost.Plot;

        // Insert the ScottPlot host control into the visual tree of this UserControl.
        // In this case, we assume there is a Grid named "RootGrid" in the XAML where we want to place the plot.
        RootGrid.Children.Add(_plotHost);
    }

    #region BoxPlotData handling
    private static void OnBoxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctrl = (BoxPlotView)d;
        ctrl.ApplyBoxPlotData();
    }

    private void ApplyBoxPlotData()
    {
        if (Box is null)
            return;
        
        _plot.Clear();
        _plot.Add.Box( new()
        {
            Position = Box.Position,
            BoxMin = Box.BoxMin,
            BoxMax = Box.BoxMax,
            WhiskerMin = Box.WhiskerMin,
            WhiskerMax = Box.WhiskerMax,
            BoxMiddle = Box.BoxMiddle
        });
        _plot.Axes.AutoScale();
        _plotHost.Refresh();
    }
    #endregion

    #region Titles handling
    private static void OnTitlesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctrl = (BoxPlotView)d;
        ctrl.ApplyTitles();
    }

    private void ApplyTitles()
    {
        _plot.Axes.Title.Label.Text = PlotTitle ?? string.Empty;
        _plot.Axes.Bottom.Label.Text = XAxisTitle ?? string.Empty;
        _plot.Axes.Left.Label.Text = YAxisTitle ?? string.Empty;

        _plotHost.Refresh();
    }

    #endregion
}
