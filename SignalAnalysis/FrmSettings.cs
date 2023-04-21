using System.Data;
using System.Globalization;

namespace SignalAnalysis;

public partial class FrmSettings : Form
{
    private CultureInfo _culture = CultureInfo.CurrentCulture;
    private readonly ClassSettings? Settings;
    private readonly string _baseName = "SignalAnalysis.localization.strings";
    private int _derivativeAlgorithm;
    private int _integrationAlgorithm;

    public FrmSettings()
    {
        InitializeComponent();
        FillDefinedCultures(_baseName, typeof(FrmSettings).Assembly);
    }

    public FrmSettings(ClassSettings settings)
        : this()
    {
        Settings = settings;
        _culture = settings.AppCulture;
        _derivativeAlgorithm = (int)settings.DerivativeAlgorithm;
        _integrationAlgorithm = (int)settings.IntegrationAlgorithm;
        UpdateControls(settings);
    }

    private void Accept_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.None;

        if (Settings is null) return;

        if (!int.TryParse(txtStart.Text, out int num)) return;
        if (num < 0)
        {
            txtStart.SelectionStart = 0;
            txtStart.SelectionLength = txtStart.Text.Length;
            txtStart.Focus();
            return;
        }
        Settings.IndexStart = num;

        if (!int.TryParse(txtEnd.Text, out num)) return;
        //if (num > 0 && num <= Settings.IndexStart) return;
        if (num < Settings.IndexStart || num > Settings.IndexMax)
        {
            txtEnd.SelectionStart = 0;
            txtEnd.SelectionLength = txtEnd.Text.Length;
            txtEnd.Focus();
            return;
        }
        Settings.IndexEnd = num;

        Settings.Boxplot = chkBoxplot.Checked;
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

        // Differentiation
        Settings.ComputeDerivative = chkComputeDerivative.Checked;
        Settings.AbsoluteIntegral = chkAbsolute.Checked;
        Settings.ExportDerivative = chkExportDerivative.Checked;
        Settings.DerivativeAlgorithm = (DerivativeMethod)_derivativeAlgorithm;

        // Integration
        Settings.ComputeIntegration = chkComputeIntegration.Checked;
        Settings.AbsoluteIntegral = chkAbsolute.Checked;
        Settings.ExportIntegration = chkExportIntegration.Checked;
        Settings.IntegrationAlgorithm = (IntegrationMethod)_integrationAlgorithm;

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
            DlgResult = MessageBox.Show(this,
                StringResources.DlgReset,
                StringResources.DlgResetTitle,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);
        }

        if (DlgResult == DialogResult.Yes)
        {
            UpdateControls(new ClassSettings());
        }
    }

    private void ComputeDerivative_CheckedChanged(object sender, EventArgs e)
    {
        chkExportDerivative.Enabled = chkComputeDerivative.Checked;
        cboAlgorithms.Enabled = chkComputeDerivative.Checked;
        lblAlgorithms.Enabled = chkComputeDerivative.Checked;
    }

    private void ComputeIntegration_CheckedChanged(object sender, EventArgs e)
    {
        chkAbsolute.Enabled = chkComputeIntegration.Checked;
        chkExportIntegration.Enabled = chkComputeIntegration.Checked;
        cboIntegration.Enabled = chkComputeIntegration.Checked;
        lblIntegration.Enabled = chkComputeIntegration.Checked;
    }

    private void CurrentCulture_CheckedChanged(object sender, EventArgs e)
    {
        if (radCurrentCulture.Checked)
        {
            _culture = System.Globalization.CultureInfo.CurrentCulture;
            UpdateUI_Language();

            int index = cboAllCultures.SelectedIndex;
            FillDefinedCultures(_baseName, typeof(FrmSettings).Assembly);
            cboAllCultures.SelectedIndex = index;
        }
    }

    private void InvariantCulture_CheckedChanged(object sender, EventArgs e)
    {
        if (radInvariantCulture.Checked)
        {
            _culture = System.Globalization.CultureInfo.InvariantCulture;
            UpdateUI_Language();

            int index = cboAllCultures.SelectedIndex;
            FillDefinedCultures(_baseName, typeof(FrmSettings).Assembly);
            cboAllCultures.SelectedIndex = index;
        }
    }

    private void UserCulture_CheckedChanged(object sender, EventArgs e)
    {
        cboAllCultures.Enabled = radUserCulture.Checked;
        if (cboAllCultures.Enabled && cboAllCultures.SelectedValue is not null)
        {
            _culture = new((string)cboAllCultures.SelectedValue);
            if (_culture.Name != string.Empty)
                UpdateUI_Language();

            int index = cboAllCultures.SelectedIndex;
            FillDefinedCultures(_baseName, typeof(FrmSettings).Assembly);
            cboAllCultures.SelectedIndex = index;
        }
    }

    private void AllCultures_SelectionChangeCommitted(object sender, EventArgs e)
    {
        var cbo = sender as ComboBox;
        if (cbo is not null && cbo.Items.Count > 0 && cbo.SelectedValue is not null)
        {
            _culture = new((string)cbo.SelectedValue);
            UpdateUI_Language();

            FillDefinedCultures(_baseName, typeof(FrmSettings).Assembly);
        }
    }

    private void Differentiation_SelectionChangeCommitted(object sender, EventArgs e)
    {
        var cbo = sender as ComboBox;
        if (cbo is not null && cbo.Items.Count > 0 && cbo.SelectedIndex > -1)
            _derivativeAlgorithm = cbo.SelectedIndex;
    }

    private void Integration_SelectionChangeCommitted(object sender, EventArgs e)
    {
        var cbo = sender as ComboBox;
        if (cbo is not null && cbo.Items.Count > 0 && cbo.SelectedIndex > -1)
            _integrationAlgorithm = cbo.SelectedIndex;
    }

    /// <summary>
    /// Updates the form's controls with values from the settings class
    /// </summary>
    /// <param name="settings">Class containing the values to show on the form's controls</param>
    private void UpdateControls(ClassSettings settings)
    {
        txtStart.Text = settings.IndexStart.ToString();
        txtEnd.Text = settings.IndexEnd.ToString();
        chkBoxplot.Checked = settings.Boxplot;
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

        chkComputeDerivative.Checked = settings.ComputeDerivative;
        chkExportDerivative.Checked = settings.ExportDerivative;
        lblAlgorithms.Enabled = settings.ComputeDerivative;
        cboAlgorithms.Enabled = settings.ComputeDerivative;

        chkComputeIntegration.Checked = settings.ComputeIntegration;
        chkAbsolute.Checked = settings.AbsoluteIntegral;
        chkExportIntegration.Checked = settings.ExportIntegration;
        lblIntegration.Enabled = settings.ComputeIntegration;
        cboIntegration.Enabled = settings.ComputeIntegration;
        FillAlgorithms();

    }

    /// <summary>
    /// Databind the algorithms to the combobox
    /// </summary>
    private void FillAlgorithms()
    {
        if (Settings is null) return;

        // Get the all the differentiation algorithms
        string[] strAlgorithms = StringResources.DifferentiationAlgorithms.Split(", ");

        cboAlgorithms.DisplayMember = "DisplayName";
        cboAlgorithms.ValueMember = "Value";
        cboAlgorithms.DataSource = strAlgorithms;
        cboAlgorithms.SelectedIndex = _derivativeAlgorithm;

        // Get the all the integration algorithms
        strAlgorithms = StringResources.IntegrationAlgorithms.Split(", ");
        cboIntegration.DisplayMember = "DisplayName";
        cboIntegration.ValueMember = "Value";
        cboIntegration.DataSource = strAlgorithms;
        cboIntegration.SelectedIndex = _integrationAlgorithm;
    }

    /// <summary>
    /// Databind only the cultures found in .resources files for a given type
    /// </summary>
    /// <param name="type">A type from which the resource manager derives all information for finding .resources files</param>
    private void FillDefinedCultures(string baseName, System.Reflection.Assembly assembly)
    {
        string cultureName = _culture.Name;
        //string _cultureUI = CultureInfo.CurrentUICulture.Name;

        // Retrieve the culture list using the culture currently selected. The UI culture needs to be temporarily changed
        CultureInfo.CurrentUICulture = new CultureInfo(cultureName);
        var cultures = System.Globalization.GlobalizationUtilities.GetAvailableCultures(baseName, assembly);

        cboAllCultures.DisplayMember = "DisplayName";
        cboAllCultures.ValueMember = "Name";
        cboAllCultures.DataSource = cultures.ToArray();
        cboAllCultures.SelectedValue = cultureName;

        // Reset the UI culture to its previous value
        //CultureInfo.CurrentUICulture = new(_cultureUI);
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
        StringResources.Culture = culture;

        this.Text = StringResources.FrmSettings; ;

        this.tabPlot.Text = StringResources.TabPlot;
        this.tabGUI.Text = StringResources.TabGUI;
        this.tabDerivative.Text = StringResources.TabDerivative;
        this.tabIntegration.Text = StringResources.TabIntegration;

        this.lblStart.Text = StringResources.LblStart;
        this.lblEnd.Text = String.Format(StringResources.LblEnd, Settings?.IndexMax);
        this.grpAxis.Text = StringResources.GrpAxis;
        this.radPoints.Text = StringResources.RadPoints;
        this.radSeconds.Text = StringResources.RadSeconds;
        this.radTime.Text = StringResources.RadTime;
        this.chkBoxplot.Text = StringResources.ChkBoxplot;
        this.chkPower.Text = StringResources.ChkPower;
        this.chkCumulative.Text = StringResources.ChkCumulative;
        this.chkEntropy.Text = StringResources.ChkEntropy;
        this.chkCrossHair.Text = StringResources.ChkCrossHair;

        this.grpCulture.Text = StringResources.GrpCulture;
        this.radCurrentCulture.Text = StringResources.RadCurrentCulture + $" ({System.Globalization.CultureInfo.CurrentCulture.Name})";
        this.radInvariantCulture.Text = StringResources.RadInvariantCulture;
        this.radUserCulture.Text = StringResources.RadUserCulture;
        this.chkDlgPath.Text = StringResources.ChkDlgPath;
        this.lblDataFormat.Text = StringResources.LblDataFormat;

        this.chkComputeDerivative.Text = StringResources.ChkComputeDerivative;
        this.chkExportDerivative.Text = StringResources.ChkExportDerivative;
        this.lblAlgorithms.Text = StringResources.GrpAlgorithms;
        this.chkComputeIntegration.Text = StringResources.ChkComputeIntegration;
        this.chkAbsolute.Text = StringResources.ChkAbsoluteIntegral;
        this.chkExportIntegration.Text = StringResources.ChkExportIntegration;
        this.lblIntegration.Text = StringResources.LblIntegration;
        FillAlgorithms();

        this.btnReset.Text = StringResources.BtnReset;
        this.btnCancel.Text = StringResources.BtnCancel;
        this.btnAccept.Text = StringResources.BtnAccept;

        // Relocate controls
        RelocateControls();
    }

    /// <summary>
    /// Relocate controls to compensate for the culture text length in labels
    /// </summary>
    private void RelocateControls()
    {
        int width = Math.Max(this.lblStart.Width, this.lblEnd.Width);
        this.txtStart.Left = this.lblStart.Left + width;
        this.txtEnd.Left = this.lblEnd.Left + width;
        this.lblStart.Top = this.txtStart.Top + (txtStart.Height - lblStart.Height) / 2;
        this.lblEnd.Top = this.txtEnd.Top + (txtEnd.Height - lblEnd.Height) / 2;

        this.txtDataFormat.Left = this.lblDataFormat.Left + this.lblDataFormat.Width;
        this.lblDataFormat.Top = this.txtDataFormat.Top + (txtDataFormat.Height - lblDataFormat.Height) / 2;
    }

}
