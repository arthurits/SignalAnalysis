using System.Text;

namespace SignalAnalysis
{
    public partial class FrmMain : Form
    {
        private double[][] _signalData;
        private double[] _signalX;
        private int nSeries = 0;
        private int nPoints = 0;
        double nFreq = 0.0;

        public FrmMain()
        {
            InitializeComponent();
        }

        private void btnData_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using OpenFileDialog openDlg = new OpenFileDialog();

            openDlg.Title = "Select data file";
            openDlg.InitialDirectory = "c:\\";
            openDlg.Filter = "ErgoLux files (*.elux)|*.elux|Text files (*.txt)|*.txt|All files (*.*)|*.*";
            openDlg.FilterIndex = 1;
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

                GetELuxData(openDlg.FileName);

                //Read the contents of the file into a stream
                //var fileStream = openFileDialog.OpenFile();

                //using StreamReader reader = new StreamReader(fileStream);
                //fileContent = reader.ReadToEnd();

                

            }


            //MessageBox.Show(fileContent, "File Content at path: " + filePath, MessageBoxButtons.OK);
        }

        private void GetELuxData(string FileName)
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
            if (strLine == null || !double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out nFreq)) return;

            strLine = sr.ReadLine();    // Empty line
            strLine = sr.ReadLine();    // Column header lines

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
    }
}