using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wedjat.Model.Config
{
    public class ScannerStatusEventArgs : EventArgs
    {
        public bool IsConnect { get; set; }
        public bool IsConnected { get; set; }
    }
}
