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

public class Derivative<T> where T : INumber<T>
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
        DerivativeMethod.BackwardOnePoint => BackwardOnePoint(arg),
        DerivativeMethod.ForwardOnePoint => ForwardOnePoint(arg),
        DerivativeMethod.CenteredThreePoint => CenteredThreePoint(arg),
        DerivativeMethod.CenteredFivePoint => CenteredFivePoint(arg),
        _ => throw new ArgumentOutOfRangeException(nameof(method), $"Not expected derivative method: {method}"),
    };

    /// <summary>
    /// [f(x) - f(x-h)] / h
    /// https://www.cantorsparadise.com/the-best-numerical-derivative-approximation-formulas-998703380948
    /// </summary>
    /// <returns></returns>
    private double BackwardOnePoint(T arg)
    {
        //return  (func[arg + h] - func[arg - h]) / (h * 2);
        return Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 => (func[arg] - func[arg - T.CreateChecked(1)]) / (step),
            _ => (func[arg] - func[arg - T.CreateChecked(step)]) / (step)
        };
    }

    /// <summary>
    /// [f(x+h) - f(x)] / h
    /// </summary>
    /// <returns></returns>
    private double ForwardOnePoint(T arg)
    {
        //return  (func[arg + h] - func[arg - h]) / (h * 2);
        return Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 => (func[arg + T.CreateChecked(1)] - func[arg]) / (step),
            _ => (func[arg + T.CreateChecked(step)] - func[arg]) / (step)
        };
    }

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
            TypeCode.Int32 => (func[arg - T.CreateChecked(2)] - func[arg + T.CreateChecked(2)] + (func[arg + T.CreateChecked(1)] - func[arg - T.CreateChecked(1)]) * 8) / (step2 * 6),
            _ => (func[arg - T.CreateChecked(step2)] - func[arg + T.CreateChecked(step2)] + (func[arg + T.CreateChecked(step)] - func[arg - T.CreateChecked(step)]) * 8) / (step2 * 6)
        };
        
    }
    
    /// <summary>
    /// [-f(x-3h) + 9f(x-2h) - 45f(x-h) + 0f(x) + 45f(x+h) −9f(x+2h) + f(x+3h)] / 60h
    /// </summary>
    /// <returns></returns>
    private double CenteredSevenPoint(T arg)
    {
        double step2 = step * 2;
        double step3 = step * 3;
        return Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 => (func[arg + T.CreateChecked(3)] - func[arg - T.CreateChecked(3)] + 9 * (func[arg - T.CreateChecked(2)] - func[arg + T.CreateChecked(2)]) + 45 * (func[arg + T.CreateChecked(1)] - func[arg - T.CreateChecked(1)])) / (step * 60),
            _ => (-func[arg - T.CreateChecked(step3)] + func[arg + T.CreateChecked(step3)] + 9 * (func[arg - T.CreateChecked(step2)] - func[arg + T.CreateChecked(step2)]) + 45 * (func[arg + T.CreateChecked(step)] - func[arg - T.CreateChecked(step)])) / (step * 60)
        };
        
    }
    
    /// <summary>
    /// [f(x-4h) - 8/3 * f(x-3h) + 56 * f(x-2h) - 224 * f(x-h)	+ 0F(x) + 224 * f(x+h) − 56 * f(x+2h) + 8/3 * f(x+3h) - f(x+4h)] / 280h
    /// </summary>
    /// <returns></returns>
    private double CenteredNinePoint(T arg)
    {
        double step2 = step * 2;
        double step3 = step * 3;
        double step4 = step * 4;
        return Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 => (func[arg - T.CreateChecked(4)] - func[arg + T.CreateChecked(4)] + (8/3) * (func[arg + T.CreateChecked(3)] - func[arg - T.CreateChecked(3)]) + 56 * (func[arg - T.CreateChecked(2)] - func[arg + T.CreateChecked(2)]) + 224 * (func[arg + T.CreateChecked(1)] - func[arg - T.CreateChecked(1)])) / (step * 280),
            _ => (func[arg - T.CreateChecked(step4)] - func[arg + T.CreateChecked(step4)] + (8/3) * (func[arg + T.CreateChecked(step3)] - func[arg - T.CreateChecked(step3)]) + 56 * (func[arg - T.CreateChecked(step2)] - func[arg + T.CreateChecked(step2)]) + 224 * (func[arg + T.CreateChecked(step)] - func[arg - T.CreateChecked(step)])) / (step * 280)
        };
        
    }
    
    /// <summary>
    /// [-f(x-h) + f(0) + f(x+h)] / 2h
    /// </summary>
    /// <returns></returns>
    private double SGLinearThreePoint (T arg)
    {
        return Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 => (func[arg + T.CreateChecked(1)] - func[arg - T.CreateChecked(1)]) / (step * 2),
            _ => (func[arg + T.CreateChecked(step)] - func[arg - T.CreateChecked(step)]) / (step * 2)
        };
    }
    
    /// <summary>
    /// [-2 * f(x-2h) - f(x-h) + f(0) + f(x+h) + 2 * f(x+2h)] / 10h
    /// </summary>
    /// <returns></returns>
    private double SGLinearFivePoint (T arg)
    {
        double step2 = step * 2;
        return Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 => (2 * (func[arg + T.CreateChecked(2)] - func[arg - T.CreateChecked(2)]) + func[arg + T.CreateChecked(1)] - func[arg - T.CreateChecked(1)]) / (step * 10),
            _ => (2 * (func[arg + T.CreateChecked(step2)] - func[arg - T.CreateChecked(step2)]) + func[arg + T.CreateChecked(step)] - func[arg - T.CreateChecked(step)]) / (step * 10)
        };
    }
    
    /// <summary>
    /// [-3 * f(x-3h) - 2 * f(x-2h) - f(x-h) + f(0) + f(x+h) + 2 * f(x+2h) + 3 * f(x+3h)] / 28h
    /// </summary>
    /// <returns></returns>
    private double SGLinearSevenPoint (T arg)
    {
        double step2 = step * 2;
        double step3 = step * 2;
        return Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 => (3 * (func[arg + T.CreateChecked(3)] - func[arg - T.CreateChecked(3)]) + 2 * (func[arg + T.CreateChecked(2)] - func[arg - T.CreateChecked(2)]) + func[arg + T.CreateChecked(1)] - func[arg - T.CreateChecked(1)]) / (step * 28),
            _ => (3 * (func[arg + T.CreateChecked(step3)] - func[arg - T.CreateChecked(step3)]) + 2 * (func[arg + T.CreateChecked(step2)] - func[arg - T.CreateChecked(step2)]) + func[arg + T.CreateChecked(step)] - func[arg - T.CreateChecked(step)]) / (step * 28)
        };
    }
    
    /// <summary>
    /// [-4 * f(x-4h) - 3 * f(x-3h) - 2 * f(x-2h) - f(x-h) + f(0) + f(x+h) + 2 * f(x+2h) + 3 * f(x+3h) + 4 * f(x+4h)] / 60h
    /// </summary>
    /// <returns></returns>
    private double SGLinearninePoint (T arg)
    {
        double step2 = step * 2;
        double step3 = step * 2;
        double step4 = step * 2;
        return Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 => (4 * (func[arg + T.CreateChecked(4)] - func[arg - T.CreateChecked(4)]) + 3 * (func[arg + T.CreateChecked(3)] - func[arg - T.CreateChecked(3)]) + 2 * (func[arg + T.CreateChecked(2)] - func[arg - T.CreateChecked(2)]) + func[arg + T.CreateChecked(1)] - func[arg - T.CreateChecked(1)]) / (step * 60),
            _ => (4 * (func[arg + T.CreateChecked(step4)] - func[arg - T.CreateChecked(step4)]) + 3 * (func[arg + T.CreateChecked(step3)] - func[arg - T.CreateChecked(step3)]) + 2 * (func[arg + T.CreateChecked(step2)] - func[arg - T.CreateChecked(step2)]) + func[arg + T.CreateChecked(step)] - func[arg - T.CreateChecked(step)]) / (step * 60)
        };
    }
    
    /// <summary>
    /// [f(x-2h) - 8f(x-h) + 8f(x+h) - f(x+2h)] / 12h
    /// </summary>
    /// <returns></returns>
    private double SGCubicFivePoint (T arg)
    {
        double step2 = step * 2;
        return Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 => (func[arg - T.CreateChecked(2)] - func[arg + T.CreateChecked(2)] + (func[arg + T.CreateChecked(1)] - func[arg - T.CreateChecked(1)]) * 8) / (step2 * 6),
            _ => (func[arg - T.CreateChecked(step2)] - func[arg + T.CreateChecked(step2)] + (func[arg + T.CreateChecked(step)] - func[arg - T.CreateChecked(step)]) * 8) / (step2 * 6)
        };
    }
    
    /// <summary>
    /// [22 * f(x-3h) - 67 * f(x-2h) - 58 * f(x-h) + 0 * f(x) + 58 * f(x+h) + 67 * f(x+2h) - 22 * f(x+3h)] / 252h
    /// </summary>
    /// <returns></returns>
    private double SGCubicSevenPoint (T arg)
    {
        double step2 = step * 2;
        double step3 = step * 3;
        return Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 => (22 * (func[arg - T.CreateChecked(3)] - func[arg + T.CreateChecked(3)]) + 67 * (func[arg + T.CreateChecked(2)] - func[arg - T.CreateChecked(2)]) + 58 * (func[arg + T.CreateChecked(1)] - func[arg - T.CreateChecked(1)])) / (step * 252),
            _ => (22 * (func[arg - T.CreateChecked(step3)] - func[arg + T.CreateChecked(step3)]) + 67 * (func[arg + T.CreateChecked(step2)] - func[arg - T.CreateChecked(step2)]) + 58 * (func[arg + T.CreateChecked(step)] - func[arg - T.CreateChecked(step)])) / (step * 252)
        };
    }
    
    /// <summary>
    /// [86 * f(x-4h) - 142 * f(x-3h) - 67 * f(x-2h) - 58 * f(x-h) + 0 * f(x) + 58 * f(x+h) + 67 * f(x+2h) + 142 * f(x+3h) - 86 * f(x+4h)] / 1188h
    /// </summary>
    /// <returns></returns>
    private double SGCubicNinePoint (T arg)
    {
        double step2 = step * 2;
        double step3 = step * 3;
        double step4 = step * 4;
        return Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 => (86 * (func[arg - T.CreateChecked(4)] - func[arg + T.CreateChecked(4)]) + 142 * (func[arg + T.CreateChecked(3)] - func[arg - T.CreateChecked(3)]) + 193 * (func[arg + T.CreateChecked(2)] - func[arg - T.CreateChecked(2)]) + 126 * (func[arg + T.CreateChecked(1)] - func[arg - T.CreateChecked(1)])) / (step * 1188),
            _ => (86 * (func[arg - T.CreateChecked(step4)] - func[arg + T.CreateChecked(step4)]) + 142 * (func[arg + T.CreateChecked(step3)] - func[arg - T.CreateChecked(step3)]) + 193 * (func[arg + T.CreateChecked(step2)] - func[arg - T.CreateChecked(step2)]) + 126 * (func[arg + T.CreateChecked(step)] - func[arg - T.CreateChecked(step)])) / (step * 1188)
        };
    }
}

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
