using SignalAnalysis.Converters;
using SignalAnalysis.Helpers;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace SignalAnalysis.Models;

/// <summary>
/// Represents a data transfer object (DTO) for data in an elux file, containing metadata and other details.
/// </summary>
/// <remarks>This class is used to serialize and deserialize elux data, including information about the
/// document type, file version, and signal data.</remarks>
internal class EluxlDto
{
    // Campo para indicar la cultura detectada en el fichero de texto (ej: "es-ES")
    public string CultureName { get; set; } = "es-ES";

    public string DocumentType { get; set; } = string.Empty;
    public double FileVersion { get; set; }

    [JsonConverter(typeof(LocalizedDateTimeConverter))]
    public DateTime StartingTime { get; set; }

    [JsonConverter(typeof(LocalizedDateTimeConverter))]
    public DateTime EndingTime { get; set; }

    [JsonConverter(typeof(LocalizedTimeSpanConverter))]
    public TimeSpan Duration { get; set; }

    public int SensorNumber { get; set; }
    public int SeriesPoints { get; set; }
    public double SamplingFrequency { get; set; }

    public List<string> SeriesNames { get; set; } = new();
    public List<List<double>> SeriesData { get; set; } = new();

    // Helper: crea JsonSerializerOptions con la cultura del DTO y añade converters instanciados con esa cultura.
    public JsonSerializerOptions CreateJsonOptions()
    {
        var culture = GetCultureInfoOrDefault();
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new EluxlNamingPolicy(),
            WriteIndented = true
        };
        options.Converters.Add(new LocalizedDateTimeConverter(culture));
        options.Converters.Add(new LocalizedTimeSpanConverter(culture));
        return options;
    }

    private CultureInfo GetCultureInfoOrDefault()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(CultureName))
                return new CultureInfo(CultureName);
        }
        catch { /* ignore and fallback */ }
        return new CultureInfo("en-US");
    }

    // Parsea un fichero ErgoLux en memoria (líneas) y devuelve un DTO rellenado.
    // Asume que la primera línea contiene algo como "ErgoLux data (es-ES)" o similar.
    public static EluxlDto ParseFromErgoLuxText(string[] lines)
    {
        var dto = new EluxlDto();
        if (lines == null || lines.Length == 0) return dto;

        // Detectar cultura en la primera línea: buscar "(xx-XX)" o "es-ES"
        var first = lines[0].Trim();
        var cultureMatch = Regex.Match(first, @"\(([a-zA-Z\-]+)\)");
        if (cultureMatch.Success)
            dto.CultureName = cultureMatch.Groups[1].Value;

        var culture = new CultureInfo(dto.CultureName);

        // Función auxiliar para parsear double con la cultura detectada
        double ParseDouble(string s)
        {
            s = s.Trim();
            // Reemplazar posibles separadores de miles y normalizar
            // Dejar que double.Parse con culture haga el trabajo
            return double.Parse(s, NumberStyles.Float | NumberStyles.AllowThousands, culture);
        }

        // Recorrer líneas para metadatos hasta encontrar la tabla
        int i = 0;
        for (; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            // Cabeceras con formato "Clave: valor"
            if (line.Contains(":"))
            {
                var parts = line.Split(new[] { ':' }, 2);
                var key = parts[0].Trim();
                var val = parts[1].Trim();

                switch (key)
                {
                    case string k when k.StartsWith("Tiempo inicial", StringComparison.OrdinalIgnoreCase):
                        dto.StartingTime = TryParseDateTime(val, culture) ?? DateTime.MinValue;
                        break;
                    case string k when k.StartsWith("Tiempo final", StringComparison.OrdinalIgnoreCase):
                        dto.EndingTime = TryParseDateTime(val, culture) ?? DateTime.MinValue;
                        break;
                    case string k when k.StartsWith("Duración", StringComparison.OrdinalIgnoreCase) || k.StartsWith("Duracion", StringComparison.OrdinalIgnoreCase):
                        dto.Duration = TryParseDuration(val) ?? TimeSpan.Zero;
                        break;
                    case string k when k.StartsWith("Número de sensores", StringComparison.OrdinalIgnoreCase) || k.StartsWith("Numero de sensores", StringComparison.OrdinalIgnoreCase):
                        if (int.TryParse(val, NumberStyles.Integer, culture, out var sn)) dto.SensorNumber = sn;
                        break;
                    case string k when k.StartsWith("Número de puntos", StringComparison.OrdinalIgnoreCase) || k.StartsWith("Numero de puntos", StringComparison.OrdinalIgnoreCase):
                        if (int.TryParse(val, NumberStyles.Integer, culture, out var sp)) dto.SeriesPoints = sp;
                        break;
                    case string k when k.StartsWith("Frecuencia de muestreo", StringComparison.OrdinalIgnoreCase):
                        // puede venir como "2" o "2,0"
                        if (double.TryParse(val, NumberStyles.Float, culture, out var sf)) dto.SamplingFrequency = sf;
                        break;
                    default:
                        // Otros metadatos: DocumentType, FileVersion, etc.
                        if (i == 0) dto.DocumentType = line;
                        break;
                }
            }
            else
            {
                // Si la línea contiene "Sensor #00" es la cabecera de la tabla
                if (line.IndexOf("Sensor #00", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    line.IndexOf("Sensor #0", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    break; // i apunta a la cabecera de la tabla
                }
            }
        }

        // Si no encontramos la cabecera, intentar localizarla buscando "Sensor #"
        int headerIndex = i;
        if (headerIndex >= lines.Length)
        {
            for (int j = 0; j < lines.Length; j++)
            {
                if (lines[j].IndexOf("Sensor #", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    headerIndex = j;
                    break;
                }
            }
        }

        if (headerIndex < lines.Length)
        {
            // Leer nombres de series (cabecera)
            var headerLine = lines[headerIndex].Trim();
            // Separadores posibles: tabulador o múltiples espacios
            var headerCols = Regex.Split(headerLine, @"\t+|\s{2,}|\s+\|\s+|\s+").Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            // Tomar solo las columnas que empiezan por "Sensor" o "Sensor #"
            var sensorCols = headerCols.Where(h => h.StartsWith("Sensor", StringComparison.OrdinalIgnoreCase) || Regex.IsMatch(h, @"^Sensor\s*#\d+", RegexOptions.IgnoreCase)).ToArray();
            if (sensorCols.Length == 0)
            {
                // fallback: tomar las primeras N tokens
                sensorCols = headerCols.Take(dto.SensorNumber > 0 ? dto.SensorNumber : headerCols.Length).ToArray();
            }
            dto.SeriesNames = sensorCols.ToList();

            // Inicializar SeriesData con tantas listas como sensores detectados
            int sensorsCount = dto.SeriesNames.Count;
            dto.SeriesData = Enumerable.Range(0, sensorsCount).Select(_ => new List<double>()).ToList();

            // Leer filas de datos a partir de la siguiente línea
            for (int r = headerIndex + 1; r < lines.Length; r++)
            {
                var row = lines[r].Trim();
                if (string.IsNullOrEmpty(row)) continue;

                // Romper si la fila no parece numérica (por ejemplo, otra sección)
                // Intentar extraer números de la fila
                // Separadores: tabulador o múltiples espacios
                var cols = Regex.Split(row, @"\t+|\s{2,}").Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
                if (cols.Length == 0) continue;

                // Si la fila contiene texto como "Máximo Promedio Mínimo" puede ser una fila de resumen; ignorar si no empieza por número
                if (!Regex.IsMatch(cols[0], @"^[\d\-\+]", RegexOptions.Compiled)) continue;

                // Tomar los primeros sensorsCount valores y parsearlos
                for (int c = 0; c < sensorsCount && c < cols.Length; c++)
                {
                    try
                    {
                        // Normalizar coma decimal si la cultura usa coma
                        var token = cols[c].Trim();
                        // Algunos tokens pueden contener trailing commas or dots; limpiar
                        token = token.TrimEnd(';', ',');
                        double val = ParseDouble(token);
                        dto.SeriesData[c].Add(val);
                    }
                    catch
                    {
                        // Si falla el parseo, intentar extraer número con regex
                        var m = Regex.Match(cols[c], @"-?\d+([,\.]\d+)?");
                        if (m.Success)
                        {
                            var raw = m.Value;
                            // Reemplazar coma por el culture decimal separator if needed
                            if (culture.NumberFormat.NumberDecimalSeparator == ",")
                                raw = raw.Replace(".", "").Replace(",", ".");
                            else
                                raw = raw.Replace(",", "");
                            if (double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var fallback))
                                dto.SeriesData[c].Add(fallback);
                        }
                    }
                }
            }
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

