using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wedjat.Model.Entity;

namespace Wedjat.Model.DTO
{
    public class PcbInspectionPushDTO
    {
        /// <summary>
        /// 当前检测编号
        /// </summary>
        public string InspectionCode { get; set; } = string.Empty;
        /// <summary>
        /// 当前工单编号
        /// </summary>

        public string WorkOrderCode { get; set; } = string.Empty;
        /// <summary>
        /// 当前pcb编号
        /// </summary>

        public string PCBCode { get; set; } = string.Empty;

        /// <summary>
        /// 当前检测的pcb唯一编号
        /// </summary>
        public string PCBSN { get; set; } = string.Empty;
        /// <summary>
        /// 检测时间
        /// </summary>

        public DateTime InspectionTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 是否合格
        /// </summary>

        public bool IsQualified { get; set; }
        /// <summary>
        /// 员工工号
        /// </summary>

        public string WorkId { get; set; } = string.Empty;
        /// <summary>
        /// 工人姓名
        /// </summary>
        public string WorkName { get; set; } = string.Empty;
        /// <summary>
        /// 产线
        /// </summary>
        public string ProductLine { get; set; } = string.Empty;
      /// <summary>
      /// 工单状态
      /// </summary>
        public OrderStatu WorkOrderStatus { get; set; }
        public List<MesInspectionDefect> DetectedDefects { get; set; } = new List<MesInspectionDefect>();


    }

    public class MesInspectionDefect
    {
        public string InspectionCode { get; set; } = string.Empty;
        /// <summary>
        /// 当前缺陷标识
        /// </summary>
        public string DefectCode { get; set; } = string.Empty;
        /// <summary>
        /// 当前缺陷名称
        /// </summary>
        public string DefectName { get; set; } = string.Empty;
        /// <summary>
        /// 当前缺陷数量
        /// </summary>
        public int Count { get; set; } = 0;
    }
}

