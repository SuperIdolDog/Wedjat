using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wedjat.Model.DTO
{
    /// <summary>
    /// PCB类型统计数据DTO
    /// </summary>
    public class PCBTypeStatDTO
    {
        public string PCBCode { get; set; }
        public string PCBName { get; set; }
        public int TotalCount { get; set; }
        public int QualifiedCount { get; set; }
        public int DefectiveCount { get; set; }
        public double QualifiedRate { get; set; }
    }
}
