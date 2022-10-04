using ScottPlot;
using System.Globalization;

namespace SignalAnalysis;

public partial class FrmLanguage : Form
{
    private ClassSettings _settings = new();
    private readonly System.Resources.ResourceManager StringsRM = new("SignalAnalysis.localization.strings", typeof(FrmLanguage).Assembly);

    public FrmLanguage()
    {
        InitializeComponent();
        FillDefinedCultures("SignalAnalysis.localization.strings", typeof(FrmLanguage).Assembly);
        UpdateUI_Language();
    }

    public FrmLanguage(ClassSettings settings)
    : this()
    {
        UpdateControls(settings);
    }

    private void Accept_Click(object sender, EventArgs e)
    {
        this.DialogResult = DialogResult.OK;
    }

    private void Cancel_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
    }

    private void CurrentCulture_CheckedChanged(object sender, EventArgs e)
    {
        if (radCurrentCulture.Checked)
        {
            _settings.AppCulture = System.Globalization.CultureInfo.CurrentCulture;
            UpdateUI_Language();
        }
    }

    private void InvariantCulture_CheckedChanged(object sender, EventArgs e)
    {
        if (radInvariantCulture.Checked)
        {
            _settings.AppCulture = System.Globalization.CultureInfo.InvariantCulture;
            UpdateUI_Language();
        }
    }

    private void UserCulture_CheckedChanged(object sender, EventArgs e)
    {
        cboAllCultures.Enabled = radUserCulture.Checked;
        if (cboAllCultures.Enabled)
        {
            _settings.AppCulture = new((string)cboAllCultures.SelectedValue ?? String.Empty);
            UpdateUI_Language();
        }
    }

    private void AllCultures_SelectedValueChanged(object sender, EventArgs e)
    {
        var cbo = sender as ComboBox;
        if (cbo is not null && cbo.Items.Count > 0 && cbo.SelectedValue is not null)
        {
            _settings.AppCulture = new((string)cbo.SelectedValue);
            UpdateUI_Language();
        }
    }

    /// <summary>
    /// Updates the form's controls with values from the settings class
    /// </summary>
    /// <param name="settings">Class containing the values to show on the form's controls</param>
    private void UpdateControls(ClassSettings settings)
    {
        _settings = settings;

        cboAllCultures.Enabled = false;
        if (_settings.AppCultureName == string.Empty)
            radInvariantCulture.Checked = true;
        else if (_settings.AppCultureName == System.Globalization.CultureInfo.CurrentCulture.Name)
            radCurrentCulture.Checked = true;
        else
        {
            cboAllCultures.SelectedValue = _settings.AppCultureName;
            radUserCulture.Checked = true;
        }
        
    }

    /// <summary>
    /// Databind only the cultures found in .resources files for a given type
    /// </summary>
    /// <param name="type">A type from which the resource manager derives all information for finding .resources files</param>
    private void FillDefinedCultures(string baseName, System.Reflection.Assembly assembly)
    {
        string cultureName = _settings.AppCultureName;
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
        UpdateUI_Language(_settings.AppCulture);
    }

    /// <summary>
    /// Update the form's interface language
    /// </summary>
    /// <param name="culture">Culture used to display the UI</param>
    private void UpdateUI_Language(System.Globalization.CultureInfo culture)
    {
        this.Text = StringsRM.GetString("strFrmLanguage", culture) ?? "Select culture";
        this.radCurrentCulture.Text = (StringsRM.GetString("strRadCurrentCulture", culture) ?? "Current culture formatting") + $" ({System.Globalization.CultureInfo.CurrentCulture.Name})";
        this.radInvariantCulture.Text = StringsRM.GetString("strRadInvariantCulture", culture) ?? "Invariant culture formatting";
        this.radUserCulture.Text = StringsRM.GetString("strRadUserCulture", culture) ?? "Select culture";
        this.btnCancel.Text = StringsRM.GetString("strBtnCancel", culture) ?? "&Cancel";
        this.btnAccept.Text = StringsRM.GetString("strBtnAccept", culture) ?? "&Accept";
    }

}
