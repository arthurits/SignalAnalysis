namespace SignalAnalysis;
partial class FrmMain
{
    private void UpdateOriginal(double[] signal, string strLabel = "")
    {
        plotOriginal.Clear();
        //plotOriginal.Plot.Clear(typeof(ScottPlot.Plottable.SignalPlot));
        //plotOriginal.Plot.AddSignal(_signalData[cboSeries.SelectedIndex], nSampleFreq, label: cboSeries.SelectedItem.ToString());

        switch (_settings.AxisType)
        {
            case AxisType.Points:
                plotOriginal.Plot.AddSignal(signal, nSampleFreq / nSampleFreq, label: strLabel);
                plotOriginal.Plot.XAxis.DateTimeFormat(false);
                break;
            case AxisType.Seconds:
                plotOriginal.Plot.AddSignal(signal, nSampleFreq, label: strLabel);
                plotOriginal.Plot.XAxis.DateTimeFormat(false);
                break;
            case AxisType.DateTime:
                var sig = plotOriginal.Plot.AddSignal(signal, 24 * 60 * 60 * nSampleFreq, label: strLabel);
                sig.OffsetX = nStart.ToOADate();
                plotOriginal.Plot.XAxis.DateTimeFormat(true);
                break;
        }

        plotOriginal.Plot.Title(StringsRM.GetString("strPlotOriginalTitle", _settings.AppCulture) ?? "Input signal");
        plotOriginal.Plot.YLabel(StringsRM.GetString("strPlotOriginalYLabel", _settings.AppCulture) ?? "Amplitude");
        plotOriginal.Plot.XLabel(StringsRM.GetString("strPlotOriginalXLabel", _settings.AppCulture) ?? "Time (seconds)");
        plotOriginal.Plot.AxisAuto(0);
        plotOriginal.Refresh();
    }

    private void UpdateKernel(FftSharp.IWindow window, int points)
    {
        double[] kernel = window.Create(points);
        double[] pad = ScottPlot.DataGen.Zeros(kernel.Length / 4);
        double[] ys = pad.Concat(kernel).Concat(pad).ToArray();

        plotWindow.Clear();
        //plotWindow.Plot.Clear(typeof(ScottPlot.Plottable.SignalPlot));
        plotWindow.Plot.AddSignal(ys, nSampleFreq, Color.Red);
        plotWindow.Plot.AxisAuto(0);
        plotWindow.Plot.Title(String.Format(StringsRM.GetString("strPlotWindowTitle", _settings.AppCulture) ?? "{0} window", window.Name));
        plotWindow.Plot.YLabel(StringsRM.GetString("strPlotWindowYLabel", _settings.AppCulture) ?? "Amplitude");
        plotWindow.Plot.XLabel(StringsRM.GetString("strPlotWindowXLabel", _settings.AppCulture) ?? "Time (seconds)");
        plotWindow.Refresh();
    }

    private void UpdateWindowed(double[] signal)
    {
        plotApplied.Clear();
        //plotApplied.Plot.Clear(typeof(ScottPlot.Plottable.SignalPlot));
        plotApplied.Plot.AddSignal(signal, nSampleFreq);
        plotApplied.Plot.Title(StringsRM.GetString("strPlotAppliedTitle", _settings.AppCulture) ?? "Windowed signal");
        plotApplied.Plot.YLabel(StringsRM.GetString("strPlotAppliedYLabel", _settings.AppCulture) ?? "Amplitude");
        plotApplied.Plot.XLabel(StringsRM.GetString("strPlotAppliedXLabel", _settings.AppCulture) ?? "Time (seconds)");
        plotApplied.Plot.AxisAuto(0);
        plotApplied.Refresh();
    }

    private void UpdateFractal(double[] signal, string seriesName = "", bool progressive = false)
    {
        if (_signalData.Length == 0) return;

        plotFractal.Clear();
        if (progressive && FractalDimension.DimensionCumulative.Length > 0)
        {
            plotFractal.Plot.AddSignal(FractalDimension.DimensionCumulative, nSampleFreq, label: seriesName);
        }
        else
        {
            plotFractal.Plot.AddLine(0, double.IsNaN(FractalDimension.DimensionSingle) ? Results.FractalDimension : FractalDimension.DimensionSingle, (0, signal.Length / nSampleFreq));
        }
        plotFractal.Plot.Title((StringsRM.GetString("strPlotFractalTitle", _settings.AppCulture) ?? "Fractal dimension") +
            " " +
            (progressive ? (StringsRM.GetString("strPlotFractalTitle()", _settings.AppCulture) ?? "(cumulative)") : String.Empty) +
            " (H = " + (double.IsNaN(FractalDimension.DimensionSingle) ? Results.FractalDimension : FractalDimension.DimensionSingle).ToString("0.00####") +
            " — Var(H) = " + (double.IsNaN(FractalDimension.VarianceH) ? Results.FractalVariance : FractalDimension.VarianceH).ToString("0.00####") + ")");
        plotFractal.Plot.YLabel(StringsRM.GetString("strPlotFractalYLabel", _settings.AppCulture) ?? "Dimension (H)");
        plotFractal.Plot.XLabel(StringsRM.GetString("strPlotFractalXLabel", _settings.AppCulture) ?? "Time (seconds)");
        plotFractal.Plot.AxisAuto(0);
        plotFractal.Refresh();
    }

    private void UpdateFFT(double[] signal)
    {
        double[] ys = Array.Empty<double>();

        try
        {
            ys = _settings.PowerSpectra ? FftSharp.Transform.FFTpower(signal) : FftSharp.Transform.FFTmagnitude(signal);
        }
        catch (Exception ex)
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show(String.Format(StringsRM.GetString("strMsgBoxErrorFFT", _settings.AppCulture) ?? "Unexpected error while computing the FFT." + Environment.NewLine + "{0}", ex.Message),
                    StringsRM.GetString("strMsgBoxErrorFFTTitle", _settings.AppCulture) ?? "FFT error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // Plot the results
        plotFFT.Clear();
        if (ys.Length > 0)
            plotFFT.Plot.AddSignal(ys, (double)ys.Length / nSampleFreq);
        plotFFT.Plot.Title(StringsRM.GetString("strPlotFFTTitle", _settings.AppCulture) ?? "Fast Fourier transform");
        plotFFT.Plot.YLabel(_settings.PowerSpectra ? (StringsRM.GetString("strPlotFFTYLabelPow", _settings.AppCulture) ?? "Power (dB)") : (StringsRM.GetString("strPlotFFTXLabelMag", _settings.AppCulture) ?? "Magnitude (RMS²)"));
        plotFFT.Plot.XLabel(StringsRM.GetString("strPlotFFTXLabel", _settings.AppCulture) ?? "Frequency (Hz)");
        plotFFT.Plot.AxisAuto(0);
        plotFFT.Refresh();
    }

    /// <summary>
    /// Compute stats and update the plots and results
    /// This is the main computing function that calls sub-functions
    /// </summary>
    /// <param name="serie">The series to be computed</param>
    /// <returns></returns>
    private async Task UpdateStatsPlots(int series)
    {
        // Extract the values 
        var signal = _signalData[series][_settings.IndexStart..(_settings.IndexEnd + 1)];
        if (signal is null || signal.Length == 0) return;

        string? seriesName = stripComboSeries.SelectedItem is null ? stripComboSeries.Items[0].ToString() : stripComboSeries.SelectedItem.ToString();

        // Show a waiting cursor
        var cursor = this.Cursor;
        Cursor.Current = Cursors.WaitCursor;
        Application.DoEvents();

        // ComputeStats(signal);
        tokenSource = new();
        token = tokenSource.Token;
        Results = new();
        statsTask = Task.Run(() =>
        {
            UpdateStats(signal, _settings.CumulativeDimension, _settings.Entropy);
        }, token);
        await statsTask;

        // Update plots and results
        //    UpdateBasicPlots(signal, seriesName);
        UpdateOriginal(signal, seriesName ?? string.Empty);
        UpdateFractal(signal, seriesName ?? string.Empty, _settings.CumulativeDimension);
        UpdateWindowPlots(signal);

        txtStats.Text = Results.ToString(StringsRM, _settings.AppCulture);

        // Restore the cursor
        Cursor.Current = cursor;
    }

    /// <summary>
    /// Compute the signal stats
    /// </summary>
    /// <param name="signal">Signal data</param>
    /// <param name="progressive"><see langword>True</see> if the progressive fractal dimension is to be computed</param>
    /// <param name="entropy"><see langword="True"/> if all the entropy parameters are to be computed</param>
    private void UpdateStats(double[] signal, bool progressive = false, bool entropy = false)
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
            FractalDimension.ComputeDimension(nSampleFreq, signal, token, progressive);
            Results.FractalDimension = FractalDimension.DimensionSingle;
            Results.FractalVariance = FractalDimension.VarianceH;

            // Compute entropy values
            if (entropy)
            {
                (Results.ApproximateEntropy, Results.SampleEntropy) = Complexity.Entropy(signal, token);
                (Results.ShannonEntropy, Results.EntropyBit, Results.IdealEntropy) = Complexity.ShannonEntropy(signal);
            }
        }
        catch (OperationCanceledException)
        {
            using (new CenterWinDialog(this))
                MessageBox.Show(this,
                    StringsRM.GetString("strMsgBoxTaskCancel", _settings.AppCulture),
                    StringsRM.GetString("strMsgBoxTaskCancelTitle", _settings.AppCulture),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Stop);
        }
        finally
        {
            tokenSource.Dispose();
        }

    }

    /// <summary>
    /// Updates the original data plot and the fractal dimension plot
    /// </summary>
    /// <param name="signal">Signal data</param>
    /// <param name="seriesName">Name of the data series</param>
    /// <param name="progressive"><see langword="True"/> if the progressive fractal dimension has been computed</param>
    //private void UpdateBasicPlots(double[] signal, string? seriesName = "", bool progressive = false)
    //{
    //    // Update plots
    //    UpdateOriginal(signal, seriesName ?? string.Empty);
    //    UpdateFractal(signal, seriesName ?? string.Empty, progressive);
    //}

    /// <summary>
    /// Updates the FFT related plots: FFT window, windowed signal, and signal FFT spectrum
    /// </summary>
    /// <param name="signal">Signal data</param>
    private void UpdateWindowPlots(double[] signal)
    {
        IWindow window = (IWindow)stripComboWindows.SelectedItem;
        if (window is null) return;

        // Round down to the next integer (Adjust to the lowest power of 2)
        int power2 = (int)Math.Floor(Math.Log2(signal.Length));
        //int evenPower = (power2 % 2 == 0) ? power2 : power2 - 1;
        signalFFT = new double[(int)Math.Pow(2, power2)];
        Array.Copy(signal, signalFFT, Math.Min(signalFFT.Length, signal.Length));

        // Apply window to signal
        double[] signalWindow = new double[signalFFT.Length];
        Array.Copy(signalFFT, signalWindow, signalFFT.Length);
        window.ApplyInPlace(signalWindow);

        UpdateKernel(window, signal.Length);
        UpdateWindowed(signalWindow);
        UpdateFFT(signalWindow);
    }
}

