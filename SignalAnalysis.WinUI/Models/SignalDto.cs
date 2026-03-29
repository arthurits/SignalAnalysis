using SignalAnalysis.Converters;
using SignalAnalysis.Helpers;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SignalAnalysis.Models;

/// <summary>
/// Represents a data transfer object (DTO) for data in a signal file, containing metadata and other details.
/// </summary>
/// <remarks>This class is used to serialize and deserialize signal data, including information about the
/// document type, file version, and signal data.</remarks>
internal class SignalDto
{
    // Cultura detectada en la primera línea del fichero, por ejemplo "es-ES"
    public string CultureName { get; set; } = "es-ES";

    public string DocumentType { get; set; } = string.Empty;
    public double FileVersion { get; set; }

    public int SeriesNumber { get; set; }
    public int SeriesPoints { get; set; }
    public double SamplingFrequency { get; set; }

    public List<string> SeriesNames { get; set; } = new();
    public List<List<double>> SignalData { get; set; } = new();

    /// <summary>
    /// Crea JsonSerializerOptions con la política de nombres y, opcionalmente,
    /// un converter para doubles que formatea según la cultura del DTO.
    /// </summary>
    public JsonSerializerOptions CreateJsonOptions(bool serializeDoublesAsStrings = false)
    {
        var culture = GetCultureInfoOrDefault();
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new SignalNamingPolicy(),
            WriteIndented = true
        };
        if (serializeDoublesAsStrings)
            options.Converters.Add(new LocalizedDoubleStringConverter(culture));
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
        return CultureInfo.InvariantCulture;
    }

    /// <summary>
    /// Parsea el contenido de un archivo .sig dado como array de líneas y devuelve un DTO rellenado.
    /// Maneja separador decimal según la cultura detectada en la primera línea.
    /// </summary>
    public static SignalDto ParseFromSigText(string[] lines)
    {
        var dto = new SignalDto();
        if (lines == null || lines.Length == 0) return dto;

        // Detectar cultura en la primera línea: buscar "(xx-XX)"
        var first = lines[0].Trim();
        var cultureMatch = Regex.Match(first, @"\(([a-zA-Z\-]+)\)");
        if (cultureMatch.Success)
            dto.CultureName = cultureMatch.Groups[1].Value;

        var culture = dto.GetCultureInfoOrDefault();

        // Helper para parsear double con la cultura detectada
        double ParseDouble(string s)
        {
            s = s.Trim();
            // Normalizar tokens comunes
            if (double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, culture, out var d))
                return d;
            // Fallback invariant
            if (double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out d))
                return d;
            // Intentar extraer número con regex
            var m = Regex.Match(s, @"-?\d+([,\.]\d+)?");
            if (m.Success)
            {
                var raw = m.Value;
                // Ajustar según culture decimal separator
                if (culture.NumberFormat.NumberDecimalSeparator == ",")
                    raw = raw.Replace(".", "").Replace(",", ".");
                else
                    raw = raw.Replace(",", "");
                if (double.TryParse(raw, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out d))
                    return d;
            }
            throw new FormatException($"No se pudo parsear el número '{s}' con la cultura '{culture.Name}'.");
        }

        // Recorrer líneas para metadatos hasta encontrar la cabecera de series
        int i = 0;
        for (; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            if (line.Contains(":"))
            {
                var parts = line.Split(new[] { ':' }, 2);
                var key = parts[0].Trim();
                var val = parts[1].Trim();

                if (key.StartsWith("Número de series", StringComparison.OrdinalIgnoreCase) ||
                    key.StartsWith("Numero de series", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(val, NumberStyles.Integer, culture, out var n)) dto.SeriesNumber = n;
                }
                else if (key.StartsWith("Número de puntos", StringComparison.OrdinalIgnoreCase) ||
                         key.StartsWith("Numero de puntos", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(val, NumberStyles.Integer, culture, out var p)) dto.SeriesPoints = p;
                }
                else if (key.StartsWith("Frecuencia de muestreo", StringComparison.OrdinalIgnoreCase))
                {
                    if (double.TryParse(val, NumberStyles.Float | NumberStyles.AllowThousands, culture, out var f)) dto.SamplingFrequency = f;
                }
                else
                {
                    // DocumentType en la primera línea
                    if (i == 0) dto.DocumentType = line;
                }
            }
            else
            {
                // Si la línea no contiene ":" y parece la cabecera de series (contiene texto y no números)
                // asumimos que es la línea de nombres de series
                // Ejemplo: "Seno\tSeno2x\tTotal"
                // Rompemos y la procesamos fuera del bucle
                break;
            }
        }

        // headerIndex apunta a la línea de nombres de series
        int headerIndex = i;
        if (headerIndex >= lines.Length)
        {
            // intentar buscar la primera línea que contenga letras y separadores
            for (int j = 0; j < lines.Length; j++)
            {
                if (Regex.IsMatch(lines[j], @"[A-Za-zÁÉÍÓÚáéíóúÑñ]") && lines[j].IndexOfAny(new[] { '\t', ' ' }) >= 0)
                {
                    headerIndex = j;
                    break;
                }
            }
        }

        if (headerIndex < lines.Length)
        {
            var headerLine = lines[headerIndex].Trim();
            // Separadores posibles: tabulador o múltiples espacios
            var headerCols = Regex.Split(headerLine, @"\t+|\s{2,}").Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            dto.SeriesNames = headerCols.ToList();

            int sensorsCount = dto.SeriesNames.Count;
            dto.SignalData = Enumerable.Range(0, sensorsCount).Select(_ => new List<double>()).ToList();

            // Leer filas de datos a partir de la siguiente línea
            for (int r = headerIndex + 1; r < lines.Length; r++)
            {
                var row = lines[r].Trim();
                if (string.IsNullOrEmpty(row)) continue;

                // Separar columnas por tabulador o múltiples espacios
                var cols = Regex.Split(row, @"\t+|\s{2,}").Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
                if (cols.Length == 0) continue;

                // Si la fila no empieza por número, saltarla
                if (!Regex.IsMatch(cols[0], @"^[\d\-\+]", RegexOptions.Compiled)) continue;

                // Tomar los primeros sensorsCount valores y parsearlos
                for (int c = 0; c < sensorsCount; c++)
                {
                    if (c >= cols.Length) break;
                    var token = cols[c].Trim();
                    try
                    {
                        var val = ParseDouble(token);
                        dto.SignalData[c].Add(val);
                    }
                    catch
                    {
                        // intentar extraer número con regex y parsear
                        var m = Regex.Match(token, @"-?\d+([,\.]\d+)?");
                        if (m.Success)
                        {
                            var raw = m.Value;
                            if (culture.NumberFormat.NumberDecimalSeparator == ",")
                                raw = raw.Replace(".", "").Replace(",", ".");
                            else
                                raw = raw.Replace(",", "");
                            if (double.TryParse(raw, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var fallback))
                                dto.SignalData[c].Add(fallback);
                        }
                        else
                        {
                            // Si no hay número, añadir NaN para mantener la alineación
                            dto.SignalData[c].Add(double.NaN);
                        }
                    }
                }
            }
        }

        // Validaciones básicas
        if (dto.SeriesNumber > 0 && dto.SeriesNames.Count != dto.SeriesNumber)
        {
            // ajustar SeriesNumber si no coincide
            dto.SeriesNumber = dto.SeriesNames.Count;
        }

        if (dto.SeriesPoints > 0)
        {
            // comprobar que el número de puntos coincide con las filas leídas
            var actualPoints = dto.SignalData.Count > 0 ? dto.SignalData[0].Count : 0;
            if (dto.SeriesPoints != actualPoints)
            {
                // actualizar SeriesPoints para reflejar lo leído
                dto.SeriesPoints = actualPoints;
            }
        }
        else
        {
            dto.SeriesPoints = dto.SignalData.Count > 0 ? dto.SignalData[0].Count : 0;
        }

        return dto;
    }
}