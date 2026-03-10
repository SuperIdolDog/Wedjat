using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wedjat.Model.Entity
{
    public class PCBDefectDetail:BaseModel
    {
        /// <summary>
        /// 检测记录编码（逻辑外键）
        /// </summary>
        public string InspectionCode { get; set; }


        /// <summary>
        /// 缺陷类型编码（逻辑外键）
        /// </summary>
        [StringLength(50)]
        public string DefectTypeCode { get; set; }

        /// <summary>
        /// 缺陷类型名称
        /// </summary>
        [Required]
        [StringLength(100)]
        public string DefectTypeName { get; set; }

        /// <summary>
        /// 缺陷数量
        /// </summary>
        private int _defectCount; // 改为私有字段+属性
        /// <summary>
        /// 缺陷数量
        /// </summary>
        public int DefectCount
        {
            get => _defectCount;
            set
            {
                if (_defectCount != value)
                {
                    _defectCount = value;
                    OnPropertyChanged(nameof(DefectCount)); // 修改时通知UI
                }
            }
        }

        /// <summary>
        /// 缺陷位置描述
        /// </summary>
        [StringLength(200)]
        public string DefectPosition { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }

       

        // 导航属性
        [ForeignKey("InspectionCode")]
        public virtual PCBInspection Inspection { get; set; }

        [ForeignKey("DefectTypeCode")]
        public virtual PCBDefectType DefectType { get; set; }
    }
}
