using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wedjat.Model.Entity
{
    /// <summary>
    /// 每块PCB检测结果
    /// </summary>

    public class PCBInspection:BaseModel
    {
        /// <summary>
        /// 检测记录唯一编码（逻辑主键，补充用于关联缺陷详情）
        /// </summary>
        [Required]
        [StringLength(50)]
        public string InspectionCode { get; set; }
        /// <summary>
        /// 工单号（逻辑外键）
        /// </summary>
        [Required]
        [StringLength(50)]
        public string WorkOrderCode { get; set; }
        /// <summary>
        /// PCB类型编码（逻辑外键）
        /// </summary>
        [Required]
        [StringLength(50)]
        public string PCBCode { get; set; }
        /// <summary>
        /// 当前PCB的SN
        /// </summary>
        [Required]
        [StringLength(100)]
        public string PCBSN { get; set; }
        /// <summary>
        /// 当前PCB的检测图片
        /// </summary>
        [StringLength(500)]
        public byte[] Image {  get; set; }
        /// <summary>
        /// 检测时间
        /// </summary>

        public DateTime InspectionTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 是否合格
        /// </summary>
        public bool IsQualified { get; set; }
        /// <summary>
        /// 检测员
        /// </summary>
        [StringLength(20)]
        public string InspectorWorkId { get; set; }
        /// <summary>
        /// 产线
        /// </summary>
        [StringLength(50)]
        public string ProductLine { get; set; }
        /// <summary>
        /// 总缺陷数量
        /// </summary>
        public int TotalDefectCount { get; set; }
        // 导航属性
        [ForeignKey("WorkOrderCode")]
        public virtual WorkOrder WorkOrder { get; set; }

        [ForeignKey("PCBCode")]
        public virtual PCB PCB { get; set; }
     
        public virtual ICollection<PCBDefectDetail> DefectDetails { get; set; } = new List<PCBDefectDetail>();
    }
}
