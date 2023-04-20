using Microsoft.VisualBasic.ApplicationServices;
using ScottPlot.Drawing.Colormaps;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Drawing;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.X86;

namespace SignalAnalysis;

public static class DescriptiveSatatistics
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="signal"></param>
    /// <returns></returns>
    public static (double average, double maximum, double minimum) ComputeAverage(double[] signal)
    {
        // Compute average, max, and min descriptive statistics
        double max = signal[0], min = signal[0], sum = 0;

        for (int i = 0; i < signal.Length; i++)
        {
            if (signal[i] > max) max = signal[i];
            if (signal[i] < min) min = signal[i];
            sum += signal[i];
        }
        double avg = sum / signal.Length;
        
        return (avg, max, min);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="signal"></param>
    /// <param name="average"></param>
    /// <param name="IsPopulation">If <see langword="tTrue"/> then n is used, otherwise n-1</param>
    /// <returns>Variance and standard deviation</returns>
    /// <seealso cref="https://en.wikipedia.org/wiki/Algorithms_for_calculating_variance"/>
    public static (double variance, double stddev) ComputeVariance(double[] signal, double average, bool IsPopulation = true)
    {
        double variance, stddev;
        double sum = 0;

        // If there's only 1 point
        if (signal.Length <= 1) return (0, 0);

        for (int i = 0; i < signal.Length; i++)
            sum += Math.Pow(signal[i] - average, 2);
        
        variance = sum / (IsPopulation ? signal.Length : signal.Length - 1);
        stddev = Math.Sqrt(variance);

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
    /// <seealso cref="https://www.geeksforgeeks.org/box-plot/"/>
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
    /// <returns>Minimum limit, first quartile (Q1), Median (Q2), third quartile (Q3), maximum limit</returns>
    public static (double minimum, double q1, double q2, double q3, double maximum) ComputeBoxPlotValues(double[] signal, bool IsSorted = true, double factor = 1.5)
    {
        double[] array = new double[signal.Length];
        Array.Copy(signal, array, signal.Length);
        
        if (!IsSorted) Array.Sort(array);

        (double q1, double q2, double q3) = Quartiles(array);

        double iqr = q3 - q1;

        return (q1 - factor * iqr, q1, q2, q3, q3 + factor * iqr);

    }
}

