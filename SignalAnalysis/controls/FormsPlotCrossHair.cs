using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScottPlot;

public class FormsPlotCrossHair : ScottPlot.FormsPlot
{
    public event EventHandler<LineDragEventArgs> VLineDragged;
    public event EventHandler<LineDragEventArgs> HLineDragged;
    //private ScottPlot.Plottable.Crosshair crossHair;
    private ScottPlot.Plottable.VLine vLine;
    private ScottPlot.Plottable.HLine hLine;
    public ScottPlot.Plottable.VLine GetVerticalLine => vLine;
    public ScottPlot.Plottable.HLine GetHorizontalLine => hLine;

    private bool _verticalLine;
    public bool VerticalLine
    {
        get => _verticalLine;
        set
        {
            _verticalLine = value;
            //vLine.IsVisible = value;
        }
    }

    private bool _horizontalLine;
    public bool HorizontalLine
    {
        get => _horizontalLine;
        set
        {
            _horizontalLine = value;
            //hLine.IsVisible = value;
        }
    }

    public bool SnapToPoint { get; set; } = false;

    /// <summary>
    /// Sets color for horizontal and vertical lines and their position label backgrounds
    /// </summary>
    public System.Drawing.Color CrossHairColor
    {
        set
        {
            vLine.Color = value;
            hLine.Color = value;
            vLine.PositionLabelBackground = System.Drawing.Color.FromArgb(200, value);
            hLine.PositionLabelBackground = System.Drawing.Color.FromArgb(200, value);
        }
        get => vLine.Color;
    }


    public FormsPlotCrossHair()
        : base()
    {
        //crossHair = this.Plot.AddCrosshair(0.0, 0.0);
        //crossHair.IsVisible = false;

        vLine = this.Plot.AddVerticalLine(0.0, style: ScottPlot.LineStyle.Dash);
        //VerticalLine = true;
        vLine.IsVisible = false;
        vLine.PositionLabel = true;
        //vLine.PositionLabelBackground = System.Drawing.Color.FromArgb(200, color);
        vLine.DragEnabled = true;
        vLine.Dragged += new System.EventHandler(OnDraggedVertical);
        //crossHair.VerticalLine.DragEnabled = true;
        //crossHair.VerticalLine.Dragged += new System.EventHandler(OnDraggedVertical);

        hLine = this.Plot.AddHorizontalLine(0.0, color: System.Drawing.Color.Red, width: 1, style: ScottPlot.LineStyle.Dash);
        //HorizontalLine = true;
        hLine.IsVisible = false;
        hLine.PositionLabel = true;
        //hLine.PositionLabelBackground = System.Drawing.Color.FromArgb(150, System.Drawing.Color.Red);
        hLine.DragEnabled = true;
        hLine.Dragged += new System.EventHandler(OnDraggedHorizontal);
        //crossHair.HorizontalLine.DragEnabled = true;
        //crossHair.HorizontalLine.Dragged += new System.EventHandler(OnDraggedHorizontal);

        CrossHairColor = System.Drawing.Color.FromArgb(200, System.Drawing.Color.Red);

        this.DoubleClick += new System.EventHandler(OnDoubleClick);
        this.Refresh();

    }

    private (double? pointX, double? pointY, int? pointIndex) SnapLinesToPoint(bool ToX = false, bool ToY = false)
    {
        double? pointX = null;
        double? pointY = null;
        int? pointIndex = null;

        var plot = this.Plot.GetPlottables()[2];
        var plotType = (this.Plot.GetPlottables()[2]).GetType();
        System.Reflection.MethodInfo? plotMethod = null;
        if (ToX)
            plotMethod = plotType.GetMethod("GetPointNearestX");
        else if(ToY)
            plotMethod = plotType.GetMethod("GetPointNearestY");

        if (plotMethod == null) return (null, null, null);

        if (vLine.IsVisible || hLine.IsVisible)
        {
            (double mouseCoordX, double mouseCoordY) = this.GetMouseCoordinates();
            var param = new object[1];
            if (ToX)
                param[0] = mouseCoordX;
            else if (ToY) 
                param[0] = mouseCoordY;

            var result = plotMethod.Invoke(plot, param);
            if (result != null)
            {
                (pointX, pointY, pointIndex) = ((double, double, int))result;
                vLine.X = pointX.Value;
                hLine.Y = pointY.Value;
            }
        }
        return (pointX, pointY, pointIndex);
    }

    private void OnDoubleClick(object sender, EventArgs e)
    {
        if (this.Plot.GetPlottables().Count() <= 2) return;

        vLine.IsVisible = !vLine.IsVisible;
        hLine.IsVisible = !hLine.IsVisible;

        SnapLinesToPoint(ToX: true);
    }

    private void OnDraggedVertical(object sender, EventArgs e)
    {
        // If we are reading from the sensor, then exit
        if (!vLine.IsVisible || !SnapToPoint) return;

        var snap = SnapLinesToPoint(ToX: true);

        // Raise the custom event for the subscribers
        OnVLineDragged(new LineDragEventArgs(snap.pointX, snap.pointY, snap.pointIndex));
        //EventHandler<VLineDragEventArgs> handler = VLineDragged;
        //handler?.Invoke(this, new VLineDragEventArgs(pointX, pointY, pointIndex));

    }

    private void OnDraggedHorizontal(object sender, EventArgs e)
    {
        // If we are reading from the sensor, then exit
        if (!hLine.IsVisible || !SnapToPoint) return;

        var snap = SnapLinesToPoint(ToY: true);

        // Raise the custom event for the subscribers
        OnHLineDragged(new LineDragEventArgs(snap.pointX, snap.pointY, snap.pointIndex));
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
        var dataPlots = this.Plot.GetPlottables().Where(x => !(x is Plottable.VLine) && !(x is Plottable.HLine));

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
        EventHandler<LineDragEventArgs> raiseEvent = VLineDragged;

        // Event will be null if there are no subscribers
        if (raiseEvent != null)
        {
            // Format arguments if needed
            //e.pointX = 0.0;
            //e.pointY = 0.0;
            //e.pointIndex = 0;

            // Call to raise the event.
            raiseEvent(this, e);
        }
    }

    protected virtual void OnHLineDragged(LineDragEventArgs e)
    {
        // Make a temporary copy of the event to avoid possibility of
        // a race condition if the last subscriber unsubscribes
        // immediately after the null check and before the event is raised.
        EventHandler<LineDragEventArgs> raiseEvent = HLineDragged;

        // Event will be null if there are no subscribers
        if (raiseEvent != null)
        {
            // Format arguments if needed
            //e.pointX = 0.0;
            //e.pointY = 0.0;
            //e.pointIndex = 0;

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
            if (plottables[i] is not Plottable.VLine && plottables[i] is not Plottable.HLine)
                this.Plot.RemoveAt(i);
        }
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();
        // 
        // FormsPlotCrossHair
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.Name = "FormsPlotCrossHair";
        this.ResumeLayout(false);

    }

}

public class LineDragEventArgs : EventArgs
{
    public LineDragEventArgs(double? X, double? Y, int? Index)
    {
        pointX = X ?? default;
        pointY = Y ?? default;
        pointIndex = Index ?? default;
    }

    public double pointX { get; set; }
    public double pointY { get; set; }
    public int pointIndex { get; set; }

}


