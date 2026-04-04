using SignalAnalysis.Models;
using SignalAnalysis.NumericalAlgorithms;

namespace SignalAnalysis.Helpers;

internal static class Compute
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
    /// <param name="fftBluestein"><see langword="True"/> if Bluestein algorithm is used</param>
    /// <returns></returns>
    internal static async Task ComputeAsync(DocumentBase data, int series, bool deletePreviousResults = false, bool stats = false, bool boxplot = false, bool derivative = false, bool integral = false, bool fractal = false, bool progressive = false, bool entropy = false, bool fft = false, bool fftPlot = false, bool powerSpectra = false, bool fftRoundUp = true, bool fftBluestein = true)
    {
        if (derivative)
            Derivative.Derivate(data.SeriesData[series].ToArray());

        //     Clip signal data to the user-specified bounds 
        //    if (Signal.Data is null || Signal.Data.Length == 0) return;
        //    double[] signalClipped = Signal.Data[series][Signal.IndexStart..(Signal.IndexEnd + 1)];
        //    if (signalClipped is null || signalClipped.Length == 0) return;
        //    string? seriesName = stripComboSeries.SelectedItem is null ? stripComboSeries.Items[0]?.ToString() : stripComboSeries.SelectedItem.ToString();

        //     Show waiting cursor
        //    var cursor = this.Cursor;
        //    Cursor = Cursors.WaitCursor;
        //    this.UseWaitCursor = true;

        //     Compute data;
        //    double[] signalWindowed = [];
        //    IWindow? window = (IWindow?)stripComboWindows.SelectedItem;
        //    tokenSource?.Dispose();
        //    tokenSource = new();
        //    token = tokenSource.Token;

        //    if (deletePreviousResults)
        //        Results = new();

        //    statsTask = Task.Run(() =>
        //    {
        //        try
        //        {
        //            if (stats) ComputeStatistics(signalClipped);
        //            if (boxplot) ComputeBoxplot(signalClipped);
        //            if (derivative) ComputeDerivative(signalClipped);
        //            if (integral) ComputeIntegral(signalClipped);
        //            if (fractal) ComputeFractal(signalClipped, progressive);
        //            if (entropy) ComputeEntropy(signalClipped);
        //            if (fft) signalWindowed = ComputeFFT(signalClipped, fftRoundUp, fftBluestein, window);
        //        }
        //        catch (OperationCanceledException ex)
        //        {
        //             This is needed beacuse this exception is thrown while the cumulative fractal dimension is computed from another Task in "UpdateStatsPlots".
        //            Invoke(() =>
        //            {
        //                string msg = string.Empty;
        //                string title = string.Empty;
        //                switch (ex.Message)
        //                {
        //                    case "CancelDerivative":
        //                        msg = StringResources.MsgBoxTaskDerivativeCancel;
        //                        title = StringResources.MsgBoxTaskDerivativeCancelTitle;
        //                        _settings.ComputeDerivative = false;
        //                        this.statusStripLabelExDerivative.Checked = false;
        //                        break;
        //                    case "CancelFractal":
        //                        msg = StringResources.MsgBoxTaskFractalCancel;
        //                        title = StringResources.MsgBoxTaskFractalCancelTitle;
        //                        _settings.CumulativeDimension = false;
        //                        this.statusStripLabelExCumulative.Checked = false;
        //                        break;
        //                    case "CancelEntropy":
        //                        msg = StringResources.MsgBoxTaskEntropyCancel;
        //                        title = StringResources.MsgBoxTaskEntropyCancelTitle;
        //                        _settings.ComputeEntropy = false;
        //                        this.statusStripLabelExEntropy.Checked = false;
        //                        break;
        //                }
        //                using (new CenterWinDialog(this))
        //                {
        //                    MessageBox.Show(this,
        //                          msg,
        //                          title,
        //                          MessageBoxButtons.OK,
        //                          MessageBoxIcon.Stop);
        //                }
        //            });
        //        }
        //        finally
        //        {
        //            tokenSource.Dispose();
        //        }
        //    }, token);
        //    await statsTask;

        //     Show results on plots
        //    _settings.CrossHair = false;
        //    statusStripLabelExCrossHair.Checked = false;
        //    if (stats) PlotOriginal(signalClipped, seriesName ?? string.Empty);
        //    if (boxplot) PlotBoxplot(signalClipped, seriesName ?? string.Empty);
        //    if (derivative) PlotDerivative(signalClipped, seriesName ?? string.Empty);
        //    if (fractal)
        //    {
        //        PlotFractal(signalClipped, seriesName ?? string.Empty, _settings.CumulativeDimension);
        //        PlotFractalDistribution(Results.FractalDimension, Results.FractalVariance);
        //    }
        //    if (fft)
        //    {
        //        if (window is not null) PlotKernel(window, signalClipped.Length);
        //        if (signalWindowed.Length > 0) PlotWindowedSignal(signalWindowed);
        //    }
        //    if (fftPlot)
        //        PlotFFT(powerSpectra ? Results.FFTpower : Results.FFTmagnitude);

        //     Show text results
        //    if (stats || fractal || entropy || integral)
        //    txtStats.Text = Results.ToString(
        //        culture: _settings.AppCulture,
        //        boxplot: _settings.Boxplot,
        //        entropy: _settings.ComputeEntropy,
        //        entropyAlgorithm: _settings.ComputeEntropy ? StringResources.EntropyAlgorithms.Split(", ")[(int)_settings.EntropyAlgorithm] : string.Empty,
        //        entropyM: (int)_settings.EntropyFactorM,
        //        entropyR: _settings.EntropyFactorR,
        //        integral: _settings.ComputeIntegration,
        //        integralAlgorithm: _settings.ComputeIntegration ? StringResources.IntegrationAlgorithms.Split(", ")[(int)_settings.IntegrationAlgorithm] : string.Empty);

        //     Restore the cursor
        //    this.UseWaitCursor = false;
        //    Cursor = cursor;
        }

    ///// <summary>
    ///// Computes the derivative of a 1D data array.
    ///// </summary>
    ///// <param name="signal">1D data array values whose values are expected to be uniformly spaced</param>
    ///// <exception cref="OperationCanceledException">This is thrown if the token is cancelled whenever the user presses the ESC button</exception>
    //private void ComputeDerivative(double[] signal)
    //{
    //    Results.Derivative = Derivative.Derivate(
    //        array: signal,
    //        method: _settings.DerivativeAlgorithm,
    //        lowerIndex: 0,
    //        upperIndex: signal.Length - 1,
    //        samplingFrequency: Signal.SampleFrequency);

    //    // Replacing NaN values using LinQ since ScottPlot throws an exception for NaN values
    //    //var watch = new System.Diagnostics.Stopwatch();
    //    //watch.Start();
    //    Results.Derivative = Results.Derivative.Select(x => double.IsNaN(x) ? 0 : x).ToArray();
    //    //watch.Stop();
    //    //System.Diagnostics.Debug.WriteLine($"LinQ execution time: {watch.ElapsedMilliseconds} ms");

    //    //if (!watch.IsRunning)
    //    //    watch.Restart(); // Reset time to 0 and start measuring

    //    // Replacing NaN values using a for loop
    //    //double x;
    //    //for (int i = 0; i < Results.Derivative.Length; i++)
    //    //{
    //    //    x = Results.Derivative[i];
    //    //    Results.Derivative[i] = double.IsNaN(x) ? 0 : x;
    //    //}

    //    //watch.Stop();
    //    //System.Diagnostics.Debug.WriteLine($"For-loop execution time: {watch.ElapsedMilliseconds} ms");

    //    // This doesn't quite work
    //    //Array.ForEach(Results.Derivative, x => x = double.IsNaN(x) ? 0 : x);
    //}
}
