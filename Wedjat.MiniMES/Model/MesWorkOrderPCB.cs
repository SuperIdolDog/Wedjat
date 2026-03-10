using System.ComponentModel.DataAnnotations.Schema;

namespace Wedjat.MiniMES.Model
{
    public class MesWorkOrderPCB:BaseModel
    {
        
        /// <summary>
        /// 工单的业务编号(逻辑外键)
        /// </summary>
        public string WorkOrderCode { get; set; } = null!;

        /// <summary>
        /// PCB的业务编号（逻辑外键）
        /// </summary>
        public string PCBCode { get; set; } = null!;

        /// <summary>
        /// 计划数量
        /// </summary>
        public int PlanQuantity { get; set; }
        /// <summary>
        /// 完成数量
        /// </summary>
        public int? CompleteQuantity { get; set; } = 0;
        /// <summary>
        /// 次品数量
        /// </summary>
        public int QualifiedQuantity { get; set; } = 0;


        [ForeignKey("WorkOrderCode")]
        public MesWorkOrder? WorkOrder { get; set; }


        [ForeignKey("PCBCode")]
        public MesPCB? PCB { get; set; }
    }
}
