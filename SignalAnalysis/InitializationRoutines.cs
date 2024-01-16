namespace SignalAnalysis;

partial class FrmMain
{
    /// <summary>
    /// Initialize the tool strip panel and all the controls inside
    /// </summary>
    /// <seealso>https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.toolstrippanel?view=windowsdesktop-6.0
    // https://stackoverflow.com/questions/40382105/how-to-add-two-toolstripcombobox-and-separator-horizontally-to-one-toolstripdrop</seealso>
    private void InitializeToolStripPanel()
    {
        toolStripMain.Renderer = new customRenderer<ToolStripButton>(System.Drawing.Brushes.SteelBlue, System.Drawing.Brushes.LightSkyBlue);

        toolStripMain_Exit.Image = GraphicsResources.LoadIcon(GraphicsResources.IconExit, 48);
        toolStripMain_Open.Image = GraphicsResources.LoadIcon(GraphicsResources.IconOpen, 48);
        toolStripMain_Export.Image = GraphicsResources.LoadIcon(GraphicsResources.IconExport, 48);
        toolStripMain_Settings.Image = GraphicsResources.LoadIcon(GraphicsResources.IconSettings, 48);
        toolStripMain_About.Image = GraphicsResources.LoadIcon(GraphicsResources.IconAbout, 48);

        this.tspTop.Join(this.toolStripMain);

        // Events for ToolStripComboBox
        stripComboSeries.ComboBox.SelectionChangeCommitted += ComboSeries_SelectionChangeCommitted;
        stripComboWindows.ComboBox.SelectionChangeCommitted += ComboWindow_SelectionChangeCommitted;

        return;
    }

    /// <summary>
    /// Initialize the status strip and all the controls inside
    /// </summary>
    private void InitializeStatusStrip()
    {
        statusStrip.Renderer = new customRenderer<ToolStripStatusLabelEx>(Brushes.SteelBlue, Brushes.LightSkyBlue);

        tspBottom.Join(statusStrip);

        ShowHideBoxplot(_settings.Boxplot);
        statusStripLabelExBoxplot.Checked = _settings.Boxplot;
        statusStripLabelExDerivative.Checked = _settings.ComputeDerivative;
        statusStripLabelExIntegration.Checked = _settings.ComputeIntegration;
        statusStripLabelExCumulative.Checked = _settings.CumulativeDimension;
        statusStripLabelExPower.Checked = _settings.PowerSpectra;
        statusStripLabelExEntropy.Checked = _settings.ComputeEntropy;
        statusStripLabelExCrossHair.Checked = _settings.CrossHair;
    }

    ///// <summary>
    ///// Initialize the main menu
    ///// </summary>
    //private void InitializeMenu()
    //{

    //}
}