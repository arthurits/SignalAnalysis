using System.Diagnostics;

namespace SignalAnalysis;

partial class FrmMain
{
    /// <summary>
    /// Compute stats and update the plots and results
    /// This is the main computing function that calls sub-functions
    /// </summary>
    /// <param name="series">Data series to be analised</param>
    /// <param name="deletePreviousResults"><see langword="True"/> if the previous results data should be deleted and new results should be reinitialised</param>
    /// <param name="stats"><see langword="True"/> if the descriptive statistics will be computed</param>
    /// <param name="boxplot"><see langword="True"/> if box plot is computed and shown</param>
    /// <param name="derivative"><see langword="True"/> if the derivative will be computed</param>
    /// <param name="integral"><see langword="True"/> if the integral will be computed</param>
    /// <param name="fractal"><see langword="True"/> if the fractal dimension will be computed</param>
    /// <param name="progressive"><see langword="True"/> if the progressive/cumulative fractal dimension is computed for each point</param>
    /// <param name="entropy"><see langword="True"/> if the entropy values will be computed</param>
    /// <param name="fft"><see langword="True"/> if the FFT will be computed</param>
    /// <param name="fftPlot"><see langword="True"/> if the FFT plot should update both the data points and the ordinate axis</param>
    /// <param name="powerSpectra"><see langword="True"/> if the power spectra is plotted, <see langword="false"/> if the amplitude is plotted instead</param>
    /// <param name="fftRoundUp"><see langword="True"/> if data length will be augmented by zero padding at the to make it equal to a power of 2. If <see langword="false"/>, it's rounded down to the closes power of 2</param>
    /// <returns></returns>
    private async Task ComputeAsync(int series, bool deletePreviousResults = false, bool stats = false, bool boxplot = false, bool derivative = false, bool integral = false, bool fractal = false, bool progressive = false, bool entropy = false, bool fft = false, bool fftPlot = false, bool powerSpectra = false, bool fftRoundUp = true)
    {
        // Clip signal data to the user-specified bounds 
        if (Signal.Data is null || Signal.Data.Length == 0) return;
        double[] signalClipped = Signal.Data[series][Signal.IndexStart..(Signal.IndexEnd + 1)];
        if (signalClipped is null || signalClipped.Length == 0) return;
        string? seriesName = stripComboSeries.SelectedItem is null ? stripComboSeries.Items[0]?.ToString() : stripComboSeries.SelectedItem.ToString();

        // Show waiting cursor
        var cursor = this.Cursor;
        Cursor = Cursors.WaitCursor;
        this.UseWaitCursor = true;

        // Compute data;
        double[] signalWindowed = [];
        IWindow? window = (IWindow?)stripComboWindows.SelectedItem;
        tokenSource?.Dispose();
        tokenSource = new();
        token = tokenSource.Token;

        if (deletePreviousResults)
            Results = new();

        statsTask = Task.Run(() =>
        {
            try
            {
                if (stats) ComputeStatistics(signalClipped);
                if (boxplot) ComputeBoxplot(signalClipped);
                if (derivative) ComputeDerivative(signalClipped);
                if (integral) ComputeIntegral(signalClipped);
                if (fractal) ComputeFractal(signalClipped, progressive);
                if (entropy) ComputeEntropy(signalClipped);
                if (fft) signalWindowed = ComputeFFT(signalClipped, fftRoundUp, window);
            }
            catch (OperationCanceledException ex)
            {
                // This is needed beacuse this exception is thrown while the cumulative fractal dimension is computed from another Task in "UpdateStatsPlots".
                Invoke(() =>
                {
                    string msg = string.Empty;
                    string title = string.Empty;
                    switch (ex.Message)
                    {
                        case "CancelDerivative":
                            msg = StringResources.MsgBoxTaskDerivativeCancel;
                            title = StringResources.MsgBoxTaskDerivativeCancelTitle;
                            _settings.ComputeDerivative = false;
                            this.statusStripLabelExDerivative.Checked = false;
                            break;
                        case "CancelFractal":
                            msg = StringResources.MsgBoxTaskFractalCancel;
                            title = StringResources.MsgBoxTaskFractalCancelTitle;
                            _settings.CumulativeDimension = false;
                            this.statusStripLabelExCumulative.Checked = false;
                            break;
                        case "CancelEntropy":
                            msg = StringResources.MsgBoxTaskEntropyCancel;
                            title = StringResources.MsgBoxTaskEntropyCancelTitle;
                            _settings.ComputeEntropy = false;
                            this.statusStripLabelExEntropy.Checked = false;
                            break;
                    }
                    using (new CenterWinDialog(this))
                    {
                        MessageBox.Show(this,
                              msg,
                              title,
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Stop);
                    }
                });
            }
            finally
            {
                tokenSource.Dispose();
            }
        }, token);
        await statsTask;

        // Show results on plots
        _settings.CrossHair = false;
        statusStripLabelExCrossHair.Checked = false;
        if (stats) PlotOriginal(signalClipped, seriesName ?? string.Empty);
        if (boxplot) PlotBoxplot(signalClipped, seriesName ?? string.Empty);
        if (derivative) PlotDerivative(signalClipped, seriesName ?? string.Empty);
        if (fractal)
        {
            PlotFractal(signalClipped, seriesName ?? string.Empty, _settings.CumulativeDimension);
            PlotFractalDistribution(Results.FractalDimension, Results.FractalVariance);
        }
        if (fft)
        {
            if (window is not null) PlotKernel(window, signalClipped.Length);
            if (signalWindowed.Length > 0) PlotWindowedSignal(signalWindowed);
        }
        if (fftPlot)
            PlotFFT(powerSpectra ? Results.FFTpower : Results.FFTmagnitude);

        // Show text results
        //if (stats || fractal || entropy || integral)
        txtStats.Text = Results.ToString(
            culture: _settings.AppCulture,
            boxplot: _settings.Boxplot,
            entropy: _settings.ComputeEntropy,
            entropyAlgorithm: _settings.ComputeEntropy ? StringResources.EntropyAlgorithms.Split(", ")[(int)_settings.EntropyAlgorithm] : string.Empty,
            entropyM: (int)_settings.EntropyFactorM,
            entropyR: _settings.EntropyFactorR,
            integral: _settings.ComputeIntegration,
            integralAlgorithm: _settings.ComputeIntegration ? StringResources.IntegrationAlgorithms.Split(", ")[(int)_settings.IntegrationAlgorithm] : string.Empty);

        // Restore the cursor
        this.UseWaitCursor = false;
        Cursor = cursor;
    }

    /// <summary>
    /// Computes the average, variance, maximum, and minimum values.
    /// </summary>
    /// <param name="signal">1D data array values</param>
    private void ComputeStatistics(double[] signal)
    {
        try
        {
            // Compute average, variance, maximum, and minimum descriptive statistics
            (Results.Average, Results.Variance, Results.Maximum, Results.Minimum) = DescriptiveSatatistics.ComputeAverage(signal);
        }
        catch (Exception ex)
        {
            Invoke(() =>
            {
                using (new CenterWinDialog(this))
                {
                    MessageBox.Show(this,
                        String.Format(_settings.AppCulture, StringResources.MsgBoxErrorDescriptiveStats, ex.Message),
                        StringResources.MsgBoxErrorDescriptiveStatsTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            });
        }
    }

    /// <summary>
    /// Computes boxplot-related values: quartiles Q1, Q2, and Q3, as well as minimum and maximum values excluding outliers
    /// </summary>
    /// <param name="signal">1D data array values</param>
    private void ComputeBoxplot(double[] signal)
    {
        try
        {
            // Compute Box plot values
            (Results.BoxplotMin, Results.BoxplotQ1, Results.BoxplotQ2, Results.BoxplotQ3, Results.BoxplotMax) = DescriptiveSatatistics.ComputeBoxPlotValues(signal, false);
        }
        catch (Exception ex)
        {
            Invoke(() =>
            {
                using (new CenterWinDialog(this))
                {
                    MessageBox.Show(this,
                        String.Format(_settings.AppCulture, StringResources.MsgBoxErrorDescriptiveStats, ex.Message),
                        StringResources.MsgBoxErrorDescriptiveStatsTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            });
        }
    }

    /// <summary>
    /// Computes the derivative of a 1D data array.
    /// </summary>
    /// <param name="signal">1D data array values whose values are expected to be uniformly spaced</param>
    /// <exception cref="OperationCanceledException">This is thrown if the token is cancelled whenever the user presses the ESC button</exception>
    private void ComputeDerivative(double[] signal)
    {
        Results.Derivative = Derivative.Derivate(
            array: signal,
            method: _settings.DerivativeAlgorithm,
            lowerIndex: 0,
            upperIndex: signal.Length - 1,
            samplingFrequency: Signal.SampleFrequency);

        // Replacing NaN values using LinQ since ScottPlot throws an exception for NaN values
        //var watch = new System.Diagnostics.Stopwatch();
        //watch.Start();
        Results.Derivative = Results.Derivative.Select(x => double.IsNaN(x) ? 0 : x).ToArray();
        //watch.Stop();
        //System.Diagnostics.Debug.WriteLine($"LinQ execution time: {watch.ElapsedMilliseconds} ms");

        //if (!watch.IsRunning)
        //    watch.Restart(); // Reset time to 0 and start measuring

        // Replacing NaN values using a for loop
        //double x;
        //for (int i = 0; i < Results.Derivative.Length; i++)
        //{
        //    x = Results.Derivative[i];
        //    Results.Derivative[i] = double.IsNaN(x) ? 0 : x;
        //}

        //watch.Stop();
        //System.Diagnostics.Debug.WriteLine($"For-loop execution time: {watch.ElapsedMilliseconds} ms");

        // This doesn't quite work
        //Array.ForEach(Results.Derivative, x => x = double.IsNaN(x) ? 0 : x);
    }

    /// <summary>
    /// Computes the integral value of a 1D data array.
    /// </summary>
    /// <param name="signal">1D data array, whose values are expected to be uniformly spaced</param>
    private void ComputeIntegral(double[] signal)
    {
        Results.Integral = Integration.Integrate(
            array: signal,
            method: _settings.IntegrationAlgorithm,
            lowerIndex: signal.GetLowerBound(0),
            upperIndex: signal.GetUpperBound(0),
            samplingFrequency: Signal.SampleFrequency,
            absoluteIntegral: _settings.AbsoluteIntegral,
            pad: _settings.PadIntegral);
    }

    /// <summary>
    /// Computes the fractal dimension of a 1D data array.
    /// </summary>
    /// <param name="signal">1D data array whose values are expected to be uniformly spaced</param>
    /// <param name="progressive"></param>
    private void ComputeFractal(double[] signal, bool progressive = false)
    {
        // Compute fractal values
        FractalDimension.ComputeDimension(Signal.SampleFrequency, signal, token, progressive);
        Results.FractalDimension = FractalDimension.DimensionSingle;
        Results.FractalVariance = FractalDimension.VarianceH;
    }

    /// <summary>
    /// Computes entropy values (Shannon, approximate, entropy bit, and ideal entropy).
    /// </summary>
    /// <param name="signal">1D data array whose values are expected to be uniformly spaced</param>
    private void ComputeEntropy(double[] signal)
    {
        (Results.ApproximateEntropy, Results.SampleEntropy) = Complexity.Entropy_Parallel(signal, token, _settings.EntropyFactorM, _settings.EntropyFactorR);
        (Results.ShannonEntropy, Results.EntropyBit, Results.IdealEntropy, Results.ShannonIdeal) = Complexity.ShannonEntropy(signal);


#if DEBUG

        Stopwatch stopwatch = new();
        
        stopwatch.Start();
        //(Results.ApproximateEntropy, Results.SampleEntropy) = Complexity.Entropy(signal, token);
        stopwatch.Stop();
        TimeSpan elapsed = stopwatch.Elapsed;
        Debug.WriteLine($"Elapsed time - Normal: {elapsed.Hours} hours, {elapsed.Minutes} minutes, {elapsed.Seconds} seconds, and {elapsed.Milliseconds} milliseconds");
        
        stopwatch.Restart();
        (Results.ApproximateEntropy, Results.SampleEntropy) = Complexity.Entropy(signal, token, _settings.EntropyFactorM, 0.15, 1);
        stopwatch.Stop();
        elapsed = stopwatch.Elapsed;
        Debug.WriteLine($"ApEn: {Results.ApproximateEntropy}, SampEn: {Results.SampleEntropy}");
        Debug.WriteLine($"Elapsed time: {elapsed.Hours} hours, {elapsed.Minutes} minutes, {elapsed.Seconds} seconds, and {elapsed.Milliseconds} milliseconds");

        stopwatch.Restart();
        //(Results.ApproximateEntropy, Results.SampleEntropy) = Complexity.Entropy_Parallel(signal, token, _settings.EntropyFactorM, _settings.EntropyFactorR);
        (Results.ApproximateEntropy, Results.SampleEntropy) = Complexity.Entropy_Parallel(signal, token, _settings.EntropyFactorM, 0.15, 1);
        stopwatch.Stop();
        elapsed = stopwatch.Elapsed;
        Debug.WriteLine($"ApEn: {Results.ApproximateEntropy}, SampEn: {Results.SampleEntropy}");
        Debug.WriteLine($"Elapsed time - Parallel For: {elapsed.Hours} hours, {elapsed.Minutes} minutes, {elapsed.Seconds} seconds, and {elapsed.Milliseconds} milliseconds");

        stopwatch.Restart();
        //(Results.ApproximateEntropy, Results.SampleEntropy) = Complexity.Entropy_SuperFast(signal, token, EntropyMethod.BruteForce, _settings.EntropyFactorM, _settings.EntropyFactorR);
        (Results.ApproximateEntropy, Results.SampleEntropy) = Complexity.Entropy_SuperFast(signal, token, EntropyMethod.BruteForce, _settings.EntropyFactorM, 0.15, 1);
        stopwatch.Stop();
        elapsed = stopwatch.Elapsed;
        Debug.WriteLine($"ApEn: {Results.ApproximateEntropy}, SampEn: {Results.SampleEntropy}");
        Debug.WriteLine($"Elapsed time - SuperFast ({EntropyMethod.BruteForce}): {elapsed.Hours} hours, {elapsed.Minutes} minutes, {elapsed.Seconds} seconds, and {elapsed.Milliseconds} milliseconds");

        stopwatch.Restart();
        //(Results.ApproximateEntropy, Results.SampleEntropy) = Complexity.Entropy_SuperFast(signal, token, EntropyMethod.BruteForce, _settings.EntropyFactorM, _settings.EntropyFactorR);
        (Results.ApproximateEntropy, Results.SampleEntropy) = Complexity.Entropy_SuperFast(signal, token, EntropyMethod.MonteCarloUniform, _settings.EntropyFactorM, 0.15, 1);
        stopwatch.Stop();
        elapsed = stopwatch.Elapsed;
        Debug.WriteLine($"ApEn: {Results.ApproximateEntropy}, SampEn: {Results.SampleEntropy}");
        Debug.WriteLine($"Elapsed time - SuperFast ({EntropyMethod.MonteCarloUniform}): {elapsed.Hours} hours, {elapsed.Minutes} minutes, {elapsed.Seconds} seconds, and {elapsed.Milliseconds} milliseconds");

        var apEnFast = Complexity.FastApEn_Cfun(signal, 2, 1, 0.15);
        apEnFast = Complexity.FastSampEn_Cfun(signal, 2, 1, 0.15);

        //stopwatch.Restart();
        //(Results.ApproximateEntropy, Results.SampleEntropy) = Complexity.Entropy_SuperFast(signal, token, EntropyMethod.MonteCarloUniform, _settings.EntropyFactorM, _settings.EntropyFactorR);
        //stopwatch.Stop();
        //elapsed = stopwatch.Elapsed;
        //Debug.WriteLine($"ApEn: {Results.ApproximateEntropy}, SampEn: {Results.SampleEntropy}");
        //Debug.WriteLine($"Elapsed time - SuperFast ({EntropyMethod.MonteCarloUniform}): {elapsed.Hours} hours, {elapsed.Minutes} minutes, {elapsed.Seconds} seconds, and {elapsed.Milliseconds} milliseconds");
#endif

    }

    /// <summary>
    /// Computes the FFT: both the power and the magnitude data along with the frequency scale
    /// </summary>
    /// <param name="signal">1D data array whose values are expected to be uniformly spaced</param>
    /// <param name="roundUp"><see langword="True"/> if <paramref name="signal"/> length will be augmented by zero padding at the to make it equal to a power of 2. If <see langword="false"/>, it's rounded down to the closes power of 2</param>
    /// <param name="window">Window function</param>
    /// <returns>The windowed signal</returns>
    private double[] ComputeFFT(double[] signal, bool roundUp = true, IWindow? window = null)
    {
        //IWindow window = (IWindow)stripComboWindows.SelectedItem;
        //if (window is null) return Array.Empty<double>();

        double[] signalWindow = [];
        System.Numerics.Complex[] spectrum = [];

        // First, round down to the next integer (adjust to the lowest power of 2)
        int power2;
        if (roundUp)
            power2 = (int)Math.Ceiling(Math.Log2(signal.Length));
        else
            power2 = (int)Math.Floor(Math.Log2(signal.Length));
        //int evenPower = (power2 % 2 == 0) ? power2 : power2 - 1;

        // Apply window to signal
        signalWindow = new double[(int)Math.Pow(2, power2)];
        Array.Copy(signal, signalWindow, Math.Min(signalWindow.Length, signal.Length));
        window?.ApplyInPlace(signalWindow);

        try
        {
            spectrum = FftSharp.FFT.Forward(signalWindow);
            Results.FFTpower = FftSharp.FFT.Power(spectrum);
            Results.FFTpower = Results.FFTpower.Select(x => double.IsInfinity(x) ? -1000 : x).ToArray();
            Results.FFTmagnitude = FftSharp.FFT.Magnitude(spectrum);
            Results.FFTfrequencies = FftSharp.FFT.FrequencyScale(Results.FFTpower.Length, Signal.SampleFrequency);
        }
        catch (Exception ex)
        {
            Invoke(() =>
            {
                using (new CenterWinDialog(this))
                {
                    MessageBox.Show(this,
                        String.Format(_settings.AppCulture, StringResources.MsgBoxErrorFFT, ex.Message),
                        StringResources.MsgBoxErrorFFTTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            });
        }

        return signalWindow;
    }

    /// <summary>
    /// Plots the original data into <see cref="plotOriginal"/>.
    /// </summary>
    /// <param name="signal">Data values to be plotted</param>
    /// <param name="strLabel">Text to show in the legend</param>
    private void PlotOriginal(double[] signal, string strLabel = "")
    {
        plotOriginal.Clear();
        //plotOriginal.Plot.Clear(typeof(ScottPlot.Plottable.SignalPlot));
        //plotOriginal.Plot.AddSignal(Data.Data[cboSeries.SelectedIndex], Data.SampleFrequency, label: cboSeries.SelectedItem.ToString());

        switch (_settings.AxisType)
        {
            case AxisType.Points:
                plotOriginal.Plot.AddSignal(signal, Signal.SampleFrequency / Signal.SampleFrequency, label: strLabel);
                plotOriginal.Plot.BottomAxis.DateTimeFormat(false);
                break;
            case AxisType.Seconds:
                plotOriginal.Plot.AddSignal(signal, Signal.SampleFrequency, label: strLabel);
                plotOriginal.Plot.BottomAxis.DateTimeFormat(false);
                break;
            case AxisType.DateTime:
                var sig = plotOriginal.Plot.AddSignal(signal, 24 * 60 * 60 * Signal.SampleFrequency, label: strLabel);
                sig.OffsetX = Signal.StartTime.ToOADate();
                plotOriginal.Plot.BottomAxis.DateTimeFormat(true);
                break;
        }

        plotOriginal.Plot.Title(StringResources.PlotOriginalTitle);
        plotOriginal.Plot.LeftAxis.Label(StringResources.PlotOriginalYLabel);
        plotOriginal.Plot.BottomAxis.Label(StringResources.PlotOriginalXLabel);
        //plotOriginal.Plot.AxisAuto(0);
        plotOriginal.Refresh();
    }

    /// <summary>
    /// Plots the original data as a population BoxPlot <see cref="plotBoxPlot"/>.
    /// </summary>
    /// <param name="signal">Data values to be plotted</param>
    /// <param name="strLabel">Text to show in the legend</param>
    private void PlotBoxplot(double[] signal, string strLabel = "")
    {
        foreach (var plot in plotBoxPlot.Plot.GetPlottables())
            plotBoxPlot.Plot.Remove(plot);

        plotBoxPlot.Plot.AddPopulation(new ScottPlot.Statistics.Population(signal), strLabel);

        // Format plot
        plotBoxPlot.Plot.Title(StringResources.PlotBoxPlotTitle);
        plotBoxPlot.Plot.LeftAxis.Label(StringResources.PlotBoxplotYLabel);
        plotBoxPlot.Plot.XTicks(new string[] { strLabel });
        plotBoxPlot.Plot.XAxis.Grid(false);
        plotBoxPlot.Refresh();
    }

    /// <summary>
    /// Plots the windowing function into <see cref="plotWindow"/> that has been applied to the signal data before the FFT transform.
    /// </summary>
    /// <param name="window">Window function</param>
    /// <param name="points">Number of points to be plotted</param>
    private void PlotKernel(FftSharp.IWindow window, int points)
    {
        double[] kernel = window.Create(points);
        //double[] pad = ScottPlot.DataGen.Zeros(kernel.Length / 4);
        //double[] ys = pad.Concat(kernel).Concat(pad).ToArray();

        plotWindow.Clear();
        //plotWindow.Plot.Clear(typeof(ScottPlot.Plottable.SignalPlot));
        var plot = plotWindow.Plot.AddSignal(kernel, Signal.SampleFrequency, Color.Crimson);
        plot.LineWidth = 1.0;
        plot.MarkerSize = 5;
        plot.MarkerShape = ScottPlot.MarkerShape.filledCircle;
        //plotWindow.Plot.AxisAuto(0);
        plotWindow.Plot.Title(String.Format(StringResources.PlotWindowTitle, window.Name));
        plotWindow.Plot.LeftAxis.Label(StringResources.PlotWindowYLabel);
        plotWindow.Plot.BottomAxis.Label(StringResources.PlotWindowXLabel);
        plotWindow.Refresh();
    }

    /// <summary>
    /// Plots the windowed signal values into <see cref="plotApplied"/>
    /// </summary>
    /// <param name="signal">Data values to be plotted</param>
    private void PlotWindowedSignal(double[] signal)
    {
        plotApplied.Clear();
        //plotApplied.Plot.Clear(typeof(ScottPlot.Plottable.SignalPlot));
        plotApplied.Plot.AddSignal(signal, Signal.SampleFrequency);
        plotApplied.Plot.Title(StringResources.PlotAppliedTitle);
        plotApplied.Plot.LeftAxis.Label(StringResources.PlotAppliedYLabel);
        plotApplied.Plot.BottomAxis.Label(StringResources.PlotAppliedXLabel);
        //plotApplied.Plot.AxisAuto(0);
        plotApplied.Refresh();
    }

    /// <summary>
    /// Plots the fractal value into <see cref="plotFractal"/>.
    /// </summary>
    /// <param name="signal">Data values to be plotted</param>
    /// <param name="seriesName">Text to show in the legend</param>
    /// <param name="progressive"><see langword="True"/> if signal contains the cumulativive fractal values</param>
    private void PlotFractal(double[] signal, string seriesName = "", bool progressive = false)
    {
        if (Signal.Data.Length == 0) return;

        plotFractal.Clear();
        if (progressive && FractalDimension.DimensionCumulative.Length > 0)
        {
            plotFractal.Plot.AddSignal(FractalDimension.DimensionCumulative, Signal.SampleFrequency, label: seriesName);
        }
        else
        {
            plotFractal.Plot.AddLine(0, double.IsNaN(FractalDimension.DimensionSingle) ? Results.FractalDimension : FractalDimension.DimensionSingle, (0, signal.Length / Signal.SampleFrequency));
        }
        plotFractal.Plot.Title($"{StringResources.PlotFractalTitle1}" +
            $"{(progressive ? $" {StringResources.PlotFractalTitle2}" : String.Empty)}" +
            $" (H = {Results.FractalDimension.ToString("0.00####", _settings.AppCulture)}" +
            $" — Var(H) = {Results.FractalVariance.ToString("0.00####", _settings.AppCulture)})");
        plotFractal.Plot.LeftAxis.Label(StringResources.PlotFractalYLabel);
        plotFractal.Plot.BottomAxis.Label(StringResources.PlotFractalXLabel);
        plotFractal.Plot.AxisAuto(0);
        plotFractal.Refresh();
    }

    /// <summary>
    /// Plots the fractal distribution data into <see cref="plotFractalDistribution"/> and draws a vertical line at the mean point.
    /// </summary>
    /// <param name="mean">Mean value</param>
    /// <param name="variance">Variance</param>
    private void PlotFractalDistribution(double mean, double variance)
    {
        plotFractalDistribution.Clear();

        if (variance > 0)
        {
            Random rand = new(0);
            double std = Math.Sqrt(variance);
            //double[] values = ScottPlot.DataGen.RandomNormal(rand, pointCount: 1000, mean: mean, stdDev: std);
            //for (int i = 0; i < values.Length; i++)
            //    values[i] = SampleGaussian(rand, mean, std);

            // create a Population object from the data
            //var pop = new ScottPlot.Statistics.Population(values);
            var pop = new ScottPlot.Statistics.Population(rand, pointCount: 1000, mean: mean, stdDev: std);

            //(double[] counts, double[] binEdges) = ScottPlot.Statistics.Common.Histogram(values, min: mean - 3 * std, max: mean + 3 * std, binSize: pop.span/100);
            //double[] curveXs = binEdges;
            //double[] curveYs = ScottPlot.Statistics.Common.ProbabilityDensity(values, curveXs, percent: true);

            double[] curveXs = ScottPlot.DataGen.Range(pop.minus3stDev, pop.plus3stDev, pop.span / 100);
            double[] curveYs = pop.GetDistribution(curveXs, normalize: true);

            plotFractalDistribution.Plot.AddScatter(curveXs, curveYs, color: Color.Crimson, markerSize: 0, lineWidth: 2);
            plotFractalDistribution.Plot.AddVerticalLine(x: mean, color: Color.DarkGray, width: 1.2f, style: ScottPlot.LineStyle.Solid);
        }

        plotFractalDistribution.Plot.Title(StringResources.PlotFractalDistributionTitle);
        plotFractalDistribution.Plot.BottomAxis.Label(StringResources.PlotFractalDistributionXLabel);
        plotFractalDistribution.Plot.LeftAxis.Label(StringResources.PlotFractalDistributionYLabel);
        plotFractalDistribution.Plot.AxisAuto(0, null);
        plotFractalDistribution.Refresh();
    }

    /// <summary>
    /// Plots the FFT data into <see cref="plotFFT"/>.
    /// </summary>
    /// <param name="signal">Data values to be plotted</param>
    /// <param name="frequency">Sampling frequency of the signal data</param>
    private void PlotFFT(double[] signal, double[]? frequency = null)
    {
        // Plot the results
        plotFFT.Clear();
        if (signal.Length > 0)
        {
            if (frequency is not null && frequency.Length > 0)
                plotFFT.Plot.AddScatter(frequency, signal);
            else
                plotFFT.Plot.AddSignal(signal, 2 * (double)(signal.Length - 1) / Signal.SampleFrequency);
        }
        plotFFT.Plot.Title(StringResources.PlotFFTTitle);
        plotFFT.Plot.LeftAxis.Label(_settings.PowerSpectra ? StringResources.PlotFFTYLabelPow : StringResources.PlotFFTYLabelMag);
        plotFFT.Plot.BottomAxis.Label(StringResources.PlotFFTXLabel);
        plotFFT.Plot.AxisAuto(0);
        plotFFT.Refresh();
    }

    /// <summary>
    /// Plots the derivative data into <see cref="plotDerivative"/>.
    /// </summary>
    /// <param name="signal">Data values to be plotted</param>
    /// <param name="strLabel">Text to show in the legend</param>
    private void PlotDerivative(double[] signal, string strLabel = "")
    {
        plotDerivative.Clear();

        // Plot data
        ScottPlot.Plottable.SignalPlot pOriginal = new();
        ScottPlot.Plottable.SignalPlot pDerivative = new();

        switch (_settings.AxisType)
        {
            case AxisType.Points:
                pOriginal = plotDerivative.Plot.AddSignal(signal, Signal.SampleFrequency / Signal.SampleFrequency, color: Color.DarkGray, label: strLabel);
                pDerivative = plotDerivative.Plot.AddSignal(Results.Derivative, Signal.SampleFrequency / Signal.SampleFrequency, color: plotDerivative.Plot.Palette.Colors[0], label: StringResources.FileHeader28);
                plotDerivative.Plot.BottomAxis.DateTimeFormat(false);
                break;
            case AxisType.Seconds:
                pOriginal = plotDerivative.Plot.AddSignal(signal, Signal.SampleFrequency, color: Color.DarkGray, label: strLabel);
                pDerivative = plotDerivative.Plot.AddSignal(Results.Derivative, Signal.SampleFrequency, color: plotDerivative.Plot.Palette.Colors[0], label: StringResources.FileHeader28);
                plotDerivative.Plot.BottomAxis.DateTimeFormat(false);
                break;
            case AxisType.DateTime:
                pOriginal = plotDerivative.Plot.AddSignal(signal, 24 * 60 * 60 * Signal.SampleFrequency, color: Color.DarkGray, label: strLabel);
                pDerivative = plotDerivative.Plot.AddSignal(Results.Derivative, 24 * 60 * 60 * Signal.SampleFrequency, color: plotDerivative.Plot.Palette.Colors[0], label: StringResources.FileHeader28);
                pOriginal.OffsetX = Signal.StartTime.ToOADate();
                plotDerivative.Plot.BottomAxis.DateTimeFormat(true);
                break;
        }

        pOriginal.YAxisIndex = plotDerivative.Plot.RightAxis.AxisIndex;
        pOriginal.XAxisIndex = plotDerivative.Plot.BottomAxis.AxisIndex;
        pDerivative.YAxisIndex = plotDerivative.Plot.LeftAxis.AxisIndex;
        pDerivative.XAxisIndex = plotDerivative.Plot.BottomAxis.AxisIndex;

        plotDerivative.Plot.RightAxis.Ticks(true);
        plotDerivative.Plot.RightAxis.Color(pOriginal.Color);
        plotDerivative.Plot.RightAxis.Label(StringResources.PlotDerivativeYLabel2);

        plotDerivative.Plot.Title(StringResources.PlotDerivativeTitle);
        plotDerivative.Plot.BottomAxis.Label(StringResources.PlotDerivativeXLabel);
        plotDerivative.Plot.LeftAxis.Color(pDerivative.Color);
        plotDerivative.Plot.LeftAxis.Label(StringResources.PlotDerivativeYLabel1);
        //plotDerivative.Plot.AxisAuto(0, null);
        plotDerivative.Refresh();

    }
}