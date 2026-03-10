using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wedjat.Model.DTO
{
    public class PLCSlaveVarDTO
    {
        /// <summary>
        /// 从站地址
        /// </summary>
        public int SlaveId {  get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public int Address { get; set; }

        /// <summary>
        /// 变量名称
        /// </summary>
        public string VariableName { get; set; }

        /// <summary>
        /// 变量描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// 当前值
        /// </summary>
        public object CurrentValue { get; set; }
       
        /// <summary>
        /// Modbus寄存器类型
        /// </summary>
        public string ModbusType { get; set; }

        /// <summary>
        /// 是否可写
        /// </summary>
        public bool IsWritable { get; set; }
    }
}
