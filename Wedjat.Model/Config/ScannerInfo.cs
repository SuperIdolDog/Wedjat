using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wedjat.Model.Config
{
    public class ScannerInfo
    {
        public string PortName { get; set; } 
        public int BaudRate { get; set; } 
        public Parity Parity { get; set; } 
        public int DataBits { get; set; } 
        public StopBits StopBits { get; set; }

        public string MesAPI { get; set; }  

        public int OverTime { get; set; }

        public DateTime LastScannerTime { get; set; }
    }
}
