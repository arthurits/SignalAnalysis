namespace SignalAnalysis;

/// <summary>
/// Routines to compute the entropy in signals (data-array points)
/// <seealso cref="https://www.codeproject.com/Articles/27030/Approximate-and-Sample-Entropies-Complexity-Metric"/>
/// <seealso cref="https://pdfs.semanticscholar.org/6841/efb4c40a74c1faa9f36e6d949d1ee330bfa9.pdf"/>
/// <seealso cref="https://en.wikipedia.org/wiki/Sample_entropy"/>
/// </summary>
public static class Complexity
{
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
        int AppEn_Cum, AppEn_Cum1;
        int SampEn_Cum = 0, SampEn_Cum1 = 0;
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

    ///// <summary>
    ///// Computes the Shannon entropy, the entropy bit, and the ideal entropy for a vector of numeric values
    ///// </summary>
    ///// <param name="data">Numeric values vector</param>
    ///// <returns>The Shannon entropy value, the entropy bit, and the ideal entropy</returns>
    //public static (double Entropy, double EntropyBit, double IdealEntropy) ShannonEntropy<T>(IEnumerable<T> data)
    //{
    //    double entropy = 0;
    //    double entropyBit;
    //    double entropyIdeal;
    //    double prob;

    //    // Convert into an enumerable of doubles.
    //    IEnumerable<double> values = data.Select(value => Convert.ToDouble(value));
    //    int nLength = values.Count();
    //    double nSum = values.Sum();
       
    //    // Compute the Shannon entropy
    //    foreach (double s in values)
    //    {
    //        if (s > 0)
    //        {
    //            prob = s / nSum;
    //            entropy -= prob * Math.Log2(prob);
    //        }
    //    }

    //    // https://github.com/wqyeo/Shannon-Entropy/blob/master/EntropyCal.cs
    //    entropyBit = Math.Ceiling(entropy) * nLength;

    //    // https://stackoverflow.com/questions/2979174/how-do-i-compute-the-approximate-entropy-of-a-bit-string
    //    prob = 1.0 / nLength;
    //    entropyIdeal = -1.0 * nLength * prob * Math.Log(prob);

    //    return (entropy, entropyBit, entropyIdeal);
    //}

    /// <summary>
    /// Computes the Shannon entropy, the entropy bit, and the ideal entropy for a vector of numeric values
    /// </summary>
    /// <param name="data">Numeric values vector</param>
    /// <returns>The Shannon entropy value, the entropy bit, and the ideal entropy</returns>
    public static (double Entropy, double EntropyBit, double IdealEntropy) ShannonEntropy<T>(IEnumerable<T> data, int decimalPrecision = 1)
    {
        double entropy = 0;
        double entropyBit;
        double entropyIdeal;
        double prob;
        int decimalFactor = (int)Math.Pow(10, decimalPrecision);

        // Convert into an enumerable of doubles.
        IEnumerable<double> values = data.Select(value => Convert.ToDouble(value));
        int nLength = values.Count();

        // Group data values
        // https://stackoverflow.com/questions/20765589/how-do-i-find-duplicates-in-an-array-and-display-how-many-times-they-occurred
        var dict = new Dictionary<double, int>();
        foreach (var value in values)
        {
            double val = value * decimalFactor;
            // When the key is not found, "count" will be initialized to 0
            dict.TryGetValue(val, out int count);
            dict[val] = count + 1;
        }
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
        entropyBit = Math.Ceiling(entropy) * nLength;

        // https://stackoverflow.com/questions/2979174/how-do-i-compute-the-approximate-entropy-of-a-bit-string
        prob = 1.0 / nLength;
        entropyIdeal = -1.0 * nLength * prob * Math.Log2(prob);

        return (entropy, entropyBit, entropyIdeal);
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
}

