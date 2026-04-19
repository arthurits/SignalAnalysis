namespace SignalAnalysis.NumericalAlgorithms;

internal static class DescriptiveStatistics
{
    /// <summary>
    /// Computes the average and variance from either a sample or a pupulation data set. It also computes the maximum and minimum values of the data set.
    /// </summary>
    /// <param name="signal">1D array (vector) with values</param>
    /// <param name="isPopulation">If <see langword="true"/>, assumes data from a finite population (n is used). If <see langword="false"/>, assumes data from a sample (n-1 is used)</param>
    /// <returns>The average, variance, maximum and minimum</returns>
    public static (double average, double variance, double maximum, double minimum) ComputeAverage(double[] signal, bool isPopulation = true)
    {
        // Check input data set
        if (signal is null || signal.Length == 0) return (0, 0, 0, 0);

        // Compute average, max, and min descriptive statistics
        double max = signal[0], min = signal[0], sum = 0;
        double K = signal[0], Ex = 0, Ex2 = 0;

        for (int i = 0; i < signal.Length; i++)
        {
            // Average computation
            if (signal[i] > max) max = signal[i];
            if (signal[i] < min) min = signal[i];
            sum += signal[i];

            // Variance computation by shifting data
            Ex += signal[i] - K;
            Ex2 += Math.Pow((signal[i] - K), 2);
        }
        double avg = sum / signal.Length;
        double variance = (Ex2 - Math.Pow(Ex, 2) / signal.Length) / (isPopulation ? signal.Length : signal.Length - 1);

        return (avg, variance, max, min);
    }
}
