using SignalAnalysis.Contracts.Numerical;

namespace SignalAnalysis.NumericalAlgorithms;

/// <summary>
/// Computes the derivative using the Savitzky-Golay cubic/quartic five-point coefficients.
/// The formula is the same as <see cref="CenteredFivePoint(Func{double, double}, double, double, int)"/>
/// [-f(x+2h) + 8f(x+h) - 8f(x-h) - f(x-2h)] / 12h
/// </summary>
/// <seealso cref="https://en.wikipedia.org/wiki/Savitzky%E2%80%93Golay_filter"/>
public sealed class SGCubicFivePointStrategy : IDerivativeStrategy
{
    /// <summary>
    /// Computes the derivative of a function (which is discretised) using the Savitzky-Golay cubic/quartic five-point coefficients.
    /// The formula is the same as <see cref="CenteredFivePoint(Func{double, double}, double, double, int)"/>
    /// [-f(x+2h) + 8f(x+h) - 8f(x-h) - f(x-2h)] / 12h
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

        // If we have less than 5 points, we cannot apply the formula, so we return NaN for all points        
        if (n < 5)
        {
            for (int i = 0; i < n; i++) result[i] = double.NaN;
            return result;
        }

        // Limit extreme points to NaN, as the formula cannot be applied there
        for (int i = 0; i < 2; i++) result[i] = double.NaN;
        for (int i = n - 2; i < n; i++) result[i] = double.NaN;

        double x = lowerLimit + step2;
        for (int j = 2; j < n - 2; j++)
        {
            result[j] = (function(x - step2) - function(x + step2) + 8.0 * (function(x + step) - function(x - step))) / (12.0 * step);
            x += step;
        }

        return result;
    }

    public double[] ComputeFromSamples(ReadOnlySpan<double> samples, double samplingFrequency)
    {
        if (samples.Length == 0) return [];
        int n = samples.Length;
        var result = new double[n];

        // If we have less than 5 points, we cannot apply the formula, so we return NaN for all points
        if (n < 5)
        {
            for (int i = 0; i < n; i++) result[i] = double.NaN;
            return result;
        }

        double step = 1.0 / samplingFrequency;

        // Limit extreme points to NaN, as the formula cannot be applied there
        for (int i = 0; i < 2; i++) result[i] = double.NaN;
        for (int i = n - 2; i < n; i++) result[i] = double.NaN;

        for (int i = 2; i < n - 2; i++)
        {
            result[i] = (samples[i - 2] - samples[i + 2] + 8.0 * (samples[i + 1] - samples[i - 1])) / (12.0 * step);
        }

        return result;
    }
}
}
