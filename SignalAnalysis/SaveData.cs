using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalAnalysis;
partial class FrmMain
{
    /// <summary>
    /// Saves data into a text-formatted file.
    /// </summary>
    /// <param name="FileName">Path (including name) of the text file</param>
    /// <param name="Data">Array of values to be saved</param>
    /// <param name="ArrIndexInit">Offset index of _signalData</param>
    /// <param name="SeriesName">Name of the serie data to be saved</param>
    private void SaveTextData(string FileName, double[] Data, int ArrIndexInit, string SeriesName)
    {
        var cursor = Cursor.Current;
        Cursor.Current = Cursors.WaitCursor;

        try
        {
            using var fs = File.Open(FileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            using var sw = new StreamWriter(fs, Encoding.UTF8);

            // Append millisecond pattern to current culture's full date time pattern
            //string fullPattern = System.Globalization.DateTimeFormatInfo.CurrentInfo.FullDateTimePattern;
            string fullPattern = _settings.AppCulture.DateTimeFormat.FullDateTimePattern;
            fullPattern = System.Text.RegularExpressions.Regex.Replace(fullPattern, "(:ss|:s)", _settings.MillisecondsFormat);

            // Save the header text into the file
            string content = string.Empty;
            TimeSpan nTime = nStart.AddSeconds((Data.Length - 1) / nSampleFreq) - nStart; // At least there should be 1 point

            sw.WriteLine($"SignalAnalysis data ({_settings.AppCultureName})");
            sw.WriteLine("Start time: {0}", nStart.AddSeconds(ArrIndexInit / nSampleFreq).ToString(fullPattern, _settings.AppCulture));
            sw.WriteLine("End time: {0}", nStart.AddSeconds((Data.Length - 1 + ArrIndexInit) / nSampleFreq).ToString(fullPattern, _settings.AppCulture));
            ////outfile.WriteLine("Total measuring time: {0} days, {1} hours, {2} minutes, {3} seconds, and {4} milliseconds ({5})", nTime.Days, nTime.Hours, nTime.Minutes, nTime.Seconds, nTime.Milliseconds, nTime.ToString(@"dd\-hh\:mm\:ss.fff"));
            sw.WriteLine("Total measuring time: {0} days, {1} hours, {2} minutes, {3} seconds, and {4} milliseconds", nTime.Days, nTime.Hours, nTime.Minutes, nTime.Seconds, nTime.Milliseconds);
            sw.WriteLine("Number of data points: {0}", Data.Length.ToString());
            sw.WriteLine("Sampling frequency: {0}", nSampleFreq.ToString(_settings.AppCulture));
            sw.WriteLine("Average illuminance: {0}", Results.Average.ToString(_settings.AppCulture));
            sw.WriteLine("Maximum illuminance: {0}", Results.Maximum.ToString(_settings.AppCulture));
            sw.WriteLine("Minimum illuminance: {0}", Results.Minimum.ToString(_settings.AppCulture));
            sw.WriteLine("Fractal dimension: {0}", Results.FractalDimension.ToString(_settings.AppCulture));
            sw.WriteLine("Fractal variance: {0}", Results.FractalVariance.ToString(_settings.AppCulture));
            sw.WriteLine("Approximate entropy: {0}", Results.ApproximateEntropy.ToString(_settings.AppCulture));
            sw.WriteLine("Sample entropy: {0}", Results.SampleEntropy.ToString(_settings.AppCulture));
            sw.WriteLine();
            sw.WriteLine($"Time\t{SeriesName}");

            string time;
            // Save the numerical values
            for (int j = 0; j < Data.Length; j++)
            {
                time = nStart.AddSeconds((j+ ArrIndexInit) / nSampleFreq).ToString(fullPattern, _settings.AppCulture);
                content = $"{time}\t{Data[j].ToString(_settings.DataFormat, _settings.AppCulture)}";
                
                //trying to write data to csv
                sw.WriteLine(content);
            }

            // Show OK save data
            using (new CenterWinDialog(this))
                MessageBox.Show("Data has been successfully saved to disk.", "Data saving", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch
        {
            // Show error message
            using (new CenterWinDialog(this))
                MessageBox.Show("An unexpected error happened while saving data to disk.\nPlease try again later or contact the software engineer.", "Error saving data", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Cursor.Current = cursor;
        }
    }

    /// <summary>
    /// Saves data into a SignalAnalysis-formatted file.
    /// </summary>
    /// <param name="FileName">Path (including name) of the sig file</param>
    /// <param name="Data">Array of values to be saved</param>
    /// <param name="ArrIndexInit">Offset index of _signalData</param>
    /// <param name="SeriesName">Name of the serie data to be saved</param>
    private void SaveSigData(string FileName, double[] Data, int ArrIndexInit, string SeriesName)
    {
        var cursor = Cursor.Current;
        Cursor.Current = Cursors.WaitCursor;

        try
        {
            using var fs = File.Open(FileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            using var sw = new StreamWriter(fs, Encoding.UTF8);

            // Append millisecond pattern to current culture's full date time pattern
            string fullPattern = _settings.AppCulture.DateTimeFormat.FullDateTimePattern;
            fullPattern = System.Text.RegularExpressions.Regex.Replace(fullPattern, "(:ss|:s)", _settings.MillisecondsFormat);

            // Save the header text into the file
            string content = string.Empty;
            //TimeSpan nTime = nStart.AddSeconds((nPoints - 1) / nSampleFreq) - nStart; // At least there should be 1 point

            sw.WriteLine($"SignalAnalysis data ({_settings.AppCultureName})");
            sw.WriteLine("Number of data series: {0}", "1");
            sw.WriteLine("Number of data points: {0}", Data.Length.ToString(_settings.AppCulture));
            sw.WriteLine("Sampling frequency: {0}", nSampleFreq.ToString(_settings.AppCulture));
            sw.WriteLine();
            sw.WriteLine($"{SeriesName}");

            // Save the numerical values
            for (int j = 0; j < Data.Length; j++)
                sw.WriteLine(Data[j].ToString(_settings.DataFormat, _settings.AppCulture));

            // Show OK save data
            using (new CenterWinDialog(this))
                MessageBox.Show("Data has been successfully saved to disk.", "Data saving", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch
        {
            // Show error message
            using (new CenterWinDialog(this))
                MessageBox.Show("An unexpected error happened while saving data to disk.\nPlease try again later or contact the software engineer.", "Error saving data", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Cursor.Current = cursor;
        }
    }

    /// <summary>
    /// Saves data into a binary format file.
    /// </summary>
    /// <param name="FileName">Path (including name) of the elux file</param>
    private void SaveBinaryData(string FileName)
    {
        throw new Exception("Saving to binary has not yet been implemented.");
    }

    /// <summary>
    /// Saves data. Default behaviour
    /// </summary>
    /// <param name="FileName">Path (including name) of the elux file</param>
    private void SaveDefaultData(string FileName)
    {
        using (new CenterWinDialog(this))
            MessageBox.Show("No data has been saved to disk.", "No data saved", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
