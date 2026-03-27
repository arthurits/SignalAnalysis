using CommunityToolkit.Mvvm.ComponentModel;

namespace SignalAnalysis.Models;

public partial class PlotSeries(string name, int value) : ObservableObject
{
    [ObservableProperty]
    public partial string Name { get; set; } = name;
    public int Value { get; set; } = value;

    public override string ToString()
    {
        return $"{Name} ({Value})";
    }
}