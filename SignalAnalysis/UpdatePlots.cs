namespace SignalAnalysis;
partial class FrmMain
{
    private void UpdateOriginal()
    {
        plotOriginal.Clear();
        //plotOriginal.Plot.Clear(typeof(ScottPlot.Plottable.SignalPlot));
        plotOriginal.Plot.AddSignal(_signalData[cboSeries.SelectedIndex], nSampleFreq, label: cboSeries.SelectedItem.ToString());
        plotOriginal.Plot.Title("Input signal");
        plotOriginal.Plot.YLabel("Amplitude");
        plotOriginal.Plot.XLabel("Time (seconds)");
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
        plotWindow.Plot.Title($"{window} Window");
        plotWindow.Plot.YLabel("Amplitude");
        plotWindow.Plot.XLabel("Time (seconds)");
        plotWindow.Refresh();
    }

    private void UpdateWindowed(double[] signal)
    {
        plotApplied.Clear();
        //plotApplied.Plot.Clear(typeof(ScottPlot.Plottable.SignalPlot));
        plotApplied.Plot.AddSignal(signal, nSampleFreq);
        plotApplied.Plot.Title("Windowed signal");
        plotApplied.Plot.YLabel("Amplitude");
        plotApplied.Plot.XLabel("Time (seconds)");
        plotApplied.Plot.AxisAuto(0);
        plotApplied.Refresh();
    }

    private async void UpdateFractal(bool progressive = false)
    {
        if (_signalData.Length == 0) return;

        var cursor = this.Cursor;
        this.Cursor = Cursors.WaitCursor;

        int index = cboSeries.SelectedIndex;

        tokenSource = new();
        token = tokenSource.Token;
        fractalTask = Task.Run(() =>
        {
            FractalDimension.GetDimension(nSampleFreq, _signalData[index], token, progressive);
        }, token);

        try
        {
            await fractalTask;

            plotFractal.Clear();
            if (progressive && FractalDimension.DimensionCumulative.Length > 0)
            {
                plotFractal.Plot.AddSignal(FractalDimension.DimensionCumulative, nSampleFreq, label: cboSeries.SelectedItem.ToString());
            }
            else
            {
                plotFractal.Plot.AddLine(0, FractalDimension.DimensionSingle, (0, nPoints / nSampleFreq));
            }
            plotFractal.Plot.Title("Fractal dimension" + (progressive ? " (progressive)" : String.Empty) + " (H = " + FractalDimension.DimensionSingle.ToString("#.00000") + ")");
            plotFractal.Plot.YLabel("Dimension (H)");
            plotFractal.Plot.XLabel("Time (seconds)");
            plotFractal.Plot.AxisAuto(0);
            plotFractal.Refresh();
        }
        catch (OperationCanceledException)
        {
            using (new CenterWinDialog(this))
                MessageBox.Show(this,
                    "Computation of the Hausdorff-Besicovitch fractal\ndimension has been stopped.",
                    "Stop",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Stop);
            this.chkProgressive.Checked = false;
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
        double[] ys = chkLog.Checked ? FftSharp.Transform.FFTpower(signal) : FftSharp.Transform.FFTmagnitude(signal);

        // Plot the results
        plotFFT.Clear();
        plotFFT.Plot.AddSignal(ys, (double)ys.Length / nSampleFreq);
        plotFFT.Plot.Title("Fast Fourier transform");
        plotFFT.Plot.YLabel(chkLog.Checked ? "Power (dB)" : "Magnitude (RMS²)");
        plotFFT.Plot.XLabel("Frequency (Hz)");
        plotFFT.Plot.AxisAuto(0);
        plotFFT.Refresh();
    }
}
