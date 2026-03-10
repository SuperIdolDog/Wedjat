using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wedjat.Model.Entity
{
    /// <summary>
    /// 系统日志实体类，记录所有的信息
    /// </summary>
    public class SysLog:BaseModel
    {
        /// <summary>
        /// 日志类型(与字典详情表逻辑关联)
        /// </summary>
        public LogType LogType { get; set; }
        /// <summary>
        /// 日志详情
        /// </summary>
        public string Detail { get; set; }

        /// <summary>
        /// 日志的警告级别(与字典详情表逻辑关联)
        /// </summary>
        public LogStatu LogStatu { get; set; }
    }
    public enum LogType
    {
        /// <summary>
        /// 系统操作
        /// </summary>
        [Description("系统操作")]
        SystemOperation = 0,

        /// <summary>
        /// 用户操作
        /// </summary>
        [Description("用户操作")]
        UserOperation = 1,

        /// <summary>
        /// 扫码操作
        /// </summary>
        [Description("扫码操作")]
        ScanOperation = 2,

        /// <summary>
        /// 检测操作
        /// </summary>
        [Description("检测操作")]
        InspectionOperation = 3,

        /// <summary>
        /// PLC通信
        /// </summary>
        [Description("PLC通信")]
        PLCCommunication = 4,

        /// <summary>
        /// MES接口调用
        /// </summary>
        [Description("MES接口调用")]
        MESApiCall = 5,

        /// <summary>
        /// 数据库操作
        /// </summary>
        [Description("数据库操作")]
        DatabaseOperation = 6,

        /// <summary>
        /// 系统启动/关闭
        /// </summary>
        [Description("系统启动/关闭")]
        SystemStartupShutdown = 7,

        /// <summary>
        /// 配置变更
        /// </summary>
        [Description("配置变更")]
        ConfigurationChange = 8
    }
    public enum LogStatu
    {
       
        /// <summary>
        /// 一般信息
        /// </summary>
        [Description("系统提示")]
        System = 0,

        /// <summary>
        /// 警告信息
        /// </summary>
        [Description("业务的警告")]
        Warning = 1,

        /// <summary>
        /// 错误信息
        /// </summary>
        [Description("底层错误")]
        Error = 2,

        /// <summary>
        /// 失败信息
        /// </summary>
        [Description("业务的失败")]
        Failure=3,

        /// <summary>
        /// 成功信息
        /// </summary>
        [Description("业务的成功")]
        Success = 4,
    }
}
