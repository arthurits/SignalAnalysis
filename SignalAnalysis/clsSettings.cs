using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalAnalysis
{
    public class clsSettings
    {
        public int IndexStart { get; set; }
        public int IndexEnd { get; set; }
        public bool PowerSpectra { get; set; }
        public bool CumulativeDimension { get; set; }
        public AxisType AxisType { get; set; }

        public clsSettings()
        {
            IndexStart = 0;
            IndexEnd = 0;
            PowerSpectra = true;
            CumulativeDimension = false;
            AxisType = AxisType.Seconds;
        }
    }

    public enum AxisType
    {
        Seconds,
        Points,
        DateTime
    }
}
