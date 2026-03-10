// Wedjat.BLL/ReportBLL.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wedjat.DAL;
using Wedjat.Model;
using Wedjat.Model.DTO;

namespace Wedjat.BLL
{
    public class ReportBLL
    {
        private readonly ReportDAL _reportDAL;

        public ReportBLL() : base()
        {
            _reportDAL = new ReportDAL(AppDbContext.Sqlite);
        }

        #region 卡片数据
        public async Task<ReportCardDataDTO> GetCardData(
            string workOrderCode,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            try
            {
                return await _reportDAL.GetCardData(workOrderCode, startDate, endDate);
            }
            catch (Exception ex)
            {
                throw new Exception($"BLL: 获取卡片数据失败 - {ex.Message}", ex);
            }
        }
        #endregion

        #region 趋势数据
        public async Task<List<ReportTrendDataDTO>> GetTrendData(
            string workOrderCode,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            try
            {
                return await _reportDAL.GetTrendData(workOrderCode, startDate, endDate);
            }
            catch (Exception ex)
            {
                throw new Exception($"BLL: 获取趋势数据失败 - {ex.Message}", ex);
            }
        }
        #endregion

        #region 缺陷数据
        public async Task<List<ReportDefectDataDTO>> GetDefectData(
            string workOrderCode,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            try
            {
                return await _reportDAL.GetDefectData(workOrderCode, startDate, endDate);
            }
            catch (Exception ex)
            {
                throw new Exception($"BLL: 获取缺陷数据失败 - {ex.Message}", ex);
            }
        }
        #endregion

        #region 检测统计数据（新方法）
        public async Task<ReportStatsDTO> GetInspectionStats(
            string workOrderCode,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            try
            {
                return await _reportDAL.GetInspectionStats(workOrderCode, startDate, endDate);
            }
            catch (Exception ex)
            {
                throw new Exception($"BLL: 获取检测统计数据失败 - {ex.Message}", ex);
            }
        }
        #endregion

        #region 检测效率（兼容旧方法）
        public async Task<double> GetInspectionEfficiency(
            string workOrderCode,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            try
            {
                var stats = await _reportDAL.GetInspectionStats(workOrderCode, startDate, endDate);
                return stats.Efficiency;
            }
            catch (Exception ex)
            {
                throw new Exception($"BLL: 获取检测效率失败 - {ex.Message}", ex);
            }
        }
        #endregion
    }
}