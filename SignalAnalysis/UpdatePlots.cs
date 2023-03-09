

namespace SignalAnalysis;
partial class FrmMain
{
    private void PlotOriginal(double[] signal, string strLabel = "")
    {
        plotOriginal.Clear();
        //plotOriginal.Plot.Clear(typeof(ScottPlot.Plottable.SignalPlot));
        //plotOriginal.Plot.AddSignal(Data.Data[cboSeries.SelectedIndex], Data.SampleFrequency, label: cboSeries.SelectedItem.ToString());

        switch (_settings.AxisType)
        {
            case AxisType.Points:
                plotOriginal.Plot.AddSignal(signal, Signal.SampleFrequency / Signal.SampleFrequency, label: strLabel);
                plotOriginal.Plot.XAxis.DateTimeFormat(false);
                break;
            case AxisType.Seconds:
                plotOriginal.Plot.AddSignal(signal, Signal.SampleFrequency, label: strLabel);
                plotOriginal.Plot.XAxis.DateTimeFormat(false);
                break;
            case AxisType.DateTime:
                var sig = plotOriginal.Plot.AddSignal(signal, 24 * 60 * 60 * Signal.SampleFrequency, label: strLabel);
                sig.OffsetX = Signal.StartTime.ToOADate();
                plotOriginal.Plot.XAxis.DateTimeFormat(true);
                break;
        }

        plotOriginal.Plot.Title(StringResources.PlotOriginalTitle);
        plotOriginal.Plot.YLabel(StringResources.PlotOriginalYLabel);
        plotOriginal.Plot.XLabel(StringResources.PlotOriginalXLabel);
        //plotOriginal.Plot.AxisAuto(0);
        plotOriginal.Refresh();
    }

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
        plotWindow.Plot.YLabel(StringResources.PlotWindowYLabel);
        plotWindow.Plot.XLabel(StringResources.PlotWindowXLabel);
        plotWindow.Refresh();
    }

    private void PlotWindowedSignal(double[] signal)
    {
        plotApplied.Clear();
        //plotApplied.Plot.Clear(typeof(ScottPlot.Plottable.SignalPlot));
        plotApplied.Plot.AddSignal(signal, Signal.SampleFrequency);
        plotApplied.Plot.Title(StringResources.PlotAppliedTitle);
        plotApplied.Plot.YLabel(StringResources.PlotAppliedYLabel);
        plotApplied.Plot.XLabel(StringResources.PlotAppliedXLabel);
        //plotApplied.Plot.AxisAuto(0);
        plotApplied.Refresh();
    }

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
        plotFractal.Plot.Title((StringResources.PlotFractalTitle1) +
            " " +
            (progressive ? StringResources.PlotFractalTitle2 : String.Empty) +
            " (H = " + (double.IsNaN(FractalDimension.DimensionSingle) ? Results.FractalDimension : FractalDimension.DimensionSingle).ToString("0.00####", _settings.AppCulture) +
            " — Var(H) = " + (double.IsNaN(FractalDimension.VarianceH) ? Results.FractalVariance : FractalDimension.VarianceH).ToString("0.00####", _settings.AppCulture) + ")");
        plotFractal.Plot.YLabel(StringResources.PlotFractalYLabel);
        plotFractal.Plot.XLabel(StringResources.PlotFractalXLabel);
        plotFractal.Plot.AxisAuto(0);
        plotFractal.Refresh();
    }

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
        plotFractalDistribution.Plot.XLabel(StringResources.PlotFractalDistributionXLabel);
        plotFractalDistribution.Plot.YLabel(StringResources.PlotFractalDistributionYLabel);
        plotFractalDistribution.Plot.AxisAuto(0, null);
        plotFractalDistribution.Refresh();
    }

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
        plotFFT.Plot.YLabel(_settings.PowerSpectra ? StringResources.PlotFFTYLabelPow : StringResources.PlotFFTYLabelMag);
        plotFFT.Plot.XLabel(StringResources.PlotFFTXLabel);
        plotFFT.Plot.AxisAuto(0);
        plotFFT.Refresh();
    }

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
                plotDerivative.Plot.XAxis.DateTimeFormat(false);
                break;
            case AxisType.Seconds:
                pOriginal = plotDerivative.Plot.AddSignal(signal, Signal.SampleFrequency, color: Color.DarkGray, label: strLabel);
                pDerivative = plotDerivative.Plot.AddSignal(Results.Derivative, Signal.SampleFrequency, color: plotDerivative.Plot.Palette.Colors[0], label: StringResources.FileHeader28);
                plotDerivative.Plot.XAxis.DateTimeFormat(false);
                break;
            case AxisType.DateTime:
                pOriginal = plotDerivative.Plot.AddSignal(signal, 24 * 60 * 60 * Signal.SampleFrequency, color: Color.DarkGray, label: strLabel);
                pDerivative = plotDerivative.Plot.AddSignal(Results.Derivative, 24 * 60 * 60 * Signal.SampleFrequency, color: plotDerivative.Plot.Palette.Colors[0], label: StringResources.FileHeader28);
                pOriginal.OffsetX = Signal.StartTime.ToOADate();
                plotDerivative.Plot.XAxis.DateTimeFormat(true);
                break;
        }
        
        pOriginal.YAxisIndex = 1;
        pOriginal.XAxisIndex = 0;
        pDerivative.YAxisIndex = 0;
        pDerivative.XAxisIndex = 0;

        plotDerivative.Plot.YAxis2.Ticks(true);
        plotDerivative.Plot.YAxis2.Color(pOriginal.Color);
        plotDerivative.Plot.YAxis2.Label(StringResources.PlotDerivativeYLabel2);

        plotDerivative.Plot.Title(StringResources.PlotDerivativeTitle);
        plotDerivative.Plot.XAxis.Label(StringResources.PlotDerivativeXLabel);
        plotDerivative.Plot.YAxis.Color(pDerivative.Color);
        plotDerivative.Plot.YAxis.Label(StringResources.PlotDerivativeYLabel1);
        plotDerivative.Plot.AxisAuto(0, null);
        plotDerivative.Refresh();

    }

    /// <summary>
    /// Compute stats and update the plots and results
    /// This is the main computing function that calls sub-functions
    /// </summary>
    /// <param name="serie">The series to be computed</param>
    /// <returns></returns>
    private async Task UpdateStatsPlots(int series, bool stats = false, bool derivative = false, bool fractal = false, bool progressive = false, bool entropy = false, bool fft = false, bool powerSpectra = false)
    {
        // Clip signal data to the user-specified bounds 
        if (Signal.Data is null || Signal.Data.Length == 0) return;
        double[] signalClipped = Signal.Data[series][Signal.IndexStart..(Signal.IndexEnd + 1)];
        if (signalClipped is null || signalClipped.Length == 0) return;
        string? seriesName = stripComboSeries.SelectedItem is null ? stripComboSeries.Items[0].ToString() : stripComboSeries.SelectedItem.ToString();

        // Show waiting cursor
        var cursor = this.Cursor;
        Cursor.Current = Cursors.WaitCursor;
        this.UseWaitCursor = true;

        // Compute data;
        double[] signalWindowed = Array.Empty<double>();
        IWindow? window = (IWindow)stripComboWindows.SelectedItem;
        tokenSource?.Dispose();
        tokenSource = new();
        token = tokenSource.Token;
        Results = new();
        statsTask = Task.Run(() =>
        {
            try
            {
                // UpdateStats(signalClipped, _settings.CumulativeDimension, _settings.Entropy);
                if (stats) ComputeStatistics(signalClipped);
                if (derivative) ComputeDerivative(signalClipped);
                if (fractal) ComputeFractal(signalClipped, progressive);
                if (entropy) ComputeEntropy(signalClipped);
                if (fft) signalWindowed = ComputeFFT(signalClipped, window);
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
                            _settings.Entropy = false;
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
        // Update plots and results
        //    UpdateBasicPlots(signal, seriesName);
        _settings.CrossHair = false;
        statusStripLabelExCrossHair.Checked = false;
        if (stats) PlotOriginal(signalClipped, seriesName ?? string.Empty);
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
            PlotFFT(powerSpectra ? Results.FFTpower : Results.FFTmagnitude);
        }

        //await UpdateWindowPlots(signalClipped);

        // Show text results
        if (stats || fractal || entropy)
            txtStats.Text = Results.ToString(_settings.AppCulture);

        // Restore the cursor
        this.UseWaitCursor = false;
        Cursor.Current = cursor;
    }

    private void ComputeStatistics(double[] signal)
    {
        try
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

            Results.Maximum = max;
            Results.Minimum = min;
            Results.Average = avg;
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

    private void ComputeDerivative(double[] signal)
    {
        Function<int> func = new(DataFunction);

        Derivative<int> derivative = new(func, 1 / Signal.SampleFrequency, _settings.DerivativeAlgorithm);
        Results.Derivative = new double[signal.Length];

        for (int i = 0; i < signal.Length; i++)
        {
            Results.Derivative[i] = derivative[i];
            if (token.IsCancellationRequested)
                throw new OperationCanceledException("CancelDerivative", token);
        }

        // Local function
        double DataFunction(int index)
        {
            if (index < 0)
                return 0;

            if (index >= signal.Length)
                return 0;

            return signal[index];
        }
    }

    private void ComputeFractal(double[] signal, bool progressive = false)
    {
        // Compute fractal values
        FractalDimension.ComputeDimension(Signal.SampleFrequency, signal, token, progressive);
        Results.FractalDimension = FractalDimension.DimensionSingle;
        Results.FractalVariance = FractalDimension.VarianceH;
    }

    private void ComputeEntropy(double[] signal)
    {
        (Results.ApproximateEntropy, Results.SampleEntropy) = Complexity.Entropy(signal, token);
        (Results.ShannonEntropy, Results.EntropyBit, Results.IdealEntropy) = Complexity.ShannonEntropy(signal);
    }

    private double[] ComputeFFT(double[] signal, IWindow? window)
    {
        //IWindow window = (IWindow)stripComboWindows.SelectedItem;
        if (window is null) return Array.Empty<double>();

        double[] signalWindow = Array.Empty<double>();
        double[] signalFFT = Array.Empty<double>();

        // Round down to the next integer (Adjust to the lowest power of 2)
        int power2 = (int)Math.Floor(Math.Log2(signal.Length));
        //int evenPower = (power2 % 2 == 0) ? power2 : power2 - 1;

        // Apply window to signal
        signalWindow = new double[(int)Math.Pow(2, power2)];
        Array.Copy(signal, signalWindow, Math.Min(signalWindow.Length, signal.Length));
        window.ApplyInPlace(signalWindow);

        try
        {
            signalFFT = FftSharp.Transform.FFTpower(signalWindow);
            // Substitute -Infinity values (which will throw an exception when plotting) for a minimum value of -340
            signalFFT = signalFFT.Select(x => Double.IsInfinity(x) ? -340.0 : x).ToArray();
            Results.FFTpower = signalFFT;

            signalFFT = FftSharp.Transform.FFTmagnitude(signalWindow);
            Results.FFTmagnitude = signalFFT;

            signalFFT = _settings.PowerSpectra ? Results.FFTpower : Results.FFTmagnitude;
            Results.FFTfrequencies = FftSharp.Transform.FFTfreq(Signal.SampleFrequency, signalFFT.Length);
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


    // Below code is deprecated. ToDo: review and delete it

    /// <summary>
    /// Compute the signal stats
    /// </summary>
    /// <param name="signal">Signal data</param>
    /// <param name="progressive"><see langword>True</see> if the progressive fractal dimension is to be computed</param>
    /// <param name="entropy"><see langword="True"/> if all the entropy parameters are to be computed</param>
    /// <param name="derivative"><see langword="True"/> if the derivative is computed</param>
    private void UpdateStats(double[] signal, bool progressive = false, bool entropy = false, bool derivative = true)
    {
        if (signal.Length == 0) return;

        try
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

            Results.Maximum = max;
            Results.Minimum = min;
            Results.Average = avg;

            // Compute fractal values
            FractalDimension.ComputeDimension(Signal.SampleFrequency, signal, token, progressive);
            Results.FractalDimension = FractalDimension.DimensionSingle;
            Results.FractalVariance = FractalDimension.VarianceH;

            // Compute entropy values
            if (entropy)
            {
                (Results.ApproximateEntropy, Results.SampleEntropy) = Complexity.Entropy(signal, token);
                (Results.ShannonEntropy, Results.EntropyBit, Results.IdealEntropy) = Complexity.ShannonEntropy(signal);
            }

            if (derivative)
            {

            }
        }
        catch (OperationCanceledException)
        {
            // This is needed beacuse this exception is thrown while the cumulative fractal dimension is computed from another Task in "UpdateStatsPlots".
            Invoke(() =>
            {
                using (new CenterWinDialog(this))
                {
                    MessageBox.Show(this,
                          StringResources.MsgBoxTaskFractalCancel,
                          StringResources.MsgBoxTaskFractalCancelTitle,
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Stop);
                }
                _settings.CumulativeDimension = false;
                this.statusStripLabelExCumulative.Checked = false;
            }
            );
        }
        finally
        {
            tokenSource.Dispose();
        }

    }

    /// <summary>
    /// Updates the FFT related plots: FFT window, windowed signal, and signal FFT spectrum
    /// </summary>
    /// <param name="signal">Signal data</param>
    private async Task UpdateWindowPlots(double[] signal)
    {
        IWindow window = (IWindow)stripComboWindows.SelectedItem;
        if (window is null) return;

        double[] signalWindow = Array.Empty<double>();
        double[] signalFFT = Array.Empty<double>();
        //double[] freq = Array.Empty<double>();

        // Show a waiting cursor
        var cursor = this.Cursor;
        Cursor.Current = Cursors.WaitCursor;
        this.UseWaitCursor = true;

        // Run the intensive code on a separate task
        statsTask = Task.Run(() =>
        {
            // Round down to the next integer (Adjust to the lowest power of 2)
            int power2 = (int)Math.Floor(Math.Log2(signal.Length));
            //int evenPower = (power2 % 2 == 0) ? power2 : power2 - 1;

            // Apply window to signal
            signalWindow = new double[(int)Math.Pow(2, power2)];
            Array.Copy(signal, signalWindow, Math.Min(signalWindow.Length, signal.Length));
            window.ApplyInPlace(signalWindow);

            try
            {
                signalFFT = FftSharp.Transform.FFTpower(signalWindow);
                // Substitute -Infinity values (which will throw an exception when plotting) for a minimum value of -340
                signalFFT = signalFFT.Select(x => Double.IsInfinity(x) ? -340.0 : x).ToArray();
                Results.FFTpower = signalFFT;

                signalFFT = FftSharp.Transform.FFTmagnitude(signalWindow);
                Results.FFTmagnitude = signalFFT;

                signalFFT = _settings.PowerSpectra ? Results.FFTpower : Results.FFTmagnitude;
                Results.FFTfrequencies = FftSharp.Transform.FFTfreq(Signal.SampleFrequency, signalFFT.Length);
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
        });
        await statsTask;

        // Update plots
        _settings.CrossHair = false;
        statusStripLabelExCrossHair.Checked = false;
        plotOriginal.ShowCrossHair = false;
        plotOriginal.Refresh();
        plotFractal.ShowCrossHair = false;
        plotFractal.Refresh();
        PlotKernel(window, signal.Length);
        PlotWindowedSignal(signalWindow);
        PlotFFT(signalFFT);
        //UpdateFFT(signalFFT, Results.FFTfrequencies);

        // Restore the cursor
        this.UseWaitCursor = false;
        Cursor.Current = cursor;
    }


    private double[] NormalDistributionX(int pointCount = 100, double mean = .5, double stdDev = .5, double maxSdMultiple = 3)
    {
        double[] values = new double[pointCount];
        for (int i = 0; i < pointCount; i++)
            values[i] = (-maxSdMultiple + 2 * maxSdMultiple * (double)i / pointCount) * stdDev + mean;
        return values;
    }

    private double[] NormalDistributionY(int pointCount = 100, double maxSdMultiple = 3)
    {
        double[] values = new double[pointCount];
        for (int i = 0; i < pointCount; i++)
            values[i] = Phi(-maxSdMultiple + 2 * maxSdMultiple * (double)i / pointCount);
        return values;
    }

    private double[] RandomNormal(Random rand, int pointCount, double mean = .5, double stdDev = .5)
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
    public double RandomNormalValue(Random rand, double mean, double stdDev)
    {
        return Phi(mean + stdDev);
    }

    /// <summary>
    /// The function Φ(x) is the cumulative density function (CDF) of a standard normal (Gaussian) random variable. It is closely related to the error function erf(x).
    /// </summary>
    /// <param name="x">Normalized standard variable</param>
    /// <returns>The probability in the range [0, 1]</returns>
    /// <seealso cref="https://www.johndcook.com/blog/csharp_phi/"/>
    private double Phi(double x)
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

    public double SampleGaussian(Random random, double mean, double stdDev)
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

