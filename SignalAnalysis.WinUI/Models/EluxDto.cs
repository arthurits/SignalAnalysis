namespace SignalAnalysis.Models;

/// <summary>
/// Represents a data transfer object (DTO) for data in an elux file, containing metadata and other details.
/// </summary>
/// <remarks>This class is used to serialize and deserialize elux data, including information about the
/// document type, file version, and signal data.</remarks>
internal class EluxlDto
{
    public string DocumentType { get; set; } = string.Empty;
    public double FileVersion { get; set; }
    public string StartingTime { get; set; } = string.Empty;
    public string EndingTime { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public int SensorNumber { get; set; }
    public int SeriesPoints { get; set; }
    public double SamplingFrequency { get; set; }
    public List<List<string>> SeriesNames { get; set; } = [];
    public List<List<double>> SeriesData { get; set; } = [];
}

