using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wedjat.Helper;
using Wedjat.Model.Config;




namespace Wedjat.BLL
{
    public class IniService
    {
        [Description("读取配置文件,返回一个通信信息对象")]
        public SerialInfo GetSerialInfoFromPath(string path)
        {
            SerialInfo serialInfo = new SerialInfo();
            serialInfo.PortName =
                IniConfigHelper.ReadIniData("配置信息", "端口号", serialInfo.PortName, path);
            serialInfo.BaudRate = Convert.ToInt32(
                IniConfigHelper.ReadIniData("配置信息", "波特率", serialInfo.BaudRate.ToString(), path));
            serialInfo.Parity = (Parity)Enum.Parse(typeof(Parity),
               IniConfigHelper.ReadIniData("配置信息", "校验位", serialInfo.Parity.ToString(), path), true);
            serialInfo.DataBits = Convert.ToInt32(
               IniConfigHelper.ReadIniData("配置信息", "数据位", serialInfo.DataBits.ToString(), path));
            serialInfo.StopBits = (StopBits)Enum.Parse(typeof(StopBits),
               IniConfigHelper.ReadIniData("配置信息", "停止位", serialInfo.StopBits.ToString(), path), true);
            return serialInfo;
        }
        [Description("将通信信息对象写入配置文件路径中")]
        public bool SetSerialInfoToPath(SerialInfo serialInfo, string path)
        {
            bool result = true;
            result &= IniConfigHelper.WriteIniData("配置信息", "端口号", serialInfo.PortName, path);
            result &= IniConfigHelper.WriteIniData("配置信息", "波特率", serialInfo.BaudRate.ToString(), path);
            result &= IniConfigHelper.WriteIniData("配置信息", "校验位", serialInfo.Parity.ToString(), path);
            result &= IniConfigHelper.WriteIniData("配置信息", "数据位", serialInfo.DataBits.ToString(), path);
            result &= IniConfigHelper.WriteIniData("配置信息", "停止位", serialInfo.StopBits.ToString(), path);
            return result;
        }
        [Description("读取配置文件,返回一个通信信息对象")]
        public SocketInfo GetSocketInfoFromPath(string path)
        {
            SocketInfo socketInfo = new SocketInfo();
            socketInfo.Ip =
                IniConfigHelper.ReadIniData("配置信息", "Ip", socketInfo.Ip, path);
            socketInfo.Port = Convert.ToInt32(
                IniConfigHelper.ReadIniData("配置信息", "网口", socketInfo.Port.ToString(), path));
            return socketInfo;
        }
        [Description("将通信信息对象写入配置文件路径中")]
        public bool SetSocketInfoToPath(SocketInfo socketInfo, string path)
        {
            bool result = true;
            result &= IniConfigHelper.WriteIniData("配置信息", "Ip", socketInfo.Ip, path);
            result &= IniConfigHelper.WriteIniData("配置信息", "网口", socketInfo.Port.ToString(), path);
            
            return result;
        }

        [Description("读取配置文件,返回一个通信信息对象")]
        public ScannerInfo GetScannerInfoFromPath(string path)
        {
            ScannerInfo scannerInfo = new ScannerInfo();
            scannerInfo.PortName =
                IniConfigHelper.ReadIniData("配置信息", "端口号", scannerInfo.PortName, path);
            scannerInfo.BaudRate = Convert.ToInt32(
                IniConfigHelper.ReadIniData("配置信息", "波特率", scannerInfo.BaudRate.ToString(), path));
            scannerInfo.Parity = (Parity)Enum.Parse(typeof(Parity),
               IniConfigHelper.ReadIniData("配置信息", "校验位", scannerInfo.Parity.ToString(), path), true);
            scannerInfo.DataBits = Convert.ToInt32(
               IniConfigHelper.ReadIniData("配置信息", "数据位", scannerInfo.DataBits.ToString(), path));
            scannerInfo.StopBits = (StopBits)Enum.Parse(typeof(StopBits),
               IniConfigHelper.ReadIniData("配置信息", "停止位", scannerInfo.StopBits.ToString(), path), true);
            scannerInfo.MesAPI=
                 IniConfigHelper.ReadIniData("配置信息", "MesAPI", scannerInfo.MesAPI, path);
            scannerInfo.OverTime = Convert.ToInt32(
                 IniConfigHelper.ReadIniData("配置信息", "查询超时时间", scannerInfo.OverTime.ToString(), path));
            return scannerInfo;
        }
        [Description("将通信信息对象写入配置文件路径中")]
        public bool SetScannerInfoToPath(ScannerInfo scannerInfo, string path)
        {
            bool result = true;
            result &= IniConfigHelper.WriteIniData("配置信息", "端口号", scannerInfo.PortName, path);
            result &= IniConfigHelper.WriteIniData("配置信息", "波特率", scannerInfo.BaudRate.ToString(), path);
            result &= IniConfigHelper.WriteIniData("配置信息", "校验位", scannerInfo.Parity.ToString(), path);
            result &= IniConfigHelper.WriteIniData("配置信息", "数据位", scannerInfo.DataBits.ToString(), path);
            result &= IniConfigHelper.WriteIniData("配置信息", "停止位", scannerInfo.StopBits.ToString(), path);
            result &= IniConfigHelper.WriteIniData("配置信息", "MesAPI", scannerInfo.MesAPI, path);
            result &= IniConfigHelper.WriteIniData("配置信息", "查询超时时间", scannerInfo.OverTime.ToString(), path);
            return result;
        }
    }
}
