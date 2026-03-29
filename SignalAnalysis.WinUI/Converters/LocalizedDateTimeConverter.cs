using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SignalAnalysis.Converters;

internal sealed class LocalizedDateTimeConverter : JsonConverter<DateTime>
{
    private readonly CultureInfo _culture;
    private readonly string[] _formats;

    public LocalizedDateTimeConverter(CultureInfo culture, params string[] formats)
    {
        _culture = culture ?? CultureInfo.InvariantCulture;
        _formats = (formats != null && formats.Length > 0)
            ? formats
            : new[] { "dddd, d 'de' MMMM 'de' yyyy HH:mm:ss,fff", "d/M/yyyy H:mm:ss,fff", "yyyy-MM-ddTHH:mm:ss.fff", "o" };
    }

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var s = reader.GetString() ?? throw new JsonException("Expected date string.");
        // Try exact formats with provided culture
        if (DateTime.TryParseExact(s, _formats, _culture, DateTimeStyles.None, out var dt))
            return dt;
        // Try general parse with culture
        if (DateTime.TryParse(s, _culture, DateTimeStyles.None, out dt))
            return dt;
        // Fallback to invariant parse
        if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out dt))
            return dt;
        throw new JsonException($"Unable to parse DateTime: '{s}'.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        // Prefer the verbose localized format if culture supports month/day names
        string formatted;
        try
        {
            formatted = value.ToString(_formats[0], _culture);
        }
        catch
        {
            formatted = value.ToString("o", CultureInfo.InvariantCulture);
        }
        writer.WriteStringValue(formatted);
    }
}
