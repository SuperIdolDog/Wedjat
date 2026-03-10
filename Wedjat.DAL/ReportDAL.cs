using FreeSql;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Wedjat.Model.DTO;
using Wedjat.Model.Entity;

namespace Wedjat.DAL
{
    public class ReportDAL
    {
        private readonly IFreeSql _fSql;

        public ReportDAL(IFreeSql fSql)
        {
            _fSql = fSql;
        }

        #region 时间粒度通用工具方法
        private (DateTime EffectiveStart, DateTime EffectiveEnd) ProcessTimeParams(
            DateTime? startDate, DateTime? endDate, int defaultRecentDays = 30)
        {
            var now = DateTime.Now;
            var effectiveStart = startDate ?? endDate?.AddDays(-defaultRecentDays) ?? now.AddDays(-defaultRecentDays);
            var effectiveEnd = endDate ?? now;

            if (effectiveStart > effectiveEnd)
            {
                var temp = effectiveStart;
                effectiveStart = effectiveEnd;
                effectiveEnd = temp;
            }

            effectiveStart = effectiveStart.Date;
            effectiveEnd = effectiveEnd.Date.AddDays(1).AddSeconds(-1);

            return (effectiveStart, effectiveEnd);
        }

        private string GetAutoGranularity(double totalDays)
        {
            if (totalDays <= 7)
                return "Day";
            else if (totalDays <= 90)
                return "Week";
            else
                return "Month";
        }

        private List<(DateTime SegmentStart, DateTime SegmentEnd)> GenerateSegments(
            DateTime effectiveStart, DateTime effectiveEnd, string granularity)
        {
            var segments = new List<(DateTime SegmentStart, DateTime SegmentEnd)>();
            var currentStart = effectiveStart;

            while (currentStart <= effectiveEnd)
            {
                DateTime segmentEnd;

                switch (granularity)
                {
                    case "Day":
                        segmentEnd = currentStart.Date.AddDays(1).AddSeconds(-1);
                        break;
                    case "Week":
                        int currentDayOfWeek = (int)currentStart.DayOfWeek;
                        currentDayOfWeek = currentDayOfWeek == 0 ? 7 : currentDayOfWeek;
                        int daysToSunday = 7 - currentDayOfWeek;
                        segmentEnd = currentStart.Date.AddDays(daysToSunday).AddDays(1).AddSeconds(-1);
                        break;
                    case "Month":
                        segmentEnd = new DateTime(currentStart.Year, currentStart.Month, 1)
                            .AddMonths(1)
                            .AddSeconds(-1);
                        break;
                    default:
                        segmentEnd = currentStart.Date.AddDays(1).AddSeconds(-1);
                        break;
                }

                if (segmentEnd > effectiveEnd)
                    segmentEnd = effectiveEnd;

                segments.Add((currentStart, segmentEnd));

                switch (granularity)
                {
                    case "Day":
                        currentStart = segmentEnd.AddSeconds(1);
                        break;
                    case "Week":
                        currentStart = segmentEnd.AddSeconds(1);
                        break;
                    case "Month":
                        currentStart = new DateTime(currentStart.Year, currentStart.Month, 1).AddMonths(1);
                        break;
                    default:
                        currentStart = segmentEnd.AddSeconds(1);
                        break;
                }
            }

            return segments;
        }

        private DateTime GetSegmentGroupKey(DateTime date, string granularity)
        {
            switch (granularity)
            {
                case "Day":
                    return date.Date;
                case "Week":
                    return date.Date.AddDays(-(int)date.DayOfWeek + 1);
                case "Month":
                    return new DateTime(date.Year, date.Month, 1);
                default:
                    return date.Date;
            }
        }

        private string GetSegmentDisplayName(DateTime segmentStart, string granularity)
        {
            switch (granularity)
            {
                case "Day":
                    return segmentStart.ToString("MM-dd");
                case "Week":
                    return $"第{GetWeekOfYear(segmentStart)}周";
                case "Month":
                    return segmentStart.ToString("yyyy-MM");
                default:
                    return segmentStart.ToString("MM-dd");
            }
        }

        private int GetWeekOfYear(DateTime date)
        {
            return new GregorianCalendar()
                .GetWeekOfYear(
                    date,
                    CalendarWeekRule.FirstFourDayWeek,
                    DayOfWeek.Monday
                );
        }
        #endregion

        #region 基础查询
        public ISelect<PCBInspection> BaseQuery(string workOrderCode, DateTime? startDate, DateTime? endDate)
        {
            var query = _fSql.Select<PCBInspection>();

            if (!string.IsNullOrEmpty(workOrderCode))
            {
                query = query.Where(a => a.WorkOrderCode.Contains(workOrderCode));
            }

            if (startDate.HasValue)
            {
                query = query.Where(a => a.InspectionTime >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(a => a.InspectionTime <= endDate.Value);
            }

            return query;
        }
        #endregion

        #region 获取报表卡片数据
        public async Task<ReportCardDataDTO> GetCardData(string workOrderCode, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var (effectiveStart, effectiveEnd) = ProcessTimeParams(startDate, endDate);
                var baseQuery = BaseQuery(workOrderCode, startDate, endDate);

                var workOrderCount = await baseQuery
                    .GroupBy(a => a.WorkOrderCode)
                    .CountAsync();

                var totalInspectedCount = await baseQuery.CountAsync();

                var defectiveCount = await baseQuery
                    .Where(a => !a.IsQualified)
                    .CountAsync();

                var okCount = totalInspectedCount - defectiveCount;

                double averageQualifiedRate = 0;
                if (totalInspectedCount > 0)
                {
                    averageQualifiedRate = (double)okCount / totalInspectedCount * 100;
                }

                return new ReportCardDataDTO
                {
                    WorkOrderCount = workOrderCount,
                    TotalInspectionCount = totalInspectedCount,
                    DefectiveCount = defectiveCount,
                    AveragePassRate = Math.Round(averageQualifiedRate, 2)
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"获取卡片数据时发生错误: {ex.Message}", ex);
            }
        }
        #endregion

        #region 获取检测趋势数据
        public async Task<List<ReportTrendDataDTO>> GetTrendData(
            string workOrderCode,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            try
            {
                // 1. 处理时间参数
                var (effectiveStart, effectiveEnd) = ProcessTimeParams(startDate, endDate);
                var totalDays = (effectiveEnd - effectiveStart).TotalDays;
                var granularity = GetAutoGranularity(totalDays);
                var segments = GenerateSegments(effectiveStart, effectiveEnd, granularity);

                // 2. 核心修复：先查询完整实体到内存
                var allData = await BaseQuery(workOrderCode, effectiveStart, effectiveEnd)
                    .ToListAsync();

                // 3. 内存中筛选需要的字段 + 分组
                var rawData = allData
                    .Select(a => new
                    {
                        a.InspectionTime,
                        a.IsQualified
                    })
                    .ToList();

                var groupedData = rawData
                    .GroupBy(a => GetSegmentGroupKey(a.InspectionTime, granularity))
                    .Select(g => new
                    {
                        GroupKey = g.Key,
                        TotalCount = g.Count(),
                        OKCount = g.Count(x => x.IsQualified),
                        NGCount = g.Count(x => !x.IsQualified)
                    })
                    .ToList();

                // 4. 组装结果
                var result = new List<ReportTrendDataDTO>();
                foreach (var segment in segments)
                {
                    var groupKey = GetSegmentGroupKey(segment.SegmentStart, granularity);
                    var currentStats = groupedData.FirstOrDefault(x => x.GroupKey == groupKey);

                    result.Add(new ReportTrendDataDTO
                    {
                        PeriodStart = segment.SegmentStart,
                        PeriodEnd = segment.SegmentEnd,
                        PeriodName = GetSegmentDisplayName(segment.SegmentStart, granularity),
                        TotalCount = currentStats?.TotalCount ?? 0,
                        OKCount = currentStats?.OKCount ?? 0,
                        NGCount = currentStats?.NGCount ?? 0
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"获取趋势数据时发生错误: {ex.Message}", ex);
            }
        }
        #endregion

        #region 获取缺陷分布数据（修复缺陷名称显示）
        public async Task<List<ReportDefectDataDTO>> GetDefectData(
            string workOrderCode,
            DateTime? startDate,
            DateTime? endDate)
        {
            try
            {
                // 1. 获取检测记录
                var inspectionData = await BaseQuery(workOrderCode, startDate, endDate)
                    .ToListAsync();

                var validInspectionCodes = inspectionData
                    .Select(a => a.InspectionCode)
                    .Distinct()
                    .ToList();

                if (!validInspectionCodes.Any())
                    return new List<ReportDefectDataDTO>();

                // 2. 查询缺陷明细
                var defectData = await _fSql.Select<PCBDefectDetail>()
                    .Where(d => validInspectionCodes.Contains(d.InspectionCode))
                    .ToListAsync();

                if (!defectData.Any())
                    return new List<ReportDefectDataDTO>();

                // 3. 按缺陷类型分组统计
                var groupedDefects = defectData
                    .Where(d => !string.IsNullOrEmpty(d.DefectTypeName)) // 过滤空名称
                    .GroupBy(d => d.DefectTypeName.Trim()) // 去除前后空格
                    .Select(g => new
                    {
                        DefectTypeName = g.Key,
                        DefectCount = g.Sum(x => x.DefectCount)
                    })
                    .ToList();

                // 4. 计算总数和百分比
                var totalDefects = groupedDefects.Sum(g => g.DefectCount);

                var result = groupedDefects
                    .Select(g => new ReportDefectDataDTO
                    {
                        DefectTypeName = g.DefectTypeName,
                        DefectCount = g.DefectCount,
                        DefectPercentage = totalDefects > 0 ?
                            (double)g.DefectCount / totalDefects * 100 : 0
                    })
                    .OrderByDescending(x => x.DefectCount)
                    .ThenBy(x => x.DefectTypeName) // 按名称排序
                    .ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"获取缺陷数据时发生错误: {ex.Message}", ex);
            }
        }
        #endregion
        #region 获取检测统计数据（检测总数 + 效率）
        public async Task<ReportStatsDTO> GetInspectionStats(
      string workOrderCode,
      DateTime? startDate,
      DateTime? endDate)
        {
            try
            {
                var (effectiveStart, effectiveEnd) = ProcessTimeParams(startDate, endDate);

                // 获取检测总数+合格/不合格数
                var baseQuery = BaseQuery(workOrderCode, effectiveStart, effectiveEnd);
                var inspectionCount = await baseQuery.CountAsync();
                var okCount = await baseQuery.Where(a => a.IsQualified).CountAsync();
                var ngCount = await baseQuery.Where(a => !a.IsQualified).CountAsync();

                // 计算合格率
                double passRate = 0;
                if (inspectionCount > 0)
                {
                    passRate = (double)okCount / inspectionCount * 100;
                }

                // 计算时间跨度和效率
                TimeSpan timeSpan = effectiveEnd - effectiveStart;
                double totalHours = timeSpan.TotalHours;
                double efficiency = totalHours > 0 && inspectionCount > 0
                    ? inspectionCount / totalHours
                    : 0;

                return new ReportStatsDTO
                {
                    InspectionCount = inspectionCount,
                    OKCount = okCount,       // 补充赋值
                    NGCount = ngCount,       // 补充赋值
                    PassRate = Math.Round(passRate, 2), // 补充赋值
                    Efficiency = Math.Round(efficiency, 2),
                    TimeSpanHours = Math.Round(totalHours, 2)
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"获取检测统计数据时发生错误: {ex.Message}", ex);
            }
        }
        #endregion

        #region 获取设备检测效率（兼容旧版本）
        public async Task<double> GetInspectionEfficiency(
            string workOrderCode,
            DateTime? startDate,
            DateTime? endDate)
        {
            try
            {
                var stats = await GetInspectionStats(workOrderCode, startDate, endDate);
                return stats.Efficiency;
            }
            catch (Exception ex)
            {
                throw new Exception($"获取检测效率时发生错误: {ex.Message}", ex);
            }
        }
        #endregion
    }
}