namespace SignalAnalysis;

//https://stackoverflow.com/questions/373186/mathematical-function-differentiation-with-c

public interface IFunction
{
    // Since operator () can't be overloaded, we'll use this trick.
    double this[double arg] { get; }
}

delegate double RealFunction(double arg);

class Function : IFunction
{
    readonly RealFunction func;

    public Function(RealFunction func)
    {
        this.func = func;
    }

    public double this[double arg]
    {
        get
        {
            return func(arg);
        }
    }
}

public class Derivative
{
    private readonly IFunction func;
    private readonly DerivativeMethod method = DerivativeMethod.CenteredThreePoint;
    public readonly double h = 10e-6;

    public Derivative(IFunction func, double step, DerivativeMethod method = DerivativeMethod.CenteredThreePoint)
    {
        this.func = func;
        this.method = method;
        this.h = step;
    }

    public double this[double arg] => method switch
    {
        DerivativeMethod.CenteredThreePoint => CenteredThreePoint(arg),
        DerivativeMethod.CenteredFivePoint => CenteredFivePoint(arg),
        _ => throw new ArgumentOutOfRangeException(nameof(method), $"Not expected derivative method: {method}"),
    };

    /// <summary>
    /// [f(x+h) - f(x-h)] / 2h
    /// </summary>
    /// <returns></returns>
    private double CenteredThreePoint(double arg)
    {
        return (func[arg + h] - func[arg - h]) / (h * 2);
    }

    /// <summary>
    /// [f(x-2h) - 8f(x-h) + 8f(x+h) - f(x+2h)] / 12h
    /// </summary>
    /// <returns></returns>
    private double CenteredFivePoint(double arg)
    {
        double h2 = h * 2;
        return (func[arg - h2] - func[arg + h2] + (func[arg + h] - func[arg - h]) * 8) / (h2 * 6);
    }
}

public enum DerivativeMethod
{
    CenteredThreePoint,
    CenteredFivePoint
}
