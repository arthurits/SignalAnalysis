namespace ScottPlot;

public class FormsPlotCrossHair : ScottPlot.FormsPlot
{
    private readonly ContextMenuStrip customMenu = new ();
    private System.Windows.Forms.ToolStripMenuItem detachLegendMenuItem = new();
    private System.Windows.Forms.ToolStripMenuItem crossHairMenuItem = new();

    /// <summary>
    /// Event fired whenever the vertical line is dragged.
    /// </summary>
    public event EventHandler<LineDragEventArgs>? VLineDragged;
    /// <summary>
    /// Event fired whenever the horizontal line is dragged.
    /// </summary>
    public event EventHandler<LineDragEventArgs>? HLineDragged;
    
    public ScottPlot.Plottable.VLine? VerticalLine { get; private set; }
    public ScottPlot.Plottable.HLine? HorizontalLine { get; private set; }

    private System.Globalization.CultureInfo _cultureUI = System.Globalization.CultureInfo.CurrentCulture;
    /// <summary>
    /// Culture used to show the right click context menu
    /// </summary>
    public System.Globalization.CultureInfo CultureUI
    {
        get { return _cultureUI; }
        set { _cultureUI = value; ContextMenuUILanguage(); }
    }

    private readonly System.Resources.ResourceManager StringsRM;

    public bool ShowCrossHair
    {
        get => crossHairMenuItem.Checked;
        set
        {
            if (value)
            {
                ShowCrossHairLines(true, true);
                crossHairMenuItem.Checked = value;
            }
            else
            {
                DeleteCrossHairLines();
                crossHairMenuItem.Checked = value;
            }
        }
    }
    public bool ShowCrossHairVertical { get; set; } = false;
    public bool ShowCrossHairHorizontal { get; set; } = false;

    public bool SnapToPoint { get; set; } = false;

    /// <summary>
    /// Sets color for horizontal and vertical lines and their position label backgrounds
    /// </summary>
    public System.Drawing.Color CrossHairColor
    {
        set
        {
            if (VerticalLine is not null)
            {
                VerticalLine.Color = value;
                VerticalLine.PositionLabelBackground = System.Drawing.Color.FromArgb(200, value);
            }
            if (HorizontalLine is not null)
            {
                HorizontalLine.Color = value;
                HorizontalLine.PositionLabelBackground = System.Drawing.Color.FromArgb(200, value);
            }
        }
        get => VerticalLine?.Color ?? System.Drawing.Color.Red;
    }

    public FormsPlotCrossHair()
        :base()
    {
        InitilizeContextMenu();
        // Unsubscribe from the default right-click menu event
        this.RightClicked -= DefaultRightClickEvent;
        // Add a custom right-click action
        this.RightClicked += CustomRightClickEvent;
        this.DoubleClick += OnDoubleClick;

        StringsRM = new("ScottPlot.FormsPlotCrossHair", typeof(FormsPlotCrossHair).Assembly);
    }

    public FormsPlotCrossHair(System.Globalization.CultureInfo? culture = null)
        : this()
    {
        CultureUI = culture ?? System.Globalization.CultureInfo.CurrentCulture;
        //this.Refresh();
    }

    private void ContextMenuUILanguage()
    {
        customMenu.Items["Copy"].Text = StringsRM.GetString("strMenuCopy", CultureUI) ?? "Copy image";
        customMenu.Items["Save"].Text = StringsRM.GetString("strMenuSave", CultureUI) ?? "Save image as...";
        customMenu.Items["Zoom"].Text = StringsRM.GetString("strMenuZoom", CultureUI) ?? "Zoom to fit data";
        customMenu.Items["Help"].Text = StringsRM.GetString("strMenuHelp", CultureUI) ?? "Help";
        customMenu.Items["Open"].Text = StringsRM.GetString("strMenuOpen", CultureUI) ?? "Open in new window";
        detachLegendMenuItem.Text = StringsRM.GetString("strMenuDetach", CultureUI) ?? "Detach legend";
        crossHairMenuItem.Text = StringsRM.GetString("strMenuCrossHair", CultureUI) ?? "Show crosshair";
    }

    private void InitilizeContextMenu()
    {
        int item;
        System.Windows.Forms.ToolStripMenuItem menuItem;

        item = customMenu.Items.Add(new ToolStripMenuItem("Copy image", null, new EventHandler(RightClickMenu_Copy_Click)));
        menuItem = (ToolStripMenuItem)customMenu.Items[item];
        menuItem.Name = "Copy";
       
        item = customMenu.Items.Add(new ToolStripMenuItem("Save image as...", null, new EventHandler(RightClickMenu_Help_Click)));
        menuItem = (ToolStripMenuItem)customMenu.Items[item];
        menuItem.Name = "Save";
        
        item = customMenu.Items.Add(new ToolStripSeparator());
        
        item = customMenu.Items.Add(new ToolStripMenuItem("Zoom to fit data", null, new EventHandler(RightClickMenu_AutoAxis_Click)));
        menuItem = (ToolStripMenuItem)customMenu.Items[item];
        menuItem.Name = "Zoom";

        item = customMenu.Items.Add(new ToolStripSeparator());
        
        item = customMenu.Items.Add(new ToolStripMenuItem("Help", null, new EventHandler(RightClickMenu_Help_Click)));
        menuItem = (ToolStripMenuItem)customMenu.Items[item];
        menuItem.Name = "Help";

        item = customMenu.Items.Add(new ToolStripSeparator());

        item = customMenu.Items.Add(new ToolStripMenuItem("Open in new window", null, new EventHandler(RightClickMenu_OpenInNewWindow_Click)));
        menuItem = (ToolStripMenuItem)customMenu.Items[item];
        menuItem.Name = "Open";

        item = customMenu.Items.Add(new ToolStripMenuItem("Detach legend", null, new EventHandler(RightClickMenu_DetachLegend_Click)));
        detachLegendMenuItem = (ToolStripMenuItem)customMenu.Items[item];
        detachLegendMenuItem.Name = "Detach";

        item = customMenu.Items.Add(new ToolStripSeparator());
        
        item = customMenu.Items.Add(new ToolStripMenuItem("Show crosshair", null, new EventHandler(RightClickMenu_CrossHair_Click)));
        crossHairMenuItem = (ToolStripMenuItem)customMenu.Items[item];
        crossHairMenuItem.Name = "CrossHair";
    }

    /// <summary>
    /// Add vertical and horizontal plottable lines.
    /// Subscribe to the line's dragged events.
    /// </summary>
    private void CreateCrossHairLines()
    {
        VerticalLine = this.Plot.AddVerticalLine(0.0, style: ScottPlot.LineStyle.Dash);
        VerticalLine.IsVisible = false;
        VerticalLine.PositionLabel = true;
        VerticalLine.DragEnabled = true;
        VerticalLine.Dragged += new System.EventHandler(OnDraggedVertical);

        HorizontalLine = this.Plot.AddHorizontalLine(0.0, color: System.Drawing.Color.Red, width: 1, style: ScottPlot.LineStyle.Dash);
        HorizontalLine.IsVisible = false;
        HorizontalLine.PositionLabel = true;
        HorizontalLine.DragEnabled = true;
        HorizontalLine.Dragged += new System.EventHandler(OnDraggedHorizontal);

        CrossHairColor = System.Drawing.Color.FromArgb(200, System.Drawing.Color.Red);
    }

    private void ShowCrossHairLines(bool showVertical = false, bool showHorizontal = false)
    {
        if (!showVertical && !showHorizontal) return;

        if (Plot.GetPlottables().Where(x => x is Plottable.VLine || x is Plottable.HLine).Any()) return;

        // There should be at last one plottable added, otherwise
        if (this.Plot.GetPlottables().Length >= 1)
            CreateCrossHairLines();

        if (showVertical && VerticalLine is not null)
        {
            VerticalLine.IsVisible = true;
            var axis = this.Plot.GetPlottables()[0].GetAxisLimits();
            VerticalLine.X = axis.XCenter;
            //SnapLinesToPoint(ToX: true);
        }

        if (showHorizontal && HorizontalLine is not null)
        {
            HorizontalLine.IsVisible = true;
            var axis = this.Plot.GetPlottables()[0].GetAxisLimits();
            HorizontalLine.Y = axis.YCenter;
            //SnapLinesToPoint(ToY: true);
        }
    }

    /// <summary>
    /// Delete vertical and horizontal plottable lines.
    /// Unsubscribe to the line's dragged events.
    /// </summary>
    private void DeleteCrossHairLines()
    {
        if (VerticalLine is not null)
        {
            VerticalLine.Dragged -= new System.EventHandler(OnDraggedVertical);
            this.Plot.Clear(typeof(Plottable.VLine));
        }

        if (HorizontalLine is not null)
        {
            HorizontalLine.Dragged -= new System.EventHandler(OnDraggedHorizontal);
            this.Plot.Clear(typeof(Plottable.HLine));
        }
    }

    /// <summary>
    /// Move the vertical and horizontal lines to the nearest point.
    /// </summary>
    /// <param name="ToX"><see langword="True"/> if the lines are moved according to the nearest X point.</param>
    /// <param name="ToY"><see langword="True"/> if the lines are moved according to the nearest Y point.</param>
    /// <returns>The closest X/Y coordinate as well as the array index of the closest point.</returns>
    private (double? pointX, double? pointY, int? pointIndex) SnapLinesToPoint(bool ToX = false, bool ToY = false)
    {
        double? pointX = null;
        double? pointY = null;
        int? pointIndex = null;

        var plot = this.Plot.GetPlottables().First();
        var plotType = (this.Plot.GetPlottables().First()).GetType();
        System.Reflection.MethodInfo? plotMethod = null;
        if (ToX)
            plotMethod = plotType.GetMethod("GetPointNearestX");
        else if(ToY)
            plotMethod = plotType.GetMethod("GetPointNearestY");

        if (plotMethod is null || VerticalLine is null || HorizontalLine is null) return (null, null, null);

        if (VerticalLine.IsVisible || HorizontalLine.IsVisible)
        {
            (double mouseCoordX, double mouseCoordY) = this.GetMouseCoordinates();
            var param = new object[1];
            if (ToX)
                param[0] = mouseCoordX;
            else if (ToY) 
                param[0] = mouseCoordY;

            var result = plotMethod.Invoke(plot, param);
            if (result is not null)
            {
                (pointX, pointY, pointIndex) = ((double, double, int))result;
                VerticalLine.X = pointX.Value;
                HorizontalLine.Y = pointY.Value;
            }
        }
        return (pointX, pointY, pointIndex);
    }

    private void OnDoubleClick(object? sender, EventArgs e)
    {
        ShowCrossHair = !ShowCrossHair;
    }

    private void OnDraggedVertical(object? sender, EventArgs e)
    {
        // If we are reading from the sensor, then exit
        if (VerticalLine is null || !VerticalLine.IsVisible || !SnapToPoint) return;

        var (pointX, pointY, pointIndex) = SnapLinesToPoint(ToX: true);

        // Raise the custom event for the subscribers
        OnVLineDragged(new LineDragEventArgs(pointX, pointY, pointIndex));
        //EventHandler<VLineDragEventArgs> handler = VLineDragged;
        //handler?.Invoke(this, new VLineDragEventArgs(pointX, pointY, pointIndex));

    }

    private void OnDraggedHorizontal(object? sender, EventArgs e)
    {
        // If we are reading from the sensor, then exit
        if (HorizontalLine is null || !HorizontalLine.IsVisible || !SnapToPoint) return;

        var (pointX, pointY, pointIndex) = SnapLinesToPoint(ToY: true);

        // Raise the custom event for the subscribers
        OnHLineDragged(new LineDragEventArgs(pointX, pointY, pointIndex));
        //EventHandler<VLineDragEventArgs> handler = VLineDragged;
        //handler?.Invoke(this, new VLineDragEventArgs(pointX, pointY, pointIndex));

    }

    /// <summary>
    /// Gets the plottables that represent data. Therefore, no VLine nor other auxiliar plottables are returned
    /// </summary>
    /// <returns></returns>
    public Plottable.IPlottable[] GetDataCurves()
    {
        //System.Collections.ObjectModel.ObservableCollection<ScottPlot.Plottable.IPlottable> plots = new();
        var dataPlots = this.Plot.GetPlottables().Where(x => x is not Plottable.VLine && x is not Plottable.HLine);

        //foreach (var plot in this.Plot.GetPlottables())
        //{
        //    if (plot.GetType() != typeof(Plottable.VLine))
        //    {
        //        plots.Add(plot);
        //    }
        //}

        return dataPlots.ToArray();
    }

    // Wrap event invocations inside a protected virtual method to allow derived classes to override the event invocation behavior
    // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/events/how-to-publish-events-that-conform-to-net-framework-guidelines
    protected virtual void OnVLineDragged(LineDragEventArgs e)
    {
        // Make a temporary copy of the event to avoid possibility of
        // a race condition if the last subscriber unsubscribes
        // immediately after the null check and before the event is raised.
        EventHandler<LineDragEventArgs>? raiseEvent = VLineDragged;

        // Event will be null if there are no subscribers
        if (raiseEvent is not null)
        {
            // Call to raise the event.
            raiseEvent(this, e);
        }
    }

    protected virtual void OnHLineDragged(LineDragEventArgs e)
    {
        // Make a temporary copy of the event to avoid possibility of
        // a race condition if the last subscriber unsubscribes
        // immediately after the null check and before the event is raised.
        EventHandler<LineDragEventArgs>? raiseEvent = HLineDragged;

        // Event will be null if there are no subscribers
        if (raiseEvent is not null)
        {
            // Call to raise the event.
            raiseEvent(this, e);
        }
    }

    /// <summary>
    /// Override Clear method.
    /// </summary>
    public void Clear()
    {
        Plottable.IPlottable[] plottables = this.Plot.GetPlottables();
        for (int i = plottables.Length - 1; i >= 0; i--)
        {
            //if (plottables[i] is not Plottable.VLine && plottables[i] is not Plottable.HLine)
                this.Plot.RemoveAt(i);
        }
    }


    /// <summary>
    /// Launch the default right-click menu.
    /// </summary>
    private void CustomRightClickEvent(object? sender, EventArgs e)
    {
        detachLegendMenuItem.Visible = Plot.Legend(null).Count > 0;
        crossHairMenuItem.Enabled = Plot.GetPlottables().Length > 0;
        customMenu.Show(System.Windows.Forms.Cursor.Position);
    }
    private void RightClickMenu_Copy_Click(object? sender, EventArgs e) => Clipboard.SetImage(Plot.Render());
    private void RightClickMenu_Help_Click(object? sender, EventArgs e) => new FormHelp().Show();
    private void RightClickMenu_AutoAxis_Click(object? sender, EventArgs e) { Plot.AxisAuto(); Refresh(); }
    private void RightClickMenu_OpenInNewWindow_Click(object? sender, EventArgs e) => new FormsPlotViewer(Plot).Show();
    private void RightClickMenu_DetachLegend_Click(object? sender, EventArgs e) => new FormsPlotLegendViewer(this);
    private void RightClickMenu_SaveImage_Click(object? sender, EventArgs e)
    {
        var sfd = new SaveFileDialog
        {
            FileName = "ScottPlot.png",
            Filter = "PNG Files (*.png)|*.png;*.png" +
                     "|JPG Files (*.jpg, *.jpeg)|*.jpg;*.jpeg" +
                     "|BMP Files (*.bmp)|*.bmp;*.bmp" +
                     "|All files (*.*)|*.*"
        };

        if (sfd.ShowDialog() == DialogResult.OK)
            Plot.SaveFig(sfd.FileName);
    }
    private void RightClickMenu_CrossHair_Click(object? sender, EventArgs e)
    {
        if (sender is not null)
        {
            ShowCrossHair = !ShowCrossHair;
            Refresh();
        }
    }

}

public class LineDragEventArgs : EventArgs
{
    public LineDragEventArgs(double? X, double? Y, int? Index)
    {
        PointX = X ?? default;
        PointY = Y ?? default;
        PointIndex = Index ?? default;
    }

    public double PointX { get; set; }
    public double PointY { get; set; }
    public int PointIndex { get; set; }

}


