using AntdUI;
using FreeSql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using Wedjat.BLL;
using Wedjat.Driver;
using Wedjat.Helper;
using Wedjat.Model.Config;
using Wedjat.Model.DTO;
using Wedjat.Model.Entity;

namespace Wedjat.WinForm.MainMenuUI
{
    public partial class PLC : UserControl
    {
        public event EventHandler<DeviceSwitchStatusChangedEventArgs> SwitchStatusChanged;
      //  public event EventHandler<WorkOrderReceivedEventArgs> ModbusReceived;
       public NModbusDriver _modbusDriver;
        SerialInfo serialInfo = new SerialInfo();
        SocketInfo socketInfo = new SocketInfo();
        IniService iniService = new IniService();
        PLCSlaveVariableBLL _plcvalueBLL;
        PLCLogBLL _plclogBLL;
        System.Timers.Timer _readTimer;
        private bool _isSubmittingEdit = false;
        private BindingList<PLCSlaveVariable> _bindingList;
        private BindingList<PLCLog> _bindLogList;
        private string serialPath = Application.StartupPath + "\\serial.ini";
        private string socketPath = Application.StartupPath + "\\socket.ini";

        public PLC()
        {
            InitializeComponent();
            _readTimer = new System.Timers.Timer(1000);
            _readTimer.Elapsed += async (s, e) => await AutoReadAndRefresh();
            _readTimer.AutoReset = true;
            _modbusDriver = new NModbusDriver();
            _plcvalueBLL = new PLCSlaveVariableBLL(_modbusDriver);
            _plclogBLL = new PLCLogBLL();
            _bindLogList = new BindingList<PLCLog>();
            table_PLCLog.DataSource = _bindLogList;
            LoadRtuParamFromIni();
            LoadTcpParamFromIni();
            select_Serialport.Items.AddRange(SerialPort.GetPortNames());
            select_Serialport.SelectedValue = select_Serialport.Items[0].ToString().Trim();
            serialInfo.PortName = select_Serialport.Text;

            select_BaudRate.Items.AddRange(new string[] { "9600", "19200", "38400", "57600", "115200", "230400" });
            select_BaudRate.SelectedValue = select_BaudRate.Items[0].ToString().Trim();
            serialInfo.BaudRate = int.Parse(select_BaudRate.Text);

            

            select_DataBits.Items.AddRange(new string[] { "7", "8" });
            select_DataBits.SelectedValue = select_DataBits.Items[1].ToString().Trim();
            serialInfo.DataBits = int.Parse(select_DataBits.Text);

            select_StopBits.Items.AddRange(Enum.GetNames(typeof(StopBits)));
            select_StopBits.SelectedValue = select_StopBits.Items[1].ToString().Trim();
            serialInfo.StopBits = (StopBits)Enum.Parse(typeof(StopBits), this.select_StopBits.Text.Trim(), true);

            select_Parity.Items.AddRange(Enum.GetNames(typeof(Parity)));
            select_Parity.SelectedValue = select_Parity.Items[0].ToString().Trim();
            serialInfo.Parity = (Parity)Enum.Parse(typeof(Parity), this.select_Parity.Text.Trim(), true);

            table_Equipmentlocation.Columns = new AntdUI.ColumnCollection
            {
                new AntdUI.Column("VariableName", "名称", ColumnAlign.Center).SetLocalizationTitleID("Table.Column.").SetWidth("auto"),
                new AntdUI.Column("CurrentValue", "数值", ColumnAlign.Center)
                {
                  Width = "120",


                }.SetLocalizationTitleID("Table.Column.").SetWidth("auto"),
                 new AntdUI.Column("DataType", "数值类型", ColumnAlign.Center).SetLocalizationTitleID("Table.Column.").SetWidth("auto"),
                new AntdUI.Column("IsWritable", "是否可写", ColumnAlign.Center)
                {
                    Render = (value, record, rowIndex) =>
                    {
                        var var = record as PLCSlaveVariable;
                        if (var.IsWritable)
                        {
                            return new AntdUI.CellTag(
                                AntdUI.Localization.Get("Table.Data.IsWritable.True", "可写"), 
                               AntdUI.TTypeMini.Success
                            );
                        }
                        else
                        {
                            return new AntdUI.CellTag(
                                AntdUI.Localization.Get("Table.Data.IsWritable.False", "只读"), 
                                AntdUI.TTypeMini.Warn 
                            );
                        }
                    }
                }.SetLocalizationTitleID("Table.Column.").SetWidth("auto"),

                new AntdUI.Column("Description", "描述" ).SetLocalizationTitleID("Table.Column.")

            };

            table_PLCLog.Columns = new AntdUI.ColumnCollection
            {
                new AntdUI.Column("time", "时间", ColumnAlign.Center)
                {
                    Render=(value, record, index) =>
                    {
                        var log = record as PLCLog;
                        return log?.CreateTime.ToString("yyyy-MM-dd HH:mm:ss.fff") ?? "";
                    }
                }.SetFixed(),
                new AntdUI.Column("LogText", "日志信息")
                {
                    Render = (value, record, rowIndex) =>
                    {
                        if (record is PLCLog logEntity)
                        {
                            var dto = new PLCLogDTO
                            {
                                SlaveAddress = logEntity.SlaveAddress,
                                CommunicationType = logEntity.CommunicationType,
                                AddressInfo = logEntity.AddressInfo,
                                Data = logEntity.Data,
                                IsSuccess = logEntity.IsSuccess,
                                ErrorMessage = logEntity.ErrorMessage,
                                CommunicationTime = logEntity.CreateTime, 
                                ConnectionType = logEntity.ConnectionType,
                                ConnectionParams = logEntity.ConnectionParams
                            };
                            return dto.ToLogString();
                        }
                        else if (record is PLCLogDTO logDto)
                        {
                            return logDto.ToLogString();
                        }
                        return "日志数据格式错误";
                    }
                }.SetLocalizationTitleID("Table.Column.").SetFixed(),
            };
        }
        #region 自动读取和更新modbus协议传递的值
        private DateTime _lastUpdateTime = DateTime.MinValue;
        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(1500);
        //private async Task AutoReadAndRefresh()
        //{

        //    if (!IsConnected() || _bindingList == null)
        //        return;

        //    try
        //    {
        //        await Task.Run(async () =>
        //        {
        //            await _plcvalueBLL.ReadSlaveFromPLCAsync().ConfigureAwait(false);
        //            var latestVariables = await _plcvalueBLL.ShowSlaveVarToTable().ConfigureAwait(false);
        //            if (latestVariables == null || !latestVariables.Any())
        //                return;

        //            this.BeginInvoke((Action)(() =>
        //            {
        //                UpdateBindingList(latestVariables);
        //            }));
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        this.BeginInvoke((Action)(() =>
        //        {
        //            AntdUI.Message.success(this.FindForm(), $"自动更新失败：{ex.Message}");
        //        }));
        //    }

        //}

        private async Task AutoReadAndRefresh()
        {
            // 控制更新频率
            if (DateTime.Now - _lastUpdateTime < _updateInterval)
                return;

            if (!IsConnected() || _bindingList == null)
                return;

            _lastUpdateTime = DateTime.Now;

            try
            {
                // 使用优化的只读取变化值的方法
                var latestVariables = await _plcvalueBLL.ReadChangedValuesOnlyAsync().ConfigureAwait(false);
                if (latestVariables == null || !latestVariables.Any())
                    return;

                // 更新UI
                this.BeginInvoke((Action)(() =>
                {
                    try
                    {
                        bool anyChanged = false;

                        // 快速更新（假设ID是连续的或数量不大）
                        foreach (var existingVar in _bindingList)
                        {
                            var latestVar = latestVariables.FirstOrDefault(v => v.Id == existingVar.Id);
                            if (latestVar != null)
                            {
                                if (Math.Abs(existingVar.CurrentValue - latestVar.CurrentValue) > 0.0001)
                                {
                                    existingVar.CurrentValue = latestVar.CurrentValue;
                                    existingVar.UpdateTime = latestVar.UpdateTime;
                                    anyChanged = true;
                                }
                            }
                        }

                        // 只有变化时才刷新
                        if (anyChanged)
                        {
                            table_Equipmentlocation.Refresh();
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"UI更新异常: {ex.Message}");
                    }
                }));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"自动更新失败: {ex.Message}");
            }
        }
        #endregion

        #region 更新绑定列表信息
        private void UpdateBindingList(List<PLCSlaveVariable> latestVariables)
        {

            foreach (var latestVar in latestVariables)
            {
                var existingVar = _bindingList.FirstOrDefault(v => v.Id == latestVar.Id);
                if (existingVar != null)
                {
                    bool valueChanged = Math.Abs(existingVar.CurrentValue - latestVar.CurrentValue) > 0.0001;
                    if (valueChanged)
                    {
                        existingVar.CurrentValue = latestVar.CurrentValue;
                        existingVar.UpdateTime = latestVar.UpdateTime;
                    }
                }
            }
        }
        #endregion

        #region 根据按钮样式判断连接状态
        private bool IsConnected()
        {
            return btn_switchRTU.IconSvg == IconSvg.switch_IconSvg_Success
                || btn_switchTCP.IconSvg == IconSvg.switch_IconSvg_Success;
        }
        #endregion

        #region 从Ini文件中获取RTU相关参数信息
        private void LoadRtuParamFromIni()
        {
            if (File.Exists(serialPath))
            {
                serialInfo = iniService.GetSerialInfoFromPath(serialPath);
                if (select_Serialport.Items.Contains(serialInfo.PortName))
                {
                    select_Serialport.Text = serialInfo.PortName;
                }
                string baudStr = serialInfo.BaudRate.ToString();
                if (select_BaudRate.Items.Contains(baudStr))
                {
                    select_BaudRate.Text = baudStr;
                }
                string dataBitsStr = serialInfo.DataBits.ToString();
                if (select_DataBits.Items.Contains(dataBitsStr))
                {
                    select_DataBits.Text = dataBitsStr;
                }
                if (select_StopBits.Items.Contains(serialInfo.StopBits.ToString()))
                {
                    select_StopBits.Text = serialInfo.StopBits.ToString();
                }
                if (select_Parity.Items.Contains(serialInfo.Parity.ToString()))
                {
                    select_Parity.Text = serialInfo.Parity.ToString();
                }
            }
        }
        #endregion

        #region 从Ini文件中获取TCP相关参数信息
        private void LoadTcpParamFromIni()
        {
            // 1. 若socket.ini存在，读取参数；不存在则用默认值
            if (File.Exists(socketPath))
            {
                socketInfo = iniService.GetSocketInfoFromPath(socketPath);

                // 2. 将读取到的参数填充到TCP输入框
                input_ip.Text = socketInfo.Ip; // IP地址
                // 端口号：确保是有效数字（避免INI中参数错误）
                if (socketInfo.Port > 0 && socketInfo.Port <= 65535)
                {
                    inputNumber_TCPPort.Value = socketInfo.Port;
                }
                else
                {
                    inputNumber_TCPPort.Value = 502; // 默认Modbus TCP端口
                }
            }
            // 若文件不存在，TCP输入框用默认值（IP空、端口502）
        }
        #endregion

        #region RTU连接开关（屎山）
        private async void btn_switchRTU_ClickAsync(object sender, EventArgs e)
        {

            serialInfo.PortName = select_Serialport.Text;
            serialInfo.BaudRate = int.Parse(select_BaudRate.Text);
            serialInfo.DataBits = int.Parse(select_DataBits.Text);
            serialInfo.StopBits = (StopBits)Enum.Parse(typeof(StopBits), select_StopBits.Text.Trim(), true);
            serialInfo.Parity = (Parity)Enum.Parse(typeof(Parity), select_Parity.Text.Trim(), true);
            if (btn_switchRTU.IconSvg == IconSvg.switch_IconSvg_Success || btn_switchRTU.IconSvg == IconSvg.switch_IconSvg_Fail)
            {
                _readTimer.Stop();
                _modbusDriver.DisConnectRTU();
                //  btn_switchRTU.IconSvg = IconSvg.switch_IconSvg_Default;
                // AntdUI.Modal.open("已断开RTU连接!", AntdUI.TType.Success);
                //AntdUI.Notification.success(this.FindForm(),"PLC", "已断开RTU连接!");
                this.BeginInvoke((Action)(() =>
                {
                    btn_switchRTU.IconSvg = IconSvg.switch_IconSvg_Default;
                    AntdUI.Notification.success(this.FindForm(), "PLC", "已断开RTU连接!");
                }));
              await  LogEventHelper.PushLog(LogType.SystemStartupShutdown, "PLC：RTU断开连接", LogStatu.Success);
                await AddLog(new PLCLogDTO
                {
                    ConnectionType = "RTU",
                    ConnectionParams = serialInfo.PortName,
                    CommunicationType = "断开",
                    IsSuccess = true,
                    Data = $"RTU串口 {serialInfo.PortName} 断开连接"
                }).ConfigureAwait(false);
                //var plc = new PLCLog
                //{
                //    ConnectionType = logDto.ConnectionType,
                //    CommunicationType = logDto.ConnectionParams,
                //    ConnectionParams = logDto.ConnectionParams,
                //    IsSuccess = logDto.IsSuccess,
                //    Data = logDto.Data
                //};
                //_bindLogList.Add(plc);

                //table_PLCLog.Binding(_bindLogList);
                // await _plclogBLL.AddPLCLog(logDto);

                EnableTCP();
                return;
            }


            bool statu = await Task.Run(() =>
            {
                return _modbusDriver.InitializeRTU(
                    serialInfo.PortName,
                    serialInfo.BaudRate,
                    serialInfo.Parity,
                    serialInfo.DataBits,
                    serialInfo.StopBits
                );
            }).ConfigureAwait(false);
            //bool statu = _modbusDriver.InitializeRTU(
            //    serialInfo.PortName,
            //    serialInfo.BaudRate,
            //    serialInfo.Parity,
            //    serialInfo.DataBits,
            //    serialInfo.StopBits
            //);

            if (statu)
            {
                BeginInvoke(new Action(() =>
                {
                    btn_switchRTU.IconSvg = IconSvg.switch_IconSvg_Success;
                    AntdUI.Notification.success(this.FindForm(), "PLC", "RTU连接成功！");
                }));
              
              await  LogEventHelper.PushLog(LogType.SystemStartupShutdown, "PLC：RTU连接成功", LogStatu.Success).ConfigureAwait(false);
                DisableTCP();
                //var logDto = new PLCLogDTO
                //{
                //    ConnectionType = "RTU",
                //    ConnectionParams = serialInfo.PortName,
                //    CommunicationType = "连接",
                //    IsSuccess = true,
                //    Data = $"RTU串口 {serialInfo.PortName} 连接成功"
                //};
                //await _plclogBLL.AddPLCLog(logDto);
                await AddLog(new PLCLogDTO
                {
                    ConnectionType = "RTU",
                    ConnectionParams = serialInfo.PortName,
                    CommunicationType = "连接",
                    IsSuccess = true,
                    Data = $"RTU串口 {serialInfo.PortName} 连接成功"
                }).ConfigureAwait(false);
                _readTimer.Start();
            }
            else
            {
                BeginInvoke(new Action(() =>
                {
                    btn_switchRTU.IconSvg = IconSvg.switch_IconSvg_Fail;
                    AntdUI.Notification.error(this.FindForm(), "PLC", "RTU连接失败！");
                }));
                //var logDto = new PLCLogDTO
                //{
                //    ConnectionType = "RTU",
                //    ConnectionParams = serialInfo.PortName,
                //    CommunicationType = "连接",
                //    IsSuccess = true,
                //    Data = $"RTU串口 {serialInfo.PortName} 连接失败"
                //};
                ////await _plclogBLL.AddPLCLog(logDto);
                //await AddLog(logDto);
                await  LogEventHelper.PushLog(LogType.SystemStartupShutdown, "PLC：RTU连接失败", LogStatu.Failure).ConfigureAwait(false);
                await AddLog(new PLCLogDTO
                {
                    ConnectionType = "RTU",
                    ConnectionParams = serialInfo.PortName,
                    CommunicationType = "连接",
                    IsSuccess = false,
                    Data = $"RTU串口 {serialInfo.PortName} 连接失败",
                    ErrorMessage = "串口初始化失败（可能被占用或参数错误）"
                }).ConfigureAwait(false);
            }
        }
        #endregion()

        #region TCP连接开关（屎山）
        private async void btn_switchTCP_Click(object sender, EventArgs e)
        {
            socketInfo.Ip = input_ip.Text;
            socketInfo.Port = (int)inputNumber_TCPPort.Value;
            
            if (btn_switchTCP.IconSvg == IconSvg.switch_IconSvg_Success||btn_switchTCP.IconSvg  == IconSvg.switch_IconSvg_Fail)
            {
                _readTimer.Stop();
                _modbusDriver.DisConnectTCP();
                // btn_switchTCP.IconSvg = IconSvg.switch_IconSvg_Default;
                //  AntdUI.Modal.open("已断开TCP连接!", AntdUI.TType.Info);
                // AntdUI.Message.success(this.FindForm(), "已断开TCP连接!");
                // AntdUI.Notification.success(this.FindForm(), "PLC", "已断开TCP连接!");
                 BeginInvoke((Action)(() =>
                {
                    btn_switchTCP.IconSvg = IconSvg.switch_IconSvg_Default;
                    AntdUI.Notification.success(this.FindForm(), "PLC", "已断开TCP连接!");
                }));
                // var logDto = new PLCLogDTO
                // {
                //     ConnectionType = "TCP",
                //     ConnectionParams = $"{socketInfo.Ip}:{socketInfo.Port}",
                //     CommunicationType = "断开",
                //     IsSuccess = true,
                //     Data = $"TCP网口 {socketInfo.Ip}:{socketInfo.Port} 断开连接"
                // };
                //await  AddLog(logDto);
                // await _plclogBLL.AddPLCLog(logDto);
              await  LogEventHelper.PushLog(LogType.SystemStartupShutdown, "PLC：TCP断开连接", LogStatu.Success).ConfigureAwait(false);
                await AddLog(new PLCLogDTO
                {
                    ConnectionType = "TCP",
                    ConnectionParams = $"{socketInfo.Ip}:{socketInfo.Port}",
                    CommunicationType = "断开",
                    IsSuccess = true,
                    Data = $"TCP {socketInfo.Ip}:{socketInfo.Port} 断开连接"
                }).ConfigureAwait(false);
               
                SwitchStatusChanged?.Invoke(this, new DeviceSwitchStatusChangedEventArgs(DeviceSwitchType.PLC_Switch, false));
                return;
            }

            //bool statu = 
            //   _modbusDriver.InitializeTCP(
            //   socketInfo.Ip,
            //   socketInfo.Port);
            bool statu = await Task.Run(() =>
            {
                return _modbusDriver.InitializeTCP(socketInfo.Ip, socketInfo.Port);
            }).ConfigureAwait(false);
            if (statu)
            {
                BeginInvoke(new Action(() =>
                {
                    btn_switchTCP.IconSvg = IconSvg.switch_IconSvg_Success;
                    AntdUI.Notification.success(this.FindForm(), "PLC", "TCP连接开启！");
                }));
                // AntdUI.Modal.open("TCP连接开启！", AntdUI.TType.Success);
                // AntdUI.Message.success(this.FindForm(), "TCP连接开启！");
                // AntdUI.Notification.success(this.FindForm(), "PLC", "TCP连接开启！");
                DisableRTU();
             await   LogEventHelper.PushLog(LogType.SystemStartupShutdown, "PLC：TCP连接成功", LogStatu.Success).ConfigureAwait(false);
                await AddLog(new PLCLogDTO
                {
                    ConnectionType = "TCP",
                    ConnectionParams = $"{socketInfo.Ip}:{socketInfo.Port}",
                    CommunicationType = "连接",
                    IsSuccess = true,
                    Data = $"TCP {socketInfo.Ip}:{socketInfo.Port} 连接成功"
                });
                _readTimer.Start();
                SwitchStatusChanged?.Invoke(this, new DeviceSwitchStatusChangedEventArgs(DeviceSwitchType.PLC_Switch, true));
            }
            else
            {
                BeginInvoke(new Action(() =>
                {
                    btn_switchTCP.IconSvg = IconSvg.switch_IconSvg_Fail;
                    AntdUI.Notification.error(this.FindForm(), "PLC", "TCP连接失败！");
                }));
                // AntdUI.Modal.open("TCP连接失败！", AntdUI.TType.Error);
                // AntdUI.Notification.error(this.FindForm(), "PLC", "TCP连接失败！");
                await LogEventHelper.PushLog(LogType.SystemStartupShutdown, "PLC：TCP连接失败", LogStatu.Failure).ConfigureAwait(false);
                await AddLog(new PLCLogDTO
                {
                    ConnectionType = "TCP",
                    ConnectionParams = $"{socketInfo.Ip}:{socketInfo.Port}",
                    CommunicationType = "连接",
                    IsSuccess = false,
                    Data = $"TCP {socketInfo.Ip}:{socketInfo.Port} 连接失败",
                    ErrorMessage = "IP/端口错误或PLC无响应"
                });
                SwitchStatusChanged?.Invoke(this, new DeviceSwitchStatusChangedEventArgs(DeviceSwitchType.PLC_Switch, false));
            }
           
        }
        #endregion

        #region 保存RTU参数到Ini
        private void btn_SaveRTU_Click(object sender, EventArgs e)
        {
            if (this.serialInfo == null) this.serialInfo = new SerialInfo();
            this.serialInfo.PortName = this.select_Serialport.Text.Trim();
            this.serialInfo.BaudRate = Convert.ToInt32(
                this.select_BaudRate.Text.Trim());
            this.serialInfo.Parity = (Parity)Enum.Parse(typeof(Parity),
                this.select_Parity.Text.Trim(), true);
            this.serialInfo.DataBits = Convert.ToInt32(
                this.select_DataBits.Text.Trim());
            this.serialInfo.StopBits = (StopBits)Enum.Parse(typeof(StopBits), this.select_StopBits.Text.Trim(), true);
            bool result = iniService.SetSerialInfoToPath(serialInfo, serialPath);
            BeginInvoke((Action)(async () =>
            {
                if (result)
                {
                    AntdUI.Modal.open("配置信息保存成功", "保存配置", AntdUI.TType.Success);
                await    LogEventHelper.PushLog(LogType.ConfigurationChange, "PLC：RTU参数信息保存成功", LogStatu.Success).ConfigureAwait(false);
                }
                else
                {
                    AntdUI.Modal.open("配置信息保存失败", "保存配置", AntdUI.TType.Error);
                 await   LogEventHelper.PushLog(LogType.ConfigurationChange, "PLC：RTU参数信息保存失败", LogStatu.Failure).ConfigureAwait(false);
                }
            }));
          }
        #endregion

        #region 保存TCP参数到Ini
        private void btn_SaveTCP_Click(object sender, EventArgs e)
        {
            if (this.socketInfo == null) this.socketInfo = new SocketInfo();
            this.socketInfo.Ip = this.input_ip.Text.Trim();
            this.socketInfo.Port = (int)inputNumber_TCPPort.Value;
            bool result = iniService.SetSocketInfoToPath(socketInfo, socketPath);
            BeginInvoke((Action)(async () =>
            {
                if (result)
                {
                    AntdUI.Modal.open("配置信息保存成功" + socketPath + "", "保存配置", AntdUI.TType.Success);
                 await   LogEventHelper.PushLog(LogType.ConfigurationChange, "PLC：TCP参数信息保存成功", LogStatu.Success).ConfigureAwait(false);
                }
                else
                {
                    AntdUI.Modal.open("配置信息保存失败", "保存配置", AntdUI.TType.Error);
                  await  LogEventHelper.PushLog(LogType.ConfigurationChange, "PLC：TCP参数信息保存失败", LogStatu.Failure).ConfigureAwait(false);
                }
            }));
        }
        #endregion

        #region PLC窗体加载事件
        private async void PLC_LoadAsync(object sender, EventArgs e)
        {
            table_Equipmentlocation.CellEndEdit += Table_Equipmentlocation_CellEndEdit;
            table_Equipmentlocation.FixedHeader = true;
            table_Equipmentlocation.Bordered = true;
            var list = await _plcvalueBLL.ShowSlaveVarToTable().ConfigureAwait(false);
            _bindingList = new BindingList<PLCSlaveVariable>(list);
            if (table_Equipmentlocation.InvokeRequired)
            {
                table_Equipmentlocation.BeginInvoke(new Action(() =>
                {
                    table_Equipmentlocation.DataSource = _bindingList;
                }));
            }
            else
            {
                table_Equipmentlocation.DataSource = _bindingList;
            }

        }
        #endregion

        # region 添加日志到数据库和表格
       
        private async Task AddLog(PLCLogDTO dto)
        {
            try
            {

                dto.CommunicationTime = dto.CommunicationTime == default ? DateTime.Now : dto.CommunicationTime;

                var logEntity = await _plclogBLL.AddPLCLog(dto).ConfigureAwait(false);

                this.BeginInvoke((Action)(() =>
                {
                    _bindLogList.Insert(0, logEntity); 
                    table_PLCLog.Refresh(); 
                }));
            }
            catch (Exception ex)
            {
              
                await  LogEventHelper.PushLog(LogType.DatabaseOperation,$"日志记录失败：{ex.Message}",LogStatu.Error).ConfigureAwait(false);
                
            }
        }
        #endregion

        #region 更新PLC按钮状态
        public void UpdatePLCSwitch(DeviceSwitchType switchType, bool isOn)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<DeviceSwitchType, bool>(UpdatePLCSwitch), switchType, isOn);
                return;
            }

           
            // 处理TCP（同步执行，确保图标更新）
             if (switchType == DeviceSwitchType.PLC_Switch)
            {
                bool currentState = btn_switchTCP.IconSvg == IconSvg.switch_IconSvg_Success;
                if (isOn != currentState)
                {
                    btn_switchTCP.BeginInvoke(new Action(() =>
                    {
                        btn_switchTCP_Click(null, EventArgs.Empty);
                    }));
                }
            }

        }
        #endregion

        #region 单击单元格进行修改数据事件(根据条件判断是否可以修改该单元格)

        private PLCSlaveVariable _currentEditingRow;
        //private bool Table_Equipmentlocation_CellEndEdit(object sender, TableEndEditEventArgs e)
        //{
        //    if (e.Column.Key != "CurrentValue" || _currentEditingRow == null)
        //        return false;
        //    double oldValue = _currentEditingRow.CurrentValue;
        //    string newValue = e.Value?.ToString()?.Trim();
        //    if (string.IsNullOrEmpty(newValue))
        //    {
        //        AntdUI.Notification.warn(this.FindForm(), "PLC", "输入值不能为空");

        //        return false;
        //    }
        //    _isSubmittingEdit = true;

        //    _ = ExecuteEditAsync(oldValue, newValue); 


        //    return true;

        //}
        private bool Table_Equipmentlocation_CellEndEdit(object sender, TableEndEditEventArgs e)
        {
            if (e.Column.Key != "CurrentValue") return false;

            var row = e.Record as PLCSlaveVariable;
            if (row == null || !row.IsWritable) return false;

            if (!double.TryParse(e.Value?.ToString(), out var newVal))
            {
                AntdUI.Notification.warn(this.FindForm(), "PLC", "请输入合法数字");
                return false;
            }

            _currentEditingRow = row;
            _ = ExecuteEditAsync(row.CurrentValue, newVal);
            return true;
        }

        //private async void Table_Equipmentlocation_CellEndEdit(object sender, TableEndEditEventArgs e)
        //{
        //    if (e.Column.Key != "CurrentValue" || _currentEditingRow == null)
        //        return false;

        //    try
        //    {
        //        string newValue = e.Value?.ToString()?.Trim();
        //        if (string.IsNullOrEmpty(newValue))
        //        {
        //            AntdUI.Modal.open("输入值不能为空", AntdUI.TType.Warn);
        //            return false;
        //        }

        //        var updateDto = new PLCValueUpdateDTO
        //        {
        //            Id = _currentEditingRow.Id,
        //            ModbusType = _currentEditingRow.ModbusType,
        //            SlaveId = _currentEditingRow.SlaveAddress,
        //            Address = _currentEditingRow.ModbusAddress,
        //            Value = e.Value?.ToString()
        //        };
        //        var result = await _plcvalueBLL.UpdateSlaveFromScadaAsync(updateDto).ConfigureAwait(false); 
        //        if (result==null)
        //        {
        //            AntdUI.Modal.open("变量更新失败（业务逻辑返回失败）", AntdUI.TType.Error);
        //            return false;
        //        }
        //        var newDataSource = await _plcvalueBLL.ShowSlaveVarToTable().ConfigureAwait(false); 

        //        _bindingList = new BindingList<PLCSlaveVariable>(newDataSource);
        //        table_Equipmentlocation.DataSource = _bindingList;
        //        table_Equipmentlocation.Refresh();

        //        //var logDto = new PLCLogDTO
        //        //{
        //        //    SlaveAddress = _currentEditingRow.SlaveAddress,
        //        //    AddressInfo = _currentEditingRow.ModbusAddress,
        //        //    CommunicationType = "写入", // 修正为"写入"而非DataType
        //        //    ConnectionType = btn_switchRTU.IconSvg == IconSvg.switch_IconSvg_Success ? "RTU" : "TCP",
        //        //    ConnectionParams = btn_switchRTU.IconSvg == IconSvg.switch_IconSvg_Success ? serialInfo.PortName : $"{socketInfo.Ip}:{socketInfo.Port}",
        //        //    IsSuccess = true,
        //        //    Data = e.Value?.ToString()
        //        //};
        //        //var log = Task.Run(async () =>
        //        //{
        //        //    return await _plclogBLL.AddPLCLog(logDto).ConfigureAwait(false);
        //        //}).Result;

        //        //AddLog(logDto).ConfigureAwait(false);
        //        //table_PLCLog.DataSource = log;
        //        //table_PLCLog.Refresh();
        //        //AntdUI.Modal.open("变量更新成功", AntdUI.TType.Success);
        //        //return true;
        //        LogEventHelper.PushLog(LogType.SystemOperation, $"PLC：地址 {_currentEditingRow.ModbusAddress} 从 {_currentEditingRow.CurrentValue} 改为 {e.Value}成功", LogStatu.Success);
        //        await AddLog(new PLCLogDTO
        //        {
        //            SlaveAddress = _currentEditingRow.SlaveAddress,
        //            AddressInfo = _currentEditingRow.ModbusAddress,
        //            CommunicationType = "写入",
        //            ConnectionType = btn_switchRTU.IconSvg == IconSvg.switch_IconSvg_Success ? "RTU" : "TCP",
        //            ConnectionParams = btn_switchRTU.IconSvg == IconSvg.switch_IconSvg_Success ? serialInfo.PortName : $"{socketInfo.Ip}:{socketInfo.Port}",
        //            IsSuccess = true,
        //            Data = $"地址 {_currentEditingRow.ModbusAddress} 从 {_currentEditingRow.CurrentValue} 改为 {e.Value}"
        //        }).ConfigureAwait(false);

        //        AntdUI.Modal.open("变量更新成功", AntdUI.TType.Success);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        var errorMsg = ex.InnerException != null ? $"{ex.Message} -> {ex.InnerException.Message}" : ex.Message;
        //        //var logDto = new PLCLogDTO
        //        //{
        //        //    SlaveAddress = _currentEditingRow.SlaveAddress,
        //        //    AddressInfo = _currentEditingRow.ModbusAddress,
        //        //    IsSuccess = false,
        //        //    CommunicationType = _currentEditingRow.DataType,
        //        //    Data = e.Value?.ToString(),
        //        //    ErrorMessage = errorMsg,
        //        //};
        //        //var log = Task.Run(async () =>
        //        //{
        //        //    return await _plclogBLL.AddPLCLog(logDto).ConfigureAwait(false);
        //        //}).Result;

        //        // AddLog(logDto).ConfigureAwait(false);
        //        //table_PLCLog.DataSource = log;
        //        //table_PLCLog.Refresh();
        //        //AntdUI.Modal.open($"更新失败：{errorMsg}", AntdUI.TType.Error);
        //        //return false;
        //        LogEventHelper.PushLog(LogType.SystemOperation, $"PLC：地址 {_currentEditingRow.ModbusAddress} 从 {_currentEditingRow.CurrentValue} 改为 {e.Value}失败", LogStatu.Failure);
        //      await  AddLog(new PLCLogDTO
        //        {
        //            SlaveAddress = _currentEditingRow.SlaveAddress,
        //            AddressInfo = _currentEditingRow.ModbusAddress,
        //            CommunicationType = "写入",
        //            ConnectionType = btn_switchRTU.IconSvg == IconSvg.switch_IconSvg_Success ? "RTU" : "TCP",
        //            ConnectionParams = btn_switchRTU.IconSvg == IconSvg.switch_IconSvg_Success ? serialInfo.PortName : $"{socketInfo.Ip}:{socketInfo.Port}",
        //            IsSuccess = false,
        //            Data = $"尝试将地址 {_currentEditingRow.ModbusAddress} 改为 {e.Value}",
        //            ErrorMessage = errorMsg
        //        }).ConfigureAwait(false); ;
        //        AntdUI.Modal.open($"更新失败：{errorMsg}", AntdUI.TType.Error);
        //        return false;
        //    }
        //    finally
        //    {

        //            table_Equipmentlocation.EditMode = AntdUI.TEditMode.None;
        //            table_Equipmentlocation.CellEndEdit -= Table_Equipmentlocation_CellEndEdit;
        //            _currentEditingRow = null;

        //    }
        //}

        //private async Task ExecuteEditAsync(double oldValue, string newValue)
        //{
        //    double originalValue = oldValue;
        //    string originalValueString = oldValue.ToString();
        //    if (!double.TryParse(newValue, out double newValueParsed)) // 先验证格式
        //    {
        //        BeginInvoke(new Action(() =>
        //        {
        //            AntdUI.Notification.error(this.FindForm(), "PLC", "输入值不是有效数字！");
        //            _isSubmittingEdit = false;
        //            table_Equipmentlocation.EditMode = AntdUI.TEditMode.None;
        //            _currentEditingRow = null;
        //        }));
        //        return;
        //    }
        //    try
        //    {
        //        var updateDto = new PLCValueUpdateDTO
        //        {
        //            Id = _currentEditingRow.Id,
        //            ModbusType = _currentEditingRow.ModbusType,
        //            SlaveId = _currentEditingRow.SlaveAddress,
        //            Address = _currentEditingRow.ModbusAddress,
        //            Value = newValue
        //        };

        //        // 异步写入PLC
        //        var result = await _plcvalueBLL.UpdateSlaveFromScadaAsync(updateDto).ConfigureAwait(false);
        //        if (result == null)
        //        {
        //            await LogEventHelper.PushLog(LogType.PLCCommunication, "PLC:变量更新失败（业务返回空）", LogStatu.Failure).ConfigureAwait(false);
        //            BeginInvoke((Action)(() =>
        //            {
        //                _currentEditingRow.CurrentValue = originalValue;
        //                _currentEditingRow.UpdateTime = DateTime.Now; // 这里可以不变，因为更新失败了，但为了显示错误，我们可能不想更新时间
        //                AntdUI.Notification.error(this.FindForm(), "PLC", "变量更新失败");
        //            }));
        //            return;
        //        }

        //        // 确定连接类型
        //        string connType = btn_switchRTU.IconSvg == IconSvg.switch_IconSvg_Success ? "RTU" : "TCP";
        //        string connParams = connType == "RTU" ? serialInfo.PortName : $"{socketInfo.Ip}:{socketInfo.Port}";

        //        // 异步添加成功日志
        //        await AddLog(new PLCLogDTO
        //        {
        //            SlaveAddress = _currentEditingRow.SlaveAddress,
        //            AddressInfo = _currentEditingRow.ModbusAddress,
        //            CommunicationType = "写入",
        //            ConnectionType = connType,
        //            ConnectionParams = connParams,
        //            IsSuccess = true,
        //            Data = $"地址 {_currentEditingRow.ModbusAddress} 从 {oldValue} 改为 {newValue}"
        //        }).ConfigureAwait(false);

        //        await LogEventHelper.PushLog(LogType.PLCCommunication, $"变量更新成功：{_currentEditingRow.ModbusAddress}从 {oldValue} 改为 {newValue}", LogStatu.Success).ConfigureAwait(false);

        //        // UI线程更新表格
        //        BeginInvoke((Action)(() =>
        //        {
        //            _currentEditingRow.CurrentValue = double.Parse(newValue);
        //            _currentEditingRow.UpdateTime = DateTime.Now;
        //            AntdUI.Notification.success(this.FindForm(), "PLC", "变量更新成功");
        //        }));
        //    }
        //    catch (Exception ex)
        //    {
        //        string errorMsg = ex.InnerException != null ? $"{ex.Message} -> {ex.InnerException.Message}" : ex.Message;
        //        string connType = btn_switchRTU.IconSvg == IconSvg.switch_IconSvg_Success ? "RTU" : "TCP";
        //        string connParams = connType == "RTU" ? serialInfo.PortName : $"{socketInfo.Ip}:{socketInfo.Port}";

        //        // 异步添加错误日志
        //        await AddLog(new PLCLogDTO
        //        {
        //            SlaveAddress = _currentEditingRow.SlaveAddress,
        //            AddressInfo = _currentEditingRow.ModbusAddress,
        //            CommunicationType = "写入",
        //            ConnectionType = connType,
        //            ConnectionParams = connParams,
        //            IsSuccess = false,
        //            Data = $"尝试将地址 {_currentEditingRow.ModbusAddress} 从 {oldValue} 改为 {newValue}",
        //            ErrorMessage = errorMsg
        //        }).ConfigureAwait(false);

        //        await LogEventHelper.PushLog(LogType.PLCCommunication, $"变量更新失败：{errorMsg}", LogStatu.Failure).ConfigureAwait(false);

        //    }
        //    finally
        //    {
        //        // UI线程重置状态
        //        BeginInvoke((Action)(() =>
        //        {
        //            table_Equipmentlocation.EditMode = AntdUI.TEditMode.None;
        //            //table_Equipmentlocation.CellEndEdit -= Table_Equipmentlocation_CellEndEdit;
        //            _currentEditingRow = null;
        //            _isSubmittingEdit = false;
        //        }));
        //    }

        //}
       
        #endregion

        #region 真正异步的执行函数
        private async Task ExecuteEditAsync(double oldVal, double newVal)
        {
            if (_currentEditingRow == null)
            {
                await LogEventHelper.PushLog(LogType.PLCCommunication, "PLC写入操作失败：当前编辑行数据为空", LogStatu.Error).ConfigureAwait(false);
                this.BeginInvoke(new Action(() =>
                {
                    AntdUI.Notification.error(this.FindForm(), "PLC", "编辑数据异常，请重试");
                }));
                return;
            }

            // 校验关键属性（ModbusAddress 为空时给默认值，.NET 4.8 支持 ??）
            int modbusAddress;
            bool isAddressValid = true;
            modbusAddress = _currentEditingRow.ModbusAddress;
            if (modbusAddress < 0)
            {
                isAddressValid = false;
            }
            if (!isAddressValid)
            {
                await LogEventHelper.PushLog(LogType.PLCCommunication, $"PLC写入操作失败：变量ID={_currentEditingRow.Id} 的Modbus地址无效（{modbusAddress}）", LogStatu.Error).ConfigureAwait(false);
                this.BeginInvoke(new Action(() =>
                {
                    AntdUI.Notification.error(this.FindForm(), "PLC", "变量地址异常（无效地址），无法写入");
                }));
                return;
            }

            var dto = new PLCValueUpdateDTO
            {
                Id = _currentEditingRow.Id,
                SlaveId = _currentEditingRow.SlaveAddress,
                Address = modbusAddress, // 使用校验后的地址
                ModbusType = _currentEditingRow.ModbusType,
                Value = newVal.ToString()
            };

            try
            {
                // 1. 异步写 PLC（.NET 4.8 支持 await + ConfigureAwait(false)）
                var ok = await _plcvalueBLL.UpdateSlaveFromScadaAsync(dto).ConfigureAwait(false);
                if (ok == null) throw new Exception("PLC 写入返回失败");

                // 2. 异步写日志（适配 .NET 4.8：用传统 switch 替代 switch 表达式）
                string connType = btn_switchRTU.IconSvg == IconSvg.switch_IconSvg_Success ? "RTU" : "TCP";
                string connParams;
                // .NET 4.8 不支持 switch 表达式，改为传统 switch
                switch (connType)
                {
                    case "RTU":
                        connParams = serialInfo.PortName ?? "未知串口"; // 空合并运算符 .NET 4.8 支持
                        break;
                    case "TCP":
                        string ip = socketInfo.Ip ?? "未知IP";
                        connParams = $"{ip}:{socketInfo.Port}";
                        break;
                    default:
                        connParams = "未知连接";
                        break;
                }

                // 拼接日志时避免空引用（所有属性都加空校验）
                await AddLog(new PLCLogDTO
                {
                    SlaveAddress = _currentEditingRow.SlaveAddress,
                    AddressInfo = modbusAddress,
                    CommunicationType = "写入",
                    ConnectionType = connType,
                    ConnectionParams = connParams,
                    IsSuccess = true,
                    Data = $"地址 {modbusAddress} 从 {oldVal} 改为 {newVal}"
                }).ConfigureAwait(false);

                // 3. 回到 UI 线程更新（.NET 4.8 支持 BeginInvoke）
                this.BeginInvoke(new Action(() =>
                {
                    _currentEditingRow.CurrentValue = newVal;
                    _currentEditingRow.UpdateTime = DateTime.Now;
                    table_Equipmentlocation.Refresh();
                    AntdUI.Notification.success(this.FindForm(), "PLC", "变量更新成功");
                }));
            }
            catch (Exception ex)
            {
                // 异常日志同样适配 .NET 4.8
                string connType = btn_switchRTU.IconSvg == IconSvg.switch_IconSvg_Success ? "RTU" : "TCP";
                string connParams;
                switch (connType)
                {
                    case "RTU":
                        connParams = serialInfo.PortName ?? "未知串口";
                        break;
                    case "TCP":
                        string ip = socketInfo.Ip ?? "未知IP";
                        connParams = $"{ip}:{socketInfo.Port}";
                        break;
                    default:
                        connParams = "未知连接";
                        break;
                }

                await AddLog(new PLCLogDTO
                {
                    SlaveAddress = _currentEditingRow.SlaveAddress,
                    AddressInfo = modbusAddress,
                    CommunicationType = "写入",
                    ConnectionType = connType,
                    ConnectionParams = connParams,
                    IsSuccess = false,
                    ErrorMessage = ex.Message,
                    Data = $"尝试将地址 {modbusAddress} 从 {oldVal} 改为 {newVal}"
                }).ConfigureAwait(false);

                this.BeginInvoke(new Action(() =>
                {
                    AntdUI.Notification.error(this.FindForm(), "PLC", $"写入失败：{ex.Message}");
                }));
            }
            finally
            {
                // UI 线程清理状态（.NET 4.8 支持 BeginInvoke）
                this.BeginInvoke(new Action(() =>
                {
                    table_Equipmentlocation.EditMode = AntdUI.TEditMode.None;
                    _currentEditingRow = null; // 最后重置，避免残留空引用
                }));
            }
        }

      
        #endregion

        #region 当UDP启用时禁用TCP协议各种控件
        public void DisableTCP()
        {
            if (input_ip.InvokeRequired) 
            {
                input_ip.BeginInvoke(new Action(() =>
                {
                    input_ip.Enabled = false;
                    inputNumber_TCPPort.Enabled = false;
                    btn_switchTCP.Enabled = false;
                    btn_SaveTCP.Enabled = false;
                }));
            }
            else
            {
                input_ip.Enabled = false;
                inputNumber_TCPPort.Enabled = false;
                btn_switchTCP.Enabled = false;
                btn_SaveTCP.Enabled = false;
            }
        }
        #endregion

        #region 当TCP启用时禁用UDP协议各种控件
        public void DisableRTU()
        {
            if (select_Serialport.InvokeRequired)
            {
                select_Serialport.BeginInvoke(new Action(() =>
                {
                    select_Serialport.Enabled = false;
                    select_BaudRate.Enabled = false;
                    select_DataBits.Enabled = false;
                    select_StopBits.Enabled = false;
                    select_Parity.Enabled = false;
                    btn_switchRTU.Enabled = false;
                    btn_SaveRTU.Enabled = false;
                }));
            }
            else
            {
                select_Serialport.Enabled = false;
                select_BaudRate.Enabled = false;
                select_DataBits.Enabled = false;
                select_StopBits.Enabled = false;
                select_Parity.Enabled = false;
                btn_switchRTU.Enabled = false;
                btn_SaveRTU.Enabled = false;
            }
        }
        #endregion

        #region 启用TCP协议各种控件状态
        private void EnableTCP()
        {
            if (input_ip.InvokeRequired)
            {
                input_ip.BeginInvoke(new Action(() =>
                {
                    input_ip.Enabled = true;
                    inputNumber_TCPPort.Enabled = true;
                    btn_switchTCP.Enabled = true;
                    btn_SaveTCP.Enabled = true;
                }));
            }
            else
            {
                input_ip.Enabled = true;
                inputNumber_TCPPort.Enabled = true;
                btn_switchTCP.Enabled = true;
                btn_SaveTCP.Enabled = true;
            }
        }
        #endregion

        #region Table单击单元格事件
        private void table_Equipmentlocation_CellClick(object sender, TableClickEventArgs e)
        {
            if (_isSubmittingEdit) return;
            if (btn_switchRTU.IconSvg != IconSvg.switch_IconSvg_Success && btn_switchTCP.IconSvg != IconSvg.switch_IconSvg_Success)
            {
                AntdUI.Modal.open("请先连接PLC!", AntdUI.TType.Warn);
                return;
            }

            var currentRowData = e.Record as PLCSlaveVariable;
            if (currentRowData == null)
            {
                AntdUI.Modal.open("数据异常，无法获取当前行信息", AntdUI.TType.Error);
                return;
            }

            if (e.Column.Key != "CurrentValue" || !currentRowData.IsWritable)
            {
                if (!currentRowData.IsWritable)
                    AntdUI.Modal.open("当前变量不可写，无法编辑", AntdUI.TType.Warn);
                return;
            }

            if (_currentEditingRow == null)
            {
                _currentEditingRow = currentRowData;
                table_Equipmentlocation.CellEndEdit -= Table_Equipmentlocation_CellEndEdit;
                table_Equipmentlocation.CellEndEdit += Table_Equipmentlocation_CellEndEdit;

                table_Equipmentlocation.EditMode = AntdUI.TEditMode.DoubleClick;
                table_Equipmentlocation.EnterEditMode(e.RowIndex, e.ColumnIndex);
            }
            else
            {
                AntdUI.Notification.warn(this.FindForm(), "PLC", "已有正在编辑的行，请完成后再操作");
            }

        }
        #endregion

    }
}

