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

        toolStripMain_Exit.Image = new System.Drawing.Icon(GraphicsResources.IconExit, 48, 48).ToBitmap();
        toolStripMain_Open.Image = new System.Drawing.Icon(GraphicsResources.IconOpen, 48, 48).ToBitmap();
        toolStripMain_Export.Image = new System.Drawing.Icon(GraphicsResources.IconExport, 48, 48).ToBitmap();
        toolStripMain_Settings.Image = new System.Drawing.Icon(GraphicsResources.IconSettings, 48, 48).ToBitmap();
        toolStripMain_About.Image = new System.Drawing.Icon(GraphicsResources.IconAbout, 48, 48).ToBitmap();

        this.tspTop.Join(this.toolStripMain);

        return;
    }

    /// <summary>
    /// Initialize the status strip and all the controls inside
    /// </summary>
    private void InitializeStatusStrip()
    {
        statusStrip.Renderer = new customRenderer<ToolStripStatusLabelEx>(Brushes.SteelBlue, Brushes.LightSkyBlue);

        tspBottom.Join(statusStrip);

        // Simulate a click on each label in order to...
        statusStripLabelExPower.PerformClick();
        statusStripLabelExCumulative.PerformClick();
        statusStripLabelExEntropy.PerformClick();
        statusStripLabelExCrossHair.PerformClick();
    }

    /// <summary>
    /// Initialize the main menu
    /// </summary>
    private void InitializeMenu()
    {

    }
}