namespace SignalAnalysis.Models;

/// <summary>
/// Represents a data transfer object (DTO) for data in a signal file, containing metadata and other details.
/// </summary>
/// <remarks>This class is used to serialize and deserialize signal data, including information about the
/// document type, file version, and signal data.</remarks>
internal class SignalDto
{
    public string DocumentType { get; set; } = string.Empty;
    public double FileVersion { get; set; }
    public int SeriesNumber { get; set; }
    public int SeriesPoints { get; set; }
    public double SamplingFrequency { get; set; }
    public List<List<string>> SignalNames { get; set; } = [];
    public List<List<double>> SignalData{ get; set; } = [];
}
