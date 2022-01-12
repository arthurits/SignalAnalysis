using System.Text;

namespace SignalAnalysis;

partial class FrmMain
{
    /// <summary>
    /// Reads data from an elux file and stores it into _signalData.
    /// </summary>
    /// <param name="FileName">Path (including name) of the elux file</param>
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
        if (strLine != null)
        {
            if (!strLine.Contains("Start time: ", StringComparison.Ordinal))
            {
                using (new CenterWinDialog(this))
                    MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }
            else
                DateTime.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out nStart);
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

    /// <summary>
    /// Readas data from a sig-formatted file and stores it into _signalData.
    /// </summary>
    /// <param name="FileName">Path (including name) of the sig file</param>
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
        if (strLine != null && !strLine.Contains("Number of data series: ", StringComparison.Ordinal))
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

    /// <summary>
    /// Readas data from a text-formatted file and stores it into _signalData.
    /// </summary>
    /// <param name="FileName">Path (including name) of the text file</param>
    private void ReadTextData(string FileName)
    {
        using var fs = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var sr = new StreamReader(fs, Encoding.UTF8);
        double readValue;

        string? strLine = sr.ReadLine();
        if (strLine != null && strLine != "Signal analysis data")
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return;
        }

        strLine = sr.ReadLine();
        if (strLine != null && !strLine.Contains("Start time: ", StringComparison.Ordinal))
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return;
        }
        if (strLine == null || DateTime.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out nStart)) return;

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

        strLine = sr.ReadLine();
        if (strLine != null && !strLine.Contains("Average illuminance: ", StringComparison.Ordinal))
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return;
        }
        if (strLine == null || !double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out readValue)) return;
        Results.Average = readValue;

        strLine = sr.ReadLine();
        if (strLine != null && !strLine.Contains("Maximum illuminance: ", StringComparison.Ordinal))
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return;
        }
        if (strLine == null || !double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out readValue)) return;
        Results.Maximum = readValue;

        strLine = sr.ReadLine();
        if (strLine != null && !strLine.Contains("Minimum illuminance: ", StringComparison.Ordinal))
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return;
        }
        if (strLine == null || !double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out readValue)) return;
        Results.Minimum = readValue;

        strLine = sr.ReadLine();
        if (strLine != null && !strLine.Contains("Fractal dimension: ", StringComparison.Ordinal))
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return;
        }
        if (strLine == null || !double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out readValue)) return;
        Results.FractalDimension = readValue;

        strLine = sr.ReadLine();
        if (strLine != null && !strLine.Contains("Fractal variance: ", StringComparison.Ordinal))
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return;
        }
        if (strLine == null || !double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out readValue)) return;
        Results.FractalVariance = readValue;

        strLine = sr.ReadLine();
        if (strLine != null && !strLine.Contains("Approximate entropy: ", StringComparison.Ordinal))
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return;
        }
        if (strLine == null || !double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out readValue)) return;
        Results.ApproximateEntropy = readValue;

        strLine = sr.ReadLine();
        if (strLine != null && !strLine.Contains("Sample entropy: ", StringComparison.Ordinal))
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return;
        }
        if (strLine == null || !double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out readValue)) return;
        Results.SampleEntropy = readValue;

        strLine = sr.ReadLine();    // Empty line

        strLine = sr.ReadLine();    // Column header lines
        _series = strLine != null ? strLine.Split('\t') : Array.Empty<string>();

        InitializeDataArrays(sr);
    }

    /// <summary>
    /// Reads the numeric data section pointed at.
    /// </summary>
    /// <param name="sr">This reader should be pointing to the beginning of the numeric data section</param>
    private void InitializeDataArrays(StreamReader sr)
    {
        string? strLine;

        // Initialize data arrays
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

}

