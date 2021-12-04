using System.Text;
using FftSharp;

namespace SignalAnalysis
{
    public partial class FrmMain : Form
    {
        private double[][] _signalData = Array.Empty<double[]>();
        private double[] _signalFFT = Array.Empty<double>();
        private double[] _signalX = Array.Empty<double>();
        private string[] _series = Array.Empty<string>();
        private int nSeries = 0;
        private int nPoints = 0;
        double nSampleFreq = 0.0;

        public FrmMain()
        {
            InitializeComponent();

            PopulateCboWindow();
            chkLog.Checked = true;
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
            openDlg.InitialDirectory = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            openDlg.Filter = "ErgoLux files (*.elux)|*.elux|SignalAnalysis files (*.sig)|*.sig|Text files (*.txt)|*.txt|All files (*.*)|*.*";
            openDlg.FilterIndex = 4;
            openDlg.RestoreDirectory = true;

            DialogResult result;
            using (new CenterWinDialog(this))
            {
                result = openDlg.ShowDialog(this);
            }

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
                    throw new Exception("No generic text file reader has yet been implemented");

                PopulateCboSeries();

                //Read the contents of the file into a stream
                //var fileStream = openDlg.OpenFile();

                //using StreamReader reader = new StreamReader(fileStream);
                //fileContent = reader.ReadToEnd();

            }


            //MessageBox.Show(fileContent, "File Content at path: " + filePath, MessageBoxButtons.OK);
        }

        private void ReadELuxData(string FileName)
        {
            using var fs = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sr = new StreamReader(fs, Encoding.UTF8);

            string? strLine = sr.ReadLine();
            if (strLine != null && strLine != "ErgoLux data")
            {
                using (new CenterWinDialog(this))
                {
                    MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }

            // Better implement a try parse block. Each read line should throw an exception instead of "return"
            strLine = sr.ReadLine();
            if (strLine != null && !strLine.Contains("Start time: ", StringComparison.Ordinal))
            {
                using (new CenterWinDialog(this))
                {
                    MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }

            strLine = sr.ReadLine();
            if (strLine != null && !strLine.Contains("End time: ", StringComparison.Ordinal))
            {
                using (new CenterWinDialog(this))
                {
                    MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }

            strLine = sr.ReadLine();
            if (strLine != null && !strLine.Contains("Total measuring time: ", StringComparison.Ordinal))
            {
                using (new CenterWinDialog(this))
                {
                    MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }

            strLine = sr.ReadLine();
            if (strLine != null && !strLine.Contains("Number of sensors: ", StringComparison.Ordinal))
            {
                using (new CenterWinDialog(this))
                {
                    MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }
            if (strLine == null || !int.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out nSeries)) return;
            if (nSeries == 0) return;
            nSeries += 6;

            strLine = sr.ReadLine();
            if (strLine != null && !strLine.Contains("Number of data points: ", StringComparison.Ordinal))
            {
                using (new CenterWinDialog(this))
                {
                    MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }
            if (strLine == null || !int.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out nPoints)) return;
            if (nPoints == 0) return;

            strLine = sr.ReadLine();
            if (strLine != null && !strLine.Contains("Sampling frequency: ", StringComparison.Ordinal))
            {
                using (new CenterWinDialog(this))
                {
                    MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }
            if (strLine == null || !double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out nSampleFreq)) return;

            strLine = sr.ReadLine();    // Empty line
            
            strLine = sr.ReadLine();    // Column header lines
            _series = strLine != null ? strLine.Split('\t') : Array.Empty<string>();

            InitializeDataArrays(sr);
        }

        private void ReadSigData(string FileName)
        {
            using var fs = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sr = new StreamReader(fs, Encoding.UTF8);

            string? strLine = sr.ReadLine();
            if (strLine != null && strLine != "SignalAnalysis data")
            {
                using (new CenterWinDialog(this))
                {
                    MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }

            strLine = sr.ReadLine();
            if (strLine != null && !strLine.Contains("Number of series: ", StringComparison.Ordinal))
            {
                using (new CenterWinDialog(this))
                {
                    MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }
            if (strLine == null || !int.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out nSeries)) return;
            if (nSeries == 0) return;
            nSeries += 6;

            strLine = sr.ReadLine();
            if (strLine != null && !strLine.Contains("Number of points: ", StringComparison.Ordinal))
            {
                using (new CenterWinDialog(this))
                {
                    MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }
            if (strLine == null || !int.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out nPoints)) return;
            if (nPoints == 0) return;

            strLine = sr.ReadLine();
            if (strLine != null && !strLine.Contains("Sampling frequency: ", StringComparison.Ordinal))
            {
                using (new CenterWinDialog(this))
                {
                    MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }
            if (strLine == null || !double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out nSampleFreq)) return;

            strLine = sr.ReadLine();    // Empty line

            strLine = sr.ReadLine();    // Column header lines
            _series = strLine != null ? strLine.Split('\t') : Array.Empty<string>();

            InitializeDataArrays(sr);
        }

        private void InitializeDataArrays(StreamReader sr)
        {
            string? strLine;

            // Initialize data arrays
            _signalX = new double[nPoints];
            _signalData = new double[nSeries][];
            for (int i = 0; i < nSeries; i++)
                _signalData[i] = new double[nPoints];

            // Read data into _plotData
            for (int i = 0; i < _signalData.Length; i++)
            {
                _signalData[i] = new double[nPoints];
            }
            string[] data;
            int row = 0, col = 0;
            while ((strLine = sr.ReadLine()) != null)
            {
                data = strLine.Split("\t");
                for (row = 0; row < data.Length; row++)
                {
                    double.TryParse(data[row], out _signalData[row][col]);
                }
                col++;
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
            UpdateFractal(chkProgressive.Checked);
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
            UpdateFractal(chkProgressive.Checked);
        }

        private void chkLog_CheckedChanged(object sender, EventArgs e)
        {
            cboWindow_SelectedIndexChanged(this, EventArgs.Empty);
        }

        private void UpdateOriginal()
        {
            plotOriginal.Plot.Clear();
            plotOriginal.Plot.AddSignal(_signalData[cboSeries.SelectedIndex], nSampleFreq, label: cboSeries.SelectedItem.ToString());
            plotOriginal.Plot.Title("Input signal");
            plotOriginal.Plot.YLabel("Amplitude");
            plotOriginal.Plot.XLabel("Time (seconds)");
            plotOriginal.Plot.AxisAuto(0);
            plotOriginal.Refresh();
        }

        private void UpdateKernel(IWindow window)
        {
            double[] kernel = window.Create(nPoints);
            double[] pad = ScottPlot.DataGen.Zeros(kernel.Length / 4);
            double[] ys = pad.Concat(kernel).Concat(pad).ToArray();

            plotWindow.Plot.Clear();
            plotWindow.Plot.AddSignal(ys, nSampleFreq, Color.Red);
            plotWindow.Plot.AxisAuto(0);
            plotWindow.Plot.Title($"{window} Window");
            plotWindow.Plot.YLabel("Amplitude");
            plotWindow.Plot.XLabel("Time (seconds)");
            plotWindow.Refresh();
        }

        private void UpdateWindowed(double[] signal)
        {
            plotApplied.Plot.Clear();
            plotApplied.Plot.AddSignal(signal, nSampleFreq);
            plotApplied.Plot.Title("Windowed signal");
            plotApplied.Plot.YLabel("Amplitude");
            plotApplied.Plot.XLabel("Time (seconds)");
            plotApplied.Plot.AxisAuto(0);
            plotApplied.Refresh();
        }

        private void UpdateFractal(bool progressive = false)
        {
            if (_signalData.Length == 0) return;

            var cursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            var fractalDim = new FractalDimension(nSampleFreq, _signalData[cboSeries.SelectedIndex], progressive);
            var dimension = fractalDim.Dimension;

            plotFractal.Plot.Clear();
            if (progressive && fractalDim.ProgressDim != null)
            {
                plotFractal.Plot.AddSignal(fractalDim.ProgressDim, nSampleFreq, label: cboSeries.SelectedItem.ToString());
            }
            else
            {
                plotFractal.Plot.AddLine(0, dimension, (0, nPoints / nSampleFreq));
            }
            plotFractal.Plot.Title("Fractal dimension" + (progressive ? " (progressive)" : String.Empty) + " (H = " + dimension.ToString("#.00000") + ")");
            plotFractal.Plot.YLabel("Dimension (H)");
            plotFractal.Plot.XLabel("Time (seconds)");
            plotFractal.Plot.AxisAuto(0);
            plotFractal.Refresh();

            this.Cursor = cursor;
        }

        private void UpdateFFT(double[] signal)
        {
            double[] ys = chkLog.Checked ? Transform.FFTpower(signal) : Transform.FFTmagnitude(signal);

            // Plot the results
            plotFFT.Plot.Clear();
            plotFFT.Plot.AddSignal(ys, (double)ys.Length / nSampleFreq);
            plotFFT.Plot.Title("Fast Fourier Transform");
            plotFFT.Plot.YLabel(chkLog.Checked ? "Power (dB)" : "Magnitude (RMS²)");
            plotFFT.Plot.XLabel("Frequency (Hz)");
            plotFFT.Plot.AxisAuto(0);
            plotFFT.Refresh();
        }

        
    }
}