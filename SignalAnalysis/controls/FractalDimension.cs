using FftSharp.Windows;

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
    /// Hausdorff-Besicovitch dimension for each cumulative segment
    /// </summary>
    public static double[] DimensionCumulative { get; private set; } = Array.Empty<double>();

    /// <summary>
    /// Hausdorff-Besicovitch dimension for the whole data set
    /// </summary>
    public static double DimensionSingle { get; private set; } = double.NaN;

    public static double VarianceH { get; private set; } = double.NaN;

    private static readonly double DimensionMinimum = 1.0;

    public static void ComputeDimension(double[] xValues, double[] yValues, CancellationToken ct, bool progress = false)
    {
        if (!progress)
            (DimensionSingle, VarianceH) = ComputeH(xValues, yValues);
        else
        {
            DimensionCumulative = new double[yValues.Length];

            for (int i = 0; i < yValues.Length; i++)
            {
                DimensionCumulative[i] = ComputeH(xValues, yValues, i).dimension;
                if (ct.IsCancellationRequested)
                    throw new OperationCanceledException("CancelFractal", ct);
            }
        }
    }

    public static void ComputeDimension(double samplingFreq, double[] yValues, CancellationToken ct, bool progress = false)
    {
        if (!progress)
            (DimensionSingle, VarianceH) = ComputeH(samplingFreq, yValues);
        else
        {
            DimensionCumulative = new double[yValues.Length];
            for (int i = 0; i < yValues.Length; i++)
            {
                DimensionCumulative[i] = ComputeH(samplingFreq, yValues, i).dimension;
                if (ct.IsCancellationRequested)
                    throw new OperationCanceledException("CancelFractal", ct);
            }
        }
    }

    /// <summary>
    /// Computes the fractal dimension of a discrete curve.
    /// </summary>
    /// <param name="xValues">Array containing the abscissa points</param>
    /// <param name="yValues">Array containing the ordinate points</param>
    /// /// <param name="arrayIndex">Array index cutoff. Array values above this index are ignored</param>
    /// <returns>The fractal dimension of the curve and its variance</returns>
    private static (double dimension, double variance) ComputeH(double[] xValues, double[] yValues, int? arrayIndex = null)
    {
        int _nPoints;
        double _xMax;
        double _xMin;
        double _yMax;
        double _yMin;
        double _length;
        double _varLength;
        double _LN;
        double dimensionH;
        double varianceH;

        if (arrayIndex.HasValue)
            _nPoints = arrayIndex.Value + 1;
        else
            _nPoints = yValues.Length;

        if (_nPoints < 2)
        {
            dimensionH = DimensionMinimum;
            varianceH = double.NaN;
        }
        else
        {
            (_xMax, _xMin) = GetMaxMin(xValues, arrayIndex);
            (_yMax, _yMin) = GetMaxMin(yValues, arrayIndex);

            (_length, _varLength) = NormalizedLength(xValues, yValues, _nPoints, _xMax, _xMin, _yMax, _yMin);
            _LN = System.Math.Log(2 * (_nPoints - 1));
            dimensionH = 1 + System.Math.Log(_length) / _LN;
            varianceH = (_nPoints - 1) * _varLength / (_length * _LN * _LN);
        }

        return (dimensionH, varianceH);
    }

    /// <summary>
    /// Computes the fractal dimension of a discrete curve with a constant sampling rate starting at time 0 seconds.
    /// </summary>
    /// <param name="samplingFreq">The sampling frequency</param>
    /// <param name="yValues">Array containing the ordinate points</param>
    /// <param name="arrayIndex">Array index cutoff. Array values above this index are ignored</param>
    /// <returns>The fractal dimension of the curve and its variance</returns>
    private static (double dimension, double variance) ComputeH(double samplingFreq, double[] yValues, int? arrayIndex = null)
    {
        int _nPoints;
        double _yMax;
        double _yMin;
        double _length;
        double _LN;
        double _varLength;
        double dimensionH;
        double varianceH;

        if (arrayIndex.HasValue)
            _nPoints = arrayIndex.Value + 1;
        else
            _nPoints = yValues.Length;

        if (_nPoints < 2)
        {
            dimensionH = DimensionMinimum;
            varianceH = double.NaN;
        }
        else
        {
            (_yMax, _yMin) = GetMaxMin(yValues, arrayIndex);

            (_length, _varLength) = NormalizedLength(yValues, _nPoints, _yMax, _yMin);
            _LN = System.Math.Log10(2 * (_nPoints - 1));
            dimensionH = 1 + System.Math.Log10(_length) / _LN;
            varianceH = (_nPoints - 1) * _varLength / (_length * _LN * _LN);
        }

        return (dimensionH, varianceH);
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
    /// <returns>The normalized length of the curve and its variance</returns>
    private static (double normLength, double variance) NormalizedLength(double[] xValues, double[] yValues, int nPoints, double xMax, double xMin, double yMax, double yMin)
    {
        double lengthTotal = 0.0;
        double lengthPartial;
        double mean = 0;
        double oldMean;
        double variance = 0;
        double xNorm1;
        double xNorm2 = 0.0;
        double yNorm1;
        double yNorm2 = 0.0;

        if (nPoints <= 1) return (0.0, 0.0);

        for (int i = 0; i < nPoints; i++)
        {
            xNorm1 = (xValues[i] - xMin) / (xMax - xMin);
            yNorm1 = (yValues[i] - yMin) / (yMax - yMin);
            if (i > 0)
            {
                lengthPartial = System.Math.Sqrt(System.Math.Pow(yNorm1 - yNorm2, 2) + System.Math.Pow(xNorm1 - xNorm2, 2));
                lengthTotal += lengthPartial;

                // Compute variance incrementally
                oldMean = mean;
                mean += (lengthPartial - mean) / i;
                variance += (lengthPartial - mean) * (lengthPartial - oldMean);
            }
            xNorm2 = xNorm1;
            yNorm2 = yNorm1;
        }

        variance /= (nPoints - 1);
        return (lengthTotal, variance);
    }

    /// <summary>
    /// Computes the normalized length of a discrete curve with a constant sampling rate starting at time 0 seconds.
    /// The passed values are normalized to be in the range [0, 1] both in the abscissa and the ordinate axes.
    /// </summary>
    /// <param name="yValues">Array containing the ordinate points</param>
    /// <param name="nPoints">Number of points</param>
    /// <param name="yMax">The maximum y value</param>
    /// <param name="yMin">The minimum y value</param>
    /// <returns>The normalized length of the curve and its variance</returns>
    private static (double normLength, double variance) NormalizedLength(double[] yValues, int nPoints, double yMax, double yMin)
    {
        double lengthTotal = 0;
        double lengthPartial;
        double mean = 0;
        double oldMean;
        double variance = 0;
        double yNorm1;
        double yNorm2 = 0;

        if (nPoints <= 1) return (0.0, 0.0);

        for (int i = 0; i < nPoints; i++)
        {
            yNorm1 = (yValues[i] - yMin) / (yMax - yMin);
            if (i > 0)
            {
                lengthPartial = System.Math.Sqrt(System.Math.Pow(yNorm1 - yNorm2, 2) + System.Math.Pow(1 / (nPoints - 1), 2));
                lengthTotal += lengthPartial;

                // Compute variance incrementally
                oldMean = mean;
                mean += (lengthPartial - mean) / i;
                variance += (lengthPartial - mean) * (lengthPartial - oldMean);
            }
            yNorm2 = yNorm1;
        }

        variance /= (nPoints - 1);
        return (lengthTotal, variance);
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
    public static (double AppEn, double SampEn) Entropy(double[] data, CancellationToken ct, uint dim = 2, double fTol = 0.2, double? std = null)
    {
        long upper = data.Length - (dim + 1) + 1;
        bool isEqual;
        int AppEn_Cum, AppEn_Cum1;
        int SampEn_Cum = 0, SampEn_Cum1 = 0;
        double sum = 0.0;
        double appEn, sampEn;
        double tolerance;
        if (std.HasValue)
            tolerance = std.Value * fTol;
        else
            tolerance = StdDev<double>(data) * fTol;


        for (uint i = 0; i < upper; i++)
        {
            AppEn_Cum = 0;
            AppEn_Cum1 = 0;
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
                    if (ct.IsCancellationRequested)
                        throw new OperationCanceledException("CancelEntropy", ct);
                }
                if (isEqual)
                {
                    AppEn_Cum++;
                    SampEn_Cum++;
                }

                //m+1 - length series
                if (isEqual && Math.Abs(data[i + dim] - data[j + dim]) <= tolerance)
                {
                    AppEn_Cum1++;
                    SampEn_Cum1++;
                }
            }

            if (AppEn_Cum > 0 && AppEn_Cum1 > 0)
                sum += Math.Log((double)AppEn_Cum / (double)AppEn_Cum1);
        }

        appEn = sum / (double)(data.Length - dim);
        sampEn = SampEn_Cum > 0 && SampEn_Cum1 > 0 ? Math.Log((double)SampEn_Cum / (double)SampEn_Cum1) : 0.0;

        return (appEn, sampEn);
    }

    /// <summary>
    /// Computes the Shannon entropy, the entropy bit, and the ideal entropy for a vector of numeric values
    /// </summary>
    /// <param name="data">Numeric values vector</param>
    /// <returns>The Shannon entropy value, the entropy bit, and the ideal entropy</returns>
    public static (double Entropy, double EntropyBit, double IdealEntropy) ShannonEntropy<T>(IEnumerable<T> data)
    {
        double entropy = 0;
        double entropyBit;
        double entropyIdeal;
        double prob;

        // Convert into an enumerable of doubles.
        IEnumerable<double> values = data.Select(value => Convert.ToDouble(value));
        int nLength = values.Count();
        double nSum = values.Sum();

        // Compute the Shannon entropy
        foreach (double s in values)
        {
            if (s > 0)
            {
                prob = s / nSum;
                entropy -= prob * Math.Log2(prob);
            }
        }

        // https://github.com/wqyeo/Shannon-Entropy/blob/master/EntropyCal.cs
        entropyBit = Math.Ceiling(entropy) * nLength;

        // https://stackoverflow.com/questions/2979174/how-do-i-compute-the-approximate-entropy-of-a-bit-string
        prob = 1.0 / nLength;
        entropyIdeal = -1.0 * nLength * prob * Math.Log(prob);

        return (entropy, entropyBit, entropyIdeal);
    }

    /// <summary>
    /// Computes the standard deviation of a data series.
    /// </summary>
    /// <param name="values">Data values</param>
    /// <param name="asSample"><see langword="True"/> to compute the sample standard deviation (N-1); otherwise (by default), it computes the population (N) deviation</param>
    /// <returns>Standard deviation. A value equal to -1 indicates insufficient data points.</returns>
    public static double StdDev<T>(IEnumerable<T> values, bool asSample = false)
    {
        // Convert into an enumerable of doubles.
        IEnumerable<double> doubles = values.Select(value => Convert.ToDouble(value));

        // Then compute the standard deviation
        double avg = System.Linq.Enumerable.Average(doubles);
        double sum = System.Linq.Enumerable.Sum(System.Linq.Enumerable.Select(doubles, x => (x - avg) * (x - avg)));
        double denominator = values.Count() - (asSample ? 1 : 0);
        return denominator > 0.0 ? Math.Sqrt(sum / denominator) : -1;
    }
}

