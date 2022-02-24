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
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader00", _settings.AppCulture) ?? "ErgoLux data"));
            if (!strLine.Contains($"{StringsRM.GetString("strFileHeader00", _settings.AppCulture) ?? "ErgoLux data"} (", StringComparison.Ordinal))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader00", _settings.AppCulture) ?? "ErgoLux data"));
            System.Globalization.CultureInfo fileCulture = new(strLine[(strLine.IndexOf("(") + 1)..^1]);

            strLine = sr.ReadLine();    // Start time
            if (strLine is null)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader02", _settings.AppCulture) ?? "Start time"));
            if (!strLine.Contains($"{StringsRM.GetString("strFileHeader02", _settings.AppCulture) ?? "Start time"}: ", StringComparison.Ordinal))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader02", _settings.AppCulture) ?? "Start time"));
            string fullPattern = fileCulture.DateTimeFormat.FullDateTimePattern;
            fullPattern = System.Text.RegularExpressions.Regex.Replace(fullPattern, "(:ss|:s)", ClassSettings.GetMillisecondsFormat(fileCulture));
            if (strLine == null || !DateTime.TryParseExact(strLine[(strLine.IndexOf(":") + 2)..], fullPattern, fileCulture, System.Globalization.DateTimeStyles.None, out nStart))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader02", _settings.AppCulture) ?? "Start time"));

            strLine = sr.ReadLine();    // End time
            if (strLine is null)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader03", _settings.AppCulture) ?? "End time"));
            if (!strLine.Contains($"{StringsRM.GetString("strFileHeader03", _settings.AppCulture) ?? "End time"}: ", StringComparison.Ordinal))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader03", _settings.AppCulture) ?? "End time"));

            strLine = sr.ReadLine();    // Total measuring time
            if (strLine is null)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader04", _settings.AppCulture) ?? "Total measuring time"));
            if (!strLine.Contains($"{StringsRM.GetString("strFileHeader04", _settings.AppCulture) ?? "Total measuring time"}: ", StringComparison.Ordinal))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader04", _settings.AppCulture) ?? "Total measuring time"));

            strLine = sr.ReadLine();    // Number of sensors
            if (strLine is null)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader18", _settings.AppCulture) ?? "Number of sensors"));
            if (!strLine.Contains($"{StringsRM.GetString("strFileHeader18", _settings.AppCulture) ?? "Number of sensors"}: ", StringComparison.Ordinal))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader18", _settings.AppCulture) ?? "Number of sensors"));
            if (!int.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out nSeries))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader18", _settings.AppCulture) ?? "Number of sensors"));
            if (nSeries == 0)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader18", _settings.AppCulture) ?? "Number of sensors"));
            nSeries += 6;

            strLine = sr.ReadLine();    // Number of data points
            if (strLine is null)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader05", _settings.AppCulture) ?? "Number of data points"));
            if (!strLine.Contains($"{StringsRM.GetString("strFileHeader05", _settings.AppCulture) ?? "Number of data points"}: ", StringComparison.Ordinal))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader05", _settings.AppCulture) ?? "Number of data points"));
            if (!int.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out nPoints))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader05", _settings.AppCulture) ?? "Number of data points"));
            if (nPoints == 0)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader05", _settings.AppCulture) ?? "Number of data points"));

            strLine = sr.ReadLine();    // Sampling frequency
            if (strLine is null)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader06", _settings.AppCulture) ?? "Sampling frequency"));
            if (!strLine.Contains($"{StringsRM.GetString("strFileHeader06", _settings.AppCulture) ?? "Sampling frequency"}: ", StringComparison.Ordinal))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader06", _settings.AppCulture) ?? "Sampling frequency"));
            if (!double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out nSampleFreq))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader06", _settings.AppCulture) ?? "Sampling frequency"));
            if (nSampleFreq <= 0)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader06", _settings.AppCulture) ?? "Sampling frequency"));

            strLine = sr.ReadLine();    // Empty line
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strFileHeader19", _settings.AppCulture) ?? "Missing an empty line.");
            if (strLine != string.Empty)
                throw new FormatException(StringsRM.GetString("strFileHeader19", _settings.AppCulture) ?? "Missing an empty line.");

            strLine = sr.ReadLine();    // Column header lines
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strFileHeader20", _settings.AppCulture) ?? "Missing column headers (series names).");
            seriesLabels = strLine.Split('\t');
            if (seriesLabels == Array.Empty<string>())
                throw new FormatException(StringsRM.GetString("strFileHeader20", _settings.AppCulture) ?? "Missing column headers (series names).");

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

            strLine = sr.ReadLine();    // SignalAnalysis data
            if (strLine is null)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader01", _settings.AppCulture) ?? "SignalAnalysis data"));
            if (!strLine.Contains($"{StringsRM.GetString("strFileHeader01", _settings.AppCulture) ?? "SignalAnalysis data"} (", StringComparison.Ordinal))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader01", _settings.AppCulture) ?? "SignalAnalysis data"));
            System.Globalization.CultureInfo fileCulture = new(strLine[(strLine.IndexOf("(") + 1)..^1]);

            strLine = sr.ReadLine();    // Number of series
            if (strLine is null)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader17", _settings.AppCulture) ?? "Number of data series"));
            if (!strLine.Contains($"{StringsRM.GetString("strFileHeader17", _settings.AppCulture) ?? "Number of data series"}: ", StringComparison.Ordinal))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader17", _settings.AppCulture) ?? "Number of data series"));
            if (!int.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out nSeries))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader17", _settings.AppCulture) ?? "Number of data series"));
            if (nSeries == 0)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader17", _settings.AppCulture) ?? "Number of data series"));

            strLine = sr.ReadLine();    // Number of data points
            if (strLine is null)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader05", _settings.AppCulture) ?? "Number of data points"));
            if (!strLine.Contains($"{StringsRM.GetString("strFileHeader05", _settings.AppCulture) ?? "Number of data points"}: ", StringComparison.Ordinal))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader05", _settings.AppCulture) ?? "Number of data points"));
            if (!int.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out nPoints))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader05", _settings.AppCulture) ?? "Number of data points"));
            if (nPoints == 0)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader05", _settings.AppCulture) ?? "Number of data points"));

            strLine = sr.ReadLine();    // Sampling frequency
            if (strLine is null)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader06", _settings.AppCulture) ?? "Sampling frequency"));
            if (!strLine.Contains($"{StringsRM.GetString("strFileHeader06", _settings.AppCulture) ?? "Sampling frequency"}: ", StringComparison.Ordinal))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader06", _settings.AppCulture) ?? "Sampling frequency"));
            if (!double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out nSampleFreq))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader06", _settings.AppCulture) ?? "Sampling frequency"));
            if (nSampleFreq <= 0)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader06", _settings.AppCulture) ?? "Sampling frequency"));

            strLine = sr.ReadLine();    // Empty line
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strFileHeader19", _settings.AppCulture) ?? "Missing an empty line.");
            if (strLine != string.Empty)
                throw new FormatException(StringsRM.GetString("strFileHeader19", _settings.AppCulture) ?? "Missing an empty line.");

            strLine = sr.ReadLine();    // Column header names
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strFileHeader20", _settings.AppCulture) ?? "Missing column headers (series names).");
            seriesLabels = strLine.Split('\t');
            if (seriesLabels == Array.Empty<string>())
                throw new FormatException(StringsRM.GetString("strFileHeader20", _settings.AppCulture) ?? "Missing column headers (series names).");

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
    /// <param name="results">Numeric results read from the file</param>
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

            strLine = sr.ReadLine();    // SignalAnalysis data
            if (strLine is null)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader01", _settings.AppCulture) ?? "SignalAnalysis data"));
            if (!strLine.Contains($"{StringsRM.GetString("strFileHeader01", _settings.AppCulture) ?? "SignalAnalysis data"} (", StringComparison.Ordinal))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader01", _settings.AppCulture) ?? "SignalAnalysis data"));
            System.Globalization.CultureInfo fileCulture = new(strLine[(strLine.IndexOf("(") + 1)..^1]);

            strLine = sr.ReadLine();    // Start time
            if (strLine is null)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader02", _settings.AppCulture) ?? "Start time"));
            if (!strLine.Contains($"{StringsRM.GetString("strFileHeader02", _settings.AppCulture) ?? "Start time"}: ", StringComparison.Ordinal))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader02", _settings.AppCulture) ?? "Start time"));
            string fullPattern = fileCulture.DateTimeFormat.FullDateTimePattern;
            fullPattern = System.Text.RegularExpressions.Regex.Replace(fullPattern, "(:ss|:s)", ClassSettings.GetMillisecondsFormat(fileCulture));
            if (!DateTime.TryParseExact(strLine[(strLine.IndexOf(":") + 2)..], fullPattern, fileCulture, System.Globalization.DateTimeStyles.None, out nStart))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader02", _settings.AppCulture) ?? "Start time"));

            strLine = sr.ReadLine();    // End time
            if (strLine is null)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader03", _settings.AppCulture) ?? "End time"));
            if (!strLine.Contains($"{StringsRM.GetString("strFileHeader03", _settings.AppCulture) ?? "End time"}: ", StringComparison.Ordinal))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader03", _settings.AppCulture) ?? "End time"));

            strLine = sr.ReadLine();    // Total measuring time
            if (strLine is null)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader04", _settings.AppCulture) ?? "Total measuring time"));
            if (!strLine.Contains($"{StringsRM.GetString("strFileHeader04", _settings.AppCulture) ?? "Total measuring time"}: ", StringComparison.Ordinal))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader04", _settings.AppCulture) ?? "Total measuring time"));

            strLine = sr.ReadLine();    // Number of series
            if (strLine is null)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader17", _settings.AppCulture) ?? "Number of series"));
            if (!strLine.Contains($"{StringsRM.GetString("strFileHeader17", _settings.AppCulture) ?? "Number of series"}: ", StringComparison.Ordinal))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader17", _settings.AppCulture) ?? "Number of series"));
            if (!int.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out nPoints))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader17", _settings.AppCulture) ?? "Number of series"));
            if (nPoints <= 0)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader17", _settings.AppCulture) ?? "Number of series"));

            strLine = sr.ReadLine();    // Number of data points
            if (strLine is null)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader05", _settings.AppCulture) ?? "Number of data points"));
            if (!strLine.Contains($"{StringsRM.GetString("strFileHeader05", _settings.AppCulture) ?? "Number of data points"}: ", StringComparison.Ordinal))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader05", _settings.AppCulture) ?? "Number of data points"));
            if (!int.TryParse(strLine[(strLine.IndexOf(":") + 1)..], out nPoints))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader05", _settings.AppCulture) ?? "Number of data points"));
            if (nPoints <= 0)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader05", _settings.AppCulture) ?? "Number of data points"));

            strLine = sr.ReadLine();    // Sampling frequency
            if (strLine is null)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader06", _settings.AppCulture) ?? "Sampling frequency"));
            if (!strLine.Contains($"{StringsRM.GetString("strFileHeader06", _settings.AppCulture) ?? "Sampling frequency"}: ", StringComparison.Ordinal))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader06", _settings.AppCulture) ?? "Sampling frequency"));
            if (!double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out nSampleFreq))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader06", _settings.AppCulture) ?? "Sampling frequency"));
            if (nSampleFreq <= 0)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader06", _settings.AppCulture) ?? "Sampling frequency"));

            strLine = sr.ReadLine();    // Average illuminance
            if (strLine is null)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader07", _settings.AppCulture) ?? "Average"));
            if (!strLine.Contains($"{StringsRM.GetString("strFileHeader07", _settings.AppCulture) ?? "Average"}: ", StringComparison.Ordinal))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader07", _settings.AppCulture) ?? "Average"));
            if (!double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader07", _settings.AppCulture) ?? "Average"));
            results.Average = readValue;

            strLine = sr.ReadLine();    // Maximum illuminance
            if (strLine is null)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader08", _settings.AppCulture) ?? "Maximum"));
            if (!strLine.Contains($"{StringsRM.GetString("strFileHeader08", _settings.AppCulture) ?? "Maximum"}: ", StringComparison.Ordinal))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader08", _settings.AppCulture) ?? "Maximum"));
            if (!double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader08", _settings.AppCulture) ?? "Maximum"));
            results.Maximum = readValue;

            strLine = sr.ReadLine();    // Minimum illuminance
            if (strLine is null)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader09", _settings.AppCulture) ?? "Minimum"));
            if (!strLine.Contains($"{StringsRM.GetString("strFileHeader09", _settings.AppCulture) ?? "Minimum"}: ", StringComparison.Ordinal))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader09", _settings.AppCulture) ?? "Minimum"));
            if (!double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader09", _settings.AppCulture) ?? "Minimum"));
            results.Minimum = readValue;

            strLine = sr.ReadLine();    // Fractal dimension
            if (strLine is null)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader10", _settings.AppCulture) ?? "Fractal dimension"));
            if (!strLine.Contains($"{StringsRM.GetString("strFileHeader10", _settings.AppCulture) ?? "Fractal dimension"}: ", StringComparison.Ordinal))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader10", _settings.AppCulture) ?? "Fractal dimension"));
            if (!double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader10", _settings.AppCulture) ?? "Fractal dimension"));
            results.FractalDimension = readValue;

            strLine = sr.ReadLine();    // Fractal variance
            if (strLine is null)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader11", _settings.AppCulture) ?? "Fractal variance"));
            if (!strLine.Contains($"{StringsRM.GetString("strFileHeader11", _settings.AppCulture) ?? "Fractal variance"}: ", StringComparison.Ordinal))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader11", _settings.AppCulture) ?? "Fractal variance"));
            if (!double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader11", _settings.AppCulture) ?? "Fractal variance"));
            results.FractalVariance = readValue;

            strLine = sr.ReadLine();    // Approximate entropy
            if (strLine is null)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader12", _settings.AppCulture) ?? "Approximate entropy"));
            if (!strLine.Contains($"{StringsRM.GetString("strFileHeader12", _settings.AppCulture) ?? "Approximate entropy"}: ", StringComparison.Ordinal))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader12", _settings.AppCulture) ?? "Approximate entropy"));
            if (!double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader12", _settings.AppCulture) ?? "Approximate entropy"));
            results.ApproximateEntropy = readValue;

            strLine = sr.ReadLine();    // Sample entropy
            if (strLine is null)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader13", _settings.AppCulture) ?? "Sample entropy"));
            if (!strLine.Contains($"{StringsRM.GetString("strFileHeader13", _settings.AppCulture) ?? "Sample entropy"}: ", StringComparison.Ordinal))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader13", _settings.AppCulture) ?? "Sample entropy"));
            if (!double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader13", _settings.AppCulture) ?? "Sample entropy"));
            results.SampleEntropy = readValue;

            strLine = sr.ReadLine();    // Shannnon entropy
            if (strLine is null)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader14", _settings.AppCulture) ?? "Shannon entropy"));
            if (!strLine.Contains($"{StringsRM.GetString("strFileHeader14", _settings.AppCulture) ?? "Shannon entropy"}: ", StringComparison.Ordinal))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader14", _settings.AppCulture) ?? "Shannon entropy"));
            if (!double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader14", _settings.AppCulture) ?? "Shannon entropy"));
            results.ShannonEntropy = readValue;

            strLine = sr.ReadLine();    // Entropy bit
            if (strLine is null)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader15", _settings.AppCulture) ?? "Entropy bit"));
            if (!strLine.Contains($"{StringsRM.GetString("strFileHeader15", _settings.AppCulture) ?? "Entropy bit"}: ", StringComparison.Ordinal))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader15", _settings.AppCulture) ?? "Entropy bit"));
            if (!double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader15", _settings.AppCulture) ?? "Entropy bit"));
            results.EntropyBit = readValue;

            strLine = sr.ReadLine();    // Ideal entropy
            if (strLine is null)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader16", _settings.AppCulture) ?? "Ideal entropy"));
            if (!strLine.Contains($"{StringsRM.GetString("strFileHeader16", _settings.AppCulture) ?? "Ideal entropy"}: ", StringComparison.Ordinal))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader16", _settings.AppCulture) ?? "Ideal entropy"));
            if (!double.TryParse(strLine[(strLine.IndexOf(":") + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader16", _settings.AppCulture) ?? "Ideal entropy"));
            results.IdealEntropy = readValue;

            strLine = sr.ReadLine();    // Empty line
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strTextHeader19", _settings.AppCulture) ?? "Missing an empty line.");
            if (strLine != string.Empty)
                throw new FormatException(StringsRM.GetString("strTextHeader19", _settings.AppCulture) ?? "Missing an empty line.");

            strLine = sr.ReadLine();    // Column header names
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strTextHeader20", _settings.AppCulture) ?? "Missing column headers (series names).");
            seriesLabels = strLine.Split('\t');
            if (seriesLabels == Array.Empty<string>())
                throw new FormatException(StringsRM.GetString("strTextHeader20", _settings.AppCulture) ?? "Missing column headers (series names).");
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
    /// Readas data from a binary-formatted file and stores it into _signalData.
    /// </summary>
    /// <param name="FileName">Path (including name) of the text file</param>
    /// <param name="results">Numeric results read from the file</param>
    /// <returns><see langword="True"/> if successful, <see langword="false"/> otherwise</returns>
    /// <exception cref="FormatException"></exception>
    private bool ReadBinData(string FileName, Stats results)
    {
        int nPoints;
        bool result = true;

        try
        {
            using var fs = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var br = new BinaryReader(fs, System.Text.Encoding.UTF8);

            string strLine = br.ReadString();   // SignalAnalysis data
            if (strLine is null)
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader01", _settings.AppCulture) ?? "SignalAnalysis data"));
            if (!strLine.Contains($"{StringsRM.GetString("strFileHeader01", _settings.AppCulture) ?? "SignalAnalysis data"} (", StringComparison.Ordinal))
                throw new FormatException(String.Format(StringsRM.GetString("strFileHeaderSection", _settings.AppCulture) ?? "Section '{0}' is mis-formatted.", StringsRM.GetString("strFileHeader01", _settings.AppCulture) ?? "SignalAnalysis data"));
            System.Globalization.CultureInfo fileCulture = new(strLine[(strLine.IndexOf("(") + 1)..^1]);

            nStart = br.ReadDateTime();     // start time
            br.ReadDateTime();              // end time
            int dummy = br.ReadInt32();     // days
            dummy = br.ReadInt32();         // hours
            dummy = br.ReadInt32();         // minutes
            dummy = br.ReadInt32();         // seconds
            dummy = br.ReadInt32();         // milliseconds
            nPoints = br.ReadInt32();       // number of series
            nPoints = br.ReadInt32();       // number of data points
            nSampleFreq = br.ReadDouble();  // sampling frequency
            results.Average = br.ReadDouble();
            results.Maximum = br.ReadDouble();
            results.Minimum = br.ReadDouble();
            results.FractalDimension = br.ReadDouble();
            results.FractalVariance = br.ReadDouble();
            results.ApproximateEntropy = br.ReadDouble();
            results.SampleEntropy = br.ReadDouble();
            results.ShannonEntropy = br.ReadDouble();
            results.EntropyBit = br.ReadDouble();
            results.IdealEntropy = br.ReadDouble();

            strLine = br.ReadString();      // Column header names
            if (strLine is null)
                throw new FormatException(StringsRM.GetString("strFileHeader20", _settings.AppCulture) ?? "Missing column headers (series names).");
            seriesLabels = strLine.Split('\t');
            if (seriesLabels == Array.Empty<string>())
                throw new FormatException(StringsRM.GetString("strFileHeader20", _settings.AppCulture) ?? "Missing column headers (series names).");
            seriesLabels = seriesLabels[1..];
            nSeries = seriesLabels.Length;

            // Read data into array
            _settings.IndexStart = 0;
            _settings.IndexEnd = nPoints - 1;

            // Initialize data arrays
            _signalData = new double[nSeries][];
            for (int i = 0; i < nSeries; i++)
                _signalData[i] = new double[nPoints];

            // Read data into _signalData
            for (int row = 0; row < nSeries; row++)
            {
                for (int col = 0; col < _signalData[row].Length; col++)
                {
                    br.ReadDateTime();
                    _signalData[row][col] = br.ReadDouble();
                }
            }

        }
        catch (EndOfStreamException)
        {

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

            // Read data into _signalData
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

