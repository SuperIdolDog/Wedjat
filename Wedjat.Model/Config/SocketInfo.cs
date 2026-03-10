using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wedjat.Model.Config
{
    public class SocketInfo
    {
        
        //IP地址
        public string Ip { get; set; } = "192.168.1.100";


        //端口（默认 502）
        public int Port { get; set; } = 502;

    }
}
