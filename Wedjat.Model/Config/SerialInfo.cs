using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wedjat.Model.Config
{
    public class SerialInfo
    {
        public string PortName { get; set; } = "COM19";//端口号
        public int BaudRate { get; set; } = 9600;//波特率
        public Parity Parity { get; set; } = Parity.None;//校验位
        public int DataBits { get; set; } = 8;//数据位
        public StopBits StopBits { get; set; } = StopBits.One;//停止位
    }
}
