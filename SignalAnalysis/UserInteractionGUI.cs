namespace SignalAnalysis;

partial class FrmMain
{
    private void Exit_Click(object? sender, EventArgs e)
    {
        Close();
    }

    private void Open_Click(object? sender, EventArgs e)
    {
        DialogResult result;
        string filePath;

        using OpenFileDialog openDlg = new()
        {
            Filter = StringsRM.GetString("strOpenDlgFilter", _settings.AppCulture) ?? "ErgoLux files (*.elux)|*.elux|SignalAnalysis files (*.sig)|*.sig|Text files (*.txt)|*.txt|All files (*.*)|*.*",
            FilterIndex = 4,
            InitialDirectory = _settings.RememberFileDialogPath ? _settings.UserOpenPath : _settings.DefaultOpenPath,
            RestoreDirectory = true,
            Title = StringsRM.GetString("strOpenDlgTitle", _settings.AppCulture) ?? "Select data file",
        };

        using (new CenterWinDialog(this))
            result = openDlg.ShowDialog(this);

        if (result == DialogResult.OK && openDlg.FileName != "")
        {
            //Get the path of specified file and store the directory for future calls
            filePath = openDlg.FileName;
            if (_settings.RememberFileDialogPath) _settings.UserOpenPath = Path.GetDirectoryName(filePath) ?? string.Empty;

            // Read the data file in the corresponding format
            bool boolRead = false;
            if (".elux".Equals(Path.GetExtension(filePath), StringComparison.OrdinalIgnoreCase))
                boolRead = ReadELuxData(filePath);
            else if (".sig".Equals(Path.GetExtension(filePath), StringComparison.OrdinalIgnoreCase))
                boolRead = ReadSigData(filePath);
            else if (".txt".Equals(Path.GetExtension(filePath), StringComparison.OrdinalIgnoreCase))
                boolRead = ReadTextData(filePath);

            if (boolRead)
            {
                PopulateComboSeries();
                this.Text = StringsRM.GetString("strFrmTitle", _settings.AppCulture) + " - " + openDlg.FileName;
            }
        }

    }

    private void Export_Click(object? sender, EventArgs e)
    {
        DialogResult result;
        string filePath;

        // Extract the values to be exported
        var signal = _signalData[stripComboSeries.SelectedIndex][_settings.IndexStart..(_settings.IndexEnd + 1)];

        // Exit if there is no data to be saved
        if (signal is null || signal.Length == 0)
        {
            // Exit if no data has been received or the matrices are still un-initialized
            using (new CenterWinDialog(this))
            {
                MessageBox.Show(StringsRM.GetString("strMsgBoxNoData", _settings.AppCulture) ?? "There is no data available to be saved.",
                    StringsRM.GetString("strMsgBoxNoDataTitle", _settings.AppCulture) ?? "No data",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            return;
        }

        // Displays a SaveFileDialog, so the user can save the data into a file  
        SaveFileDialog SaveDlg = new()
        {
            DefaultExt = "*.txt",
            Filter = StringsRM.GetString("strSaveDlgFilter", _settings.AppCulture) ?? "Text file (*.txt)|*.txt|SignalAnalysis file (*.sig)|*.sig|Binary file (*.bin)|*.bin|All files (*.*)|*.*",
            FilterIndex = 1,
            InitialDirectory = _settings.RememberFileDialogPath ? _settings.UserSavePath : _settings.DefaultSavePath,
            OverwritePrompt = true,
            Title = StringsRM.GetString("strSaveDlgTitle", _settings.AppCulture) ?? "Export data"
        };

        using (new CenterWinDialog(this))
            result = SaveDlg.ShowDialog(this.Parent);

        // If the file name is not an empty string, call the corresponding routine to save the data into a file.  
        if (result == DialogResult.OK && SaveDlg.FileName != "")
        {
            //Get the path of specified file and store the directory for future calls
            filePath = SaveDlg.FileName;
            if (_settings.RememberFileDialogPath) _settings.UserSavePath = Path.GetDirectoryName(filePath) ?? string.Empty;

            switch (Path.GetExtension(SaveDlg.FileName).ToLower())
            {
                //case ".elux":
                //    SaveELuxData(SaveDlg.FileName);
                //    break;
                case ".txt":
                    SaveTextData(SaveDlg.FileName, signal, _settings.IndexStart, stripComboSeries.SelectedText);
                    break;
                case ".sig":
                    SaveSigData(SaveDlg.FileName, signal, _settings.IndexStart, stripComboSeries.SelectedText);
                    break;
                case ".bin":
                    SaveBinaryData(SaveDlg.FileName);
                    break;
                default:
                    SaveDefaultData(SaveDlg.FileName);
                    break;
            }
        }
    }

    private void Settings_Click(object? sender, EventArgs e)
    {
        var frm = new FrmSettings(_settings);
        frm.ShowDialog();
        if (frm.DialogResult == DialogResult.OK)
        {
            _settings = frm.Settings;
            ComboSeries_SelectedIndexChanged(this, EventArgs.Empty);

            ((ToolStripStatusLabelEx)((StatusStrip)((ToolStripPanel)Controls["StripPanelBottom"]).Controls["StatusStrip"]).Items["LabelExPower"]).Checked = _settings.PowerSpectra;
            ((ToolStripStatusLabelEx)((StatusStrip)((ToolStripPanel)Controls["StripPanelBottom"]).Controls["StatusStrip"]).Items["LabelExCumulative"]).Checked = _settings.CumulativeDimension;
            ((ToolStripStatusLabelEx)((StatusStrip)((ToolStripPanel)Controls["StripPanelBottom"]).Controls["StatusStrip"]).Items["LabelExEntropy"]).Checked = _settings.Entropy;
            ((ToolStripStatusLabelEx)((StatusStrip)((ToolStripPanel)Controls["StripPanelBottom"]).Controls["StatusStrip"]).Items["LabelExCrossHair"]).Checked = _settings.CrossHair;

            UpdateUI_Language();
        }

    }

    private void About_Click(object? sender, EventArgs e)
    {
        FrmAbout frmAbout = new();
        frmAbout.ShowDialog();
    }

    private void LabelEx_CheckedChanged(object? sender, EventArgs e)
    {
        if (sender is not null && sender is ToolStripStatusLabelEx LabelEx)
        {
            var label = LabelEx;
            // Change the text color
            if (label.Checked)
                label.ForeColor = Color.Black;
            else
                label.ForeColor = Color.LightGray;
        }
    }

    private void LabelEx_Click(object? sender, EventArgs e)
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

            // Update the settings
            switch (label.Name)
            {
                case "LabelExPower":
                    _settings.PowerSpectra = label.Checked;
                    break;
                case "LabelExCumulative":
                    _settings.CumulativeDimension = label.Checked;
                    if (label.Checked && statsTask is not null && statsTask.Status == TaskStatus.Running)
                        FrmMain_KeyPress(sender, new KeyPressEventArgs((char)Keys.Escape));
                    break;
                case "LabelExEntropy":
                    _settings.Entropy = label.Checked;
                    if (label.Checked && statsTask is not null && statsTask.Status == TaskStatus.Running)
                        FrmMain_KeyPress(sender, new KeyPressEventArgs((char)Keys.Escape));
                    break;
                case "LabelExCrossHair":
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
                    break;
            }
        }
    }

}

