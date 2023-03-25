using System.Numerics;

namespace SignalAnalysis;

//https://stackoverflow.com/questions/373186/mathematical-function-differentiation-with-c

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
    private IFunction<T> Function { get; set; }
    private DerivativeMethod Method { get; set; } = DerivativeMethod.CenteredThreePoint;
    public double Step { get; set; }  = 10e-6;

    public Derivative(IFunction<T> func, double step, DerivativeMethod method = DerivativeMethod.CenteredThreePoint)
    {
        this.Function = func;
        this.Method = method;
        this.Step = step;
    }

    public double this[T arg] => Method switch
    {
        DerivativeMethod.BackwardOnePoint => BackwardOnePoint(arg),
        DerivativeMethod.ForwardOnePoint => ForwardOnePoint(arg),
        DerivativeMethod.CenteredThreePoint => CenteredThreePoint(arg),
        DerivativeMethod.CenteredFivePoint => CenteredFivePoint(arg),
        DerivativeMethod.CenteredSevenPoint => CenteredSevenPoint(arg),
        DerivativeMethod.CenteredNinePoint => CenteredNinePoint(arg),
        DerivativeMethod.SGLinearThreePoint => SGLinearThreePoint(arg),
        DerivativeMethod.SGLinearFivePoint => SGLinearFivePoint(arg),
        DerivativeMethod.SGLinearSevenPoint => SGLinearSevenPoint(arg),
        DerivativeMethod.SGLinearNinePoint => SGLinearNinePoint(arg),
        DerivativeMethod.SGCubicFivePoint => SGCubicFivePoint(arg),
        DerivativeMethod.SGCubicSevenPoint => SGCubicSevenPoint(arg),
        DerivativeMethod.SGCubicNinePoint => SGCubicNinePoint(arg),
        _ => throw new ArgumentOutOfRangeException(nameof(Method), $"Not expected derivative method: {Method}"),
    };

    public double[] Derivate(Func<double, double> function, DerivativeMethod method = DerivativeMethod.CenteredThreePoint, double lowerLimit = 0, double upperLimit = 1, int segments = 1)
    {
        double[] result = new double[segments + 1];

        int indexStart = 0, indexEnd = segments;
        switch (method)
        {
            case DerivativeMethod.BackwardOnePoint:
                indexStart = 1;
                indexEnd = signal.Length;
                break;
            case DerivativeMethod.ForwardOnePoint:
                indexStart = 0;
                indexEnd = signal.Length - 1;
                break;
            case DerivativeMethod.CenteredThreePoint or DerivativeMethod.SGLinearThreePoint:
                indexStart = 1;
                indexEnd = signal.Length - 1;
                break;
            case DerivativeMethod.CenteredFivePoint or DerivativeMethod.SGLinearFivePoint or DerivativeMethod.SGCubicFivePoint:
                indexStart = 2;
                indexEnd = signal.Length - 2;
                break;
            case DerivativeMethod.CenteredSevenPoint or DerivativeMethod.SGLinearSevenPoint or DerivativeMethod.SGCubicSevenPoint:
                indexStart = 3;
                indexEnd = signal.Length - 3;
                break;
            case DerivativeMethod.CenteredNinePoint or DerivativeMethod.SGLinearNinePoint or DerivativeMethod.SGCubicNinePoint:
                indexStart = 4;
                indexEnd = signal.Length - 4;
                break;
        }


        return result;
    }

    public double[] Derivate(double[] array)
    {
        double[] result = new double[array.Length];

        return result;
    }

    public double[] DerivateArray(double[] array)
    {
        int indexStart = 0, indexEnd = array.Length - 1;
        double[] result = new double[array.Length];

        switch (Method)
        {
            case DerivativeMethod.BackwardOnePoint:
                indexStart = 1;
                indexEnd = array.Length;
                break;
            case DerivativeMethod.ForwardOnePoint:
                indexStart = 0;
                indexEnd = array.Length - 1;
                break;
            case DerivativeMethod.CenteredThreePoint or DerivativeMethod.SGLinearThreePoint:
                indexStart = 1;
                indexEnd = array.Length - 1;
                break;
            case DerivativeMethod.CenteredFivePoint or DerivativeMethod.SGLinearFivePoint or DerivativeMethod.SGCubicFivePoint:
                indexStart = 2;
                indexEnd = array.Length - 2;
                break;
            case DerivativeMethod.CenteredSevenPoint or DerivativeMethod.SGLinearSevenPoint or DerivativeMethod.SGCubicSevenPoint:
                indexStart = 3;
                indexEnd = array.Length - 3;
                break;
            case DerivativeMethod.CenteredNinePoint or DerivativeMethod.SGLinearNinePoint or DerivativeMethod.SGCubicNinePoint:
                indexStart = 4;
                indexEnd = array.Length - 4;
                break;
        }

        for (int i = indexStart; i < indexEnd; i++)
            result[i] = this[T.CreateChecked(i)];

        return result;

    }

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
            TypeCode.Int32 => (Function[arg] - Function[arg - T.CreateChecked(1)]) / (Step),
            _ => (Function[arg] - Function[arg - T.CreateChecked(Step)]) / (Step)
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
            TypeCode.Int32 => (Function[arg + T.CreateChecked(1)] - Function[arg]) / (Step),
            _ => (Function[arg + T.CreateChecked(Step)] - Function[arg]) / (Step)
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
            TypeCode.Int32 => (Function[arg + T.CreateChecked(1)] - Function[arg - T.CreateChecked(1)]) / (Step * 2),
            _ => (Function[arg + T.CreateChecked(Step)] - Function[arg - T.CreateChecked(Step)]) / (Step * 2)
        };
    }

    /// <summary>
    /// [f(x-2h) - 8f(x-h) + 8f(x+h) - f(x+2h)] / 12h
    /// </summary>
    /// <returns></returns>
    private double CenteredFivePoint(T arg)
    {
        double step2 = Step * 2;
        return Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 => (Function[arg - T.CreateChecked(2)] - Function[arg + T.CreateChecked(2)] + (Function[arg + T.CreateChecked(1)] - Function[arg - T.CreateChecked(1)]) * 8) / (step2 * 6),
            _ => (Function[arg - T.CreateChecked(step2)] - Function[arg + T.CreateChecked(step2)] + (Function[arg + T.CreateChecked(Step)] - Function[arg - T.CreateChecked(Step)]) * 8) / (step2 * 6)
        };
        
    }
    
    /// <summary>
    /// [-f(x-3h) + 9f(x-2h) - 45f(x-h) + 0f(x) + 45f(x+h) −9f(x+2h) + f(x+3h)] / 60h
    /// </summary>
    /// <returns></returns>
    private double CenteredSevenPoint(T arg)
    {
        double step2 = Step * 2;
        double step3 = Step * 3;
        return Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 => (Function[arg + T.CreateChecked(3)] - Function[arg - T.CreateChecked(3)] + 9 * (Function[arg - T.CreateChecked(2)] - Function[arg + T.CreateChecked(2)]) + 45 * (Function[arg + T.CreateChecked(1)] - Function[arg - T.CreateChecked(1)])) / (Step * 60),
            _ => (-Function[arg - T.CreateChecked(step3)] + Function[arg + T.CreateChecked(step3)] + 9 * (Function[arg - T.CreateChecked(step2)] - Function[arg + T.CreateChecked(step2)]) + 45 * (Function[arg + T.CreateChecked(Step)] - Function[arg - T.CreateChecked(Step)])) / (Step * 60)
        };
        
    }
    
    /// <summary>
    /// [f(x-4h) - 8/3 * f(x-3h) + 56 * f(x-2h) - 224 * f(x-h)	+ 0F(x) + 224 * f(x+h) − 56 * f(x+2h) + 8/3 * f(x+3h) - f(x+4h)] / 280h
    /// </summary>
    /// <returns></returns>
    private double CenteredNinePoint(T arg)
    {
        double step2 = Step * 2;
        double step3 = Step * 3;
        double step4 = Step * 4;
        return Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 => (Function[arg - T.CreateChecked(4)] - Function[arg + T.CreateChecked(4)] + (8/3) * (Function[arg + T.CreateChecked(3)] - Function[arg - T.CreateChecked(3)]) + 56 * (Function[arg - T.CreateChecked(2)] - Function[arg + T.CreateChecked(2)]) + 224 * (Function[arg + T.CreateChecked(1)] - Function[arg - T.CreateChecked(1)])) / (Step * 280),
            _ => (Function[arg - T.CreateChecked(step4)] - Function[arg + T.CreateChecked(step4)] + (8/3) * (Function[arg + T.CreateChecked(step3)] - Function[arg - T.CreateChecked(step3)]) + 56 * (Function[arg - T.CreateChecked(step2)] - Function[arg + T.CreateChecked(step2)]) + 224 * (Function[arg + T.CreateChecked(Step)] - Function[arg - T.CreateChecked(Step)])) / (Step * 280)
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
            TypeCode.Int32 => (Function[arg + T.CreateChecked(1)] - Function[arg - T.CreateChecked(1)]) / (Step * 2),
            _ => (Function[arg + T.CreateChecked(Step)] - Function[arg - T.CreateChecked(Step)]) / (Step * 2)
        };
    }
    
    /// <summary>
    /// [-2 * f(x-2h) - f(x-h) + f(0) + f(x+h) + 2 * f(x+2h)] / 10h
    /// </summary>
    /// <returns></returns>
    private double SGLinearFivePoint (T arg)
    {
        double step2 = Step * 2;
        return Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 => (2 * (Function[arg + T.CreateChecked(2)] - Function[arg - T.CreateChecked(2)]) + Function[arg + T.CreateChecked(1)] - Function[arg - T.CreateChecked(1)]) / (Step * 10),
            _ => (2 * (Function[arg + T.CreateChecked(step2)] - Function[arg - T.CreateChecked(step2)]) + Function[arg + T.CreateChecked(Step)] - Function[arg - T.CreateChecked(Step)]) / (Step * 10)
        };
    }
    
    /// <summary>
    /// [-3 * f(x-3h) - 2 * f(x-2h) - f(x-h) + f(0) + f(x+h) + 2 * f(x+2h) + 3 * f(x+3h)] / 28h
    /// </summary>
    /// <returns></returns>
    private double SGLinearSevenPoint (T arg)
    {
        double step2 = Step * 2;
        double step3 = Step * 2;
        return Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 => (3 * (Function[arg + T.CreateChecked(3)] - Function[arg - T.CreateChecked(3)]) + 2 * (Function[arg + T.CreateChecked(2)] - Function[arg - T.CreateChecked(2)]) + Function[arg + T.CreateChecked(1)] - Function[arg - T.CreateChecked(1)]) / (Step * 28),
            _ => (3 * (Function[arg + T.CreateChecked(step3)] - Function[arg - T.CreateChecked(step3)]) + 2 * (Function[arg + T.CreateChecked(step2)] - Function[arg - T.CreateChecked(step2)]) + Function[arg + T.CreateChecked(Step)] - Function[arg - T.CreateChecked(Step)]) / (Step * 28)
        };
    }
    
    /// <summary>
    /// [-4 * f(x-4h) - 3 * f(x-3h) - 2 * f(x-2h) - f(x-h) + f(0) + f(x+h) + 2 * f(x+2h) + 3 * f(x+3h) + 4 * f(x+4h)] / 60h
    /// </summary>
    /// <returns></returns>
    private double SGLinearNinePoint (T arg)
    {
        double step2 = Step * 2;
        double step3 = Step * 2;
        double step4 = Step * 2;
        return Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 => (4 * (Function[arg + T.CreateChecked(4)] - Function[arg - T.CreateChecked(4)]) + 3 * (Function[arg + T.CreateChecked(3)] - Function[arg - T.CreateChecked(3)]) + 2 * (Function[arg + T.CreateChecked(2)] - Function[arg - T.CreateChecked(2)]) + Function[arg + T.CreateChecked(1)] - Function[arg - T.CreateChecked(1)]) / (Step * 60),
            _ => (4 * (Function[arg + T.CreateChecked(step4)] - Function[arg - T.CreateChecked(step4)]) + 3 * (Function[arg + T.CreateChecked(step3)] - Function[arg - T.CreateChecked(step3)]) + 2 * (Function[arg + T.CreateChecked(step2)] - Function[arg - T.CreateChecked(step2)]) + Function[arg + T.CreateChecked(Step)] - Function[arg - T.CreateChecked(Step)]) / (Step * 60)
        };
    }
    
    /// <summary>
    /// [f(x-2h) - 8f(x-h) + 8f(x+h) - f(x+2h)] / 12h
    /// </summary>
    /// <returns></returns>
    private double SGCubicFivePoint (T arg)
    {
        double step2 = Step * 2;
        return Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 => (Function[arg - T.CreateChecked(2)] - Function[arg + T.CreateChecked(2)] + (Function[arg + T.CreateChecked(1)] - Function[arg - T.CreateChecked(1)]) * 8) / (step2 * 6),
            _ => (Function[arg - T.CreateChecked(step2)] - Function[arg + T.CreateChecked(step2)] + (Function[arg + T.CreateChecked(Step)] - Function[arg - T.CreateChecked(Step)]) * 8) / (step2 * 6)
        };
    }
    
    /// <summary>
    /// [22 * f(x-3h) - 67 * f(x-2h) - 58 * f(x-h) + 0 * f(x) + 58 * f(x+h) + 67 * f(x+2h) - 22 * f(x+3h)] / 252h
    /// </summary>
    /// <returns></returns>
    private double SGCubicSevenPoint (T arg)
    {
        double step2 = Step * 2;
        double step3 = Step * 3;
        return Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 => (22 * (Function[arg - T.CreateChecked(3)] - Function[arg + T.CreateChecked(3)]) + 67 * (Function[arg + T.CreateChecked(2)] - Function[arg - T.CreateChecked(2)]) + 58 * (Function[arg + T.CreateChecked(1)] - Function[arg - T.CreateChecked(1)])) / (Step * 252),
            _ => (22 * (Function[arg - T.CreateChecked(step3)] - Function[arg + T.CreateChecked(step3)]) + 67 * (Function[arg + T.CreateChecked(step2)] - Function[arg - T.CreateChecked(step2)]) + 58 * (Function[arg + T.CreateChecked(Step)] - Function[arg - T.CreateChecked(Step)])) / (Step * 252)
        };
    }
    
    /// <summary>
    /// [86 * f(x-4h) - 142 * f(x-3h) - 67 * f(x-2h) - 58 * f(x-h) + 0 * f(x) + 58 * f(x+h) + 67 * f(x+2h) + 142 * f(x+3h) - 86 * f(x+4h)] / 1188h
    /// </summary>
    /// <returns></returns>
    private double SGCubicNinePoint (T arg)
    {
        double step2 = Step * 2;
        double step3 = Step * 3;
        double step4 = Step * 4;
        return Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 => (86 * (Function[arg - T.CreateChecked(4)] - Function[arg + T.CreateChecked(4)]) + 142 * (Function[arg + T.CreateChecked(3)] - Function[arg - T.CreateChecked(3)]) + 193 * (Function[arg + T.CreateChecked(2)] - Function[arg - T.CreateChecked(2)]) + 126 * (Function[arg + T.CreateChecked(1)] - Function[arg - T.CreateChecked(1)])) / (Step * 1188),
            _ => (86 * (Function[arg - T.CreateChecked(step4)] - Function[arg + T.CreateChecked(step4)]) + 142 * (Function[arg + T.CreateChecked(step3)] - Function[arg - T.CreateChecked(step3)]) + 193 * (Function[arg + T.CreateChecked(step2)] - Function[arg - T.CreateChecked(step2)]) + 126 * (Function[arg + T.CreateChecked(Step)] - Function[arg - T.CreateChecked(Step)])) / (Step * 1188)
        };
    }
}