using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wedjat.Model.DTO
{
    public class ReportDefectDataDTO
    {
        /// <summary>
        /// 缺陷类型名称
        /// </summary>
        public string DefectTypeName { get; set; }

        /// <summary>
        /// 缺陷数量
        /// </summary>
        public int DefectCount { get; set; }

        /// <summary>
        /// 缺陷占比（百分比）
        /// </summary>
        public double DefectPercentage { get; set; }

        /// <summary>
        /// 格式化显示文本
        /// </summary>
        public string DisplayText => $"{DefectTypeName}: {DefectCount} ({DefectPercentage:F1}%)";

    }
}
