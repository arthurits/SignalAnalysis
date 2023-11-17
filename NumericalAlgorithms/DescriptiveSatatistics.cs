using ScottPlot.Statistics;

namespace SignalAnalysis;

public static class DescriptiveSatatistics
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

    /// <summary>
    /// Computes the variance and the standard deviation from either a sample or a pupulation data set.
    /// </summary>
    /// <param name="signal">1D array (vector) with values</param>
    /// <param name="average">Average of the data set</param>
    /// <param name="isPopulation">If <see langword="true"/>, assumes data from a finite population (n is used). If <see langword="false"/>, assumes data from a sample (n-1 is used)</param>
    /// <returns>Variance and standard deviation</returns>
    /// <seealso cref="https://en.wikipedia.org/wiki/Algorithms_for_calculating_variance"/>
    public static (double variance, double stddev) ComputeVariance(double[] signal, double average, bool isPopulation = true)
    {
        // If there's only 1 point
        if (signal is null || signal.Length <= 1) return (0, 0);

        double sum = 0;
        for (int i = 0; i < signal.Length; i++)
            sum += Math.Pow(signal[i] - average, 2);
        
        double variance = sum / (isPopulation ? signal.Length : signal.Length - 1);
        double stddev = Math.Sqrt(variance);

        return (variance, stddev);
    }

    /// <summary>
    /// Computes the variance and the standard deviation from either a sample or a pupulation data set.
    /// To minimize numerical errors, data is shifted while computing variance (var(x-K) = var(x)).
    /// </summary>
    /// <param name="signal">1D array (vector) with values</param>
    /// <param name="isPopulation">If <see langword="true"/>, assumes data from a finite population (n is used). If <see langword="false"/>, assumes data from a sample (n-1 is used)</param>
    /// <returns>Variance and standard deviation</returns>
    /// <seealso cref="https://en.wikipedia.org/wiki/Algorithms_for_calculating_variance"/>
    public static (double variance, double stddev) ComputeVariance(double[] signal, bool isPopulation = true)
    {   
        // Check input data set
        if (signal is null || signal.Length <= 1) return (0, 0);

        // Compute average, max, and min descriptive statistics
        double K = signal[0], Ex = 0, Ex2 = 0;

        for (int i = 0; i < signal.Length; i++)
        {
            // Variance computation by shifting data
            Ex += signal[i] - K;
            Ex2 += Math.Pow((signal[i] - K), 2);
        }
        double variance = (Ex2 - Math.Pow(Ex, 2) / signal.Length) / (isPopulation ? signal.Length : signal.Length - 1);
        double stddev = Math.Sqrt(variance);
        
        return (variance, stddev);
    }

    /// <summary>
    /// Return the quartile values of an ordered set of doubles
    ///   assume the sorting has already been done.
    ///   
    /// This actually turns out to be a bit of a PITA, because there is no universal agreement 
    ///   on choosing the quartile values. In the case of odd values, some count the median value
    ///   in finding the 1st and 3rd quartile and some discard the median value. 
    ///   the two different methods result in two different answers.
    ///   The below method produces the arithmatic mean of the two methods, and insures the median
    ///   is given it's correct weight so that the median changes as smoothly as possible as 
    ///   more data points are added.
    ///    
    /// This method uses the following logic:
    /// 
    ///  - If there are an even number of data points:
    ///    Use the median to divide the ordered data set into two halves. 
    ///    The lower quartile value is the median of the lower half of the data. 
    ///    The upper quartile value is the median of the upper half of the data.
    ///    
    ///  - If there are (4n+1) data points:
    ///    The lower quartile is 25% of the nth data value plus 75% of the (n+1)th data value.
    ///    The upper quartile is 75% of the (3n+1)th data point plus 25% of the (3n+2)th data point.
    ///    
    ///  - If there are (4n+3) data points:
    ///   The lower quartile is 75% of the (n+1)th data value plus 25% of the (n+2)th data value.
    ///   The upper quartile is 25% of the (3n+2)th data point plus 75% of the (3n+3)th data point.
    /// 
    /// </summary>
    /// <param name="signal">1D array (vector) with values already sorted in ascending order</param>
    /// <returns>First quartile (Q1), median (Q2), third quartile (Q3)</returns>
    /// <seealso cref="https://stackoverflow.com/questions/14683467/finding-the-first-and-third-quartiles"/>
    public static (double, double, double) Quartiles(double[] signal)
    {
        int midIndex = signal.Length / 2;   // If length is 4 or 5, then midIndex is 2. If length is 6 or 7 the midIndex is 3, etc.

        // Special case
        if (signal.Length == 1) return (0.0, 0.0, 0.0);

        double q1 = 0;
        double q2 = 0;
        double q3 = 0;

        if (signal.Length % 2 == 0) // Even number of points
        {
            // Even between low and high point
            q2 = (signal[midIndex - 1] + signal[midIndex]) / 2;

            int iMidMid = midIndex / 2;

            //easy split 
            if (midIndex % 2 == 0)
            {
                q1 = (signal[iMidMid - 1] + signal[iMidMid]) / 2;
                q3 = (signal[midIndex + iMidMid - 1] + signal[midIndex + iMidMid]) / 2;
            }
            else
            {
                q1 = signal[iMidMid];
                q3 = signal[iMidMid + midIndex];
            }
        }
        else    // Odd number so the median is just the midpoint in the array.
        {
            
            q2 = signal[midIndex];

            if ((signal.Length - 1) % 4 == 0)
            {
                // (4n-1) points
                int n = (signal.Length - 1) / 4;
                q1 = (signal[n - 1] * .25) + (signal[n] * .75);
                q3 = (signal[3 * n] * .75) + (signal[3 * n + 1] * .25);
            }
            else if ((signal.Length - 3) % 4 == 0)
            {
                // (4n-3) points
                int n = (signal.Length - 3) / 4;

                q1 = (signal[n] * .75) + (signal[n + 1] * .25);
                q3 = (signal[3 * n + 1] * .25) + (signal[3 * n + 2] * .75);
            }
        }

        return (q1, q2, q3);
    }

    /// <summary>
    /// Computes the parameters needed to draw a BoxPlot: the three quartiles (q1, q2, q3) and the outlier's limits
    /// </summary>
    /// <param name="signal">1D array (vector) with values</param>
    /// <param name="IsSorted"><see langword="True"/> if the array values are sorted in ascending order, false otherwise</param>
    /// <param name="factor">Factor to compute the minimum and maximun outliers limits</param>
    /// <returns>Minimum value excluding outliers, first quartile (Q1), Median (Q2), third quartile (Q3), maximum value excluding outliers</returns>
    /// <seealso cref="https://www.geeksforgeeks.org/box-plot/"/>
    public static (double minimum, double q1, double q2, double q3, double maximum) ComputeBoxPlotValues(double[] signal, bool IsSorted = true, double factor = 1.5)
    {
        double[] array = new double[signal.Length];
        Array.Copy(signal, array, signal.Length);
        
        if (!IsSorted) Array.Sort(array);

        (double q1, double q2, double q3) = Quartiles(array);

        double iqr = q3 - q1;
        double minimum = q1 - factor * iqr;
        double maximum = q3 + factor * iqr;

        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] >= minimum)
            {
                minimum = array[i];
                break;
            }
        }

        for (int i = array.Length - 1; i >= 0; i--)
        {
            if (array[i] <= maximum)
            {
                maximum = array[i];
                break;
            }
        }

        return (minimum, q1, q2, q3, maximum);

    }
}

