using SignalAnalysis.Contracts.Numerical;

namespace SignalAnalysis.NumericalAlgorithms;

/// <summary>
/// Computes the derivative using the central nine-points finite difference.
/// [-f(x+4h) + 32/3f(x+3h) - 56f(x+2h) + 224f(x+h) - 224f(x-h) + 56f(x-2h) - 32/3f(x-3h) + f(x-4h)] / 280h
/// </summary>
/// <seealso cref="https://en.wikipedia.org/wiki/Finite_difference_coefficient"/>
public sealed class CenteredNinePointStrategy : IDerivativeStrategy
{
    /// <summary>
    /// Computes the derivative of a function (which is discretised) using the central nine-points finite difference.
    /// [-f(x+4h) + 32/3f(x+3h) - 56f(x+2h) + 224f(x+h) - 224f(x-h) + 56f(x-2h) - 32/3f(x-3h) + f(x-4h)] / 280h
    /// </summary>
    /// <param name="function">Function to be derivated</param>
    /// <param name="lowerLimit">Differentiation lower limit</param>
    /// <param name="upperLimit">Differentiation upper limit</param>
    /// <param name="segments">Number of equal segments the differentiation interval is divided into</param>
    /// <returns>1D array (vector) with the discrete derivate for the discretised <paramref name="function"/></returns>
    /// <seealso cref="https://en.wikipedia.org/wiki/Finite_difference_coefficient"/>
    public double[] Compute(Func<double, double> function, double lowerLimit, double upperLimit, int segments)
    {
        if (function is null) throw new ArgumentNullException(nameof(f));
        if (segments <= 0) throw new ArgumentOutOfRangeException(nameof(segments));
        if (lowerLimit >= upperLimit) throw new ArgumentException("lowerLimit must be < upperLimit");

        int n = segments + 1;
        var result = new double[n];
        double step = (upperLimit - lowerLimit) / segments;
        double step2 = 2 * step;
        double step3 = 3 * step;
        double step4 = 4 * step;

        // If there are less than 9 points, we cannot apply the formula, so we return NaN for all points
        if (n < 9)
        {
            for (int i = 0; i < n; i++) result[i] = double.NaN;
            return result;
        }

        // Bordes no definidos: 4 primeros y 4 últimos
        for (int i = 0; i < 4; i++) result[i] = double.NaN;
        for (int i = n - 4; i < n; i++) result[i] = double.NaN;

        double x = lowerLimit + step4;
        for (int j = 4; j < n - 4; j++)
        {
            // formula (rearranged to match original): 
            // (f(x - 4h) - f(x + 4h) + (32/3)*(f(x + 3h) - f(x - 3h)) + 56*(f(x - 2h) - f(x + 2h)) + 224*(f(x + h) - f(x - h))) / (280*h)
            result[j] = (function(x - step4) - function(x + step4) + (32.0 / 3.0) * (function(x + step3) - function(x - step3)) + 56.0 * (function(x - step2) - function(x + step2)) + 224.0 * (function(x + step) - function(x - step))) / (280.0 * step);
            x += step;
        }

        return result;
    }

    public double[] ComputeFromSamples(ReadOnlySpan<double> samples, double samplingFrequency)
    {
        if (samples.Length == 0) return Array.Empty<double>();
        int n = samples.Length;
        var result = new double[n];

        // If there are less than 9 points, we cannot apply the formula, so we return NaN for all points
        if (n < 9)
        {
            for (int i = 0; i < n; i++) result[i] = double.NaN;
            return result;
        }

        double h = 1.0 / samplingFrequency;
        // Bordes: 4 primeros y 4 últimos no definidos
        for (int i = 0; i < 4; i++) result[i] = double.NaN;
        for (int i = n - 4; i < n; i++) result[i] = double.NaN;

        for (int i = 4; i < n - 4; i++)
        {
            // indices: i-4..i+4 excluding i
            result[i] = (samples[i - 4] - samples[i + 4] + (32.0 / 3.0) * (samples[i + 3] - samples[i - 3]) + 56.0 * (samples[i - 2] - samples[i + 2]) + 224.0 * (samples[i + 1] - samples[i - 1])) / (280.0 * h);
        }

        return result;
    }
}
