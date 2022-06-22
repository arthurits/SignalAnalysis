namespace SignalAnalysis;


public class Stats
{
    public Stats(double Max = 0, double Min = 0, double Avg = 0, double FractalDim = 0, double FractalVar = 0, double AppEn = 0, double SampEn = 0, double ShannEn = 0, double BitEn = 0, double IdealEn = 0)
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

    public double Maximum { get; set; }
    public double Minimum { get; set; }
    public double Average { get; set; }
    public double FractalDimension { get; set; }
    public double FractalVariance { get; set; }
    public double ApproximateEntropy { get; set; }
    public double SampleEntropy { get; set; }
    public double ShannonEntropy { get; set; }      // The Shannon entropy value of a message
    public double EntropyBit { get; set; }          // The entropy bit value of a message
    public double IdealEntropy { get; set; }        // The ideal entropy value of a message

    public double[] FFTpower { get; set; }
    public double[] FFTmagnitude { get; set; }
    public double[] FFTfrequencies { get; set; }

    public string ToString(System.Resources.ResourceManager stringsRM, System.Globalization.CultureInfo culture) =>
        (stringsRM.GetString("strFileHeader07", culture) ?? "Average") + ": " + Average.ToString("0.######", culture) + Environment.NewLine +
        (stringsRM.GetString("strFileHeader08", culture) ?? "Maximum") + ": " + Maximum.ToString("0.##", culture) + Environment.NewLine +
        (stringsRM.GetString("strFileHeader09", culture) ?? "Minimum") + ": " + Minimum.ToString("0.##", culture) + Environment.NewLine +
        (stringsRM.GetString("strFileHeader10", culture) ?? "Fractal dimension") + ": " + FractalDimension.ToString("0.########", culture) + Environment.NewLine +
        (stringsRM.GetString("strFileHeader11", culture) ?? "Fractal variance") + ": " + FractalVariance.ToString("0.########", culture) + Environment.NewLine +
        (stringsRM.GetString("strFileHeader12", culture) ?? "Approximate entropy") + ": " + ApproximateEntropy.ToString("0.########", culture) + Environment.NewLine +
        (stringsRM.GetString("strFileHeader13", culture) ?? "Sample entropy") + ": " + SampleEntropy.ToString("0.########", culture) + Environment.NewLine +
        (stringsRM.GetString("strFileHeader14", culture) ?? "Shannon entropy") + ": " + ShannonEntropy.ToString("0.########", culture) + Environment.NewLine +
        (stringsRM.GetString("strFileHeader15", culture) ?? "Entropy bit") + ": " + EntropyBit.ToString("0.########", culture) + Environment.NewLine +
        (stringsRM.GetString("strFileHeader16", culture) ?? "Ideal entropy") + ": " + IdealEntropy.ToString("0.########", culture);
}

