using CommunityToolkit.Mvvm.ComponentModel;
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
    public BoxPlotView()
    {
        InitializeComponent();
    }
}
