namespace SignalAnalysis;

partial class FrmMain
{
    private void Exit_Click(object? sender, EventArgs e)
    {
        Close();
    }

    private async void Open_Click(object? sender, EventArgs e)
    {
        DialogResult result;
        string filePath;

        using OpenFileDialog openDlg = new()
        {
            Filter = StringResources.OpenDlgFilter,
            FilterIndex = 5,
            InitialDirectory = _settings.RememberFileDialogPath ? _settings.UserOpenPath : _settings.DefaultOpenPath,
            RestoreDirectory = true,
            Title = StringResources.OpenDlgTitle,
        };

        using (new CenterWinDialog(this))
        {
            result = openDlg.ShowDialog(this);
        }

        if (result == DialogResult.OK && openDlg.FileName != "")
        {
            // Show a waiting cursor
            var cursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            //Get the path of specified file and store the directory for future calls
            filePath = openDlg.FileName;
            if (_settings.RememberFileDialogPath) _settings.UserOpenPath = Path.GetDirectoryName(filePath) ?? string.Empty;

            // Read the data file in the corresponding format
            SignalStats? results = null;
            SignalData signalData = new();
            bool boolRead = Path.GetExtension(filePath).ToLower() switch
            {
                ".elux" => ReadELuxData(filePath, signalData),
                ".sig" => ReadSigData(filePath, signalData),
                ".txt" => ReadTextData(filePath, signalData, results = new()),
                ".bin" => ReadBinData(filePath, signalData, results = new()),
                _ => ReadNotImplemented(filePath)
            };

            if (boolRead)
            {
                // Data
                Signal = signalData;

                // Update UI
                PopulateComboSeries();
                SetFormTitle(this, openDlg.FileName);
                UpdateUI_MeasuringTime();

                // Check if computations are needed
                if (results is not null)
                {
                    Results = results;
                    txtStats.Text = Results.ToString(
                        culture: _settings.AppCulture,
                        boxplot: _settings.Boxplot,
                        entropy: _settings.ComputeEntropy,
                        entropyAlgorithm: _settings.ComputeEntropy ? StringResources.EntropyAlgorithms.Split(", ")[(int)_settings.EntropyAlgorithm] : string.Empty,
                        entropyM: (int)_settings.EntropyFactorM,
                        entropyR: _settings.EntropyFactorR,
                        integral: _settings.ComputeIntegration,
                        integralAlgorithm: _settings.ComputeIntegration ? StringResources.IntegrationAlgorithms.Split(", ")[(int)_settings.IntegrationAlgorithm] : string.Empty);
                }
                else
                {
                    await ComputeAsync(stripComboSeries.SelectedIndex,
                                    deletePreviousResults: true,
                                    stats: true,
                                    boxplot: _settings.Boxplot,
                                    derivative: _settings.ComputeDerivative,
                                    integral: _settings.ComputeIntegration,
                                    fractal: true,
                                    progressive: _settings.CumulativeDimension,
                                    entropy: _settings.ComputeEntropy,
                                    fft: true,
                                    fftPlot: true,
                                    powerSpectra: _settings.PowerSpectra,
                                    fftRoundUp: _settings.FFTRoundUp);
                }
            }

            

            // Restore the cursor
            Cursor.Current = cursor;
        }

    }

    private void Export_Click(object? sender, EventArgs e)
    {
        DialogResult result;
        string filePath;

        // Extract the values to be exported
        double[] signal = [];
        if (Signal.Data.Length > 0)
            signal = Signal.Data[stripComboSeries.SelectedIndex][Signal.IndexStart..(Signal.IndexEnd + 1)];

        // Exit if there is no data to be saved
        if (signal.Length == 0)
        {
            // Exit if no data has been received or the matrices are still un-initialized
            using (new CenterWinDialog(this))
            {
                MessageBox.Show(this,
                    StringResources.MsgBoxNoData,
                    StringResources.MsgBoxNoDataTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            return;
        }

        // Displays a SaveFileDialog, so the user can save the data into a file  
        SaveFileDialog SaveDlg = new()
        {
            DefaultExt = "*.txt",
            Filter = StringResources.SaveDlgFilter,
            FilterIndex = 1,
            InitialDirectory = _settings.RememberFileDialogPath ? _settings.UserSavePath : _settings.DefaultSavePath,
            OverwritePrompt = true,
            Title = StringResources.SaveDlgTitle
        };

        using (new CenterWinDialog(this))
        {
            result = SaveDlg.ShowDialog(this.Parent);
        }

        // If the file name is not an empty string, call the corresponding routine to save the data into a file.  
        if (result == DialogResult.OK && SaveDlg.FileName != "")
        {
            // Show a waiting cursor
            var cursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            //Get the path of specified file and store the directory for future calls
            filePath = SaveDlg.FileName;
            if (_settings.RememberFileDialogPath) _settings.UserSavePath = Path.GetDirectoryName(filePath) ?? string.Empty;
            
            var boolSave = Path.GetExtension(filePath).ToLower() switch
            {
                ".txt" => SaveTextData(SaveDlg.FileName, signal, _settings.IndexStart, stripComboSeries.SelectedItem?.ToString()),
                ".sig" => SaveSigData(SaveDlg.FileName, signal, _settings.IndexStart, stripComboSeries.SelectedItem?.ToString()),
                ".bin" => SaveBinaryData(SaveDlg.FileName, signal, _settings.IndexStart, stripComboSeries.SelectedItem?.ToString()),
                ".results" => SaveResultsData(SaveDlg.FileName, signal, _settings.IndexStart),
                _ => SaveDefaultData(SaveDlg.FileName, signal, _settings.IndexStart, stripComboSeries.SelectedItem?.ToString()),
            };

            // Restore the cursor
            Cursor.Current = cursor;

            if (boolSave)
            {
                // Show OK save data
                using (new CenterWinDialog(this))
                {
                    MessageBox.Show(this,
                        StringResources.MsgBoxOKSaveData,
                        StringResources.MsgBoxOKSaveDataTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }
    }

    private async void Settings_Click(object? sender, EventArgs e)
    {
        _settings.IndexStart = Signal.IndexStart;
        _settings.IndexEnd = Signal.IndexEnd;
        _settings.IndexMax = Signal.SeriesPoints > 0 ? Signal.SeriesPoints - 1 : 0;
        
        FrmSettings frm = new(_settings);
        frm.Icon = GraphicsResources.Load<Icon>(GraphicsResources.AppLogo);
        frm.ShowDialog(this);

        if (frm.DialogResult == DialogResult.OK)
        {
            Signal.IndexStart = _settings.IndexStart;
            Signal.IndexEnd = _settings.IndexEnd;

            // Update UI
            //ComboSeries_SelectedIndexChanged(this, EventArgs.Empty);

            ShowHideBoxplot(_settings.Boxplot);
            statusStripLabelExBoxplot.Checked = _settings.Boxplot;
            statusStripLabelExDerivative.Checked = _settings.ComputeDerivative;
            statusStripLabelExIntegration.Checked = _settings.ComputeIntegration;
            statusStripLabelExCumulative.Checked = _settings.CumulativeDimension;
            statusStripLabelExPower.Checked = _settings.PowerSpectra;
            statusStripLabelExEntropy.Checked = _settings.ComputeEntropy;
            statusStripLabelExCrossHair.Checked = _settings.CrossHair;

            UpdateUI_Language();

            await ComputeAsync(stripComboSeries.SelectedIndex,
                deletePreviousResults: false,
                stats: true,
                boxplot: _settings.Boxplot,
                derivative: _settings.ComputeDerivative,
                integral: _settings.ComputeIntegration,
                fractal: true,
                progressive: _settings.CumulativeDimension,
                entropy: _settings.ComputeEntropy,
                fft: true,
                fftPlot: true,
                powerSpectra: _settings.PowerSpectra,
                fftRoundUp: _settings.FFTRoundUp);
        }

    }

    private void About_Click(object? sender, EventArgs e)
    {
        FrmAbout frmAbout = new();
        frmAbout.ShowDialog();
    }

    private void Language_Click(object? sender, EventArgs e)
    {
        FrmLanguage frm = new(_settings);
        frm.Icon = GraphicsResources.Load<Icon>(GraphicsResources.AppLogo);
        frm.ShowDialog();

        if (frm.DialogResult == DialogResult.OK)
            UpdateUI_Language();
    }

    private async void LabelEx_Click(object? sender, EventArgs e)
    {
        if (sender is not null && sender is ToolStripStatusLabelEx LabelEx)
        {
            var label = LabelEx;
            label.Checked = !label.Checked;

            // Change the text color
            if (label.Checked)
                label.ForeColor = Color.Black;
            else
                label.ForeColor = Color.LightGray;

            // Update settings values
            switch (label.Name)
            {
                case "statusStripLabelExBoxplot":
                    _settings.Boxplot = label.Checked;
                    ShowHideBoxplot(label.Checked);
                    await ComputeAsync(stripComboSeries.SelectedIndex, boxplot: _settings.Boxplot);
                    break;
                case "statusStripLabelExDerivative":
                    _settings.ComputeDerivative = label.Checked;
                    await ComputeAsync(stripComboSeries.SelectedIndex, derivative: _settings.ComputeDerivative);
                    if (_settings.ComputeDerivative == false)
                    {
                        plotDerivative.Clear();
                        plotDerivative.Refresh();
                    }
                    break;
                case "statusStripLabelExIntegration":
                    _settings.ComputeIntegration = label.Checked;
                    await ComputeAsync(stripComboSeries.SelectedIndex, integral: _settings.ComputeIntegration);
                    break;
                case "statusStripLabelExCumulative":
                    _settings.CumulativeDimension = label.Checked;
                    await ComputeAsync(stripComboSeries.SelectedIndex, fractal: true, progressive: _settings.CumulativeDimension);
                    if (!label.Checked && statsTask is not null && statsTask.Status == TaskStatus.Running)
                        FrmMain_KeyPress(sender, new KeyPressEventArgs((char)Keys.Escape));
                    break;
                case "statusStripLabelExPower":
                    _settings.PowerSpectra = label.Checked;
                    await ComputeAsync(stripComboSeries.SelectedIndex, fftPlot: true, powerSpectra: _settings.PowerSpectra, fftRoundUp: _settings.FFTRoundUp);
                    break;
                case "statusStripLabelExEntropy":
                    _settings.ComputeEntropy = label.Checked;
                    await ComputeAsync(stripComboSeries.SelectedIndex, entropy: _settings.ComputeEntropy);
                    if (!label.Checked && statsTask is not null && statsTask.Status == TaskStatus.Running)
                        FrmMain_KeyPress(sender, new KeyPressEventArgs((char)Keys.Escape));
                    break;
                case "statusStripLabelExCrossHair":
                    _settings.CrossHair = label.Checked;
                    if (plotOriginal is not null && plotOriginal.Plot.GetPlottables().Length > 0)
                    {
                        plotOriginal.ShowCrossHair = label.Checked;
                        plotOriginal.Refresh();
                    }
                    if (plotWindow is not null && plotWindow.Plot.GetPlottables().Length > 0)
                    {
                        plotWindow.ShowCrossHair = label.Checked;
                        plotWindow.Refresh();
                    }
                    if (plotApplied is not null && plotApplied.Plot.GetPlottables().Length > 0)
                    {
                        plotApplied.ShowCrossHair = label.Checked;
                        plotApplied.Refresh();
                    }
                    if (plotFractal is not null && plotFractal.Plot.GetPlottables().Length > 0)
                    {
                        plotFractal.ShowCrossHair = label.Checked;
                        plotFractal.Refresh();
                    }
                    if (plotFFT is not null && plotFFT.Plot.GetPlottables().Length > 0)
                    {
                        plotFFT.ShowCrossHair = label.Checked;
                        plotFFT.Refresh();
                    }
                    if (plotDerivative is not null && plotDerivative.Plot.GetPlottables().Length > 0)
                    {
                        plotDerivative.ShowCrossHair = label.Checked;
                        plotDerivative.Refresh();
                    }
                    break;
            }
        }
    }

    private void PopulateComboSeries(params string[] values)
    {
        stripComboSeries.Items.Clear();
        if (values.Length != 0)
        {
            stripComboSeries.Items.AddRange(values);
            stripComboSeries.Text = values[0];
        }
        else
        {
            stripComboSeries.Items.AddRange(Signal.SeriesLabels);
            stripComboSeries.Text = Signal.SeriesLabels[0];
        }

    }

    private async void ComboSeries_SelectionChangeCommitted(object? sender, EventArgs e)
    {
        // Move the focus away in order to deselect the text
        //this.tableLayoutPanel1.Focus();

        if (Signal.Data.Length == 0) return;

        //statusStripLabelExEntropy.Checked = false;
        //_settings.Entropy = false;

        // Update stats and plots
        await ComputeAsync(stripComboSeries.SelectedIndex,
            deletePreviousResults: false,
            stats: true,
            boxplot: _settings.Boxplot,
            derivative: _settings.ComputeDerivative,
            integral: _settings.ComputeIntegration,
            fractal: true,
            progressive: _settings.CumulativeDimension,
            entropy: _settings.ComputeEntropy,
            fft: true,
            powerSpectra: _settings.PowerSpectra,
            fftRoundUp: _settings.FFTRoundUp);
    }

    private async void ComboWindow_SelectionChangeCommitted(object? sender, EventArgs e)
    {
        // Move the focus away in order to deselect the text
        //this.tableLayoutPanel1.Focus();

        if (stripComboSeries.SelectedIndex < 0) return;

        // Extract the values 
        var signal = Signal.Data[stripComboSeries.SelectedIndex][Signal.IndexStart..(Signal.IndexEnd + 1)];
        if (signal is null || signal.Length == 0) return;

        //UpdateWindowPlots(signal);
        await ComputeAsync(stripComboSeries.SelectedIndex,
            deletePreviousResults: false,
            stats: false,
            boxplot: _settings.Boxplot,
            derivative: false,
            integral: false,
            fractal: false,
            progressive: false,
            entropy: false,
            fft: true,
            powerSpectra: _settings.PowerSpectra,
            fftRoundUp: _settings.FFTRoundUp);
    }

    private void ShowHideBoxplot (bool show = true)
    {
        layoutData.ColumnStyles[1].Width = show ? 20F : 0F;
    }
}
