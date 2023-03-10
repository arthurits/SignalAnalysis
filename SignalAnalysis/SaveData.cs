using Microsoft.VisualBasic.ApplicationServices;

namespace SignalAnalysis;

partial class FrmMain
{
    /// <summary>
    /// Saves data into a text-formatted file.
    /// </summary>
    /// <param name="fileName">Path (including name) of the text file</param>
    /// <param name="signal">Array of values to be saved</param>
    /// <param name="ArrIndexInit">Offset index of _signalData (used to compute the time data field)</param>
    /// <param name="SeriesName">Name of the serie data to be saved</param>
    /// <returns><see langword="True"/> if successful, <see langword="false"/> otherwise</returns>
    private bool SaveTextData(string fileName, double[] signal, int ArrIndexInit, string? SeriesName)
    {
        bool result = false;
        int numSeries = 1;

        if (_settings.ExportDerivative)
        {
            numSeries = 2;
            SeriesName += $"\t{StringResources.FileHeader28}";
        }

        try
        {
            using var fs = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            using var sw = new StreamWriter(fs, System.Text.Encoding.UTF8);

            // Append millisecond pattern to current culture's full date time pattern
            string fullPattern = _settings.AppCulture.DateTimeFormat.FullDateTimePattern;
            fullPattern = System.Text.RegularExpressions.Regex.Replace(fullPattern, "(:ss|:s)", _settings.MillisecondsFormat);

            // Save the header text into the file
            string content = string.Empty;
            TimeSpan nTime = Signal.StartTime.AddSeconds((signal.Length - 1) / Signal.SampleFrequency) - Signal.StartTime; // At least there should be 1 point

            sw.WriteLine($"{StringResources.FileHeader01} ({_settings.AppCultureName})");
            sw.WriteLine($"{StringResources.FileHeader02}: {Signal.StartTime.AddSeconds(ArrIndexInit / Signal.SampleFrequency).ToString(fullPattern, _settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader03}: {Signal.StartTime.AddSeconds((signal.Length - 1 + ArrIndexInit) / Signal.SampleFrequency).ToString(fullPattern, _settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader04}: " +
                $"{nTime.Days} {StringResources.FileHeader22}, " +
                $"{nTime.Hours} {StringResources.FileHeader23}, " +
                $"{nTime.Minutes} {StringResources.FileHeader24}, " +
                $"{nTime.Seconds} {StringResources.FileHeader25} " +
                $"{StringResources.FileHeader26} " +
                $"{nTime.Milliseconds} {StringResources.FileHeader27}");
            sw.WriteLine($"{StringResources.FileHeader17}: {numSeries}");
            sw.WriteLine($"{StringResources.FileHeader05}: {signal.Length.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader06}: {Signal.SampleFrequency.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader07}: {Results.Average.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader08}: {Results.Maximum.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader09}: {Results.Minimum.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader10}: {Results.FractalDimension.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader11}: {Results.FractalVariance.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader12}: {Results.ApproximateEntropy.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader13}: {Results.SampleEntropy.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader14}: {Results.ShannonEntropy.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader15}: {Results.EntropyBit.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader16}: {Results.IdealEntropy.ToString(_settings.AppCulture)}");
            if (_settings.ExportDerivative && _settings.ComputeDerivative)
                sw.WriteLine($"{StringResources.FileHeader29}: {StringResources.DifferentiationAlgorithms.Split(", ")[(int)_settings.DerivativeAlgorithm]}");
            else
                sw.WriteLine($"{StringResources.FileHeader29}: -");
            sw.WriteLine();
            sw.WriteLine($"{StringResources.FileHeader21}\t{SeriesName}");

            string time;
            // Save the numerical values
            for (int j = 0; j < signal.Length; j++)
            {
                time = Signal.StartTime.AddSeconds((j+ ArrIndexInit) / Signal.SampleFrequency).ToString(fullPattern, _settings.AppCulture);
                content = $"{time}\t{signal[j].ToString(_settings.DataFormat, _settings.AppCulture)}";
                if (_settings.ExportDerivative && _settings.ComputeDerivative)
                    content += $"\t{Results.Derivative[j]}";
                
                //trying to write data to file
                sw.WriteLine(content);
            }

            // Success!
            result = true;
        }
        catch (Exception ex)
        {
            // Show error message
            using (new CenterWinDialog(this))
            {
                MessageBox.Show(this,
                    String.Format(_settings.AppCulture, StringResources.MsgBoxErrorSaveData, ex.Message),
                    StringResources.MsgBoxErrorSaveDataTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        return result;
    }

    /// <summary>
    /// Saves data into a SignalAnalysis-formatted file.
    /// </summary>
    /// <param name="fileName">Path (including name) of the sig file</param>
    /// <param name="signal">Array of values to be saved</param>
    /// <param name="ArrIndexInit">Offset index of _signalData (used to compute the time data field)</param>
    /// <param name="SeriesName">Name of the serie data to be saved</param>
    /// <returns><see langword="True"/> if successful, <see langword="false"/> otherwise</returns>
    private bool SaveSigData(string fileName, double[] signal, int ArrIndexInit, string? SeriesName)
    {
        bool result = false;
        int numSeries = 1;

        if (_settings.ExportDerivative)
        {
            numSeries = 2;
            SeriesName += $"\t{StringResources.FileHeader28}";
        }

        try
        {
            using var fs = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            using var sw = new StreamWriter(fs, System.Text.Encoding.UTF8);

            // Append millisecond pattern to current culture's full date time pattern
            string fullPattern = _settings.AppCulture.DateTimeFormat.FullDateTimePattern;
            fullPattern = System.Text.RegularExpressions.Regex.Replace(fullPattern, "(:ss|:s)", _settings.MillisecondsFormat);

            // Save the header text into the file
            string content = string.Empty;

            sw.WriteLine($"{StringResources.FileHeader01} ({_settings.AppCultureName})");
            sw.WriteLine($"{StringResources.FileHeader17}: {numSeries}");
            sw.WriteLine($"{StringResources.FileHeader05}: {signal.Length.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader06}: {Signal.SampleFrequency.ToString(_settings.AppCulture)}");
            if (_settings.ExportDerivative && _settings.ComputeDerivative)
                sw.WriteLine($"{StringResources.FileHeader29}: {StringResources.DifferentiationAlgorithms.Split(", ")[(int)_settings.DerivativeAlgorithm]}");
            else
                sw.WriteLine($"{StringResources.FileHeader29}: -");
            sw.WriteLine();
            sw.WriteLine($"{SeriesName}");

            // Save the numerical values
            for (int j = 0; j < signal.Length; j++)
            {
                content = signal[j].ToString(_settings.DataFormat, _settings.AppCulture);
                if (_settings.ExportDerivative)
                    content += $"\t{Results.Derivative[j]}";

                // Trying to write data to file
                sw.WriteLine(content);
            }

            // Success!
            result = true;
        }
        catch (Exception ex)
        {
            // Show error message
            using (new CenterWinDialog(this))
            {
                MessageBox.Show(this,
                    String.Format(_settings.AppCulture, StringResources.MsgBoxErrorSaveData, ex.Message),
                    StringResources.MsgBoxErrorSaveDataTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        return result;
    }

    /// <summary>
    /// Saves data into a binary-formatted file. Adapts the text-format to a binary format.
    /// </summary>
    /// <param name="fileName">Path (including name) of the sig file</param>
    /// <param name="signal">Array of values to be saved</param>
    /// <param name="ArrIndexInit">Offset index of _signalData (used to compute the time data field)</param>
    /// <param name="SeriesName">Name of the serie data to be saved</param>
    /// <returns><see langword="True"/> if successful, <see langword="false"/> otherwise</returns>
    private bool SaveBinaryData(string fileName, double[] signal, int ArrIndexInit, string? SeriesName)
    {
        bool result = false;
        int numSeries = 1;

        if (_settings.ExportDerivative)
        {
            numSeries = 2;
            SeriesName += $"\t{StringResources.FileHeader28}";
        }

        try
        {
            using var fs = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            using var bw = new BinaryWriter(fs, System.Text.Encoding.UTF8, false);
            
            string content = string.Empty;
            TimeSpan nTime = Signal.StartTime.AddSeconds((signal.Length - 1) / Signal.SampleFrequency) - Signal.StartTime; // At least there should be 1 point

            // Save the header text into the file
            bw.Write($"{StringResources.FileHeader01} ({_settings.AppCultureName})");
            bw.Write(Signal.StartTime.AddSeconds(ArrIndexInit / Signal.SampleFrequency));
            bw.Write(Signal.StartTime.AddSeconds((signal.Length - 1 + ArrIndexInit) / Signal.SampleFrequency));
            bw.Write(nTime.Days);
            bw.Write(nTime.Hours);
            bw.Write(nTime.Minutes);
            bw.Write(nTime.Seconds);
            bw.Write(nTime.Milliseconds);
            bw.Write(numSeries);
            bw.Write(signal.Length);
            bw.Write(Signal.SampleFrequency);
            bw.Write(Results.Average);
            bw.Write(Results.Maximum);
            bw.Write(Results.Minimum);
            bw.Write(Results.FractalDimension);
            bw.Write(Results.FractalVariance);
            bw.Write(Results.ApproximateEntropy);
            bw.Write(Results.SampleEntropy);
            bw.Write(Results.ShannonEntropy);
            bw.Write(Results.EntropyBit);
            bw.Write(Results.IdealEntropy);

            bw.Write($"{StringResources.FileHeader21}\t{SeriesName}");

            // Save the numerical values
            for (int j = 0; j < signal.Length; j++)
            {
                bw.Write(Signal.StartTime.AddSeconds((j + ArrIndexInit) / Signal.SampleFrequency));
                bw.Write(signal[j]);
                if (_settings.ExportDerivative)
                    bw.Write(Results.Derivative[j]);
            }

            // Success!
            result = true;
        }
        catch (Exception ex)
        {
            // Show error message
            using (new CenterWinDialog(this))
            {
                MessageBox.Show(this,
                    String.Format(_settings.AppCulture, StringResources.MsgBoxErrorSaveData, ex.Message),
                    StringResources.MsgBoxErrorSaveDataTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        
        return result;
    }

    /// <summary>
    /// Default saving, reverting to SaveTextData function.
    /// </summary>
    /// <param name="fileName">Path (including name) of the sig file</param>
    /// <param name="signal">Array of values to be saved</param>
    /// <param name="ArrIndexInit">Offset index of _signalData</param>
    /// <param name="SeriesName">Name of the serie data to be saved</param>
    /// <returns><see langword="True"/> if successful, <see langword="false"/> otherwise</returns>
    private bool SaveDefaultData(string fileName, double[] signal, int ArrIndexInit, string? SeriesName)
    {
        return SaveTextData(fileName, signal, ArrIndexInit, SeriesName);
    }

    /// <summary>
    /// Results saving function
    /// </summary>
    /// <param name="fileName">Path (including name) of the results file</param>
    /// <returns><see langword="True"/> if successful, <see langword="false"/> otherwise</returns>
    private bool SaveResultsData(string fileName)
    {
        bool result = false;

        try
        {
            using var fs = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            using var sw = new StreamWriter(fs, System.Text.Encoding.UTF8);

            sw.WriteLine($"{StringResources.FileHeader01} ({_settings.AppCultureName})");
            sw.WriteLine(Results.ToString(_settings.AppCulture));
            sw.WriteLine();
            sw.WriteLine($"{StringResources.PlotFFTXLabel}\t{StringResources.PlotFFTYLabelMag}\t{StringResources.PlotFFTYLabelPow}");

            // Save the numerical values
            for (int j = 0; j < Results.FFTfrequencies.Length; j++)
            {
                //trying to write data to text file
                sw.WriteLine($"{Results.FFTfrequencies[j].ToString("0.########", _settings.AppCulture)}\t" +
                    $"{Results.FFTmagnitude[j].ToString("0.########", _settings.AppCulture)}\t" +
                    $"{Results.FFTpower[j].ToString("0.########", _settings.AppCulture)}"
                    );
            }

            // Success!
            result = true;

        }
        catch (Exception ex)
        {
            // Show error message
            using (new CenterWinDialog(this))
            {
                MessageBox.Show(this,
                    String.Format(_settings.AppCulture, StringResources.MsgBoxErrorSaveData, ex.Message),
                    StringResources.MsgBoxErrorSaveDataTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        return result;
    }
}
