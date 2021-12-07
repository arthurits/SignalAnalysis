namespace SignalAnalysis;

// This class implements the computation of the fractal dimension of a discrete curve according to 
// Carlos Sevcik's "A procedure to estimate the fractal dimension of waveforms" (https://arxiv.org/abs/1003.5266)
public class FractalDimension
{
    private int _nPoints;
    private double _xMax;
    private double _xMin;
    private double _yMax;
    private double _yMin;
    private double _length;

    public double[] ProgressDim { get; private set; } = Array.Empty<double>();

    public double Dimension { get; private set; }


    public FractalDimension()
    {

    }

    public FractalDimension(double[] xValues, double[] yValues, bool progress = false)
        : this()
    {
        if (!progress)
            Dimension = ComputeDim(xValues, yValues);
        else
        {
            ProgressDim = new double[yValues.Length];
            //ProgressDim[0] = 0;
            for (int i = 0; i < yValues.Length; i++)
                ProgressDim[i] = ComputeDim(xValues, yValues, i);
        }
    }

    public FractalDimension(double samplingFreq, double[] yValues, bool progress = false)
        : this()
    {
        if (!progress)
            Dimension = ComputeDim(samplingFreq, yValues);
        else
        {
            ProgressDim = new double[yValues.Length];
            for (int i = 0; i < yValues.Length; i++)
                ProgressDim[i] = ComputeDim(samplingFreq, yValues, i);
        }
    }

    /// <summary>
    /// Computes the fractal dimension of a discrete curve.
    /// </summary>
    /// <param name="xValues">Array containing the abscissa points</param>
    /// <param name="yValues">Array containing the ordinate points</param>
    /// /// <param name="arrayIndex">Array index cutoff. Array values above this index are ignored</param>
    /// <returns>The fractal dimension of the curve</returns>
    public double ComputeDim(double[] xValues, double[] yValues, int? arrayIndex = null)
    {
        if (arrayIndex.HasValue)
            _nPoints = arrayIndex.Value + 1;
        else
            _nPoints = yValues.Length;

        if (_nPoints < 2)
            Dimension = 0;
        else
        {
            (_xMax, _xMin) = GetMaxMin(xValues, arrayIndex);
            (_yMax, _yMin) = GetMaxMin(yValues, arrayIndex);

            _length = NormalizedLength(xValues, yValues, _nPoints, _xMax, _xMin, _yMax, _yMin);
            Dimension = 1 + System.Math.Log(_length) / System.Math.Log(2 * (_nPoints - 1));
        }

        return Dimension;
    }

    /// <summary>
    /// Computes the fractal dimension of a discrete curve with a constant sampling rate starting at time 0 seconds.
    /// </summary>
    /// <param name="samplingFreq">The sampling frequency</param>
    /// <param name="yValues">Array containing the ordinate points</param>
    /// <param name="arrayIndex">Array index cutoff. Array values above this index are ignored</param>
    /// <returns>The fractal dimension of the curve</returns>
    public double ComputeDim(double samplingFreq, double[] yValues, int? arrayIndex = null)
    {
        if (arrayIndex.HasValue)
            _nPoints = arrayIndex.Value + 1;
        else
            _nPoints = yValues.Length;

        if (_nPoints < 2)
            Dimension = 0;
        else
        {
            (_yMax, _yMin) = GetMaxMin(yValues, arrayIndex);

            _length = NormalizedLength(yValues, _nPoints, _yMax, _yMin);
            Dimension = 1 + System.Math.Log10(_length) / System.Math.Log10(2 * (_nPoints - 1));
        }

        return Dimension;
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
    private double NormalizedLength(double[] xValues, double[] yValues, int nPoints, double xMax, double xMin, double yMax, double yMin)
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
    private double NormalizedLength(double[] yValues, int nPoints, double yMax, double yMin)
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
    private (double max, double min) GetMaxMin(double[] values, int? arrayIndex = null)
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
