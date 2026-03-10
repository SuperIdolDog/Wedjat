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
    /// 根据工单查出来的PCB
    /// </summary>
    [Table("PCB")]
    public class PCB :BaseModel
    {
        /// <summary>
        /// 当前工单的PCB
        /// </summary>
        [StringLength(50)]
        public string PCBCode { get; set; }
        /// <summary>
        /// PCB名称
        /// </summary>
        [Required]
        [StringLength(100)]
        public string PCBName { get; set; }

        // 导航属性
        public virtual ICollection<PCBDefectType> Defects { get; set; } = new List<PCBDefectType>();

        public virtual ICollection<WorkOrderPCB> WorkOrderPCBs { get; set; } = new List<WorkOrderPCB>();

        public virtual ICollection<PCBInspection> PCBInspections { get; set; } = new List<PCBInspection>();
    }
}


