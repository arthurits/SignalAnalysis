using SignalAnalysis.Contracts.Numerical;

namespace SignalAnalysis.NumericalAlgorithms;

/// <summary>
/// Computes the derivative using the Savitzky-Golay linear seven-point coefficients.
/// [3f(x+3h) + 2f(x+2h) + f(x+h) - f(x-h) - 2f(x-2h) - 3f(x-3h)] / 28h
/// </summary>
/// <seealso cref="https://en.wikipedia.org/wiki/Savitzky%E2%80%93Golay_filter"/>
public sealed class SGLinearSevenPointStrategy : IDerivativeStrategy
{
    /// <summary>
    /// Computes the derivative of a function (which is discretised) using the Savitzky-Golay linear seven-point coefficients.
    /// [3f(x+3h) + 2f(x+2h) + f(x+h) - f(x-h) - 2f(x-2h) - 3f(x-3h)] / 28h
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
        double step2 = 2 * step;
        double step3 = 3 * step;

        // If we have less than 7 points, we cannot apply the formula, so we return NaN for all points
        if (n < 7)
        {
            for (int i = 0; i < n; i++) result[i] = double.NaN;
            return result;
        }

        // First and last three points cannot be computed using the formula, so we set them to NaN
        for (int i = 0; i < 3; i++) result[i] = double.NaN;
        for (int i = n - 3; i < n; i++) result[i] = double.NaN;

        double x = lowerLimit + step3;
        for (int j = 3; j < n - 3; j++)
        {
            // formula: (3*(f(x+3h)-f(x-3h)) + 2*(f(x+2h)-f(x-2h)) + f(x+h)-f(x-h)) / (28*h)
            result[j] = (3.0 * (function(x + step3) - function(x - step3)) + 2.0 * (function(x + step2) - function(x - step2)) + function(x + step) - function(x - step)) / (28.0 * step);
            x += step;
        }

        return result;
    }

    public double[] ComputeFromSamples(ReadOnlySpan<double> samples, double samplingFrequency)
    {
        if (samples.Length == 0) return [];
        int n = samples.Length;
        var result = new double[n];

        // If we have less than 7 points, we cannot apply the formula, so we return NaN for all points
        if (n < 7)
        {
            for (int i = 0; i < n; i++) result[i] = double.NaN;
            return result;
        }

        double step = 1.0 / samplingFrequency;

        // First and last three points cannot be computed using the formula, so we set them to NaN
        for (int i = 0; i < 3; i++) result[i] = double.NaN;
        for (int i = n - 3; i < n; i++) result[i] = double.NaN;

        for (int i = 3; i < n - 3; i++)
        {
            result[i] = (3.0 * (samples[i + 3] - samples[i - 3]) + 2.0 * (samples[i + 2] - samples[i - 2]) + samples[i + 1] - samples[i - 1]) / (28.0 * step);
        }

        return result;
    }
}
