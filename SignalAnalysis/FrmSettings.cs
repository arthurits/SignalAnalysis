using System.Globalization;

namespace SignalAnalysis;

public partial class FrmSettings : Form
{
    private CultureInfo _culture = CultureInfo.CurrentCulture;
    public ClassSettings? Settings;
    private readonly System.Resources.ResourceManager StringsRM = new("SignalAnalysis.localization.strings", typeof(FrmSettings).Assembly);

    public FrmSettings()
    {
        InitializeComponent();
        FillDefinedCultures("SignalAnalysis.localization.strings", typeof(FrmSettings).Assembly);
    }

    public FrmSettings(ClassSettings settings)
        : this()
    {
        Settings = settings;
        _culture = settings.AppCulture;
        UpdateControls(settings);
    }

    private void Accept_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;

        if (!int.TryParse(txtStart.Text, out int num)) return;
        if (num < 0) return;
        if (Settings is null) return;
        
        Settings.IndexStart = num;

        if (!int.TryParse(txtEnd.Text, out num)) return;
        if (num > 0 && num <= Settings.IndexStart) return;
        Settings.IndexEnd = num;

        Settings.PowerSpectra = chkPower.Checked;
        Settings.CumulativeDimension = chkCumulative.Checked;
        Settings.Entropy = chkEntropy.Checked;
        Settings.CrossHair = chkCrossHair.Checked;

        Settings.AxisType = AxisType.Seconds;
        if (radPoints.Checked) Settings.AxisType = AxisType.Points;
        if (radTime.Checked) Settings.AxisType = AxisType.DateTime;

        Settings.RememberFileDialogPath = chkDlgPath.Checked;
        Settings.DataFormat = txtDataFormat.Text;

        Settings.AppCulture = _culture;

        DialogResult = DialogResult.OK;
    }

    private void Cancel_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
    }

    private void Reset_Click(object sender, EventArgs e)
    {
        DialogResult DlgResult;
        using (new CenterWinDialog(this))
        {
            DlgResult = MessageBox.Show(StringsRM.GetString("strDlgReset", _culture) ?? "Do you want to reset all fields\nto their default values?",
                StringsRM.GetString("strDlgResetTitle", _culture) ?? "Reset settings?",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);
        }

        if (DlgResult == DialogResult.Yes)
        {
            UpdateControls(new ClassSettings());
        }
    }

    private void CurrentCulture_CheckedChanged(object sender, EventArgs e)
    {
        if (radCurrentCulture.Checked)
        {
            _culture = System.Globalization.CultureInfo.CurrentCulture;
            UpdateUI_Language();
        }
    }

    private void InvariantCulture_CheckedChanged(object sender, EventArgs e)
    {
        if (radInvariantCulture.Checked)
        {
            _culture = System.Globalization.CultureInfo.InvariantCulture;
            UpdateUI_Language();
        }
    }

    private void UserCulture_CheckedChanged(object sender, EventArgs e)
    {
        cboAllCultures.Enabled = radUserCulture.Checked;
        if (cboAllCultures.Enabled)
        {
            _culture = new((string)cboAllCultures.SelectedValue ?? String.Empty);
            UpdateUI_Language();
        }
    }

    private void AllCultures_SelectedValueChanged(object sender, EventArgs e)
    {
        var cbo = sender as ComboBox;
        if (cbo is not null && cbo.Items.Count > 0 && cbo.SelectedValue is not null)
        {
            _culture = new((string)cbo.SelectedValue);
            UpdateUI_Language();
        }
    }

    /// <summary>
    /// Updates the form's controls with values from the settings class
    /// </summary>
    /// <param name="settings">Class containing the values to show on the form's controls</param>
    private void UpdateControls(ClassSettings settings)
    {
        txtStart.Text = settings.IndexStart.ToString();
        txtEnd.Text = settings.IndexEnd.ToString();
        chkPower.Checked = settings.PowerSpectra;
        chkCumulative.Checked = settings.CumulativeDimension;
        chkEntropy.Checked = settings.Entropy;
        chkCrossHair.Checked = settings.CrossHair;
        chkDlgPath.Checked = settings.RememberFileDialogPath;

        switch (settings.AxisType)
        {
            case AxisType.Seconds:
                radSeconds.Checked = true;
                break;
            case AxisType.Points:
                radPoints.Checked = true;
                break;
            case AxisType.DateTime:
                radTime.Checked = true;
                break;
        }

        if (_culture.Name == string.Empty)
            radInvariantCulture.Checked = true;
        else if (_culture.Name == System.Globalization.CultureInfo.CurrentCulture.Name)
            radCurrentCulture.Checked = true;
        else
        {
            cboAllCultures.SelectedValue = _culture.Name;
            radUserCulture.Checked = true;
        }

        chkDlgPath.Checked = settings.RememberFileDialogPath;
        txtDataFormat.Text = settings.DataFormat;
    }

    /// <summary>
    /// Databind only the cultures found in .resources files for a given type
    /// </summary>
    /// <param name="type">A type from which the resource manager derives all information for finding .resources files</param>
    private void FillDefinedCultures(string baseName, System.Reflection.Assembly assembly)
    {
        string cultureName = _culture.Name;
        var cultures = System.Globalization.GlobalizationUtilities.GetAvailableCultures(baseName, assembly);
        cboAllCultures.DisplayMember = "DisplayName";
        cboAllCultures.ValueMember = "Name";
        cboAllCultures.DataSource = cultures.ToArray();
        cboAllCultures.SelectedValue = cultureName;
    }

    /// <summary>
    /// Update the form's interface language
    /// </summary>
    /// <param name="culture">Culture used to display the UI</param>
    private void UpdateUI_Language()
    {
        UpdateUI_Language(_culture);
    }

    /// <summary>
    /// Update the form's interface language
    /// </summary>
    /// <param name="culture">Culture used to display the UI</param>
    private void UpdateUI_Language(System.Globalization.CultureInfo culture)
    {
        this.Text = StringsRM.GetString("strFrmSettings", culture) ?? "Settings";

        this.tabPlot.Text = StringsRM.GetString("strTabPlot", culture) ?? "Plotting";
        this.tabGUI.Text = StringsRM.GetString("strTabGUI", culture) ?? "User interface";

        this.lblStart.Text = StringsRM.GetString("strLblStart", culture) ?? "Array index start";
        this.lblEnd.Text = StringsRM.GetString("strLblEnd", culture) ?? "Array index end";
        this.grpAxis.Text = StringsRM.GetString("strGrpAxis", culture) ?? "Abscissa axis";
        this.radPoints.Text = StringsRM.GetString("strRadPoints", culture) ?? "Data points";
        this.radSeconds.Text = StringsRM.GetString("strRadSeconds", culture) ?? "Seconds";
        this.radTime.Text = StringsRM.GetString("strRadTime", culture) ?? "Date and time";
        this.chkPower.Text = StringsRM.GetString("strChkPower", culture) ?? "Power (dB)";
        this.chkCumulative.Text = StringsRM.GetString("strChkCumulative", culture) ?? "Cumulative fractal dimension";
        this.chkEntropy.Text = StringsRM.GetString("strChkEntropy", culture) ?? "Entropy (approximate && sample)";
        this.chkCrossHair.Text = StringsRM.GetString("strChkCrossHair", culture) ?? "Show plots' crosshair";

        this.grpCulture.Text = StringsRM.GetString("strGrpCulture", culture) ?? "UI and data format";
        this.radCurrentCulture.Text = (StringsRM.GetString("strRadCurrentCulture", culture) ?? "Current culture formatting") + $" ({System.Globalization.CultureInfo.CurrentCulture.Name})";
        //this.radCurrentCulture.Text = StringsRM.GetString("strRadCurrentCulture", culture) ?? "Current culture formatting";
        this.radInvariantCulture.Text = StringsRM.GetString("strRadInvariantCulture", culture) ?? "Invariant culture formatting";
        this.radUserCulture.Text = StringsRM.GetString("strRadUserCulture", culture) ?? "Select culture";
        this.chkDlgPath.Text = StringsRM.GetString("strChkDlgPath", culture) ?? "Remember open/save dialog previous path";
        this.lblDataFormat.Text = StringsRM.GetString("strLblDataFormat", culture) ?? "Numeric data-formatting string";

        this.btnReset.Text = StringsRM.GetString("strBtnReset", culture) ?? "&Reset";
        this.btnCancel.Text = StringsRM.GetString("strBtnCancel", culture) ?? "&Cancel";
        this.btnAccept.Text = StringsRM.GetString("strBtnAccept", culture) ?? "&Accept";
    }

}