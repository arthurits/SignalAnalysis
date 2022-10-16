namespace SignalAnalysis;

/// <summary>
/// Stores all data that defines the signal
/// </summary>
public class ClassSignalData
{
    public string Name { get; set; } = String.Empty;

    public DateTime StartTime { get; set; } = DateTime.MinValue;

    public DateTime EndTime { get; set; } = DateTime.MinValue;

    public TimeSpan MeasuringTime { get; set; } = TimeSpan.Zero;

    public int SeriesNumber { get; set; } = 0;

    public int SeriesPoints { get; set; } = 0;

    public double SampleFrequency { get; set; } = 0;

    public double[][] Data { get; set; } = Array.Empty<double[]>();

    public string[] SeriesLabels { get; set; } = Array.Empty<string>();
}
