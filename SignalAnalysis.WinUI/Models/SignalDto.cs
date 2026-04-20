using ScottPlot.Plottables;
using SignalAnalysis.Converters;
using SignalAnalysis.Helpers;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using static SignalAnalysis.Helpers.MessageBox;

namespace SignalAnalysis.Models;

/// <summary>
/// Represents a data transfer object (DTO) for data in a signal file, containing metadata and other details.
/// </summary>
/// <remarks>This class is used to serialize and deserialize signal data, including information about the
/// document type, file version, and signal data.</remarks>
internal class SignalDto:DocumentBase
{
    //public int SeriesNumber { get; set; }
    //public int SeriesPoints { get; set; }
    //public double SamplingFrequency { get; set; }

    //public List<string> SeriesNames { get; set; } = [];
    //public List<List<double>> SignalData { get; set; } = [];

    /// <summary>
    /// Crea JsonSerializerOptions con la política de nombres y, opcionalmente,
    /// un converter para doubles que formatea según la cultura del DTO.
    /// </summary>
    public override JsonSerializerOptions CreateJsonOptions(bool serializeDoublesAsStrings = false)
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

        //// Detectar cultura en la primera línea: buscar "(xx-XX)"
        //var first = lines[0].Trim();
        //var cultureMatch = Regex.Match(first, @"\(([a-zA-Z\-]+)\)");
        //if (cultureMatch.Success)
        //    dto.CultureName = cultureMatch.Groups[1].Value;

        //var culture = dto.GetCultureInfoOrDefault();
        var primaryLanguage = Microsoft.Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride;

        int points = 0, series = 0;
        bool result = false;
        double sampleFreq;
        string[] seriesLabels;
        //string? strLine;

        try
        {
            if (lines[0] is null)
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile"), "StrOpenDataFile00".GetLocalized("ReadDataFile")));
            System.Globalization.CultureInfo fileCulture = new(lines[0][(lines[0].IndexOf('(') + 1)..^1]);
            if (!lines[0].Contains($"{"StrOpenDataFile01".GetLocalized("ReadDataFile", fileCulture) ?? "SignalAnalysis data"} (", StringComparison.Ordinal))
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile", fileCulture), "StrOpenDataFile01".GetLocalized("ReadDataFile", fileCulture)));
            dto.DocumentType = "sig";
            dto.CultureName = fileCulture.Name;

            // Number of series
            if (lines[1] is null)
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile"), "StrOpenDataFile17".GetLocalized("ReadDataFile")));
            if (!lines[1].Contains($"{"StrOpenDataFile17".GetLocalized("ReadDataFile", fileCulture) ?? "Number of series"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile"), "StrOpenDataFile17".GetLocalized("ReadDataFile")));
            if (!int.TryParse(lines[1][(lines[1].IndexOf(':') + 1)..], out series))
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile"), "StrOpenDataFile17".GetLocalized("ReadDataFile")));
            if (series <= 0)
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile"), "StrOpenDataFile17".GetLocalized("ReadDataFile")));
            dto.SeriesNumber = series;

            // Number of data points
            if (lines[2] is null)
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile"), "StrOpenDataFile05".GetLocalized("ReadDataFile")));
            if (!lines[2].Contains($"{"StrOpenDataFile05".GetLocalized("ReadDataFile", fileCulture) ?? "Number of data points"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile"), "StrOpenDataFile05".GetLocalized("ReadDataFile")));
            if (!int.TryParse(lines[2][(lines[2].IndexOf(':') + 1)..], out points))
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile"), "StrOpenDataFile05".GetLocalized("ReadDataFile")));
            if (points <= 0)
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile"), "StrOpenDataFile05".GetLocalized("ReadDataFile")));
            dto.SeriesPoints = points;

            // Sampling frequency
            if (lines[3] is null)
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile"), "StrOpenDataFile06".GetLocalized("ReadDataFile")));
            if (!lines[3].Contains($"{"StrOpenDataFile05".GetLocalized("ReadDataFile", fileCulture) ?? "Sampling frequency"}: ", StringComparison.Ordinal))
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile"), "StrOpenDataFile06".GetLocalized("ReadDataFile")));
            if (!double.TryParse(lines[3][(lines[3].IndexOf(':') + 1)..], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, fileCulture, out sampleFreq))
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile"), "StrOpenDataFile06".GetLocalized("ReadDataFile")));
            if (sampleFreq <= 0)
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile"), "StrOpenDataFile06".GetLocalized("ReadDataFile")));
            dto.SamplingFrequency = sampleFreq;

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

            // Empty line
            if (lines[4] is null)
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile", fileCulture), "StrOpenDataFile19".GetLocalized("ReadDataFile", fileCulture)));
            if (lines[4] != string.Empty)
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile", fileCulture), "StrOpenDataFile19".GetLocalized("ReadDataFile", fileCulture)));

            // Column header names
            if (lines[5] is null)
                throw new FormatException(string.Format("StrOpenDataFileSection".GetLocalized("ReadDataFile", fileCulture), "StrOpenDataFile20".GetLocalized("ReadDataFile", fileCulture)));
            seriesLabels = lines[5].Split('\t');
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



            //result = InitializeDataArrays(sr, ref signal.Data, series, points, fileCulture);

            //// Store information regarding the signal
            //signal.SeriesNumber = series;
            //signal.SeriesPoints = points;
            //signal.SampleFrequency = sampleFreq;
            //signal.SeriesLabels = seriesLabels;
            //signal.IndexStart = 0;
            //signal.IndexEnd = signal.SeriesPoints - 1;
            //signal.MeasuringTime = new(signal.StartTime.AddSeconds((signal.SeriesPoints - 1) / signal.SampleFrequency).Ticks);
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




        //// Helper para parsear double con la cultura detectada
        //double ParseDouble(string s)
        //{
        //    s = s.Trim();
        //    // Normalizar tokens comunes
        //    if (double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, culture, out var d))
        //        return d;
        //    // Fallback invariant
        //    if (double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out d))
        //        return d;
        //    // Intentar extraer número con regex
        //    var m = Regex.Match(s, @"-?\d+([,\.]\d+)?");
        //    if (m.Success)
        //    {
        //        var raw = m.Value;
        //        // Ajustar según culture decimal separator
        //        if (culture.NumberFormat.NumberDecimalSeparator == ",")
        //            raw = raw.Replace(".", "").Replace(",", ".");
        //        else
        //            raw = raw.Replace(",", "");
        //        if (double.TryParse(raw, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out d))
        //            return d;
        //    }
        //    throw new FormatException($"No se pudo parsear el número '{s}' con la cultura '{culture.Name}'.");
        //}

        //// Recorrer líneas para metadatos hasta encontrar la cabecera de series
        //int i = 0;
        //for (; i < lines.Length; i++)
        //{
        //    var line = lines[i].Trim();
        //    if (string.IsNullOrEmpty(line)) continue;

        //    if (line.Contains(":"))
        //    {
        //        var parts = line.Split(new[] { ':' }, 2);
        //        var key = parts[0].Trim();
        //        var val = parts[1].Trim();

        //        if (key.StartsWith("Número de series", StringComparison.OrdinalIgnoreCase) ||
        //            key.StartsWith("Numero de series", StringComparison.OrdinalIgnoreCase))
        //        {
        //            if (int.TryParse(val, NumberStyles.Integer, culture, out var n)) dto.SeriesNumber = n;
        //        }
        //        else if (key.StartsWith("Número de puntos", StringComparison.OrdinalIgnoreCase) ||
        //                 key.StartsWith("Numero de puntos", StringComparison.OrdinalIgnoreCase))
        //        {
        //            if (int.TryParse(val, NumberStyles.Integer, culture, out var p)) dto.SeriesPoints = p;
        //        }
        //        else if (key.StartsWith("Frecuencia de muestreo", StringComparison.OrdinalIgnoreCase))
        //        {
        //            if (double.TryParse(val, NumberStyles.Float | NumberStyles.AllowThousands, culture, out var f)) dto.SamplingFrequency = f;
        //        }
        //        else
        //        {
        //            // DocumentType en la primera línea
        //            if (i == 0) dto.DocumentType = line;
        //        }
        //    }
        //    else
        //    {
        //        // Si la línea no contiene ":" y parece la cabecera de series (contiene texto y no números)
        //        // asumimos que es la línea de nombres de series
        //        // Ejemplo: "Seno\tSeno2x\tTotal"
        //        // Rompemos y la procesamos fuera del bucle
        //        break;
        //    }
        //}

        //// headerIndex apunta a la línea de nombres de series
        //int headerIndex = i;
        //if (headerIndex >= lines.Length)
        //{
        //    // intentar buscar la primera línea que contenga letras y separadores
        //    for (int j = 0; j < lines.Length; j++)
        //    {
        //        if (Regex.IsMatch(lines[j], @"[A-Za-zÁÉÍÓÚáéíóúÑñ]") && lines[j].IndexOfAny(new[] { '\t', ' ' }) >= 0)
        //        {
        //            headerIndex = j;
        //            break;
        //        }
        //    }
        //}

        //if (headerIndex < lines.Length)
        //{
        //    var headerLine = lines[headerIndex].Trim();
        //    // Separadores posibles: tabulador o múltiples espacios
        //    var headerCols = Regex.Split(headerLine, @"\t+|\s{2,}").Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
        //    dto.SeriesNames = headerCols.ToList();

        //    int sensorsCount = dto.SeriesNames.Count;
        //    dto.SeriesData = Enumerable.Range(0, sensorsCount).Select(_ => new List<double>()).ToList();

        //    // Leer filas de datos a partir de la siguiente línea
        //    for (int r = headerIndex + 1; r < lines.Length; r++)
        //    {
        //        var row = lines[r].Trim();
        //        if (string.IsNullOrEmpty(row)) continue;

        //        // Separar columnas por tabulador o múltiples espacios
        //        var cols = Regex.Split(row, @"\t+|\s{2,}").Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
        //        if (cols.Length == 0) continue;

        //        // Si la fila no empieza por número, saltarla
        //        if (!Regex.IsMatch(cols[0], @"^[\d\-\+]", RegexOptions.Compiled)) continue;

        //        // Tomar los primeros sensorsCount valores y parsearlos
        //        for (int c = 0; c < sensorsCount; c++)
        //        {
        //            if (c >= cols.Length) break;
        //            var token = cols[c].Trim();
        //            try
        //            {
        //                var val = ParseDouble(token);
        //                dto.SeriesData[c].Add(val);
        //            }
        //            catch
        //            {
        //                // intentar extraer número con regex y parsear
        //                var m = Regex.Match(token, @"-?\d+([,\.]\d+)?");
        //                if (m.Success)
        //                {
        //                    var raw = m.Value;
        //                    if (culture.NumberFormat.NumberDecimalSeparator == ",")
        //                        raw = raw.Replace(".", "").Replace(",", ".");
        //                    else
        //                        raw = raw.Replace(",", "");
        //                    if (double.TryParse(raw, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var fallback))
        //                        dto.SeriesData[c].Add(fallback);
        //                }
        //                else
        //                {
        //                    // Si no hay número, añadir NaN para mantener la alineación
        //                    dto.SeriesData[c].Add(double.NaN);
        //                }
        //            }
        //        }
        //    }
        //}

        //// Validaciones básicas
        //if (dto.SeriesNumber > 0 && dto.SeriesNames.Count != dto.SeriesNumber)
        //{
        //    // ajustar SeriesNumber si no coincide
        //    dto.SeriesNumber = dto.SeriesNames.Count;
        //}

        //if (dto.SeriesPoints > 0)
        //{
        //    // comprobar que el número de puntos coincide con las filas leídas
        //    var actualPoints = dto.SeriesData.Count > 0 ? dto.SeriesData[0].Count : 0;
        //    if (dto.SeriesPoints != actualPoints)
        //    {
        //        // actualizar SeriesPoints para reflejar lo leído
        //        dto.SeriesPoints = actualPoints;
        //    }
        //}
        //else
        //{
        //    dto.SeriesPoints = dto.SeriesData.Count > 0 ? dto.SeriesData[0].Count : 0;
        //}

        return dto;
    }
}