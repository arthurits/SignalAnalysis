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
    /// <param name="method">Integration method from <see cref="IntegrationMethod"> enumeration</param>
    /// <param name="lowerLimit">Integral lower limit</param>
    /// <param name="upperLimit">Integral upper limit</param>
    /// <param name="segments">Number of equal segments the integration interval is divided into</param>
    /// <param name="absoluteIntegral"><see langword="True"/> if the absolute integral value is computed. <see langword="False"/> if positive and negative areas are computed and compensated</param>
    /// <param name="roundupSegments"><see langword="True"/> if the data array should be increased to fit the required algorith length. <see langword="False"/> (default value) otherwise</param>
    /// <returns>The estimated integral value</returns>
    public static double Integrate(Func<double, double> function, IntegrationMethod method = IntegrationMethod.TrapezoidRule, double lowerLimit = 0, double upperLimit = 1, int segments = 1, bool absoluteIntegral = false, bool roundupSegments = false)
    {
        // This should only be true when we are dealing with real function (not an array transformed into a function)
        if (roundupSegments)
            segments += PaddingQuantity(segments + 1, method);

        // Determine whether "function" is a user-defined function or a data array
        // by checking if we are coming from the overloaded "Integrate" function
        bool isFunction = !function.Method.Name.Contains("<Integrate>");

        double result = 0;
        int i = 0;
        double delta;
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
                delta = isFunction ? i / segments : i;
                result = TrapezoidalUniformRule(function, upperLimit - delta, upperLimit, i, absoluteIntegral);
                result += SimpsonRule3(function, lowerLimit, upperLimit - delta, segments - i, absoluteIntegral);
                break;
            case IntegrationMethod.SimpsonRule8:
                i = segments % 3;
                delta = isFunction ? (double)i / segments : i;
                if (i == 1)
                    result = TrapezoidalUniformRule(function, upperLimit - delta, upperLimit, i, absoluteIntegral);
                else if (i == 2)
                    result = SimpsonRule3(function, upperLimit - delta, upperLimit, i, absoluteIntegral);
                result += SimpsonRule8(function, lowerLimit, upperLimit - delta, segments - i, absoluteIntegral);
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
    /// <param name="method">Integration method from <see cref="IntegrationMethod"> enumeration</param>
    /// <param name="lowerIndex">Integral lower limit</param>
    /// <param name="upperIndex">Integral upper limit</param>
    /// <param name="samplingFrequency">Data sampling frequency</param>
    /// <param name="absoluteIntegral"><see langword="True"/> if the absolute integral value is computed. <see langword="False"/> if positive and negative areas are computed and compensated</param>
    /// <param name="pad"><see langword="True"/> if the data array should be increased to fit the required algorith length. <see langword="False"/> (default value) otherwise</param>
    /// <returns>The estimated integral value</returns>
    public static double Integrate(double[] array, IntegrationMethod method = IntegrationMethod.TrapezoidRule, int lowerIndex = 0, int upperIndex = 1, double samplingFrequency = 1, bool absoluteIntegral = false, bool pad = false)
    {
        // Create subarray according to the lower and upper indexes
        array = array[lowerIndex..(upperIndex + 1)];

        // Add data if necessary so that the number of segments match the required algorithm
        double subtract = 0;
        if (pad)
            (array, subtract) = PadDataToIntegrate(array, method);

        // After the above array transformations, recalculate the lower an upper indexes
        lowerIndex = array.GetLowerBound(0);
        upperIndex = array.GetUpperBound(0);

        // Compute the array and adjust the result by the sampling frequency. This is only possible when data is uniformly spaced.
        double result = Integrate(Function, method, lowerIndex, upperIndex, (upperIndex - lowerIndex), absoluteIntegral, false);
        
        // Subtract the area under the data that was added to match the algorithm-required number of segments
        result -= subtract;

        // Adjust the result by the sampling frequency. This is only possible when data is uniformly spaced.
        result /= samplingFrequency;

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
    /// Duplicates the last array point so that the number of segments is a multiple of the required segments for the numerical integration algorithms.
    /// This adds a new rectangular area under the data that must be deal off afterwards.
    /// </summary>
    /// <param name="array">The original data array</param>
    /// <param name="method">Integration method from <see cref="IntegrationMethod"> enumeration/param>
    /// <returns>A new data array where the last point has been multiplicated to fit the algorith requirements and the rectangular area that has been added.</returns>
    public static (double[] newArray, double subtract) PadDataToIntegrate(double[] array, IntegrationMethod method = IntegrationMethod.TrapezoidRule)
    {
        double[] newSignal = [];
        double lastValue = array[^1];
        double subtract = 0;
        int padding;

        switch (method)
        {
            case IntegrationMethod.SimpsonRule3:
                //padding = (array.Length - 1) % 2;
                padding = PaddingQuantity(array.Length, method);
                if (padding > 0)
                {
                    newSignal = new double[array.Length + padding];
                    Array.Copy(array, newSignal, array.Length);
                    Array.Fill(newSignal, lastValue, array.Length, newSignal.Length - array.Length);
                    subtract = lastValue * (newSignal.Length - array.Length);   // This is the rectangle area under the upward-padded data. Needs to be divided by the sampling frequency
                }
                break;
            case IntegrationMethod.SimpsonRule8:
                //padding = 3 - (array.Length - 1) % 3;
                padding = PaddingQuantity(array.Length, method);
                if (padding > 0 && padding < 3)
                {
                    newSignal = new double[array.Length + padding];
                    Array.Copy(array, newSignal, array.Length);
                    Array.Fill(newSignal, lastValue, array.Length, newSignal.Length - array.Length);
                    subtract = lastValue * (newSignal.Length - array.Length);   // This is the rectangle area under the upward-padded data. Needs to be divided by the sampling frequency
                }
                break;
            case IntegrationMethod.Romberg: // This needs to be padded to the upward power of 2
                
                if (!System.Numerics.BitOperations.IsPow2(array.Length - 1))
                {
                    //newSignal = new double[1 + (int)Math.Pow(2, Math.Round(Math.Log2(array.Length - 1), MidpointRounding.ToPositiveInfinity))];
                    padding = PaddingQuantity(array.Length, method);
                    newSignal = new double[array.Length + padding];
                    Array.Copy(array, newSignal, array.Length);
                    Array.Fill(newSignal, lastValue, array.Length, newSignal.Length - array.Length);
                    subtract = lastValue * (newSignal.Length - array.Length);    // This is the rectangle area under the upward-padded data. Needs to be divided by the sampling frequency
                }
                break;
        }

        return (newSignal, subtract);
    }

    /// <summary>
    /// Computes the required padding quantity, in terms of additional needed points, for each specific integration method
    /// </summary>
    /// <param name="points">Current number of data points</param>
    /// <param name="method">Integration method from <see cref="IntegrationMethod"> enumeration</param>
    /// <returns>The additional needed points that should be added</returns>
    public static int PaddingQuantity (int points, IntegrationMethod method = IntegrationMethod.TrapezoidRule)
    {
        return method switch
        {
            IntegrationMethod.SimpsonRule3 => (points - 1) % 2,
            IntegrationMethod.SimpsonRule8 => 3 - (points - 1) % 3,
            IntegrationMethod.Romberg => (1 + (int)Math.Pow(2, Math.Round(Math.Log2(points - 1), MidpointRounding.ToPositiveInfinity))) - points,
            _ => 0
        };
    }

    /// <summary>
    /// Computes the numerical integral using the left-point rule. Data is assumed to be uniformly spaced.
    /// </summary>
    /// <param name="function">Function to be integrated</param>
    /// <param name="lowerLimit">Integral lower limit</param>
    /// <param name="upperLimit">Integral upper limit</param>
    /// <param name="segments">Number of equal segments the integration interval is divided into</param>
    /// <param name="absoluteIntegral"><see langword="True"/> if the absolute integral value is computed. False if positive and negative areas are computed and compensated</param>
    /// <returns>The estimated integral value</returns>
    private static double LeftPointRule(Func<double, double> function, double lowerLimit, double upperLimit, double segments = 1, bool absoluteIntegral = false)
    {
        if (segments == 0 || lowerLimit >= upperLimit) return 0;

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
    /// <param name="absoluteIntegral"><see langword="True"/> if the absolute integral value is computed. False if positive and negative areas are computed and compensated</param>
    /// <returns>The estimated integral value</returns>
    private static double MidPointRule(Func<double, double> function, double lowerLimit, double upperLimit, double segments = 1, bool absoluteIntegral = false)
    {
        if (segments == 0 || lowerLimit >= upperLimit) return 0;

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
    /// <param name="absoluteIntegral"><see langword="True"/> if the absolute integral value is computed. False if positive and negative areas are computed and compensated</param>
    /// <returns>The estimated integral value</returns>
    private static double RightPointRule(Func<double, double> function, double lowerLimit, double upperLimit, double segments = 1, bool absoluteIntegral = false)
    {
        if (segments == 0 || lowerLimit >= upperLimit) return 0;

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
    /// <param name="absoluteIntegral"><see langword="True"/> if the absolute integral value is computed. False if positive and negative areas are computed and compensated</param>
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
    /// <param name="absoluteIntegral"><see langword="True"/> if the absolute integral value is computed. False if positive and negative areas are computed and compensated</param>
    /// <returns>The estimated integral value</returns>
    private static double SimpsonRule3(Func<double, double> function, double lowerLimit, double upperLimit, int segments = 1, bool absoluteIntegral = false)
    {
        if (segments == 0 || lowerLimit >= upperLimit) return 0;

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
    /// <param name="absoluteIntegral"><see langword="True"/> if the absolute integral value is computed. False if positive and negative areas are computed and compensated</param>
    /// <returns>The estimated integral value</returns>
    private static double SimpsonRule8(Func<double, double> function, double lowerLimit, double upperLimit, int segments = 1, bool absoluteIntegral = false)
    {
        if (segments == 0 || lowerLimit >= upperLimit) return 0;

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

    /// <summary>
    /// Computes the numerical integral using Simpson's composite rule. Data is assumed to be uniformly spaced.
    /// </summary>
    /// <param name="function">Function to be integrated</param>
    /// <param name="lowerLimit">Integral lower limit</param>
    /// <param name="upperLimit">Integral upper limit</param>
    /// <param name="segments">Number of equal segments the integration interval is divided into</param>
    /// <param name="absoluteIntegral"><see langword="True"/> if the absolute integral value is computed. False if positive and negative areas are computed and compensated</param>
    /// <seealso cref="https://en.wikipedia.org/wiki/Simpson%27s_rule"/>
    /// <seealso cref="http://mathforcollege.com/nm/mws/gen/07int/mws_gen_int_txt_simpson3by8.pdf"/>
    /// <returns>The estimated integral value</returns>
    private static double SimpsonComposite(Func<double, double> function, double lowerLimit, double upperLimit, int segments = 1, bool absoluteIntegral = false)
    {
        if (segments == 0 || lowerLimit >= upperLimit) return 0;

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
    /// Computes the numerical integral using the Romberg method: trapezoidal rule plus Richardson's extrapolation
    /// </summary>
    /// <param name="function">Function to be integrated function to be integrated</param>
    /// <param name="lowerLimit">lower limit</param>
    /// <param name="upperLimit">upper limit</param>
    /// <param name="maxSteps">maximum steps of the procedure</param>
    /// <param name="epsilon">desired accuracy</param>
    /// <param name="absoluteIntegral"><see langword="True"/> if the absolute integral value is computed. False if positive and negative areas are computed and compensated</param>
    /// <returns>Approximate value of the integral of the function f for x in [a,b] with accuracy 'acc' and steps 'max_steps'</returns>
    /// <seealso cref="https://en.wikipedia.org/wiki/Romberg%27s_method"/>
    private static double Romberg(Func<double, double> function, double lowerLimit, double upperLimit, int maxSteps = 10, double epsilon = 1E-10, bool absoluteIntegral = false)
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

            if (i > 2 && Math.Abs(R1[i - 1] - R2[i]) < epsilon)
                return R2[i];

            // swap Rn and Rc as we only need the last row
            Array.Copy(R2, R1, i+1);
            Array.Clear(R2, 0, i+1);
        }
        
        return R1[maxSteps - 1]; // return our best guess

        double functionAbs(double x) => absoluteIntegral ? Math.Abs(function(x)) : function(x);
    }

}
