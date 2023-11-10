using System.Numerics;
using System.Runtime.Intrinsics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SignalAnalysis;

public enum EntropyMethod
{
    BruteForce,
    MonteCarlo
}

/// <summary>
/// Routines to compute the entropy in signals (data-array points)
/// <seealso cref="https://www.codeproject.com/Articles/27030/Approximate-and-Sample-Entropies-Complexity-Metric"/>
/// <seealso cref="https://pdfs.semanticscholar.org/6841/efb4c40a74c1faa9f36e6d949d1ee330bfa9.pdf"/>
/// <seealso cref="https://en.wikipedia.org/wiki/Sample_entropy"/>
/// </summary>
public static class Complexity
{
    [Obsolete("This is deprecated. Use Entropy_Parallel instead with is roughly twice as faster")]
    /// <summary>
    /// Computes the approximate and sample entropies of a physiological time-series signals (typically used to diagnose diseased states).
    /// ApEn reflects the likelihood that similar patterns of observations will not be followed by additional similar observations. A time series containing many repetitive patterns has a relatively small ApEn; a less predictable process has a higher ApEn.
    /// A smaller value of SampEn also indicates more self-similarity in data set or less noise.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="dim">Embedding dimension</param>
    /// <param name="fTol">Factor to compute the tolerance so that the total is typically equal to 0.2*std</param>
    /// <param name="std">Standard deviation of the population</param>
    /// <returns>AppEn and SampEn</returns>
    public static (double AppEn, double SampEn) Entropy(double[] data, CancellationToken ct, uint dim = 2, double fTol = 0.2, double? std = null)
    {
        long upper = data.Length - (dim + 1) + 1;
        bool isEqual;
        ulong AppEn_Cum, AppEn_Cum1;
        ulong SampEn_Cum = 0, SampEn_Cum1 = 0;
        double sum = 0.0;
        double appEn, sampEn;
        double tolerance;
        if (std.HasValue)
            tolerance = std.Value * fTol;
        else
            tolerance = StdDev<double>(data) * fTol;


        for (uint i = 0; i < upper; i++)
        {
            AppEn_Cum = 0;
            AppEn_Cum1 = 0;
            for (uint j = 0; j < upper; j++)
            {
                isEqual = true;
                //m - length series
                for (uint k = 0; k < dim; k++)
                {
                    if (Math.Abs(data[i + k] - data[j + k]) > tolerance)
                    {
                        isEqual = false;
                        break;
                    }
                    if (ct.IsCancellationRequested)
                        throw new OperationCanceledException("CancelEntropy", ct);
                }
                if (isEqual)
                {
                    AppEn_Cum++;
                    SampEn_Cum++;
                }

                //m+1 - length series
                if (isEqual && Math.Abs(data[i + dim] - data[j + dim]) <= tolerance)
                {
                    AppEn_Cum1++;
                    SampEn_Cum1++;
                }
            }

            if (AppEn_Cum > 0 && AppEn_Cum1 > 0)
                sum += Math.Log((double)AppEn_Cum / (double)AppEn_Cum1);
        }

        appEn = sum / (double)(data.Length - dim);
        sampEn = SampEn_Cum > 0 && SampEn_Cum1 > 0 ? Math.Log((double)SampEn_Cum / (double)SampEn_Cum1) : 0.0;

        return (appEn, sampEn);
    }

    /// <summary>
    /// Parallel computes the approximate and sample entropies of a physiological time-series signals (typically used to diagnose diseased states).
    /// ApEn reflects the likelihood that similar patterns of observations will not be followed by additional similar observations. A time series containing many repetitive patterns has a relatively small ApEn; a less predictable process has a higher ApEn.
    /// A smaller value of SampEn also indicates more self-similarity in data set or less noise.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="dim">Embedding dimension</param>
    /// <param name="fTol">Factor to compute the tolerance so that the total is typically equal to 0.2*std</param>
    /// <param name="std">Standard deviation of the population</param>
    /// <returns>AppEn and SampEn</returns>
    public static (double AppEn, double SampEn) Entropy_Parallel(double[] data, CancellationToken ct, uint dim = 2, double fTol = 0.2, double? std = null)
    {
        long upper = data.Length - (dim + 1) + 1;
        double appEn, sampEn;
        double tolerance;
        if (std.HasValue)
            tolerance = std.Value * fTol;
        else
            tolerance = StdDev<double>(data) * fTol;


        int[] AppEn_Cum_Arr = new int[upper];
        int[] AppEn_Cum1_Arr = new int[upper];
        int[] SampEn_Cum_Arr = new int[upper];
        int[] SampEn_Cum1_Arr = new int[upper];
        double[] sum_Arr = new double[upper];
        bool[] isEqual_Arr = new bool[upper];

        Parallel.For(0, upper, i =>
        {
            AppEn_Cum_Arr[i] = 0;
            AppEn_Cum1_Arr[i] = 0;
            for (uint j = 0; j < upper; j++)
            {
                isEqual_Arr[i] = true;
                //m - length series
                for (uint k = 0; k < dim; k++)
                {
                    if (Math.Abs(data[i + k] - data[j + k]) > tolerance)
                    {
                        isEqual_Arr[i] = false;
                        break;
                    }
                    if (ct.IsCancellationRequested)
                        throw new OperationCanceledException("CancelEntropy", ct);
                }
                if (isEqual_Arr[i])
                {
                    AppEn_Cum_Arr[i]++;
                    SampEn_Cum_Arr[i]++;
                }

                //m+1 - length series
                if (isEqual_Arr[i] && Math.Abs(data[i + dim] - data[j + dim]) <= tolerance)
                {
                    AppEn_Cum1_Arr[i]++;
                    SampEn_Cum1_Arr[i]++;
                }
            }
            if (AppEn_Cum_Arr[i] > 0 && AppEn_Cum1_Arr[i] > 0)
                sum_Arr[i] = Math.Log((double)AppEn_Cum_Arr[i] / (double)AppEn_Cum1_Arr[i]);
        });

        appEn = sum_Arr.Sum() / (double)(data.Length - dim);

        ulong SampEn_Cum = 0;
        foreach (int x in SampEn_Cum_Arr)
            SampEn_Cum += (ulong)x;

        ulong SampEn_Cum1 = 0;
        foreach (int x in SampEn_Cum1_Arr)
            SampEn_Cum1 += (ulong)x;

        sampEn = SampEn_Cum > 0 && SampEn_Cum1 > 0 ? Math.Log((double)SampEn_Cum / (double)SampEn_Cum1) : 0.0;

        return (appEn, sampEn);
    }

    /// <summary>
    /// Computes the Shannon entropy, the entropy bit, the ideal entropy, and the ratio Shannon/ideal for a vector of numeric values
    /// </summary>
    /// <param name="data">Numeric values vector</param>
    /// <returns>The Shannon entropy value (bits per symbol), the entropy bit (min. number of bits needed to encode the vector), he ideal entropy, and the ratio Shannon/Ideal</returns>
    public static (double Entropy, double EntropyBit, double IdealEntropy, double EntropyRatio) ShannonEntropy<T>(IEnumerable<T> data)
    {
        double entropy = 0;
        double entropyBit;
        double entropyIdeal;
        double entropyRatio;
        double prob;

        // Convert into an enumerable of doubles.
        IEnumerable<double> values = data.Select(value => Convert.ToDouble(value));
        int nLength = values.Count();

        // Group data values
        // https://stackoverflow.com/questions/20765589/how-do-i-find-duplicates-in-an-array-and-display-how-many-times-they-occurred
        var dict = new Dictionary<double, int>();
        foreach (var value in values)
        {
            // When the key is not found, "count" will be initialized to 0
            dict.TryGetValue(value, out int count);
            dict[value] = count + 1;
        }
        // This also works, but it might be slower since each group in groups need to be counted again
        //var groups = values.GroupBy(v => v);

        // Compute the Shannon entropy
        foreach (var value in dict)
        {
            if (value.Key > 0)
            {
                //prob = value.Value / (value.Key / decimalFactor);
                prob = (double)value.Value / nLength;
                entropy -= prob * Math.Log2(prob);
            }
        }

        // https://github.com/wqyeo/Shannon-Entropy/blob/master/EntropyCal.cs
        // This represents the minimum number of bits to encode the whole vector
        entropyBit = Math.Ceiling(entropy) * nLength;

        // https://stackoverflow.com/questions/2979174/how-do-i-compute-the-approximate-entropy-of-a-bit-string
        // The Shannon entropy if all elements where different
        prob = 1.0 / nLength;
        entropyIdeal = -1.0 * nLength * prob * Math.Log2(prob);

        entropyRatio = entropy / entropyIdeal;

        return (entropy, entropyBit, entropyIdeal, entropyRatio);
    }

    /// <summary>
    /// Computes the standard deviation of a data series.
    /// </summary>
    /// <param name="values">Data values</param>
    /// <param name="asSample"><see langword="True"/> to compute the sample standard deviation (N-1); otherwise (by default), it computes the population (N) deviation</param>
    /// <returns>Standard deviation. A value equal to -1 indicates insufficient data points.</returns>
    public static double StdDev<T>(IEnumerable<T> values, bool asSample = false)
    {
        // Convert into an enumerable of doubles.
        IEnumerable<double> doubles = values.Select(value => Convert.ToDouble(value));

        // Then compute the standard deviation
        double avg = System.Linq.Enumerable.Average(doubles);
        double sum = System.Linq.Enumerable.Sum(System.Linq.Enumerable.Select(doubles, x => (x - avg) * (x - avg)));
        double denominator = values.Count() - (asSample ? 1 : 0);
        return denominator > 0.0 ? Math.Sqrt(sum / denominator) : -1;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="ct"></param>
    /// <param name="dim"></param>
    /// <param name="fTol"></param>
    /// <param name="std"></param>
    /// <returns></returns>
    /// <seealso cref="https://www.mdpi.com/1099-4300/24/4/524"/>
    /// <seealso cref="https://github.com/phreer/sampen_estimation"/>
    //public static (double AppEn, double SampEn) Entropy_SuperFast(double[] data, CancellationToken ct, uint dim = 2, double fTol = 0.2, double? std = null)
    //{
    //    int N = data.Length;
    //    double result = 0;
    //    long A = 0;
    //    long B = 0;

    //    // Check we have enough data points
    //    if (N <= dim) return (-1.0, -1.0);

    //    int[] AB = _ComputeAB(data, dim, fTol);
    //    int sample_num = AB.Length / 2;

    //    for (int i = 0; i < sample_num; i++)
    //    {
    //        A += AB[i * 2];
    //        B += AB[i * 2 + 1];
    //    }
    //    //if (a) *a = A / sample_num;
    //    //if (b) *b = B / sample_num;

    //    if (A > 0 && B > 0)
    //        result = -Math.Log(B / A);
    //    else
    //        result = -Math.Log((N - dim - 1) / (N - dim));

    //    return (0.0, 0.0);
    //}

    //private static List<long> _ComputeAB(List<double> data, uint m, double r)
    //{
    //    List<Point> points = GetPoints(data, m + 1);
    //    return ComputeAB(points, r);
    //}

    //private static List<Point> GetPoints(List<double> data, uint m)
    //{
    //    List<Point> result = new List<Point>(data.Count - (int)m + 1);
    //    for (int i = 0; i < result.Count; i++)
    //    {
    //        //List<int> subData = data.GetRange(i, (int)m);
    //        //Point p = new Point(subData, 0);
    //        //Point p = new(data[0] + i + (int)m, 0);
    //        result[i] = new(data[0] + i + (int)m, 0);
    //    }
    //    return result;
    //}

    //private static List<long> ComputeAB(List<Point> points, int r)
    //{
    //    int n = points.Count;
    //    List<long> result = new List<long>(2) { 0, 0 };
    //    if (n == 0) return result;
    //    result = CountMatchedPara(points, r);
    //    return result;
    //}

    //private static List<long> CountMatchedPara(List<Point> points, int r)
    //{
    //    int n = points.Count;
    //    int num_threads = Environment.ProcessorCount;
    //    if (num_threads == 0) num_threads = 16;
    //    else if (num_threads > 12) num_threads -= 8;
    //    else num_threads /= 2;
    //    List<long> As = new List<long>(num_threads);
    //    List<long> Bs = new List<long>(num_threads);
    //    for (int i = 0; i < num_threads; i++)
    //    {
    //        As.Add(0);
    //        Bs.Add(0);
    //    }
    //    if (num_threads > n) num_threads = n / 2;
    //    List<Thread> threads = new List<Thread>();
    //    for (int i = 0; i < num_threads; i++)
    //    {
    //        threads.Add(new Thread(() => CountMatched(points, r, (uint)i, (uint)num_threads, As, Bs)));
    //    }
    //    foreach (Thread thread in threads)
    //    {
    //        thread.Start();
    //    }
    //    foreach (Thread thread in threads)
    //    {
    //        thread.Join();
    //    }
    //    List<long> AB = new List<long>(2);
    //    AB[0] = As.Sum();
    //    AB[1] = Bs.Sum();
    //    return AB;
    //}

    //private static void CountMatched(List<Point> points, int r, uint offset, uint interval, List<long> As, List<long> Bs)
    //{
    //    uint n = (uint)points.Count;
    //    uint m = (uint)points[0].dim() - 1;
    //    uint index = 0;
    //    for (uint i = 0; (index = i * interval + offset) < n; ++i)
    //    {
    //        Point p = points[(int)index];
    //        for (uint j = index + 1; j < n; j++)
    //        {
    //            if (p.within(points[(int)j], m, r))
    //            {
    //                As[(int)offset] += 1;
    //                if (-r <= p[m] - points[(int)j][m] && p[m] - points[(int)j][m] <= r)
    //                {
    //                    Bs[(int)offset] += 1;
    //                }
    //            }
    //        }
    //    }
    //}

}

