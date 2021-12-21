using System.Text;
using FftSharp;

namespace SignalAnalysis;

public partial class FrmMain : Form
{
    private double[][] _signalData = Array.Empty<double[]>();
    private double[] _signalRange = Array.Empty<double>();
    private double[] _signalFFT = Array.Empty<double>();
    private double[] _signalX = Array.Empty<double>();
    private string[] _series = Array.Empty<string>();
    private int nSeries = 0;
    private int nPoints = 0;
    double nSampleFreq = 0.0;
    DateTime nStart;
    clsSettings _settings = new();
    private struct Stats
    {
        public Stats(double Max = 0, double Min = 0, double Avg = 0, double FractalDim = 0, double AppEn = 0, double SampEn = 0)
        {
            Maximum = Max;
            Minimum = Min;
            Average = Avg;
            FractalDimension = FractalDim;
            ApproximateEntropy = AppEn;
            SampleEntropy = SampEn;
        }

        public double Maximum { get; set; }
        public double Minimum { get; set; }
        public double Average { get; set; }
        public double FractalDimension { get; set; }
        public double ApproximateEntropy { get; set; }
        public double SampleEntropy { get; set; }

        public override string ToString() => $"Average {Average} lux, maximum {Maximum} lux, minimum {Minimum} lux, fractal dimension {FractalDimension}, approximate entropy {ApproximateEntropy}, sample entropy {SampleEntropy})";
    }
    Stats Results;

    Task fractalTask;
    private CancellationTokenSource tokenSource;
    private CancellationToken token;

    /// <summary>
    /// https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2010/y99d1cd3(v=vs.100)?WT.mc_id=DT-MVP-5003235
    /// https://stackoverflow.com/questions/32989100/how-to-make-multi-language-app-in-winforms
    /// </summary>
    private System.Resources.ResourceManager StringsRM = new("SignalAnalysis.localization.strings", typeof(FrmMain).Assembly);

    public FrmMain()
    {
        InitializeToolStripPanel();
        InitializeComponent();
        
        this.plotOriginal.SnapToPoint = true;
        this.plotFFT.SnapToPoint = true;

        PopulateCboWindow();

        UpdateUI_Language();
    }

    // https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.toolstrippanel?view=windowsdesktop-6.0
    private void InitializeToolStripPanel()
    {
        String path = Path.GetDirectoryName(Environment.ProcessPath);

        ToolStripPanel tspTop = new();
        tspTop.Dock = DockStyle.Top;
        ToolStrip toolStripMain = new()
        {
            ImageScalingSize = new System.Drawing.Size(48, 48),
            Location = new System.Drawing.Point(0, 24),
            Renderer = new customRenderer<ToolStripButton>(System.Drawing.Brushes.SteelBlue, System.Drawing.Brushes.LightSkyBlue),
            RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional,
            Text = "Main toolbar"
        };

        ToolStripItem toolStripItem;
        toolStripItem = toolStripMain.Items.Add("Exit", new System.Drawing.Icon(path + @"\images\exit.ico", 48, 48).ToBitmap(), new EventHandler(Exit_Click));
        toolStripItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
        toolStripItem.Name = "Exit";
        toolStripItem = toolStripMain.Items.Add("Open", new System.Drawing.Icon(path + @"\images\openfolder.ico", 48, 48).ToBitmap(), new EventHandler(Open_Click));
        toolStripItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
        toolStripItem = toolStripMain.Items.Add("Export", new System.Drawing.Icon(path + @"\images\save.ico", 48, 48).ToBitmap(), new EventHandler(Export_Click));
        toolStripItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
        toolStripMain.Items.Add(new ToolStripSeparator());
        toolStripItem = toolStripMain.Items.Add("Settings", new System.Drawing.Icon(path + @"\images\settings.ico", 48, 48).ToBitmap(), new EventHandler(Settings_Click));
        toolStripItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
        toolStripMain.Items.Add(new ToolStripSeparator());
        toolStripItem = toolStripMain.Items.Add("About", new System.Drawing.Icon(path + @"\images\about.ico", 48, 48).ToBitmap(), new EventHandler(About_Click));
        toolStripItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
        tspTop.Join(toolStripMain);


        ToolStripPanel tspBottom = new();
        tspBottom.Dock = DockStyle.Bottom;

        StatusStrip statusStrip = new()
        {
            ShowItemToolTips = true,
            TabIndex = 1,
            Text = "Status bar"
        };

        int item;
        item = statusStrip.Items.Add(new ToolStripStatusLabel()
        {
            AutoSize = true,
            BorderSides = ToolStripStatusLabelBorderSides.Right,
            DisplayStyle = ToolStripItemDisplayStyle.Text,
            Name = "LabelEmpty",
            Spring = true,
            TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
            ToolTipText = ""
        });

        item = statusStrip.Items.Add(new ToolStripStatusLabelEx()
        {
            AutoSize = false,
            Checked = false,
            DisplayStyle = ToolStripItemDisplayStyle.Text,
            Name = "LabelExPower",
            Size = new System.Drawing.Size(28, 23),
            Text = "P",
            ToolTipText = "Power spectra (dB)"
        });
        statusStrip.Items[item].Click += LabelExPower_Click;

        var test = new ToolStripStatusLabelEx();
        

        //tspTop.Join(mnuMainFrm);
        tspBottom.Join(statusStrip);

        //this.Controls.Add(tspBottom);
        this.Controls.Add(tspTop);
        this.Controls.Add(tspBottom);

        // Exit the method
        return;
    }

    private void toolStripMain_Exit_Click(object sender, EventArgs e)
    {
        this.Close();
    }

    private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
    {
        using (new CenterWinDialog(this))
        {
            if (DialogResult.No == MessageBox.Show(this,
                                                    "Are you sure you want to exit\nthe application?",
                                                    "Exit?",
                                                    MessageBoxButtons.YesNo,
                                                    MessageBoxIcon.Question,
                                                    MessageBoxDefaultButton.Button2))
            {
                // Cancel
                e.Cancel = true;
            }
        }
    }

    

    private void PopulateCboSeries(params string[] values)
    {
        cboSeries.Items.Clear();
        if (values.Length != 0)
            cboSeries.Items.AddRange(values);
        else
            cboSeries.Items.AddRange(_series);
        cboSeries.SelectedIndex = 0;
    }
    private void PopulateCboWindow()
    {
        IWindow[] windows = Window.GetWindows();
        cboWindow.Items.AddRange(windows);
        cboWindow.SelectedIndex = windows.ToList().FindIndex(x => x.Name == "Hanning");
    }

    private void cboSeries_SelectedIndexChanged(object sender, EventArgs e)
    {
        //int nStart = int.Parse(txtStart.Text);
        //int nLength = int.Parse(txtEnd.Text) - nStart + 1;
        nPoints = _settings.IndexEnd - _settings.IndexStart + 1;
        _signalRange = new double[_settings.IndexEnd - _settings.IndexStart + 1];

        Array.Copy(
            _signalData[cboSeries.SelectedIndex],
            _settings.IndexStart,
            _signalRange,
            0,
            _settings.IndexEnd - _settings.IndexStart + 1);

        UpdateOriginal(_signalRange);
        UpdateStats(_signalRange, nPoints <= 50000);
        UpdateFractal(_signalRange, _settings.CumulativeDimension);
        cboWindow_SelectedIndexChanged(this, EventArgs.Empty);

    }

    private void cboWindow_SelectedIndexChanged(object sender, EventArgs e)
    {
        IWindow window = (IWindow)cboWindow.SelectedItem;
        if (window is null)
        {
            //richTextBox1.Clear();
            return;
        }
        else
        {
            //richTextBox1.Text = window.Description;
        }

        if (nPoints == 0) return;

        // Adjust to the lowest power of 2
        int power2 = (int)Math.Log2(nPoints);
        //int evenPower = (power2 % 2 == 0) ? power2 : power2 - 1;
        _signalFFT = new double[(int)Math.Pow(2, power2)];
        Array.Copy(_signalData[cboSeries.SelectedIndex], _signalFFT, _signalFFT.Length);

        // apply window
        double[] signalWindow = new double[_signalFFT.Length];
        Array.Copy(_signalFFT, signalWindow, _signalFFT.Length);
        window.ApplyInPlace(signalWindow);

        UpdateKernel(window);
        UpdateWindowed(signalWindow);
        UpdateFFT(signalWindow);
    }

    //private void chkProgressive_CheckedChanged(object sender, EventArgs e)
    //{
    //    if (!chkCumulative.Checked)
    //        FrmMain_KeyPress(sender, new KeyPressEventArgs((char)Keys.Escape));

    //    UpdateFractal(_signalRange, _settings.CumulativeDimension);
    //}

    private void chkLog_CheckedChanged(object sender, EventArgs e)
    {
        cboWindow_SelectedIndexChanged(this, EventArgs.Empty);
    }

    private void FrmMain_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == (char)Keys.Escape && fractalTask.Status == TaskStatus.Running)
            tokenSource.Cancel();   
    }

    private void UpdateUI_Language()
    {
        this.Text = StringsRM.GetString("strFrmTitle");
        //this.btnData.Text = StringsRM.GetString("strBtnData");
        //this.btnExport.Text = StringsRM.GetString("strBtnExport");
        //this.btnSettings.Text = StringsRM.GetString("strBtnSettings");
        this.lblSeries.Text = StringsRM.GetString("strLblSeries");
        this.lblWindow.Text = StringsRM.GetString("strLblWindow");

        
        // Update plots if they contain series
        if(plotOriginal.Plot.GetPlottables().Length > 2)
        {
            plotOriginal.Plot.Title(StringsRM.GetString("strPlotOriginalTitle"));
            plotOriginal.Plot.YLabel(StringsRM.GetString("strPlotOriginalYLabel"));
            plotOriginal.Plot.XLabel(StringsRM.GetString("strPlotOriginalXLabel"));
        }

        if (plotWindow.Plot.GetPlottables().Length > 2)
        {
            plotWindow.Plot.Title(StringsRM.GetString("strPlotWindowTitle"));
            plotWindow.Plot.YLabel(StringsRM.GetString("strPlotWindowYLabel"));
            plotWindow.Plot.XLabel(StringsRM.GetString("strPlotWindowXLabel"));
        }
        if (plotApplied.Plot.GetPlottables().Length > 2)
        {
            plotApplied.Plot.Title(StringsRM.GetString("strPlotAppliedTitle"));
            plotApplied.Plot.YLabel(StringsRM.GetString("strPlotAppliedYLabel"));
            plotApplied.Plot.XLabel(StringsRM.GetString("strPlotAppliedXLabel"));
        }

        if (plotFractal.Plot.GetPlottables().Length > 2)
        {
            plotFractal.Plot.YLabel(StringsRM.GetString("strPlotFractalYLabel"));
            plotFractal.Plot.XLabel(StringsRM.GetString("strPlotFractalXLabel"));
        }

        if (plotFFT.Plot.GetPlottables().Length > 2)
        {
            plotFFT.Plot.Title(StringsRM.GetString("strPlotFFTTitle"));
            plotFFT.Plot.YLabel(_settings.PowerSpectra ? StringsRM.GetString("strPlotFFTYLabelPow") : StringsRM.GetString("strPlotFFTXLabelMag"));
            plotFFT.Plot.XLabel(StringsRM.GetString("strPlotFFTXLabel"));
        }
        
    }

}
