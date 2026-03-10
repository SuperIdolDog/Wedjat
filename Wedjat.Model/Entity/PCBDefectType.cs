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
    /// 当前PCB的缺陷
    /// </summary>
    public class PCBDefectType:BaseModel
    {
        /// <summary>
        /// 缺陷类型唯一编码（逻辑主键，补充该字段用于关联）
        /// </summary>
        [Required]
        [StringLength(50)]
        public string DefectTypeCode { get; set; }
        /// <summary>
        /// PCB类型逻辑外键
        /// </summary>
        [Required]
        [StringLength(50)]
        public string PCBCode { get; set; }
        /// <summary>
        /// 当前型号PCB的缺陷名称
        /// </summary>
        [Required]
        [StringLength(100)]
        public string DefectName { get; set; }
        /// <summary>
        ///  当前型号PCB的缺陷描述
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }  // 缺陷描述
        // 导航属性
        [ForeignKey("PCBCode")]
        public virtual PCB PCB { get; set; }
        public virtual ICollection<PCBDefectDetail> DefectDetails { get; set; } = new List<PCBDefectDetail>();
    }
}
