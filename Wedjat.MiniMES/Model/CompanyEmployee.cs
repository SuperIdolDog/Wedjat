namespace Wedjat.MiniMES.Model
{
    public class CompanyEmployee:BaseModel
    {
        /// <summary>
        /// 员工工号
        /// </summary>
        public string WorkId { get; set; } = null!;

        /// <summary>
        /// 员工名称
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 员工密码
        /// </summary>
        public string Password {  get; set; } = null!;

        public List<MesPCBInspection> Inspections { get; set; } = new List<MesPCBInspection>();
    }
}
