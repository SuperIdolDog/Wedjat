using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wedjat.Model.Entity;

namespace Wedjat.Model.DTO
{
    public delegate List<WorkOrderResponse> DelegateReturnWorkOrder(object sender, WorkOrderResponse e);
    public class WorkOrderResponse : EventArgs
    {
       
            public int Id { get; set; }
            public string WorkOrderCode { get; set; } 

            public OrderStatu OrderStatus { get; set; }

            public DateTime? StartTime { get; set; }
            public DateTime? EndTime { get; set; }
        
           
        public List<MesWorkOrderPCB> WorkOrderPCBs { get; set; } = new List<MesWorkOrderPCB>();

        public class MesWorkOrderPCB
        {
            public string PCBCode { get; set; }
            public int PlanQuantity { get; set; }
            public int? CompleteQuantity { get; set; }
            public int QualifiedQuantity { get; set; }
            public MesPCB mesPCBs { get; set; } = new MesPCB();
        }

        public class MesPCB
        {
            public string PCBCode { get; set; }
            public string PCBName { get; set; }
            public List<MesPCBDefect> mesPCBDefects { get; set; }
        }

        public class MesPCBDefect
        {
            public string DefectCode { get; set; }
            public string DefectName { get; set; }

            public int DefectCount { get; set; }
        }

    }
}
