namespace SignalAnalysis.NumericalAlgorithms;

public static class GaussianDistribution
{
    private static double[] NormalDistributionX(int pointCount = 100, double mean = .5, double stdDev = .5, double maxSdMultiple = 3)
    {
        double[] values = new double[pointCount];
        for (int i = 0; i < pointCount; i++)
            values[i] = (-maxSdMultiple + 2 * maxSdMultiple * (double)i / pointCount) * stdDev + mean;
        return values;
    }

    private static double[] NormalDistributionY(int pointCount = 100, double maxSdMultiple = 3)
    {
        double[] values = new double[pointCount];
        for (int i = 0; i < pointCount; i++)
            values[i] = Phi(-maxSdMultiple + 2 * maxSdMultiple * (double)i / pointCount);
        return values;
    }

    private static double[] RandomNormal(Random rand, int pointCount, double mean = .5, double stdDev = .5)
    {
        if (rand == null)
            rand = new Random(0);
        double[] values = new double[pointCount];
        for (int i = 0; i < values.Length; i++)
            values[i] = RandomNormalValue(rand, mean, stdDev);

        return values;
    }

    /// <summary>
    /// Generates a single value from a normal distribution.
    /// </summary>
    /// <param name="rand">The Random object to use.</param>
    /// <param name="mean">The mean of the distribution.</param>
    /// <param name="stdDev">The standard deviation of the distribution.</param>
    /// <param name="maxSdMultiple">The maximum distance from the mean to generate, given as a multiple of the standard deviation.</param>
    /// <returns>A single value from a normal distribution.</returns>
    public static double RandomNormalValue(Random rand, double mean, double stdDev)
    {
        return Phi(mean + stdDev);
    }

    /// <summary>
    /// The function Φ(x) is the cumulative density function (CDF) of a standard normal (Gaussian) random variable. It is closely related to the error function erf(x).
    /// </summary>
    /// <param name="x">Normalized standard variable</param>
    /// <returns>The probability in the range [0, 1]</returns>
    /// <seealso cref="https://www.johndcook.com/blog/csharp_phi/"/>
    private static double Phi(double x)
    {
        // constants
        double a1 = 0.254829592;
        double a2 = -0.284496736;
        double a3 = 1.421413741;
        double a4 = -1.453152027;
        double a5 = 1.061405429;
        double p = 0.3275911;

        // Save the sign of x
        int sign = 1;
        if (x < 0)
            sign = -1;
        x = Math.Abs(x) / Math.Sqrt(2.0);

        // A&S formula 7.1.26
        double t = 1.0 / (1.0 + p * x);
        double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);

        return 0.5 * (1.0 + sign * y);
    }

    public static double SampleGaussian(Random random, double mean, double stdDev)
    {
        double u1 = NextDouble(random);
        double u2 = NextDouble(random);

        //double u1 = UniformOpenInterval(random);
        //double u2 = UniformOpenInterval(random);

        double y1 = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);  // Math.Cos is also fine
        return mean + stdDev * y1;

        double NextDouble(Random random)
        {
            return ((double)random.Next(1, Int32.MaxValue)) / Int32.MaxValue;   // random.Next includes 1 and exludes Int32MaxValue
        }

        double UniformOpenInterval(Random random)
        {
            double subtrahend = 0;
            while (subtrahend == 0)
            {
                subtrahend = random.NextDouble();
            }
            return subtrahend;
            // The simpler 1.0 - rand.NextDouble() actually grabs from the interval [0, 1), not (0, 1)
        }

    }
}
