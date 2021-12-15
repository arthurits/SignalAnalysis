using System.Text;
using FftSharp;

namespace SignalAnalysis;

public partial class FrmMain : Form
{
    private double[][] _signalData = Array.Empty<double[]>();
    private double[] _signalFFT = Array.Empty<double>();
    private double[] _signalX = Array.Empty<double>();
    private string[] _series = Array.Empty<string>();
    private int nSeries = 0;
    private int nPoints = 0;
    double nSampleFreq = 0.0;

    Task fractalTask;
    private CancellationTokenSource tokenSource;
    private CancellationToken token;

    /// <summary>
    /// https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2010/y99d1cd3(v=vs.100)?WT.mc_id=DT-MVP-5003235
    /// https://stackoverflow.com/questions/32989100/how-to-make-multi-language-app-in-winforms
    /// </summary>
    private System.Resources.ResourceManager StringsRM = new System.Resources.ResourceManager("SignalAnalysis.localization.strings", typeof(FrmMain).Assembly);

    public FrmMain()
    {
        InitializeComponent();
        this.plotOriginal.SnapToPoint = true;
        this.plotFFT.SnapToPoint = true;

        PopulateCboWindow();
        chkPower.Checked = true;

        UpdateUI_Language();
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

    private void btnData_Click(object sender, EventArgs e)
    {
        var fileContent = string.Empty;
        var filePath = string.Empty;

        using OpenFileDialog openDlg = new OpenFileDialog();

        openDlg.Title = "Select data file";
        openDlg.InitialDirectory = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\examples";
        openDlg.Filter = "ErgoLux files (*.elux)|*.elux|SignalAnalysis files (*.sig)|*.sig|Text files (*.txt)|*.txt|All files (*.*)|*.*";
        openDlg.FilterIndex = 4;
        openDlg.RestoreDirectory = true;

        DialogResult result;
        using (new CenterWinDialog(this))
            result = openDlg.ShowDialog(this);

        if (result == DialogResult.OK && openDlg.FileName != "")
        {
            //Get the path of specified file
            filePath = openDlg.FileName;
            lblData.Text = openDlg.FileName;

            if (".elux".Equals(Path.GetExtension(filePath), StringComparison.OrdinalIgnoreCase))
                ReadELuxData(filePath);
            else if (".sig".Equals(Path.GetExtension(filePath), StringComparison.OrdinalIgnoreCase))
                ReadSigData(filePath);
            else if (".txt".Equals(Path.GetExtension(filePath), StringComparison.OrdinalIgnoreCase))
                throw new Exception("No generic text file reader has yet been implemented.");

            PopulateCboSeries();
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
        UpdateOriginal();
        UpdateFractal(chkCumulative.Checked);
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

    private void chkProgressive_CheckedChanged(object sender, EventArgs e)
    {
        if (!chkCumulative.Checked)
            FrmMain_KeyPress(sender, new KeyPressEventArgs((char)Keys.Escape));

        UpdateFractal(chkCumulative.Checked);
    }

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
        this.btnData.Text = StringsRM.GetString("strBtnData");
        this.lblData.Text = StringsRM.GetString("strLblData");
        this.lblSeries.Text = StringsRM.GetString("strLblSeries");
        this.lblWindow.Text = StringsRM.GetString("strLblWindow");
        this.chkPower.Text = StringsRM.GetString("strChkPower");
        this.chkCumulative.Text = StringsRM.GetString("strChkCumulative");
        
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
            plotFFT.Plot.YLabel(chkPower.Checked ? StringsRM.GetString("strPlotFFTYLabelPow") : StringsRM.GetString("strPlotFFTXLabelMag"));
            plotFFT.Plot.XLabel(StringsRM.GetString("strPlotFFTXLabel"));
        }
        
    }

    private void cmdExport_Click(object sender, EventArgs e)
    {

    }
}
