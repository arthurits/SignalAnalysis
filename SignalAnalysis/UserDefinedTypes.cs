using System.Text.Json.Serialization;

namespace SignalAnalysis;


public class SignalStats
{
    public SignalStats(double Max = 0, double Min = 0, double Avg = 0, double FractalDim = 0, double FractalVar = 0, double AppEn = 0, double SampEn = 0, double ShannEn = 0, double BitEn = 0, double IdealEn = 0)
    {
        Maximum = Max;
        Minimum = Min;
        Average = Avg;
        FractalDimension = FractalDim;
        FractalVariance = FractalVar;
        ApproximateEntropy = AppEn;
        SampleEntropy = SampEn;
        ShannonEntropy = ShannEn;
        EntropyBit = BitEn;
        IdealEntropy = IdealEn;
    }

    public double Maximum { get; set; } = 0;
    public double Minimum { get; set; } = 0;
    public double Average { get; set; } = 0;
    public double FractalDimension { get; set; } = 0;
    public double FractalVariance { get; set; } = 0;
    public double ApproximateEntropy { get; set; } = 0;
    public double SampleEntropy { get; set; } = 0;
    public double ShannonEntropy { get; set; } = 0; // The Shannon entropy value of a message
    public double EntropyBit { get; set; } = 0;     // The entropy bit value of a message
    public double IdealEntropy { get; set; } = 0;   // The ideal entropy value of a message

    public double[] Derivative { get; set; } = Array.Empty<double>();

    public double Integral { get; set; } = -1;

    public double[] FFTpower { get; set; } = Array.Empty<double>();
    public double[] FFTmagnitude { get; set; } = Array.Empty<double>();
    public double[] FFTfrequencies { get; set; } = Array.Empty<double>();

    public string ToString(System.Globalization.CultureInfo culture, bool integral = false) =>
        StringResources.FileHeader07 + ": " + Average.ToString("0.######", culture) + Environment.NewLine +
        StringResources.FileHeader08 + ": " + Maximum.ToString("0.##", culture) + Environment.NewLine +
        StringResources.FileHeader09 + ": " + Minimum.ToString("0.##", culture) + Environment.NewLine +
        StringResources.FileHeader10 + ": " + FractalDimension.ToString("0.########", culture) + Environment.NewLine +
        StringResources.FileHeader11 + ": " + FractalVariance.ToString("0.########", culture) + Environment.NewLine +
        StringResources.FileHeader12 + ": " + ApproximateEntropy.ToString("0.########", culture) + Environment.NewLine +
        StringResources.FileHeader13 + ": " + SampleEntropy.ToString("0.########", culture) + Environment.NewLine +
        StringResources.FileHeader14 + ": " + ShannonEntropy.ToString("0.########", culture) + Environment.NewLine +
        StringResources.FileHeader15 + ": " + EntropyBit.ToString("0.########", culture) + Environment.NewLine +
        StringResources.FileHeader16 + ": " + IdealEntropy.ToString("0.########", culture) +
        $"{(integral ? Environment.NewLine + StringResources.FileHeader31 + ": " + Integral.ToString("0.########", culture) : string.Empty)}";
}

/// <summary>
/// Stores all data that defines the signal
/// </summary>
public class SignalData
{
    public DateTime StartTime { get; set; } = DateTime.MinValue;

    public DateTime EndTime { get; set; } = DateTime.MinValue;

    public TimeSpan MeasuringTime { get; set; } = TimeSpan.Zero;

    public int SeriesNumber { get; set; } = 0;

    public int SeriesPoints { get; set; } = 0;

    public double SampleFrequency { get; set; } = 0;

    public double[][] Data = Array.Empty<double[]>();

    public string[] SeriesLabels = Array.Empty<string>();

    /// <summary>
    /// Array starting index
    /// </summary>
    public int IndexStart { get; set; } = 0;
    
    /// <summary>
    /// Array ending index
    /// </summary>
    public int IndexEnd { get; set; } = 0;
}