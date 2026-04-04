using SignalAnalysis.Contracts.Numerical;

namespace SignalAnalysis.NumericalAlgorithms;

/// <summary>
/// Computes the derivative using the forwards one-point finite difference.
/// [f(x+h) - f(x)] / h
/// </summary>
/// <seealso cref="https://www.cantorsparadise.com/the-best-numerical-derivative-approximation-formulas-998703380948"/>
public sealed class ForwardOnePointStrategy : IDerivativeStrategy
{
    /// <summary>
    /// Computes the derivative of a function (which is discretised) using the forwards one-point finite difference.
    /// [f(x+h) - f(x)] / h
    /// </summary>
    /// <param name="function">Function to be derivated</param>
    /// <param name="lowerLimit">Differentiation lower limit</param>
    /// <param name="upperLimit">Differentiation upper limit</param>
    /// <param name="segments">Number of equal segments the differentiation interval is divided into</param>
    /// <returns>1D array (vector) with the discrete derivate for the discretised <paramref name="function"/></returns>
    /// <seealso cref="https://www.cantorsparadise.com/the-best-numerical-derivative-approximation-formulas-998703380948"/>
    public double[] Compute(Func<double, double> function, double lowerLimit, double upperLimit, int segments)
    {
        ArgumentNullException.ThrowIfNull(function);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(segments);
        if (lowerLimit >= upperLimit) throw new ArgumentException("lowerLimit must be < upperLimit");

        int n = segments + 1;
        var result = new double[n];
        double step = (upperLimit - lowerLimit) / segments;

        double x = lowerLimit;
        for (int j = 0; j < n - 1; j++)
        {
            result[j] = (function(x + step) - function(x)) / step;
            x += step;
        }

        // The last point does not have a forward neighbor, so we set it to NaN
        result[n - 1] = double.NaN;

        return result;
    }

    public double[] ComputeFromSamples(ReadOnlySpan<double> samples, double samplingFrequency)
    {
        if (samples.Length == 0) return [];
        int n = samples.Length;
        var result = new double[n];
        double step = 1.0 / samplingFrequency;

        for (int i = 0; i < n - 1; i++)
        {
            result[i] = (samples[i + 1] - samples[i]) / step;
        }

        // The last point does not have a forward neighbor, so we set it to NaN
        result[n - 1] = double.NaN;

        return result;
    }
}
