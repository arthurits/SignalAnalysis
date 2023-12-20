namespace Statistics;

public static class Descriptive
{
    /// <summary>
    /// Return the sample sum.
    /// </summary>
    public static double Sum(double[] values)
    {
        if (!values.Any())
            throw new ArgumentException($"{nameof(values)} cannot be empty");

        double sum = 0;

        foreach (double value in values)
            sum += value;

        return sum;
    }

    /// <summary>
    /// Return the sample sum.
    /// </summary>
    public static double Sum<T>(IReadOnlyList<T> values)
    {
        double[] values2 = values.AsParallel().Select(value => Convert.ToDouble(value)).ToArray();
        return Sum(values2);
    }

    /// <summary>
    /// Return the sample mean.
    /// </summary>
    public static double Mean(double[] values)
    {
        if (!values.Any())
            throw new ArgumentException($"{nameof(values)} cannot be empty");

        return Sum(values) / values.Length;
    }

    /// <summary>
    /// Return the sample mean.
    /// </summary>
    public static double Mean<T>(IReadOnlyList<T> values)
    {
        double[] values2 = values.AsParallel().Select(value => Convert.ToDouble(value)).ToArray();
        return Mean(values2);
    }

    /// <summary>
    /// Computes the variance (second moment about the mean) of the data series.
    /// </summary>
    /// <param name="values">Data values</param>
    /// <param name="asSample"><see langword="True"/> to compute the sample standard deviation (N-1); otherwise (by default), it computes the population (N) deviation</param>
    /// <returns>Standard deviation. A value equal to -1 indicates insufficient data points.</returns>
    public static double Variance<T>(IReadOnlyList<T> values, bool asSample = false)
    {
        if (values.Count < 2)
            return -1;
            //throw new ArgumentException($"{nameof(values)} must have at least 2 values");

        // Then compute the standard deviation
        //double avg = System.Linq.Enumerable.Average(doubles);
        //double sst = System.Linq.Enumerable.Sum(System.Linq.Enumerable.Select(doubles, x => (x - avg) * (x - avg)));    // Sum of squares total
        double avg = Mean(values);
        double sst = SST(values, avg);   // Sum of squares total
        int denominator = values.Count - (asSample ? 1 : 0);
        return denominator > 0.0 ? Math.Sqrt(sst / denominator) : -1.0;
    }

    /// <summary>
    /// Computes the variance (second moment about the mean) of the data series.
    /// </summary>
    /// <param name="values">Data values</param>
    /// <param name="asSample"><see langword="True"/> to compute the sample standard deviation (N-1); otherwise (by default), it computes the population (N) deviation</param>
    /// <returns>Standard deviation. A value equal to -1 indicates insufficient data points.</returns>
    public static double VarianceParallel<T>(IReadOnlyList<T> values, bool asSample = false)
    {
        if (values.Count < 2)
            return -1;
        //throw new ArgumentException($"{nameof(values)} must have at least 2 values");

        // Convert into an enumerable of doubles.
        IEnumerable<double> doubles = values.AsParallel().Select(value => Convert.ToDouble(value));

        // Then compute the standard deviation
        //double avg = System.Linq.Enumerable.Average(doubles);
        //double sst = System.Linq.Enumerable.Sum(System.Linq.Enumerable.Select(doubles, x => (x - avg) * (x - avg)));    // Sum of squares total
        double avg = doubles.AsParallel().Average();
        double sst = doubles.AsParallel().Sum(x => (x - avg) * (x - avg));   // Sum of squares total
        int denominator = values.Count - (asSample ? 1 : 0);
        return denominator > 0.0 ? Math.Sqrt(sst / denominator) : -1.0;
    }

    /// <summary>
    /// Computes the total sum of squares
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values"></param>
    /// <param name="mean"></param>
    /// <returns></returns>
    public static double SST<T>(IReadOnlyList<T> values, double? mean)
    {
        // Convert into an enumerable of doubles.
        IEnumerable<double> doubles = values.AsParallel().Select(value => Convert.ToDouble(value));
        
        double average;
        if (mean.HasValue)
            average = mean.Value;
        else
            average = doubles.Average();

        return doubles.Sum(x => (x - average) * (x - average));
    }

    /// <summary>
    /// Computes the total sum of squares
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values"></param>
    /// <param name="mean"></param>
    /// <returns></returns>
    public static double SSTParallel<T>(IReadOnlyList<T> values, double? mean)
    {
        // Convert into an enumerable of doubles.
        IEnumerable<double> doubles = values.AsParallel().Select(value => Convert.ToDouble(value));

        double average;
        if (mean.HasValue)
            average = mean.Value;
        else
            average = doubles.AsParallel().Average();

        return doubles.AsParallel().Sum(x => (x - average) * (x - average));
    }
}
