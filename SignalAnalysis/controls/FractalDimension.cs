namespace SignalAnalysis;

// This class implements the computation of the fractal dimension of a discrete curve according to 
// Carlos Sevcik's "A procedure to estimate the fractal dimension of waveforms" (https://arxiv.org/abs/1003.5266)
public static class FractalDimension
{
    //private int _nPoints;
    //private double _xMax;
    //private double _xMin;
    //private double _yMax;
    //private double _yMin;
    //private double _length;

    /// <summary>
    /// Gets the Hausdorff-Besicovitch dimension for each cumulative segment
    /// </summary>
    public static double[] DimensionCumulative { get; private set; } = Array.Empty<double>();

    /// <summary>
    /// Gets the  Hausdorff-Besicovitch dimension for the whole data set
    /// </summary>
    public static double DimensionSingle { get; private set; } = double.NaN;

    private static readonly double DimensionMinimum = 1.0;

    //public FractalDimension()
    //{

    //}

    public static void GetDimension(double[] xValues, double[] yValues, CancellationToken ct, bool progress = false)
    {
        DimensionCumulative = Array.Empty<double>();
        DimensionSingle = double.NaN;

        if (!progress)
            DimensionSingle = ComputeH(xValues, yValues);
        else
        {
            DimensionCumulative = new double[yValues.Length];
            //ProgressDim[0] = 0;
            for (int i = 0; i < yValues.Length; i++)
            {
                DimensionCumulative[i] = ComputeH(xValues, yValues, i);
                if (ct.IsCancellationRequested)
                    ct.ThrowIfCancellationRequested();
            }
        }
    }

    public static void GetDimension(double samplingFreq, double[] yValues, CancellationToken ct, bool progress = false)
    {
        DimensionCumulative = Array.Empty<double>();
        DimensionSingle = double.NaN;

        if (!progress)
            DimensionSingle = ComputeH(samplingFreq, yValues);
        else
        {
            DimensionCumulative = new double[yValues.Length];
            for (int i = 0; i < yValues.Length; i++)
            {
                DimensionCumulative[i] = ComputeH(samplingFreq, yValues, i);
                if (ct.IsCancellationRequested)
                    ct.ThrowIfCancellationRequested();
            }
        }
    }

    /// <summary>
    /// Computes the fractal dimension of a discrete curve.
    /// </summary>
    /// <param name="xValues">Array containing the abscissa points</param>
    /// <param name="yValues">Array containing the ordinate points</param>
    /// /// <param name="arrayIndex">Array index cutoff. Array values above this index are ignored</param>
    /// <returns>The fractal dimension of the curve</returns>
    private static double ComputeH(double[] xValues, double[] yValues, int? arrayIndex = null)
    {
        int _nPoints;
        double _xMax;
        double _xMin;
        double _yMax;
        double _yMin;
        double _length;
        double dimensionH;

        //DimensionCumulative = Array.Empty<double>();
        //DimensionSingle = double.NaN;

        if (arrayIndex.HasValue)
            _nPoints = arrayIndex.Value + 1;
        else
            _nPoints = yValues.Length;

        if (_nPoints < 2)
            dimensionH = DimensionMinimum;
        else
        {
            (_xMax, _xMin) = GetMaxMin(xValues, arrayIndex);
            (_yMax, _yMin) = GetMaxMin(yValues, arrayIndex);

            _length = NormalizedLength(xValues, yValues, _nPoints, _xMax, _xMin, _yMax, _yMin);
            dimensionH = 1 + System.Math.Log(_length) / System.Math.Log(2 * (_nPoints - 1));
        }

        return dimensionH;
    }

    /// <summary>
    /// Computes the fractal dimension of a discrete curve with a constant sampling rate starting at time 0 seconds.
    /// </summary>
    /// <param name="samplingFreq">The sampling frequency</param>
    /// <param name="yValues">Array containing the ordinate points</param>
    /// <param name="arrayIndex">Array index cutoff. Array values above this index are ignored</param>
    /// <returns>The fractal dimension of the curve</returns>
    private static double ComputeH(double samplingFreq, double[] yValues, int? arrayIndex = null)
    {
        int _nPoints;
        double _yMax;
        double _yMin;
        double _length;
        double dimensionH;

        //DimensionCumulative = Array.Empty<double>();
        //DimensionSingle = double.NaN;

        if (arrayIndex.HasValue)
            _nPoints = arrayIndex.Value + 1;
        else
            _nPoints = yValues.Length;

        if (_nPoints < 2)
            dimensionH = DimensionMinimum;
        else
        {
            (_yMax, _yMin) = GetMaxMin(yValues, arrayIndex);

            _length = NormalizedLength(yValues, _nPoints, _yMax, _yMin);
            dimensionH = 1 + System.Math.Log10(_length) / System.Math.Log10(2 * (_nPoints - 1));
        }

        return dimensionH;
    }

    /// <summary>
    /// Computes the normalized length of a discrete curve.
    /// To do so, the passed values are normalized to be in the range [0, 1] both in the abscissa and the ordinate axes.
    /// </summary>
    /// <param name="xValues">Array containing the abscissa points</param>
    /// <param name="yValues">Array containing the ordinate points</param>
    /// <param name="nPoints">Number of points</param>
    /// <param name="xMax">The maximum x value</param>
    /// <param name="xMin">The minimum x value</param>
    /// <param name="yMax">The maximum y value</param>
    /// <param name="yMin">The minimum y value</param>
    /// <returns>The normalized length of the curve</returns>
    private static double NormalizedLength(double[] xValues, double[] yValues, int nPoints, double xMax, double xMin, double yMax, double yMin)
    {
        double length = 0.0;
        double xNorm1;
        double xNorm2 = 0.0;
        double yNorm1;
        double yNorm2 = 0.0;

        if (nPoints <= 1) return 0.0;

        for (int i = 0; i < nPoints; i++)
        {
            xNorm1 = (xValues[i] - xMin) / (xMax - xMin);
            yNorm1 = (yValues[i] - yMin) / (yMax - yMin);
            if (i > 0)
                length += System.Math.Sqrt(System.Math.Pow(yNorm1 - yNorm2, 2) + System.Math.Pow(xNorm1 - xNorm2, 2));
            xNorm2 = xNorm1;
            yNorm2 = yNorm1;
        }
        return length;
    }

    /// <summary>
    /// Computes the normalized length of a discrete curve with a constant sampling rate starting at time 0 seconds.
    /// The passed values are normalized to be in the range [0, 1] both in the abscissa and the ordinate axes.
    /// </summary>
    /// <param name="yValues">Array containing the ordinate points</param>
    /// <param name="nPoints">Number of points</param>
    /// <param name="yMax">The maximum y value</param>
    /// <param name="yMin">The minimum y value</param>
    /// <returns>The normalized length of the curve</returns>
    private static double NormalizedLength(double[] yValues, int nPoints, double yMax, double yMin)
    {
        double length = 0.0;
        double yNorm1;
        double yNorm2 = 0.0;

        if (nPoints <= 1) return 0.0;

        for (int i = 0; i < nPoints; i++)
        {
            yNorm1 = (yValues[i] - yMin) / (yMax - yMin);
            if (i > 0)
                length += System.Math.Sqrt(System.Math.Pow(yNorm1 - yNorm2, 2) + System.Math.Pow(1 / (nPoints - 1), 2));
            yNorm2 = yNorm1;
        }
        return length;
    }

    /// <summary>
    /// Get the maximum and minimum values whithin an array.
    /// </summary>
    /// <param name="values">Array of values</param>
    /// <param name="arrayIndex">Array index cutoff. Array values above this index are ignored</param>
    /// <returns>Maximum and minimum values</returns>
    private static (double max, double min) GetMaxMin(double[] values, int? arrayIndex = null)
    {
        if (values.Length == 0) return (0.0, 0.0);

        double highest = values[0];
        double lowest = values[0];
        int nPoints;

        if (arrayIndex.HasValue)
            nPoints = arrayIndex.Value + 1;
        else
            nPoints = values.Length;

        for (int i = 1; i < nPoints; i++)
        {
            if (values[i] > highest)
                highest = values[i];
            if (values[i] < lowest)
                lowest = values[i];
        }

        return (highest, lowest);
    }
}

// https://www.codeproject.com/Articles/27030/Approximate-and-Sample-Entropies-Complexity-Metric
// https://pdfs.semanticscholar.org/6841/efb4c40a74c1faa9f36e6d949d1ee330bfa9.pdf
// https://en.wikipedia.org/wiki/Sample_entropy
public class Complexity
{
    /// <summary>
    /// Computes the approximate and sample entropies of a physiological time-series signals (typically used to diagnose diseased states).
    /// ApEn reflects the likelihood that similar patterns of observations will not be followed by additional similar observations. A time series containing many repetitive patterns has a relatively small ApEn; a less predictable process has a higher ApEn.
    /// A smaller value of SampEn also indicates more self-similarity in data set or less noise.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="dim">Embedding dimension</param>
    /// <param name="fTol">Factor to compute the tolerance so that the total is typically equal to 0.2*std</param>
    /// <param name="std">Standard deviation of the population</param>
    /// <returns>AppEn and SampEn</returns>
    public static (double AppEn, double SampEn) Entropy(double[] data, uint dim = 2, double fTol = 0.2, double? std = null)
    {
        long upper = data.Length - (dim + 1) + 1;
        bool isEqual;
        int Cm = 0, Cm1 = 0;
        double sum = 0.0;
        double appEn = 0.0, sampEn = 0.0;
        double tolerance;
        if (std.HasValue)
            tolerance = std.Value * fTol;
        else
            tolerance = StdDev(data) * fTol;


        for (uint i = 0; i < upper; i++)
        {
            Cm = 0;
            Cm1 = 0;
            for (uint j = 0; j < upper; j++)
            {
                isEqual = true;
                //m - length series
                for (uint k = 0; k < dim; k++)
                {
                    if (Math.Abs(data[i + k] - data[j + k]) > tolerance)
                    {
                        isEqual = false;
                        break;
                    }
                }
                if (isEqual) Cm++;

                //m+1 - length series
                if (isEqual && Math.Abs(data[i + dim] - data[j + dim]) <= tolerance)
                    Cm1++;
            }

            if (Cm > 0 && Cm1 > 0)
                sum += Math.Log((double)Cm / (double)Cm1);
        }

        appEn = sum / (double)(data.Length - dim);
        sampEn = Cm > 0 && Cm1 > 0 ? Math.Log((double)Cm / (double)Cm1) : 0.0;

        return (appEn, sampEn);
    }

    /// <summary>
    /// Computes the standard deviation of a data series.
    /// </summary>
    /// <param name="values">Data values</param>
    /// <param name="asSample"><see langword="True"/> to compute the sample standard deviation (N-1); otherwise (by default), it computes the population (N) deviation</param>
    /// <returns>Standard deviation</returns>
    public static double StdDev(double[] values, bool asSample = false)
    {
        double avg = System.Linq.Enumerable.Average(values);
        double sum = System.Linq.Enumerable.Sum(System.Linq.Enumerable.Select(values, v => (v - avg) * (v - avg)));
        double denominator = values.Length - (asSample ? 1 : 0);
        return denominator > 0.0 ? Math.Sqrt(sum / denominator) : -1;
    }
}
