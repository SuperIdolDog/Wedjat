using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wedjat.Model.Entity;

namespace Wedjat.Model.DTO
{
    public class WorkOrderUpdateDTO
    {
        public string PCBSN { get; set; } // 关联的PCB条码（可选，用于校验）
        public bool IsQualified { get; set; } // 是否合格（合格才累加完成数，可按需调整）
        public string TargetPCBType { get; set; } // 对应的PCB类型（避免跨类型累加）

        public OrderStatu OrderStatus { get; set; } // 工单状态更新（可选）
    }
}
