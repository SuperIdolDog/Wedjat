using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wedjat.Model.Entity
{
    public class WorkOrderPCB:BaseModel
    {
        /// <summary>
        /// 关联的工单号（外键，对应WorkOrder.WorkOrderCode）
        /// </summary>
        [Required]
        [StringLength(50)]
        public string WorkOrderCode { get; set; }

        /// <summary>
        /// 关联的PCB编码（外键，对应PCB.PCBCode）
        /// </summary>
        [Required]
        [StringLength(50)]
        public string PCBCode { get; set; }


        public virtual WorkOrder WorkOrder { get; set; }


        public virtual PCB PCB { get; set; }
    }
}
