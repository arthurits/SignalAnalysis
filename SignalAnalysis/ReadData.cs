namespace SignalAnalysis;

partial class FrmMain
{
    /// <summary>
    /// Reads data from an elux-formatted file and stores it into _signalData.
    /// </summary>
    /// <param name="FileName">Path (including name) of the elux file</param>
    /// <returns><see langword="True"/> if successful, <see langword="false"/> otherwise</returns>
    private bool ReadELuxData(string FileName)
    {
        int nPoints = 0;
        bool result = true;
        string? strLine;

        try
        {
            using var fs = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sr = new StreamReader(fs, System.Text.Encoding.UTF8);
            
            strLine = sr.ReadLine();    // ErgoLux data
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strELuxHeader01", _settings.AppCulture) ?? "Section 'ErgoLux data' is mis-formatted.");
            if (!strLine.Contains("ErgoLux data (", StringComparison.Ordinal))
                throw new FormatException(StringsRM.GetString("strELuxHeader01", _settings.AppCulture) ?? "Section 'ErgoLux data' is mis-formatted.");
            System.Globalization.CultureInfo fileCulture = new(strLine[(strLine.IndexOf("(") + 1)..^1]);

            strLine = sr.ReadLine();    // Start time
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strELuxHeader02", _settings.AppCulture) ?? "Section 'Start time' is mis-formatted.");
            if (!strLine.Contains("Start time: ", StringComparison.Ordinal))
                throw new FormatException(StringsRM.GetString("strELuxHeader02", _settings.AppCulture) ?? "Section 'Start time' is mis-formatted.");
            string fullPattern = fileCulture.DateTimeFormat.FullDateTimePattern;
            fullPattern = System.Text.RegularExpressions.Regex.Replace(fullPattern, "(:ss|:s)", ClassSettings.GetMillisecondsFormat(fileCulture));
            if (strLine == null || !DateTime.TryParseExact(strLine[(strLine.IndexOf(":") + 2)..], fullPattern, fileCulture, System.Globalization.DateTimeStyles.None, out nStart))
                throw new FormatException(StringsRM.GetString("strELuxHeader02", _settings.AppCulture) ?? "Section 'Start time' is mis-formatted.");

            strLine = sr.ReadLine();    // End time
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strELuxHeader03", _settings.AppCulture) ?? "Section 'End time' is mis-formatted.");
            if (!strLine.Contains("End time: ", StringComparison.Ordinal))
                throw new FormatException(StringsRM.GetString("strELuxHeader03", _settings.AppCulture) ?? "Section 'End time' is mis-formatted.");

            strLine = sr.ReadLine();    // Total measuring time
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strELuxHeader04", _settings.AppCulture) ?? "Section 'Total measuring time' is mis-formatted.");
            if (!strLine.Contains("Total measuring time: ", StringComparison.Ordinal))
                throw new FormatException(StringsRM.GetString("strELuxHeader04", _settings.AppCulture) ?? "Section 'Total measuring time' is mis-formatted.");

            strLine = sr.ReadLine();    // Number of sensors
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strELuxHeader05", _settings.AppCulture) ?? "Section 'Number of sensors' is mis-formatted.");
            if (!strLine.Contains("Number of sensors: ", StringComparison.Ordinal))
                throw new FormatException(StringsRM.GetString("strELuxHeader05", _settings.AppCulture) ?? "Section 'Number of sensors' is mis-formatted.");
            if (!int.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out nSeries))
                throw new FormatException(StringsRM.GetString("strELuxHeader05", _settings.AppCulture) ?? "Section 'Number of sensors' is mis-formatted.");
            if (nSeries == 0)
                throw new FormatException(StringsRM.GetString("strELuxHeader05", _settings.AppCulture) ?? "Section 'Number of sensors' is mis-formatted.");
            nSeries += 6;

            strLine = sr.ReadLine();    // Number of data points
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strELuxHeader06", _settings.AppCulture) ?? "Section 'Number of data points' is mis-formatted.");
            if (!strLine.Contains("Number of data points: ", StringComparison.Ordinal))
                throw new FormatException(StringsRM.GetString("strELuxHeader06", _settings.AppCulture) ?? "Section 'Number of data points' is mis-formatted.");
            if (!int.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out nPoints))
                throw new FormatException(StringsRM.GetString("strELuxHeader06", _settings.AppCulture) ?? "Section 'Number of data points' is mis-formatted.");
            if (nPoints == 0)
                throw new FormatException(StringsRM.GetString("strELuxHeader06", _settings.AppCulture) ?? "Section 'Number of data points' is mis-formatted.");

            strLine = sr.ReadLine();    // Sampling frequency
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strELuxHeader07", _settings.AppCulture) ?? "Section 'Sampling frequency' is mis-formatted.");
            if (!strLine.Contains("Sampling frequency: ", StringComparison.Ordinal))
                throw new FormatException(StringsRM.GetString("strELuxHeader07", _settings.AppCulture) ?? "Section 'Sampling frequency' is mis-formatted.");
            if (!double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out nSampleFreq))
                throw new FormatException(StringsRM.GetString("strELuxHeader07", _settings.AppCulture) ?? "Section 'Sampling frequency' is mis-formatted.");
            if (nSampleFreq <= 0)
                throw new FormatException(StringsRM.GetString("strELuxHeader07", _settings.AppCulture) ?? "Section 'Sampling frequency' is mis-formatted.");

            strLine = sr.ReadLine();    // Empty line
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strELuxHeader08", _settings.AppCulture) ?? "Missing an empty line.");
            if (strLine != string.Empty)
                throw new FormatException(StringsRM.GetString("strELuxHeader08", _settings.AppCulture) ?? "Missing an empty line.");

            strLine = sr.ReadLine();    // Column header lines
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strELuxHeader09", _settings.AppCulture) ?? "Missing column headers (series names).");
            seriesLabels = strLine.Split('\t');
            if (seriesLabels == Array.Empty<string>())
                throw new FormatException(StringsRM.GetString("strELuxHeader09", _settings.AppCulture) ?? "Missing column headers (series names).");

            result = InitializeDataArrays(sr, nPoints, fileCulture);
        }
        catch (System.Globalization.CultureNotFoundException ex)
        {
            result = false;
            using (new CenterWinDialog(this))
                MessageBox.Show(String.Format(StringsRM.GetString("strReadDataErrorCulture", _settings.AppCulture) ?? "The culture identifier string name is not valid.\n{0}", ex.Message),
                    StringsRM.GetString("strReadDataErrorCultureTitle" ?? "Culture name error", _settings.AppCulture),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
        }
        catch (FormatException ex)
        {
            result = false;
            using (new CenterWinDialog(this))
                MessageBox.Show(String.Format(StringsRM.GetString("strReadDataError", _settings.AppCulture) ?? "Unable to read data from file.\n{0}", ex.Message),
                    StringsRM.GetString("strReadDataErrorTitle", _settings.AppCulture) ?? "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            result = false;
            using (new CenterWinDialog(this))
            {
                MessageBox.Show(String.Format(StringsRM.GetString("strMsgBoxErrorOpenData", _settings.AppCulture) ?? "An unexpected error happened while opening file data.\nPlease try again later or contact the software engineer." + Environment.NewLine + "{0}", ex.Message),
                    StringsRM.GetString("strMsgBoxErrorOpenDataTitle", _settings.AppCulture) ?? "Error opening data",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        return result;
    }

    /// <summary>
    /// Readas data from a signal-formatted file and stores it into _signalData.
    /// </summary>
    /// <param name="FileName">Path (including name) of the sig file</param>
    /// <returns><see langword="True"/> if successful, <see langword="false"/> otherwise</returns>
    private bool ReadSigData(string FileName)
    {
        int nPoints = 0;
        bool result = false;
        string? strLine;

        try
        {
            using var fs = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sr = new StreamReader(fs, System.Text.Encoding.UTF8);

            strLine = sr.ReadLine();
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strSignalHeader01", _settings.AppCulture) ?? "Section 'SignalAnalysis data' is mis-formatted.");
            if (!strLine.Contains("SignalAnalysis data (", StringComparison.Ordinal))
                throw new FormatException(StringsRM.GetString("strSignalHeader01", _settings.AppCulture) ?? "Section 'SignalAnalysis data' is mis-formatted.");
            System.Globalization.CultureInfo fileCulture = new(strLine[(strLine.IndexOf("(") + 1)..^1]);

            strLine = sr.ReadLine();    // Number of series
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strSignalHeader02", _settings.AppCulture) ?? "Section 'Number of data series' is mis-formatted.");
            if (!strLine.Contains("Number of data series: ", StringComparison.Ordinal))
                throw new FormatException(StringsRM.GetString("strSignalHeader02", _settings.AppCulture) ?? "Section 'Number of data series' is mis-formatted.");
            if (!int.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out nSeries))
                throw new FormatException(StringsRM.GetString("strSignalHeader02", _settings.AppCulture) ?? "Section 'Number of data series' is mis-formatted.");
            if (nSeries == 0)
                throw new FormatException(StringsRM.GetString("strSignalHeader02", _settings.AppCulture) ?? "Section 'Number of data series' is mis-formatted.");

            strLine = sr.ReadLine();    // Number of data points
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strSignalHeader03", _settings.AppCulture) ?? "Section 'Number of data points' is mis-formatted.");
            if (!strLine.Contains("Number of data points: ", StringComparison.Ordinal))
                throw new FormatException(StringsRM.GetString("strSignalHeader03", _settings.AppCulture) ?? "Section 'Number of data points' is mis-formatted.");
            if (!int.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out nPoints))
                throw new FormatException(StringsRM.GetString("strSignalHeader03", _settings.AppCulture) ?? "Section 'Number of data points' is mis-formatted.");
            if (nPoints == 0)
                throw new FormatException(StringsRM.GetString("strSignalHeader03", _settings.AppCulture) ?? "Section 'Number of data points' is mis-formatted.");

            strLine = sr.ReadLine();    // Sampling frequency
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strSignalHeader04", _settings.AppCulture) ?? "Section 'Sampling frequency' is mis-formatted.");
            if (!strLine.Contains("Sampling frequency: ", StringComparison.Ordinal))
                throw new FormatException(StringsRM.GetString("strSignalHeader04", _settings.AppCulture) ?? "Section 'Sampling frequency' is mis-formatted.");
            if (!double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out nSampleFreq))
                throw new FormatException(StringsRM.GetString("strSignalHeader04", _settings.AppCulture) ?? "Section 'Sampling frequency' is mis-formatted.");
            if (nSampleFreq <= 0)
                throw new FormatException(StringsRM.GetString("strSignalHeader04", _settings.AppCulture) ?? "Section 'Sampling frequency' is mis-formatted.");

            strLine = sr.ReadLine();    // Empty line
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strSignalHeader05", _settings.AppCulture) ?? "Missing an empty line.");
            if (strLine != string.Empty)
                throw new FormatException(StringsRM.GetString("strSignalHeader05", _settings.AppCulture) ?? "Missing an empty line.");

            strLine = sr.ReadLine();    // Column header names
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strSignalHeader06", _settings.AppCulture) ?? "Missing column headers(series names).");
            seriesLabels = strLine.Split('\t');
            if (seriesLabels == Array.Empty<string>())
                throw new FormatException(StringsRM.GetString("strSignalHeader06", _settings.AppCulture) ?? "Missing column headers(series names).");

            result = InitializeDataArrays(sr, nPoints, fileCulture);
        }
        catch (System.Globalization.CultureNotFoundException ex)
        {
            result = false;
            using (new CenterWinDialog(this))
                MessageBox.Show(String.Format(StringsRM.GetString("strReadDataErrorCulture", _settings.AppCulture) ?? "The culture identifier string name is not valid.\n{0}", ex.Message),
                    StringsRM.GetString("strReadDataErrorCultureTitle" ?? "Culture name error", _settings.AppCulture),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
        }
        catch (FormatException ex)
        {
            result = false;
            using (new CenterWinDialog(this))
                MessageBox.Show(String.Format(StringsRM.GetString("strReadDataError", _settings.AppCulture) ?? "Unable to read data from file.\n{0}", ex.Message),
                    StringsRM.GetString("strReadDataErrorTitle" ?? "Error opening data", _settings.AppCulture),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            result = false;
            using (new CenterWinDialog(this))
            {
                MessageBox.Show(String.Format(StringsRM.GetString("strMsgBoxErrorOpenData", _settings.AppCulture) ?? "An unexpected error happened while opening file data.\nPlease try again later or contact the software engineer." + Environment.NewLine + "{0}", ex.Message),
                    StringsRM.GetString("strMsgBoxErrorOpenDataTitle", _settings.AppCulture) ?? "Error opening data",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        return result;
    }

    /// <summary>
    /// Readas data from a text-formatted file and stores it into _signalData.
    /// </summary>
    /// <param name="FileName">Path (including name) of the text file</param>
    /// <returns><see langword="True"/> if successful, <see langword="false"/> otherwise</returns>
    private bool ReadTextData(string FileName, Stats results)
    {
        double readValue;
        int nPoints = 0;
        bool result = false;
        string? strLine;

        try
        {
            using var fs = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sr = new StreamReader(fs, System.Text.Encoding.UTF8);

            strLine = sr.ReadLine();    // Signal analysis header
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strTextHeader01", _settings.AppCulture) ?? "Section 'ErgoLux data' is mis-formatted.");
            if (!strLine.Contains("SignalAnalysis data (", StringComparison.Ordinal))
                throw new FormatException(StringsRM.GetString("strTextHeader01", _settings.AppCulture) ?? "Section 'ErgoLux data' is mis-formatted.");
            System.Globalization.CultureInfo fileCulture = new(strLine[(strLine.IndexOf("(") + 1)..^1]);

            strLine = sr.ReadLine();    // Start time
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strTextHeader02", _settings.AppCulture) ?? "Section 'Start time' is mis-formatted.");
            if (!strLine.Contains("Start time: ", StringComparison.Ordinal))
                throw new FormatException(StringsRM.GetString("strTextHeader02", _settings.AppCulture) ?? "Section 'Start time' is mis-formatted.");
            string fullPattern = fileCulture.DateTimeFormat.FullDateTimePattern;
            fullPattern = System.Text.RegularExpressions.Regex.Replace(fullPattern, "(:ss|:s)", ClassSettings.GetMillisecondsFormat(fileCulture));
            if (!DateTime.TryParseExact(strLine[(strLine.IndexOf(":") + 2)..], fullPattern, fileCulture, System.Globalization.DateTimeStyles.None, out nStart))
                throw new FormatException(StringsRM.GetString("strTextHeader02", _settings.AppCulture) ?? "Section 'Start time' is mis-formatted.");

            strLine = sr.ReadLine();    // End time
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strTextHeader03", _settings.AppCulture) ?? "Section 'End time' is mis-formatted.");
            if (!strLine.Contains("End time: ", StringComparison.Ordinal))
                throw new FormatException(StringsRM.GetString("strTextHeader03", _settings.AppCulture) ?? "Section 'End time' is mis-formatted.");

            strLine = sr.ReadLine();    // Total measuring time
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strTextHeader04", _settings.AppCulture) ?? "Section 'Total measuring time' is mis-formatted.");
            if (!strLine.Contains("Total measuring time: ", StringComparison.Ordinal))
                throw new FormatException(StringsRM.GetString("strTextHeader04", _settings.AppCulture) ?? "Section 'Total measuring time' is mis-formatted.");

            //Section 'Number of data points' is mis-formatted.
            strLine = sr.ReadLine();    // Number of data points
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strTextHeader05", _settings.AppCulture) ?? "Section 'Number of data points' is mis-formatted.");
            if (!strLine.Contains("Number of data points: ", StringComparison.Ordinal))
                throw new FormatException(StringsRM.GetString("strTextHeader05", _settings.AppCulture) ?? "Section 'Number of data points' is mis-formatted.");
            if (!int.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out nPoints))
                throw new FormatException(StringsRM.GetString("strTextHeader05", _settings.AppCulture) ?? "Section 'Number of data points' is mis-formatted.");
            if (nPoints == 0)
                throw new FormatException(StringsRM.GetString("strTextHeader05", _settings.AppCulture) ?? "Section 'Number of data points' is mis-formatted.");

            strLine = sr.ReadLine();    // Sampling frequency
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strTextHeader06", _settings.AppCulture) ?? "Section 'Sampling frequency' is mis-formatted.");
            if (!strLine.Contains("Sampling frequency: ", StringComparison.Ordinal))
                throw new FormatException(StringsRM.GetString("strTextHeader06", _settings.AppCulture) ?? "Section 'Sampling frequency' is mis-formatted.");
            if (!double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out nSampleFreq))
                throw new FormatException(StringsRM.GetString("strTextHeader06", _settings.AppCulture) ?? "Section 'Sampling frequency' is mis-formatted.");
            if (nSampleFreq <= 0)
                throw new FormatException(StringsRM.GetString("strTextHeader06", _settings.AppCulture) ?? "Section 'Sampling frequency' is mis-formatted.");

            strLine = sr.ReadLine();    // Average illuminance
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strTextHeader07", _settings.AppCulture) ?? "Section 'Average illuminance' is mis-formatted.");
            if (!strLine.Contains("Average illuminance: ", StringComparison.Ordinal))
                throw new FormatException(StringsRM.GetString("strTextHeader07", _settings.AppCulture) ?? "Section 'Average illuminance' is mis-formatted.");
            if (!double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(StringsRM.GetString("strTextHeader07", _settings.AppCulture) ?? "Section 'Average illuminance' is mis-formatted.");
            results.Average = readValue;

            strLine = sr.ReadLine();    // Maximum illuminance
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strTextHeader08", _settings.AppCulture) ?? "Section 'Maximum illuminance' is mis-formatted.");
            if (!strLine.Contains("Maximum illuminance: ", StringComparison.Ordinal))
                throw new FormatException(StringsRM.GetString("strTextHeader08", _settings.AppCulture) ?? "Section 'Maximum illuminance' is mis-formatted.");
            if (!double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(StringsRM.GetString("strTextHeader08", _settings.AppCulture) ?? "Section 'Maximum illuminance' is mis-formatted.");
            results.Maximum = readValue;

            strLine = sr.ReadLine();    // Minimum illuminance
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strTextHeader09", _settings.AppCulture) ?? "Section 'Minimum illuminance' is mis-formatted.");
            if (!strLine.Contains("Minimum illuminance: ", StringComparison.Ordinal))
                throw new FormatException(StringsRM.GetString("strTextHeader09", _settings.AppCulture) ?? "Section 'Minimum illuminance' is mis-formatted.");
            if (!double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(StringsRM.GetString("strTextHeader09", _settings.AppCulture) ?? "Section 'Minimum illuminance' is mis-formatted.");
            results.Minimum = readValue;

            strLine = sr.ReadLine();    // Fractal dimension
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strTextHeader10", _settings.AppCulture) ?? "Section 'Fractal dimension' is mis-formatted.");
            if (!strLine.Contains("Fractal dimension: ", StringComparison.Ordinal))
                throw new FormatException(StringsRM.GetString("strTextHeader10", _settings.AppCulture) ?? "Section 'Fractal dimension' is mis-formatted.");
            if (!double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(StringsRM.GetString("strTextHeader10", _settings.AppCulture) ?? "Section 'Fractal dimension' is mis-formatted.");
            results.FractalDimension = readValue;

            strLine = sr.ReadLine();    // Fractal variance
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strTextHeader11", _settings.AppCulture) ?? "Section 'Fractal variance' is mis-formatted.");
            if (!strLine.Contains("Fractal variance: ", StringComparison.Ordinal))
                throw new FormatException(StringsRM.GetString("strTextHeader11", _settings.AppCulture) ?? "Section 'Fractal variance' is mis-formatted.");
            if (!double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(StringsRM.GetString("strTextHeader11", _settings.AppCulture) ?? "Section 'Fractal variance' is mis-formatted.");
            results.FractalVariance = readValue;

            strLine = sr.ReadLine();    // Approximate entropy
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strTextHeader12", _settings.AppCulture) ?? "Section 'Approximate entropy' is mis-formatted.");
            if (!strLine.Contains("Approximate entropy: ", StringComparison.Ordinal))
                throw new FormatException(StringsRM.GetString("strTextHeader12", _settings.AppCulture) ?? "Section 'Approximate entropy' is mis-formatted.");
            if (!double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(StringsRM.GetString("strTextHeader12", _settings.AppCulture) ?? "Section 'Approximate entropy' is mis-formatted.");
            results.ApproximateEntropy = readValue;

            strLine = sr.ReadLine();    // Sample entropy
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strTextHeader13", _settings.AppCulture) ?? "Section 'Sample entropy' is mis-formatted.");
            if (!strLine.Contains("Sample entropy: ", StringComparison.Ordinal))
                throw new FormatException(StringsRM.GetString("strTextHeader13", _settings.AppCulture) ?? "Section 'Sample entropy' is mis-formatted.");
            if (!double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(StringsRM.GetString("strTextHeader13", _settings.AppCulture) ?? "Section 'Sample entropy' is mis-formatted.");
            results.SampleEntropy = readValue;

            strLine = sr.ReadLine();    // Shannnon entropy
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strTextHeader14", _settings.AppCulture) ?? "Section 'Shannon entropy' is mis-formatted.");
            if (!strLine.Contains("Shannon entropy: ", StringComparison.Ordinal))
                throw new FormatException(StringsRM.GetString("strTextHeader14", _settings.AppCulture) ?? "Section 'Shannon entropy' is mis-formatted.");
            if (!double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(StringsRM.GetString("strTextHeader14", _settings.AppCulture) ?? "Section 'Shannon entropy' is mis-formatted.");
            results.ShannonEntropy = readValue;

            strLine = sr.ReadLine();    // Entropy bit
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strTextHeader15", _settings.AppCulture) ?? "Section 'Entropy bit' is mis-formatted.");
            if (!strLine.Contains("Entropy bit: ", StringComparison.Ordinal))
                throw new FormatException(StringsRM.GetString("strTextHeader15", _settings.AppCulture) ?? "Section 'Entropy bit' is mis-formatted.");
            if (!double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(StringsRM.GetString("strTextHeader15", _settings.AppCulture) ?? "Section 'Entropy bit' is mis-formatted.");
            results.EntropyBit = readValue;

            strLine = sr.ReadLine();    // Ideal entropy
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strTextHeader16", _settings.AppCulture) ?? "Section 'Ideal entropy' is mis-formatted.");
            if (!strLine.Contains("Ideal entropy: ", StringComparison.Ordinal))
                throw new FormatException(StringsRM.GetString("strTextHeader16", _settings.AppCulture) ?? "Section 'Ideal entropy' is mis-formatted.");
            if (!double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(StringsRM.GetString("strTextHeader16", _settings.AppCulture) ?? "Section 'Ideal entropy' is mis-formatted.");
            results.IdealEntropy = readValue;

            strLine = sr.ReadLine();    // Empty line
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strTextHeader17", _settings.AppCulture) ?? "Missing an empty line.");
            if (strLine != string.Empty)
                throw new FormatException(StringsRM.GetString("strTextHeader17", _settings.AppCulture) ?? "Missing an empty line.");

            strLine = sr.ReadLine();    // Column header names
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strTextHeader18", _settings.AppCulture) ?? "Missing column headers(series names).");
            seriesLabels = strLine.Split('\t');
            if (seriesLabels == Array.Empty<string>())
                throw new FormatException(StringsRM.GetString("strTextHeader18", _settings.AppCulture) ?? "Missing column headers(series names).");
            seriesLabels = seriesLabels[1..];
            nSeries = seriesLabels.Length;

            result = InitializeDataArrays(sr, nPoints, fileCulture, true);
        }
        catch (System.Globalization.CultureNotFoundException ex)
        {
            result = false;
            using (new CenterWinDialog(this))
                MessageBox.Show(String.Format(StringsRM.GetString("strReadDataErrorCulture", _settings.AppCulture) ?? "The culture identifier string name is not valid.\n{0}", ex.Message),
                    StringsRM.GetString("strReadDataErrorCultureTitle" ?? "Culture name error", _settings.AppCulture),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
        }
        catch (FormatException ex)
        {
            result = false;
            using (new CenterWinDialog(this))
                MessageBox.Show(String.Format(StringsRM.GetString("strReadDataError", _settings.AppCulture) ?? "Unable to read data from file.\n{0}", ex.Message),
                    StringsRM.GetString("strReadDataErrorTitle" ?? "Error opening data", _settings.AppCulture),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            result = false;
            using (new CenterWinDialog(this))
            {
                MessageBox.Show(String.Format(StringsRM.GetString("strMsgBoxErrorOpenData", _settings.AppCulture) ?? "An unexpected error happened while opening file data.\nPlease try again later or contact the software engineer." + Environment.NewLine + "{0}", ex.Message),
                    StringsRM.GetString("strMsgBoxErrorOpenDataTitle", _settings.AppCulture) ?? "Error opening data",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        return result;
    }

    /// <summary>
    /// Reads and parse the data into a numeric format.
    /// </summary>
    /// <param name="sr">This reader should be pointing to the beginning of the numeric data section</param>
    /// <param name="culture">Culture to parse the read data into numeric values</param>
    /// <param name="IsFirstColumDateTime"><see langword="True"/> if successfull</param>
    /// <returns><see langword="True"/> if the first data-column is a DateTime value and thus it will be ingnores, <see langword="false"/> otherwise</returns>
    private bool InitializeDataArrays(StreamReader sr, int points, System.Globalization.CultureInfo culture, bool IsFirstColumDateTime = false)
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
                    if (!double.TryParse(data[col], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, culture, out _signalData[col - (IsFirstColumDateTime ? 1 : 0)][row]))
                        throw new FormatException(data[col].ToString());
                }
                row++;
            }
        }
        catch (FormatException ex)
        {
            result = false;
            using (new CenterWinDialog(this))
                MessageBox.Show(String.Format(StringsRM.GetString("strReadDataErrorNumber", _settings.AppCulture) ?? "Invalid numeric value: {0}", ex.Message),
                    StringsRM.GetString("strReadDataErrorNumberTitle", _settings.AppCulture),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            result = false;
            using (new CenterWinDialog(this))
            {
                MessageBox.Show( String.Format(StringsRM.GetString("strMsgBoxInitArray", _settings.AppCulture) ?? "Unexpected error in 'InitializeDataArrays'." + Environment.NewLine + "{0}", ex.Message),
                    StringsRM.GetString("strMsgBoxInitArrayTitle", _settings.AppCulture) ?? "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        
        return result;
    }

}

