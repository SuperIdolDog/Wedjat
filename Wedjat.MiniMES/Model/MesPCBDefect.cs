using System.ComponentModel.DataAnnotations.Schema;

namespace Wedjat.MiniMES.Model
{
    public class MesPCBDefect : BaseModel
    {
        /// <summary>
        /// PCB编号(逻辑外键)
        /// </summary>
        public string PCBCode { get; set; } = null!;

        /// <summary>
        /// 缺陷标识
        /// </summary>
        public string DefectCode {  get; set; } = null!;
        /// <summary>
        /// 缺陷名称
        /// </summary>

        public string DefectName { get; set; } = null!;
        /// <summary>
        /// 描述
        /// </summary>

        public string Description {  get; set; } = null!;

        [ForeignKey("PCBCode")]

        public MesPCB? PCB { get; set; }
        public List<MesInspectionDefect> InspectionDefects { get; set; } = new List<MesInspectionDefect>();

    }
}
