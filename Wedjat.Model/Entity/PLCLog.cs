using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wedjat.Model.Entity
{
    /// <summary>
    /// PLC的日志
    /// </summary>
    public class PLCLog:BaseModel
    {
        /// <summary>
        /// 从站地址
        /// </summary>
        public int SlaveAddress { get; set; }

        /// <summary>
        /// 通信类型（读/写）
        /// </summary>
        public string CommunicationType { get; set; }

        /// <summary>
        /// 地址信息
        /// </summary>
        public int AddressInfo { get; set; }

        /// <summary>
        /// 通信数据
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// 通信状态（成功/失败）
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 连接类型（TCP/RTU）
        /// </summary>
        public string ConnectionType { get; set; } 

        /// <summary>
        /// 串口名
        /// </summary>
        public string ConnectionParams { get; set; }

    }
}
