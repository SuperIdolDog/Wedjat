using Wedjat.MiniMES.Model;

namespace Wedjat.MiniMES.DTO
{
    public class ScadaToMes
    {
        /// <summary>
        /// 当前检测编号
        /// </summary>
        public string InspectionCode { get; set; } = null!;
        /// <summary>
        /// 当前工单编号
        /// </summary>

        public string WorkOrderCode { get; set; } = null!;
        /// <summary>
        /// 当前pcb编号
        /// </summary>

        public string PCBCode { get; set; } = null!;

        /// <summary>
        /// 当前检测的pcb唯一编号
        /// </summary>
        public string PCBSN { get; set; } = null!;
        /// <summary>
        /// 检测时间
        /// </summary>

        public DateTime InspectionTime { get; set; }
        /// <summary>
        /// 是否合格
        /// </summary>

        public bool IsQualified { get; set; }
        /// <summary>
        /// 员工工号
        /// </summary>

        public string WorkId { get; set; } = null!;
        /// <summary>
        /// 工人姓名
        /// </summary>
        public string WorkName { get; set; }=null!;
        /// <summary>
        /// 产线
        /// </summary>
        public string ProductLine { get; set; } = null!;
        /// <summary>
        /// 工单状态
        /// </summary>
        public OrderStatus WorkOrderStatus { get; set; }


        public List<MesInspectionDefect> DetectedDefects { get; set; } = new List<MesInspectionDefect>();


    }

    public class MesInspectionDefect
    {
        public string InspectionCode { get; set; } = null!;
        /// <summary>
        /// 当前缺陷标识
        /// </summary>
        public string DefectCode { get; set; } = null!;
        /// <summary>
        /// 当前缺陷名称
        /// </summary>
        public string DefectName { get; set; } = null!;
        /// <summary>
        /// 当前缺陷数量
        /// </summary>
        public int Count { get; set; } = 0;
    }
}
