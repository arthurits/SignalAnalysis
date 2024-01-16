using System.Diagnostics;

namespace SignalAnalysis;

/// <summary>
/// This class implements the computation of the fractal dimension of a discrete curve according to
/// Carlos Sevcik's "A procedure to estimate the fractal dimension of waveforms"
/// <seealso cref="https://arxiv.org/abs/1003.5266"/>
/// <seealso cref="https://dev.to/kamilbugnokrk/parallel-programming-in-c-53eg"/> 
/// </summary>
public static class FractalDimension
{

    /// <summary>
    /// Hausdorff-Besicovitch dimension for each cumulative segment
    /// </summary>
    public static double[] DimensionCumulative { get; private set; } = [];

    /// <summary>
    /// Hausdorff-Besicovitch dimension for the whole data set
    /// </summary>
    public static double DimensionSingle { get; private set; } = double.NaN;

    public static double VarianceH { get; private set; } = double.NaN;

    private static readonly double DimensionMinimum = 1.0;

    public static void ComputeDimension(double[] xValues, double[] yValues, CancellationToken ct, bool progress = false)
    {
        (double[] xMax, double[] xMin) = GetMaxMin(xValues);
        (double[] yMax, double[] yMin) = GetMaxMin(yValues);

        (DimensionSingle, VarianceH) = ComputeH(xValues, yValues, xMax[^1], xMin[^1], yMax[^1], yMin[^1]);
        if (progress)
        {
            DimensionCumulative = new double[yValues.Length];
            // Compute all but the last point
            Parallel.For(0, yValues.Length - 1, i =>
            {
                (DimensionCumulative[i], VarianceH) = ComputeH(xValues, yValues, xMax[i], xMin[i], yMax[i], yMin[i], i);
                if (ct.IsCancellationRequested)
                    throw new OperationCanceledException("CancelFractal", ct);
            });
            //// Finally, compute the last point and get both the dimension and the variance
            //(DimensionSingle, VarianceH) = ComputeH(xValues, yValues, xMax[^1], xMin[^1], yMax[^1], yMin[^1], yValues.Length - 1);
            DimensionCumulative[^1] = DimensionSingle;
            //DimensionSingle = DimensionCumulative[^1];
        }
    }

    public static void ComputeDimension(double samplingFreq, double[] yValues, CancellationToken ct, bool progress = false)
    {
        (double[] max, double[] min) = GetMaxMin(yValues);

        (DimensionSingle, VarianceH) = ComputeH(yValues, max[^1], min[^1]);
        if (progress)
        {
            DimensionCumulative = new double[yValues.Length];
            
            Stopwatch stopwatch = new();
            stopwatch.Start();

            Parallel.For(0, yValues.Length - 1, i =>
            {
                (DimensionCumulative[i], VarianceH) = ComputeH(yValues, max[i], min[i], i);
                if (ct.IsCancellationRequested)
                    throw new OperationCanceledException("CancelFractal", ct);
            });
            stopwatch.Stop();
            TimeSpan elapsed = stopwatch.Elapsed;
            Debug.WriteLine($"Elapsed time - Parallel For: {elapsed.Hours} hours, {elapsed.Minutes} minutes, {elapsed.Seconds} seconds, and {elapsed.Milliseconds} milliseconds");
            //// Finally, compute the last point and get both the dimension and the variance
            //(DimensionSingle, VarianceH) = ComputeH(samplingFreq, yValues, max[^1], min[^1], yValues.Length - 1);
            DimensionCumulative[^1] = DimensionSingle;
            Debug.WriteLine($"Fractal dimension: {DimensionCumulative[^1]}");
            //DimensionSingle = DimensionCumulative[^1];
        }
    }

    /// <summary>
    /// Computes the fractal dimension of a discrete curve.
    /// </summary>
    /// <param name="xValues">Array containing the abscissa points</param>
    /// <param name="yValues">Array containing the ordinate points</param>
    /// /// <param name="arrayIndex">Array index cutoff. Array values above this index are ignored</param>
    /// <returns>The fractal dimension of the curve and its variance</returns>
    private static (double dimension, double variance) ComputeH(double[] xValues, double[] yValues, double xMax, double xMin, double yMax, double yMin, int? arrayIndex = null)
    {
        int _nPoints;
        double _length;
        double _varLength;
        double _LN;
        double dimensionH;
        double varianceH;

        _nPoints = arrayIndex.HasValue ? arrayIndex.Value + 1 : Math.Min(xValues.Length, yValues.Length);

        if (_nPoints < 2)
        {
            dimensionH = DimensionMinimum;
            varianceH = double.NaN;
        }
        else
        {
            (_length, _varLength) = NormalizedLength(xValues, yValues, _nPoints, xMax, xMin, yMax, yMin);
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
    private static (double dimension, double variance) ComputeH(double[] yValues, double max, double min, int? arrayIndex = null)
    {
        int _nPoints;
        double _length;
        double _LN;
        double _varLength;
        double dimensionH;
        double varianceH;

        _nPoints = arrayIndex.HasValue ? arrayIndex.Value + 1 : yValues.Length;

        if (_nPoints < 2)
        {
            dimensionH = DimensionMinimum;
            varianceH = double.NaN;
        }
        else
        {
            //(_yMax, _yMin) = GetMaxMin(yValues, arrayIndex);

            (_length, _varLength) = NormalizedLength(yValues, _nPoints, max, min);
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
        double xRange = xMax - xMin;
        double yRange = yMax - yMin;

        if (nPoints <= 1) return (0.0, 0.0);

        for (int i = 1; i < nPoints; i++)
        {
            lengthPartial = System.Math.Sqrt(System.Math.Pow((yValues[i] - yValues[i - 1]) / yRange, 2) + System.Math.Pow((xValues[i] - xValues[i - 1]) / xRange, 2));
            lengthTotal += lengthPartial;

            // Compute variance incrementally
            oldMean = mean;
            mean += (lengthPartial - mean) / i;
            variance += (lengthPartial - mean) * (lengthPartial - oldMean);

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
        double yRange = yMax - yMin;
        double xlength = System.Math.Pow((double)1 / (nPoints - 1), 2);

        if (nPoints <= 1) return (0.0, 0.0);

        for (int i = 1; i < nPoints; i++)
        {
            lengthPartial = System.Math.Sqrt(System.Math.Pow((yValues[i] - yValues[i - 1]) / yRange, 2) + xlength);
            lengthTotal += lengthPartial;

            // Compute variance incrementally
            oldMean = mean;
            mean += (lengthPartial - mean) / i;
            variance += (lengthPartial - mean) * (lengthPartial - oldMean);

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

    private static (double[] max, double[] min) GetMaxMin(double[] values)
    {
        double[] max = new double[values.Length];
        double[] min = new double[values.Length];

        max[0] = values[0];
        min[0] = values[0];

        for (int i = 1; i < values.Length; i++)
        {
            max[i] = (values[i] > max[i - 1]) ? values[i] : max[i - 1];
            min[i] = (values[i] < min[i - 1]) ? values[i] : min[i - 1];
        }

        return (max, min);
    }
}