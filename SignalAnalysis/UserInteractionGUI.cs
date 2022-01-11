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
            InitialDirectory = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\examples",
            Filter = "ErgoLux files (*.elux)|*.elux|SignalAnalysis files (*.sig)|*.sig|Text files (*.txt)|*.txt|All files (*.*)|*.*",
            FilterIndex = 4,
            RestoreDirectory = true
        };

        using (new CenterWinDialog(this))
            result = openDlg.ShowDialog(this);

        if (result == DialogResult.OK && openDlg.FileName != "")
        {
            //Get the path of specified file
            filePath = openDlg.FileName;

            if (".elux".Equals(Path.GetExtension(filePath), StringComparison.OrdinalIgnoreCase))
                ReadELuxData(filePath);
            else if (".sig".Equals(Path.GetExtension(filePath), StringComparison.OrdinalIgnoreCase))
                ReadSigData(filePath);
            else if (".txt".Equals(Path.GetExtension(filePath), StringComparison.OrdinalIgnoreCase))
                ReadTextData(filePath);

            nPoints = _signalData[0].Length;
            _settings.IndexStart = 0;
            _settings.IndexEnd = _signalData[0].Length - 1;

            PopulateComboSeries();

            this.Text = StringsRM.GetString("strFrmTitle") + " - " + openDlg.FileName;
        }

    }

    private void Export_Click(object sender, EventArgs e)
    {
        // Exit if there is no data to be saved
        if (_signalRange.Length == 0)
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
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
        };

        DialogResult result;
        using (new CenterWinDialog(this))
            result = SaveDlg.ShowDialog(this.Parent);

        // If the file name is not an empty string, call the corresponding routine to save the data into a file.  
        if (result == DialogResult.OK && SaveDlg.FileName != "")
        {
            switch (Path.GetExtension(SaveDlg.FileName).ToLower())
            {
                //case ".elux":
                //    SaveELuxData(SaveDlg.FileName);
                //    break;
                case ".txt":
                    SaveTextData(SaveDlg.FileName, _signalRange.Length, stripComboSeries.SelectedText);
                    break;
                case ".sig":
                    SaveSigData(SaveDlg.FileName, _signalRange.Length, stripComboSeries.SelectedText);
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

            // Update the settings
            switch (label.Name)
            {
                case "LabelExPower":
                    _settings.PowerSpectra = label.Checked;
                    //mnuMainFrm_View_Raw.Checked = label.Checked;
                    break;
                case "LabelExCumulative":
                    _settings.CumulativeDimension = label.Checked;
                    if (label.Checked && statsTask != null)
                        FrmMain_KeyPress(sender, new KeyPressEventArgs((char)Keys.Escape));
                        break;
                case "LabelExEntropy":
                    _settings.Entropy = label.Checked;
                    if (label.Checked && statsTask != null)
                        FrmMain_KeyPress(sender, new KeyPressEventArgs((char)Keys.Escape));
                    break;
                case "LabelExCrossHair":
                    _settings.CrossHair = label.Checked;
                    //mnuMainFrm_View_Ratio.Checked = label.Checked;
                    break;
            }
        }
    }

}

