using System.Numerics;

namespace SignalAnalysis;

//https://stackoverflow.com/questions/373186/mathematical-function-differentiation-with-c

public interface IFunction<T>
{
    // Since operator () can't be overloaded, we'll use this trick.
    double this[T arg] { get; }
}

delegate double RealFunction<T>(T arg);

class Function<T> : IFunction<T>
{
    readonly RealFunction<T> func;

    public Function(RealFunction<T> func)
    {
        this.func = func;
    }

    public double this[T arg]
    {
        get
        {
            return func(arg);
        }
    }
}

public class Derivative<T, TResult> where T : INumber<T>
{
    private readonly IFunction<T> func;
    private readonly DerivativeMethod method = DerivativeMethod.CenteredThreePoint;
    public readonly double step = 10e-6;

    public Derivative(IFunction<T> func, double step, DerivativeMethod method = DerivativeMethod.CenteredThreePoint)
    {
        this.func = func;
        this.method = method;
        this.step = step;
    }

    public double this[T arg] => method switch
    {
        DerivativeMethod.CenteredThreePoint => CenteredThreePoint(arg),
        DerivativeMethod.CenteredFivePoint => CenteredFivePoint(arg),
        _ => throw new ArgumentOutOfRangeException(nameof(method), $"Not expected derivative method: {method}"),
    };

    /// <summary>
    /// [f(x+h) - f(x-h)] / 2h
    /// </summary>
    /// <returns></returns>
    private double CenteredThreePoint(T arg)
    {
        //return  (func[arg + h] - func[arg - h]) / (h * 2);
        return Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 => (func[arg + T.CreateChecked(1)] - func[arg - T.CreateChecked(1)]) / (step * 2),
            _ => (func[arg + T.CreateChecked(step)] - func[arg - T.CreateChecked(step)]) / (step * 2)
        };
    }

    /// <summary>
    /// [f(x-2h) - 8f(x-h) + 8f(x+h) - f(x+2h)] / 12h
    /// </summary>
    /// <returns></returns>
    private double CenteredFivePoint(T arg)
    {
        double step2 = step * 2;
        return Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 => (func[arg - T.CreateChecked(2)] - func[arg + T.CreateChecked(2)] - func[arg - T.CreateChecked(1)]) / (step2 * 6),
            _ => (func[arg - T.CreateChecked(step2)] - func[arg + T.CreateChecked(step2)] + (func[arg + T.CreateChecked(step)] - func[arg - T.CreateChecked(step)]) * 8) / (step2 * 6)
        };
        
    }
}

public enum DerivativeMethod
{
    CenteredThreePoint,
    CenteredFivePoint
}
