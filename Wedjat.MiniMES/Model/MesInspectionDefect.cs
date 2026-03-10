using System.ComponentModel.DataAnnotations.Schema;

namespace Wedjat.MiniMES.Model
{
    public class MesInspectionDefect:BaseModel
    {
        /// <summary>
        /// 检测编号(逻辑外键)
        /// </summary>
        public string InspectionCode { get; set; } = null!;
        /// <summary>
        /// 缺陷名称(逻辑外键)
        /// </summary>
        public string DefectCode { get; set; } = null!;
        /// <summary>
        /// 缺陷数量
        /// </summary>
        public int Count { get; set; } = 0;
        /// <summary>
        /// 导航属性：关联对应的质检记录（无用户关联）
        /// </summary>
        [ForeignKey("InspectionCode")]

        public MesPCBInspection? Inspection { get; set; }
        [ForeignKey("DefectCode")]

        public MesPCBDefect? Defect { get; set; }
    }
}
