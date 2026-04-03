using SignalAnalysis.Converters;
using SignalAnalysis.Helpers;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using static SignalAnalysis.Helpers.MessageBox;

namespace SignalAnalysis.Models;

/// <summary>
/// Represents a data transfer object (DTO) for data in an elux file, containing metadata and other details.
/// </summary>
/// <remarks>This class is used to serialize and deserialize elux data, including information about the
/// document type, file version, and signal data.</remarks>
internal class EluxlDto: DocumentBase
{
    [JsonConverter(typeof(LocalizedDateTimeConverter))]
    public DateTime StartingTime { get; set; }

    [JsonConverter(typeof(LocalizedDateTimeConverter))]
    public DateTime EndingTime { get; set; }

    [JsonConverter(typeof(LocalizedTimeSpanConverter))]
    public TimeSpan Duration { get; set; }

    //public int SensorNumber { get; set; }
    //public int SeriesPoints { get; set; }
    //public double SamplingFrequency { get; set; }

    //public List<string> SeriesNames { get; set; } = [];
    //public List<List<double>> SeriesData { get; set; } = [];

    // Helper: crea JsonSerializerOptions con la cultura del DTO y añade converters instanciados con esa cultura.
    public override JsonSerializerOptions CreateJsonOptions(bool serializeDoublesAsStrings = false)
    {
        var culture = !string.IsNullOrWhiteSpace(CultureName) ? new CultureInfo(CultureName) : CultureInfo.InvariantCulture;
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new EluxlNamingPolicy(),
            WriteIndented = true
        };
        options.Converters.Add(new LocalizedDateTimeConverter(culture));
        options.Converters.Add(new LocalizedTimeSpanConverter(culture));
        // añadir otros converters si procede
        return options;
    }

    //private CultureInfo GetCultureInfoOrDefault()
    //{
    //    try
    //    {
    //        if (!string.IsNullOrWhiteSpace(CultureName))
    //            return new CultureInfo(CultureName);
    //    }
    //    catch { /* ignore and fallback */ }
    //    return new CultureInfo("en-US");
    //}

    // Parsea un fichero ErgoLux en memoria (líneas) y devuelve un DTO rellenado.
    // Asume que la primera línea contiene algo como "ErgoLux data (es-ES)" o similar.
    public static EluxlDto ParseFromErgoLuxText(string[] lines)
    {
        var dto = new EluxlDto();

        /// <summary>
        /// Reads data from an elux-formatted file and stores it into a <see cref="SignalData"/> parameter.
        /// </summary>
        /// <param name="fileName">Path (including name) of the elux file</param>
        /// <param name="signal"><see cref="SignalData"/> variable to store data read from the elux file</param>
        /// <returns><see langword="True"/> if successful, <see langword="false"/> otherwise</returns>
   
        DateTime start;
        DateTime end;
        int points, series;
        double sampleFreq;
        string[] seriesLabels;

        try
        {

            if (lines[0] is null)
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile"), "StrOpenDataFile00".GetLocalized("ReadDataFile")));
            System.Globalization.CultureInfo fileCulture = new(lines[0][(lines[0].IndexOf('(') + 1)..^1]);
            if (!lines[0].Contains($"{"StrOpenDataFile00".GetLocalized("ReadDataFile", fileCulture) ?? "ErgoLux data"} (", StringComparison.Ordinal))
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile", fileCulture), "StrOpenDataFile00".GetLocalized("ReadDataFile", fileCulture)));
            dto.DocumentType = "elux";
            dto.CultureName = fileCulture.Name;

            // Start time
            if (lines[1] is null)
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile", fileCulture), "StrOpenDataFile02".GetLocalized("ReadDataFile", fileCulture)));
            if (!lines[1].Contains($"{"StrOpenDataFile02".GetLocalized("ReadDataFile", fileCulture) ?? "Start time"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile", fileCulture), "StrOpenDataFile02".GetLocalized("ReadDataFile", fileCulture)));
            string fullPattern = fileCulture.DateTimeFormat.FullDateTimePattern;
            fullPattern = System.Text.RegularExpressions.Regex.Replace(fullPattern, "(:ss|:s)", GetMillisecondsFormat(fileCulture));
            if (lines[1] == null || !DateTime.TryParseExact(lines[1][(lines[1].IndexOf(':') + 2)..], fullPattern, fileCulture, System.Globalization.DateTimeStyles.None, out start))
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile", fileCulture), "StrOpenDataFile02".GetLocalized("ReadDataFile", fileCulture)));
            dto.StartingTime = start;

            string GetMillisecondsFormat(System.Globalization.CultureInfo culture)
            {
                return $"$1{culture.NumberFormat.NumberDecimalSeparator}fff";
            }

            // End time
            if (lines[2] is null)
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile", fileCulture), "StrOpenDataFile03".GetLocalized("ReadDataFile", fileCulture)));
            if (!lines[2].Contains($"{"StrOpenDataFile03".GetLocalized("ReadDataFile", fileCulture) ?? "End time"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile", fileCulture), "StrOpenDataFile03".GetLocalized("ReadDataFile", fileCulture)));
            if (lines[2] == null || !DateTime.TryParseExact(lines[2][(lines[2].IndexOf(':') + 2)..], fullPattern, fileCulture, System.Globalization.DateTimeStyles.None, out end))
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile", fileCulture), "StrOpenDataFile03".GetLocalized("ReadDataFile", fileCulture)));
            dto.EndingTime = end;

            // Total measuring time
            if (lines[3] is null)
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile", fileCulture), "StrOpenDataFile04".GetLocalized("ReadDataFile", fileCulture)));
            if (!lines[3].Contains($"{"StrOpenDataFile04".GetLocalized("ReadDataFile", fileCulture) ?? "Total measuring time"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile", fileCulture), "StrOpenDataFile04".GetLocalized("ReadDataFile", fileCulture)));
            //if(!TimeSpan.TryParse(lines[3][(lines[3].IndexOf(':') + 2)..], fileCulture, out TimeSpan duration))
            //    throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile", fileCulture), "StrOpenDataFile04".GetLocalized("ReadDataFile", fileCulture)));
            dto.Duration = dto.EndingTime - dto.StartingTime;

            // Number of sensors
            if (lines[4] is null)
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile", fileCulture), "StrOpenDataFile18".GetLocalized("ReadDataFile", fileCulture)));
            if (!lines[4].Contains($"{"StrOpenDataFile18".GetLocalized("ReadDataFile", fileCulture) ?? "Number of sensors"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile", fileCulture), "StrOpenDataFile18".GetLocalized("ReadDataFile", fileCulture)));
            if (!int.TryParse(lines[4][(lines[4].IndexOf(':') + 1)..], out series))
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile", fileCulture), "StrOpenDataFile18".GetLocalized("ReadDataFile", fileCulture)));
            if (series <= 0)
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile", fileCulture), "StrOpenDataFile18".GetLocalized("ReadDataFile", fileCulture)));
            dto.SeriesNumber = series + 6;

            // Number of data points
            if (lines[5] is null)
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile", fileCulture), "StrOpenDataFile05".GetLocalized("ReadDataFile", fileCulture)));
            if (!lines[5].Contains($"{"StrOpenDataFile05".GetLocalized("ReadDataFile", fileCulture) ?? "Number of data points"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile", fileCulture), "StrOpenDataFile05".GetLocalized("ReadDataFile", fileCulture)));
            if (!int.TryParse(lines[5][(lines[5].IndexOf(':') + 1)..], out points))
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile", fileCulture), "StrOpenDataFile05".GetLocalized("ReadDataFile", fileCulture)));
            if (points <= 0)
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile", fileCulture), "StrOpenDataFile05".GetLocalized("ReadDataFile", fileCulture)));
            dto.SeriesPoints = points;

            // Sampling frequency
            if (lines[6] is null)
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile", fileCulture), "StrOpenDataFile06".GetLocalized("ReadDataFile", fileCulture)));
            if (!lines[6].Contains($"{"StrOpenDataFile06".GetLocalized("ReadDataFile", fileCulture) ?? "Sampling frequency"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile", fileCulture), "StrOpenDataFile06".GetLocalized("ReadDataFile", fileCulture)));
            if (!double.TryParse(lines[6][(lines[6].IndexOf(':') + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out sampleFreq))
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile", fileCulture), "StrOpenDataFile06".GetLocalized("ReadDataFile", fileCulture)));
            if (sampleFreq <= 0)
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile", fileCulture), "StrOpenDataFile06".GetLocalized("ReadDataFile", fileCulture)));
            dto.SamplingFrequency = sampleFreq;

            // Empty line
            if (lines[7] is null)
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile", fileCulture), "StrOpenDataFile19".GetLocalized("ReadDataFile", fileCulture)));
            if (lines[7] != string.Empty)
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile", fileCulture), "StrOpenDataFile19".GetLocalized("ReadDataFile", fileCulture)));

            // Column header lines
            if (lines[8] is null)
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile", fileCulture), "StrOpenDataFile20".GetLocalized("ReadDataFile", fileCulture)));
            seriesLabels = lines[8].Split('\t');
            if (seriesLabels == Array.Empty<string>())
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile", fileCulture), "StrOpenDataFile20".GetLocalized("ReadDataFile", fileCulture)));
            dto.SeriesNames = seriesLabels.ToList();

            // Retrieve data rows and parse values
            dto.SeriesData = Enumerable.Range(0, seriesLabels.Length).Select(_ => new List<double>()).ToList();
            for (int i = 9; i < lines.Length; i++)
            {
                var line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;
                var tokens = line.Split('\t');
                for (int s = 0; s < seriesLabels.Length && s < tokens.Length; s++)
                {
                    if (double.TryParse(tokens[s], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out double val))
                    {
                        dto.SeriesData[s].Add(val);
                    }
                    else
                    {
                        // Try to extract a number with regex as a fallback, in case of trailing characters or formatting issues
                        var m = Regex.Match(tokens[s], @"-?\d+([,\.]\d+)?");
                        if (m.Success)
                        {
                            var raw = m.Value;
                            // Replace comma with the culture decimal separator if needed, and remove thousand separators
                            if (fileCulture.NumberFormat.NumberDecimalSeparator == ",")
                                raw = raw.Replace(".", "").Replace(",", ".");
                            else
                                raw = raw.Replace(",", "");
                            if (double.TryParse(raw, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out var fallback))
                                dto.SeriesData[s].Add(fallback);
                        }
                    }
                }
            }

        }
        catch (System.Globalization.CultureNotFoundException ex)
        {
            //result = false;
            //using (new CenterWinDialog(this))
            //    MessageBox.Show(this,
            //        string.Format(_settings.AppCulture, StringResources.ReadDataErrorCulture, ex.Message),
            //        StringResources.ReadDataErrorCultureTitle,
            //        MessageBoxButtons.OK,
            //        MessageBoxIcon.Error);
        }
        catch (FormatException ex)
        {
            //result = false;
            //using (new CenterWinDialog(this))
            //    MessageBox.Show(this,
            //        string.Format(_settings.AppCulture, StringResources.ReadDataError, ex.Message),
            //        StringResources.ReadDataErrorTitle,
            //        MessageBoxButtons.OK,
            //        MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            //result = false;
            //using (new CenterWinDialog(this))
            //{
            //    MessageBox.Show(this,
            //        string.Format(_settings.AppCulture, StringResources.MsgBoxErrorOpenData, ex.Message),
            //        StringResources.MsgBoxErrorOpenDataTitle,
            //        MessageBoxButtons.OK,
            //        MessageBoxIcon.Error);
            //}
        }

        return dto;
    }

    // Intentos de parseo auxiliares
    private static DateTime? TryParseDateTime(string s, CultureInfo culture)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        // Quitar prefijos como "martes," si el parseo falla
        var formats = new[]
        {
            "dddd, d 'de' MMMM 'de' yyyy HH:mm:ss,fff",
            "d/M/yyyy H:mm:ss,fff",
            "d/M/yyyy H:mm:ss",
            "yyyy-MM-ddTHH:mm:ss.fff",
            "o"
        };
        if (DateTime.TryParseExact(s, formats, culture, DateTimeStyles.None, out var dt))
            return dt;
        if (DateTime.TryParse(s, culture, DateTimeStyles.None, out dt))
            return dt;
        // Try invariant
        if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out dt))
            return dt;
        return null;
    }

    private static TimeSpan? TryParseDuration(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        var nums = Regex.Matches(s, @"\d+").Cast<Match>().Select(m => int.Parse(m.Value, CultureInfo.InvariantCulture)).ToArray();
        int d = nums.Length > 0 ? nums[0] : 0;
        int h = nums.Length > 1 ? nums[1] : 0;
        int m = nums.Length > 2 ? nums[2] : 0;
        int sec = nums.Length > 3 ? nums[3] : 0;
        int ms = nums.Length > 4 ? nums[4] : 0;
        return new TimeSpan(d, h, m, sec, ms);
    }
}

