using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wedjat.Model.DTO
{
    public class PLCValueUpdateDTO
    {
        public long Id { get; set; }
        public int SlaveId {  get; set; }
        public string VariableName { get; set; }
        public object  Value { get; set; }

        public int Address { get; set; }
        public string ModbusType { get; set; }
    }
}
