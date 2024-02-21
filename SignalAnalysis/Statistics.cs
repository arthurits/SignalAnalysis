namespace Statistics;

public static class Descriptive
{
    /// <summary>
    /// Computes the sum of all the elements in the data serie.
    /// </summary>
    /// <param name="values">Data values</param>
    /// <returns>The total sum of the elements. If length is equal to 0, then it returns NaN value</returns>
    /// <exception cref="ArgumentException">Throws exception if data length is equal to 0</exception>
    public static double Sum(double[] values)
    {
        //if (values.Length == 0)
        //    throw new ArgumentException($"{nameof(values)} cannot be empty");

        if (values.Length == 0)
            return double.NaN;

        double sum = 0;
        foreach (double value in values)
            sum += value;

        return sum;
    }

    /// <summary>
    /// Computes the sum of all the elements in the data serie.
    /// </summary>
    /// <param name="values">Data values</param>
    /// <returns>The total sum of the elements. If length is equal to 0, then it returns NaN value</returns>
    /// <exception cref="ArgumentException">Throws exception if data length is equal to 0</exception>
    public static double Sum<T>(IReadOnlyList<T> values)
    {
        //if (values.Length == 0)
        //    throw new ArgumentException($"{nameof(values)} cannot be empty");

        if (values.Count == 0)
            return double.NaN;

        double[] values2 = values.Select(value => Convert.ToDouble(value)).ToArray();

        return Sum(values2);
    }

    /// <summary>
    /// Computes the sum of all the elements in the data serie.
    /// </summary>
    /// <param name="values">Data values</param>
    /// <returns>The total sum of the elements. If length is equal to 0, then it returns NaN value</returns>
    /// <exception cref="ArgumentException">Throws exception if data length is equal to 0</exception>
    public static double SumParallel(double[] values)
    {
        if (values.Length == 0)
            return double.NaN;

        // Write our own custom parallel summation algorithm without depending on LINQ, which is would be values.AsParallel().Sum()
        // https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.parallel.for?view=net-8.0#system-threading-tasks-parallel-for-1(system-int32-system-int32-system-threading-tasks-paralleloptions-system-func((-0))-system-func((system-int32-system-threading-tasks-parallelloopstate-0-0))-system-action((-0)))
        // https://michaelscodingspot.com/array-iteration-vs-parallelism-in-c-net/
        //double sum = 0;
        //int processorCount = Environment.ProcessorCount;
        //int numLoops = values.Length / processorCount;
        //Parallel.For<double>(0, processorCount, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
        //    localInit: () => 0,     // Initialize the local states
        //    // Accumulate the thread-local computations in the loop body
        //    body: (i, loop, localTotal) =>
        //    {
        //        for (int j = i * numLoops; j < (i + 1) * numLoops; j++)
        //            localTotal += values[j];

        //        return localTotal;
        //    },
        //    // Combine all local states
        //    localFinally: (localTotal) => { sum += localTotal; });
        
        //for (int i = numLoops*processorCount; i<values.Length; i++)
        //    sum += values[i];

        return values.AsParallel().Sum();
    }

    /// <summary>
    /// Computes the sum of all the elements in the data serie. Uses paralellization.
    /// </summary>
    /// <param name="values">Data values</param>
    /// <returns>The total sum of the elements. If length is equal to 0, then it returns NaN value</returns>
    /// <exception cref="ArgumentException">Throws exception if data length is equal to 0</exception>
    public static double SumParallel<T>(IReadOnlyList<T> values)
    {
        if (values.Count == 0)
            return double.NaN;

        double[] values2 = values.AsParallel().Select(value => Convert.ToDouble(value)).ToArray();
        return values2.AsParallel().Sum();
    }

    /// <summary>
    /// Computes the mean (first moment about the mean) of the data serie.
    /// </summary>
    /// <param name="values">Data values</param>
    /// <returns>The sample mean value. If length is equal to 0, then it returns NaN value</returns>
    /// <exception cref="ArgumentException">Throws exception if data length is equal to 0</exception>
    public static double Mean(double[] values)
    {
        //if (values.Length == 0)
        //    throw new ArgumentException($"{nameof(values)} cannot be empty");
        
        //if (values.Length == 0)
        //    return double.NaN;

        return Sum(values) / values.Length;
    }

    /// <summary>
    /// Computes the mean (first moment about the mean) of the data serie.
    /// </summary>
    /// <param name="values">Data values</param>
    /// <returns>The sample mean value. If length is equal to 0, then it returns NaN value</returns>
    /// <exception cref="ArgumentException">Throws exception if data length is equal to 0</exception>
    public static double Mean<T>(IReadOnlyList<T> values)
    {
        //if (values.Count == 0)
        //    throw new ArgumentException($"{nameof(values)} cannot be empty");

        //if (values.Count == 0)
        //    return double.NaN;

        return Sum(values) / values.Count;
    }

    /// <summary>
    /// Computes the mean (first moment about the mean) of the data serie. Uses paralellization.
    /// </summary>
    /// <param name="values">Data values</param>
    /// <returns>The sample mean value. If length is equal to 0, then it returns NaN value</returns>
    /// <exception cref="ArgumentException">Throws exception if data length is equal to 0</exception>
    public static double MeanParallel(double[] values)
    {
        //if (values.Length == 0)
        //    throw new ArgumentException($"{nameof(values)} cannot be empty");

        //if (values.Length == 0)
        //    return double.NaN;

        return SumParallel(values) / values.Length;
    }

    /// <summary>
    /// Computes the mean (first moment about the mean) of the data serie. Uses paralellization.
    /// </summary>
    /// <param name="values">Data values</param>
    /// <returns>The sample mean value. If length is equal to 0, then it returns NaN value</returns>
    /// <exception cref="ArgumentException">Throws exception if data length is equal to 0</exception>
    public static double MeanParallel<T>(IReadOnlyList<T> values)
    {
        //if (values.Length == 0)
        //    throw new ArgumentException($"{nameof(values)} cannot be empty");

        //if (values.Count == 0)
        //    return double.NaN;

        return SumParallel(values) / values.Count;
    }

    /// <summary>
    /// Computes the variance (second moment about the mean) of the data series.
    /// </summary>
    /// <param name="values">Data values</param>
    /// <param name="asSample"><see langword="True"/> to compute the sample variance (N-1); otherwise (by default), it computes the population (N) variance</param>
    /// <returns>The variance value. A value equal to NaN indicates insufficient data points.</returns>
    /// <exception cref="ArgumentException">Throws exception if data length is equal to 0</exception>
    public static double Variance(double[] values, double? mean = null, bool asSample = false)
    {
        if (values.Length < 2)
            return double.NaN;
        //throw new ArgumentException($"{nameof(values)} must have at least 2 values");

        // Then compute the standard deviation
        //double avg = System.Linq.Enumerable.Average(doubles);
        //double sst = System.Linq.Enumerable.Sum(System.Linq.Enumerable.Select(doubles, x => (x - avg) * (x - avg)));    // Sum of squares total
        double avg = mean ?? Mean(values);
        double sst = SST(values, avg);   // Sum of squares total
        int denominator = values.Length - (asSample ? 1 : 0);
        return denominator > 0.0 ? sst / denominator : -1.0;
    }

    /// <summary>
    /// Computes the variance (second moment about the mean) of the data series.
    /// </summary>
    /// <param name="values">Data values</param>
    /// <param name="asSample"><see langword="True"/> to compute the sample variance (N-1); otherwise (by default), it computes the population (N) variance</param>
    /// <returns>The variance value. A value equal to NaN indicates insufficient data points.</returns>
    /// <exception cref="ArgumentException">Throws exception if data length is equal to 0</exception>
    public static double Variance<T>(IReadOnlyList<T> values, double? mean = null, bool asSample = false)
    {
        // Convert into an enumerable of doubles.
        double[] values2 = values.Select(value => Convert.ToDouble(value)).ToArray();

        return Variance(values2, mean, asSample);
    }

    /// <summary>
    /// Computes the variance (second moment about the mean) of the data series using paralellization.
    /// </summary>
    /// <param name="values">Data values</param>
    /// <param name="asSample"><see langword="True"/> to compute the sample variance (N-1); otherwise (by default), it computes the population (N) variance</param>
    /// <returns>The variance value. A value equal to NaN indicates insufficient data points.</returns>
    /// <exception cref="ArgumentException">Throws exception if data length is equal to 0</exception>
    public static double VarianceParallel(double[] values, double? mean = null, bool asSample = false)
    {
        if (values.Length < 2)
            return double.NaN;
        //throw new ArgumentException($"{nameof(values)} must have at least 2 values");

        // Then compute the standard deviation
        //double avg = System.Linq.Enumerable.Average(doubles);
        //double sst = System.Linq.Enumerable.Sum(System.Linq.Enumerable.Select(doubles, x => (x - avg) * (x - avg)));    // Sum of squares total
        double avg = mean ?? MeanParallel(values);
        double sst = SSTParallel(values, avg);   // Sum of squares total
        int denominator = values.Length - (asSample ? 1 : 0);
        return denominator > 0.0 ? sst / denominator : -1.0;
    }

    /// <summary>
    /// Computes the variance (second moment about the mean) of the data series using paralellization.
    /// </summary>
    /// <param name="values">Data values</param>
    /// <param name="asSample"><see langword="True"/> to compute the sample variance (N-1); otherwise (by default), it computes the population (N) variance</param>
    /// <returns>The variance value. A value equal to NaN indicates insufficient data points.</returns>
    /// <exception cref="ArgumentException">Throws exception if data length is equal to 0</exception>
    public static double VarianceParallel<T>(IReadOnlyList<T> values, double? mean = null, bool asSample = false)
    {
        // Convert into an enumerable of doubles.
        double[] values2 = values.AsParallel().Select(value => Convert.ToDouble(value)).ToArray();

        return VarianceParallel(values2, mean, asSample);
    }

    /// <summary>
    /// Computes the standard deviation (square root of the variance) of the data series.
    /// </summary>
    /// <param name="values">Data values</param>
    /// <param name="asSample"><see langword="True"/> to compute the sample standard deviation (N-1); otherwise (by default), it computes the population (N) deviation</param>
    /// <returns>Standard deviation. A value equal to NaN indicates insufficient data points.</returns>
    public static double StandardDeviation(double[] values, double? mean = null, bool asSample = false)
    {
        return Math.Sqrt(Variance(values, mean, asSample));
    }

    /// <summary>
    /// Computes the standard deviation (square root of the variance) of the data series.
    /// </summary>
    /// <param name="values">Data values</param>
    /// <param name="asSample"><see langword="True"/> to compute the sample standard deviation (N-1); otherwise (by default), it computes the population (N) deviation</param>
    /// <returns>Standard deviation. A value equal to NaN indicates insufficient data points.</returns>
    public static double StandardDeviation<T>(IReadOnlyList<T> values, double? mean = null, bool asSample = false)
    {
        return Math.Sqrt(Variance<T>(values, mean, asSample));
    }

    /// <summary>
    /// Computes the standard deviation (square root of the variance) of the data series using paralellization.
    /// </summary>
    /// <param name="values">Data values</param>
    /// <param name="asSample"><see langword="True"/> to compute the sample standard deviation (N-1); otherwise (by default), it computes the population (N) deviation</param>
    /// <returns>Standard deviation. A value equal to NaN indicates insufficient data points.</returns>
    public static double StandardDeviationParallel(double[] values, double? mean = null, bool asSample = false)
    {
        return Math.Sqrt(VarianceParallel(values, mean, asSample));
    }

    /// <summary>
    /// Computes the standard deviation (square root of the variance) of the data series using paralellization.
    /// </summary>
    /// <param name="values">Data values</param>
    /// <param name="asSample"><see langword="True"/> to compute the sample standard deviation (N-1); otherwise (by default), it computes the population (N) deviation</param>
    /// <returns>Standard deviation. A value equal to NaN indicates insufficient data points.</returns>
    public static double StandardDeviationParallel<T>(IReadOnlyList<T> values, double? mean = null, bool asSample = false)
    {
        return Math.Sqrt(VarianceParallel<T>(values, mean, asSample));
    }

    /// <summary>
    /// Computes the total sum of squares
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values"></param>
    /// <param name="mean"></param>
    /// <returns></returns>
    public static double SST(double[] values, double? mean = null)
    {
        double average = mean ?? Mean(values);

        double subtraction;
        double sum = 0;
        foreach (double value in values)
        {
            subtraction = value-average;
            sum += subtraction * subtraction;
        }

        return sum;
    }

    /// <summary>
    /// Computes the total sum of squares
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values"></param>
    /// <param name="mean"></param>
    /// <returns></returns>
    public static double SST<T>(IReadOnlyList<T> values, double? mean = null)
    {
        // Convert into an array of doubles.
        double[] values2 = values.Select(value => Convert.ToDouble(value)).ToArray();

        return SST(values2, mean);
    }

    /// <summary>
    /// Computes the total sum of squares
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values"></param>
    /// <param name="mean"></param>
    /// <returns></returns>
    public static double SSTParallel(double[] values, double? mean = null)
    {
        double average = mean ?? MeanParallel(values);

        return values.AsParallel().Sum(x => (x - average) * (x - average));
    }

    /// <summary>
    /// Computes the total sum of squares
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values"></param>
    /// <param name="mean"></param>
    /// <returns></returns>
    public static double SSTParallel<T>(IReadOnlyList<T> values, double? mean = null)
    {
        // Convert into an enumerable of doubles.
        double[] values2 = values.AsParallel().Select(value => Convert.ToDouble(value)).ToArray();

        return SSTParallel(values2);
    }
}
