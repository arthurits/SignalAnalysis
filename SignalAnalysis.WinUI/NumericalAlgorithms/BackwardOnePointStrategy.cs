using SignalAnalysis.Contracts.Numerical;

namespace SignalAnalysis.NumericalAlgorithms;

/// <summary>
/// Computes the derivative using the backwards one-point finite difference.
/// [f(x) - f(x-h)] / h
/// </summary>
/// <seealso cref="https://www.cantorsparadise.com/the-best-numerical-derivative-approximation-formulas-998703380948"/>
public sealed class BackwardOnePointStrategy : IDerivativeStrategy
{
    /// <summary>
    /// Computes the derivative of a function (which is discretised) using the backwards one-point finite difference.
    /// [f(x) - f(x-h)] / h
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

        // Primer punto no definido con esta fórmula
        result[0] = double.NaN;

        double x = lowerLimit + step;
        for (int j = 1; j < n; j++)
        {
            result[j] = (function(x) - function(x - step)) / step;
            x += step;
        }

        return result;
    }

    public double[] ComputeFromSamples(ReadOnlySpan<double> samples, double samplingFrequency)
    {
        if (samples.Length == 0) return [];
        int n = samples.Length;
        var result = new double[n];
        double step = 1.0 / samplingFrequency;

        result[0] = double.NaN;
        
        for (int i = 1; i < n; i++)
            result[i] = (samples[i] - samples[i - 1]) / step;

        return result;
    }
}
