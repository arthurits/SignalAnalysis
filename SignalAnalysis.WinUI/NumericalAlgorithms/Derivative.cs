using SignalAnalysis.Contracts.Numerical;
using SignalAnalysis.Enumerations;
using System.Collections.ObjectModel;

namespace SignalAnalysis.NumericalAlgorithms;

/// <summary>
/// Fachada pública que expone las sobrecargas para derivar funciones y arrays.
/// Internamente delega a estrategias concretas.
/// </summary>
internal static class Derivative
{
    private static readonly IReadOnlyDictionary<DerivativeMethod, IDerivativeStrategy> _strategies =
        new ReadOnlyDictionary<DerivativeMethod, IDerivativeStrategy>(
            new Dictionary<DerivativeMethod, IDerivativeStrategy>
            {
                    { DerivativeMethod.BackwardOnePoint, new BackwardOnePointStrategy() },
                    { DerivativeMethod.ForwardOnePoint, new ForwardOnePointStrategy() },
                    { DerivativeMethod.CenteredThreePoint, new CenteredThreePointStrategy() },
                    { DerivativeMethod.CenteredFivePoint, new CenteredFivePointStrategy() },
                    { DerivativeMethod.CenteredSevenPoint, new CenteredSevenPointStrategy() },
                    { DerivativeMethod.CenteredNinePoint, new CenteredNinePointStrategy() },
                    { DerivativeMethod.SGLinearThreePoint, new SGLinearThreePointStrategy() },
                    { DerivativeMethod.SGLinearFivePoint, new SGLinearFivePointStrategy() },
                    { DerivativeMethod.SGLinearSevenPoint, new SGLinearSevenPointStrategy() },
                    { DerivativeMethod.SGLinearNinePoint, new SGLinearNinePointStrategy() },
                    { DerivativeMethod.SGCubicFivePoint, new SGCubicFivePointStrategy() },
                    { DerivativeMethod.SGCubicSevenPoint, new SGCubicSevenPointStrategy() },
                    { DerivativeMethod.SGCubicNinePoint, new SGCubicNinePointStrategy() },
            });

    private static IDerivativeStrategy GetStrategy(DerivativeMethod method)
    {
        if (!_strategies.TryGetValue(method, out var strategy))
            throw new ArgumentOutOfRangeException(nameof(method), $"Unknown derivative method: {method}");
        return strategy;
    }

    // 1) Derivate a partir de Func<double,double> con segments
    public static double[] Derivate(Func<double, double> function, DerivativeMethod method = DerivativeMethod.CenteredThreePoint,
        double lowerLimit = 0, double upperLimit = 1, int segments = 1)
    {
        ArgumentNullException.ThrowIfNull(function);
        var strat = GetStrategy(method);
        return strat.Compute(function, lowerLimit, upperLimit, segments);
    }

    // 2) Derivate con step en lugar de segments
    public static double[] Derivate(Func<double, double> function, DerivativeMethod method, double lowerLimit, double upperLimit, double step)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(step);
        int segments = (int)Math.Round((upperLimit - lowerLimit) / step, MidpointRounding.ToZero);
        double uLimit = lowerLimit + segments * step;
        return Derivate(function, method, lowerLimit, uLimit, segments);
    }

    // 3) Derivate con número de puntos (points)
    public static double[] Derivate(Func<double, double> function, DerivativeMethod method, double lowerLimit, double upperLimit, short points)
    {
        if (points < 2) throw new ArgumentOutOfRangeException(nameof(points));
        return Derivate(function, method, lowerLimit, upperLimit, points + 1);
    }

    // 4) Derivate a partir de array de muestras
    public static double[] Derivate(double[] array, DerivativeMethod method = DerivativeMethod.CenteredThreePoint,
        int lowerIndex = 0, int upperIndex = 1, double samplingFrequency = 1)
    {
        ArgumentNullException.ThrowIfNull(array);
        if (lowerIndex < 0 || upperIndex <= lowerIndex) throw new ArgumentOutOfRangeException(nameof(lowerIndex));
        if (upperIndex > array.Length) upperIndex = array.Length;

        int length = upperIndex - lowerIndex;
        if (length <= 0) return [];

        var span = new ReadOnlySpan<double>(array, lowerIndex, length);
        var strategy = GetStrategy(method);
        var result = strategy.ComputeFromSamples(span, samplingFrequency);

        // Si la API original devolvía un vector con la misma longitud que el array original,
        // podemos expandir el resultado para cubrir índices fuera del intervalo con NaN.
        var full = new double[array.Length];
        //for (int i = 0; i < array.Length; i++) full[i] = double.NaN;
        for (int i = 0; i < result.Length; i++) full[lowerIndex + i] = result[i];

        return full;
    }
}
