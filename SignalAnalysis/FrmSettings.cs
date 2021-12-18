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

        public FrmSettings()
        {
            InitializeComponent();
        }

        public FrmSettings(clsSettings settings)
            :this()
        {
            Settings = settings;
            txtStart.Text = Settings.IndexStart.ToString();
            txtEnd.Text = Settings.IndexEnd.ToString();
            chkPower.Checked = Settings.PowerSpectra;
            chkCumulative.Checked = Settings.CumulativeDimension;
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
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtStart.Text, out int num)) return;
            Settings.IndexStart = num;

            if (!int.TryParse(txtEnd.Text, out num)) return;
            Settings.IndexEnd = num;

            Settings.PowerSpectra = chkPower.Checked;
            Settings.CumulativeDimension = chkCumulative.Checked;
            
            Settings.AxisType = AxisType.Seconds;
            if (radPoints.Checked) Settings.AxisType = AxisType.Points;
            if (radTime.Checked) Settings.AxisType = AxisType.DateTime;

            this.DialogResult = DialogResult.OK;
        }
    }
}
