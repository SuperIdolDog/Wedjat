using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wedjat.Model.DTO
{
    public class ReportTrendDataDTO
    {
        /// <summary>
        /// 分段开始时间
        /// </summary>
        public DateTime PeriodStart { get; set; }

        /// <summary>
        /// 分段结束时间
        /// </summary>
        public DateTime PeriodEnd { get; set; }

        /// <summary>
        /// 分段名称（如：12-01、第48周、2025-09）
        /// </summary>
        public string PeriodName { get; set; }

        public int TotalCount { get; set; } // 总数
        public int OKCount { get; set; } // OK数
        public int NGCount { get; set; } // NG数
        /// <summary>
        /// 该分段合格率（保留2位小数）
        /// </summary>
        public double PassRate { get; set; }

        /// <summary>
        /// 计算合格率的便捷方法
        /// </summary>
        public void CalculatePassRate()
        {
            PassRate = TotalCount == 0 ? 0 : Math.Round((double)OKCount / TotalCount * 100, 2);
        }
    }
}
