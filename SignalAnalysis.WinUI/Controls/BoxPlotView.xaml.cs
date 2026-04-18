using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;

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

    public BoxPlotView()
    {
        InitializeComponent();
    }
}
