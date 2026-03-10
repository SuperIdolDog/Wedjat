using Wedjat.MiniMES.Model;

namespace Wedjat.MiniMES.DTO
{
    public class MesToScada
    {
        public int Id {  get; set; }
        public string WorkOrderCode { get; set; } = null!;

        public OrderStatus OrderStatus { get; set; }

        public List<MesWorkOrderPCB> WorkOrderPCBs { get; set; } = new List<MesWorkOrderPCB>();

     
    }
    public class MesWorkOrderPCB
    {
        public string PCBCode { get; set; } = null!;
        public int PlanQuantity { get; set; }

        public int? CompleteQuantity { get; set; } = 0;

        public int QualifiedQuantity { get; set; } = 0;

        public MesPCB mesPCBs { get; set; } =new MesPCB();
    }
    public class MesPCB
    {
        public string PCBCode { get; set; } = null!;
        public string PCBName { get; set; } = null!;

       
        public List<MesPCBDefect> mesPCBDefects { get; set; } = new List<MesPCBDefect>();


    }
    public class MesPCBDefect
    {
        public string DefectCode { get; set; } = null!;

        public string DefectName { get; set; } = null!;
    }
}
