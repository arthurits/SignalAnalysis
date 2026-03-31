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

    public int SensorNumber { get; set; }
    public int SeriesPoints { get; set; }
    public double SamplingFrequency { get; set; }

    public List<string> SeriesNames { get; set; } = [];
    public List<List<double>> SeriesData { get; set; } = [];

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
        bool result = false;
        double sampleFreq;
        string[] seriesLabels;
        string? strLine;

        try
        {

            if (lines[0] is null)
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile"), "StrOpenDataFile00".GetLocalized("ReadDataFile")));
            System.Globalization.CultureInfo fileCulture = new(lines[0][(lines[0].IndexOf('(') + 1)..^1]);
            if (!lines[0].Contains($"{"StrOpenDataFile00".GetLocalized("ReadDataFile") ?? "ErgoLux data"} (", StringComparison.Ordinal))
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile"), "StrOpenDataFile00".GetLocalized("ReadDataFile")));

            // Start time
            if (lines[1] is null)
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile"), "StrOpenDataFile02".GetLocalized("ReadDataFile")));
            if (!lines[1].Contains($"{"StrOpenDataFile02".GetLocalized("ReadDataFile") ?? "Start time"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile"), "StrOpenDataFile02".GetLocalized("ReadDataFile")));
            string fullPattern = fileCulture.DateTimeFormat.FullDateTimePattern;
            //fullPattern = System.Text.RegularExpressions.Regex.Replace(fullPattern, "(:ss|:s)", AppSettings.GetMillisecondsFormat(fileCulture));
            //if (lines[1] == null || !DateTime.TryParseExact(lines[1][(lines[1].IndexOf(':') + 2)..], fullPattern, fileCulture, System.Globalization.DateTimeStyles.None, out start))
            //    throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile"), "StrOpenDataFile02".GetLocalized("ReadDataFile")));

            //strLine = sr.ReadLine();    // End time
            //if (strLine is null)
            //    throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader03));
            //if (!strLine.Contains($"{StringResources.GetString("strFileHeader03", fileCulture) ?? "End time"}: ", StringComparison.Ordinal))
            //    throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader03));
            //if (strLine == null || !DateTime.TryParseExact(strLine[(strLine.IndexOf(':') + 2)..], fullPattern, fileCulture, System.Globalization.DateTimeStyles.None, out end))
            //    throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader03));

            //strLine = sr.ReadLine();    // Total measuring time
            //if (strLine is null)
            //    throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader04));
            //if (!strLine.Contains($"{StringResources.GetString("strFileHeader04", fileCulture) ?? "Total measuring time"}: ", StringComparison.Ordinal))
            //    throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader04));

            //strLine = sr.ReadLine();    // Number of sensors
            //if (strLine is null)
            //    throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader18));
            //if (!strLine.Contains($"{StringResources.GetString("strFileHeader18", fileCulture) ?? "Number of sensors"}: ", StringComparison.Ordinal))
            //    throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader18));
            //if (!int.TryParse(strLine[(strLine.IndexOf(':') + 1)..], out series))
            //    throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader18));
            //if (series <= 0)
            //    throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader18));
            //series += 6;

            //strLine = sr.ReadLine();    // Number of data points
            //if (strLine is null)
            //    throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader05));
            //if (!strLine.Contains($"{StringResources.GetString("strFileHeader05", fileCulture) ?? "Number of data points"}: ", StringComparison.Ordinal))
            //    throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader05));
            //if (!int.TryParse(strLine[(strLine.IndexOf(':') + 1)..], out points))
            //    throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader05));
            //if (points <= 0)
            //    throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader05));

            //strLine = sr.ReadLine();    // Sampling frequency
            //if (strLine is null)
            //    throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader06));
            //if (!strLine.Contains($"{StringResources.GetString("strFileHeader06", fileCulture) ?? "Sampling frequency"}: ", StringComparison.Ordinal))
            //    throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader06));
            //if (!double.TryParse(strLine[(strLine.IndexOf(':') + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out sampleFreq))
            //    throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader06));
            //if (sampleFreq <= 0)
            //    throw new FormatException(string.Format(StringResources.FileHeaderSection, StringResources.FileHeader06));

            //strLine = sr.ReadLine();    // Empty line
            //if (strLine is null)
            //    throw new FormatException(StringResources.FileHeader19);
            //if (strLine != string.Empty)
            //    throw new FormatException(StringResources.FileHeader19);

            //strLine = sr.ReadLine();    // Column header lines
            //if (strLine is null)
            //    throw new FormatException(StringResources.FileHeader20);
            //seriesLabels = strLine.Split('\t');
            //if (seriesLabels == Array.Empty<string>())
            //    throw new FormatException(StringResources.FileHeader20);

            //result = InitializeDataArrays(sr, ref signal.Data, series, points, fileCulture);

            //// Store information regarding the signal
            //signal.StartTime = start;
            //signal.EndTime = end;
            //signal.MeasuringTime = end - start;
            //signal.SeriesNumber = series;
            //signal.SeriesPoints = points;
            //signal.SampleFrequency = sampleFreq;
            //signal.SeriesLabels = seriesLabels;
            //signal.IndexStart = 0;
            //signal.IndexEnd = signal.SeriesPoints - 1;
            ////signal.SeriesLabels = new string [seriesLabels.Length];
            ////Array.Copy(seriesLabels, signal.SeriesLabels, seriesLabels.Length);

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

        //var dto = new EluxlDto();
        //if (lines == null || lines.Length == 0) return dto;

        //// Detectar cultura en la primera línea: buscar "(xx-XX)" o "es-ES"
        //var first = lines[0].Trim();
        //var cultureMatch = Regex.Match(first, @"\(([a-zA-Z\-]+)\)");
        //if (cultureMatch.Success)
        //    dto.CultureName = cultureMatch.Groups[1].Value;

        //var culture = new CultureInfo(dto.CultureName);

        //// Función auxiliar para parsear double con la cultura detectada
        //double ParseDouble(string s)
        //{
        //    s = s.Trim();
        //    // Reemplazar posibles separadores de miles y normalizar
        //    // Dejar que double.Parse con culture haga el trabajo
        //    return double.Parse(s, NumberStyles.Float | NumberStyles.AllowThousands, culture);
        //}

        //// Recorrer líneas para metadatos hasta encontrar la tabla
        //int i = 0;
        //for (; i < lines.Length; i++)
        //{
        //    var line = lines[i].Trim();
        //    if (string.IsNullOrEmpty(line)) continue;

        //    // Cabeceras con formato "Clave: valor"
        //    if (line.Contains(":"))
        //    {
        //        var parts = line.Split(new[] { ':' }, 2);
        //        var key = parts[0].Trim();
        //        var val = parts[1].Trim();

        //        switch (key)
        //        {
        //            case string k when k.StartsWith("Tiempo inicial", StringComparison.OrdinalIgnoreCase):
        //                dto.StartingTime = TryParseDateTime(val, culture) ?? DateTime.MinValue;
        //                break;
        //            case string k when k.StartsWith("Tiempo final", StringComparison.OrdinalIgnoreCase):
        //                dto.EndingTime = TryParseDateTime(val, culture) ?? DateTime.MinValue;
        //                break;
        //            case string k when k.StartsWith("Duración", StringComparison.OrdinalIgnoreCase) || k.StartsWith("Duracion", StringComparison.OrdinalIgnoreCase):
        //                dto.Duration = TryParseDuration(val) ?? TimeSpan.Zero;
        //                break;
        //            case string k when k.StartsWith("Número de sensores", StringComparison.OrdinalIgnoreCase) || k.StartsWith("Numero de sensores", StringComparison.OrdinalIgnoreCase):
        //                if (int.TryParse(val, NumberStyles.Integer, culture, out var sn)) dto.SensorNumber = sn;
        //                break;
        //            case string k when k.StartsWith("Número de puntos", StringComparison.OrdinalIgnoreCase) || k.StartsWith("Numero de puntos", StringComparison.OrdinalIgnoreCase):
        //                if (int.TryParse(val, NumberStyles.Integer, culture, out var sp)) dto.SeriesPoints = sp;
        //                break;
        //            case string k when k.StartsWith("Frecuencia de muestreo", StringComparison.OrdinalIgnoreCase):
        //                // puede venir como "2" o "2,0"
        //                if (double.TryParse(val, NumberStyles.Float, culture, out var sf)) dto.SamplingFrequency = sf;
        //                break;
        //            default:
        //                // Otros metadatos: DocumentType, FileVersion, etc.
        //                if (i == 0) dto.DocumentType = line;
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        // Si la línea contiene "Sensor #00" es la cabecera de la tabla
        //        if (line.Contains("Sensor #00", StringComparison.OrdinalIgnoreCase) ||
        //            line.Contains("Sensor #0", StringComparison.OrdinalIgnoreCase))
        //        {
        //            break; // i apunta a la cabecera de la tabla
        //        }
        //    }
        //}

        //// Si no encontramos la cabecera, intentar localizarla buscando "Sensor #"
        //int headerIndex = i;
        //if (headerIndex >= lines.Length)
        //{
        //    for (int j = 0; j < lines.Length; j++)
        //    {
        //        if (lines[j].Contains("Sensor #", StringComparison.OrdinalIgnoreCase))
        //        {
        //            headerIndex = j;
        //            break;
        //        }
        //    }
        //}

        //if (headerIndex < lines.Length)
        //{
        //    // Leer nombres de series (cabecera)
        //    var headerLine = lines[headerIndex].Trim();
        //    // Separadores posibles: tabulador o múltiples espacios
        //    var headerCols = Regex.Split(headerLine, @"\t+|\s{2,}|\s+\|\s+|\s+").Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
        //    // Tomar solo las columnas que empiezan por "Sensor" o "Sensor #"
        //    var sensorCols = headerCols.Where(h => !h.StartsWith("#", StringComparison.OrdinalIgnoreCase) || Regex.IsMatch(h, @"^Sensor\s*#\d+", RegexOptions.IgnoreCase)).ToArray();
        //    if (sensorCols.Length == 0)
        //    {
        //        // fallback: tomar las primeras N tokens
        //        sensorCols = headerCols.Take(dto.SensorNumber > 0 ? dto.SensorNumber : headerCols.Length).ToArray();
        //    }
        //    dto.SeriesNames = sensorCols.ToList();

        //    // Inicializar SeriesData con tantas listas como sensores detectados
        //    int sensorsCount = dto.SeriesNames.Count;
        //    dto.SeriesData = Enumerable.Range(0, sensorsCount).Select(_ => new List<double>()).ToList();

        //    // Leer filas de datos a partir de la siguiente línea
        //    for (int r = headerIndex + 1; r < lines.Length; r++)
        //    {
        //        var row = lines[r].Trim();
        //        if (string.IsNullOrEmpty(row)) continue;

        //        // Romper si la fila no parece numérica (por ejemplo, otra sección)
        //        // Intentar extraer números de la fila
        //        // Separadores: tabulador o múltiples espacios
        //        var cols = Regex.Split(row, @"\t+|\s{2,}").Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
        //        if (cols.Length == 0) continue;

        //        // Si la fila contiene texto como "Máximo Promedio Mínimo" puede ser una fila de resumen; ignorar si no empieza por número
        //        if (!Regex.IsMatch(cols[0], @"^[\d\-\+]", RegexOptions.Compiled)) continue;

        //        // Tomar los primeros sensorsCount valores y parsearlos
        //        for (int c = 0; c < sensorsCount && c < cols.Length; c++)
        //        {
        //            try
        //            {
        //                // Normalizar coma decimal si la cultura usa coma
        //                var token = cols[c].Trim();
        //                // Algunos tokens pueden contener trailing commas or dots; limpiar
        //                token = token.TrimEnd(';', ',');
        //                double val = ParseDouble(token);
        //                dto.SeriesData[c].Add(val);
        //            }
        //            catch
        //            {
        //                // Si falla el parseo, intentar extraer número con regex
        //                var m = Regex.Match(cols[c], @"-?\d+([,\.]\d+)?");
        //                if (m.Success)
        //                {
        //                    var raw = m.Value;
        //                    // Reemplazar coma por el culture decimal separator if needed
        //                    if (culture.NumberFormat.NumberDecimalSeparator == ",")
        //                        raw = raw.Replace(".", "").Replace(",", ".");
        //                    else
        //                        raw = raw.Replace(",", "");
        //                    if (double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var fallback))
        //                        dto.SeriesData[c].Add(fallback);
        //                }
        //            }
        //        }
        //    }
        //}

        //return dto;
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

