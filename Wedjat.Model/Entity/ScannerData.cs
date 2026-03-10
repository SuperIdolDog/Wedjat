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
    /// 扫码枪获取数据的实体类
    /// </summary>
    public class ScannerData:BaseModel
    {
        /// <summary>
        /// 扫描解析到的二维码内容
        /// </summary>
        [Required]
        [StringLength(500)]
        public string CodeContent { get; set; }
        /// <summary>
        /// 调用MESAPI获取到的工单号
        /// </summary>
        [StringLength(50)]
        public string WorkOrderNo { get; set; }
        /// <summary>
        /// 扫码枪扫描时间
        /// </summary>
        public DateTime ScanTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 是否查询到工单(与字典详情表逻辑关联)
        /// </summary>
        public ScanStatus Statu { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        [StringLength(20)]
        public string OperatorWorkId { get; set; }
        // 导航属性 - 关联到对应的工单
        [ForeignKey("WorkOrderNo")]
        public WorkOrder WorkOrder { get; set; }
    }
    public enum ScanStatus
    {
        /// <summary>
        /// 扫码成功
        /// </summary>
        [Description("扫码成功")]
        Success = 0,

        /// <summary>
        /// 扫码失败
        /// </summary>
        [Description("扫码失败")]
        Failed = 1,

        /// <summary>
        /// 工单不存在
        /// </summary>
        [Description("工单不存在")]
        WorkOrderNotFound = 2,

        /// <summary>
        /// MES接口异常
        /// </summary>
        [Description("MES接口异常")]
        MESApiException = 3,

      
    }
}
