namespace SignalAnalysis;
partial class FrmMain
{
    private void UpdateOriginal(double[] signal)
    {
        plotOriginal.Clear();
        //plotOriginal.Plot.Clear(typeof(ScottPlot.Plottable.SignalPlot));
        //plotOriginal.Plot.AddSignal(_signalData[cboSeries.SelectedIndex], nSampleFreq, label: cboSeries.SelectedItem.ToString());

        switch (_settings.AxisType)
        {
            case AxisType.Points:
                plotOriginal.Plot.AddSignal(signal, nSampleFreq/nSampleFreq, label: stripComboSeries.SelectedItem.ToString());
                plotOriginal.Plot.XAxis.DateTimeFormat(false);
                break;
            case AxisType.Seconds:
                plotOriginal.Plot.AddSignal(signal, nSampleFreq, label: stripComboSeries.SelectedItem.ToString());
                plotOriginal.Plot.XAxis.DateTimeFormat(false);
                break;
            case AxisType.DateTime:
                var sig = plotOriginal.Plot.AddSignal(signal, 24 * 60 * 60 * nSampleFreq, label: stripComboSeries.SelectedItem.ToString());
                sig.OffsetX = nStart.ToOADate();
                plotOriginal.Plot.XAxis.DateTimeFormat(true);
                break;
        }

        plotOriginal.Plot.Title(StringsRM.GetString("strPlotOriginalTitle", _settings.AppCulture));
        plotOriginal.Plot.YLabel(StringsRM.GetString("strPlotOriginalYLabel", _settings.AppCulture));
        plotOriginal.Plot.XLabel(StringsRM.GetString("strPlotOriginalXLabel", _settings.AppCulture));
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
        plotWindow.Plot.YLabel(StringsRM.GetString("strPlotWindowYLabel", _settings.AppCulture));
        plotWindow.Plot.XLabel(StringsRM.GetString("strPlotWindowXLabel", _settings.AppCulture));
        plotWindow.Refresh();
    }

    private void UpdateWindowed(double[] signal)
    {
        plotApplied.Clear();
        //plotApplied.Plot.Clear(typeof(ScottPlot.Plottable.SignalPlot));
        plotApplied.Plot.AddSignal(signal, nSampleFreq);
        plotApplied.Plot.Title(StringsRM.GetString("strPlotAppliedTitle", _settings.AppCulture));
        plotApplied.Plot.YLabel(StringsRM.GetString("strPlotAppliedYLabel", _settings.AppCulture));
        plotApplied.Plot.XLabel(StringsRM.GetString("strPlotAppliedXLabel", _settings.AppCulture));
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
            plotFractal.Plot.AddLine(0, FractalDimension.DimensionSingle, (0, signal.Length / nSampleFreq));
        }
        plotFractal.Plot.Title((StringsRM.GetString("strPlotFractalTitle", _settings.AppCulture) ?? "Fractal dimension") +
            " " +
            (progressive ? (StringsRM.GetString("strPlotFractalTitle()", _settings.AppCulture) ?? "(cumulative)") : String.Empty) +
            " (H = " + FractalDimension.DimensionSingle.ToString("#.00####") +
            " — Var(H) = " + FractalDimension.VarianceH.ToString("#.00####") + ")");
        plotFractal.Plot.YLabel(StringsRM.GetString("strPlotFractalYLabel", _settings.AppCulture) ?? );
        plotFractal.Plot.XLabel(StringsRM.GetString("strPlotFractalXLabel", _settings.AppCulture) ??);
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
                MessageBox.Show("Unexpected error while computing the FFT." + Environment.NewLine + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // Plot the results
        plotFFT.Clear();
        if (ys.Length > 0)
            plotFFT.Plot.AddSignal(ys, (double)ys.Length / nSampleFreq);
        plotFFT.Plot.Title(StringsRM.GetString("strPlotFFTTitle", _settings.AppCulture));
        plotFFT.Plot.YLabel(_settings.PowerSpectra ? StringsRM.GetString("strPlotFFTYLabelPow", _settings.AppCulture) : StringsRM.GetString("strPlotFFTXLabelMag", _settings.AppCulture));
        plotFFT.Plot.XLabel(StringsRM.GetString("strPlotFFTXLabel", _settings.AppCulture));
        plotFFT.Plot.AxisAuto(0);
        plotFFT.Refresh();
    }

    private void UpdateStats(double[] signal, bool cumulative = false, bool entropy = false)
    {
        if (signal.Length == 0) return;
        
        //var cursor = this.Cursor;
        //this.Cursor = Cursors.WaitCursor;

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

        // Compute fractal and entropy values
        try
        {
            FractalDimension.ComputeDimension(nSampleFreq, signal, token, cumulative);
            Results.FractalDimension = FractalDimension.DimensionSingle;
            Results.FractalVariance = FractalDimension.VarianceH;

            if (entropy)
                (Results.ApproximateEntropy, Results.SampleEntropy) = Complexity.Entropy(signal, token);
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
            //this.Cursor = cursor;
            //this.Cursor = Cursors.Default;
            //txtStats.Text = Results.ToString();
        }
        
    }
}
