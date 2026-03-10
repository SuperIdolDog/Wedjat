using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wedjat.Model.Entity;

namespace Wedjat.Model.DTO
{
    public class ScannerDataDTO
    {
        public DateTime ScanTime { get; set; }
        public string ScanContent { get; set; }

        public string Result {  get; set; }

        public ScanStatus ScanStatus { get; set; }

        public string OperatorWorkId { get; set; }
    }
}
