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
                throw new Exception("No generic text file reader has yet been implemented.");

            nPoints = _signalData[0].Length;
            _settings.IndexStart = 0;
            _settings.IndexEnd = _signalData[0].Length - 1;

            PopulateCboSeries();

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
            DefaultExt = "*.elux",
            Filter = "Text file (*.txt)|*.txt|Binary file (*.bin)|*.bin|All files (*.*)|*.*",
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
                    SaveTextData(SaveDlg.FileName, _signalRange.Length, cboSeries.SelectedText);
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
            cboSeries_SelectedIndexChanged(this, EventArgs.Empty);
        }

    }

    private void About_Click(object sender, EventArgs e)
    {
        
    }

    private void LabelExPower_Click(object sender, EventArgs e)
    {

    }
}

