using CommunityToolkit.Mvvm.ComponentModel;
using SignalAnalysis.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SignalAnalysis.ViewModels;

public partial class StartUpViewModel : ObservableRecipient
{
    [ObservableProperty]
    public partial string StrOpenDocument { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrOpenDocumentToolTip { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrDragDrop { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrDragDropToolTip { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrDragDropCaption { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrDragUIOvereride { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrButtonPlotSave { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrButtonPlotSaveToolTip { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrButtonPlotLegend { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrButtonPlotLegendToolTip { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrButtonPlotSeries { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrButtonPlotSeriesToolTip { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrButtonPlotPalette { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrButtonPlotPaletteToolTip { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string StrOriginalPlotTitle { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrOriginalXAxisTitle { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrOriginalYAxisTitle { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string StrDerivativePlotTitle { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrDerivativeXAxisTitle { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrDerivativeYAxisTitle { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrDerivativeYAxisSecondaryTitle { get; set; } = string.Empty;


    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        StrOpenDocument = "StrOpenDocument".GetLocalized("SignalAnalysis");
        StrOpenDocumentToolTip = "StrOpenDocumentToolTip".GetLocalized("SignalAnalysis");
        StrDragDrop = "StrDragDrop".GetLocalized("SignalAnalysis");
        StrDragDropToolTip = "StrDragDropToolTip".GetLocalized("SignalAnalysis");
        StrDragDropCaption = "StrDragDropCaption".GetLocalized("SignalAnalysis");
        StrDragUIOvereride = "StrDragUIOvereride".GetLocalized("SignalAnalysis");
        StrButtonPlotSave = "StrButtonPlotSave".GetLocalized("SignalAnalysis");
        StrButtonPlotSaveToolTip = "StrButtonPlotSaveToolTip".GetLocalized("SignalAnalysis");
        StrButtonPlotLegend = "StrButtonPlotLegend".GetLocalized("SignalAnalysis");
        StrButtonPlotLegendToolTip = "StrButtonPlotLegendToolTip".GetLocalized("SignalAnalysis");
        //StrButtonPlotSeries = "StrButtonPlotSeries".GetLocalized("SignalAnalysis");
        StrButtonPlotSeriesToolTip = "StrButtonPlotSeriesToolTip".GetLocalized("SignalAnalysis");
        //StrButtonPlotPalette = "StrButtonPlotPalette".GetLocalized("SignalAnalysis");
        StrButtonPlotPaletteToolTip = "StrButtonPlotPaletteToolTip".GetLocalized("SignalAnalysis");

        // Plots titles and axis labels
        StrOriginalPlotTitle = "StrOriginalPlotTitle".GetLocalized("SignalAnalysis");
        StrOriginalXAxisTitle = "StrOriginalXAxisTitle".GetLocalized("SignalAnalysis");
        StrOriginalYAxisTitle = "StrOriginalYAxisTitle".GetLocalized("SignalAnalysis");
        StrDerivativePlotTitle = "StrDerivativePlotTitle".GetLocalized("SignalAnalysis");
        StrDerivativeXAxisTitle = "StrDerivativeXAxisTitle".GetLocalized("SignalAnalysis");
        StrDerivativeYAxisTitle = "StrDerivativeYAxisTitle".GetLocalized("SignalAnalysis");
        StrDerivativeYAxisSecondaryTitle = "StrDerivativeYAxisSecondaryTitle".GetLocalized("SignalAnalysis");

        // Derivative algorithms
        DerivativeMethods = [.. "StrDifferentiationAlgorithms".GetLocalized("Numerical").Split(',')];

    }
}
