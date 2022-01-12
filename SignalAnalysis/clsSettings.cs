using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalAnalysis
{
    public class clsSettings
    {
        public int IndexStart { get; set; } = 0;
        public int IndexEnd { get; set; } = 0;
        public bool PowerSpectra { get; set; } = true;
        public bool CumulativeDimension { get; set; } = false;
        public bool Entropy { get; set; } = false;
        public bool CrossHair { get; set; } = false;
        public AxisType AxisType { get; set; } = AxisType.Seconds;

        public clsSettings()
        {
        }
    }

    public enum AxisType
    {
        Seconds,
        Points,
        DateTime
    }
}
