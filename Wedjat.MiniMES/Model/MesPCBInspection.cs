using System.ComponentModel.DataAnnotations.Schema;

namespace Wedjat.MiniMES.Model
{
    public class MesPCBInspection:BaseModel
    {
        /// <summary>
        /// 工单编号(逻辑外键)
        /// </summary>
        public string WorkOrderCode { get; set; } = null!;
        /// <summary>
        /// PCB编号(逻辑外键)
        /// </summary>
        public string PCBCode { get; set; } = null!;
        /// <summary>
        /// 检测编号
        /// </summary>
        public string InspectionCode { get; set; } = null!;
        /// <summary>
        /// 单个PCB的二维码标识(用于追溯)
        /// </summary>
        public string PCBSN { get; set; } = null!; 
        /// <summary>
        /// 检测时间
        /// </summary>
        public DateTime InspectionTime { get; set; } = DateTime.Now; 
        /// <summary>
        /// 是否合格
        /// </summary>
        public bool IsQualified { get; set; }
        /// <summary>
        /// 检测员工号
        /// </summary>
        public string WorkId { get; set; } = null!;
        /// <summary>
        /// 产线名称
        /// </summary>
        public string ProductLine { get;set; } = null!;

        public List<MesInspectionDefect> DetectedDefects { get; set; } = new List<MesInspectionDefect>();
      
        [ForeignKey("WorkOrderCode")]

        public MesWorkOrder? WorkOrder { get; set; }

        [ForeignKey("PCBCode")]

        public MesPCB? PCB { get; set; }

        [ForeignKey("WorkId")]

        public CompanyEmployee? Inspector { get; set; }
    }
    

}
