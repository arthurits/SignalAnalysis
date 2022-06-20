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
        //plotOriginal.Plot.AxisAuto(0);
        plotOriginal.Refresh();
    }

    private void UpdateKernel(FftSharp.IWindow window, int points)
    {
        double[] kernel = window.Create(points);
        double[] pad = ScottPlot.DataGen.Zeros(kernel.Length / 4);
        double[] ys = pad.Concat(kernel).Concat(pad).ToArray();

        plotWindow.Clear();
        //plotWindow.Plot.Clear(typeof(ScottPlot.Plottable.SignalPlot));
        plotWindow.Plot.AddSignal(kernel, nSampleFreq, Color.Red);
        //plotWindow.Plot.AxisAuto(0);
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
        //plotApplied.Plot.AxisAuto(0);
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

    private void UpdateFFT(double[] signal, double[]? frequency = null)
    {
        // Plot the results
        plotFFT.Clear();
        if (signal.Length > 0)
        {
            if (frequency is not null)
                plotFFT.Plot.AddScatterLines(frequency, signal);
            else
                plotFFT.Plot.AddSignal(signal, 2 * (double)(signal.Length - 1) / nSampleFreq);
        }
        plotFFT.Plot.Title(StringsRM.GetString("strPlotFFTTitle", _settings.AppCulture) ?? "Fast Fourier transform");
        plotFFT.Plot.YLabel(_settings.PowerSpectra ? (StringsRM.GetString("strPlotFFTYLabelPow", _settings.AppCulture) ?? "Power (dB)") : (StringsRM.GetString("strPlotFFTYLabelMag", _settings.AppCulture) ?? "Magnitude (RMS²)"));
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
        this.UseWaitCursor = true;

        // ComputeStats(signal);
        if (tokenSource is not null) tokenSource.Dispose();
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
        _settings.CrossHair = false;
        ((ToolStripStatusLabelEx)((StatusStrip)((ToolStripPanel)this.Controls["StripPanelBottom"]).Controls["StatusStrip"]).Items["LabelExCrossHair"]).Checked = false;
        UpdateOriginal(signal, seriesName ?? string.Empty);
        UpdateFractal(signal, seriesName ?? string.Empty, _settings.CumulativeDimension);
        await UpdateWindowPlots(signal);

        txtStats.Text = Results.ToString(StringsRM, _settings.AppCulture);

        // Restore the cursor
        this.UseWaitCursor = false;
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
            {
                MessageBox.Show(
                      StringsRM.GetString("strMsgBoxTaskCancel", _settings.AppCulture) ?? $"Computation of the Hausdorff-Besicovitch fractal{Environment.NewLine}dimension has been stopped.",
                      StringsRM.GetString("strMsgBoxTaskCancelTitle", _settings.AppCulture) ?? "Stop",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Stop);
            }
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
        double[] freq = Array.Empty<double>();

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
                    signalFFT = _settings.PowerSpectra ? FftSharp.Transform.FFTpower(signalWindow) : FftSharp.Transform.FFTmagnitude(signalWindow);
                    // Substitute -Infinity values (which will throw an exception when plotting) for a minimum value of -340
                    signalFFT = signalFFT.Select(x => Double.IsInfinity(x) ? -340.0 : x).ToArray();
                    //freq = FftSharp.Transform.FFTfreq(nSampleFreq, signalFFT.Length);
                }
                catch (Exception ex)
                {
                    using (new CenterWinDialog(this))
                    {
                        MessageBox.Show(
                            String.Format(StringsRM.GetString("strMsgBoxErrorFFT", _settings.AppCulture) ?? "Unexpected error while computing the FFT." + Environment.NewLine + "{0}", ex.Message),
                            StringsRM.GetString("strMsgBoxErrorFFTTitle", _settings.AppCulture) ?? "FFT error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            });
        await statsTask;

        // Update plots
        _settings.CrossHair = false;
        ((ToolStripStatusLabelEx)((StatusStrip)((ToolStripPanel)this.Controls["StripPanelBottom"]).Controls["StatusStrip"]).Items["LabelExCrossHair"]).Checked = false;
        plotOriginal.ShowCrossHair = false;
        plotOriginal.Refresh();
        plotFractal.ShowCrossHair = false;
        plotFractal.Refresh();
        UpdateKernel(window, signal.Length);
        UpdateWindowed(signalWindow);
        UpdateFFT(signalFFT);
        //UpdateFFT(signalFFT, freq);

        // Restore the cursor
        this.UseWaitCursor = false;
        Cursor.Current = cursor;
    }
}

