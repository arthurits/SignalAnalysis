//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

namespace SignalAnalysis
{
    
    public partial class FrmSettings : Form
    {
        public ClassSettings Settings = new();
        private System.Resources.ResourceManager StringsRM = new("SignalAnalysis.localization.strings", typeof(FrmMain).Assembly);

        public FrmSettings()
        {
            InitializeComponent();
            FillCulture();
            UpdateUI_Language();
        }

        public FrmSettings(ClassSettings settings)
            :this()
        {
            UpdateControls(settings);
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtStart.Text, out int num)) return;
            if (num < 0) return;
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

            if (radCurrentCulture.Checked) Settings.AppCulture = System.Globalization.CultureInfo.CurrentCulture;
            if (radInvariantCulture.Checked) Settings.AppCulture = System.Globalization.CultureInfo.InvariantCulture;
            if (radUserCulture.Checked) Settings.AppCulture = System.Globalization.CultureInfo.CreateSpecificCulture((string)cboAllCultures.SelectedValue);
            Settings.RememberFileDialogPath = chkDlgPath.Checked;
            Settings.DataFormat = txtDataFormat.Text;

            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            DialogResult DlgResult;
            using (new CenterWinDialog(this))
            {
                DlgResult = MessageBox.Show(StringsRM.GetString("strDlgReset", Settings.AppCulture) ?? "Do you want to reset all fields\nto their default values?",
                    StringsRM.GetString("strDlgResetTitle", Settings.AppCulture) ?? "Reset settings?",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);
            }

            if (DlgResult == DialogResult.Yes)
            {
                UpdateControls(new ClassSettings());
            }
        }

        private void radCurrentCulture_CheckedChanged(object sender, EventArgs e)
        {
            if (radCurrentCulture.Checked)
                radCurrentCulture.Text = (StringsRM.GetString("strRadCurrentCulture", Settings.AppCulture) ?? "Current culture formatting") + $" ({Settings.AppCultureName})";
            else
                radCurrentCulture.Text = StringsRM.GetString("strRadCurrentCulture", Settings.AppCulture) ?? "Current culture formatting";
        }

        private void radUserCulture_CheckedChanged(object sender, EventArgs e)
        {
            cboAllCultures.Enabled = radUserCulture.Checked;
        }

        private void cboAllCultures_SelectedValueChanged(object sender, EventArgs e)
        {
            var cbo = sender as ComboBox;
            if (cbo is not null && cbo.Items.Count > 0)
                UpdateUI_Language(System.Globalization.CultureInfo.CreateSpecificCulture((string)cbo.SelectedValue));
        }

        /// <summary>
        /// Updates the form's controls with values from the settings class
        /// </summary>
        /// <param name="settings">Class containing the values to show on the form's controls</param>
        private void UpdateControls(ClassSettings settings)
        {
            Settings = settings;
            txtStart.Text = Settings.IndexStart.ToString();
            txtEnd.Text = Settings.IndexEnd.ToString();
            chkPower.Checked = Settings.PowerSpectra;
            chkCumulative.Checked = Settings.CumulativeDimension;
            chkEntropy.Checked = Settings.Entropy;
            chkCrossHair.Checked = Settings.CrossHair;
            chkDlgPath.Checked = Settings.RememberFileDialogPath;

            switch (Settings.AxisType)
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

            if (Settings.AppCultureName == string.Empty)
                radInvariantCulture.Checked = true;
            else if (Settings.AppCultureName == System.Globalization.CultureInfo.CurrentCulture.Name)
                radCurrentCulture.Checked = true;
            else
                radUserCulture.Checked = true;
            cboAllCultures.SelectedValue = Settings.AppCultureName;

            chkDlgPath.Checked = Settings.RememberFileDialogPath;
            txtDataFormat.Text = Settings.DataFormat;
        }

        private void FillCulture()
        {
            var cultures = System.Globalization.CultureInfo.GetCultures(System.Globalization.CultureTypes.AllCultures);
            //var cultures = System.Globalization.CultureInfo.GetCultures(System.Globalization.CultureTypes.AllCultures & ~System.Globalization.CultureTypes.SpecificCultures);

            // create an arraylist for the locales 
            System.Collections.ArrayList _locales = new ();

            foreach (var culture in cultures)
            {
                // load in the string value 
                _locales.Add(culture);
            }

            // Databind https://social.msdn.microsoft.com/Forums/vstudio/en-US/2ff7d56d-d91d-48b4-815d-cf9356794a69/binding-cultureinfo-to-combobox-lcid-1033-name-french-france-what-is-going-on-?forum=netfxbcl
            cboAllCultures.DisplayMember = "DisplayName";
            cboAllCultures.ValueMember = "Name";
            cboAllCultures.DataSource = _locales;
        }

        /// <summary>
        /// Update the form's interface language
        /// </summary>
        /// <param name="culture">Culture used to display the UI</param>
        private void UpdateUI_Language()
        {
            UpdateUI_Language(Settings.AppCulture);
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

            this.lblStart.Text = StringsRM.GetString("strLblStart", culture);
            this.lblEnd.Text = StringsRM.GetString("strLblEnd", culture);
            this.grpAxis.Text = StringsRM.GetString("strGrpAxis", culture);
            this.radPoints.Text = StringsRM.GetString("strRadPoints", culture);
            this.radSeconds.Text = StringsRM.GetString("strRadSeconds", culture);
            this.radTime.Text = StringsRM.GetString("strRadTime", culture);
            this.chkPower.Text = StringsRM.GetString("strChkPower", culture);
            this.chkCumulative.Text = StringsRM.GetString("strChkCumulative", culture);
            this.chkEntropy.Text = StringsRM.GetString("strChkEntropy", culture);
            this.chkCrossHair.Text = StringsRM.GetString("strChkCrossHair", culture);
            
            this.grpCulture.Text = StringsRM.GetString("strGrpReset", culture);
            this.radCurrentCulture.Text = StringsRM.GetString("stRadCurrentCulture", culture);
            this.radInvariantCulture.Text = StringsRM.GetString("strRadInvariantCulture", culture);
            this.radUserCulture.Text = StringsRM.GetString("strRadUserCulture", culture);
            this.chkDlgPath.Text = StringsRM.GetString("strChkDlgPath", culture);
            this.lblDataFormat.Text = StringsRM.GetString("strLblDataFormat", culture);

            this.btnCancel.Text = StringsRM.GetString("strBtnCancel", culture);
            this.btnAccept.Text = StringsRM.GetString("strBtnAccept", culture);
            this.btnReset.Text = StringsRM.GetString("strBtnReset", culture);

        }

    }
}
