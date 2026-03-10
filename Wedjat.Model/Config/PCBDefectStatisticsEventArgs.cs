using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wedjat.Model.Config
{
    public class PCBDefectStatisticsEventArgs : EventArgs
    {
        public string PCBSN { get; set; }
        public string DefectName { get; set; }
        public int DefectCount { get; set; }
    }
}
