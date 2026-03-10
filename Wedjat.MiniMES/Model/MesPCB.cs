namespace Wedjat.MiniMES.Model
{
    public class MesPCB:BaseModel
    {
        /// <summary>
        /// PCB编号
        /// </summary>
        public string PCBCode { get; set; } = null!;
        /// <summary>
        /// PCB名称
        /// </summary>
        public string PCBName { get; set; }= null!;
        /// <summary>
        /// 该PCB包含的缺陷类型
        /// </summary>
        public List<MesPCBDefect> Defects { get; set; }=new List<MesPCBDefect>();
        // 导航属性：关联包含该PCB的工单（多对多，通过中间表）
        public List<MesWorkOrderPCB> WorkOrderPCBs { get; set; } = new List<MesWorkOrderPCB>();

        // 导航属性：该PCB的所有质检记录
        public List<MesPCBInspection> Inspections { get; set; } = new List<MesPCBInspection>();

    }
}
