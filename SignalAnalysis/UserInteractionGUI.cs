namespace SignalAnalysis;

partial class FrmMain
{
    private void Exit_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void Open_Click(object sender, EventArgs e)
    {
        DialogResult result;
        string filePath;

        using OpenFileDialog openDlg = new()
        {
            Title = "Select data file",
            InitialDirectory = _settings.RememberFileDialogPath ? _settings.UserOpenPath : _settings.DefaultOpenPath,
            Filter = "ErgoLux files (*.elux)|*.elux|SignalAnalysis files (*.sig)|*.sig|Text files (*.txt)|*.txt|All files (*.*)|*.*",
            FilterIndex = 4,
            RestoreDirectory = true
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

    private void Export_Click(object sender, EventArgs e)
    {
        DialogResult result;
        string filePath;

        // Extract the values to be exported
        var signal = _signalData[stripComboSeries.SelectedIndex][_settings.IndexStart..(_settings.IndexEnd + 1)];

        // Exit if there is no data to be saved
        if (signal is null || signal.Length == 0)
        {
            using (new CenterWinDialog(this))
                MessageBox.Show("There is no data available to be saved.", "No data", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        // Displays a SaveFileDialog, so the user can save the data into a file  
        SaveFileDialog SaveDlg = new()
        {
            DefaultExt = "*.txt",
            Filter = "Text file (*.txt)|*.txt|SignalAnalysis file (*.sig)|*.sig|Binary file (*.bin)|*.bin|All files (*.*)|*.*",
            FilterIndex = 1,
            Title = "Export data",
            OverwritePrompt = true,
            InitialDirectory = _settings.RememberFileDialogPath ? _settings.UserSavePath : _settings.DefaultSavePath
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

    private void Settings_Click(object sender, EventArgs e)
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
        }

    }

    private void About_Click(object sender, EventArgs e)
    {
        
    }

    private void LabelEx_Click(object sender, EventArgs e)
    {
        if (sender is not null && sender is ToolStripStatusLabelEx)
        {
            var label = (ToolStripStatusLabelEx)sender;
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
                    if (label.Checked && statsTask !=null && statsTask.Status == TaskStatus.Running)
                        FrmMain_KeyPress(sender, new KeyPressEventArgs((char)Keys.Escape));
                    break;
                case "LabelExEntropy":
                    _settings.Entropy = label.Checked;
                    if (label.Checked && statsTask != null && statsTask.Status == TaskStatus.Running)
                        FrmMain_KeyPress(sender, new KeyPressEventArgs((char)Keys.Escape));
                    break;
                case "LabelExCrossHair":
                    _settings.CrossHair = label.Checked;
                    break;
            }
        }
    }

}

