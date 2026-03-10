using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wedjat.Model.DTO
{
    public class PcbDefectDTO
    {
        /// <summary>
        /// PCB型号编码+名称
        /// </summary>
        public string PcbFullName { get; set; } = string.Empty;

        /// <summary>
        /// 缺陷类型名称
        /// </summary>
        public string DefectTypeName { get; set; } = string.Empty;

        /// <summary>
        /// 该PCB型号下该缺陷的总数量
        /// </summary>
        public int DefectCount { get; set; } = 0;
    }
}
