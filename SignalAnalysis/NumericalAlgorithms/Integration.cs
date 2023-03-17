namespace SignalAnalysis;

public enum IntegrationMethod
{
    LeftPointRule,
    MidPointRule,
    RightPointRule,
    TrapezoidRule,
    SimpsonRule3,
    SimpsonRule8,
    SimpsonComposite,
    Romberg
}

/// <summary>
/// Numerical integration algorithms. Data is expected to be uniformly spaced.
/// </summary>
public static class Integration
{
    /// <summary>
    /// Computes the numerical integral of a function
    /// </summary>
    /// <param name="function">Function to be integrated</param>
    /// <param name="method">Integration method</param>
    /// <param name="lowerLimit">Integral lower limit</param>
    /// <param name="upperLimit">Integral upper limit</param>
    /// <param name="segments">Number of equal segments the integration interval is divided into</param>
    /// <param name="absoluteIntegral">True if the absolute integral value is computed. False if positive and negative areas are computed and compensated</param>
    /// <returns>The estimated integral value</returns>
    public static double Integrate(Func<double, double> function, IntegrationMethod method = IntegrationMethod.TrapezoidRule, double lowerLimit = 0, double upperLimit = 1, int segments = 1, bool absoluteIntegral = false)
    {
        double result = 0;
        int i = 0;

        switch (method)
        {
            case IntegrationMethod.LeftPointRule:
                result = Integration.LeftPointRule(function, lowerLimit, upperLimit, segments, absoluteIntegral);
                break;
            case IntegrationMethod.MidPointRule:
                result = MidPointRule(function, lowerLimit, upperLimit, segments, absoluteIntegral);
                break;
            case IntegrationMethod.RightPointRule:
                result = RightPointRule(function, lowerLimit, upperLimit, segments, absoluteIntegral);
                break;
            case IntegrationMethod.TrapezoidRule:
                result = TrapezoidalUniformRule(function, lowerLimit, upperLimit, segments, absoluteIntegral);
                break;
            case IntegrationMethod.SimpsonRule3:
                i = segments % 2;
                result = TrapezoidalUniformRule(function, upperLimit - i, upperLimit, i, absoluteIntegral);
                result += SimpsonRule3(function, lowerLimit, upperLimit - i, segments - i, absoluteIntegral);
                break;
            case IntegrationMethod.SimpsonRule8:
                i = segments % 3;
                if (i == 1)
                    result = TrapezoidalUniformRule(function, upperLimit - i, upperLimit, i, absoluteIntegral);
                else
                    result = SimpsonRule3(function, upperLimit - i, upperLimit, i, absoluteIntegral);
                result += SimpsonRule8(function, lowerLimit, upperLimit - i, segments - i, absoluteIntegral);
                break;
            case IntegrationMethod.SimpsonComposite:
                if (segments < 8)
                    result = 0;
                else
                    result = SimpsonComposite(function, lowerLimit, upperLimit, segments, absoluteIntegral);
                break;
            case IntegrationMethod.Romberg:
                if (System.Numerics.BitOperations.IsPow2(segments)) // Check if segments is a power of 2. The classic way to do it was: (x != 0) && ((x & (x - 1)) == 0)
                    result = Romberg(function, lowerLimit, upperLimit, maxSteps: (int)Math.Log2(segments) + 1, absoluteIntegral: absoluteIntegral);
                break;
        }

        return result;
    }

    /// <summary>
    /// Computes the numerical integral of uniformly-spaced tabular data
    /// </summary>
    /// <param name="array">1D array containing the data</param>
    /// <param name="lowerIndex">Integral lower limit</param>
    /// <param name="upperIndex">Integral upper limit</param>
    /// <param name="samplingFrequency">Data sampling frequency</param>
    /// <param name="absoluteIntegral">True if the absolute integral value is computed. False if positive and negative areas are computed and compensated</param>
    /// <returns>The estimated integral value</returns>
    public static double Integrate(double[] array, IntegrationMethod method = IntegrationMethod.TrapezoidRule, int lowerIndex = 0, int upperIndex = 1, double samplingFrequency = 1, bool absoluteIntegral = false)
    {
        // Compute the array and adjust the result by the sampling frequency. This is only possible when data is uniformly spaced.
        double result = Integrate(Function, method, lowerIndex, upperIndex, (upperIndex - lowerIndex), absoluteIntegral);
        result /= samplingFrequency;
        
        return result;

        double Function(double index)
        {
            return array[(int)(index)];
        }
    }


    /// <summary>
    /// Computes the numerical integral using the left-point rule. Data is assumed to be uniformly spaced.
    /// </summary>
    /// <param name="function">Function to be integrated</param>
    /// <param name="lowerLimit">Integral lower limit</param>
    /// <param name="upperLimit">Integral upper limit</param>
    /// <param name="segments">Number of equal segments the integration interval is divided into</param>
    /// <param name="absoluteIntegral">True if the absolute integral value is computed. False if positive and negative areas are computed and compensated</param>
    /// <returns>The estimated integral value</returns>
    private static double LeftPointRule(Func<double, double> function, double lowerLimit, double upperLimit, double segments = 1, bool absoluteIntegral = false)
    {
        double result = 0;
        double step = (upperLimit - lowerLimit) / segments;
        double x = lowerLimit;

        for (int j = 0; j < segments; j++)
        {
            result += absoluteIntegral ? Math.Abs(function(x)) : function(x);
            x += step;
        }

        return step * result;
    }

    /// <summary>
    /// Computes the numerical integral using the trapezoidal rule. Data is assumed to be uniformly spaced.
    /// </summary>
    /// <param name="function">Function to be integrated</param>
    /// <param name="lowerLimit">Integral lower limit</param>
    /// <param name="upperLimit">Integral upper limit</param>
    /// <param name="segments">Number of equal segments the integration interval is divided into</param>
    /// <param name="absoluteIntegral">True if the absolute integral value is computed. False if positive and negative areas are computed and compensated</param>
    /// <returns>The estimated integral value</returns>
    private static double MidPointRule(Func<double, double> function, double lowerLimit, double upperLimit, double segments = 1, bool absoluteIntegral = false)
    {
        double result = 0;
        double step = (upperLimit - lowerLimit) / segments;
        double x = lowerLimit + step / 2;

        for (int j = 0; j < segments; j++)
        {
            result += absoluteIntegral ? Math.Abs(function(x)) : function(x);
            x += step;
        }

        return step * result;
    }

    /// <summary>
    /// Computes the numerical integral using the left-point rule. Data is assumed to be uniformly spaced.
    /// </summary>
    /// <param name="function">Function to be integrated</param>
    /// <param name="lowerLimit">Integral lower limit</param>
    /// <param name="upperLimit">Integral upper limit</param>
    /// <param name="segments">Number of equal segments the integration interval is divided into</param>
    /// <param name="absoluteIntegral">True if the absolute integral value is computed. False if positive and negative areas are computed and compensated</param>
    /// <returns>The estimated integral value</returns>
    private static double RightPointRule(Func<double, double> function, double lowerLimit, double upperLimit, double segments = 1, bool absoluteIntegral = false)
    {
        double result = 0;
        double step = (upperLimit - lowerLimit) / segments;
        double x = lowerLimit;

        for (int j = 0; j < segments; j++)
        {
            x += step;
            result += absoluteIntegral ? Math.Abs(function(x)) : function(x);
        }

        return step * result;
    }

    /// <summary>
    /// Computes the numerical integral using the trapezoidal rule. Data is assumed to be uniformly spaced.
    /// </summary>
    /// <param name="function">Function to be integrated</param>
    /// <param name="lowerLimit">Integral lower limit</param>
    /// <param name="upperLimit">Integral upper limit</param>
    /// <param name="segments">Number of equal segments the integration interval is divided into</param>
    /// <param name="absoluteIntegral">True if the absolute integral value is computed. False if positive and negative areas are computed and compensated</param>
    /// <returns>The estimated integral value</returns>
    private static double TrapezoidalUniformRule(Func<double, double> function, double lowerLimit, double upperLimit, double segments = 1, bool absoluteIntegral = false)
    {
        if (segments == 0 || lowerLimit >= upperLimit) return 0;

        double result = (functionAbs(lowerLimit) + functionAbs(upperLimit)) / 2;

        double step = (upperLimit - lowerLimit) / segments;
        double x = lowerLimit;
        for (int j = 1; j < segments; j++)
        {
            x += step;
            result += functionAbs(x);
        }
        return step * result;

        double functionAbs(double x) => absoluteIntegral ? Math.Abs(function(x)) : function(x);
    }

    /// <summary>
    /// Computes the numerical integral using Simpson's 1/3 rule. Data is assumed to be uniformly spaced.
    /// </summary>
    /// <param name="function">Function to be integrated</param>
    /// <param name="lowerLimit">Integral lower limit</param>
    /// <param name="upperLimit">Integral upper limit</param>
    /// <param name="segments">Number of equal segments the integration interval is divided into</param>
    /// <param name="absoluteIntegral">True if the absolute integral value is computed. False if positive and negative areas are computed and compensated</param>
    /// <returns>The estimated integral value</returns>
    private static double SimpsonRule3(Func<double, double> function, double lowerLimit, double upperLimit, int segments = 1, bool absoluteIntegral = false)
    {
        double result = functionAbs(lowerLimit) + functionAbs(upperLimit);
        
        double step = (upperLimit - lowerLimit) / segments;
        double x = lowerLimit;
        for (int j = 1; j < segments; j++)
        {
            x += step;
            result += (2 << (j % 2)) * functionAbs(x);
        }

        return result * step / 3;

        double functionAbs(double x) => absoluteIntegral ? Math.Abs(function(x)) : function(x);
    }

    /// <summary>
    /// Computes the numerical integral using Simpson's 3/8 rule. Data is assumed to be uniformly spaced.
    /// </summary>
    /// <param name="function">Function to be integrated</param>
    /// <param name="lowerLimit">Integral lower limit</param>
    /// <param name="upperLimit">Integral upper limit</param>
    /// <param name="segments">Number of equal segments the integration interval is divided into</param>
    /// <param name="absoluteIntegral">True if the absolute integral value is computed. False if positive and negative areas are computed and compensated</param>
    /// <returns>The estimated integral value</returns>
    private static double SimpsonRule8(Func<double, double> function, double lowerLimit, double upperLimit, int segments = 1, bool absoluteIntegral = false)
    {
        double result = functionAbs(lowerLimit) + functionAbs(upperLimit);

        double step = (upperLimit - lowerLimit) / segments;
        double x = lowerLimit;
        for (int j = 1; j < segments; j++)
        {
            x += step;
            if (j % 3 == 0)
                result += 2 * functionAbs(x);
            else
                result += 3 * functionAbs(x);
        }

        return result * step * 3 / 8;

        double functionAbs(double x) => absoluteIntegral ? Math.Abs(function(x)) : function(x);
    }



    // https://en.wikipedia.org/wiki/Simpson%27s_rule
    // http://mathforcollege.com/nm/mws/gen/07int/mws_gen_int_txt_simpson3by8.pdf

    /// <summary>
    /// Computes the numerical integral using Simpson's composite rule. Data is assumed to be uniformly spaced.
    /// </summary>
    /// <param name="function">Function to be integrated</param>
    /// <param name="lowerLimit">Integral lower limit</param>
    /// <param name="upperLimit">Integral upper limit</param>
    /// <param name="segments">Number of equal segments the integration interval is divided into</param>
    /// <param name="absoluteIntegral">True if the absolute integral value is computed. False if positive and negative areas are computed and compensated</param>
    /// <returns>The estimated integral value</returns>
    private static double SimpsonComposite(Func<double, double> function, double lowerLimit, double upperLimit, int segments = 1, bool absoluteIntegral = false)
    {
        double step = (upperLimit - lowerLimit) / segments;
        double x = lowerLimit;

        double result = 17 * (functionAbs(x) + functionAbs(upperLimit));
        x += step;
        result += 59 * (functionAbs(x) + functionAbs(upperLimit - step));
        x += step;
        result += 43 * (functionAbs(x) + functionAbs(upperLimit - 2 * step));
        x += step;
        result += 49 * (functionAbs(x) + functionAbs(upperLimit - 3 * step));

        for (int j = 4; j < segments - 3; j++)
        {
            x += step;
            result += 48 * functionAbs(x);
        }

        return result * step / 48;

        double functionAbs(double x) => absoluteIntegral ? Math.Abs(function(x)) : function(x);
    }


    /// <summary>
    /// https://en.wikipedia.org/wiki/Romberg%27s_method
    /// </summary>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name="">pointer to the function to be integrated</param>
    /// <param name="lowerLimit">lower limit</param>
    /// <param name="upperLimit">upper limit</param>
    /// <param name="maxSteps">maximum steps of the procedure</param>
    /// <param name="epsilon">desired accuracy</param>
    /// <param name="absoluteIntegral">True if the absolute integral value is computed. False if positive and negative areas are computed and compensated</param>
    /// <returns>Approximate value of the integral of the function f for x in [a,b] with accuracy 'acc' and steps 'max_steps'</returns>
    private static double Romberg(Func<double, double> function, double lowerLimit, double upperLimit, int maxSteps = 10, double epsilon = 1E-6, bool absoluteIntegral = false)
    {
        double[] R1 = new double[maxSteps]; // buffer previous row
        double[] R2 = new double[maxSteps];   // buffer current row
        double h = upperLimit - lowerLimit; //step size
    
        // First trapezoidal step
        R1[0] = (functionAbs(lowerLimit) + functionAbs(upperLimit)) * h * 0.5;

        for (int i = 1; i < maxSteps; ++i)
        {
            h /= 2;
            double c = 0;
            int ep = 1 << (i - 1); //2^(n-1)
            for (int j = 1; j <= ep; ++j)
                c += functionAbs(lowerLimit + (2 * j - 1) * h);
            R2[0] = h * c + .5 * R1[0]; // R(i,0)

            for (int j = 1; j <= i; ++j)
            {
                double n_k = Math.Pow(4, j);
                R2[j] = (n_k * R2[j - 1] - R1[j - 1]) / (n_k - 1); // compute R(i,j)
            }

            if (i > 1 && Math.Abs(R1[i - 1] - R2[i]) < epsilon)
                return R2[i];

            // swap Rn and Rc as we only need the last row
            Array.Copy(R2, R1, i+1);
            Array.Clear(R2, 0, i+1);
        }
        
        return R1[maxSteps - 1]; // return our best guess

        double functionAbs(double x) => absoluteIntegral ? Math.Abs(function(x)) : function(x);
    }

}
