using SignalAnalysis.Contracts.Numerical;

namespace SignalAnalysis.NumericalAlgorithms;

/// <summary>
/// Computes the derivative using the central seven-points finite difference.
/// [f(x+3h) - 9f(x+2h) + 45f(x+h) - 45f(x-h) + 9f(x-2h) - f(x-3h)] / 60h
/// </summary>
/// <seealso cref="https://en.wikipedia.org/wiki/Finite_difference_coefficient"/>
public sealed class CenteredSevenPointStrategy : IDerivativeStrategy
{
    /// <summary>
    /// Computes the derivative of a function (which is discretised) using the central seven-points finite difference.
    /// [f(x+3h) - 9f(x+2h) + 45f(x+h) - 45f(x-h) + 9f(x-2h) - f(x-3h)] / 60h
    /// </summary>
    /// <param name="function">Function to be derivated</param>
    /// <param name="lowerLimit">Differentiation lower limit</param>
    /// <param name="upperLimit">Differentiation upper limit</param>
    /// <param name="segments">Number of equal segments the differentiation interval is divided into</param>
    /// <returns>1D array (vector) with the discrete derivate for the discretised <paramref name="function"/></returns>
    /// <seealso cref="https://en.wikipedia.org/wiki/Finite_difference_coefficient"/>
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

        // First 3 and last 3 points are not defined due to the formula requiring points at x±3h, so we set them to NaN
        for (int i = 0; i < 3; i++) result[i] = double.NaN;
        for (int i = n - 3; i < n; i++) result[i] = double.NaN;

        double x = lowerLimit + step3;
        for (int j = 3; j < n - 3; j++)
        {
            // formula (rearranged): (f(x+3h) - f(x-3h) + 9*(f(x-2h) - f(x+2h)) + 45*(f(x+h) - f(x-h))) / (60*h)
            result[j] = (function(x + step3) - function(x - step3) + 9.0 * (function(x - step2) - function(x + step2)) + 45.0 * (function(x + step) - function(x - step))) / (60.0 * step);
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

        // First 3 and last 3 points are not defined due to the formula requiring points at x±3h, so we set them to NaN
        for (int i = 0; i < 3; i++) result[i] = double.NaN;
        for (int i = n - 3; i < n; i++) result[i] = double.NaN;

        for (int i = 3; i < n - 3; i++)
        {
            // indices: i-3, i-2, i-1, i+1, i+2, i+3
            result[i] = (samples[i + 3] - samples[i - 3] + 9.0 * (samples[i - 2] - samples[i + 2]) + 45.0 * (samples[i + 1] - samples[i - 1])) / (60.0 * step);
        }

        return result;
    }
}
