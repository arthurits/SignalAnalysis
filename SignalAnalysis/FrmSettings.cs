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
        public clsSettings Settings = new();
        private System.Resources.ResourceManager StringsRM = new("SignalAnalysis.localization.strings", typeof(FrmMain).Assembly);

        public FrmSettings()
        {
            InitializeComponent();
            UpdateUI_Language();
        }

        public FrmSettings(clsSettings settings)
            :this()
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
            else
                radCurrentCulture.Checked = true;

            lblCulture.Text = Settings.AppCulture.Name;
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

            Settings.RememberFileDialogPath = chkDlgPath.Checked;
            if (radCurrentCulture.Checked) Settings.AppCulture = System.Globalization.CultureInfo.CurrentCulture;
            else Settings.AppCulture = System.Globalization.CultureInfo.InvariantCulture;

            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void UpdateUI_Language()
        {
            this.Text = StringsRM.GetString("strFrmSettings");
            this.lblStart.Text = StringsRM.GetString("strLblStart");
            this.lblEnd.Text = StringsRM.GetString("strLblEnd");
            this.grpAxis.Text = StringsRM.GetString("strGrpAxis");
            this.radPoints.Text = StringsRM.GetString("strRadPoints");
            this.radSeconds.Text = StringsRM.GetString("strRadSeconds");
            this.radTime.Text = StringsRM.GetString("strRadTime");
            this.chkPower.Text = StringsRM.GetString("strChkPower");
            this.chkCumulative.Text = StringsRM.GetString("strChkCumulative");
            this.chkEntropy.Text = StringsRM.GetString("strChkEntropy");
            this.chkCrossHair.Text = StringsRM.GetString("strChkCrossHair");
            this.btnCancel.Text = StringsRM.GetString("strBtnCancel");
            this.btnAccept.Text = StringsRM.GetString("strBtnAccept");

        }
    }
}
