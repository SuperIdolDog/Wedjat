using System.ComponentModel.DataAnnotations.Schema;

namespace Wedjat.MiniMES.Model
{
   public enum OrderStatus
    {
        Pending,    // 待处理
        Processing, // 处理中
        Paused,     // 已暂停
        Completed,  // 已完成
        Canceled    // 已取消
    }
    public class MesWorkOrder:BaseModel
    {
        /// <summary>
        /// 工单编号
        /// </summary>
        public string WorkOrderCode { get; set; } = null!;
       
        /// <summary>
        /// 工单状态
        /// </summary>
        public OrderStatus OrderStatus { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 该工单包含的PCB类型
        /// </summary>

        public List<MesWorkOrderPCB> WorkOrderPCBs { get; set; } = new List<MesWorkOrderPCB>();
        /// <summary>
        /// 该工单检测PCB的记录
        /// </summary>
        public List<MesPCBInspection> Inspections { get; set; } = new List<MesPCBInspection>();
    }
}
