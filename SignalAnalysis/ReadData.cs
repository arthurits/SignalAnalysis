namespace SignalAnalysis;

partial class FrmMain
{
    /// <summary>
    /// Reads data from an elux-formatted file and stores it into a <see cref="SignalData"/> parameter.
    /// </summary>
    /// <param name="fileName">Path (including name) of the elux file</param>
    /// <param name="signal"><see cref="SignalData"/> variable to store data read from the elux file</param>
    /// <returns><see langword="True"/> if successful, <see langword="false"/> otherwise</returns>
    private bool ReadELuxData(string fileName, SignalData signal)
    {
        DateTime start;
        DateTime end;
        int points, series;
        bool result = false;
        double sampleFreq;
        string[] seriesLabels;
        string? strLine;

        try
        {
            using var fs = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sr = new StreamReader(fs, System.Text.Encoding.UTF8);
            
            strLine = sr.ReadLine();    // ErgoLux data
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader00));
            System.Globalization.CultureInfo fileCulture = new(strLine[(strLine.IndexOf('(') + 1)..^1]);
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader00", fileCulture) ?? "ErgoLux data"} (", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader00));

            strLine = sr.ReadLine();    // Start time
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader02));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader02", fileCulture) ?? "Start time"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader02));
            string fullPattern = fileCulture.DateTimeFormat.FullDateTimePattern;
            fullPattern = System.Text.RegularExpressions.Regex.Replace(fullPattern, "(:ss|:s)", AppSettings.GetMillisecondsFormat(fileCulture));
            if (strLine == null || !DateTime.TryParseExact(strLine[(strLine.IndexOf(':') + 2)..], fullPattern, fileCulture, System.Globalization.DateTimeStyles.None, out start))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader02));

            strLine = sr.ReadLine();    // End time
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader03));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader03", fileCulture) ?? "End time"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader03));
            if (strLine == null || !DateTime.TryParseExact(strLine[(strLine.IndexOf(':') + 2)..], fullPattern, fileCulture, System.Globalization.DateTimeStyles.None, out end))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader03));

            strLine = sr.ReadLine();    // Total measuring time
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader04));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader04", fileCulture) ?? "Total measuring time"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader04));

            strLine = sr.ReadLine();    // Number of sensors
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader18));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader18", fileCulture) ?? "Number of sensors"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader18));
            if (!int.TryParse(strLine[(strLine.IndexOf(':') + 1)..], out series))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader18));
            if (series <= 0)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader18));
            series += 6;

            strLine = sr.ReadLine();    // Number of data points
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader05));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader05", fileCulture) ?? "Number of data points"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader05));
            if (!int.TryParse(strLine[(strLine.IndexOf(':') + 1)..], out points))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader05));
            if (points <= 0)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader05));

            strLine = sr.ReadLine();    // Sampling frequency
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader06));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader06", fileCulture) ?? "Sampling frequency"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader06));
            if (!double.TryParse(strLine[(strLine.IndexOf(':') + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out sampleFreq))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader06));
            if (sampleFreq <= 0)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader06));

            strLine = sr.ReadLine();    // Empty line
            if (strLine is null)
                throw new FormatException(StringResources.FileHeader19);
            if (strLine != string.Empty)
                throw new FormatException(StringResources.FileHeader19);

            strLine = sr.ReadLine();    // Column header lines
            if (strLine is null)
                throw new FormatException(StringResources.FileHeader20);
            seriesLabels = strLine.Split('\t');
            if (seriesLabels == Array.Empty<string>())
                throw new FormatException(StringResources.FileHeader20);

            result = InitializeDataArrays(sr, ref signal.Data, series, points, fileCulture);

            // Store information regarding the signal
            signal.StartTime = start;
            signal.EndTime = end;
            signal.MeasuringTime = end - start;
            signal.SeriesNumber = series;
            signal.SeriesPoints = points;
            signal.SampleFrequency = sampleFreq;
            signal.SeriesLabels = seriesLabels;
            signal.IndexStart = 0;
            signal.IndexEnd = signal.SeriesPoints - 1;
            //signal.SeriesLabels = new string [seriesLabels.Length];
            //Array.Copy(seriesLabels, signal.SeriesLabels, seriesLabels.Length);

        }
        catch (System.Globalization.CultureNotFoundException ex)
        {
            result = false;
            using (new CenterWinDialog(this))
                MessageBox.Show(this, 
                    string.Format(_settings.AppCulture, StringResources.ReadDataErrorCulture, ex.Message),
                    StringResources.ReadDataErrorCultureTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
        }
        catch (FormatException ex)
        {
            result = false;
            using (new CenterWinDialog(this))
                MessageBox.Show(this,
                    string.Format(_settings.AppCulture, StringResources.ReadDataError, ex.Message),
                    StringResources.ReadDataErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            result = false;
            using (new CenterWinDialog(this))
            {
                MessageBox.Show(this,
                    string.Format(_settings.AppCulture, StringResources.MsgBoxErrorOpenData, ex.Message),
                    StringResources.MsgBoxErrorOpenDataTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        return result;
    }

    /// <summary>
    /// Readas data from a signal-formatted file and stores it into a <see cref="SignalData"/> parameter.
    /// </summary>
    /// <param name="fileName">Path (including name) of the sig file</param>
    /// <param name="signal"><see cref="SignalData"/> variable to store data read from the elux file</param>
    /// <returns><see langword="True"/> if successful, <see langword="false"/> otherwise</returns>
    private bool ReadSigData(string fileName, SignalData signal)
    {
        int points = 0, series = 0;
        bool result = false;
        double sampleFreq;
        string[] seriesLabels;
        string? strLine;

        try
        {
            using var fs = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sr = new StreamReader(fs, System.Text.Encoding.UTF8);

            strLine = sr.ReadLine();    // SignalAnalysis data
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader01));
            System.Globalization.CultureInfo fileCulture = new(strLine[(strLine.IndexOf('(') + 1)..^1]);
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader01", fileCulture) ?? "SignalAnalysis data"} (", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader01));

            strLine = sr.ReadLine();    // Number of series
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader17));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader17", fileCulture) ?? "Number of series"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader17));
            if (!int.TryParse(strLine[(strLine.IndexOf(':') + 1)..], out series))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader17));
            if (series <= 0)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader17));

            strLine = sr.ReadLine();    // Number of data points
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader05));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader05", fileCulture) ?? "Number of data points"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader05));
            if (!int.TryParse(strLine[(strLine.IndexOf(':') + 1)..], out points))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader05));
            if (points <= 0)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader05));

            strLine = sr.ReadLine();    // Sampling frequency
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader06));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader06", fileCulture) ?? "Sampling frequency"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader06));
            if (!double.TryParse(strLine[(strLine.IndexOf(':') + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out sampleFreq))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader06));
            if (sampleFreq <= 0)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader06));

            //strLine = sr.ReadLine();    // Differentiation
            //if (strLine is null)
            //    throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader29));
            //if (!strLine.Contains($"{StringResources.GetString("strFileHeader29", fileCulture) ?? "Numerical differentiation"}: ", StringComparison.Ordinal))
            //    throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader29));

            //if (strLine[(strLine.IndexOf(':') + 2)..] != "-")
            //{
            //    string[] str = StringResources.GetString("strDifferentiationAlgorithms", fileCulture).Split(", ");
            //    _settings.DerivativeAlgorithm = (DerivativeMethod)Array.IndexOf(str, strLine[(strLine.IndexOf(':') + 2)..]);
            //}

            strLine = sr.ReadLine();    // Empty line
            if (strLine is null)
                throw new FormatException(StringResources.FileHeader19);
            if (strLine != string.Empty)
                throw new FormatException(StringResources.FileHeader19);

            strLine = sr.ReadLine();    // Column header names
            if (strLine is null)
                throw new FormatException(StringResources.FileHeader20);
            seriesLabels = strLine.Split('\t');
            if (seriesLabels == Array.Empty<string>())
                throw new FormatException(StringResources.FileHeader20);

            result = InitializeDataArrays(sr, ref signal.Data, series, points, fileCulture);

            // Store information regarding the signal
            signal.SeriesNumber = series;
            signal.SeriesPoints = points;
            signal.SampleFrequency = sampleFreq;
            signal.SeriesLabels = seriesLabels;
            signal.IndexStart = 0;
            signal.IndexEnd = signal.SeriesPoints - 1;
            signal.MeasuringTime = new(signal.StartTime.AddSeconds((signal.SeriesPoints - 1)/ signal.SampleFrequency).Ticks);
        }
        catch (System.Globalization.CultureNotFoundException ex)
        {
            result = false;
            using (new CenterWinDialog(this))
                MessageBox.Show(this,
                    string.Format(_settings.AppCulture, StringResources.ReadDataErrorCulture, ex.Message),
                    StringResources.ReadDataErrorCultureTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
        }
        catch (FormatException ex)
        {
            result = false;
            using (new CenterWinDialog(this))
                MessageBox.Show(this,
                    string.Format(_settings.AppCulture, StringResources.ReadDataError, ex.Message),
                    StringResources.ReadDataErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            result = false;
            using (new CenterWinDialog(this))
            {
                MessageBox.Show(this,
                    string.Format(_settings.AppCulture, StringResources.MsgBoxErrorOpenData, ex.Message),
                    StringResources.MsgBoxErrorOpenDataTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        return result;
    }

    /// <summary>
    /// Reads data from a text-formatted file and stores it into a <see cref="SignalData"/> parameter.
    /// </summary>
    /// <param name="fileName">Path (including name) of the text file</param>
    /// <param name="signal"><see cref="SignalData"/> variable to store data read from the elux file</param>
    /// <param name="results"><see cref="SignalStats"/> variable to store the numerical results (entropies, dimensions, stats) read from the elux file</param>
    /// <returns><see langword="True"/> if successful, <see langword="false"/> otherwise</returns>
    private bool ReadTextData(string fileName, SignalData signal, SignalStats? results)
    {

        DateTime start;
        DateTime end;
        int points = 0, series = 0;
        uint factorM = 0;
        bool result = false;
        double sampleFreq;
        double readValue;
        string[] seriesLabels;
        string? strLine;

        if (results is null) results = new();

        try
        {
            using var fs = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sr = new StreamReader(fs, System.Text.Encoding.UTF8);

            strLine = sr.ReadLine();    // SignalAnalysis data
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader01));
            System.Globalization.CultureInfo fileCulture = new(strLine[(strLine.IndexOf('(') + 1)..^1]);
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader01", fileCulture) ?? "SignalAnalysis data"} (", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader01));

            strLine = sr.ReadLine();    // Start time
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader02));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader02", fileCulture) ?? "Start time"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader02));
            string fullPattern = fileCulture.DateTimeFormat.FullDateTimePattern;
            fullPattern = System.Text.RegularExpressions.Regex.Replace(fullPattern, "(:ss|:s)", AppSettings.GetMillisecondsFormat(fileCulture));
            if (!DateTime.TryParseExact(strLine[(strLine.IndexOf(':') + 2)..], fullPattern, fileCulture, System.Globalization.DateTimeStyles.None, out start))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader02));

            strLine = sr.ReadLine();    // End time
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader03));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader03", fileCulture) ?? "End time"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader03));
            if (!DateTime.TryParseExact(strLine[(strLine.IndexOf(':') + 2)..], fullPattern, fileCulture, System.Globalization.DateTimeStyles.None, out end))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader02));

            strLine = sr.ReadLine();    // Total measuring time
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader04));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader04", fileCulture) ?? "Total measuring time"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader04));

            strLine = sr.ReadLine();    // Number of series
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader17));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader17", fileCulture) ?? "Number of series"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader17));
            if (!int.TryParse(strLine[(strLine.IndexOf(':') + 1)..], out series))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader17));
            if (series <= 0)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader17));

            strLine = sr.ReadLine();    // Number of data points
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader05));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader05", fileCulture) ?? "Number of data points"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader05));
            if (!int.TryParse(strLine[(strLine.IndexOf(':') + 1)..], out points))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader05));
            if (points <= 0)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader05));

            strLine = sr.ReadLine();    // Sampling frequency
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader06));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader06", fileCulture) ?? "Sampling frequency"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader06));
            if (!double.TryParse(strLine[(strLine.IndexOf(':') + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out sampleFreq))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader06));
            if (sampleFreq <= 0)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader06));

            strLine = sr.ReadLine();    // Average illuminance
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader07));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader07", fileCulture) ?? "Average"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader07));
            if (!double.TryParse(strLine[(strLine.IndexOf(':') + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader07));
            results.Average = readValue;

            strLine = sr.ReadLine();    // Variance illuminance
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader32));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader32", fileCulture) ?? "Variance"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader32));
            if (!double.TryParse(strLine[(strLine.IndexOf(':') + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader32));
            results.Variance = readValue;

            strLine = sr.ReadLine();    // Maximum illuminance
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader08));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader08", fileCulture) ?? "Maximum"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader08));
            if (!double.TryParse(strLine[(strLine.IndexOf(':') + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader08));
            results.Maximum = readValue;

            strLine = sr.ReadLine();    // Minimum illuminance
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader09));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader09", fileCulture) ?? "Minimum"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader09));
            if (!double.TryParse(strLine[(strLine.IndexOf(':') + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader09));
            results.Minimum = readValue;

            strLine = sr.ReadLine();    // Box plot minimum excluding outliers
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader33));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader33", fileCulture) ?? "Box plot lower limit"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader33));
            if (!double.TryParse(strLine[(strLine.IndexOf(':') + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader33));
            results.BoxplotMin = readValue;

            strLine = sr.ReadLine();    // Box plot Q1 quartile
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader35));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader35", fileCulture) ?? "Quartile 1 (25%)"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader35));
            if (!double.TryParse(strLine[(strLine.IndexOf(':') + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader35));
            results.BoxplotQ1 = readValue;

            strLine = sr.ReadLine();    // Box plot Q2 quartile
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader36));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader36", fileCulture) ?? "Quartile 2 (50%)"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader36));
            if (!double.TryParse(strLine[(strLine.IndexOf(':') + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader36));
            results.BoxplotQ2 = readValue;

            strLine = sr.ReadLine();    // Box plot Q3 quartile
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader37));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader37", fileCulture) ?? "Quartile 3 (75%)"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader37));
            if (!double.TryParse(strLine[(strLine.IndexOf(':') + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader37));
            results.BoxplotQ3 = readValue;

            strLine = sr.ReadLine();    // Box plot maximum excluding outliers
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader34));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader34", fileCulture) ?? "Box plot upper limit"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader34));
            if (!double.TryParse(strLine[(strLine.IndexOf(':') + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader34));
            results.BoxplotMax = readValue;

            strLine = sr.ReadLine();    // Fractal dimension
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader10));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader10", fileCulture) ?? "Fractal dimension"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader10));
            if (!double.TryParse(strLine[(strLine.IndexOf(':') + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader10));
            results.FractalDimension = readValue;

            strLine = sr.ReadLine();    // Fractal variance
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader11));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader11", fileCulture) ?? "Fractal variance"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader11));
            if (!double.TryParse(strLine[(strLine.IndexOf(':') + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader11));
            results.FractalVariance = readValue;

            strLine = sr.ReadLine();    // Shannnon entropy
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader14));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader14", fileCulture) ?? "Shannon entropy"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader14));
            if (!double.TryParse(strLine[(strLine.IndexOf(':') + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader14));
            results.ShannonEntropy = readValue;

            strLine = sr.ReadLine();    // Entropy bit
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader15));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader15", fileCulture) ?? "Entropy bit"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader15));
            if (!double.TryParse(strLine[(strLine.IndexOf(':') + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader15));
            results.EntropyBit = readValue;

            strLine = sr.ReadLine();    // Ideal entropy
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader16));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader16", fileCulture) ?? "Ideal entropy"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader16));
            if (!double.TryParse(strLine[(strLine.IndexOf(':') + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader16));
            results.IdealEntropy = readValue;

            strLine = sr.ReadLine();    // Ratio Shannon/Ideal entropy
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader38));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader38", fileCulture) ?? "Shannon / Ideal"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader38));
            if (!double.TryParse(strLine[(strLine.IndexOf(':') + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader38));
            results.ShannonIdeal = readValue;

            strLine = sr.ReadLine();    // Entropy algorithm
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader39));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader39", fileCulture) ?? "Entropy algorithm"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader39));
            if (strLine[(strLine.IndexOf(':') + 2)..] != "-")
            {
                string[] str = StringResources.GetString("strEntropyAlgorithms", fileCulture).Split(", ");
                _settings.EntropyAlgorithm = (EntropyMethod)Array.IndexOf(str, strLine[(strLine.IndexOf(':') + 2)..]);
            }

            strLine = sr.ReadLine();    // Entropy factor R
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader40));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader40", fileCulture) ?? "Entropy tolerance factor"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader40));
            if (!double.TryParse(strLine[(strLine.IndexOf(':') + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader40));
            _settings.EntropyFactorR = readValue;

            strLine = sr.ReadLine();    // Entropy factor M
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader41));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader41", fileCulture) ?? "Entropy embedding dimension"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader41));
            if (!uint.TryParse(strLine[(strLine.IndexOf(':') + 1)..], out factorM))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader41));
            _settings.EntropyFactorM = factorM;

            strLine = sr.ReadLine();    // Approximate entropy
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader12));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader12", fileCulture) ?? "Approximate entropy"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader12));
            if (!double.TryParse(strLine[(strLine.IndexOf(':') + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader12));
            results.ApproximateEntropy = readValue;

            strLine = sr.ReadLine();    // Sample entropy
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader13));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader13", fileCulture) ?? "Sample entropy"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader13));
            if (!double.TryParse(strLine[(strLine.IndexOf(':') + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader13));
            results.SampleEntropy = readValue;

            strLine = sr.ReadLine();    // Differentiation algorithm
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader29));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader29", fileCulture) ?? "Differentiation algorithm"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader29));
            if (strLine[(strLine.IndexOf(':') + 2)..] != "-")
            {
                string[] str = StringResources.GetString("strDifferentiationAlgorithms", fileCulture).Split(", ");
                _settings.DerivativeAlgorithm = (DerivativeMethod)Array.IndexOf(str, strLine[(strLine.IndexOf(':') + 2)..]);
            }

            strLine = sr.ReadLine();    // Integration algorithm
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader30));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader30", fileCulture) ?? "Integration algorithm"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader30));
            if (strLine[(strLine.IndexOf(':') + 2)..] != "-")
            {
                string[] str = StringResources.GetString("strIntegrationAlgorithms", fileCulture).Split(", ");
                _settings.IntegrationAlgorithm = (IntegrationMethod)Array.IndexOf(str, strLine[(strLine.IndexOf(':') + 2)..]);
            }

            strLine = sr.ReadLine();    // Integral value
            if (strLine is null)
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader31));
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader31", fileCulture) ?? "Integral value"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader31));
            if (!double.TryParse(strLine[(strLine.IndexOf(':') + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out readValue))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader16));
            results.Integral = readValue;

            strLine = sr.ReadLine();    // Empty line
            if (strLine is null)
                throw new FormatException(StringResources.FileHeader19);
            if (strLine != string.Empty)
                throw new FormatException(StringResources.FileHeader19);

            strLine = sr.ReadLine();    // Column header names
            if (strLine is null)
                throw new FormatException(StringResources.FileHeader20);
            seriesLabels = strLine.Split('\t');
            if (seriesLabels == Array.Empty<string>())
                throw new FormatException(StringResources.FileHeader20);
            series = seriesLabels.Length;

            result = InitializeDataArrays(sr, ref signal.Data, series, points, fileCulture, true);

            // Store information regarding the signal
            signal.StartTime = start;
            signal.EndTime = end;
            signal.MeasuringTime = end - start;
            signal.SeriesNumber = series;
            signal.SeriesPoints = points;
            signal.SampleFrequency = sampleFreq;
            signal.IndexStart = 0;
            signal.IndexEnd = signal.SeriesPoints - 1;
            seriesLabels = seriesLabels[1..];
            signal.SeriesLabels = seriesLabels;

        }
        catch (System.Globalization.CultureNotFoundException ex)
        {
            result = false;
            using (new CenterWinDialog(this))
                MessageBox.Show(string.Format(StringResources.ReadDataErrorCulture, ex.Message),
                    StringResources.ReadDataErrorCultureTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
        }
        catch (FormatException ex)
        {
            result = false;
            using (new CenterWinDialog(this))
                MessageBox.Show(this,
                    string.Format(_settings.AppCulture, StringResources.ReadDataError, ex.Message),
                    StringResources.ReadDataErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            result = false;
            using (new CenterWinDialog(this))
            {
                MessageBox.Show(this,
                    string.Format(_settings.AppCulture, StringResources.MsgBoxErrorOpenData, ex.Message),
                    StringResources.MsgBoxErrorOpenDataTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        return result;
    }

    /// <summary>
    /// Reads data from a binary-formatted file and stores it into a <see cref="SignalData"/> parameter.
    /// </summary>
    /// <param name="fileName">Path (including name) of the text file</param>
    /// <param name="signal"><see cref="SignalData"/> variable to store data read from the elux file</param>
    /// <param name="results"><see cref="SignalStats"/> variable to store the numerical results (entropies, dimensions, stats) read from the elux file</param>
    /// <returns><see langword="True"/> if successful, <see langword="false"/> otherwise</returns>
    /// <exception cref="FormatException"></exception>
    private bool ReadBinData(string fileName, SignalData signal, SignalStats? results)
    {
        DateTime start;
        DateTime end;
        int points = 0, series = 0, dummy = 0;
        bool result = true;
        double sampleFreq;
        string[] seriesLabels;

        if (results is null) results = new();

        try
        {
            using var fs = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var br = new BinaryReader(fs, System.Text.Encoding.UTF8);

            // SignalAnalysis data
            string strLine = br.ReadString() ?? throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader01));
            //if (!strLine.Contains($"{StringResources.FileHeader01} (", StringComparison.Ordinal))
            //    throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader01));
            System.Globalization.CultureInfo fileCulture = new(strLine[(strLine.IndexOf('(') + 1)..^1]);
            if (!strLine.Contains($"{StringResources.GetString("strFileHeader01", fileCulture) ?? "SignalAnalysis data"} (", StringComparison.Ordinal))
                throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader01));

            start = br.ReadDateTime();      // start time
            end = br.ReadDateTime();        // end time
            dummy = br.ReadInt32();         // days
            dummy = br.ReadInt32();         // hours
            dummy = br.ReadInt32();         // minutes
            dummy = br.ReadInt32();         // seconds
            dummy = br.ReadInt32();         // milliseconds
            series = br.ReadInt32();        // number of series
            points = br.ReadInt32();        // number of data points
            sampleFreq = br.ReadDouble();   // sampling frequency
            results.Average = br.ReadDouble();
            results.Variance = br.ReadDouble();
            results.Maximum = br.ReadDouble();
            results.Minimum = br.ReadDouble();
            results.BoxplotMin = br.ReadDouble();
            results.BoxplotQ1 = br.ReadDouble();
            results.BoxplotQ2 = br.ReadDouble();
            results.BoxplotQ3 = br.ReadDouble();
            results.BoxplotMax = br.ReadDouble();
            results.FractalDimension = br.ReadDouble();
            results.FractalVariance = br.ReadDouble();
            results.ShannonEntropy = br.ReadDouble();
            results.EntropyBit = br.ReadDouble();
            results.IdealEntropy = br.ReadDouble();
            results.ShannonIdeal = br.ReadDouble();
            _settings.EntropyAlgorithm = (EntropyMethod)br.ReadByte();
            _settings.EntropyFactorM = br.ReadUInt32();
            _settings.EntropyFactorR = br.ReadDouble();
            results.ApproximateEntropy = br.ReadDouble();
            results.SampleEntropy = br.ReadDouble();
            _settings.IntegrationAlgorithm = (IntegrationMethod)br.ReadByte();
            results.Integral = br.ReadDouble();

            strLine = br.ReadString();      // Column header names
            if (strLine is null)
                throw new FormatException(StringResources.FileHeader20);
            seriesLabels = strLine.Split('\t');
            if (seriesLabels == Array.Empty<string>())
                throw new FormatException(StringResources.FileHeader20);
            
            seriesLabels = seriesLabels[1..];
            series = seriesLabels.Length;

            signal.StartTime = start;
            signal.EndTime = end;
            signal.MeasuringTime = end - start;
            signal.SeriesPoints = points;
            signal.SampleFrequency = sampleFreq;
            signal.SeriesLabels = seriesLabels;
            signal.IndexStart = 0;
            signal.IndexEnd = signal.SeriesPoints - 1;
            //signal.SeriesLabels = new string[seriesLabels.Length];
            //Array.Copy(seriesLabels, signal.SeriesLabels, seriesLabels.Length);
            signal.SeriesNumber = signal.SeriesLabels.Length;

            // Initialize data arrays
            signal.Data = new double[series][];
            for (int i = 0; i < series; i++)
                signal.Data[i] = new double[points];

            // Read data into _signalData
            for (int col = 0; col < points; col++)
            {
                br.ReadDateTime();

                for (int row = 0; row < series; row++)
                    signal.Data[row][col] = br.ReadDouble();
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
                MessageBox.Show(this,
                    string.Format(_settings.AppCulture, StringResources.MsgBoxErrorOpenData, ex.Message),
                    StringResources.MsgBoxErrorOpenDataTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        return result;
    }

    /// <summary>
    /// Reads data from a json file and stores it into a <see cref="SignalData"/> parameter.
    /// </summary>
    /// <param name="fileName">Path (including name) of the json file</param>
    /// <param name="signal"><see cref="SignalData"/> variable to store data read from the elux file</param>
    /// <param name="results"><see cref="SignalStats"/> variable to store the numerical results (entropies, dimensions, stats) read from the elux file</param>
    /// <returns><see langword="True"/> if successful, <see langword="false"/> otherwise</returns>
    private bool ReadJsonData(string fileName, SignalData signal, SignalStats? results)
    {
        int points = 0;
        bool result = true;

        return result;
    }

    /// <summary>
    /// Default not implemented file-read function showing an error message
    /// </summary>
    /// <param name="FileName">Path (including name and extension) of the text file</param>
    /// <returns><see langword="True"/> if successful, <see langword="false"/> otherwise</returns>
    private bool ReadNotImplemented(string FileName)
    {
        bool result = false;

        using (new CenterWinDialog(this))
            MessageBox.Show(this,
                string.Format(StringResources.ReadNotimplementedError, Path.GetExtension(FileName).ToUpper()),
                StringResources.ReadNotimplementedErrorTitle,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

        return result;
    }

    /// <summary>
    /// Reads and parses the <see cref="StreamReader"/> data into an array.
    /// </summary>
    /// <param name="sr">This reader should be pointing to the beginning of the numeric data section</param>
    /// <param name="dataPoints">Array where the read data will be stored</param>
    /// <param name="series">Number of series of the array (first dimension)</param>
    /// <param name="points">Number of points in each serie (second dimension)</param>
    /// <param name="culture">Culture to parse the read data into numeric values</param>
    /// <param name="isFirstColumDateTime"><see langword="True"/> if the first element in the <see cref="StreamReader"/> row is a <see cref="DateTime"/> value and so it will be ignored</param>
    /// <returns><see langword="True"/> if the first data-column is a DateTime value and thus it will be ingnores, <see langword="false"/> otherwise</returns>
    private bool InitializeDataArrays(StreamReader sr, ref double[][] dataPoints, int series, int points, System.Globalization.CultureInfo culture, bool isFirstColumDateTime = false)
    {
        bool result = true;
        string? strLine;

        try {
            // Initialize data arrays
            dataPoints = new double[series][];
            for (int i = 0; i < series; i++)
                dataPoints[i] = new double[points];

            // Read data into _signalData
            for (int i = 0; i < dataPoints.Length; i++)
            {
                dataPoints[i] = new double[points];
            }
            string[] data;
            int col = 0, row = 0;
            while ((strLine = sr.ReadLine()) != null)
            {
                data = strLine.Split("\t");
                for (col = isFirstColumDateTime ? 1 : 0; col < data.Length; col++)
                {
                    if (!double.TryParse(data[col], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, culture, out dataPoints[col - (isFirstColumDateTime ? 1 : 0)][row]))
                        throw new FormatException(data[col].ToString());
                }
                row++;
            }
        }
        catch (FormatException ex)
        {
            result = false;
            using (new CenterWinDialog(this))
                MessageBox.Show(this,
                    string.Format(_settings.AppCulture, StringResources.ReadDataErrorNumber, ex.Message),
                    StringResources.ReadDataErrorNumberTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            result = false;
            using (new CenterWinDialog(this))
            {
                MessageBox.Show(this,
                    string.Format(_settings.AppCulture, StringResources.MsgBoxInitArray, ex.Message),
                    StringResources.MsgBoxInitArrayTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        
        return result;
    }

}