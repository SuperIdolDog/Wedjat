using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wedjat.Model.Entity;

namespace Wedjat.Model.Config
{
   public class LogEventArgs : EventArgs
    {
        public LogType LogType { get; set; }
        public string Message { get; set; }
        public LogStatu Status { get; set; }
    }
}
