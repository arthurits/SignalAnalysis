using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SignalAnalysis.Converters;

/// <summary>
/// Converter opcional: serializa/deserializa double como string usando la cultura indicada.
/// Útil si quieres que el JSON guarde los números con coma decimal para la cultura.
/// Si prefieres números JSON nativos, no lo añadas a JsonSerializerOptions.
/// </summary>
internal sealed class LocalizedDoubleStringConverter : JsonConverter<double>
{
    private readonly CultureInfo _culture;
    public LocalizedDoubleStringConverter(CultureInfo culture) => _culture = culture ?? CultureInfo.InvariantCulture;

    public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
            return reader.GetDouble();
        if (reader.TokenType == JsonTokenType.String)
        {
            var s = reader.GetString()!;
            if (double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, _culture, out var d))
                return d;
            if (double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out d))
                return d;
        }
        throw new JsonException("Unable to parse double value.");
    }

    public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
    {
        // Escribe como string formateado con la cultura
        var s = value.ToString("G", _culture);
        writer.WriteStringValue(s);
    }
}
