namespace SignalAnalysis;

public class SignalStats(double Max = 0, double Min = 0, double Avg = 0, double Var = 0, double FractalDim = 0, double FractalVar = 0, double AppEn = 0, double SampEn = 0, double ShannEn = 0, double BitEn = 0, double IdealEn = 0, double ShannonIdeal = 0, double IntegralValue = 0)
{
    public double Maximum { get; set; } = Max;
    public double Minimum { get; set; } = Min;
    public double Average { get; set; } = Avg;
    public double Variance { get; set; } = Var;
    public double BoxplotQ1 { get; set; } = 0;
    public double BoxplotQ2 { get; set; } = 0;
    public double BoxplotQ3 { get; set; } = 0;
    public double BoxplotMax { get; set; } = 0;
    public double BoxplotMin { get; set; } = 0;
    public double FractalDimension { get; set; } = FractalDim;
    public double FractalVariance { get; set; } = FractalVar;
    public double ApproximateEntropy { get; set; } = AppEn;
    public double SampleEntropy { get; set; } = SampEn;
    /// <summary>
    /// // The Shannon entropy (in bits per symbol) of the data
    /// </summary>
    public double ShannonEntropy { get; set; } = ShannEn;
    /// <summary>
    /// The minimum number of bits to encode the data
    /// </summary>
    public double EntropyBit { get; set; } = BitEn;
    /// <summary>
    /// The Shannon entropy assuming all data symbols are different. This is, in fact, the maximum Shannon entropy
    /// </summary>
    public double IdealEntropy { get; set; } = IdealEn;
    /// <summary>
    /// The ratio Shannon entropy / Ideal entropy
    /// </summary>
    public double ShannonIdeal { get; set; } = ShannonIdeal;

    public double[] Derivative { get; set; } = [];

    public double Integral { get; set; } = IntegralValue;

    public double[] FFTpower { get; set; } = [];
    public double[] FFTmagnitude { get; set; } = [];
    public double[] FFTfrequencies { get; set; } = [];

    /// <summary>
    /// Gets a text-formatted result string
    /// </summary>
    /// <param name="culture">Culture used to format the string</param>
    /// <param name="boxplot"><see langword="True"/> if the parameters defining the box plot are included in the string</param>
    /// <param name="entropy"><see langword="True"/> if the entropy variables are included in the string</param>
    /// <param name="integral"><see langword="True"/> if the integral value and the algorithm name are included in the string</param>
    /// <param name="integralAlgorithm"><see langword="True"/>Integration algorithm name</param>
    /// <returns>Results formatted as string</returns>
    public string ToString(System.Globalization.CultureInfo culture, bool boxplot = false, bool entropy = false, string entropyAlgorithm = "", int entropyM = 0, double entropyR = 0.0, bool integral = false, string integralAlgorithm = "")
    {
        string strTemp;

        strTemp = $"{StringResources.FileHeader07}{StringResources.FileHeaderColon}{Average.ToString("0.######", culture)}{Environment.NewLine}" +
        $"{StringResources.FileHeader32}{StringResources.FileHeaderColon}{Variance.ToString("0.######", culture)}{Environment.NewLine}" +
        $"{StringResources.FileHeader08}{StringResources.FileHeaderColon}{Maximum.ToString("0.##", culture)}{Environment.NewLine}" +
        $"{StringResources.FileHeader09}{StringResources.FileHeaderColon}{Minimum.ToString("0.##", culture)}{Environment.NewLine}";
        
        if (boxplot)
        {
            strTemp += $"{StringResources.FileHeader33}{StringResources.FileHeaderColon}{BoxplotMin.ToString("0.######", culture)}{Environment.NewLine}" +
                $"{StringResources.FileHeader35}{StringResources.FileHeaderColon}{BoxplotQ1.ToString("0.######", culture)}{Environment.NewLine}" +
                $"{StringResources.FileHeader36}{StringResources.FileHeaderColon}{BoxplotQ2.ToString("0.######", culture)}{Environment.NewLine}" +
                $"{StringResources.FileHeader37}{StringResources.FileHeaderColon}{BoxplotQ3.ToString("0.######", culture)}{Environment.NewLine}" +
                $"{StringResources.FileHeader34}{StringResources.FileHeaderColon}{BoxplotMax.ToString("0.######", culture)}{Environment.NewLine}";
        }

        strTemp += $"{StringResources.FileHeader10}{StringResources.FileHeaderColon}{FractalDimension.ToString("0.########", culture)}{Environment.NewLine}" +
        $"{StringResources.FileHeader11}{StringResources.FileHeaderColon}{FractalVariance.ToString("0.########", culture)}{Environment.NewLine}";

        if (entropy)
        {
            strTemp += $"{StringResources.FileHeader14}{StringResources.FileHeaderColon}{ShannonEntropy.ToString("0.########", culture)}{Environment.NewLine}" +
            $"{StringResources.FileHeader15}{StringResources.FileHeaderColon}{EntropyBit.ToString("0.########", culture)}{Environment.NewLine}" +
            $"{StringResources.FileHeader16}{StringResources.FileHeaderColon}{IdealEntropy.ToString("0.########", culture)}{Environment.NewLine}" +
            $"{StringResources.FileHeader38}{StringResources.FileHeaderColon}{ShannonIdeal.ToString("0.########", culture)}{Environment.NewLine}" +
            $"{StringResources.FileHeader39}{StringResources.FileHeaderColon}{entropyAlgorithm}{Environment.NewLine}" +
            $"{StringResources.FileHeader12} (m={entropyM}, r={entropyR.ToString("0.##", culture)}){StringResources.FileHeaderColon}{ApproximateEntropy.ToString("0.########", culture)}{Environment.NewLine}" +
            $"{StringResources.FileHeader13} (m={entropyM}, r={entropyR.ToString("0.##", culture)}){StringResources.FileHeaderColon}{SampleEntropy.ToString("0.########", culture)}{Environment.NewLine}";
        }
        
        if (integral)
        {
            strTemp += $"{StringResources.FileHeader30}{StringResources.FileHeaderColon}{integralAlgorithm}{Environment.NewLine}" +
                $"{StringResources.FileHeader31}{StringResources.FileHeaderColon}{Integral.ToString("0.########", culture)}";
        }

        return strTemp;
    }
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

    public double[][] Data = [];

    public string[] SeriesLabels = [];

    /// <summary>
    /// Array starting index
    /// </summary>
    public int IndexStart { get; set; } = 0;
    
    /// <summary>
    /// Array ending index
    /// </summary>
    public int IndexEnd { get; set; } = 0;
}