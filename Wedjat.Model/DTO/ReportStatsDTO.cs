using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wedjat.Model.DTO
{
    /// <summary>
    /// 检测统计数据DTO
    /// </summary>
    public class ReportStatsDTO
    {
   

            /// <summary>
            /// 检测总数
            /// </summary>
            public long InspectionCount { get; set; }

            /// <summary>
            /// 合格数
            /// </summary>
            public long OKCount { get; set; }

            /// <summary>
            /// 不合格数
            /// </summary>
            public long NGCount { get; set; }

            /// <summary>
            /// 检测效率（个/小时）
            /// </summary>
            public double Efficiency { get; set; }

            /// <summary>
            /// 时间范围（小时）
            /// </summary>
            public double TimeSpanHours { get; set; }

            /// <summary>
            /// 合格率
            /// </summary>
            public double PassRate { get; set; }

            /// <summary>
            /// 格式化显示文本
            /// </summary>
            public string DisplayText =>
                $"检测总数: {InspectionCount:N0} 片\n" +
                $"合格数: {OKCount:N0} 片\n" +
                $"不合格数: {NGCount:N0} 片\n" +
                $"合格率: {PassRate:F2}%\n" +
                $"{(Efficiency > 0 ? $"平均效率: {Efficiency:F2} 个/小时" : "效率: 无数据")}";
        }
    
}
