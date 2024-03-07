using System.Numerics;

namespace SignalAnalysis;

/// <summary>
/// Differentiation algorithms
/// </summary>
public enum DerivativeMethod
{
    BackwardOnePoint,
    ForwardOnePoint,
    CenteredThreePoint,
    CenteredFivePoint,
    CenteredSevenPoint,
    CenteredNinePoint,
    SGLinearThreePoint,
    SGLinearFivePoint,
    SGLinearSevenPoint,
    SGLinearNinePoint,
    SGCubicFivePoint,
    SGCubicSevenPoint,
    SGCubicNinePoint
}

/// <summary>
/// This idea comes from https://stackoverflow.com/questions/373186/mathematical-function-differentiation-with-c
/// However, this approach has been replaced for a more direct one.
/// </summary>
public interface IFunction<T>
{
    // Since operator () can't be overloaded, we'll use this trick.
    double this[T arg] { get; }
}

delegate double RealFunction<T>(T arg);

class Function<T>(RealFunction<T> function) : IFunction<T>
{
    readonly RealFunction<T> func = function;

    public double this[T arg]
    {
        get
        {
            return func(arg);
        }
    }
}


/// <summary>
/// Numerical differentiatin computation. Data is expected to be uniformly spaced.
/// </summary>
public static class Derivative
{
    /// <summary>
    /// Computes the numerical derivative of a function
    /// </summary>
    /// <param name="function">Function to be derivated</param>
    /// <param name="method">Differentiation algorithm</param>
    /// <param name="lowerLimit">Differentiation lower limit</param>
    /// <param name="upperLimit">Differentiation upper limit</param>
    /// <param name="segments">Number of equal segments the differentiation interval is divided into</param>
    /// <returns>1D array (vector) with the discrete derivate at each point</returns>
    /// <exception cref="ArgumentOutOfRangeException">Exception thrown when the method isn't defined at <see cref="DerivativeMethod"/></exception>
    public static double[] Derivate(Func<double, double> function, DerivativeMethod method = DerivativeMethod.CenteredThreePoint, double lowerLimit = 0, double upperLimit = 1, int segments = 1)
    {
        return method switch
        {
            DerivativeMethod.BackwardOnePoint => BackwardOnePoint(function, lowerLimit, upperLimit, segments),
            DerivativeMethod.ForwardOnePoint => ForwardOnePoint(function, lowerLimit, upperLimit, segments),
            DerivativeMethod.CenteredThreePoint => CenteredThreePoint(function, lowerLimit, upperLimit, segments),
            DerivativeMethod.CenteredFivePoint => CenteredFivePoint(function, lowerLimit, upperLimit, segments),
            DerivativeMethod.CenteredSevenPoint => CenteredSevenPoint(function, lowerLimit, upperLimit, segments),
            DerivativeMethod.CenteredNinePoint => CenteredNinePoint(function, lowerLimit, upperLimit, segments),
            DerivativeMethod.SGLinearThreePoint => SGLinearThreePoint(function, lowerLimit, upperLimit, segments),
            DerivativeMethod.SGLinearFivePoint => SGLinearFivePoint(function, lowerLimit, upperLimit, segments),
            DerivativeMethod.SGLinearSevenPoint => SGLinearSevenPoint(function, lowerLimit, upperLimit, segments),
            DerivativeMethod.SGLinearNinePoint => SGLinearNinePoint(function, lowerLimit, upperLimit, segments),
            DerivativeMethod.SGCubicFivePoint => SGCubicFivePoint(function, lowerLimit, upperLimit, segments),
            DerivativeMethod.SGCubicSevenPoint => SGCubicSevenPoint(function, lowerLimit, upperLimit, segments),
            DerivativeMethod.SGCubicNinePoint => SGCubicNinePoint(function, lowerLimit, upperLimit, segments),
            _ => throw new ArgumentOutOfRangeException(nameof(method), $"Not expected derivative method: {method}")
        };
    }

    /// <summary>
    /// Computes the numerical derivative of a function. Not yet tested.
    /// </summary>
    /// <param name="function">Function to be derivated</param>
    /// <param name="method">Differentiation algorithm</param>
    /// <param name="lowerLimit">Differentiation lower limit</param>
    /// <param name="upperLimit">Differentiation upper limit</param>
    /// <param name="step">Step value between points. This should ba a multiple of the interval length.</param>
    /// <returns>1D array (vector) with the discrete derivate at each point</returns>
    /// <exception cref="ArgumentOutOfRangeException">Exception thrown when the method isn't defined at <see cref="DerivativeMethod"/></exception>
    public static double[] Derivate(Func<double, double> function, DerivativeMethod method = DerivativeMethod.CenteredThreePoint, double lowerLimit = 0, double upperLimit = 1, double step = 1)
    {
        // Compute the number of segments in the interval, rounded (truncated) to the lowest integer.
        int segments = (int)Math.Round((upperLimit - lowerLimit) / step, MidpointRounding.ToZero);
        double uLimit = lowerLimit + segments * step;

        // Needs further checking
        return Derivate(function, method, lowerLimit, uLimit, segments);
    }

    /// <summary>
    /// Computes the numerical derivative of a function. Not yet tested.
    /// </summary>
    /// <param name="function">Function to be derivated</param>
    /// <param name="method">Differentiation algorithm</param>
    /// <param name="lowerLimit">Differentiation lower limit</param>
    /// <param name="upperLimit">Differentiation upper limit</param>
    /// <param name="points">Number of points in the differentiation interval</param>
    /// <returns>1D array (vector) with the discrete derivate at each point</returns>
    /// <exception cref="ArgumentOutOfRangeException">Exception thrown when the method isn't defined at <see cref="DerivativeMethod"/></exception>
    public static double[] Derivate(Func<double, double> function, DerivativeMethod method = DerivativeMethod.CenteredThreePoint, double lowerLimit = 0, double upperLimit = 1, Int16 points = 2)
    {
        // Needs further checking
        return Derivate(function, method, lowerLimit, upperLimit, points + 1);
    }
    
    /// <summary>
    /// Computes the numerical derivative of a data table. Data is expected to be uniformly spaced.
    /// </summary>
    /// <param name="array">1D array (vector) containing the original data to be differentiated</param>
    /// <param name="method">Differentiation algorithm</param>
    /// <param name="lowerIndex">Differentiation lower limit</param>
    /// <param name="upperIndex">Differentiation upper limit</param>
    /// <param name="samplingFrequency">Data sampling frequency</param>
    /// <returns>1D array (vector) with the discrete derivate for each point in <paramref name="array"/></returns>
    public static double[] Derivate(double[] array, DerivativeMethod method = DerivativeMethod.CenteredThreePoint, int lowerIndex = 0, int upperIndex = 1, double samplingFrequency = 1)
    {
        double[] result = Derivate(Function, method, lowerIndex, upperIndex, upperIndex - lowerIndex);

        // Adjust the result by the sampling frequency. This is only possible when data is uniformly spaced.
        for (int i = 0; i < result.Length; i++)
            result[i] *= samplingFrequency;

        return result;

        // Convert the data array to a function
        double Function(double index)
        {
            if (index < 0)
                return Double.NaN;

            if (index >= array.Length)
                return Double.NaN;

            return array[(int)(index)];
        }
    }

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
    private static double[] BackwardOnePoint(Func<double, double> function, double lowerLimit, double upperLimit, int segments = 1)
    {
        if (segments == 0 || lowerLimit >= upperLimit) return [];

        double[] result = new double[segments + 1];
        double step = (upperLimit - lowerLimit) / segments;
        double x = lowerLimit + step;

        result[0] = double.NaN;
        for (int j = 1; j < segments + 1; j++)
        {
            result[j] = (function(x) - function(x - step)) / step;
            x += step;
        }
        
        return result;
    }

    /// <summary>
    /// Point derivative using the backwards one-point finite difference.
    /// [f(x) - f(x-h)] / h
    /// </summary>
    /// <param name="function">Function to be derivated</param>
    /// <param name="abscissa">Abscissa value where the derivative will be computed at</param>
    /// <param name="step">The absissa increment between this point and the previous one</param>
    /// <returns>Derivative value</returns>
    /// <seealso cref="https://www.cantorsparadise.com/the-best-numerical-derivative-approximation-formulas-998703380948"/>
    private static double BackwardOnePoint(Func<double, double> function, double abscissa = 1, double step = 1)
    {
        return (function(abscissa) - function(abscissa - step)) / step;
    }

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
    private static double[] ForwardOnePoint(Func<double, double> function, double lowerLimit, double upperLimit, int segments = 1)
    {
        if (segments == 0 || lowerLimit >= upperLimit) return [];

        double[] result = new double[segments + 1];
        double step = (upperLimit - lowerLimit) / segments;
        double x = lowerLimit;

        result[segments] = double.NaN;
        for (int j = 0; j < segments; j++)
        {
            result[j] = (function(x + step) - function(x)) / step;
            x += step;
        }

        return result;
    }

    /// <summary>
    /// Point derivative using the forwards one-point finite difference.
    /// [f(x+h) - f(x)] / h
    /// </summary>
    /// <param name="function">Function to be derivated</param>
    /// <param name="abscissa">Abscissa value where the derivative will be computed at</param>
    /// <param name="step">The absissa increment between this point and the previous one</param>
    /// <returns>Derivative value</returns>
    /// <seealso cref="https://www.cantorsparadise.com/the-best-numerical-derivative-approximation-formulas-998703380948"/>
    private static double ForwardOnePoint(Func<double, double> function, double abscissa = 0, double step = 1)
    {
        return (function(abscissa + step) - function(abscissa)) / step;
    }

    /// <summary>
    /// Computes the derivative of a function (which is discretised) using the central three-points finite difference.
    /// [f(x+h) - f(x-h)] / 2h
    /// </summary>
    /// <param name="function">Function to be derivated</param>
    /// <param name="lowerLimit">Differentiation lower limit</param>
    /// <param name="upperLimit">Differentiation upper limit</param>
    /// <param name="segments">Number of equal segments the differentiation interval is divided into</param>
    /// <returns>1D array (vector) with the discrete derivate for the discretised <paramref name="function"/></returns>
    /// <seealso cref="https://www.cantorsparadise.com/the-best-numerical-derivative-approximation-formulas-998703380948"/>
    private static double[] CenteredThreePoint(Func<double, double> function, double lowerLimit, double upperLimit, int segments = 1)
    {
        if (segments == 0 || lowerLimit >= upperLimit) return [];

        double[] result = new double[segments + 1];
        double step = (upperLimit - lowerLimit) / segments;
        double x = lowerLimit + step;

        result[0] = double.NaN;
        result[segments] = double.NaN;
        for (int j = 1; j < segments; j++)
        {
            result[j] = (function(x + step) - function(x - step)) / (2 * step);
            x += step;
        }

        return result;
    }

    /// <summary>
    /// Point derivative using the central three-points finite difference.
    /// [f(x+h) - f(x-h)] / 2h
    /// </summary>
    /// <param name="function">Function to be derivated</param>
    /// <param name="abscissa">Abscissa value where the derivative will be computed at</param>
    /// <param name="step">The absissa increment between this point and the previous one</param>
    /// <returns>Derivative value</returns>
    /// <seealso cref="https://www.cantorsparadise.com/the-best-numerical-derivative-approximation-formulas-998703380948"/>
    private static double CenteredThreePoint(Func<double, double> function, double abscissa = 0, double step = 1)
    {
        return (function(abscissa + step) - function(abscissa - step)) / (2 * step);
    }

    /// <summary>
    /// Computes the derivative of a function (which is discretised) using the central five-points finite difference.
    /// [-f(x+2h) + 8f(x+h) - 8f(x-h) + f(x-2h)] / 12h
    /// </summary>
    /// <param name="function">Function to be derivated</param>
    /// <param name="lowerLimit">Differentiation lower limit</param>
    /// <param name="upperLimit">Differentiation upper limit</param>
    /// <param name="segments">Number of equal segments the differentiation interval is divided into</param>
    /// <returns>1D array (vector) with the discrete derivate for the discretised <paramref name="function"/></returns>
    /// <seealso cref="https://en.wikipedia.org/wiki/Finite_difference_coefficient"/>
    private static double[] CenteredFivePoint(Func<double, double> function, double lowerLimit, double upperLimit, int segments = 1)
    {
        if (segments == 0 || lowerLimit >= upperLimit) return [];

        double[] result = new double[segments + 1];
        double step = (upperLimit - lowerLimit) / segments;
        double step2 = 2 * step;
        double x = lowerLimit + step2;

        result[0] = double.NaN;
        result[1] = double.NaN;
        result[segments] = double.NaN;
        result[segments - 1] = double.NaN;
        for (int j = 2; j < segments - 1; j++)
        {
            result[j] = (function(x - step2) - function(x + step2) + (function(x + step) - function(x - step)) * 8) / (step * 12);
            x += step;
        }

        return result;
    }

    /// <summary>
    /// Point derivative using the central five-points finite difference.
    /// [-f(x+2h) + 8f(x+h) - 8f(x-h) + f(x-2h)] / 12h
    /// </summary>
    /// <param name="function">Function to be derivated</param>
    /// <param name="abscissa">Abscissa value where the derivative will be computed at</param>
    /// <param name="step">The absissa increment between this point and the previous one</param>
    /// <returns>Derivative value</returns>
    /// <seealso cref="https://en.wikipedia.org/wiki/Finite_difference_coefficient"/>
    private static double CenteredFivePoint(Func<double, double> function, double abscissa = 0, double step = 1)
    {
        double step2 = 2 * step;
        return (function(abscissa - step2) - function(abscissa + step2) + (function(abscissa + step) - function(abscissa - step)) * 8) / (step * 12);
    }

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
    private static double[] CenteredSevenPoint(Func<double, double> function, double lowerLimit, double upperLimit, int segments = 1)
    {
        if (segments == 0 || lowerLimit >= upperLimit) return [];

        double[] result = new double[segments + 1];
        double step = (upperLimit - lowerLimit) / segments;
        double step2 = 2 * step;
        double step3 = 3 * step;
        double x = lowerLimit + step3;

        result[0] = double.NaN;
        result[1] = double.NaN;
        result[2] = double.NaN;
        result[segments] = double.NaN;
        result[segments - 1] = double.NaN;
        result[segments - 2] = double.NaN;
        for (int j = 3; j < segments - 2; j++)
        {
            result[j] = (function(x + step3) - function(x - step3) + 9 * (function(x - step2) - function(x + step2)) + 45 * (function(x + step) - function(x - step))) / (step * 60);
            x += step;
        }

        return result;
    }

    /// <summary>
    /// Point derivative using the central seven-points finite difference.
    /// [f(x+3h) - 9f(x+2h) + 45f(x+h) - 45f(x-h) + 9f(x-2h) - f(x-3h)] / 60h
    /// </summary>
    /// <param name="function">Function to be derivated</param>
    /// <param name="abscissa">Abscissa value where the derivative will be computed at</param>
    /// <param name="step">The absissa increment between this point and the previous one</param>
    /// <returns>Derivative value</returns>
    /// <seealso cref="https://en.wikipedia.org/wiki/Finite_difference_coefficient"/>
    private static double CenteredSevenPoint(Func<double, double> function, double abscissa = 0, double step = 1)
    {
        double step2 = 2 * step;
        double step3 = 3 * step;
        return (function(abscissa + step3) - function(abscissa - step3) + 9 * (function(abscissa - step2) - function(abscissa + step2)) + 45 * (function(abscissa + step) - function(abscissa - step))) / (step * 60);
    }

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
    private static double[] CenteredNinePoint(Func<double, double> function, double lowerLimit, double upperLimit, int segments = 1)
    {
        if (segments == 0 || lowerLimit >= upperLimit) return [];

        double[] result = new double[segments + 1];
        double step = (upperLimit - lowerLimit) / segments;
        double step2 = 2 * step;
        double step3 = 3 * step;
        double step4 = 4 * step;
        double x = lowerLimit + step4;

        result[0] = double.NaN;
        result[1] = double.NaN;
        result[2] = double.NaN;
        result[3] = double.NaN;
        result[segments] = double.NaN;
        result[segments - 1] = double.NaN;
        result[segments - 2] = double.NaN;
        result[segments - 3] = double.NaN;
        for (int j = 4; j < segments - 3; j++)
        {
            result[j] = (function(x - step4) - function(x + step4) + (32.0 / 3) * (function(x + step3) - function(x - step3)) + 56 * (function(x - step2) - function(x + step2)) + 224 * (function(x + step) - function(x - step))) / (step * 280);
            x += step;
        }

        return result;
    }

    /// <summary>
    /// Point derivative using the central nine-points finite difference.
    /// [-f(x+4h) + 32/3f(x+3h) - 56f(x+2h) + 224f(x+h) - 224f(x-h) + 56f(x-2h) - 32/3f(x-3h) + f(x-4h)] / 280h
    /// </summary>
    /// <param name="function">Function to be derivated</param>
    /// <param name="abscissa">Abscissa value where the derivative will be computed at</param>
    /// <param name="step">The absissa increment between this point and the previous one</param>
    /// <returns>Derivative value</returns>
    /// <seealso cref="https://en.wikipedia.org/wiki/Finite_difference_coefficient"/>
    private static double CenteredNinePoint(Func<double, double> function, double abscissa = 0, double step = 1)
    {
        double step2 = 2 * step;
        double step3 = 3 * step;
        double step4 = 4 * step;
        return (function(abscissa - step4) - function(abscissa + step4) + (32.0 / 3) * (function(abscissa + step3) - function(abscissa - step3)) + 56 * (function(abscissa - step2) - function(abscissa + step2)) + 224 * (function(abscissa + step) - function(abscissa - step))) / (step * 280);
    }

    /// <summary>
    /// Computes the derivative of a function (which is discretised) using the Savitzky-Golay linear three-point coefficients.
    /// The formula is the same as <see cref="CenteredThreePoint(Func{double, double}, double, double, int)"/>
    /// [f(x+h) - f(x-h)] / 2h
    /// </summary>
    /// <param name="function">Function to be derivated</param>
    /// <param name="lowerLimit">Differentiation lower limit</param>
    /// <param name="upperLimit">Differentiation upper limit</param>
    /// <param name="segments">Number of equal segments the differentiation interval is divided into</param>
    /// <returns>1D array (vector) with the discrete derivate for the discretised <paramref name="function"/></returns>
    /// <seealso cref="https://en.wikipedia.org/wiki/Savitzky%E2%80%93Golay_filter"/>
    private static double[] SGLinearThreePoint(Func<double, double> function, double lowerLimit, double upperLimit, int segments = 1)
    {
        if (segments == 0 || lowerLimit >= upperLimit) return [];

        double[] result = new double[segments + 1];
        double step = (upperLimit - lowerLimit) / segments;
        double x = lowerLimit + step;

        result[0] = double.NaN;
        result[segments] = double.NaN;
        for (int j = 1; j < segments; j++)
        {
            result[j] = (function(x + step) - function(x - step)) / (2 * step);
            x += step;
        }

        return result;
    }

    /// <summary>
    /// Point derivative using Savitzky-Golay's linear three-point coefficients.
    /// The formula is the same as <see cref="CenteredThreePoint(Func{double, double}, double, double)"/>
    /// [f(x+h) - f(x-h)] / 2h
    /// </summary>
    /// <param name="function">Function to be derivated</param>
    /// <param name="abscissa">Abscissa value where the derivative will be computed at</param>
    /// <param name="step">The absissa increment between this point and the previous one</param>
    /// <returns>Derivative value</returns>
    /// <seealso cref="https://en.wikipedia.org/wiki/Finite_difference_coefficient"/>
    private static double SGLinearThreePoint(Func<double, double> function, double abscissa = 0, double step = 1)
    {
        return (function(abscissa + step) - function(abscissa - step)) / (2 * step);
    }

    /// <summary>
    /// Computes the derivative of a function (which is discretised) using the Savitzky-Golay linear five-point coefficients.
    /// [2f(x+2h) + f(x+h) - f(x-h) - 2f(x-2h)] / 10h
    /// </summary>
    /// <param name="function">Function to be derivated</param>
    /// <param name="lowerLimit">Differentiation lower limit</param>
    /// <param name="upperLimit">Differentiation upper limit</param>
    /// <param name="segments">Number of equal segments the differentiation interval is divided into</param>
    /// <returns>1D array (vector) with the discrete derivate for the discretised <paramref name="function"/></returns>
    /// <seealso cref="https://en.wikipedia.org/wiki/Savitzky%E2%80%93Golay_filter"/>
    private static double[] SGLinearFivePoint(Func<double, double> function, double lowerLimit, double upperLimit, int segments = 1)
    {
        if (segments == 0 || lowerLimit >= upperLimit) return [];

        double[] result = new double[segments + 1];
        double step = (upperLimit - lowerLimit) / segments;
        double step2 = 2 * step;
        double x = lowerLimit + step2;

        result[0] = double.NaN;
        result[1] = double.NaN;
        result[segments] = double.NaN;
        result[segments - 1] = double.NaN;
        for (int j = 2; j < segments - 1; j++)
        {
            result[j] = (2 * (function(x + step2) - function(x - step2)) + function(x + step) - function(x - step)) / (step * 10);
            x += step;
        }

        return result;
    }

    /// <summary>
    /// Point derivative using Savitzky-Golay's linear five-point coefficients.
    /// [2f(x+2h) + f(x+h) - f(x-h) - 2f(x-2h)] / 10h
    /// </summary>
    /// <param name="function">Function to be derivated</param>
    /// <param name="abscissa">Abscissa value where the derivative will be computed at</param>
    /// <param name="step">The absissa increment between this point and the previous one</param>
    /// <returns>Derivative value</returns>
    /// <seealso cref="https://en.wikipedia.org/wiki/Savitzky%E2%80%93Golay_filter"/>
    private static double SGLinearFivePoint(Func<double, double> function, double abscissa = 0, double step = 1)
    {
        double step2 = 2 * step;
        return (2 * (function(abscissa + step2) - function(abscissa - step2)) + function(abscissa + step) - function(abscissa - step)) / (step * 10);
    }

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
    private static double[] SGLinearSevenPoint(Func<double, double> function, double lowerLimit, double upperLimit, int segments = 1)
    {
        if (segments == 0 || lowerLimit >= upperLimit) return [];

        double[] result = new double[segments + 1];
        double step = (upperLimit - lowerLimit) / segments;
        double step2 = 2 * step;
        double step3 = 3 * step;
        double x = lowerLimit + step3;

        result[0] = double.NaN;
        result[1] = double.NaN;
        result[2] = double.NaN;
        result[segments] = double.NaN;
        result[segments - 1] = double.NaN;
        result[segments - 2] = double.NaN;
        for (int j = 3; j < segments - 2; j++)
        {
            result[j] = (3 * (function(x + step3) - function(x - step3)) + 2 * (function(x + step2) - function(x - step2)) + function(x + step) - function(x - step)) / (step * 28);
            x += step;
        }

        return result;
    }

    /// <summary>
    /// Point derivative using Savitzky-Golay's linear seven-point coefficients.
    /// [3f(x+3h) + 2f(x+2h) + f(x+h) - f(x-h) - 2f(x-2h) - 3f(x-3h)] / 28h
    /// </summary>
    /// <param name="function">Function to be derivated</param>
    /// <param name="abscissa">Abscissa value where the derivative will be computed at</param>
    /// <param name="step">The absissa increment between this point and the previous one</param>
    /// <returns>Derivative value</returns>
    /// <seealso cref="https://en.wikipedia.org/wiki/Savitzky%E2%80%93Golay_filter"/>
    private static double SGLinearSevenPoint(Func<double, double> function, double abscissa = 0, double step = 1)
    {
        double step2 = 2 * step;
        double step3 = 3 * step;
        return (3 * (function(abscissa + step3) - function(abscissa - step3)) + 2 * (function(abscissa + step2) - function(abscissa - step2)) + function(abscissa + step) - function(abscissa - step)) / (step * 28);
    }

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
    private static double[] SGLinearNinePoint(Func<double, double> function, double lowerLimit, double upperLimit, int segments = 1)
    {
        if (segments == 0 || lowerLimit >= upperLimit) return [];

        double[] result = new double[segments + 1];
        double step = (upperLimit - lowerLimit) / segments;
        double step2 = 2 * step;
        double step3 = 3 * step;
        double step4 = 4 * step;
        double x = lowerLimit + step4;

        result[0] = double.NaN;
        result[1] = double.NaN;
        result[2] = double.NaN;
        result[3] = double.NaN;
        result[segments] = double.NaN;
        result[segments - 1] = double.NaN;
        result[segments - 2] = double.NaN;
        result[segments - 3] = double.NaN;
        for (int j = 4; j < segments - 3; j++)
        {
            result[j] = (4 * (function(x + step4) - function(x - step4)) + 3 * (function(x + step3) - function(x - step3)) + 2 * (function(x + step2) - function(x - step2)) + function(x + step) - function(x - step)) / (step * 60);
            x += step;
        }

        return result;
    }

    /// <summary>
    /// Point derivative using Savitzky-Golay's linear nine-point coefficients.
    /// [4f(x+4h) + 3f(x+3h) + 2f(x+2h) + f(x+h) - f(x-h) - 2f(x-2h) - 3f(x-3h) - 4f(x-4h)] / 60h
    /// </summary>
    /// <param name="function">Function to be derivated</param>
    /// <param name="abscissa">Abscissa value where the derivative will be computed at</param>
    /// <param name="step">The absissa increment between this point and the previous one</param>
    /// <returns>Derivative value</returns>
    /// <seealso cref="https://en.wikipedia.org/wiki/Savitzky%E2%80%93Golay_filter"/>
    private static double SGLinearNinePoint(Func<double, double> function, double abscissa = 0, double step = 1)
    {
        double step2 = 2 * step;
        double step3 = 3 * step;
        double step4 = 4 * step;
        return (4 * (function(abscissa + step4) - function(abscissa - step4)) + 3 * (function(abscissa + step3) - function(abscissa - step3)) + 2 * (function(abscissa + step2) - function(abscissa - step2)) + function(abscissa + step) - function(abscissa - step)) / (step * 60);
    }

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
    private static double[] SGCubicFivePoint(Func<double, double> function, double lowerLimit, double upperLimit, int segments = 1)
    {
        if (segments == 0 || lowerLimit >= upperLimit) return [];

        double[] result = new double[segments + 1];
        double step = (upperLimit - lowerLimit) / segments;
        double step2 = 2 * step;
        double x = lowerLimit + step2;

        result[0] = double.NaN;
        result[1] = double.NaN;
        result[segments] = double.NaN;
        result[segments - 1] = double.NaN;
        for (int j = 2; j < segments - 1; j++)
        {
            result[j] = (function(x - step2) - function(x + step2) + 8 * (function(x + step) - function(x - step))) / (step * 12);
            x += step;
        }

        return result;
    }

    /// <summary>
    /// Point derivative using Savitzky-Golay's cubic/quartic five-point coefficients.
    /// The formula is the same as <see cref="CenteredFivePoint(Func{double, double}, double, double)"/>
    /// [-f(x+2h) + 8f(x+h) - 8f(x-h) - f(x-2h)] / 12h
    /// </summary>
    /// <param name="function">Function to be derivated</param>
    /// <param name="abscissa">Abscissa value where the derivative will be computed at</param>
    /// <param name="step">The absissa increment between this point and the previous one</param>
    /// <returns>Derivative value</returns>
    /// <seealso cref="https://en.wikipedia.org/wiki/Savitzky%E2%80%93Golay_filter"/>
    private static double SGCubicFivePoint(Func<double, double> function, double abscissa = 0, double step = 1)
    {
        double step2 = 2 * step;
        return (function(abscissa - step2) - function(abscissa + step2) + 8 * (function(abscissa + step) - function(abscissa - step))) / (step * 12);
    }

    /// <summary>
    /// Computes the derivative of a function (which is discretised) using the Savitzky-Golay cubic/quartic seven-point coefficients.
    /// [-22f(x+3h) + 67f(x+2h) + 58f(x+h) - 58f(x-h) - 67f(x-2h) + 22f(x-3h)] / 252h
    /// </summary>
    /// <param name="function">Function to be derivated</param>
    /// <param name="lowerLimit">Differentiation lower limit</param>
    /// <param name="upperLimit">Differentiation upper limit</param>
    /// <param name="segments">Number of equal segments the differentiation interval is divided into</param>
    /// <returns>1D array (vector) with the discrete derivate for the discretised <paramref name="function"/></returns>
    /// <seealso cref="https://en.wikipedia.org/wiki/Savitzky%E2%80%93Golay_filter"/>
    private static double[] SGCubicSevenPoint(Func<double, double> function, double lowerLimit, double upperLimit, int segments = 1)
    {
        if (segments == 0 || lowerLimit >= upperLimit) return [];

        double[] result = new double[segments + 1];
        double step = (upperLimit - lowerLimit) / segments;
        double step2 = 2 * step;
        double step3 = 3 * step;
        double x = lowerLimit + step3;

        result[0] = double.NaN;
        result[1] = double.NaN;
        result[2] = double.NaN;
        result[segments] = double.NaN;
        result[segments - 1] = double.NaN;
        result[segments - 2] = double.NaN;
        for (int j = 3; j < segments - 2; j++)
        {
            result[j] = (22 * (function(x - step3) - function(x + step3)) + 67 * (function(x + step2) - function(x - step2)) + 58 * (function(x + step) - function(x - step))) / (step * 252);
            x += step;
        }

        return result;
    }

    /// <summary>
    /// Point derivative using Savitzky-Golay's cubic/quartic seven-point coefficients.
    /// [-22f(x+3h) + 67f(x+2h) + 58f(x+h) - 58f(x-h) - 67f(x-2h) + 22f(x-3h)] / 252h
    /// </summary>
    /// <param name="function">Function to be derivated</param>
    /// <param name="abscissa">Abscissa value where the derivative will be computed at</param>
    /// <param name="step">The absissa increment between this point and the previous one</param>
    /// <returns>Derivative value</returns>
    /// <seealso cref="https://en.wikipedia.org/wiki/Savitzky%E2%80%93Golay_filter"/>
    private static double SGCubicSevenPoint(Func<double, double> function, double abscissa = 0, double step = 1)
    {
        double step2 = 2 * step;
        double step3 = 3 * step;
        return (22 * (function(abscissa - step3) - function(abscissa + step3)) + 67 * (function(abscissa + step2) - function(abscissa - step2)) + 58 * (function(abscissa + step) - function(abscissa - step))) / (step * 252);
    }

    /// <summary>
    /// Computes the derivative of a function (which is discretised) using the Savitzky-Golay cubic/quartic nine-point coefficients.
    /// [-86f(x+4h) + 142f(x+3h) + 193f(x+2h) + 126f(x+h) - 126f(x-h) - 193f(x-2h) + 142f(x-3h) - 86f(x-4h)] / 1188h
    /// </summary>
    /// <param name="function">Function to be derivated</param>
    /// <param name="lowerLimit">Differentiation lower limit</param>
    /// <param name="upperLimit">Differentiation upper limit</param>
    /// <param name="segments">Number of equal segments the differentiation interval is divided into</param>
    /// <returns>1D array (vector) with the discrete derivate for the discretised <paramref name="function"/></returns>
    /// <seealso cref="https://en.wikipedia.org/wiki/Savitzky%E2%80%93Golay_filter"/>
    private static double[] SGCubicNinePoint(Func<double, double> function, double lowerLimit, double upperLimit, int segments = 1)
    {
        if (segments == 0 || lowerLimit >= upperLimit) return [];

        double[] result = new double[segments + 1];
        double step = (upperLimit - lowerLimit) / segments;
        double step2 = 2 * step;
        double step3 = 3 * step;
        double step4 = 4 * step;
        double x = lowerLimit + step4;

        result[0] = double.NaN;
        result[1] = double.NaN;
        result[2] = double.NaN;
        result[3] = double.NaN;
        result[segments] = double.NaN;
        result[segments - 1] = double.NaN;
        result[segments - 2] = double.NaN;
        result[segments - 3] = double.NaN;
        for (int j = 4; j < segments - 3; j++)
        {
            result[j] = (86 * (function(x - step4) - function(x + step4)) + 142 * (function(x + step3) - function(x - step3)) + 193 * (function(x + step2) - function(x - step2)) + 126 * (function(x + step) - function(x - step))) / (step * 1188);
            x += step;
        }

        return result;
    }

    /// <summary>
    /// Point derivative using Savitzky-Golay's cubic/quartic nine-point coefficients.
    /// [-86f(x+4h) + 142f(x+3h) + 193f(x+2h) + 126f(x+h) - 126f(x-h) - 193f(x-2h) + 142f(x-3h) - 86f(x-4h)] / 1188h
    /// </summary>
    /// <param name="function">Function to be derivated</param>
    /// <param name="abscissa">Abscissa value where the derivative will be computed at</param>
    /// <param name="step">The absissa increment between this point and the previous one</param>
    /// <returns>Derivative value</returns>
    /// <seealso cref="https://en.wikipedia.org/wiki/Savitzky%E2%80%93Golay_filter"/>
    private static double SGCubicNinePoint(Func<double, double> function, double abscissa = 0, double step = 1)
    {
        double step2 = 2 * step;
        double step3 = 3 * step;
        double step4 = 4 * step;
        return (86 * (function(abscissa - step4) - function(abscissa + step4)) + 142 * (function(abscissa + step3) - function(abscissa - step3)) + 193 * (function(abscissa + step2) - function(abscissa - step2)) + 126 * (function(abscissa + step) - function(abscissa - step))) / (step * 1188);
    }

}