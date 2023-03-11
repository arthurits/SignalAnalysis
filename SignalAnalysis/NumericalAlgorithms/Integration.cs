using System.Numerics;

namespace SignalAnalysis;

public enum IntegrationMethod
{
    MidPointRule,
    TrapezoidRule,
    SimpsonRule
}

public class Integration<T> where T : INumber<T>
{
    private IFunction<T> Function { get; set; }
    private IntegrationMethod Method { get; set; } = IntegrationMethod.TrapezoidRule;
    public double Step { get; set; } = 10e-6;

    public Integration(IFunction<T> func, double step, IntegrationMethod method = IntegrationMethod.TrapezoidRule)
    {
        this.Function = func;
        this.Method = method;
        this.Step = step;
    }

    public double this[T arg] => Method switch
    {
        IntegrationMethod.MidPointRule => MidPointRule(arg),
        IntegrationMethod.TrapezoidRule => TrapezoidRule(arg),
        IntegrationMethod.SimpsonRule => SimpsonRule(arg),
        _ => throw new ArgumentOutOfRangeException(nameof(Method), $"Not expected integration method: {Method}"),
    };

    public double[] IntegrateArray(double[] array)
    {
        int indexStart = 0, indexEnd = array.Length - 1;
        double[] result = new double[array.Length];

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
            case IntegrationMethod.SimpsonRule:
                indexStart = 1;
                indexEnd = array.Length - 1;
                break;
        }

        for (int i = indexStart; i < indexEnd; i++)
            result[i] = this[T.CreateChecked(i)];

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

    private double SimpsonRule(T arg)
    {
        //return  (func[arg + h] - func[arg - h]) / (h * 2);
        return Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 => Step * (Function[arg - T.CreateChecked(1)] + 4 * Function[arg] + Function[arg + T.CreateChecked(1)]) / (double)3,
            _ => Step * (Function[arg - T.CreateChecked(Step)] + 4 * Function[arg] + Function[arg + T.CreateChecked(Step)]) / (double)3
        };
    }
}
