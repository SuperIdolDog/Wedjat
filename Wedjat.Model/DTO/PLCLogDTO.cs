using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wedjat.Model.DTO
{
    public class PLCLogDTO
    {
        /// <summary>
        /// 从站地址
        /// </summary>
        public int SlaveAddress { get; set; }

        /// <summary>
        /// 通信类型（读/写/连接/断开/异常）
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
        /// 通信时间（日志生成时间）
        /// </summary>
        public DateTime CommunicationTime { get; set; }

        /// <summary>
        /// 连接类型（TCP/RTU）
        /// </summary>
        public string ConnectionType { get; set; }

        /// <summary>
        /// 串口名
        /// </summary>
        public string ConnectionParams { get; set; }
        public string ToLogString()
        {
            // 基础信息：时间 + 连接类型 + 操作类型
            var baseInfo = $"{CommunicationTime:yyyy-MM-dd HH:mm:ss} [{ConnectionType}] {CommunicationType}";

            // 连接相关信息（连接/断开操作时显示）
            if (CommunicationType == "连接" || CommunicationType == "断开")
            {
                return $"参数：{ConnectionParams} - 状态：{(IsSuccess ? "成功" : "失败")} - {Data}";
            }

            // 数据读写相关信息（读/写操作时显示）
            var rwInfo = $"从站：{SlaveAddress} - 地址：{AddressInfo}";
            if (IsSuccess)
            {
                return $"{rwInfo} - 数据：{Data} - 状态：成功";
            }
            else
            {
                return $"{rwInfo} - 尝试数据：{Data} - 状态：失败 - 原因：{ErrorMessage}";
            }
        }
    }
}
   
