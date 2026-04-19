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
    public partial string StrBoxPlotTitle { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrBoxPlotXAxisTitle { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrBoxPlotYAxisTitle { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrDerivativePlotTitle { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrDerivativeXAxisTitle { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrDerivativeYAxisTitle { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrDerivativeYAxisSecondaryTitle { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrFractalPlotTitle { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrFractalXAxisTitle { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrFractalYAxisTitle { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrDistributionPlotTitle { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrDistributionXAxisTitle { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrDistributionYAxisTitle { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrFourierPlotTitle { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrFourierXAxisTitle { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrFourierYAxisTitle { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrWindowPlotTitle { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrWindowXAxisTitle { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrWindowYAxisTitle { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrWindowedPlotTitle { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrWindowedXAxisTitle { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrWindowedYAxisTitle { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrButtonResultsSave { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrButtonResultsSaveToolTip { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrButtonResultsFontSizeToolTip { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrButtonResultsFontFamilyToolTip { get; set; } = string.Empty;

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
        StrBoxPlotTitle = "StrBoxPlotTitle".GetLocalized("SignalAnalysis");
        //StrBoxPlotXAxisTitle = "StrBoxPlotXAxisTitle".GetLocalized("SignalAnalysis");
        StrBoxPlotYAxisTitle = "StrBoxPlotYAxisTitle".GetLocalized("SignalAnalysis");
        StrDerivativePlotTitle = "StrDerivativePlotTitle".GetLocalized("SignalAnalysis");
        StrDerivativeXAxisTitle = "StrDerivativeXAxisTitle".GetLocalized("SignalAnalysis");
        StrDerivativeYAxisTitle = "StrDerivativeYAxisTitle".GetLocalized("SignalAnalysis");
        StrDerivativeYAxisSecondaryTitle = "StrDerivativeYAxisSecondaryTitle".GetLocalized("SignalAnalysis");
        StrFractalPlotTitle = "StrFractalPlotTitle".GetLocalized("SignalAnalysis");
        StrFractalXAxisTitle = "StrFractalXAxisTitle".GetLocalized("SignalAnalysis");
        StrFractalYAxisTitle = "StrFractalYAxisTitle".GetLocalized("SignalAnalysis");
        StrDistributionPlotTitle = "StrDistributionPlotTitle".GetLocalized("SignalAnalysis");
        StrDistributionXAxisTitle = "StrDistributionXAxisTitle".GetLocalized("SignalAnalysis");
        StrDistributionYAxisTitle = "StrDistributionYAxisTitle".GetLocalized("SignalAnalysis");
        StrFourierPlotTitle = "StrFourierPlotTitle".GetLocalized("SignalAnalysis");
        StrFourierXAxisTitle = "StrFourierXAxisTitle".GetLocalized("SignalAnalysis");
        StrFourierYAxisTitle = "StrFourierYAxisTitle".GetLocalized("SignalAnalysis");
        StrWindowPlotTitle = "StrWindowPlotTitle".GetLocalized("SignalAnalysis");
        StrWindowXAxisTitle = "StrWindowXAxisTitle".GetLocalized("SignalAnalysis");
        StrWindowYAxisTitle = "StrWindowYAxisTitle".GetLocalized("SignalAnalysis");
        StrWindowedPlotTitle = "StrWindowedPlotTitle".GetLocalized("SignalAnalysis");
        StrWindowedXAxisTitle = "StrWindowedXAxisTitle".GetLocalized("SignalAnalysis");
        StrWindowedYAxisTitle = "StrWindowedYAxisTitle".GetLocalized("SignalAnalysis");

        // Results section
        StrButtonResultsSave = "StrButtonResultsSave".GetLocalized("SignalAnalysis");
        StrButtonResultsSaveToolTip = "StrButtonResultsSaveToolTip".GetLocalized("SignalAnalysis");
        StrButtonResultsFontSizeToolTip = "StrButtonResultsFontSizeToolTip".GetLocalized("SignalAnalysis");
        StrButtonResultsFontFamilyToolTip = "StrButtonResultsFontFamilyToolTip".GetLocalized("SignalAnalysis");

        // Derivative algorithms
        DerivativeMethods = [.. "StrDifferentiationAlgorithms".GetLocalized("Numerical").Split(',')];

    }
}
