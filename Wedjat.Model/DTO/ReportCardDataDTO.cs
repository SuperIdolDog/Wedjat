using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wedjat.Model.DTO
{
    public class ReportCardDataDTO
    {
        /// <summary>
        /// 工单数（去重后）
        /// </summary>
        public long WorkOrderCount { get; set; } = 0;

        /// <summary>
        /// 累计检测PCB数量
        /// </summary>
        public long TotalInspectionCount { get; set; } = 0;

        /// <summary>
        /// 不良品数量
        /// </summary>
        public long DefectiveCount { get; set; } = 0;

        /// <summary>
        /// 平均合格率（保留2位小数，格式：92.35%）
        /// </summary>
        public double AveragePassRate { get; set; } = 0.00;
    }
}
