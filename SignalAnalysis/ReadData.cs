using System.Text;

namespace SignalAnalysis;

partial class FrmMain
{
    /// <summary>
    /// Reads data from an elux file and stores it into _signalData.
    /// </summary>
    /// <param name="FileName">Path (including name) of the elux file</param>
    private bool ReadELuxData(string FileName)
    {
        using var fs = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var sr = new StreamReader(fs, Encoding.UTF8);
        int nPoints = 0;
        bool result = false;

        string? strLine = sr.ReadLine();
        if (strLine != null && strLine != "ErgoLux data")
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }

        // Better implement a try parse block. Each read line should throw an exception instead of "return"
        strLine = sr.ReadLine();
        if (strLine != null)
        {
            if (!strLine.Contains("Start time: ", StringComparison.Ordinal))
            {
                using (new CenterWinDialog(this))
                    MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return result;
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
            return result;
        }

        strLine = sr.ReadLine();
        if (strLine != null && !strLine.Contains("Total measuring time: ", StringComparison.Ordinal))
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }

        strLine = sr.ReadLine();
        if (strLine != null && !strLine.Contains("Number of sensors: ", StringComparison.Ordinal))
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }
        if (strLine == null || !int.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out nSeries)) return result;
        if (nSeries == 0) return result;
        nSeries += 6;

        strLine = sr.ReadLine();
        if (strLine != null && !strLine.Contains("Number of data points: ", StringComparison.Ordinal))
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }
        if (strLine == null || !int.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out nPoints)) return result;
        if (nPoints == 0) return result;

        strLine = sr.ReadLine();
        if (strLine != null && !strLine.Contains("Sampling frequency: ", StringComparison.Ordinal))
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }
        if (strLine == null || !double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out nSampleFreq)) return result;

        strLine = sr.ReadLine();    // It should be an empty line
        if (strLine != string.Empty) return result;

        strLine = sr.ReadLine();    // Column header lines
        seriesLabels = strLine != null ? strLine.Split('\t') : Array.Empty<string>();

        return InitializeDataArrays(sr, nPoints);
    }

    /// <summary>
    /// Readas data from a sig-formatted file and stores it into _signalData.
    /// </summary>
    /// <param name="FileName">Path (including name) of the sig file</param>
    private bool ReadSigData(string FileName)
    {
        using var fs = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var sr = new StreamReader(fs, Encoding.UTF8);
        int nPoints = 0;
        bool result = false;

        string? strLine = sr.ReadLine();
        if (strLine != null && strLine != "SignalAnalysis data")
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }

        strLine = sr.ReadLine();
        if (strLine != null && !strLine.Contains("Number of data series: ", StringComparison.Ordinal))
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }
        if (strLine == null || !int.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out nSeries)) return result;
        if (nSeries == 0) return result;

        strLine = sr.ReadLine();
        if (strLine != null && !strLine.Contains("Number of data points: ", StringComparison.Ordinal))
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }
        if (strLine == null || !int.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out nPoints)) return result;
        if (nPoints == 0) return result;

        strLine = sr.ReadLine();
        if (strLine != null && !strLine.Contains("Sampling frequency: ", StringComparison.Ordinal))
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }
        if (strLine == null || !double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out nSampleFreq)) return result;

        strLine = sr.ReadLine();    // It should be an empty line
        if (strLine != string.Empty) return result;

        strLine = sr.ReadLine();    // Column header lines
        seriesLabels = strLine != null ? strLine.Split('\t') : Array.Empty<string>();

        return InitializeDataArrays(sr, nPoints);
    }

    /// <summary>
    /// Readas data from a text-formatted file and stores it into _signalData.
    /// </summary>
    /// <param name="FileName">Path (including name) of the text file</param>
    private bool ReadTextData(string FileName)
    {
        using var fs = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var sr = new StreamReader(fs, Encoding.UTF8);
        double readValue;
        int nPoints = 0;
        bool result = false;

        string? strLine = sr.ReadLine();
        if (strLine != null && strLine != "Signal analysis data")
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }

        strLine = sr.ReadLine();
        if (strLine != null && !strLine.Contains("Start time: ", StringComparison.Ordinal))
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }
        // Append millisecond pattern to current culture's full date time pattern
        string fullPattern = _settings.AppCulture.DateTimeFormat.FullDateTimePattern;
        fullPattern = System.Text.RegularExpressions.Regex.Replace(fullPattern, "(:ss|:s)", strMillisecondsFormat);
        if (strLine == null || !DateTime.TryParseExact(strLine[(strLine.IndexOf(":") + 2)..], fullPattern, _settings.AppCulture, System.Globalization.DateTimeStyles.None, out nStart)) return result;

        strLine = sr.ReadLine();
        if (strLine != null && !strLine.Contains("End time: ", StringComparison.Ordinal))
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }

        strLine = sr.ReadLine();
        if (strLine != null && !strLine.Contains("Total measuring time: ", StringComparison.Ordinal))
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }

        strLine = sr.ReadLine();
        if (strLine != null && !strLine.Contains("Number of data points: ", StringComparison.Ordinal))
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }
        if (strLine == null || !int.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out nPoints)) return result;
        if (nPoints == 0) return result;

        strLine = sr.ReadLine();
        if (strLine != null && !strLine.Contains("Sampling frequency: ", StringComparison.Ordinal))
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }
        if (strLine == null || !double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out nSampleFreq)) return result;

        strLine = sr.ReadLine();
        if (strLine != null && !strLine.Contains("Average illuminance: ", StringComparison.Ordinal))
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }
        if (strLine == null || !double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out readValue)) return result;
        Results.Average = readValue;

        strLine = sr.ReadLine();
        if (strLine != null && !strLine.Contains("Maximum illuminance: ", StringComparison.Ordinal))
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }
        if (strLine == null || !double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out readValue)) return result;
        Results.Maximum = readValue;

        strLine = sr.ReadLine();
        if (strLine != null && !strLine.Contains("Minimum illuminance: ", StringComparison.Ordinal))
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }
        if (strLine == null || !double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out readValue)) return result;
        Results.Minimum = readValue;

        strLine = sr.ReadLine();
        if (strLine != null && !strLine.Contains("Fractal dimension: ", StringComparison.Ordinal))
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }
        if (strLine == null || !double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out readValue)) return result;
        Results.FractalDimension = readValue;

        strLine = sr.ReadLine();
        if (strLine != null && !strLine.Contains("Fractal variance: ", StringComparison.Ordinal))
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }
        if (strLine == null || !double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out readValue)) return result;
        Results.FractalVariance = readValue;

        strLine = sr.ReadLine();
        if (strLine != null && !strLine.Contains("Approximate entropy: ", StringComparison.Ordinal))
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }
        if (strLine == null || !double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out readValue)) return result;
        Results.ApproximateEntropy = readValue;

        strLine = sr.ReadLine();
        if (strLine != null && !strLine.Contains("Sample entropy: ", StringComparison.Ordinal))
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show("Unable to read data from file:\nwrong file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }
        if (strLine == null || !double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out readValue)) return result;
        Results.SampleEntropy = readValue;

        strLine = sr.ReadLine();    // It should be an empty line
        if (strLine != string.Empty) return result;

        strLine = sr.ReadLine();    // Column header lines
        seriesLabels = strLine != null ? strLine.Split('\t') : Array.Empty<string>();
        seriesLabels = seriesLabels[1..];
        nSeries =seriesLabels.Length;

        return InitializeDataArrays(sr, nPoints, true);
    }

    /// <summary>
    /// Reads the numeric data section pointed at.
    /// </summary>
    /// <param name="sr">This reader should be pointing to the beginning of the numeric data section</param>
    private bool InitializeDataArrays(StreamReader sr, int points, bool IsFirstColumDateTime = false)
    {
        bool result = true;
        string? strLine;

        try {
            _settings.IndexStart = 0;
            _settings.IndexEnd = points - 1;

            // Initialize data arrays
            _signalData = new double[nSeries][];
            for (int i = 0; i < nSeries; i++)
                _signalData[i] = new double[points];

            // Read data into _plotData
            for (int i = 0; i < _signalData.Length; i++)
            {
                _signalData[i] = new double[points];
            }
            string[] data;
            int col = 0, row = 0;
            while ((strLine = sr.ReadLine()) != null)
            {
                data = strLine.Split("\t");
                for (col = IsFirstColumDateTime ? 1 : 0; col < data.Length; col++)
                {
                    double.TryParse(data[col], out _signalData[col - (IsFirstColumDateTime ? 1 : 0)][row]);
                }
                row++;
            }
        }
        catch (Exception ex)
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show("Unexpected error in 'InitializeDataArrays'." + Environment.NewLine + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            result = false;
        }

        return result;
    }

}

