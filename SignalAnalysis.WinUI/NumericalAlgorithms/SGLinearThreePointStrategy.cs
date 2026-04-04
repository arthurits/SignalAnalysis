using SignalAnalysis.Contracts.Numerical;

namespace SignalAnalysis.NumericalAlgorithms;

/// <summary>
/// Computes the derivative of using the Savitzky-Golay linear three-point coefficients.
/// The formula is the same as <see cref="CenteredThreePoint(Func{double, double}, double, double, int)"/>
/// [f(x+h) - f(x-h)] / 2h
/// </summary>
/// <seealso cref="https://en.wikipedia.org/wiki/Savitzky%E2%80%93Golay_filter"/>
public sealed class SGLinearThreePointStrategy : IDerivativeStrategy
{
    /// <summary>
    /// Computes the derivative of a function (which is discretised) using the Savitzky-Golay linear three-point coefficients.
    /// The formula is the same as <see cref="CenteredThreePoint(Func{double, double}, double, double, int)"/>
    /// [f(x+h) - f(x-h)] / 2h
    /// </summary>
    /// <param name="function">Function to be derivated</param>
    /// <param name="lowerLimit">Differentiation lower limit</param>
    /// <param name="upperLimit">Differentiation upper limit</param>
    /// <param name="segments">Number of equal segments the differentiation interval is divided into</param>
    /// <returns>1D array (vector) with the discrete derivate for the discretised <paramref name="function"/></returns>
    /// <seealso cref="https://en.wikipedia.org/wiki/Savitzky%E2%80%93Golay_filter"/>
    public double[] Compute(Func<double, double> function, double lowerLimit, double upperLimit, int segments)
    {
        ArgumentNullException.ThrowIfNull(function);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(segments);
        if (lowerLimit >= upperLimit) throw new ArgumentException("lowerLimit must be < upperLimit");

        int n = segments + 1;
        var result = new double[n];
        double step = (upperLimit - lowerLimit) / segments;

        // If we have less than 3 points, we cannot apply the formula, so we return NaN for all points
        if (n < 3)
        {
            for (int i = 0; i < n; i++) result[i] = double.NaN;
            return result;
        }

        // First and last points cannot be computed using the formula, so we set them to NaN
        result[0] = double.NaN;
        result[n - 1] = double.NaN;

        double x = lowerLimit + step;
        for (int j = 1; j < n - 1; j++)
        {
            result[j] = (function(x + step) - function(x - step)) / (2.0 * step);
            x += step;
        }

        return result;
    }

    public double[] ComputeFromSamples(ReadOnlySpan<double> samples, double samplingFrequency)
    {
        if (samples.Length == 0) return [];
        int n = samples.Length;
        var result = new double[n];

        // If we have less than 3 points, we cannot apply the formula, so we return NaN for all points
        if (n < 3)
        {
            for (int i = 0; i < n; i++) result[i] = double.NaN;
            return result;
        }

        double step = 1.0 / samplingFrequency;

        // First and last points cannot be computed using the formula, so we set them to NaN
        result[0] = double.NaN;
        result[n - 1] = double.NaN;

        for (int i = 1; i < n - 1; i++)
            result[i] = (samples[i + 1] - samples[i - 1]) / (2.0 * step);

        return result;
    }
}
