namespace SignalAnalysis.Contracts.Numerical;

/// <summary>
/// Contrato para una estrategia de derivada numérica.
/// </summary>
public interface IDerivativeStrategy
{
    /// <summary>
    /// Computes the derivative of a function.
    /// </summary>
    double[] Compute(Func<double, double> f, double lowerLimit, double upperLimit, int segments);

    /// <summary>
    /// Calcula la derivada a partir de muestras discretas uniformes.
    /// El resultado tiene la misma longitud que samples.
    /// </summary>
    double[] ComputeFromSamples(ReadOnlySpan<double> samples, double samplingFrequency);
}
