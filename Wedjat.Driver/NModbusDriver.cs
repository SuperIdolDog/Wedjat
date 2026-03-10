using Modbus;
using Modbus.Device;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wedjat.Driver
{
    public class NModbusDriver
    {
        //RTU相关字段
        private SerialPort _serialPort;
        //TCP相关字段
        private TcpClient _tcpClient;

        //通用字段
        public event Action<string> CodeReceived;//自定义事件
        private IModbusMaster _master;
        private bool _isInitialized = false;


      
        /// <summary>
        /// 串口接收信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //需要加一定的延时，否则可能会接收不完整
            Thread.Sleep(50);
            int count = this._serialPort?.BytesToRead ?? 0;
            if (count > 0)
            {
                byte[] data = new byte[count];
                this._serialPort.Read(data, 0, count);
                CodeReceived?.Invoke(Encoding.Default.GetString(data));
            }
        }
        private void Rtu_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //需要加一定的延时，否则可能会接收不完整
            Thread.Sleep(50);
            int count = this._serialPort?.BytesToRead ?? 0;
            if (count > 0)
            {
                byte[] data = new byte[count];
                this._serialPort.Read(data, 0, count);
                CodeReceived?.Invoke(Encoding.Default.GetString(data));
            }
        }
        #region 初始化Modbus RTU主站
        /// <summary>
        /// 初始化 Modbus RTU 主站
        /// </summary>
        /// <param name="portName">端口名称，如"COM19"</param>
        /// <param name="baudRate">波特率</param>
        /// <param name="parity">校验位</param>
        /// <param name="dataBits">数据位</param>
        /// <param name="stopBits">停止位</param>
        /// <returns>是否初始化成功</returns>
        public bool InitializeRTU(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            if (this._serialPort != null && _serialPort.IsOpen)
            {
                this._serialPort.Close();
            }
            try
            {

                // 创建并配置串口
                _serialPort = new SerialPort()
                {
                    PortName = portName,
                    BaudRate = baudRate,
                    Parity = parity,
                    DataBits = dataBits,
                    StopBits = stopBits
                };
                // 创建主站
                _master = ModbusSerialMaster.CreateRtu(_serialPort);
                // 配置超时
                _master.Transport.ReadTimeout = 2000;
                _master.Transport.WriteTimeout = 2000;
                _master.Transport.WaitToRetryMilliseconds=500;
                _master.Transport.Retries = 3;
                _isInitialized = true;
                _serialPort.DataReceived += Rtu_DataReceived;
                _serialPort.Open();
                Console.WriteLine("RTU初始化成功");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"初始化失败: {ex.Message}");
                _isInitialized = false;
                return false;
                throw new Exception($"初始化失败: {ex.Message}");
               
            }

        }

        #endregion

        #region 初始化Modbus TCP 主站
        /// <summary>
        /// 初始化Modbus TCP主站
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool InitializeTCP(string ip,int port)
        {
            try{
                _tcpClient = new TcpClient();
                _tcpClient.Connect(IPAddress.Parse(ip), port);
              
                _master = ModbusIpMaster.CreateIp(_tcpClient);
                _master.Transport.ReadTimeout = 2000;
                _master.Transport.WriteTimeout = 2000;
                _master.Transport.WaitToRetryMilliseconds = 500;
                _master.Transport.Retries = 3;
                _isInitialized = true;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"初始化失败: {ex.Message}");
                _isInitialized = false;
                return false;
            }


        }
        
        #endregion

        #region 读取保持寄存器
        /// <summary>
        /// 读取保持寄存器
        /// </summary>
        /// <param name="slaveAddress">从站地址</param>
        /// <param name="startAddress">起始地址</param>
        /// <param name="count">读取数量</param>
        /// <returns>寄存器数据</returns>
        public byte[] ReadHoldingRegisters(byte slaveAddress, ushort start, ushort length)
        {
            if (_master == null)
            {
                throw new Exception("Modbus 连接已断开，请重新连接后再操作");
            }
            try
            {
                ushort[] data = this._master.ReadHoldingRegisters(slaveAddress, start, length);
                List<byte> result = new List<byte>();
                foreach (var item in data)
                {
                    result.AddRange(BitConverter.GetBytes(item));
                }
                return result.ToArray();
            }
            catch (Exception ex)
            {

                throw new Exception("读取保持寄存器失败:"+ex.Message);
            }

        }

        #endregion

        #region 读取输入寄存器
        /// <summary>
        /// 读取输入寄存器
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public byte[] ReadInputRegisters(byte slaveId,ushort start,ushort length)
        {
            if (!_isInitialized || _master == null)
            {
                throw new Exception("Modbus 连接已断开，请重新连接后再操作");
            }
            try
            {
                ushort[] data = this._master.ReadInputRegisters(slaveId, start, length);
                List<byte> result = new List<byte>();
                foreach (var item in data)
                {
                    result.AddRange(BitConverter.GetBytes(item));
                }
                return result.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine("读取输入寄存器失败:" + ex.Message);
                throw new Exception("读取输入寄存器失败:" + ex.Message);
            }
        }
        #endregion

        #region 读取输出线圈
        /// <summary>
        /// 读取输出线圈
        /// </summary>
        /// <param name="slaveAddress"></param>
        /// <param name="startAddress"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public bool[] ReadOutputCoils(byte slaveId, ushort start, ushort length)
        {
            if (!_isInitialized || _master == null)
            {
                throw new Exception("Modbus 连接已断开，请重新连接后再操作");
            }
            try
            {
              
                return _master.ReadCoils(slaveId, start, length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("读取输出线圈失败:" + ex.Message);
                throw new Exception("读取输出线圈失败:"+ex.Message);
            }
          
        }
        #endregion

        #region 读取输入线圈
        /// <summary>
        /// 读取输入线圈
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool[] ReadInputCoils(byte slaveId, ushort start, ushort length)
        {
            if (!_isInitialized || _master == null)
            {
                throw new Exception("Modbus 连接已断开，请重新连接后再操作");
            }
            try
            {

                return _master.ReadInputs(slaveId, start, length);
            }
            catch (Exception ex)
            {
                throw new Exception("读取输入线圈失败:" + ex.Message);
            }

        }
        #endregion

        #region 写入单个线圈
        /// <summary>
        /// 写入单个线圈
        /// </summary>
        /// <param name="slaveAddress"></param>
        /// <param name="coilAddress"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public void  WriteSingleCoil(byte slaveId, ushort address, bool value)
        {
           
            try
            {
                if (!_isInitialized|| _master == null)
                {
                    throw new Exception("Modbus 连接已断开，请重新连接后再操作");
                    
                }
                _master.WriteSingleCoil(slaveId, address, value);
              //  return true;
            }
            catch (Exception ex)
            {
                 //throw new Exception("写入单线圈失败:" + ex.Message);
                Console.WriteLine("写入单线圈失败:" + ex.Message);
         
            }
          
        }
        #endregion

        #region 写入单个寄存器
        /// <summary>
        /// 写入单个寄存器
        /// </summary>
        /// <param name="slaveAddress"></param>
        /// <param name="registerAddress"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public void WriteSingleRegister(byte slaveId, ushort address, ushort value)
        {
           
            try
            {
                if (!_isInitialized || _master == null)
                {
                    throw new Exception("Modbus 连接已断开，请重新连接后再操作");
                }
                _master.WriteSingleRegister(slaveId, address, value);
            }
            catch (Exception ex)
            {
                throw new Exception("写入单寄存器失败:" + ex.Message);
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <exception cref="Exception"></exception>
        public void WriteSingleRegister(byte slaveId, ushort address, short value)
        {
           
            try
            {
                if (!_isInitialized || _master == null)
                {
                    throw new Exception("Modbus 连接已断开，请重新连接后再操作");
                }
                ushort val=BitConverter.ToUInt16(BitConverter.GetBytes(value), 0);
                _master.WriteSingleRegister(slaveId, address, val);
            }
            catch (Exception ex)
            {
                throw new Exception("写入单寄存器失败:" + ex.Message);
            }

        }
        #endregion

        #region 写入多线圈
        /// <summary>
        /// 写入多线圈
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <exception cref="Exception"></exception>
        public void WriteMultipleCoils(byte slaveId,ushort address, bool[] value)
        {
            if (!_isInitialized || _master == null)
            {
                throw new Exception("Modbus 连接已断开，请重新连接后再操作");
            }
            try
            {
                _master.WriteMultipleCoils(slaveId, address, value);
            }
            catch (Exception ex)
            {
                throw new Exception("写入多线圈失败:"+ex.Message);
            }
        }
        #endregion

        #region 写入多寄存器
        /// <summary>
        /// 写入多寄存器
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="start"></param>
        /// <param name="value"></param>
        /// <exception cref="Exception"></exception>
        public void  WriteMultipleRegisters(byte slaveId,ushort start, byte[] value)
        {
           

            try
            {
                if (value != null && value.Length % 2 == 0)
                {
                    ushort[] data = new ushort[value.Length / 2];
                    for (int i = 0; i < data.Length; i += 2)
                    {
                        data[i / 2] = BitConverter.ToUInt16(new byte[]{ value[i+1],value[i]}, 0);
                    }
                    this._master.WriteMultipleRegisters(slaveId, start, data);
                }
                else
                {
                    throw new Exception("写入多线圈失败:字节数组必须为正偶数");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("写入多线圈失败:"+ex.Message);
            }
        }
        public void WriteMultipleRegisters(byte slaveId, ushort start, short[] value)
        {
                List<byte> bytes=new List<byte>();
                foreach(var item in value)
                {
                    bytes.AddRange(BitConverter.GetBytes(item).Reverse());
                }
                WriteMultipleRegisters(slaveId,start,bytes.ToArray());
            
        }
        public void WriteMultipleRegisters(byte slaveId, ushort start, ushort[] value)
        {
            List<byte> bytes = new List<byte>();
            foreach (var item in value)
            {
                bytes.AddRange(BitConverter.GetBytes(item).Reverse());
            }
            WriteMultipleRegisters(slaveId, start, bytes.ToArray());

        }
        #endregion

        #region 关闭串口
        /// <summary>
        /// 关闭串口
        /// </summary>
        public void DisConnectRTU()
        {
            try
            {
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    _serialPort.DataReceived -= SerialPort_DataReceived;
                    _serialPort.Close();
                    _serialPort.Dispose();
                }
                this._master = null;
            }
            catch (Exception ex)
            {

                throw new Exception("关闭串口异常:"+ex.Message);
            }
           
        }
        #endregion

        #region 关闭网口
        /// <summary>
        /// 关闭网口
        /// </summary>
        public void DisConnectTCP()
        {
            try
            {
                if (_tcpClient != null && _tcpClient.Connected)
                {
                    _tcpClient.Close();
                    _tcpClient.Dispose();
                }
                this._master = null;
            }
            catch (Exception ex)
            {

                throw new Exception("关闭网口异常:" + ex.Message);
            }
           
        }
        #endregion



    }
}



