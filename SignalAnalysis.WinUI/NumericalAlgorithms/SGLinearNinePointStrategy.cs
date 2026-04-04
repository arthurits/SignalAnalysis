using SignalAnalysis.Contracts.Numerical;

namespace SignalAnalysis.NumericalAlgorithms;

/// <summary>
/// Computes the derivative using the Savitzky-Golay linear nine-point coefficients.
/// [4f(x+4h) + 3f(x+3h) + 2f(x+2h) + f(x+h) - f(x-h) - 2f(x-2h) - 3f(x-3h) - 4f(x-4h)] / 60h
/// </summary>
/// <seealso cref="https://en.wikipedia.org/wiki/Savitzky%E2%80%93Golay_filter"/>
public sealed class SGLinearNinePointStrategy : IDerivativeStrategy
{
    /// <summary>
    /// Computes the derivative of a function (which is discretised) using the Savitzky-Golay linear nine-point coefficients.
    /// [4f(x+4h) + 3f(x+3h) + 2f(x+2h) + f(x+h) - f(x-h) - 2f(x-2h) - 3f(x-3h) - 4f(x-4h)] / 60h
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
        double step4 = 4 * step;

        // If we have less than 9 points, we cannot apply the formula, so we return NaN for all points
        if (n < 9)
        {
            for (int i = 0; i < n; i++) result[i] = double.NaN;
            return result;
        }

        // Limit extreme points to NaN, as the formula cannot be applied there
        for (int i = 0; i < 4; i++) result[i] = double.NaN;
        for (int i = n - 4; i < n; i++) result[i] = double.NaN;

        double x = lowerLimit + step4;
        for (int j = 4; j < n - 4; j++)
        {
            // formula: (4*(f(x+4h)-f(x-4h)) + 3*(f(x+3h)-f(x-3h)) + 2*(f(x+2h)-f(x-2h)) + (f(x+h)-f(x-h))) / (60*h)
            result[j] = (4.0 * (function(x + step4) - function(x - step4))
                         + 3.0 * (function(x + step3) - function(x - step3))
                         + 2.0 * (function(x + step2) - function(x - step2))
                         + (function(x + step) - function(x - step)))
                        / (60.0 * step);
            x += step;
        }

        return result;
    }

    public double[] ComputeFromSamples(ReadOnlySpan<double> samples, double samplingFrequency)
    {
        if (samples.Length == 0) return [];
        int n = samples.Length;
        var result = new double[n];

        // If we have less than 9 points, we cannot apply the formula, so we return NaN for all points
        if (n < 9)
        {
            for (int i = 0; i < n; i++) result[i] = double.NaN;
            return result;
        }

        double step = 1.0 / samplingFrequency;

        // Limit extreme points to NaN, as the formula cannot be applied there
        for (int i = 0; i < 4; i++) result[i] = double.NaN;
        for (int i = n - 4; i < n; i++) result[i] = double.NaN;

        for (int i = 4; i < n - 4; i++)
        {
            result[i] = (4.0 * (samples[i + 4] - samples[i - 4])
                         + 3.0 * (samples[i + 3] - samples[i - 3])
                         + 2.0 * (samples[i + 2] - samples[i - 2])
                         + (samples[i + 1] - samples[i - 1]))
                        / (60.0 * step);
        }

        return result;
    }
}