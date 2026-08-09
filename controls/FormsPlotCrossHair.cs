namespace ScottPlot;

public class FormsPlotCrossHair : ScottPlot.FormsPlotCulture
{
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

    public bool ShowCrossHair
    {
        get => crossHairMenuItem.Checked;
        set
        {
            if (value)
            {
                ShowLines(true, true);
                crossHairMenuItem.Checked = value;
            }
            else
            {
                DeleteLines();
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
        this.DoubleClick += OnDoubleClick;
    }

    public FormsPlotCrossHair(System.Globalization.CultureInfo? culture = null)
        : this()
    {
        CultureUI = culture ?? System.Globalization.CultureInfo.CurrentCulture;
        //this.Refresh();
    }

    private void SetLabelCulture()
    {
        if (VerticalLine is not null)
            VerticalLine.PositionFormatter = position => position.ToString("F2", CultureUI);

        if (HorizontalLine is not null)
            HorizontalLine.PositionFormatter = position => position.ToString("F2", CultureUI);

        this.Plot.XAxis.SetCulture(CultureUI);
        this.Plot.YAxis.SetCulture(CultureUI);

        this.Refresh();
    }

    protected override void InitilizeContextMenu()
    {
        base.InitilizeContextMenu();

        int item = ContextMenu.Items.Add(new ToolStripSeparator());

        item = ContextMenu.Items.Add(new ToolStripMenuItem("Show crosshair", null, new EventHandler(RightClickMenu_CrossHair)));
        crossHairMenuItem = (ToolStripMenuItem)ContextMenu.Items[item];
        crossHairMenuItem.Name = "CrossHair";
    }

    /// <summary>
    /// Add vertical and horizontal plottable lines.
    /// Subscribe to the line's dragged events.
    /// </summary>
    private void CreateLines()
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

    private void ShowLines(bool showVertical = false, bool showHorizontal = false)
    {
        if (!showVertical && !showHorizontal) return;

        if (Plot.GetPlottables().Where(x => x is Plottable.VLine || x is Plottable.HLine).Any()) return;

        // There should be at last one plottable added, otherwise
        if (this.Plot.GetPlottables().Length >= 1)
            CreateLines();

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
    /// Unsubscribe to the lines's dragged events.
    /// </summary>
    private void DeleteLines()
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
        ShowCrossHair = false;
    }


    /// <summary>
    /// Launch the default right-click menu.
    /// </summary>
    protected override void CustomRightClickEvent(object? sender, EventArgs e)
    {
        //detachLegendMenuItem.Visible = Plot.Legend(null).Count > 0;
        crossHairMenuItem.Enabled = Plot.GetPlottables().Length > 0;
        //customMenu.Show(System.Windows.Forms.Cursor.Position);
        base.CustomRightClickEvent(sender, e);
    }
    
    private void RightClickMenu_CrossHair(object? sender, EventArgs e)
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


