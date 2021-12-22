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
    ToolStripComboBox stripComboSeries;
    ToolStripComboBox stripComboWindows;


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
        InitializeStatusStrip();
        InitializeMenu();
        InitializeComponent();
        
        this.plotOriginal.SnapToPoint = true;
        this.plotFFT.SnapToPoint = true;

        PopulateComboWindow();

        UpdateUI_Language();
    }

    // https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.toolstrippanel?view=windowsdesktop-6.0
    // https://stackoverflow.com/questions/40382105/how-to-add-two-toolstripcombobox-and-separator-horizontally-to-one-toolstripdrop
    private void InitializeToolStripPanel()
    {
        String path = Path.GetDirectoryName(Environment.ProcessPath);

        stripComboSeries = new()
        {
            AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend,
            AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None,
            DropDownHeight = 110,
            DropDownWidth = 122,
            FlatStyle = System.Windows.Forms.FlatStyle.Standard,
            IntegralHeight = true,
            MaxDropDownItems = 9,
            MergeAction = System.Windows.Forms.MergeAction.MatchOnly,
            Name = "cboSeries",
            Size = new System.Drawing.Size(120, 25),
            Sorted = false,
            ToolTipText = "Select data series"
        };
        stripComboWindows = new()
        {
            AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend,
            AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None,
            DropDownHeight = 110,
            DropDownWidth = 122,
            FlatStyle = System.Windows.Forms.FlatStyle.Standard,
            IntegralHeight = true,
            MaxDropDownItems = 9,
            MergeAction = System.Windows.Forms.MergeAction.MatchOnly,
            Name = "cboWindows",
            Size = new System.Drawing.Size(120, 25),
            Sorted = false,
            ToolTipText = "Select FFT window"
        };
        stripComboSeries.SelectedIndexChanged += ComboSeries_SelectedIndexChanged;
        stripComboWindows.SelectedIndexChanged += ComboWindow_SelectedIndexChanged;

        ToolStripPanel tspTop = new();
        tspTop.Dock = DockStyle.Top;
        ToolStrip toolStripMain = new()
        {
            ImageScalingSize = new System.Drawing.Size(48, 48),
            Location = new System.Drawing.Point(0, 0),
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
        toolStripItem.Name = "Open";
        toolStripItem = toolStripMain.Items.Add("Export", new System.Drawing.Icon(path + @"\images\save.ico", 48, 48).ToBitmap(), new EventHandler(Export_Click));
        toolStripItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
        toolStripItem.Name = "Export";
        toolStripMain.Items.Add(new ToolStripSeparator());
        toolStripMain.Items.Add(stripComboSeries);
        toolStripMain.Items.Add(stripComboWindows);
        toolStripMain.Items.Add(new ToolStripSeparator());
        toolStripItem = toolStripMain.Items.Add("Settings", new System.Drawing.Icon(path + @"\images\settings.ico", 48, 48).ToBitmap(), new EventHandler(Settings_Click));
        toolStripItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
        toolStripItem.Name = "Settings";
        toolStripMain.Items.Add(new ToolStripSeparator());
        toolStripItem = toolStripMain.Items.Add("About", new System.Drawing.Icon(path + @"\images\about.ico", 48, 48).ToBitmap(), new EventHandler(About_Click));
        toolStripItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
        toolStripItem.Name = "About";

        tspTop.Join(toolStripMain);
        this.Controls.Add(tspTop);
        
        return;
    }

    private void InitializeStatusStrip()
    {
        ToolStripPanel tspBottom = new();
        tspBottom.Dock = DockStyle.Bottom;

        StatusStrip statusStrip = new()
        {
            ShowItemToolTips = true,
            Renderer = new customRenderer<ToolStripStatusLabelEx>(Brushes.SteelBlue, Brushes.LightSkyBlue),
            RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional,
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
            Checked = true,
            DisplayStyle = ToolStripItemDisplayStyle.Text,
            Name = "LabelExPower",
            Size = new System.Drawing.Size(28, 23),
            Text = "P",
            ToolTipText = "Power spectra (dB)"
        });
        statusStrip.Items[item].Click += LabelEx_Click;
        item = statusStrip.Items.Add(new ToolStripStatusLabelEx()
        {
            AutoSize = false,
            Checked = false,
            DisplayStyle = ToolStripItemDisplayStyle.Text,
            Name = "LabelExCumulative",
            Size = new System.Drawing.Size(28, 23),
            Text = "F",
            ToolTipText = "Cumulative fractal dimension"
        });
        statusStrip.Items[item].Click += LabelEx_Click;
        item = statusStrip.Items.Add(new ToolStripStatusLabelEx()
        {
            AutoSize = false,
            Checked = false,
            DisplayStyle = ToolStripItemDisplayStyle.Text,
            Name = "LabelExEntropy",
            Size = new System.Drawing.Size(28, 23),
            Text = "E",
            ToolTipText = "Approximate and sample entropy"
        });
        statusStrip.Items[item].Click += LabelEx_Click;
        item = statusStrip.Items.Add(new ToolStripStatusLabelEx()
        {
            AutoSize = false,
            Checked = false,
            DisplayStyle = ToolStripItemDisplayStyle.Text,
            Name = "LabelExCrossHair",
            Size = new System.Drawing.Size(28, 23),
            Text = "C",
            ToolTipText = "Plot's crosshair mode"
        });
        statusStrip.Items[item].Click += LabelEx_Click;

        tspBottom.Join(statusStrip);
        this.Controls.Add(tspBottom);
    }

    private void InitializeMenu()
    {

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

    

    private void PopulateComboSeries(params string[] values)
    {
        stripComboSeries.Items.Clear();
        if (values.Length != 0)
            stripComboSeries.Items.AddRange(values);
        else
            stripComboSeries.Items.AddRange(_series);
        stripComboSeries.SelectedIndex = 0;
    }
    private void PopulateComboWindow()
    {
        IWindow[] windows = Window.GetWindows();
        stripComboWindows.Items.AddRange(windows);
        stripComboWindows.SelectedIndex = windows.ToList().FindIndex(x => x.Name == "Hanning");
    }

    private void ComboSeries_SelectedIndexChanged(object sender, EventArgs e)
    {
        int nIndex = stripComboSeries.SelectedIndex;
        nPoints = _settings.IndexEnd - _settings.IndexStart + 1;
        _signalRange = new double[nPoints];

        Array.Copy(
            _signalData[nIndex],
            _settings.IndexStart,
            _signalRange,
            0,
            nPoints);

        UpdateOriginal(_signalRange);
        UpdateStats(_signalRange, _settings.Entropy);
        UpdateFractal(_signalRange, _settings.CumulativeDimension);
        ComboWindow_SelectedIndexChanged(this, EventArgs.Empty);
    }

    private void ComboWindow_SelectedIndexChanged(object sender, EventArgs e)
    {
        IWindow window = (IWindow)stripComboWindows.SelectedItem;
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
        Array.Copy(_signalData[stripComboSeries.SelectedIndex], _signalFFT, _signalFFT.Length);

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
        ComboWindow_SelectedIndexChanged(this, EventArgs.Empty);
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
        //this.lblSeries.Text = StringsRM.GetString("strLblSeries");
        //this.lblWindow.Text = StringsRM.GetString("strLblWindow");

        
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
