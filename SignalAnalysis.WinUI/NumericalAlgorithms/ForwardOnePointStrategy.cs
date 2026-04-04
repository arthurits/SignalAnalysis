using SignalAnalysis.Contracts.Numerical;

namespace SignalAnalysis.NumericalAlgorithms;

/// <summary>
/// Fórmula hacia adelante de un punto: (f(x+h) - f(x)) / h
/// </summary>
public sealed class ForwardOnePointStrategy : IDerivativeStrategy
{
    public double[] Compute(Func<double, double> f, double lowerLimit, double upperLimit, int segments)
    {
        if (f is null) throw new ArgumentNullException(nameof(f));
        if (segments <= 0) throw new ArgumentOutOfRangeException(nameof(segments));
        if (lowerLimit >= upperLimit) throw new ArgumentException("lowerLimit must be < upperLimit");

        int n = segments + 1;
        var result = new double[n];
        double step = (upperLimit - lowerLimit) / segments;

        double x = lowerLimit;
        for (int j = 0; j < n - 1; j++)
        {
            result[j] = (f(x + step) - f(x)) / step;
            x += step;
        }

        // Último punto no definido con esta fórmula
        result[n - 1] = double.NaN;
        return result;
    }

    public double[] ComputeFromSamples(ReadOnlySpan<double> samples, double samplingFrequency)
    {
        if (samples.Length == 0) return Array.Empty<double>();
        int n = samples.Length;
        var result = new double[n];
        double step = 1.0 / samplingFrequency;

        for (int i = 0; i < n - 1; i++)
            result[i] = (samples[i + 1] - samples[i]) / step;

        result[n - 1] = double.NaN;
        return result;
    }
}
