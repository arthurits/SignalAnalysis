using System.Numerics;

namespace SignalAnalysis;

public enum EntropyMethod
{
    BruteForce,
    MonteCarloUniform,
    MonteCarloRandom,
    MonteCarloRandomSorted
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
    /// <param name="dim">Embedding dimension (factor m)</param>
    /// <param name="fTol">Factor to compute the "noise filter" of "factor r" so that the total is typically equal to 0.2*std</param>
    /// <param name="std">Standard deviation of the population</param>
    /// <returns>ApEn and SampEn</returns>
    public static (double AppEn, double SampEn) Entropy(double[] data, CancellationToken ct, uint dim = 2, double fTol = 0.2, double? std = null)
    {
        long blocks = data.Length - (dim - 1);  // The total number of blocks for the "possible" template
        if (blocks <= 0) return (-1.0, -1.0);   // Check there are enough data points to define the blocks
        double apEn, sampEn = -1.0;
        double noiseFilter = fTol * (std ?? Math.Sqrt(Statistics.Descriptive.Variance(data))); // Factor r (also known as tolerance)
        uint k;

        uint[] alreadyPossible = new uint[blocks];
        uint[] alreadyMatch = new uint[blocks - 1];
        uint[] possible = new uint[blocks];
        uint[] match = new uint[blocks - 1];

        Array.Fill(alreadyPossible, (uint)1);
        Array.Fill(alreadyMatch, (uint)1);

        for (uint i = 0; i < blocks; i++)
        {
            for (uint j = i + 1 ; j < blocks; j++)  // We take advantage of the symmetry: instead of j = 0 we start from j > i and keep count of the previuos possibles and matchs
            {
                // Check for "possibles". This corresponds to the m - length block        
                for (k = 0; k < dim; k++)
                {
                    if (Math.Abs(data[i + k] - data[j + k]) > noiseFilter)
                        break;
                    if (ct.IsCancellationRequested)
                        throw new OperationCanceledException("CancelEntropy", ct);
                }

                if (k == dim)
                {
                    possible[i]++;
                    alreadyPossible[j]++;

                    if (j < match.Length)
                    {
                        // Check for "matches". This corresponds to the m+1 - length block
                        if (Math.Abs(data[i + dim] - data[j + dim]) <= noiseFilter)
                        {
                            match[i]++;
                            alreadyMatch[j]++;
                        }
                    }
                }
            }
        }

        // Add previous stored symmetric values
        for (int i = 0; i < match.Length; i++)
        {
            alreadyPossible[i] += possible[i];
            alreadyMatch[i] += match[i];
        }
        if (possible.Length > match.Length) alreadyPossible[match.Length] += possible[match.Length];

        // Compute ApEn using the original definition without any simplifications nor approximations
        double sumPossible = alreadyPossible.Sum(x => x > 0 ? Math.Log(x) : 0) / alreadyPossible.Length;
        sumPossible -= Math.Log(alreadyPossible.Length);
        double sumMatch = alreadyMatch.Sum(x => x > 0 ? Math.Log(x) : 0) / alreadyMatch.Length;
        sumMatch -= Math.Log(alreadyMatch.Length);
        apEn = sumPossible - sumMatch;

        // Compute SampEn
        ulong A = 0;
        ulong B = 0;
        foreach (uint x in match)
            A += x;
        foreach (uint x in possible)
            B += x;

        if (A > 0 && B > 0)
            sampEn = -Math.Log((double)A / B);

        // Return values
        return (apEn, sampEn);
    }

    [Obsolete("This is deprecated. Use Entropy_SuperFast instead")]
    /// <summary>
    /// Parallel computes the approximate and sample entropies of a physiological time-series signals (typically used to diagnose diseased states).
    /// ApEn reflects the likelihood that similar patterns of observations will not be followed by additional similar observations. A time series containing many repetitive patterns has a relatively small ApEn; a less predictable process has a higher ApEn.
    /// A smaller value of SampEn also indicates more self-similarity in data set or less noise.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="dim">Embedding dimension (factor m)</param>
    /// <param name="fTol">Factor to compute the "noise filter" of "factor r" so that the total is typically equal to 0.2*std</param>
    /// <param name="std">Standard deviation of the population</param>
    /// <returns>ApEn and SampEn</returns>
    public static (double AppEn, double SampEn) Entropy_Parallel(double[] data, CancellationToken ct, uint dim = 2, double fTol = 0.2, double? std = null)
    {
        long blocks = data.Length - (dim - 1);  // The total number of blocks for the "possible" template
        if (blocks <= 0) return (-1.0, -1.0);   // Check there are enough data points to define the blocks
        double apEn, sampEn;
        double noiseFilter = fTol * (std ?? Math.Sqrt(Statistics.Descriptive.Variance(data))); // Factor r (also known as tolerance)

        ulong[] ApEnPossible = new ulong[blocks];
        ulong[] ApEnMatch = new ulong[blocks - 1];
        ulong[] SampEnPossible = new ulong[blocks];
        ulong[] SampEnMatch = new ulong[blocks - 1];
        //double[] sumArr = new double[blocks];

        var options = new ParallelOptions()
        {
            CancellationToken = ct,
            MaxDegreeOfParallelism = Environment.ProcessorCount - 1
        };

        try
        {

            Parallel.For(0, blocks, options, i =>
            {
                uint k;
                for (uint j = 0; j < blocks; j++)
                {
                    // Check for "possibles". This corresponds to the m - length block
                    for (k = 0; k < dim; k++)
                    {
                        if (Math.Abs(data[i + k] - data[j + k]) > noiseFilter)
                            break;
                    }
                    if (k == dim)
                    {
                        ApEnPossible[i]++;
                        if (j > i) SampEnPossible[i]++;

                        if (j < ApEnMatch.Length && i < ApEnMatch.Length)
                        {
                            // Check for "matches". This corresponds to the m+1 - length block
                            if (Math.Abs(data[i + dim] - data[j + dim]) <= noiseFilter)
                            {
                                ApEnMatch[i]++;
                                if (j > i) SampEnMatch[i]++;
                            }
                        }
                    }
                }
                //if (AppEnPossible[i] > 0 && AppEnMatch[i] > 0)
                //    sumArr[i] = Math.Log((double)AppEnPossible[i] / (double)AppEnMatch[i]);
            });
        }
        catch(OperationCanceledException)
        {
            throw new OperationCanceledException("CancelEntropy", ct);
        }

        //appEn = sumArr.Sum() / (double)(data.Length - dim);

        // Compute ApEn using the original definition without any simplifications nor approximations 
        double sumPossible = ApEnPossible.Sum(x => x > 0 ? Math.Log(x) : 0) / ApEnPossible.Length;
        sumPossible -= Math.Log(ApEnPossible.Length);
        double sumMatch = ApEnMatch.Sum(x => x > 0 ? Math.Log(x) : 0) / ApEnMatch.Length;
        sumMatch -= Math.Log(ApEnMatch.Length);
        apEn = sumPossible - sumMatch;

        // Compute SampEn
        ulong SampEnPossible_1 = 0;
        foreach (ulong x in SampEnPossible)
            SampEnPossible_1 += x;

        ulong SampEnMatch_1 = 0;
        foreach (ulong x in SampEnMatch)
            SampEnMatch_1 += x;

        sampEn = SampEnPossible_1 > 0 && SampEnMatch_1 > 0 ? Math.Log((double)SampEnPossible_1 / SampEnMatch_1) : 0.0;
        //sampEn = SampEnPossible_1 > 0 && SampEnMatch_1 > 0 ? Math.Log((double)SampEnPossible.Sum() / SampEnMatch.Sum()) : 0.0;

        return (apEn, sampEn);
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
        IEnumerable<double> doubles = values.AsParallel().Select(value => Convert.ToDouble(value));

        // Then compute the standard deviation
        //double avg = System.Linq.Enumerable.Average(doubles);
        //double sst = System.Linq.Enumerable.Sum(System.Linq.Enumerable.Select(doubles, x => (x - avg) * (x - avg)));    // Sum of squares total
        double avg = doubles.AsParallel().Average();
        double sst = doubles.AsParallel().Sum(x => (x - avg) * (x - avg));   // Sum of squares total
        int denominator = values.Count() - (asSample ? 1 : 0);
        return denominator > 0.0 ? Math.Sqrt(sst / denominator) : -1.0;
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
    public static (double AppEn, double SampEn) Entropy_SuperFast(double[] data, CancellationToken ct, EntropyMethod entropyMethod = EntropyMethod.BruteForce, uint dim = 2, double fTol = 0.2, double? std = null)
    {
        int N = data.Length;
        double ApEn = -1.0, SampEn = -1.0;
        ulong A = 0;
        ulong B = 0;
        double noiseFilter = fTol * (std ?? Math.Sqrt(Statistics.Descriptive.Variance(data))); // Factor r (also known as tolerance)

        // Check we have enough data points
        if (N <= dim) return (-1.0, -1.0);

        // Compute sampling parameters as suggested in https://doi.org/10.3390/e24040524
        int sampleSize = data.Length - (int)dim;
        int sampleNum = 1;
        int times = 0;
        if (entropyMethod != EntropyMethod.BruteForce)
        {
            //sampleSize = data.Length < 1024 ? (int)Math.Sqrt(data.Length) : Math.Max(1024, (int)Math.Sqrt(data.Length));
            //sampleNum = Math.Min(5 + (int)Math.Log2(data.Length), data.Length / sampleSize);
            (times, sampleSize) = TimesDivisionByDivisor(sampleSize, 2, (int)Math.Max(512, (int)Math.Sqrt(sampleSize)));
            sampleNum = Math.Min(5 + (int)Math.Log2(data.Length), data.Length / sampleSize);
        }

        // Compute "possibles" (B) and "matches" (A) for ApEn and SampEn
        (ulong[] apEnPossible, ulong[] apEnMatch, ulong[] sampEnPossible, ulong[] sampEnMatch) = entropyMethod switch
        {
            EntropyMethod.BruteForce => CountMatchedParallel(data, dim, noiseFilter, ct),
            EntropyMethod.MonteCarloUniform => ComputeAB_Uniform2(data, dim, noiseFilter, sampleSize, sampleNum, ct),
            EntropyMethod.MonteCarloRandom => ComputeAB_QuasiRandom2(data, dim, noiseFilter, sampleSize, sampleNum, ct),
            EntropyMethod.MonteCarloRandomSorted => ComputeAB_QuasiRandom2(data, dim, noiseFilter, sampleSize, sampleNum, ct),
            _ => ([], [], [], [])
        };

        // Compute ApEn using the original definition without any simplifications nor approximations
        //double sumPossible = apEnPossible.Sum(x => x > 0 ? Math.Log(x) : 0) / apEnPossible.Length;
        //sumPossible -= Math.Log(apEnPossible.Length);
        //double sumMatch = apEnMatch.Sum(x => x > 0 ? Math.Log(x) : 0) / apEnMatch.Length;
        //sumMatch -= Math.Log(apEnMatch.Length);
        //ApEn = sumPossible - sumMatch;

        //double ApEn2 = ApEn * Math.Pow(Math.Log(Math.PI), times);
        //ApEn2 = 0;
        int index;
        double sumPossible;
        double sumMatch;
        if (sampleNum == 1)
        {
            // Compute ApEn using the original definition without any simplifications nor approximations
            sumPossible = apEnPossible.Sum(x => x > 0 ? Math.Log(x) : 0) / apEnPossible.Length;
            sumPossible -= Math.Log(apEnPossible.Length);
            sumMatch = apEnMatch.Sum(x => x > 0 ? Math.Log(x) : 0) / apEnMatch.Length;
            sumMatch -= Math.Log(apEnMatch.Length);
            ApEn = sumPossible - sumMatch;
        }
        else
        {
            for (int i = 0; i < sampleNum; i++)
            {
                sumPossible = sampleNum > 1 ? 0 : Math.Log(apEnPossible[^1]);
                sumMatch = 0;
                for (int j = 0; j < sampleSize; j++)
                {
                    index = i * sampleNum + j;
                    if (apEnPossible[index] > 0)
                        sumPossible += Math.Log(apEnPossible[index]);
                    if (apEnMatch[index] > 0)
                        sumMatch += Math.Log(apEnMatch[index]);
                }
                sumPossible /= (apEnPossible.Length / sampleNum);
                sumPossible -= Math.Log(apEnPossible.Length / sampleNum);
                sumMatch /= (apEnMatch.Length / sampleNum);
                sumMatch -= Math.Log(apEnMatch.Length / sampleNum);
                ApEn += (sumPossible - sumMatch) / sampleNum;
            }
            ApEn *= Math.Pow(Math.Log(Math.PI), times);
            //ApEn = sum / (double)(sampleSize * sampleNum);
            //ApEn += Math.Log(data.Length / sampleSize) / sampleNum; // This needs testing
            //ApEn = sum / (double)(data.Length - (int)dim);
            //ApEn += Math.Log((double)(data.Length - (int)dim) / (sampleSize * sampleNum)); // This needs testing
            //ApEn = sumPossible / (sampleSize * sampleNum);
            //ApEn *= (double)(data.Length - (int)dim) / (sampleSize * sampleNum);
        }

        // Compute SampEn
        foreach (ulong x in sampEnMatch)
            A += x;
        foreach (ulong x in sampEnPossible)
            B += x;

        if (A > 0 && B > 0)
            SampEn = -Math.Log((double)A / B);

        return (ApEn, SampEn);
    }

    //private static List<PointTree<double, int>> GetPoints(List<double> data, uint m)
    //{
    //    PointTree<double, int>[] result = new PointTree<double, int>[data.Count - (int)m + 1];
    //    for (int i = 0; i < result.Length; i++)
    //    {
    //        //List<double> subData = data.GetRange(i, (int)m);
    //        //PointTree<double, int> p = new PointTree<double, int>(subData, 0);
    //        //PointTree<double, int> p = new PointTree<double, int>(data.GetRange(i, (int)m), 0);
    //        result[i] = new PointTree<double, int>(data.GetRange(i, (int)m), 0);
    //    }
    //    return result.ToList();
    //}

    //private static List<long> ComputeAB(List<PointTree<double, int>> points, double r)
    //{
    //    int n = points.Count;
    //    List<long> result = new(2) { 0, 0 };
    //    if (n == 0) return result;
    //    result = CountMatchedParallel(points, r);
    //    return result;
    //}

    //private static List<long> CountMatchedParallel(List<PointTree<double, int>> data, double r)
    //{
    //    uint dim = (uint)data[0].Dim() - 1;
    //    int n = data.Count;
    //    int numThreads = Environment.ProcessorCount;
    //    //if (num_threads == 0) num_threads = 16;
    //    //else if (num_threads > 12) num_threads -= 8;
    //    //else num_threads /= 2;
    //    //if (num_threads > n) num_threads = n / 2;

    //    List<long> As = new(n);
    //    List<long> Bs = new(n);
    //    for (int i = 0; i < n; i++)
    //    {
    //        As.Add(0);
    //        Bs.Add(0);
    //    }

    //    var options = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount - 1 };

    //    Parallel.For(0, n, options, i =>
    //    {
    //        PointTree<double, int> p = data[(int)i];
    //        for (uint j = (uint)(i + 1); j < n; j++)
    //        {
    //            if (p.Within(data[(int)j], (int)dim, r))
    //            {
    //                As[(int)i] += 1;
    //                if (-r <= p[(int)dim] - data[(int)j][(int)dim] && p[(int)dim] - data[(int)j][(int)dim] <= r)
    //                {
    //                    Bs[(int)i] += 1;
    //                }
    //            }
    //        }
    //    });

    //    //for (uint i = 0; i < n; i++)
    //    //{
    //    //        PointTree<double, int> p = data[(int)i];
    //    //        for (uint j = i + 1; j < n; j++)
    //    //        {
    //    //            if (p.Within(data[(int)j], (int)dim, r))
    //    //            {
    //    //                As[(int)i] += 1;
    //    //                if (-r <= p[(int)dim] - data[(int)j][(int)dim] && p[(int)dim] - data[(int)j][(int)dim] <= r)
    //    //                {
    //    //                    Bs[(int)i] += 1;
    //    //                }
    //    //            }
    //    //        }
    //    //}

    //    //List<Thread> threads = new();
    //    //for (int i = 0; i < num_threads; i++)
    //    //{
    //    //    threads.Add(new Thread(() => CountMatched(points, r, (uint)i, (uint)num_threads, As, Bs)));
    //    //}
    //    //foreach (Thread thread in threads)
    //    //{
    //    //    thread.Start();
    //    //}
    //    //foreach (Thread thread in threads)
    //    //{
    //    //    thread.Join();
    //    //}

    //    List<long> AB = new() { As.Sum(), Bs.Sum() };
    //    return AB;
    //}

    private static (ulong[] apEnPossible, ulong[] apEnMatch, ulong[] sampEnPossible, ulong[] sampEnMatch) CountMatchedParallel(double[] data, uint dim, double r, CancellationToken ct, int[]? indices = null)
    {
        bool useData = indices is null;
        indices ??= Enumerable.Range(0, data.Length).ToArray();
        int blocks = indices.Length - (useData ? (int)dim - 1 : 0);

        ulong[] alreadyPossible = new ulong[blocks];
        ulong[] alreadyMatch = new ulong[useData ? blocks - 1 : blocks];
        ulong[] apEnPossible = new ulong[blocks];
        ulong[] apEnMatch = new ulong[useData ? blocks - 1 : blocks];
        ulong[] sampEnPossible = new ulong[blocks];
        ulong[] sampEnMatch = new ulong[useData ? blocks - 1 : blocks];

        var options = new ParallelOptions()
        {
            CancellationToken = ct,
            MaxDegreeOfParallelism = Environment.ProcessorCount - 1
        };

        try
        {
            Parallel.For(0, blocks, options, i =>
            {
                for (uint j = 0; j < blocks; j++)
                {
                    // Check for "possibles". This corresponds to the m - length block
                    int k;
                    for (k = 0; k < dim; k++)
                    {
                        if (Math.Abs(data[indices[i] + k] - data[indices[j] + k]) > r)
                            break;
                    }
                    if (k == dim)
                    {
                        apEnPossible[i]++;
                        alreadyPossible[j]++;
                        if (j > i) sampEnPossible[i]++;

                        if (j < apEnMatch.Length && i < apEnMatch.Length)
                        {
                            // Check for "matches". This corresponds to the m+1 - length block
                            if (Math.Abs(data[indices[i] + dim] - data[indices[j] + dim]) <= r)
                            {
                                apEnMatch[i]++;
                                alreadyMatch[j]++;
                                if (j > i) sampEnMatch[i]++;
                            }
                        }
                    }
                }
            });
        }
        catch (OperationCanceledException)
        {
            throw new OperationCanceledException("CancelEntropy", ct);
        }

        return (apEnPossible, apEnMatch, sampEnPossible, sampEnMatch);
    }

    //private static long[] ComputeAB_QuasiRandom(double[] data, uint m, double r, int SampleSize = 1024, int SampleNum = 8, bool presort = true)
    //{
    //    List<PointTree<double, int>> points = GetPoints(data.ToList(), m + 1);
    //    int numPoints = points.Count;
    //    Random random = new();

    //    if (presort)
    //    {
    //        points.Sort((p1, p2) =>
    //        {
    //            for (int i = 0; i < p1.Dim(); i++)
    //            {
    //                if (p1[i] > p2[i])
    //                    return 1;
    //                if (p1[i] < p2[i])
    //                    return -1;
    //            }
    //            return 1;
    //        });
    //    }

    //    long[] ABs = new long[2 * SampleNum];
    //    //for (int i = 0; i < (2 * SampleNum); ++i)
    //    //    ABs.Add(0);

    //    List<PointTree<double, int>> sampled_points = new (SampleSize);
    //    int[] indices = new int[SampleSize];
    //    int[] offsets = new int[SampleNum -1];
    //    int[] tmp_indices = new int[SampleSize];

    //    for (int j = 0; j < SampleSize; j++)
    //        indices[j] = random.Next(0, numPoints);
    //        // indices[j] = (n / sample_size) * j;

    //    for (int i = 0; i < SampleNum - 1; ++i)
    //        offsets[i] = random.Next(0, numPoints);

    //    for (int i = 0; i < SampleNum; i++)
    //    {
            
    //        if (i is not 0)
    //        {
    //            int offset = offsets[i - 1];
    //            for (uint j = 0; j < SampleSize; ++j)
    //                tmp_indices[j] = (indices[j] + offset) % numPoints;

    //        }
    //        else
    //        {
    //            for (uint j = 0; j < SampleSize; ++j)
    //                tmp_indices[j] = indices[j];
    //        }

    //        for (int j = 0; j < SampleSize; ++j)
    //            sampled_points[j] = points[tmp_indices[j]];

    //        long[] AB = ComputeAB(sampled_points, r).ToArray();
    //        ABs[2 * i] += AB[0];
    //        ABs[2 * i + 1] += AB[1];
    //    }
    //    return ABs;
    //}

    private static (ulong[] apEnPossible, ulong[] apEnMatch, ulong[] sampEnPossible, ulong[] sampEnMatch) ComputeAB_QuasiRandom2(double[] data, uint m, double r, int SampleSize, int SampleNum, CancellationToken ct, bool presort = true)
    {
        int numPoints = data.Length - (int)m;
        Random random = new();

        ulong[] apEnPossible = new ulong[SampleSize * SampleNum];
        ulong[] apEnMatch = new ulong[SampleSize * SampleNum];
        ulong[] sampEnPossible = new ulong[SampleSize * SampleNum];
        ulong[] sampEnMatch = new ulong[SampleSize * SampleNum];

        double[] sampled_points = new double[SampleSize];
        int[] indices = new int[SampleSize];
        int[] offsets = new int[SampleNum - 1];
        int[] tmp_indices = new int[SampleSize];

        if (presort)
            Array.Sort(data);

        for (int j = 0; j < SampleSize; j++)
            indices[j] = random.Next(0, numPoints);

        for (int i = 0; i < SampleNum - 1; i++)
            offsets[i] = random.Next(0, numPoints);

        for (int i = 0; i < SampleNum; i++)
        {

            if (i is not 0)
            {
                int offset = offsets[i - 1];
                for (uint j = 0; j < SampleSize; ++j)
                    tmp_indices[j] = (indices[j] + offset) % numPoints;

            }
            else
            {
                for (uint j = 0; j < SampleSize; ++j)
                    tmp_indices[j] = indices[j];
            }

            for (int j = 0; j < SampleSize; ++j)
                sampled_points[j] = data[tmp_indices[j]];

            (var a, var b, var c, var d) = CountMatchedParallel(sampled_points, m, r, ct);
            Array.Copy(a, 0, apEnPossible, SampleSize * i, SampleSize);
            Array.Copy(b, 0, apEnMatch, SampleSize * i, SampleSize);
            Array.Copy(c, 0, sampEnPossible, SampleSize * i, SampleSize);
            Array.Copy(d, 0, sampEnMatch, SampleSize * i, SampleSize);
        }
        return (apEnPossible, apEnMatch, sampEnPossible, sampEnMatch);
    }

    //private static long[] ComputeAB_Uniform(double[] data, uint m, double r, int SampleSize = 1024, int SampleNum = 8)
    //{
    //    List<PointTree<double, int>> points = GetPoints(data.ToList(), m + 1);
    //    PointTree<double, int>[] sampled_points = new PointTree<double, int>[SampleSize];
    //    Random random = new();
    //    int numPoints = points.Count;

    //    long[] ABs = new long[2 * SampleNum];
    //    //for (int i = 0; i < (2 * SampleNum); ++i)
    //    //    ABs.Add(0);

    //    for (int i = 0; i < SampleNum; i++)
    //    {
    //        // generate points
    //        for (int j = 0; j < SampleSize; j++)
    //            sampled_points[j] = points[random.Next(0, numPoints)];

    //        long[] AB = ComputeAB(sampled_points.ToList(), r).ToArray();
    //        ABs[2 * i] += AB[0];
    //        ABs[2 * i + 1] += AB[1];
    //    }
    //    return ABs;
    //}

    /// <summary>
    /// Computes the "possibles" (A) and "matches" (B) by uniformly sampling the data
    /// </summary>
    /// <param name="data">Data points to be sampled</param>
    /// <param name="dim">Embedding dimension</param>
    /// <param name="r">Factor r (noise filter)</param>
    /// <param name="SampleSize">Size (number of points) of the sample</param>
    /// <param name="SampleNum">Number of samples to be created</param>
    /// <returns>ApEn "possibles" (A), ApEn "matches" (B), SampEn "possibles" (A), SampEn "matches" (B)</returns>
    private static (ulong[] apEnPossible, ulong[] apEnMatch, ulong[] sampEnPossible, ulong[] sampEnMatch) ComputeAB_Uniform2(double[] data, uint dim, double r, int SampleSize, int SampleNum, CancellationToken ct)
    {
        Random random = new();
        int[] sampledPoints = new int[SampleSize];
        int numPoints = data.Length - (int)dim;
        //int sampleSize = SampleSize - (int)m;

        ulong[] apEnPossible = new ulong[SampleSize * SampleNum];
        ulong[] apEnMatch = new ulong[SampleSize * SampleNum];
        ulong[] sampEnPossible = new ulong[SampleSize * SampleNum];
        ulong[] sampEnMatch = new ulong[SampleSize * SampleNum];

        for (int i = 0; i < SampleNum; i++)
        {
            // Data sampling
            for (int j = 0; j < SampleSize; j++)
                sampledPoints[j] = random.Next(0, numPoints);

            // Count "possibles" (A) and "matches" (B)
            (var a, var b, var c, var d) = CountMatchedParallel(data, dim, r, ct, sampledPoints);
            Array.Copy(a, 0, apEnPossible, SampleSize * i, SampleSize);
            Array.Copy(b, 0, apEnMatch, SampleSize * i, SampleSize);
            Array.Copy(c, 0, sampEnPossible, SampleSize * i, SampleSize);
            Array.Copy(d, 0, sampEnMatch, SampleSize * i, SampleSize);
        }
        return (apEnPossible, apEnMatch, sampEnPossible, sampEnMatch);
    }

    /// <summary>
    /// Fast ApEn function translated from TSEntropies package
    /// </summary>
    /// <param name="data">Time series</param>
    /// <param name="dim">embedding dimension</param>
    /// <param name="lag">embedding lag/delay</param>
    /// <param name="noiseFilter">Noise filter</param>
    /// <returns>ApEn value</returns>
    /// <seealso cref="https://github.com/cran/TSEntropies/blob/master/src/ApEn.c"/>
    public static double FastApEn_Cfun(double[] data, int dim, int lag, double noiseFilter)
    {
        int blocks = data.Length - (dim * lag) + 1;  // number of sequencies inside of time series TS
        int validCases; // number of value classes
        int count;
        bool[] alreadyMatched;  // allocating space for match records
        double[] temp_res = new double[2];  // temporary results
        double logji;

        //ulong[] apEnPossible = new ulong[blocks];
        //ulong[] apEnMatch = new ulong[blocks - 1];
        //ulong[] sampEnPossible = new ulong[blocks];
        //ulong[] sampEnMatch = new ulong[blocks - 1];

        for (int run = 0; run < 2; run++)
        {
            validCases = 0;
            logji = 0;
            alreadyMatched = new bool[blocks]; // clearing match records

            for (int i = 0; i < blocks; i++)
            {
                if (alreadyMatched[i] == true)
                    continue;
                count = 0;
                validCases++;
                for (int j = 0; j < blocks; j++)
                {
                    if (alreadyMatched[j] == true)
                        continue;
                    int m;
                    for (m = 0; m < dim; m++)
                    {
                        if (Math.Abs(data[i + m * lag] - data[j + m * lag]) > noiseFilter)
                            break;  // test for Maximum distance L_infinity
                    }
                    if (m == dim)  // test if all dimensions distances are less than or equal to eps
                    {
                        count++;
                        alreadyMatched[j] = true;
                    }
                }
                logji += Math.Log(count);
            }

            temp_res[run] = (logji / validCases) - Math.Log(validCases);
            //temp_res[run] = (logji - Math.Log(Ncl)) / Ncl;
            dim++;
            blocks -= lag;
        }
        
        //alreadyMatched = null;
        return temp_res[0] - temp_res[1];
    }

    public static double FastSampEn_Cfun(double[] data, int dim, int lag, double noiseFilter)
    {
        int blocks = data.Length - (dim * lag) + 1; // number of sequences inside of time series TS
        int count;
        bool[] alreadyMatched;
        double[] temp_res = new double[2];
        
        for (int run = 0; run < 2; run++)
        {
            count = 0;
            alreadyMatched = new bool[blocks]; // clearing match records

            int m;
            for (int i = 0; i < blocks; i++)
            {
                if (alreadyMatched[i] == true)
                    continue;
               
                for (int j = 0; j < blocks; j++)
                {
                    if (alreadyMatched[j] == true)
                        continue;
                    
                    for (m = 0; m < dim; m++)
                    {
                        if (Math.Abs(data[i + m * lag] - data[j + m * lag]) >= noiseFilter)
                            break;
                    }
                    if (m == dim) // test if all dimensions distance is less than or equal to eps
                    {
                        count++;
                        alreadyMatched[j] = true;
                    }
                }
                count--; // because SampEn do not count on self match
            }
            temp_res[run] = count;
            dim++;
            blocks -= lag;
        }

        return temp_res[1] != 0 ? Math.Log(temp_res[0] / temp_res[1]) : -1;
    }
    
    /// <summary>
    /// Computes the number of times a dividend can be continuously divided by a divisor without the quotient ever dropping below a lower limit
    /// </summary>
    /// <param name="dividend">Quantity to be divided by</param>
    /// <param name="divisor">Divisor value</param>
    /// <param name="lowerLimit">The lower limit value that the quotient should be above</param>
    /// <returns>The number of times the division has been done with the quotient above the lower limit</returns>
    public static (int times, int quotient) TimesDivisionByDivisor(int dividend, int divisor, int lowerLimit)
    {
        // Make sure dividend is above the lower limit
        if (lowerLimit >= dividend) return (0, dividend);

        int numTimes = 0;
        while (dividend > lowerLimit*divisor)
        {
            numTimes++;
            dividend /= divisor;
        }

        return (numTimes, dividend);
    }

}

/// <summary>
/// A point in euclidean space.
/// A class that represents a multi-dimensional euclidean point with some associated value.
/// We allow for each point to have an associated value so that some more information can be stored with each point.
/// Points can also have a multiplicity/count, this corresponds to having several duplicates of the same point.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="S"></typeparam>
public class PointTree<T, S>
    where T : INumber<T>
    where S : INumber<S>
{
    private List<T> vector;
    private S value;
    private int multiplicity;

    /// <summary>
    /// Constructs an empty point. Creates a point in 0 dimensional euclidean space.
    /// This constructor is provided only to make certain edge cases easier to handle.
    /// </summary>
    public PointTree()
    {
        multiplicity = 0;
    }

    /// <summary>
    /// Constructs an empty point. Creates a point with its position in euclidean space defined by vec, value defined by val, and a multiplicity/count of 1.
    /// </summary>
    /// <param name="val">The position in euclidean space</param>
    /// <param name="vec">The value associated with the point</param>
    public PointTree(List<T> vec, S val)
    {
        this.vector = vec;
        this.value = val;
        multiplicity = 1;
    }

    /// <summary>
    /// Constructs an empty point. Copies a point.
    /// </summary>
    /// <param name="val">The position in euclidean space</param>
    /// <param name="vec">The value associated with the point</param>
    public PointTree(PointTree<T, S> p)
    {
        value = p.value;
        vector = p.vector;
        multiplicity = p.Count();
    }

    /// <summary>
    /// Euclidean position of the point.
    /// </summary>
    /// <returns>The euclidean position of the point as a List.</returns>
    public List<T> AsVector()
    {
        return vector;
    }

    /// <summary>
    /// The point's ambient dimension.
    /// </summary>
    /// <returns>the dimension of the space in which the point lives. I.e. a point of the form (1,2,3) lives in dimension 3.</returns>
    public int Dim()
    {
        return vector.Count;
    }
    
    /// <summary>
    /// The point's count/multiplicity.
    /// </summary>
    /// <returns>The count/multiplicity</returns>
    public int Count()
    {
        return multiplicity;
    }

    /// <summary>
    /// Increase the point's count/multiplicity.
    /// </summary>
    /// <param name="n">Amount to increase by</param>
    /// <exception cref="Exception">If <paramref name="n"/> is negative</exception>
    public void IncreaseCountBy(int n)
    {
        if (n < 0)
        {
            throw new Exception("Can't increase by a negative amount");
        }
        multiplicity += n;
    }

    /// <summary>
    /// Increase the point's count/multiplicity by one.
    /// </summary>
    public void IncreaseCountByOne()
    {
        multiplicity += 1;
    }

    /// <summary>
    /// The point's value.
    /// </summary>
    /// <returns>The value stored in the point.</returns>
    public S Value()
    {
        return value;
    }


    public PointTree<T, S> DropLast()
    {
        PointTree<T, S> result = new(this);
        result.vector.RemoveAt(result.vector.Count - 1);
        return result;
    }

    /// <summary>
    /// Index a point. Get the ith coordinate value of the point. I.e. if a point is of the form (4, 5, 6), then its 0th coordinate value is 4 while its 2nd is 6.
    /// </summary>
    /// <param name="index">The coordinate to index</param>
    /// <returns>The coordinate value</returns>
    /// <exception cref="IndexOutOfRangeException">If <paramref name="index"/> is out of bounds</exception>
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= Dim())
            {
                throw new IndexOutOfRangeException("[] access index for point is out of range.");
            }
            return vector[index];
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="p"></param>
    /// <param name="m"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    public bool Within(PointTree<T, S> p, int m, double r)
    {
        
        for (int i = 0; i < m; i++)
        {
            if (p[i] < vector[i] - T.CreateChecked(r) || p[i] > vector[i] + T.CreateChecked(r))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Check for equality. 
    /// Two points are considered equal if they are in the same spot, have the same multiplicity/count, and store the same value.
    /// </summary>
    /// <param name="p">Some other point.</param>
    /// <returns><see cref="true"/>if <paramref name="p"/> equals the current point, otherwise <see cref="false"/>.</returns>
    public bool Equals(PointTree<T, S> p)
    {
        return vector.Equals(p.vector) && multiplicity == p.multiplicity && value.Equals(p.value);
    }

    /// <summary>
    /// Check for inequality.
    /// The opposite of <seealso cref="Equals(PointTree{T, S})"/>.
    /// </summary>
    /// <param name="p">Some other point.</param>
    /// <returns><see cref="false"/> if <paramref name="p"/> equals the current point, otherwise <see cref="true"/>.</returns>
    public bool NotEquals(PointTree<T, S> p)
    {
        return !Equals(p);
    }

    /// <summary>
    /// Prints the point to standard out.
    /// As an example, a point with euclidean location(3,4,5) and with a multiplicity/count of 4 will be printed as (3, 4, 5) : 4
    /// </summary>
    /// <param name="withCount">Whether or not to display the points count/multiplicity.</param>
    public void ToString(bool withCount = true)
    {
        if (vector.Count == 0)
        {
            Console.WriteLine("()");
        }
        else
        {
            Console.Write("(");
            for (int i = 0; i < vector.Count - 1; i++)
            {
                Console.Write(vector[i] + ", ");
            }
            if (withCount)
            {
                Console.WriteLine(vector[^1] + ") : " + multiplicity);
            }
            else
            {
                Console.WriteLine(vector[^1] + ") : ");
            }
        }
    }
}
