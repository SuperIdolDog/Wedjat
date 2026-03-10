using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wedjat.Model.Entity
{
    /// <summary>
    /// 扫码枪扫描到的工单信息
    /// </summary>
    public class WorkOrder:BaseModel
    {
        /// <summary>
        /// 工单编号（MES系统返回）
        /// </summary>
        [StringLength(50)]
        public string WorkOrderCode { get; set; } = null;
       
        /// <summary>
        /// 计划生成数量
        /// </summary>
        public int PlanQuantity { get; set; }
        /// <summary>
        /// 完成数量
        /// </summary>
        public int? CompleteQuantity { get; set; }
        /// <summary>
        /// 工单状态
        /// </summary>

        public OrderStatu OrderStatus { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }


        // 导航属性
        public virtual ICollection<ScannerData> ScannerDatas { get; set; } = new List<ScannerData>();

        public virtual ICollection<WorkOrderPCB> WorkOrderPCBs { get; set; } = new List<WorkOrderPCB>();

        public virtual ICollection<PCBInspection> PCBInspections { get; set; } = new List<PCBInspection>();
    }
    public enum OrderStatu
    {
        Pending,    // 待处理
        Processing, // 处理中
        Paused,     // 已暂停
        Completed,  // 已完成
        Canceled    // 已取消
    }
}
