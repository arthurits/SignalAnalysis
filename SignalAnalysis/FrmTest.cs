namespace SignalAnalysis;

public partial class FrmTest : Form
{
    public FrmTest()
    {
        InitializeComponent();
        this.formsPlot1.Plot.AddSignal(ScottPlot.DataGen.Sin(51));
        this.formsPlot1.Plot.AddSignal(ScottPlot.DataGen.Cos(51));
        //this.formsPlot1.X = 20;
        //this.formsPlot1.Y = 0.5;
        this.formsPlot1.Refresh();
        this.formsPlot1.Render();

        // https://github.com/ScottPlot/ScottPlot/issues/2007
        // https://scottplot.net/cookbook/4.1/category/plottable-axis-line-and-span/
        var crossHair = this.formsPlot1.Plot.AddCrosshair(0, 0);
        crossHair.VerticalLine.DragLimitMin = -1;
        crossHair.VerticalLine.DragLimitMax = 10;
        crossHair.VerticalLine.DragEnabled = true;
        crossHair.HorizontalLine.DragEnabled = true;

        var VLine1 = formsPlot1.Plot.AddVerticalLine(5, Color.Blue);
        VLine1.DragEnabled = true;

        //ScottPlot.Crosshair2 ch = new();
        //this.formsPlot1.Plot.Add(ch);

        //// you can style both lines at the same time
        //ch.VerticalLine.DragEnabled = true;
        //ch.VerticalLine.DragEnabled = true;
        ////ch.LineStyle = ScottPlot.LineStyle.Dash;
        ////ch.Color = System.Drawing.Color.Blue;
        //ch.LineWidth = 2;
    }

    private void FrmTest_Shown(object sender, EventArgs e)
    {
        var crossHair = this.formsPlot1.Plot.AddCrosshair(0, 0);
        crossHair.VerticalLine.DragLimitMin = -1;
        crossHair.VerticalLine.DragLimitMax = 10;
        crossHair.VerticalLine.DragEnabled = true;
        crossHair.HorizontalLine.DragEnabled = true;

        var VLine1 = formsPlot1.Plot.AddVerticalLine(5, Color.Blue);
        VLine1.DragEnabled = true;
    }

    private void formsPlot1_Load(object sender, EventArgs e)
    {
        var crossHair = this.formsPlot1.Plot.AddCrosshair(0, 0);
        crossHair.VerticalLine.DragLimitMin = -1;
        crossHair.VerticalLine.DragLimitMax = 10;
        crossHair.VerticalLine.DragEnabled = true;
        crossHair.HorizontalLine.DragEnabled = true;

        var VLine1 = formsPlot1.Plot.AddVerticalLine(5, Color.Blue);
        VLine1.DragEnabled = true;
    }
}
