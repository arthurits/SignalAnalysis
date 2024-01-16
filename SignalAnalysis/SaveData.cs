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

        if (_settings.ExportDerivative && _settings.ComputeDerivative)
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
            sw.WriteLine($"{StringResources.FileHeader02}{StringResources.FileHeaderColon}{Signal.StartTime.AddSeconds(ArrIndexInit / Signal.SampleFrequency).ToString(fullPattern, _settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader03}{StringResources.FileHeaderColon}{Signal.StartTime.AddSeconds((signal.Length - 1 + ArrIndexInit) / Signal.SampleFrequency).ToString(fullPattern, _settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader04}{StringResources.FileHeaderColon}" +
                $"{nTime.Days} {GetSubstring(StringResources.FileHeader22, nTime.Days)}, " +
                $"{nTime.Hours} {GetSubstring(StringResources.FileHeader23, nTime.Hours)}, " +
                $"{nTime.Minutes} {GetSubstring(StringResources.FileHeader24, nTime.Minutes)}, " +
                $"{nTime.Seconds} {GetSubstring(StringResources.FileHeader25, nTime.Seconds)} " +
                $"{StringResources.FileHeader26} " +
                $"{nTime.Milliseconds} {GetSubstring(StringResources.FileHeader27, nTime.Milliseconds)}");
            sw.WriteLine($"{StringResources.FileHeader17}{StringResources.FileHeaderColon}{numSeries}");
            sw.WriteLine($"{StringResources.FileHeader05}{StringResources.FileHeaderColon}{signal.Length.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader06}{StringResources.FileHeaderColon}{Signal.SampleFrequency.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader07}{StringResources.FileHeaderColon}{Results.Average.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader32}{StringResources.FileHeaderColon}{Results.Variance.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader08}{StringResources.FileHeaderColon}{Results.Maximum.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader09}{StringResources.FileHeaderColon}{Results.Minimum.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader33}{StringResources.FileHeaderColon}{Results.BoxplotMin.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader35}{StringResources.FileHeaderColon}{Results.BoxplotQ1.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader36}{StringResources.FileHeaderColon}{Results.BoxplotQ2.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader37}{StringResources.FileHeaderColon}{Results.BoxplotQ3.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader34}{StringResources.FileHeaderColon}{Results.BoxplotMax.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader10}{StringResources.FileHeaderColon}{Results.FractalDimension.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader11}{StringResources.FileHeaderColon}{Results.FractalVariance.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader14}{StringResources.FileHeaderColon}{Results.ShannonEntropy.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader15}{StringResources.FileHeaderColon}{Results.EntropyBit.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader16}{StringResources.FileHeaderColon}{Results.IdealEntropy.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader38}{StringResources.FileHeaderColon}{Results.ShannonIdeal.ToString(_settings.AppCulture)}");
            if (_settings.ComputeEntropy)
            {
                sw.WriteLine($"{StringResources.FileHeader39}{StringResources.FileHeaderColon}{StringResources.EntropyAlgorithms.Split(", ")[(int)_settings.EntropyAlgorithm]}");
                sw.WriteLine($"{StringResources.FileHeader40}{StringResources.FileHeaderColon}{_settings.EntropyFactorR.ToString(_settings.AppCulture)}");
                sw.WriteLine($"{StringResources.FileHeader41}{StringResources.FileHeaderColon}{_settings.EntropyFactorM.ToString(_settings.AppCulture)}");
            }
            else
            {
                sw.WriteLine($"{StringResources.FileHeader39}{StringResources.FileHeaderColon}-");
                sw.WriteLine($"{StringResources.FileHeader40}{StringResources.FileHeaderColon}-");
                sw.WriteLine($"{StringResources.FileHeader41}{StringResources.FileHeaderColon}-");
            }
            sw.WriteLine($"{StringResources.FileHeader12}{StringResources.FileHeaderColon}{Results.ApproximateEntropy.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader13}{StringResources.FileHeaderColon}{Results.SampleEntropy.ToString(_settings.AppCulture)}");
            if (_settings.ExportDerivative && _settings.ComputeDerivative)
                sw.WriteLine($"{StringResources.FileHeader29}{StringResources.FileHeaderColon}{StringResources.DifferentiationAlgorithms.Split(", ")[(int)_settings.DerivativeAlgorithm]}");
            else
                sw.WriteLine($"{StringResources.FileHeader29}{StringResources.FileHeaderColon}-");
            if (_settings.ExportIntegration && _settings.ComputeIntegration)
            {
                sw.WriteLine($"{StringResources.FileHeader30}{StringResources.FileHeaderColon}{StringResources.IntegrationAlgorithms.Split(", ")[(int)_settings.IntegrationAlgorithm]}");
                sw.WriteLine($"{StringResources.FileHeader31}{StringResources.FileHeaderColon}{Results.Integral.ToString(_settings.AppCulture)}");
            }
            else
            {
                sw.WriteLine($"{StringResources.FileHeader30}{StringResources.FileHeaderColon}-");
                sw.WriteLine($"{StringResources.FileHeader31}{StringResources.FileHeaderColon}-");
            }
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

        //if (_settings.ExportDerivative)
        //{
        //    numSeries = 2;
        //    SeriesName += $"\t{StringResources.FileHeader28}";
        //}

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
            sw.WriteLine($"{StringResources.FileHeader17}{StringResources.FileHeaderColon}{numSeries}");
            sw.WriteLine($"{StringResources.FileHeader05}{StringResources.FileHeaderColon}{signal.Length.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader06}{StringResources.FileHeaderColon}{Signal.SampleFrequency.ToString(_settings.AppCulture)}");
            //if (_settings.ExportDerivative && _settings.ComputeDerivative)
            //    sw.WriteLine($"{StringResources.FileHeader29}: {StringResources.DifferentiationAlgorithms.Split(", ")[(int)_settings.DerivativeAlgorithm]}");
            //else
            //    sw.WriteLine($"{StringResources.FileHeader29}: -");
            sw.WriteLine();
            sw.WriteLine($"{SeriesName}");

            // Save the numerical values
            for (int j = 0; j < signal.Length; j++)
            {
                content = signal[j].ToString(_settings.DataFormat, _settings.AppCulture);
                //if (_settings.ExportDerivative)
                //    content += $"\t{Results.Derivative[j]}";

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
            bw.Write(Results.Variance);
            bw.Write(Results.Maximum);
            bw.Write(Results.Minimum);
            bw.Write(Results.BoxplotMin);
            bw.Write(Results.BoxplotQ1);
            bw.Write(Results.BoxplotQ2);
            bw.Write(Results.BoxplotQ3);
            bw.Write(Results.BoxplotMax);
            bw.Write(Results.FractalDimension);
            bw.Write(Results.FractalVariance);
            bw.Write(Results.ShannonEntropy);
            bw.Write(Results.EntropyBit);
            bw.Write(Results.IdealEntropy);
            bw.Write(Results.ShannonIdeal);
            bw.Write((byte)_settings.EntropyAlgorithm);
            bw.Write(_settings.EntropyFactorM);
            bw.Write(_settings.EntropyFactorR);
            bw.Write(Results.ApproximateEntropy);
            bw.Write(Results.SampleEntropy);
            bw.Write((byte)_settings.IntegrationAlgorithm);
            bw.Write(Results.Integral);

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
    private bool SaveResultsData(string fileName, double[]? signal = null, int ArrIndexInit = 0)
    {
        bool result = false;
        bool derivative = false;

        try
        {
            using var fs = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            using var sw = new StreamWriter(fs, System.Text.Encoding.UTF8);

            sw.WriteLine($"{StringResources.FileHeader01} ({_settings.AppCultureName})");
            sw.WriteLine(Results.ToString(culture: _settings.AppCulture, boxplot: _settings.Boxplot, entropy: false, integral: false));
            if (_settings.ExportDerivative && _settings.ComputeDerivative)
            {
                sw.WriteLine($"{StringResources.FileHeader29}{StringResources.FileHeaderColon}{StringResources.DifferentiationAlgorithms.Split(", ")[(int)_settings.DerivativeAlgorithm]}");
                derivative = true;
            }

            if (_settings.ComputeEntropy)
            {
                sw.WriteLine($"{StringResources.FileHeader14}{StringResources.FileHeaderColon}{Results.ShannonEntropy.ToString("0.########", _settings.AppCulture)}");
                sw.WriteLine($"{StringResources.FileHeader15}{StringResources.FileHeaderColon}{Results.EntropyBit.ToString("0.########", _settings.AppCulture)}");
                sw.WriteLine($"{StringResources.FileHeader16}{StringResources.FileHeaderColon}{Results.IdealEntropy.ToString("0.########", _settings.AppCulture)}");
                sw.WriteLine($"{StringResources.FileHeader38}{StringResources.FileHeaderColon}{Results.ShannonIdeal.ToString("0.########", _settings.AppCulture)}");
                sw.WriteLine($"{StringResources.FileHeader39}{StringResources.FileHeaderColon}{StringResources.EntropyAlgorithms.Split(", ")[(int)_settings.EntropyAlgorithm]}");
                sw.WriteLine($"{StringResources.FileHeader12}{StringResources.FileHeaderColon}({_settings.EntropyFactorM}, {_settings.EntropyFactorR.ToString("0.##", _settings.AppCulture)}){Results.ApproximateEntropy.ToString("0.########", _settings.AppCulture)}");
                sw.WriteLine($"{StringResources.FileHeader13}{StringResources.FileHeaderColon}({_settings.EntropyFactorM}, {_settings.EntropyFactorR.ToString("0.##", _settings.AppCulture)}){Results.SampleEntropy.ToString("0.########", _settings.AppCulture)}");
            }

            if (_settings.ExportIntegration && _settings.ComputeIntegration)
            {
                sw.WriteLine($"{StringResources.FileHeader30}{StringResources.FileHeaderColon}{StringResources.IntegrationAlgorithms.Split(", ")[(int)_settings.IntegrationAlgorithm]}");
                sw.WriteLine($"{StringResources.FileHeader31}{StringResources.FileHeaderColon}{Results.Integral.ToString("0.########", _settings.AppCulture)}");
            }

            sw.WriteLine();
            sw.WriteLine($"{StringResources.PlotFFTXLabel}\t{StringResources.PlotFFTYLabelMag}\t{StringResources.PlotFFTYLabelPow}");

            // Save the FFT numerical values
            for (int j = 0; j < Results.FFTfrequencies.Length; j++)
            {
                //trying to write data to text file
                sw.WriteLine($"{Results.FFTfrequencies[j].ToString("0.########", _settings.AppCulture)}\t" +
                    $"{Results.FFTmagnitude[j].ToString("0.########", _settings.AppCulture)}\t" +
                    $"{Results.FFTpower[j].ToString("0.########", _settings.AppCulture)}" +
                    $"{(derivative ? $"\t{Results.Derivative[j]}" : string.Empty)}"
                    );
            }

            // Save the differentiation values
            if (derivative && signal is not null)
            {
                sw.WriteLine();
                sw.WriteLine($"{(derivative ? $"\t{StringResources.FileHeader28}" : string.Empty)}");


                string time;
                // Append millisecond pattern to current culture's full date time pattern
                string fullPattern = _settings.AppCulture.DateTimeFormat.FullDateTimePattern;
                fullPattern = System.Text.RegularExpressions.Regex.Replace(fullPattern, "(:ss|:s)", _settings.MillisecondsFormat);

                for (int j = 0; j < Results.Derivative.Length; j++)
                {
                    time = Signal.StartTime.AddSeconds((j + ArrIndexInit) / Signal.SampleFrequency).ToString(fullPattern, _settings.AppCulture);

                    //trying to write data to file
                    sw.WriteLine($"{time}\t{signal[j].ToString(_settings.DataFormat, _settings.AppCulture)}\t{Results.Derivative[j]}");
                }
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
