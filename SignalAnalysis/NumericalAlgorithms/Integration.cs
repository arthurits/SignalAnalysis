using System.Numerics;

namespace SignalAnalysis;

public enum IntegrationMethod
{
    MidPointRule,
    TrapezoidRule,
    SimpsonRule3,
    SimpsonRule8,
    SimpsonComposite
}

public class Integration<T> where T : INumber<T>
{
    private IFunction<T> Function { get; set; }
    private IntegrationMethod Method { get; set; } = IntegrationMethod.TrapezoidRule;
    public double Step { get; set; } = 10e-6;
    /// <summary>
    /// True if the absolute value of the integral is computed. False if negative area values are considered
    /// </summary>
    public bool AbsoluteValue { get; set; } = true;

    public Integration(IFunction<T> func, double step, IntegrationMethod method = IntegrationMethod.TrapezoidRule, bool absolute = true)
    {
        this.Function = func;
        this.Method = method;
        this.Step = step;
        this.AbsoluteValue = absolute;
    }

    public double this[T arg] => Method switch
    {
        IntegrationMethod.MidPointRule => MidPointRule(arg),
        IntegrationMethod.TrapezoidRule => TrapezoidRule(arg),
        IntegrationMethod.SimpsonRule3 => SimpsonRule3(arg),
        IntegrationMethod.SimpsonRule8 => SimpsonRule8(arg),
        IntegrationMethod.SimpsonComposite => SimpsonComposite(arg),
        _ => throw new ArgumentOutOfRangeException(nameof(Method), $"Not expected integration method: {Method}"),
    };

    public double IntegrateArray(double[] array)
    {
        int indexStart = 0, indexEnd = array.Length - 1, loopIncrement = 1;
        double result = 0;

        switch (Method)
        {
            case IntegrationMethod.MidPointRule:
                indexStart = 1;
                indexEnd = array.Length;
                break;
            case IntegrationMethod.TrapezoidRule:
                indexStart = 0;
                indexEnd = array.Length - 1;
                break;
            case IntegrationMethod.SimpsonRule3:
                indexStart = 1;
                indexEnd = array.Length - 1;
                loopIncrement = 2;
                break;
            case IntegrationMethod.SimpsonRule8:
                indexStart = 3;
                indexEnd = array.Length - 1;
                loopIncrement = 3;
                break;
            case IntegrationMethod.SimpsonComposite:
                indexStart = 3;
                indexEnd = array.Length - 1;
                loopIncrement = 3;
                break;
        }

        for (int i = indexStart; i < indexEnd; i += loopIncrement)
            result += this[T.CreateChecked(i)];

        return result;
    }

    private double MidPointRule(T arg)
    {
        //return  (func[arg - h] + func[arg]) / (h);
        return Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 => Step * (Function[arg - T.CreateChecked(1)] + Function[arg]) / (double)2,
            _ => Step * (Function[arg - T.CreateChecked(Step)] - Function[arg]) / (double)2
        };
    }

    private double TrapezoidRule(T arg)
    {
        //return  (func[arg - h] + func[arg]) / (h);
        return Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 => Step * (Function[arg - T.CreateChecked(1)] + Function[arg]) / (double)2,
            _ => Step * (Function[arg - T.CreateChecked(Step)] - Function[arg]) / (double)2
        };
    }

    private double SimpsonRule3(T arg)
    {
        //return  (func[arg + h] - func[arg - h]) / (h * 2);
        return Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 => Step * (Function[arg - T.CreateChecked(1)] + 4 * Function[arg] + Function[arg + T.CreateChecked(1)]) / (double)3,
            _ => Step * (Function[arg - T.CreateChecked(Step)] + 4 * Function[arg] + Function[arg + T.CreateChecked(Step)]) / (double)3
        };
    }

    // https://en.wikipedia.org/wiki/Simpson%27s_rule
    private double SimpsonRule8(T arg)
    {
        //return  (func[arg + h] - func[arg - h]) / (h * 2);
        return Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 => Step * 3* (Function[arg - T.CreateChecked(3)] + 3 * Function[arg - T.CreateChecked(2)] + 3 * Function[arg - T.CreateChecked(1)] + Function[arg]) / (double)8,
            _ => Step * 3 * (Function[arg - T.CreateChecked(Step)] + 3 * Function[arg - T.CreateChecked(Step)] + 3 * Function[arg - T.CreateChecked(Step)] + Function[arg]) / (double)8
        };
    }

    // https://en.wikipedia.org/wiki/Simpson%27s_rule
    // http://mathforcollege.com/nm/mws/gen/07int/mws_gen_int_txt_simpson3by8.pdf
    private double SimpsonComposite(T arg)
    {
        //return  (func[arg + h] - func[arg - h]) / (h * 2);
        return Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 => Step * (Function[arg - T.CreateChecked(3)] + 3 * Function[arg - T.CreateChecked(2)] + 3 * Function[arg - T.CreateChecked(1)] + Function[arg]) / (double)48,
            _ => Step * (Function[arg - T.CreateChecked(Step)] + 3 * Function[arg - T.CreateChecked(Step)] + 3 * Function[arg - T.CreateChecked(Step)] + Function[arg]) / (double)48
        };
    }
}
