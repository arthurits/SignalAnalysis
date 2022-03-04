using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScottPlot;

/// <summary>
/// The Crosshair plot type draws a vertical and horizontal line to label a point
/// on the plot and displays the coordinates of that point in labels that overlap
/// the axis tick labels. 
/// 
/// This plot type is typically used in combination with
/// MouseMove events to track the location of the mouse and/or with plot types that
/// have GetPointNearest() methods.
/// </summary>
public class Crosshair2 : Plottable.IPlottable
{
    public bool IsVisible { get; set; } = true;
    public int XAxisIndex { get; set; } = 0;
    public int YAxisIndex { get; set; } = 0;

    public readonly ScottPlot.Plottable.HLine HorizontalLine = new();

    public readonly ScottPlot.Plottable.VLine VerticalLine = new();

    /// <summary>
    /// X position (axis units) of the vertical line
    /// </summary>
    public double X { get => VerticalLine.X; set => VerticalLine.X = value; }

    /// <summary>
    /// X position (axis units) of the horizontal line
    /// </summary>
    public double Y { get => HorizontalLine.Y; set => HorizontalLine.Y = value; }

    /// <summary>
    /// Sets style for horizontal and vertical lines
    /// </summary>
    public LineStyle LineStyle
    {
        set
        {
            HorizontalLine.LineStyle = value;
            VerticalLine.LineStyle = value;
        }
        [Obsolete("The get method only remain for the compatibility. Get HorizontalLine.LineStyle and VerticalLine.LineStyle instead.")]
        get => HorizontalLine.LineStyle;
    }

    /// <summary>
    /// Sets the line width for vertical and horizontal lines
    /// </summary>
    public double LineWidth
    {
        set
        {
            HorizontalLine.LineWidth = value;
            VerticalLine.LineWidth = value;
        }
        [Obsolete("The get method only remain for the compatibility. Get HorizontalLine.LineWidth and VerticalLine.LineWidth instead.")]
        get => HorizontalLine.LineWidth;
    }

    /// <summary>
    /// Sets font of the position labels for horizontal and vertical lines
    /// </summary>
    public Drawing.Font LabelFont
    {
        set
        {
            HorizontalLine.PositionLabelFont = value;
            VerticalLine.PositionLabelFont = value;
        }
        [Obsolete("The get method only remain for the compatibility. Get HorizontalLine.PositionLabelFont and VerticalLine.PositionLabelFont instead.")]
        get => HorizontalLine.PositionLabelFont;
    }

    /// <summary>
    /// Sets background color of the position labels for horizontal and vertical lines
    /// </summary>
    public Color LabelBackgroundColor
    {
        set
        {
            HorizontalLine.PositionLabelBackground = value;
            VerticalLine.PositionLabelBackground = value;
        }
        [Obsolete("The get method only remain for the compatibility. Get HorizontalLine.PositionLabelBackground and VerticalLine.PositionLabelBackground instead.")]
        get => HorizontalLine.PositionLabelBackground;
    }

    /// <summary>
    /// Sets visibility of the text labels for each line drawn over the axis
    /// </summary>
    public bool PositionLabel
    {
        set
        {
            HorizontalLine.PositionLabel = value;
            VerticalLine.PositionLabel = value;
        }
    }

    /// <summary>
    /// Sets color for horizontal and vertical lines and their position label backgrounds
    /// </summary>
    public Color Color
    {
        set
        {
            HorizontalLine.Color = value;
            VerticalLine.Color = value;
            HorizontalLine.PositionLabelBackground = value;
            VerticalLine.PositionLabelBackground = value;
        }
    }

    public Crosshair2()
    {
        LineStyle = LineStyle.Dash;
        LineWidth = 1;
        Color = Color.FromArgb(200, Color.Red);
        PositionLabel = true;
        VerticalLine.DragEnabled = true;
        VerticalLine.Dragged += new System.EventHandler(OnDraggedVertical);
        HorizontalLine.DragEnabled = true;
    }

    public AxisLimits GetAxisLimits() => new(double.NaN, double.NaN, double.NaN, double.NaN);

    public Plottable.LegendItem[]? GetLegendItems() => null;

    public void ValidateData(bool deep = false) { }

    public void Render(PlotDimensions dims, Bitmap bmp, bool lowQuality = false)
    {
        if (IsVisible == false)
            return;

        HorizontalLine.Render(dims, bmp, lowQuality);
        VerticalLine.Render(dims, bmp, lowQuality);
    }

    private void OnDraggedVertical(object? sender, EventArgs e)
    {
        // If we are reading from the sensor, then exit
        //if (!vLine.IsVisible || !SnapToPoint) return;

        //var snap = SnapLinesToPoint(ToX: true);

        // Raise the custom event for the subscribers
        //OnVLineDragged(new LineDragEventArgs(snap.pointX, snap.pointY, snap.pointIndex));
        //EventHandler<VLineDragEventArgs> handler = VLineDragged;
        //handler?.Invoke(this, new VLineDragEventArgs(pointX, pointY, pointIndex));

    }


}

