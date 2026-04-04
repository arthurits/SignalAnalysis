using System.Globalization;
using System.Text.Json;

namespace SignalAnalysis.Models;

#region Document base and exception

/// <summary>
/// Clase base común a todos los DTOs de documentos.
/// Contiene las tres propiedades obligatorias: CultureName, DocumentType, FileVersion.
/// </summary>
public abstract class DocumentBase
{
    /// <summary>
    /// Nombre de la cultura detectada en el fichero (ej: "es-ES").
    /// Debe existir en todos los documentos.
    /// </summary>
    public string CultureName { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de documento (ej: "ErgoLux data", "SignalAnalysis data" o un identificador corto "elux"/"sig").
    /// Debe existir en todos los documentos.
    /// </summary>
    public string DocumentType { get; set; } = string.Empty;

    /// <summary>
    /// Versión del fichero. Debe existir en todos los documentos.
    /// </summary>
    public double FileVersion { get; set; }

    // Series data properties are common to both DTOs, so they can be defined here.
    public int SeriesNumber { get; set; }
    public int SeriesPoints { get; set; }
    public double SamplingFrequency { get; set; }
    public List<string> SeriesNames { get; set; } = [];
    public List<List<double>> SeriesData { get; set; } = [];

    /// <summary>
    /// Array starting index
    /// </summary>
    public int IndexStart { get; set; } = 0;

    /// <summary>
    /// Array ending index
    /// </summary>
    public int IndexEnd { get; set; } = 0;

    /// <summary>
    /// Each concrete DTO must implement this method to create JsonSerializerOptions with the appropriate naming policy and converters based on the culture.
    /// </summary>
    public abstract JsonSerializerOptions CreateJsonOptions(bool serializeDoublesAsStrings = false);
}

/// <summary>
/// Excepción lanzada cuando el documento no cumple los requisitos mínimos.
/// </summary>
public class DocumentFormatException : Exception
{
    public DocumentFormatException(string message) : base(message) { }
}

#endregion

#region DocumentFactory

/// <summary>
/// Fábrica responsable de detectar el tipo de documento y serializar/deserializar de forma segura.
/// </summary>
public static class DocumentFactory
{
    /// <summary>
    /// Detecta el tipo de documento a partir de las líneas de texto del fichero y llama al parser correspondiente.
    /// Lanza DocumentFormatException si faltan las propiedades obligatorias tras el parseo.
    /// </summary>
    public static DocumentBase ParseFromText(string[] lines)
    {
        if (lines == null || lines.Length == 0)
            throw new DocumentFormatException("El contenido del fichero está vacío.");

        // Primera línea contiene el identificador y la cultura entre paréntesis, por ejemplo:
        // "ErgoLux data (es-ES)" o "SignalAnalysis data (es-ES)"
        var first = lines[0].Trim();

        if (first.StartsWith("ErgoLux", StringComparison.OrdinalIgnoreCase) ||
            first.Contains("elux", StringComparison.OrdinalIgnoreCase))
        {
            var dto = EluxlDto.ParseFromErgoLuxText(lines);
            ValidateMandatoryHeader(dto);
            return dto;
        }

        if (first.StartsWith("SignalAnalysis", StringComparison.OrdinalIgnoreCase) ||
            first.Contains("sig", StringComparison.OrdinalIgnoreCase))
        {
            var dto = SignalDto.ParseFromSigText(lines);
            ValidateMandatoryHeader(dto);
            return dto;
        }

        throw new DocumentFormatException($"Tipo de documento no reconocido en la primera línea: '{first}'.");
    }

    /// <summary>
    /// Deserializa JSON detectando primero el DocumentType y CultureName sin deserializar todo el objeto.
    /// </summary>
    public static DocumentBase DeserializeFromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new DocumentFormatException("JSON vacío.");

        // Leer el JSON mínimo para extraer los campos obligatorios
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        // Comprobar existencia de los campos obligatorios
        if (!root.TryGetProperty("cultureName", out var cultureProp) &&
            !root.TryGetProperty("CultureName", out cultureProp))
            throw new DocumentFormatException("Falta la propiedad obligatoria 'CultureName' en el JSON.");

        if (!root.TryGetProperty("documentType", out var typeProp) &&
            !root.TryGetProperty("DocumentType", out typeProp))
            throw new DocumentFormatException("Falta la propiedad obligatoria 'DocumentType' en el JSON.");

        if (!root.TryGetProperty("fileVersion", out var versionProp) &&
            !root.TryGetProperty("FileVersion", out versionProp))
            throw new DocumentFormatException("Falta la propiedad obligatoria 'FileVersion' en el JSON.");

        var cultureName = cultureProp.GetString() ?? throw new DocumentFormatException("CultureName vacío.");
        var documentType = typeProp.GetString() ?? throw new DocumentFormatException("DocumentType vacío.");
        // fileVersion puede ser número o string; intentar leer como número
        double fileVersion = versionProp.ValueKind == JsonValueKind.Number
            ? versionProp.GetDouble()
            : double.TryParse(versionProp.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var v) ? v : throw new DocumentFormatException("FileVersion inválido.");

        // Normalizar documentType para decidir el DTO
        var docTypeLower = documentType.Trim().ToLowerInvariant();

        // Crear opciones con la cultura detectada
        var culture = TryCreateCulture(cultureName);

        if (docTypeLower.Contains("ergo") || docTypeLower.Contains("elux"))
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            // Añadir converters parametrizados por la cultura (asumiendo que EluxlDto.CreateJsonOptions hace lo mismo)
            options = new EluxlDto { CultureName = cultureName }.CreateJsonOptions();
            var dto = JsonSerializer.Deserialize<EluxlDto>(json, options) ?? throw new DocumentFormatException("Deserialización a EluxlDto fallida.");
            ValidateMandatoryHeader(dto);
            return dto;
        }

        if (docTypeLower.Contains("signal") || docTypeLower.Contains("sig"))
        {
            var options = new SignalDto { CultureName = cultureName }.CreateJsonOptions();
            var dto = JsonSerializer.Deserialize<SignalDto>(json, options) ?? throw new DocumentFormatException("Deserialización a SignalDto fallida.");
            ValidateMandatoryHeader(dto);
            return dto;
        }

        throw new DocumentFormatException($"DocumentType no soportado: '{documentType}'.");
    }

    /// <summary>
    /// Serializa un DTO concreto a JSON validando primero que las propiedades obligatorias existan.
    /// </summary>
    public static string SerializeToJson(DocumentBase dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));
        ValidateMandatoryHeader(dto);

        var options = dto.CreateJsonOptions();
        return JsonSerializer.Serialize(dto, dto.GetType(), options);
    }

    private static void ValidateMandatoryHeader(DocumentBase dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));
        if (string.IsNullOrWhiteSpace(dto.CultureName))
            throw new DocumentFormatException("Falta la propiedad obligatoria 'CultureName'.");
        if (string.IsNullOrWhiteSpace(dto.DocumentType))
            throw new DocumentFormatException("Falta la propiedad obligatoria 'DocumentType'.");
        // FileVersion puede ser 0 en algunos formatos; si quieres exigir >0, cambia la condición
        if (double.IsNaN(dto.FileVersion))
            throw new DocumentFormatException("Falta o es inválida la propiedad 'FileVersion'.");
    }

    private static CultureInfo TryCreateCulture(string cultureName)
    {
        try
        {
            return new CultureInfo(cultureName);
        }
        catch
        {
            return CultureInfo.InvariantCulture;
        }
    }
}

#endregion
