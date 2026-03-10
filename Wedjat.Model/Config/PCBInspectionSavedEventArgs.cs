using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wedjat.Model.Entity;

namespace Wedjat.Model.Config
{
    public class PCBInspectionSavedEventArgs : EventArgs
    {
        public PCBInspection Inspection { get; set; }
    }
}
