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

        plotOriginal.Plot.Title(StringsRM.GetString("strPlotOriginalTitle"));
        plotOriginal.Plot.YLabel(StringsRM.GetString("strPlotOriginalYLabel"));
        plotOriginal.Plot.XLabel(StringsRM.GetString("strPlotOriginalXLabel"));
        plotOriginal.Plot.AxisAuto(0);
        plotOriginal.Refresh();
    }

    private void UpdateKernel(FftSharp.IWindow window)
    {
        double[] kernel = window.Create(nPoints);
        double[] pad = ScottPlot.DataGen.Zeros(kernel.Length / 4);
        double[] ys = pad.Concat(kernel).Concat(pad).ToArray();

        plotWindow.Clear();
        //plotWindow.Plot.Clear(typeof(ScottPlot.Plottable.SignalPlot));
        plotWindow.Plot.AddSignal(ys, nSampleFreq, Color.Red);
        plotWindow.Plot.AxisAuto(0);
        plotWindow.Plot.Title(StringsRM.GetString("strPlotWindowTitle"));
        plotWindow.Plot.YLabel(StringsRM.GetString("strPlotWindowYLabel"));
        plotWindow.Plot.XLabel(StringsRM.GetString("strPlotWindowXLabel"));
        plotWindow.Refresh();
    }

    private void UpdateWindowed(double[] signal)
    {
        plotApplied.Clear();
        //plotApplied.Plot.Clear(typeof(ScottPlot.Plottable.SignalPlot));
        plotApplied.Plot.AddSignal(signal, nSampleFreq);
        plotApplied.Plot.Title(StringsRM.GetString("strPlotAppliedTitle"));
        plotApplied.Plot.YLabel(StringsRM.GetString("strPlotAppliedYLabel"));
        plotApplied.Plot.XLabel(StringsRM.GetString("strPlotAppliedXLabel"));
        plotApplied.Plot.AxisAuto(0);
        plotApplied.Refresh();
    }

    private async void UpdateFractal(double[] signal, bool progressive = false)
    {
        if (_signalData.Length == 0) return;

        var cursor = this.Cursor;
        this.Cursor = Cursors.WaitCursor;

        int index = stripComboSeries.SelectedIndex;

        tokenSource = new();
        token = tokenSource.Token;
        fractalTask = Task.Run(() =>
        {
            //FractalDimension.GetDimension(nSampleFreq, _signalData[index], token, progressive);
            FractalDimension.GetDimension(nSampleFreq, signal, token, progressive);
        }, token);

        try
        {
            await fractalTask;
            Results.FractalDimension = FractalDimension.DimensionSingle;

            plotFractal.Clear();
            if (progressive && FractalDimension.DimensionCumulative.Length > 0)
            {
                plotFractal.Plot.AddSignal(FractalDimension.DimensionCumulative, nSampleFreq, label: stripComboSeries.SelectedItem.ToString());
            }
            else
            {
                plotFractal.Plot.AddLine(0, FractalDimension.DimensionSingle, (0, nPoints / nSampleFreq));
            }
            plotFractal.Plot.Title("Fractal dimension" + (progressive ? " (progressive)" : String.Empty) + " (H = " + FractalDimension.DimensionSingle.ToString("#.00000") + ")");
            plotFractal.Plot.YLabel(StringsRM.GetString("strPlotFractalYLabel"));
            plotFractal.Plot.XLabel(StringsRM.GetString("strPlotFractalXLabel"));
            plotFractal.Plot.AxisAuto(0);
            plotFractal.Refresh();
        }
        catch (OperationCanceledException)
        {
            using (new CenterWinDialog(this))
                MessageBox.Show(this,
                    StringsRM.GetString("strMsgBoxTaskCancel"),
                    StringsRM.GetString("strMsgBoxTaskCancelTitle"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Stop);
            _settings.CumulativeDimension = false;
        }
        finally
        {
            tokenSource.Dispose();
            this.Cursor = cursor;
            this.Cursor = Cursors.Default;
        }

    }

    private void UpdateFFT(double[] signal)
    {
        double[] ys = _settings.PowerSpectra ? FftSharp.Transform.FFTpower(signal) : FftSharp.Transform.FFTmagnitude(signal);

        // Plot the results
        plotFFT.Clear();
        plotFFT.Plot.AddSignal(ys, (double)ys.Length / nSampleFreq);
        plotFFT.Plot.Title(StringsRM.GetString("strPlotFFTTitle"));
        plotFFT.Plot.YLabel(_settings.PowerSpectra ? StringsRM.GetString("strPlotFFTYLabelPow") : StringsRM.GetString("strPlotFFTXLabelMag"));
        plotFFT.Plot.XLabel(StringsRM.GetString("strPlotFFTXLabel"));
        plotFFT.Plot.AxisAuto(0);
        plotFFT.Refresh();
    }

    private void UpdateStats(double[] signal, bool entropy = false)
    {
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
        if (entropy)
            (Results.ApproximateEntropy, Results.SampleEntropy) = Complexity.Entropy(signal);

        txtStats.Text = $"Average illuminance: {Results.Average:0.######}" + Environment.NewLine +
            $"Maximum illuminance: {Results.Maximum:0.##}" + Environment.NewLine +
            $"Minimum illuminance: {Results.Minimum:0.##}" + Environment.NewLine +
            $"Fractal dimension: {Results.FractalDimension:0.########}" + Environment.NewLine +
            $"Approximate entropy: {Results.ApproximateEntropy:0.########}" + Environment.NewLine +
            $"Sample entropy: {Results.SampleEntropy:0.########}"; 
        //lblStats.Text = String.Concat("Avg: ", avg.ToString("0.###"), " Max: ", max.ToString("0.#"), " Min: ", min.ToString("0.#"));
        // Console.WriteLine($"Hello, {name}! Today is {date.DayOfWeek}, it's {date:HH:mm} now.");
    }
}
