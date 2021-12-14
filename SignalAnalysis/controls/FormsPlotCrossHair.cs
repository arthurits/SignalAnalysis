﻿using ScottPlot.Plottable;
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
    
    public ScottPlot.Plottable.VLine VerticalLine { get; private set; }
    public ScottPlot.Plottable.HLine HorizontalLine { get; private set; }

    public bool SnapToPoint { get; set; } = false;

    /// <summary>
    /// Sets color for horizontal and vertical lines and their position label backgrounds
    /// </summary>
    public System.Drawing.Color CrossHairColor
    {
        set
        {
            if (VerticalLine != null)
            {
                VerticalLine.Color = value;
                VerticalLine.PositionLabelBackground = System.Drawing.Color.FromArgb(200, value);
            }
            if (HorizontalLine != null)
            {
                HorizontalLine.Color = value;
                HorizontalLine.PositionLabelBackground = System.Drawing.Color.FromArgb(200, value);
            }
        }
        get => VerticalLine.Color;
    }


    public FormsPlotCrossHair()
        : base()
    {       
        this.DoubleClick += new System.EventHandler(OnDoubleClick);
        this.Refresh();
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

    /// <summary>
    /// Delete vertical and horizontal plottable lines.
    /// Unsubscribe to the line's dragged events.
    /// </summary>
    private void DeleteCrossHairLines()
    {
        VerticalLine.Dragged -= new System.EventHandler(OnDraggedVertical);
        this.Plot.Clear(typeof(Plottable.VLine));

        HorizontalLine.Dragged -= new System.EventHandler(OnDraggedHorizontal);
        this.Plot.Clear(typeof(Plottable.HLine));
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

        if (plotMethod == null) return (null, null, null);

        if (VerticalLine.IsVisible || HorizontalLine.IsVisible)
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
                VerticalLine.X = pointX.Value;
                HorizontalLine.Y = pointY.Value;
            }
        }
        return (pointX, pointY, pointIndex);
    }

    private void OnDoubleClick(object sender, EventArgs e)
    {
        if (this.Plot.GetPlottables().Length == 1)
            CreateCrossHairLines();

        VerticalLine.IsVisible = !VerticalLine.IsVisible;
        HorizontalLine.IsVisible = !HorizontalLine.IsVisible;

        if (VerticalLine.IsVisible && HorizontalLine.IsVisible)
            SnapLinesToPoint(ToX: true);
        else
            DeleteCrossHairLines();
    }

    private void OnDraggedVertical(object sender, EventArgs e)
    {
        // If we are reading from the sensor, then exit
        if (!VerticalLine.IsVisible || !SnapToPoint) return;

        var snap = SnapLinesToPoint(ToX: true);

        // Raise the custom event for the subscribers
        OnVLineDragged(new LineDragEventArgs(snap.pointX, snap.pointY, snap.pointIndex));
        //EventHandler<VLineDragEventArgs> handler = VLineDragged;
        //handler?.Invoke(this, new VLineDragEventArgs(pointX, pointY, pointIndex));

    }

    private void OnDraggedHorizontal(object sender, EventArgs e)
    {
        // If we are reading from the sensor, then exit
        if (!HorizontalLine.IsVisible || !SnapToPoint) return;

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
            //if (plottables[i] is not Plottable.VLine && plottables[i] is not Plottable.HLine)
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


