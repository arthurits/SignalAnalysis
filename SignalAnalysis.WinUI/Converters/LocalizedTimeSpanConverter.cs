using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace SignalAnalysis.Converters;

internal sealed class LocalizedTimeSpanConverter : JsonConverter<TimeSpan>
{
    private readonly CultureInfo _culture;
    private static readonly Regex _nums = new(@"\d+", RegexOptions.Compiled);

    public LocalizedTimeSpanConverter(CultureInfo culture)
    {
        _culture = culture ?? CultureInfo.InvariantCulture;
    }

    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var s = reader.GetString() ?? throw new JsonException("Expected duration string.");
        // Extract integers in order: días, horas, minutos, segundos, milisegundos (if present)
        var matches = _nums.Matches(s).Cast<Match>().Select(m => int.Parse(m.Value, CultureInfo.InvariantCulture)).ToArray();
        int days = matches.Length > 0 ? matches[0] : 0;
        int hours = matches.Length > 1 ? matches[1] : 0;
        int minutes = matches.Length > 2 ? matches[2] : 0;
        int seconds = matches.Length > 3 ? matches[3] : 0;
        int milliseconds = matches.Length > 4 ? matches[4] : 0;
        // If the file used a different ordering, this simple heuristic still works for the example format.
        return new TimeSpan(days, hours, minutes, seconds, milliseconds);
    }

    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        // Recreate the same style as the example: "X días, Y horas, Z minutos, W segundos, and V milisegundos"
        var s = $"{value.Days} días, {value.Hours} horas, {value.Minutes} minutos, {value.Seconds} segundos, and {value.Milliseconds} milisegundos";
        writer.WriteStringValue(s);
    }
}
