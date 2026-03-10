using AntdUI;
using AntdUI.Chat;
using HalconControl;
using HalconDotNet;
using HZH_Controls;
using HZH_Controls.Controls;
using SpeechLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wedjat.BLL;
using Wedjat.Driver;
using Wedjat.Driver.Model;
using Wedjat.Helper;
using Wedjat.Model.Config;
using Wedjat.Model.DTO;
using Wedjat.Model.Entity;
using Wedjat.WinForm.CameraMenuUI;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static Wedjat.Model.DTO.WorkOrderResponse;

namespace Wedjat.WinForm.MainMenuUI
{
    public partial class Console : UserControl
    {
        private BindingList<PCBDefectDetail> _defectBindingList;
        public event EventHandler<DeviceSwitchStatusChangedEventArgs> SwitchStatusChanged;
        public event EventHandler<bool> Sensor2TriggerChanged;
        public event EventHandler<WorkOrderResponse> WorkOrderSyncRequested;
        public event EventHandler<MesWorkOrderPCB> PCBSyncRequested;
        
        private MVSDriver _cameradriver;
        private readonly NModbusDriver _modbusdriver;
        private System.Timers.Timer _statusReadTimer;
        private bool _isAutoControllingConv4ByDetection = false;
        private int _fixedConv4Speed = 50; // 固定速度值
        private bool _isAutoModeByDetectionEnabled = true; // 是否启用检测后自动模式
        private bool _isDetectionCompleted = false; // 检测是否完成
        private DateTime _lastDetectionTime = DateTime.MinValue; 
        private bool _isTimerRunning = false;
        private List<MesWorkOrderPCB> _currentPCB = new List<MesWorkOrderPCB>();
        //private BindingList<MesPCBDefect> _currentPCBDefect = new BindingList<MesPCBDefect>();
        private List<MesPCBDefect> _currentPCBDefect=new List<MesPCBDefect>();
     
        public MesWorkOrderPCB _currentSelectedPCB { get; set; }
        // private readonly ConcurrentQueue<SysLog> _logQueue = new ConcurrentQueue<SysLog>();
        //  public event EventHandler<ScannerStatusEventArgs> ScannerControlRequested;
        public PLCSlaveVariableBLL _plcbll;
        public SysLogBLL _syslogbll;
        private CollectionControl _collectionControl;
        private WorkOrderResponse _currentWorkOrder;
        private readonly SpVoice _voice = new SpVoice();
        public BindingList<SysLog> _sysLogs = new BindingList<SysLog>();
        string resultText;
        string resultColor;
        string resultVoice;
        private Input[] _totalLabels = new Input[4];   
        private Input[] _ngLabels = new Input[4];      
        private Input[] _okLabels = new Input[4];
        private bool _isConv1On = false;
        private bool _isConv2On = false;
        private bool _isConv3On = false;
        private bool _isConv4On = false;
        private bool _isConv5On = false;
        private bool _isSensor1On = false;
        private bool _isSensor2On = false;
        private bool _isSensor3On = false;
        private int _conv1Speed;
        private int _conv2Speed;
        private int _conv3Speed;
        private int _conv4Speed;
        private int _conv5Speed;
        private readonly ConcurrentDictionary<string, bool> _statusCache = new ConcurrentDictionary<string, bool>
        {
            ["Conv1_Run"] = false,
            ["Conv2_Run"] = false,
            ["Conv3_Run"] = false,
            ["Conv4_Run"] = false,
            ["Conv5_Run"] = false,
            ["Sensor1_Trigger"] = false,
            ["Sensor2_Trigger"] = false,
            ["Sensor3_Trigger"] = false
        };

        public Console(MVSDriver cameradriver,NModbusDriver nModbusDriver)
        {
            InitializeComponent();
            InitDeviceStatus();
            
           _cameradriver = cameradriver;
            _modbusdriver = nModbusDriver;
            _plcbll = new PLCSlaveVariableBLL(_modbusdriver);
            _syslogbll = new SysLogBLL();
           
            table_DefectStatistics.Columns = new AntdUI.ColumnCollection
            {
              
                new AntdUI.Column("DefectTypeName", "缺陷类型"),
                new AntdUI.Column("DefectCount", "缺陷数量")
            };
            _defectBindingList = new BindingList<PCBDefectDetail>();
    
            //foreach (var mesDefect in _currentPCBDefect)
            //{
            //    _defectBindingList.Add(new PCBDefectDetail
            //    {
            //        DefectTypeName = mesDefect.DefectName,
            //        DefectCount = mesDefect.DefectCount   
                   
            //    });
            //}
            table_DefectStatistics.DataSource = _defectBindingList;
            table_DefectStatistics.FixedHeader = true;
            table_DefectStatistics.Bordered = true;
            table_Logrecording.Columns = new AntdUI.ColumnCollection
            {
                  
                  new AntdUI.Column("LogType", "日志类型")  {
                       Render = (value, record, rowIndex) =>
                        {

                            var log = record as SysLog;
                            LogType logType = log.LogType;
                            string text;
                            switch (logType)
                            {
                                case LogType.SystemOperation:
                                    text = "系统提示";
                                    break;
                                case LogType.UserOperation:
                                    text = "用户操作";
                                    break;
                                case  LogType.ScanOperation:
                                    text = "扫码日志";
                                    break;
                                case LogType.InspectionOperation:
                                    text = "检测日志";
                                    break;
                                case LogType.PLCCommunication:
                                    text = "PLC日志";
                                    break;
                                case LogType.MESApiCall:
                                    text ="MesAPI日志"; 
                                    break;
                                case LogType.DatabaseOperation:
                                    text ="数据库日志";
                                    break;
                                case LogType.SystemStartupShutdown:
                                    text ="系统状态";
                                    break;
                                case LogType.ConfigurationChange:
                                    text ="配置变更";
                                    break;
                                default:
                                    text = "未定义状态";
                                    break;
                            }

                            return new AntdUI.CellTag(text);
                        }
                  }.SetFixed(),
                  new AntdUI.Column("Detail", "详细信息"),
                  new AntdUI.Column("UpdateTime", "时间", ColumnAlign.Center),
                  new AntdUI.Column("LogStatu", "状态", ColumnAlign.Center)
                  {
                       Render = (value, record, rowIndex) =>
                        {
                            var log = record as SysLog;
                            LogStatu logStatu = log.LogStatu;
                            string text;
                            AntdUI.TTypeMini type;
                            switch (logStatu)
                            {
                                case LogStatu.System:
                                    text = "提示";
                                    type = AntdUI.TTypeMini.Info;
                                    break;
                                case LogStatu.Warning:
                                    text = "警告";
                                    type = AntdUI.TTypeMini.Warn;
                                    break;
                                case LogStatu.Error:
                                    text = "错误";
                                    type = AntdUI.TTypeMini.Error;
                                    break;
                                case LogStatu.Failure:
                                    text = "失败";
                                    type = AntdUI.TTypeMini.Default;
                                    break;
                                case LogStatu.Success:
                                    text = "成功";
                                    type = AntdUI.TTypeMini.Success;
                                    break;
                                default:
                                    text = "未定义状态";
                                    type = AntdUI.TTypeMini.Default;
                                    break;
                            }
                            return new AntdUI.CellTag(text, type);
                        }
                  }
                  
            };
            menulist = new AntdUI.IContextMenuStripItem[]
            {
                        new AntdUI.ContextMenuStripItem("刷新"),
                        new AntdUI.ContextMenuStripItem("清空"),
                        new AntdUI.ContextMenuStripItem("导出")
            };
            LogEventHelper.Log += LogReceived;
            _statusReadTimer = new System.Timers.Timer(500);
            _statusReadTimer.Elapsed += async (s, e) => await AutoReadStatusAsync();
            _statusReadTimer.AutoReset = true; 
            _statusReadTimer.Enabled = false;


            _totalLabels[0] = TotalTests1;
            _totalLabels[1] = TotalTests2;
            _totalLabels[2] = TotalTests3;
            _totalLabels[3] = TotalTests4;
            _ngLabels[0] = NGCount1;
            _ngLabels[1] = NGCount2;
            _ngLabels[2] = NGCount3;
            _ngLabels[3] = NGCount4;
            _okLabels[0] = OKCount1;
            _okLabels[1] = OKCount2;
            _okLabels[2] = OKCount3;
            _okLabels[3] = OKCount4;
            
            table_Logrecording.Binding(_sysLogs);
        }

       
        #region 日志接收事件
        private async void LogReceived(object sender, LogEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<object, LogEventArgs>(LogReceived), sender, e);
                return;
            }
           await  AddLog(e.LogType, e.Message, e.Status);
        }
        #endregion

        #region 添加日志事件
        public async Task AddLog(LogType logType, string message, LogStatu status)
        {
            var sysLog = new SysLog
            {
                LogType = logType,
                Detail = message,
                UpdateTime = DateTime.Now,
                LogStatu = status
            };
             await _syslogbll.InsertAsync(sysLog);

           
            if (table_Logrecording.InvokeRequired)
            {
                table_Logrecording.BeginInvoke(new Action(() =>
                {
                    _sysLogs.Add(new SysLog
                    {
                        LogType = logType,
                        Detail = message,
                        UpdateTime = DateTime.Now,
                        LogStatu = status
                    });

                    if (_sysLogs.Count > 0)
                    {
                        int lastRowIndex = _sysLogs.Count - 1;
                        table_Logrecording.ScrollLine(lastRowIndex, true);
                    }
                }));
            }
            else
            {
               _sysLogs.Add(new SysLog
                {
                    LogType = logType,
                    Detail = message,
                    UpdateTime = DateTime.Now,
                    LogStatu = status
                });
               

                if (_sysLogs.Count > 0)
                {
                    int lastRowIndex = _sysLogs.Count - 1;
                    table_Logrecording.ScrollLine(lastRowIndex, true);
                }
            }
        }
        #endregion

        #region  采集控制传递过来的信息
        public void SetCollectionControl(CollectionControl collectionControl)
        {
            if (_collectionControl != null)
            {
                _collectionControl.DetectionCompleted -= CollectionControl_DetectionCompleted;
                _collectionControl.DefectStatisticsUpdated -= CollectionControl_DefectStatisticsUpdated;
                _collectionControl.WorkOrderDetectionUpdated -= CollectionControl_WorkOrderDetectionUpdated;
            }

            // 第二步：重新赋值并订阅
            _collectionControl = collectionControl;
            if (_collectionControl != null)
            {
                _collectionControl.DetectionCompleted += CollectionControl_DetectionCompleted;
                _collectionControl.DefectStatisticsUpdated += CollectionControl_DefectStatisticsUpdated;
                _collectionControl.WorkOrderDetectionUpdated += CollectionControl_WorkOrderDetectionUpdated;
            }
        }

        private void CollectionControl_DefectStatisticsUpdated(object sender, PCBDefectStatisticsEventArgs e)
        {
            _ = AddLog(LogType.InspectionOperation,
       $"收到缺陷数据：PCBSN={e.PCBSN}，缺陷类型={e.DefectName}，数量={e.DefectCount}",
       LogStatu.Success);
            if (this.InvokeRequired)
            {
                // 跨线程安全调用（必须用BeginInvoke，避免UI卡顿）
                this.BeginInvoke(new Action<PCBDefectStatisticsEventArgs>(UpdateDefectCountByNames), e);
                return;
            }

            // 主线程直接更新
            UpdateDefectCountByNames(e);
        }
        private  void UpdateDefectCountByNames(PCBDefectStatisticsEventArgs newDefectData)
        {
            // 1. 校验传递过来的数据是否有效
            if (newDefectData == null || string.IsNullOrEmpty(newDefectData.DefectName) || _currentPCBDefect == null)
            {
                AntdUI.Notification.warn(this.FindForm(), "缺陷数据无效", "未收到有效缺陷统计数据");
                return;
            }

            // 2. 显示PCBSN（保持原有逻辑）
            label_PCBSN.Text = string.IsNullOrEmpty(newDefectData.PCBSN) ? "未知PCBSN" : newDefectData.PCBSN;
            string targetDefectName = newDefectData.DefectName.Trim();
            // 3. 按缺陷名称匹配 _currentPCBDefect（从工单加载的缺陷列表）
            //    var matchedDefect = _defectBindingList.FirstOrDefault(
            //item => item.DefectTypeName?.Trim() == targetDefectName);
            var existingDefect = _defectBindingList.FirstOrDefault(
           item => item.DefectTypeName?.Trim().Equals(targetDefectName, StringComparison.OrdinalIgnoreCase) == true);
            if (existingDefect != null)
            {
                existingDefect.DefectCount = newDefectData.DefectCount;
               // Console.WriteLine($"更新缺陷数量: {targetDefectName} = {newDefectData.DefectCount}");
            }
            else
            {
                // 6. 如果没有找到，添加新行
                _defectBindingList.Add(new PCBDefectDetail
                {
                    DefectTypeName = targetDefectName,
                    DefectCount = newDefectData.DefectCount
                });
               // Console.WriteLine($"添加新缺陷行: {targetDefectName} = {newDefectData.DefectCount}");
            }

            // 7. 刷新表格
            table_DefectStatistics.Refresh();
        }
        // 处理检测完成事件，显示处理后图像
        private void CollectionControl_DetectionCompleted(object sender, DetectionResultEventArgs e)
        {
            _isDetectionCompleted = true;
            _lastDetectionTime = DateTime.Now;
            if (hWindow_Final1.InvokeRequired)
            {
                hWindow_Final1.Invoke(new Action<DetectionResultEventArgs>(ShowProcessedImage), e);
            }
            else
            {
                ShowProcessedImage(e);
            }
            if (_isAutoModeByDetectionEnabled)
            {
                _ = AutoControlConv4AfterDetectionAsync();
            }
        }
        #endregion

        #region 检测完成后自动控制Conveyor 4
        private async Task AutoControlConv4AfterDetectionAsync()
        {
            if (!switch_PLC.Checked || !_isDetectionCompleted)
            {
                await AddLog(LogType.PLCCommunication,
                    "PLC未开启或检测未完成，无法自动控制Conveyor 2",
                    LogStatu.Warning);
                return;
            }

            _isAutoControllingConv4ByDetection = true;

            try
            {
                // 检测完成：打开Conv4并设置固定速度
                await Task.WhenAll(
                    // 打开Conv4
                    _plcbll.UpdateCurrentValueByVariableNameAsync(new PLCValueUpdateDTO
                    {
                        VariableName = "Conv4_Run",
                        Value = 1
                    }),
                    // 设置固定速度
                    _plcbll.UpdateCurrentValueByVariableNameAsync(new PLCValueUpdateDTO
                    {
                        VariableName = "Conv4_SpeedSet",
                        Value = _fixedConv4Speed
                    })
                );

                // 更新本地状态
                _isConv4On = true;
                _conv4Speed = _fixedConv4Speed;

                // 更新UI
                this.Invoke(new Action(() =>
                {
                    btn_Conv4.IconSvg = IconSvg.switch_IconSvg_Success;
                    slider_Conv4.Value = _fixedConv4Speed;

                    // 显示通知（可选）
                    AntdUI.Notification.info(this.FindForm(),
                        "自动控制",
                        $"检测完成，Conveyor 4已启动（速度: {_fixedConv4Speed}）");
                }));

                await AddLog(LogType.PLCCommunication,
                    $"检测完成后自动控制：Conveyor 4已启动，速度设置为{_fixedConv4Speed}",
                    LogStatu.Success);

                // 启动定时器，在一定时间后自动停止（模拟PCB传送走）
                StartAutoStopTimer();
            }
            catch (Exception ex)
            {
                await AddLog(LogType.PLCCommunication,
                    $"检测完成后自动控制Conveyor 4失败：{ex.Message}",
                    LogStatu.Error);

                this.Invoke(new Action(() =>
                {
                    AntdUI.Notification.error(this.FindForm(),
                        "自动控制失败",
                        $"检测完成后控制Conveyor 4失败：{ex.Message}");
                }));
            }
            finally
            {
                _isAutoControllingConv4ByDetection = false;
                _isDetectionCompleted = false; // 重置检测完成标志
            }
        }

        // 自动停止定时器（模拟PCB传送走的时间）
        private void StartAutoStopTimer()
        {
            var stopTimer = new System.Timers.Timer(3000); // 3秒后停止
            stopTimer.Elapsed += async (s, e) =>
            {
                stopTimer.Stop();
                stopTimer.Dispose();

                await AutoStopConv4AfterTransportAsync();
            };
            stopTimer.AutoReset = false;
            stopTimer.Start();

            this.Invoke(new Action(() =>
            {
                AntdUI.Notification.info(this.FindForm(),
                    "传送计时",
                    "Conveyor 4已启动，3秒后自动停止");
            }));
        }

        // 自动停止Conveyor 4
        private async Task AutoStopConv4AfterTransportAsync()
        {
            if (!switch_PLC.Checked || !_isConv4On) return;

            try
            {
                await Task.WhenAll(
                    // 关闭Conv4
                    _plcbll.UpdateCurrentValueByVariableNameAsync(new PLCValueUpdateDTO
                    {
                        VariableName = "Conv4_Run",
                        Value = 0
                    }),
                    // 设置速度为0
                    _plcbll.UpdateCurrentValueByVariableNameAsync(new PLCValueUpdateDTO
                    {
                        VariableName = "Conv4_SpeedSet",
                        Value = 0
                    })
                );

                // 更新本地状态
                _isConv4On = false;
                _conv4Speed = 0;

                // 更新UI
                this.Invoke(new Action(() =>
                {
                    btn_Conv4.IconSvg = IconSvg.switch_IconSvg_Default;
                    slider_Conv4.Value = 0;

                    AntdUI.Notification.info(this.FindForm(),
                        "传送完成",
                        "PCB已传送走，Conveyor 4已停止");
                }));

                await AddLog(LogType.PLCCommunication,
                    "PCB传送完成，Conveyor 4已自动停止",
                    LogStatu.Success);
            }
            catch (Exception ex)
            {
                await AddLog(LogType.PLCCommunication,
                    $"自动停止Conveyor 4失败：{ex.Message}",
                    LogStatu.Error);
            }
        }
        #endregion

        #region HObject转字节(抄的茗工的,时间太久了,记不清了，抄的，然后AI改的)
        private byte[] HObjectToBytes(HObject halconImage)
        {
            if (halconImage == null || !halconImage.IsInitialized())
            {
                throw new ArgumentNullException(nameof(halconImage), "Halcon 图像未初始化");
            }

            try
            {
                // 1. 获取图像基本信息（宽、高、通道数）
                HTuple width = new HTuple(), height = new HTuple(), channels = new HTuple();
                HOperatorSet.GetImageSize(halconImage, out width, out height);
                HOperatorSet.CountChannels(halconImage, out channels);

                // 2. 统一转成 RGB 3通道图（灰度图直接复制到三通道）
                HObject rgbImage = halconImage;
                if (channels.I == 1) // 灰度图转 RGB
                {
                    HTuple pointer = new HTuple(), type = new HTuple();
                    HOperatorSet.GetImagePointer1(halconImage, out pointer, out type, out width, out height);
                    HOperatorSet.GenImage3(out rgbImage, type, width, height, pointer, pointer, pointer);
                }

                // 确认现在是3通道
                HOperatorSet.CountChannels(rgbImage, out channels);

                // 3. Halcon 图像转成 .NET Bitmap
                int w = width.I;
                int h = height.I;
                Bitmap bitmap = new Bitmap(w, h, PixelFormat.Format24bppRgb);
                Rectangle rect = new Rectangle(0, 0, w, h);
                BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, bitmap.PixelFormat);

                // 4. 根据通道数拷贝数据到 Bitmap（注意 Bitmap 的行对齐 stride）
                try
                {
                    int stride = Math.Abs(bitmapData.Stride);
                    // 目标缓冲区：按 stride 分配，包含行填充
                    byte[] pixels = new byte[stride * h];

                    if (channels.I == 3)
                    {
                        // Halcon 的 GetImagePointer3 返回每个通道的指针（按 channel plane 存储）
                        HTuple ptrR = new HTuple(), ptrG = new HTuple(), ptrB = new HTuple(), imgType = new HTuple(), outW = new HTuple(), outH = new HTuple();
                        HOperatorSet.GetImagePointer3(rgbImage, out ptrR, out ptrG, out ptrB, out imgType, out outW, out outH);

                        IntPtr rPtr = ptrR.IP;
                        IntPtr gPtr = ptrG.IP;
                        IntPtr bPtr = ptrB.IP;

                        if (rPtr == IntPtr.Zero || gPtr == IntPtr.Zero || bPtr == IntPtr.Zero)
                            throw new InvalidOperationException("获取图像通道指针失败");

                        // 按像素组装 BGR 数据（Bitmap.Format24bppRgb 要求 B,G,R 顺序）
                        for (int y = 0; y < h; y++)
                        {
                            int rowDst = y * stride;
                            int rowSrcOffset = y * w; // 每个通道每行宽度为 w 字节
                            for (int x = 0; x < w; x++)
                            {
                                int srcIndex = rowSrcOffset + x;
                                int dstIndex = rowDst + x * 3;

                                // 读取通道数据（每通道按行连续）
                                byte r = System.Runtime.InteropServices.Marshal.ReadByte(rPtr, srcIndex);
                                byte g = System.Runtime.InteropServices.Marshal.ReadByte(gPtr, srcIndex);
                                byte b = System.Runtime.InteropServices.Marshal.ReadByte(bPtr, srcIndex);

                                pixels[dstIndex + 0] = b;
                                pixels[dstIndex + 1] = g;
                                pixels[dstIndex + 2] = r;
                            }
                        }
                    }
                    else if (channels.I == 1)
                    {
                        // 单通道：GetImagePointer1 返回单个指针，之前在 GenImage3 已经生成三通道，理论上不会走到这里
                        HTuple ptr = new HTuple(), type = new HTuple();
                        HOperatorSet.GetImagePointer1(halconImage, out ptr, out type, out width, out height);
                        IntPtr p = ptr.IP;
                        if (p == IntPtr.Zero) throw new InvalidOperationException("获取灰度图像指针失败");

                        for (int y = 0; y < h; y++)
                        {
                            int rowDst = y * stride;
                            int rowSrcOffset = y * w;
                            for (int x = 0; x < w; x++)
                            {
                                byte v = System.Runtime.InteropServices.Marshal.ReadByte(p, rowSrcOffset + x);
                                int dstIndex = rowDst + x * 3;
                                pixels[dstIndex + 0] = v;
                                pixels[dstIndex + 1] = v;
                                pixels[dstIndex + 2] = v;
                            }
                        }
                    }
                    else
                    {
                        throw new NotSupportedException($"不支持的通道数：{channels.I}");
                    }

                    // 将像素数组复制到 Bitmap 内存
                    System.Runtime.InteropServices.Marshal.Copy(pixels, 0, bitmapData.Scan0, pixels.Length);
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }

                // 5. 保存为 PNG 字节数组
                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                _ = AddLog(LogType.InspectionOperation, $"图像转字节数组失败：{ex.Message}", LogStatu.Error);
                return null;
            }
            finally
            {
                // 释放 Halcon 图像对象，避免内存泄漏
                if (halconImage != null && halconImage.IsInitialized())
                {
                    halconImage.Dispose();
                }
            }
        }

        #endregion

        #region 海康控件显示检测后的图像
        // 显示处理后的图像（带标注）
        //private void ShowProcessedImage(DetectionResultEventArgs e)
        //{
        //    if (hWindow_Final1 == null || !hWindow_Final1.IsHandleCreated)
        //    {
        //        AntdUI.Notification.warn(this.FindForm(), "图像显示失败", "hWindow_Final1 控件未初始化");
        //        return;
        //    }
        //    if (e.OriginalImage == null || !e.OriginalImage.IsInitialized())
        //    {
        //        AntdUI.Notification.warn(this.FindForm(), "图像显示失败", "原始图像为空或已释放");
        //        return;
        //    }
        //    // 修复：仅声明一次missingRect，避免重复
        //    HObject missingRect = null;
        //    HObject markedImage = null;
        //    try
        //    {
        //        hWindow_Final1.ClearWindow();
        //        hWindow_Final1.HobjectToHimage(e.OriginalImage);
        //        HTuple windowID = hWindow_Final1.HWindowHalconID;
        //        bool isWindowIdValid = false;
        //        try
        //        {
        //            isWindowIdValid = windowID != null
        //                            && !windowID.IsEmpty()
        //                            && windowID.Length > 0
        //                            && windowID.I != 0;
        //        }
        //        catch (HOperatorException ex)
        //        {
        //            _ = AddLog(LogType.InspectionOperation, $"校验Halcon窗口ID出错：{ex.Message}", LogStatu.Error);
        //            isWindowIdValid = false;
        //        }

        //        if (!isWindowIdValid)
        //        {
        //            string errorDetail = windowID == null ? "窗口ID为null" :
        //                                 windowID.IsEmpty() ? "窗口ID为空" :
        //                                 $"窗口ID值为{windowID.I}（无效）";
        //            AntdUI.Notification.error(this.FindForm(), "图像显示失败", $"Halcon窗口ID无效：{errorDetail}（内核对象未初始化或窗口已销毁）");
        //            return;
        //        }
        //        if (e.MissingCount != null && e.MissingCount.I > 0 && e.MissingRegions != null && e.MissingRegions.IsInitialized())
        //        {
        //            HTuple row1 = new HTuple(), col1 = new HTuple(), row2 = new HTuple(), col2 = new HTuple();
        //            HOperatorSet.SmallestRectangle1(e.MissingRegions, out row1, out col1, out row2, out col2);

        //            // 修复：不再重新声明missingRect，直接给已有变量赋值
        //            HOperatorSet.GenRectangle1(out missingRect, row1, col1, row2, col2);
        //            if (missingRect.IsInitialized())
        //            {
        //                hWindow_Final1.DispObj(missingRect, "red");
        //            }

        //            // 释放矩形资源
        //            missingRect?.Dispose();
        //            missingRect = null; // 避免二次释放
        //        }

        //        // 显示 Pass/Fail 大字体
        //        bool isFail = (e.MissingCount != null && e.MissingCount.I > 0);
        //        resultVoice = isFail ? "Fail" : "Pass";
        //        resultText = isFail ? "NG" : "OK";
        //        resultColor = isFail ? "red" : "green";
        //        hWindow_Final1.DispTxt(resultText, resultColor, 72);

        //        // 修复：DumpWindowImage参数正确（HWindowHalconID）
        //        HOperatorSet.DumpWindowImage(out markedImage, windowID);
        //        System.Console.WriteLine(hWindow_Final1.HWindowHalconID);
        //        byte[] markedImageBytes = HObjectToBytes(markedImage);
        //        if (markedImageBytes != null && markedImageBytes.Length > 0)
        //        {
        //            if (!string.IsNullOrEmpty(e.PCBSN))
        //            {
        //                _ = _collectionControl.UpdateMarkedImageToDatabase(e.PCBSN, markedImageBytes);
        //                _ = AddLog(LogType.InspectionOperation, $"标注图已保存：PCBSN={e.PCBSN}", LogStatu.Success);
        //            }
        //            else
        //            {
        //                _ = AddLog(LogType.InspectionOperation, "标注图保存失败：PCBSN为空", LogStatu.Warning);
        //            }
        //        }
        //        else
        //        {
        //            _ = AddLog(LogType.InspectionOperation, "标注图转字节数组失败", LogStatu.Error);
        //        }

        //        ReadResultWithSpVoiceAsync(resultVoice);
        //        TestNumberInfo.TotalCount++;
        //        if (isFail)
        //        {
        //            TestNumberInfo.NGCount++;
        //            _ = AddLog(LogType.InspectionOperation, "PCB不良品", LogStatu.Failure);
        //        }
        //        else
        //        {
        //            TestNumberInfo.OKCount++;
        //            _ = AddLog(LogType.InspectionOperation, "PCB检测通过", LogStatu.Success);
        //        }
        //        UpdateCountUI();
        //    }
        //    catch (Exception ex)
        //    {
        //        AntdUI.Notification.error(this.FindForm(), "处理后图像显示失败", ex.Message);
        //    }
        //    finally
        //    {
        //        // 最终释放资源（避免内存泄漏）
        //        missingRect?.Dispose();
        //        markedImage?.Dispose();
        //    }
        //}
        private void ShowProcessedImage(DetectionResultEventArgs e)
        {
            if (hWindow_Final1 == null || !hWindow_Final1.IsHandleCreated)
            {
                AntdUI.Notification.warn(this.FindForm(), "图像显示失败", "hWindow_Final1 控件未初始化");
                return;
            }

            // 优先使用标注图像，如果没有则使用原始图像
            HObject displayImage = e.AnnotatedImage ?? e.OriginalImage;

            if (displayImage == null || !displayImage.IsInitialized())
            {
                AntdUI.Notification.warn(this.FindForm(), "图像显示失败", "图像为空或已释放");
                return;
            }

            HObject missingRect = null;

            try
            {
                hWindow_Final1.ClearWindow();

                // 直接显示标注后的图像
                hWindow_Final1.HobjectToHimage(displayImage);

                // 获取窗口ID用于后续操作
                HTuple windowID = hWindow_Final1.HWindowHalconID;

                // 验证窗口ID是否有效
                if (windowID == null || windowID.Length == 0 || windowID.I == 0)
                {
                    AntdUI.Notification.error(this.FindForm(), "窗口ID无效", "Halcon窗口未正确初始化");
                    return;
                }

                // 注意：如果已经使用了标注图像，就不需要再绘制缺陷区域了
                // 因为标注图像已经包含了缺陷标记

                // 显示检测结果文本
                bool isFail = (e.MissingCount != null && e.MissingCount.I > 0);
                resultVoice = isFail ? "Fail" : "Pass";
                resultText = isFail ? "NG" : "OK";
                resultColor = isFail ? "red" : "green";
                hWindow_Final1.DispTxt(resultText, resultColor, 72);

                // 不再需要截图保存到数据库，因为CollectionControl已经保存了标注图像
                // 数据库中的图像已经是标注后的版本

                // 语音播报和计数
                ReadResultWithSpVoiceAsync(resultVoice);
                TestNumberInfo.TotalCount++;
                if (isFail)
                {
                    TestNumberInfo.NGCount++;
                    _ = AddLog(LogType.InspectionOperation, "PCB不良品", LogStatu.Failure);
                }
                else
                {
                    TestNumberInfo.OKCount++;
                    _ = AddLog(LogType.InspectionOperation, "PCB检测通过", LogStatu.Success);
                }
                UpdateCountUI();
            }
            catch (Exception ex)
            {
                AntdUI.Notification.error(this.FindForm(), "图像显示失败", ex.Message);
                _ = AddLog(LogType.InspectionOperation, $"图像显示异常：{ex.Message}", LogStatu.Error);
                _isDetectionCompleted = true;
                _lastDetectionTime = DateTime.Now;
                if (_isAutoModeByDetectionEnabled)
                {
                    _ = AutoControlConv4AfterDetectionAsync();
                }
            }
            finally
            {
                // 释放资源
                missingRect?.Dispose();
                // 注意：不要释放displayImage，因为它来自事件参数
            }
        }
        //private async void ShowProcessedImage(DetectionResultEventArgs e)
        //{
        //    if (hWindow_Final1?.IsHandleCreated != true || e.AnnotatedImage == null || !e.AnnotatedImage.IsInitialized())
        //    {
        //        string detail = string.Empty;
        //        if (hWindow_Final1?.IsHandleCreated != true) detail += "窗口句柄未创建；";
        //        if (e.AnnotatedImage == null || !e.AnnotatedImage.IsInitialized()) detail += "标注图句柄为空；";
        //        AntdUI.Notification.warn(this.FindForm(), "图像显示失败", $"控件或标注图无效：{detail}");
        //        _ = AddLog(LogType.InspectionOperation, $"图像显示失败：{detail}，PCBSN={e.PCBSN}", LogStatu.Warning);
        //        return;
        //    }

        //    HObject windowImage = null;

        //    try
        //    {
        //        // 直接使用HObject，不需要转换
        //        HObject annotatedImageObj = e.AnnotatedImage;

        //        // 检查图像对象是否有效
        //        if (annotatedImageObj == null || !annotatedImageObj.IsInitialized())
        //        {
        //            AntdUI.Notification.warn(this.FindForm(), "图像显示失败", "标注图对象无效");
        //            _ = AddLog(LogType.InspectionOperation, $"标注图对象无效，PCBSN={e.PCBSN}", LogStatu.Warning);
        //            return;
        //        }

        //        // 校验标注图尺寸
        //        HTuple width, height;
        //        HOperatorSet.GetImageSize(annotatedImageObj, out width, out height);
        //        if (width.Length <= 0 || height.Length <= 0 || width.I <= 0 || height.I <= 0)
        //        {
        //            AntdUI.Notification.warn(this.FindForm(), "图像显示失败", "标注图尺寸无效");
        //            _ = AddLog(LogType.InspectionOperation, $"标注图尺寸无效，PCBSN={e.PCBSN}", LogStatu.Warning);
        //            return;
        //        }

        //        // 直接显示图像，避免使用GetImagePointer1
        //        hWindow_Final1.ClearWindow();
        //        hWindow_Final1.HobjectToHimage(annotatedImageObj);

        //        // 记录显示成功
        //        string expectedPath = Path.Combine(@"C:\Users\Lenovo\Desktop\CSharp项目\就业项目\Wedjat\PCBInspectionImages", $"{e.PCBSN}.png");
        //        _ = AddLog(LogType.InspectionOperation,
        //            $"标注图显示成功，PCBSN={e.PCBSN}",
        //            LogStatu.Success);

        //        // 截图并保存为字节数组（使用安全方法）
        //        if (TryDumpWindowImage(out windowImage))
        //        {
        //            byte[] pngBytes = SafeHObjectToBytes(windowImage);

        //            // 保存到数据库
        //            if (pngBytes?.Length > 0 && !string.IsNullOrEmpty(e.PCBSN))
        //            {
        //                await _collectionControl.UpdateMarkedImageToDatabase(e.PCBSN, pngBytes).ConfigureAwait(false);
        //                _ = AddLog(LogType.InspectionOperation, $"标注图存库成功：PCBSN={e.PCBSN}", LogStatu.Success);
        //            }
        //            else
        //            {
        //                _ = AddLog(LogType.InspectionOperation, $"标注图存库失败：字节长度{pngBytes?.Length ?? 0}，PCBSN={e.PCBSN}", LogStatu.Warning);
        //            }
        //        }

        //        // 语音播报和计数
        //        bool fail = e.MissingCount != null && e.MissingCount.Length > 0 && e.MissingCount.I > 0;
        //        resultVoice = fail ? "Fail" : "Pass";
        //        ReadResultWithSpVoiceAsync(resultVoice);

        //        Invoke(new Action(() =>
        //        {
        //            TestNumberInfo.TotalCount++;
        //            if (fail) TestNumberInfo.NGCount++; else TestNumberInfo.OKCount++;
        //            UpdateCountUI();
        //        }));
        //    }
        //    catch (HalconException hex)
        //    {
        //        string errorMsg = $"HALCON错误 #{hex.GetErrorCode()}: {hex.GetErrorMessage()}";
        //        AntdUI.Notification.error(this.FindForm(), "HALCON处理失败", errorMsg);
        //        _ = AddLog(LogType.InspectionOperation, $"HALCON错误：PCBSN={e.PCBSN}，错误：{errorMsg}", LogStatu.Error);
        //    }
        //    catch (Exception ex)
        //    {
        //        AntdUI.Notification.error(this.FindForm(), "处理标注图失败", ex.Message);
        //        _ = AddLog(LogType.InspectionOperation, $"处理标注图失败：PCBSN={e.PCBSN}，错误：{ex.Message}", LogStatu.Error);
        //    }
        //    finally
        //    {
        //        windowImage?.Dispose();
        //        // 注意：e中的HObject由调用者负责释放，不要在这里释放e.AnnotatedImage
        //    }
        //}

        #endregion

        #region 安全调用DumpWindowImage（捕获异常并记录日志）
        private bool TryDumpWindowImage(out HObject image)
        {
            image = null;
            try
            {
                HTuple wnd = hWindow_Final1.HWindowHalconID;
                if (wnd == null || wnd.IsEmpty() || wnd.I == 0)
                {
                    _ = AddLog(LogType.InspectionOperation, "窗口句柄无效，跳过截图", LogStatu.Warning);
                    return false;
                }
                HOperatorSet.DumpWindowImage(out image, wnd);
                return image.IsInitialized();
            }
            catch (HOperatorException ex)
            {
                _ = AddLog(LogType.InspectionOperation, $"截图失败：{ex.Message}", LogStatu.Error);
                return false;
            }
        }
        #endregion

        #region HObject转字节的安全调用（代码有些重复，当时脑袋写昏掉了）
        private byte[] SafeHObjectToBytes(HObject halconImage)
        {
            if (halconImage == null || !halconImage.IsInitialized())
            {
                _ = AddLog(LogType.InspectionOperation, "截图图像未初始化，转字节取消", LogStatu.Warning);
                return null;
            }
            return HObjectToBytes(halconImage);
        }
        #endregion

        #region 检测结果语音播报功能
        private async void ReadResultWithSpVoiceAsync(string resultVoice)
        {
            if (string.IsNullOrEmpty(resultVoice) || IsDisposed)
                return;

            try
            {
                await Task.Run(() =>
                {
                    _voice.Volume = 100;
                    _voice.Rate = -5;
                    _voice.Speak(resultVoice, SpeechVoiceSpeakFlags.SVSFlagsAsync);
                });
            }
            catch (Exception ex)
            {
                this.BeginInvoke((Action)(() =>
                {
                    AntdUI.Notification.warn(this.FindForm(), "语音提示失败", $"SpVoice朗读失败：{ex.Message}");
                }));
            }
        }
        #endregion

        #region 更新计数
        private void UpdateCountUI()
        {
            if (TestNumberInfo.TotalCount > 9999
                || TestNumberInfo.NGCount > 9999
                || TestNumberInfo.OKCount > 9999)
            {
                TestNumberInfo.ResetCounts();
            }
            int[] totalDigits = TestNumberInfo.SplitTotalCount(TestNumberInfo.TotalCount);
            for (int i = 0; i < 4; i++)
            {
                _totalLabels[i].Text = totalDigits[i].ToString();
            }

            int[] ngDigits = TestNumberInfo.SplitTotalCount(TestNumberInfo.NGCount);
            for (int i = 0; i < 4; i++)
            {
                _ngLabels[i].Text = ngDigits[i].ToString();
            }

            int[] okDigits = TestNumberInfo.SplitTotalCount(TestNumberInfo.OKCount);
            for (int i = 0; i < 4; i++)
            {
                _okLabels[i].Text = okDigits[i].ToString();
            }
        }
        #endregion

        #region 自动读取站地址状态(屎山)
        //private async Task AutoReadStatusAsync()
        //{

        //    if (_modbusdriver == null || !_isTimerRunning || IsDisposed)
        //        return;
        //    await Task.Run(async () =>
        //    {
        //        try
        //        {

        //            bool readSuccess = false;
        //            try
        //            {
        //                await _plcbll.ReadSlaveFromPLCAsync().ConfigureAwait(false);
        //                readSuccess = true;

        //            }
        //            catch (Exception ex)
        //            {

        //                if (!IsDisposed)
        //                {
        //                    this.BeginInvoke((Action)(() =>
        //                    {
        //                        AntdUI.Notification.error(this.FindForm(), "PLC", $"同步Slave到数据库失败：{ex.Message}");
        //                    }));
        //                }
        //            }

        //            if (!readSuccess)
        //                return;



        //            var statusUpdates = new Dictionary<string, bool>();
        //            var convConfigs = new List<string>
        //        {
        //            "Conv1_Run", "Conv2_Run", "Conv3_Run","Conv4_Run",  "Conv5_Run",
        //            "Conv1_SpeedSet","Conv2_SpeedSet","Conv3_SpeedSet","Conv4_SpeedSet","Conv5_SpeedSet",//
        //            "Sensor1_Trigger", "Sensor2_Trigger", "Sensor3_Trigger"
        //        };


        //            // 并行读取所有变量值
        //            var valueTasks = convConfigs.Select(varName =>
        //            _plcbll.GetCurrentValueByVariableNameAsync(
        //                new PLCValueUpdateDTO { VariableName = varName })
        //            ).ToList();
        //            var valueResults = await Task.WhenAll(valueTasks).ConfigureAwait(false);


        //            foreach (var varName in valueResults)
        //            {
        //                var variable = variables.FirstOrDefault(v =>
        //                v.VariableName.Equals(varName, StringComparison.OrdinalIgnoreCase));

        //                if (variable != null)
        //                {
        //                    statusUpdates[varName] = variable.CurrentValue == 1;
        //                }
        //            }
        //            var needUpdate = new Dictionary<string, bool>();
        //            foreach (var item in statusUpdates)
        //            {
        //                string varName = item.Key;
        //                bool newStatus = item.Value;
        //                if (_statusCache.TryGetValue(varName, out bool oldStatus) && oldStatus != newStatus)
        //                {
        //                    needUpdate[varName] = newStatus;
        //                    _statusCache[varName] = newStatus;
        //                }
        //            }


        //            if (needUpdate.Count > 0 && !IsDisposed)
        //            {
        //                this.BeginInvoke((Action)(() =>
        //                {
        //                    if (IsDisposed) return;
        //                    bool isPLCClosed = !switch_PLC.Checked;
        //                    foreach (var item in needUpdate)
        //                    {
        //                        UpdateControlStatus(item.Key, item.Value, isPLCClosed);
        //                        if (item.Key == "Sensor2_Trigger")
        //                        {
        //                            Sensor2TriggerChanged?.Invoke(this, item.Value);
        //                        }
        //                    }
        //                }));
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            if (!IsDisposed)
        //            {
        //                this.BeginInvoke((Action)(async () =>
        //                {
        //                    await LogEventHelper.PushLog(LogType.PLCCommunication, $"PLC：定时器更新失败：{ex.Message}", LogStatu.Failure).ConfigureAwait(false);
        //                }));
        //            }
        //        }
        //    });

        //}
        private async Task AutoReadStatusAsync()
        {
            if (_modbusdriver == null || !_isTimerRunning || IsDisposed)
                return;

            try
            {
                // 从数据库读取变量配置
                var variables = await _plcbll.ShowSlaveVarToTable();

                // 需要监控的状态变量
                var statusVariables = new List<string>
        {
            "Conv1_Run", "Conv2_Run", "Conv3_Run", "Conv4_Run", "Conv5_Run",
            "Sensor1_Trigger", "Sensor2_Trigger", "Sensor3_Trigger"
        };

                var statusUpdates = new Dictionary<string, bool>();

                // 并行读取状态
                foreach (var varName in statusVariables)
                {
                    try
                    {
                        var variable = variables.FirstOrDefault(v =>
                            v.VariableName.Equals(varName, StringComparison.OrdinalIgnoreCase));

                        if (variable != null)
                        {
                            int value = await _plcbll.GetCurrentValueByVariableNameAsync(
                                new PLCValueUpdateDTO { VariableName = varName });

                            statusUpdates[varName] = value == 1;
                        }
                    }
                    catch (Exception ex)
                    {
                        await LogEventHelper.PushLog(LogType.PLCCommunication,
                            $"读取变量 {varName} 失败: {ex.Message}",
                            LogStatu.Error);
                    }
                }

                // 检查状态变化
                var needUpdate = new Dictionary<string, bool>();
                foreach (var item in statusUpdates)
                {
                    string varName = item.Key;
                    bool newStatus = item.Value;

                    if (_statusCache.TryGetValue(varName, out bool oldStatus) && oldStatus != newStatus)
                    {
                        needUpdate[varName] = newStatus;
                        _statusCache[varName] = newStatus;
                    }
                }

                // 更新UI
                if (needUpdate.Count > 0 && !IsDisposed)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        if (IsDisposed) return;

                        bool isPLCClosed = !switch_PLC.Checked;
                        foreach (var item in needUpdate)
                        {
                            UpdateControlStatus(item.Key, item.Value, isPLCClosed);

                            if (item.Key == "Sensor2_Trigger")
                            {
                                Sensor2TriggerChanged?.Invoke(this, item.Value);
                            }
                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                if (!IsDisposed)
                {
                    this.BeginInvoke(new Action(async () =>
                    {
                        await LogEventHelper.PushLog(LogType.PLCCommunication,
                            $"PLC定时器更新失败：{ex.Message}",
                            LogStatu.Failure);
                    }));
                }
            }
        }
        #endregion

        #region 根据变量名更新各控件的状态
        private void UpdateControlStatus(string varName, bool status, bool isPLCClosed)
        {
            switch (varName)
            {
                case "Conv1_Run":
                    btn_Conv1.IconSvg = status ? IconSvg.switch_IconSvg_Success : IconSvg.switch_IconSvg_Default;
                    _isConv1On = status;
                    btn_Conv1.Enabled = !isPLCClosed;
                    slider_Conv1.Enabled = !isPLCClosed && status;
                    break;
                case "Conv2_Run":
                    btn_Conv2.IconSvg = status ? IconSvg.switch_IconSvg_Success : IconSvg.switch_IconSvg_Default;
                    _isConv2On = status;
                    btn_Conv2.Enabled = !isPLCClosed;
                    slider_Conv2.Enabled = !isPLCClosed && status;
                    break;
                case "Conv3_Run":
                    btn_Conv3.IconSvg = status ? IconSvg.switch_IconSvg_Success : IconSvg.switch_IconSvg_Default;
                    _isConv3On = status;
                    btn_Conv3.Enabled = !isPLCClosed;
                    slider_Conv3.Enabled = !isPLCClosed && status;
                    break;
                case "Conv4_Run":
                    btn_Conv4.IconSvg = status ? IconSvg.switch_IconSvg_Success : IconSvg.switch_IconSvg_Default;
                    _isConv4On = status;
                    btn_Conv4.Enabled = !isPLCClosed;
                    slider_Conv4.Enabled = !isPLCClosed && status;
                    break;
                case "Conv5_Run":
                    btn_Conv5.IconSvg = status ? IconSvg.switch_IconSvg_Success : IconSvg.switch_IconSvg_Default;
                    _isConv5On = status;
                    btn_Conv5.Enabled = !isPLCClosed;
                    slider_Conv5.Enabled = !isPLCClosed && status;
                    break;
                case "Sensor1_Trigger":
                    label_Sensor1Statu.Text = status ? "触发中" : "未触发";
                    label_Sensor1Statu.PrefixColor = status ? Color.Orange : Color.Green;
                    _isSensor1On = status;
                    break;
                case "Sensor2_Trigger":
                    label_Sensor2Statu.Text = status ? "触发中" : "未触发";
                    label_Sensor2Statu.PrefixColor = status ? Color.Orange : Color.Green;
                    _isSensor2On = status;
                    break;
                case "Sensor3_Trigger":
                    label_Sensor3Statu.Text = status ? "触发中" : "未触发";
                    label_Sensor3Statu.PrefixColor = status ? Color.Orange : Color.Green;
                    _isSensor3On = status;
                    break;
            }
        }
        #endregion

        #region 初始化各设备的信息
        private void InitDeviceStatus()
        {
            label_CameraStatu.Text = "未启动";
            label_CameraStatu.PrefixColor = Color.Gray;
            avatar_Camera.ForeColor = Color.Gray;

            label_ScannerStatu.Text = "未启动";
            label_ScannerStatu.PrefixColor = Color.Gray;
            avatar_Scanner.ForeColor = Color.Gray;

            label_PLCStatu.Text = "未启动";
            label_PLCStatu.PrefixColor = Color.Gray;
            avatar_PLC.ForeColor = Color.Gray;

            // 初始化时禁用所有Conv按钮和滑块（PLC未启动）
            btn_Conv1.Enabled = false;
            slider_Conv1.Enabled = false;
            btn_Conv2.Enabled = false;
            slider_Conv2.Enabled = false;
            btn_Conv3.Enabled = false;
            slider_Conv3.Enabled = false;
            btn_Conv4.Enabled = false;
            slider_Conv4.Enabled = false;
            btn_Conv5.Enabled = false;
            slider_Conv5.Enabled = false;
        }
        #endregion

        #region Table右键菜单功能(未写)
        AntdUI.IContextMenuStripItem[] menulist = { };
        private void table_Logrecording_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                AntdUI.ContextMenuStrip.open(this, it =>
                {
                    switch (it.Text)
                    {
                        case "刷新":
                            break;
                        case "清空":
                            break;
                        case "导出":
                           
                            break;
                    }
                }, menulist);
            }
        }
        #endregion

        #region Console窗体加载事件(未写,可删除)
        private void Console_Load(object sender, EventArgs e)
        {
            //if (_collectionControl != null)
            //{
            //    _collectionControl.DetectionCompleted += CollectionControl_DetectionCompleted;
            //    _collectionControl.DefectStatisticsUpdated += CollectionControl_DefectStatisticsUpdated;
            //    _collectionControl.WorkOrderDetectionUpdated += CollectionControl_WorkOrderDetectionUpdated;
            //}
        }
        #endregion

        #region 接收Scanner传递过来的工单信息
        public void OnWorkOrderReceived(object sender, WorkOrderReceivedEventArgs e)
        {
            if (e == null || e.WorkOrder == null)
            {
                this.BeginInvoke(new Action(() =>
                {
                    AntdUI.Notification.warn(this.FindForm(), "工单更新失败", "收到空的工单数据");
                    ResetWorkOrderCoreInfo();
                    //input_WorkOrder.Text = "无有效工单";
                    //select_PCBType.Items.Clear();
                    //label_PlannedQuantity.Text = "0/0";
                    //progress_WorkOrder.Value = 0;
                    //progress_WorkOrder.Text = "0.00%";
                    //_defectBindingList.Clear();
                }));
                return;
            }
            _currentWorkOrder = e.WorkOrder;
            _currentPCB = e.WorkOrder.WorkOrderPCBs ?? new List<MesWorkOrderPCB>();
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<WorkOrderResponse>(UpdateWorkOrderControls), e.WorkOrder);
            }
            else
            {
                UpdateWorkOrderControls(e.WorkOrder);
            }
        }
        #endregion

        #region 重置工单信息
        private void ResetWorkOrderCoreInfo()
        {
            // 检测数重置
            label_PlannedQuantity.Text = "0/0";
            progress_WorkOrder.Value = 0;
            progress_WorkOrder.Text = "0.00%";

            // 工单状态重置为待处理
            if (_currentWorkOrder != null)
            {
                _currentWorkOrder.OrderStatus = OrderStatu.Pending;
                if (_currentWorkOrder != null)
                {
                    switch (_currentWorkOrder.OrderStatus)
                    {
                        case OrderStatu.Pending:
                            label_PlannedQuantity.Badge = "待处理";
                            label_PlannedQuantity.BadgeBack = Color.Orange;
                            break;
                        case OrderStatu.Processing:
                            label_PlannedQuantity.Badge = "处理中";
                            label_PlannedQuantity.BadgeBack = Color.Blue;
                            break;
                        case OrderStatu.Paused:
                            label_PlannedQuantity.Badge = "已暂停";
                            label_PlannedQuantity.BadgeBack = Color.Gray;
                            break;
                        case OrderStatu.Completed:
                            label_PlannedQuantity.Badge = "已完成";
                            label_PlannedQuantity.BadgeBack = Color.Green;
                            break;
                        case OrderStatu.Canceled:
                            label_PlannedQuantity.Badge = "已取消";
                            label_PlannedQuantity.BadgeBack = Color.Red;
                            break;
                        default:
                            label_PlannedQuantity.Badge = "未知状态";
                            label_PlannedQuantity.BadgeBack = Color.LightGray;
                            break;
                    }
                }
            }

            // 重置PCB的已完成数量（扫码切换工单时）
            if (_currentPCB != null)
            {
                foreach (var pcb in _currentPCB)
                {
                    pcb.CompleteQuantity = 0;
                }
            }
        }
        #endregion

        #region 更新存储工单信息的控件
        private void UpdateWorkOrderControls(WorkOrderResponse workOrder)
        {
            if (workOrder == null) return;

            input_WorkOrder.Text = workOrder.WorkOrderCode;
            _currentPCB = workOrder.WorkOrderPCBs ?? new List<MesWorkOrderPCB>();
            var pcbNames = _currentPCB
                .Select(wop => wop.mesPCBs.PCBName)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .ToArray() ?? new string[0];

            select_PCBType.Items.Clear();
            select_PCBType.Items.AddRange(pcbNames);

            // 如果有选中的PCB，尝试保持选中状态
            if (_currentSelectedPCB != null && pcbNames.Length > 0)
            {
                var currentPCBName = _currentSelectedPCB.mesPCBs?.PCBName;
                if (!string.IsNullOrEmpty(currentPCBName) && pcbNames.Contains(currentPCBName))
                {
                    for (int i = 0; i < pcbNames.Length; i++)
                    {
                        if (pcbNames[i] == currentPCBName)
                        {
                            select_PCBType.SelectedIndex = i;
                            break;
                        }
                    }
                }
                else if (pcbNames.Length > 0)
                {
                    // 如果没有匹配的PCB，选择第一个
                    select_PCBType.SelectedIndex = 0;
                    _currentSelectedPCB = _currentPCB.FirstOrDefault(wop => wop.mesPCBs.PCBName == pcbNames[0]);
                }
            }
            else if (pcbNames.Length > 0)
            {
                select_PCBType.SelectedIndex = 0;
                _currentSelectedPCB = _currentPCB.FirstOrDefault(wop => wop.mesPCBs.PCBName == pcbNames[0]);
            }

            // 更新工单状态显示
            if (workOrder != null)
            {
                switch (workOrder.OrderStatus)
                {
                    case OrderStatu.Pending:
                        label_PlannedQuantity.Badge = "待处理";
                        label_PlannedQuantity.BadgeBack = Color.Orange;
                        break;
                    case OrderStatu.Processing:
                        label_PlannedQuantity.Badge = "处理中";
                        label_PlannedQuantity.BadgeBack = Color.Blue;
                        break;
                    case OrderStatu.Paused:
                        label_PlannedQuantity.Badge = "已暂停";
                        label_PlannedQuantity.BadgeBack = Color.Gray;
                        break;
                    case OrderStatu.Completed:
                        label_PlannedQuantity.Badge = "已完成";
                        label_PlannedQuantity.BadgeBack = Color.Green;
                        break;
                    case OrderStatu.Canceled:
                        label_PlannedQuantity.Badge = "已取消";
                        label_PlannedQuantity.BadgeBack = Color.Red;
                        break;
                    default:
                        label_PlannedQuantity.Badge = "未知状态";
                        label_PlannedQuantity.BadgeBack = Color.LightGray;
                        break;
                }
            }
        }
        #endregion

        #region  展示并更新缺陷表的信息
        private void UpdateDefectStatisticsTable()
        {
            _defectBindingList.Clear();
            if (_currentPCBDefect == null || !_currentPCBDefect.Any())
            {
                // 如果没有缺陷数据，可以添加一个默认行显示无数据
                _defectBindingList.Add(new PCBDefectDetail
                {
                    DefectTypeName = "暂无工单缺陷类型",
                    DefectCount = 0
                });
            }
            else
            {
                _defectBindingList.Clear();
                foreach (var defect in _currentPCBDefect)
                {
                    _defectBindingList.Add(new PCBDefectDetail
                    {
                        DefectTypeName = defect.DefectName?.Trim() ?? "未知缺陷", // 保留工单缺陷名称
                        DefectCount = 0 // 初始数量为0
                    });
                }
            }

          
        }
        #endregion

        #region 扫码枪开关事件
        private void switch_Scanner_CheckedChanged(object sender, BoolEventArgs e)
        {
           
            SwitchStatusChanged?.Invoke(this, new DeviceSwitchStatusChangedEventArgs(DeviceSwitchType.Scanner_Switch, e.Value));
        }
        #endregion

        #region 相机开关事件
        private void switch_Camera_CheckedChanged(object sender, BoolEventArgs e)
        {
            
            SwitchStatusChanged?.Invoke(this, new DeviceSwitchStatusChangedEventArgs(DeviceSwitchType.Camera_Switch, e.Value));
        }
        #endregion

        #region PLC开关事件(屎山)
        private async void switch_PLC_CheckedChanged(object sender, BoolEventArgs e)
        {
           
            SwitchStatusChanged?.Invoke(this, new DeviceSwitchStatusChangedEventArgs(DeviceSwitchType.PLC_Switch, e.Value));
            if (e.Value)
            {

               
                //var conv1Result = await _plcbll.UpdateCurrentValueByVariableNameAsync(new PLCValueUpdateDTO { VariableName = "Conv1_Run", Value = 1 });
                //var conv2Result = await _plcbll.UpdateCurrentValueByVariableNameAsync(new PLCValueUpdateDTO { VariableName = "Conv2_Run", Value = 1 });
                //var conv3Result = await _plcbll.UpdateCurrentValueByVariableNameAsync(new PLCValueUpdateDTO { VariableName = "Conv3_Run", Value = 1 });
                //var conv4Result = await _plcbll.UpdateCurrentValueByVariableNameAsync(new PLCValueUpdateDTO { VariableName = "Conv4_Run", Value = 1 });
                //var conv5Result = await _plcbll.UpdateCurrentValueByVariableNameAsync(new PLCValueUpdateDTO { VariableName = "Conv5_Run", Value = 1 });
                var startTasks = new List<Task<bool>>
                {
                    _plcbll.UpdateCurrentValueByVariableNameAsync(new PLCValueUpdateDTO { VariableName = "Conv1_Run", Value = 1 }),
                    _plcbll.UpdateCurrentValueByVariableNameAsync(new PLCValueUpdateDTO { VariableName = "Conv2_Run", Value = 1 }),
                    _plcbll.UpdateCurrentValueByVariableNameAsync(new PLCValueUpdateDTO { VariableName = "Conv3_Run", Value = 1 }),
                   // _plcbll.UpdateCurrentValueByVariableNameAsync(new PLCValueUpdateDTO { VariableName = "Conv4_Run", Value = 1 }),
                    _plcbll.UpdateCurrentValueByVariableNameAsync(new PLCValueUpdateDTO { VariableName = "Conv5_Run", Value = 1 })
                };
                var results = await Task.WhenAll(startTasks).ConfigureAwait(false);
                _isConv1On = results[0];
                _isConv2On = results[1];
                _isConv3On = results[2];
               // _isConv4On = results[3];
                _isConv5On = results[3];
                //if (conv1Result) _isConv1On = true;
                //if (conv2Result) _isConv2On = true;
                //if (conv3Result) _isConv3On = true;
                //if (conv4Result) _isConv4On = true;
                //if (conv5Result) _isConv5On = true;

                btn_Conv1.IconSvg = _isConv1On ? IconSvg.switch_IconSvg_Success : IconSvg.switch_IconSvg_Default;
                btn_Conv2.IconSvg = _isConv2On ? IconSvg.switch_IconSvg_Success : IconSvg.switch_IconSvg_Default;
                btn_Conv3.IconSvg = _isConv3On ? IconSvg.switch_IconSvg_Success : IconSvg.switch_IconSvg_Default;
               // btn_Conv4.IconSvg = _isConv4On ? IconSvg.switch_IconSvg_Success : IconSvg.switch_IconSvg_Default;
                btn_Conv5.IconSvg = _isConv5On ? IconSvg.switch_IconSvg_Success : IconSvg.switch_IconSvg_Default;
                label_Sensor1Statu.Text = "未触发";
                label_Sensor1Statu.PrefixColor = Color.Green;
                label_Sensor2Statu.Text = "未触发";
                label_Sensor2Statu.PrefixColor = Color.Green;
                label_Sensor3Statu.Text = "未触发";
                label_Sensor3Statu.PrefixColor = Color.Green;

                //if (conv1Result && conv2Result && conv3Result && conv4Result && conv4Result)
                //{
                //    AntdUI.Message.success(this.FindForm(), "PLC关联变量已全部启用");
                //}
                //else
                //{
                //    AntdUI.Message.warn(this.FindForm(), "部分变量更新失败，请检查日志");
                //}
                if (results.All(r => r))
                {
                   AntdUI.Message.success(this.FindForm(), "PLC关联变量已全部启用");
                  
                }
                else
                {
                    AntdUI.Message.warn(this.FindForm(), "部分变量更新失败，请检查日志");
                }
                _isTimerRunning = true;
                _statusReadTimer.Enabled = true;
               // AntdUI.Notification.success(this.FindForm(), "PLC", "定时器已启动（自动同步Slave状态）");


            }
            else
            {
               
                try
                {
                    //bool conv1Result = await _plcbll.UpdateCurrentValueByVariableNameAsync(new PLCValueUpdateDTO { VariableName = "Conv1_Run", Value = 0 });
                    //bool conv2Result = await _plcbll.UpdateCurrentValueByVariableNameAsync(new PLCValueUpdateDTO { VariableName = "Conv2_Run", Value = 0 });
                    //bool conv3Result = await _plcbll.UpdateCurrentValueByVariableNameAsync(new PLCValueUpdateDTO { VariableName = "Conv3_Run", Value = 0 });
                    //bool conv4Result = await _plcbll.UpdateCurrentValueByVariableNameAsync(new PLCValueUpdateDTO { VariableName = "Conv4_Run", Value = 0 });
                    //bool conv5Result = await _plcbll.UpdateCurrentValueByVariableNameAsync(new PLCValueUpdateDTO { VariableName = "Conv5_Run", Value = 0 });
                    var stopTasks = new List<Task<bool>>
                    {
                        _plcbll.UpdateCurrentValueByVariableNameAsync(new PLCValueUpdateDTO { VariableName = "Conv1_Run", Value = 0 }),
                        _plcbll.UpdateCurrentValueByVariableNameAsync(new PLCValueUpdateDTO { VariableName = "Conv2_Run", Value = 0 }),
                        _plcbll.UpdateCurrentValueByVariableNameAsync(new PLCValueUpdateDTO { VariableName = "Conv3_Run", Value = 0 }),
                        _plcbll.UpdateCurrentValueByVariableNameAsync(new PLCValueUpdateDTO { VariableName = "Conv4_Run", Value = 0 }),
                        _plcbll.UpdateCurrentValueByVariableNameAsync(new PLCValueUpdateDTO { VariableName = "Conv5_Run", Value = 0 })
                    };
                    var results = await Task.WhenAll(stopTasks).ConfigureAwait(false);

                    //if (conv1Result) _isConv1On = false;
                    //if (conv2Result) _isConv2On = false;
                    //if (conv3Result) _isConv3On = false;
                    //if (conv4Result) _isConv4On = false;
                    //if (conv5Result) _isConv5On = false;
                    if (results[0]) _isConv1On = false;
                    if (results[1]) _isConv2On = false;
                    if (results[2]) _isConv3On = false;
                    if (results[3]) _isConv4On = false;
                    if (results[4]) _isConv5On = false;
                }
                catch (Exception ex)
                {

                    AntdUI.Notification.warn(this.FindForm(), "PLC", $"关闭时同步Conv状态失败：{ex.Message}");
                }





                btn_Conv1.IconSvg = _isConv1On ? IconSvg.switch_IconSvg_Success : IconSvg.switch_IconSvg_Default;
                btn_Conv2.IconSvg = _isConv2On ? IconSvg.switch_IconSvg_Success : IconSvg.switch_IconSvg_Default;
                btn_Conv3.IconSvg = _isConv3On ? IconSvg.switch_IconSvg_Success : IconSvg.switch_IconSvg_Default;
              //  btn_Conv4.IconSvg = _isConv4On ? IconSvg.switch_IconSvg_Success : IconSvg.switch_IconSvg_Default;
                btn_Conv5.IconSvg = _isConv5On ? IconSvg.switch_IconSvg_Success : IconSvg.switch_IconSvg_Default;
                label_Sensor1Statu.Text = "未启动";
                label_Sensor1Statu.PrefixColor = Color.Gray;
                label_Sensor2Statu.Text = "未启动";
                label_Sensor2Statu.PrefixColor = Color.Gray;
                label_Sensor3Statu.Text = "未启动";
                label_Sensor3Statu.PrefixColor = Color.Gray;

                _statusReadTimer.Stop();
                _isTimerRunning = false;
                _statusReadTimer.Enabled = false;
                if (_modbusdriver != null)
                {
                    try
                    {
                        _modbusdriver.DisConnectTCP();
                    }
                    catch (Exception ex)
                    {
                        AntdUI.Notification.warn(this.FindForm(), "PLC", $"关闭TCP连接失败：{ex.Message}");
                    }
                }
            }



        }
        #endregion

        #region 同步开关控件
        private void SyncSwitchControl(DeviceSwitchType switchType, bool isOn)
        {
                switch (switchType)
                {
                    case DeviceSwitchType.Camera_Switch:
                        switch_Camera.Checked = isOn;
                        break;
                    case DeviceSwitchType.Scanner_Switch:
                        switch_Scanner.Checked = isOn;
                        break;
                    case DeviceSwitchType.PLC_Switch:
                        switch_PLC.Checked = isOn;
                        break;
                }
        }
        #endregion

        #region 同步状态标签
        private void SyncStatusLabel(DeviceSwitchType switchType, bool isOn)
        {
            string statusText = isOn ? "运行中" : "未运行";
            Color statusColor = isOn ? Color.Green : Color.Gray;

            switch (switchType)
            {
                case DeviceSwitchType.Camera_Switch:
                    label_CameraStatu.Text = statusText;
                    label_CameraStatu.PrefixColor = statusColor;
                    avatar_Camera.ForeColor = statusColor;
                    break;
                case DeviceSwitchType.Scanner_Switch:
                    label_ScannerStatu.Text = statusText;
                    label_ScannerStatu.PrefixColor = statusColor;
                    avatar_Scanner.ForeColor = statusColor;
                    break;
                case DeviceSwitchType.PLC_Switch:
                    label_PLCStatu.Text = statusText;
                    label_PLCStatu.PrefixColor = statusColor;
                    avatar_PLC.ForeColor = statusColor;
                    break;
            }
        }
        #endregion

        #region 更新开关状态
        public void UpdateSwitchStatus(DeviceSwitchType switchType, bool isOn)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<DeviceSwitchType, bool>(UpdateSwitchStatus), switchType, isOn);
                return;
            }
            SyncSwitchControl(switchType, isOn);
            SyncStatusLabel(switchType, isOn);
        }
        #endregion

        #region 1号传送带的开关
        private async void btn_Conv1_Click(object sender, EventArgs e)
        {
           
            bool originalStatus = _isConv1On;
            bool targetStatus = !originalStatus;
            btn_Conv1.Enabled = false;

            try
            {
  
                var (ok, current) = await Task.Run(async () =>
                {
                    bool writeOk = await _plcbll
                        .UpdateCurrentValueByVariableNameAsync(
                            new PLCValueUpdateDTO { VariableName = "Conv1_Run", Value = targetStatus ? 1 : 0 })
                        .ConfigureAwait(false);

                    int cur = await _plcbll
                        .GetCurrentValueByVariableNameAsync(
                            new PLCValueUpdateDTO { VariableName = "Conv1_Run" })
                        .ConfigureAwait(false);

                    return (writeOk, cur);
                }).ConfigureAwait(true);   

                bool currentStatus = current == 1;
                _isConv1On = currentStatus;
                btn_Conv1.IconSvg = currentStatus ? IconSvg.switch_IconSvg_Success
                                                  : IconSvg.switch_IconSvg_Default;

                if (ok && currentStatus == targetStatus)
                {

                }
                   // AntdUI.Message.success(this.FindForm(), $"Conv1已{(targetStatus ? "启用" : "禁用")}");
                else
                {

                }
                   // AntdUI.Message.warn(this.FindForm(), "写入成功，状态未同步");
            }
            catch (Exception ex)
            {
              //  AntdUI.Message.error(this.FindForm(), $"Conv1操作失败：{ex.Message}");
                _isConv1On = originalStatus;
                btn_Conv1.IconSvg = originalStatus ? IconSvg.switch_IconSvg_Success
                                                   : IconSvg.switch_IconSvg_Default;
            }
            finally
            {
                btn_Conv1.Enabled = true;
            }
        }
        #endregion

        #region 2号传送带的开关
        private async void btn_Conv2_Click(object sender, EventArgs e)
        {
            bool originalStatus = _isConv2On;
            bool targetStatus = !originalStatus;
            btn_Conv2.Enabled = false;

            try
            {

                var (ok, current) = await Task.Run(async () =>
                {
                    bool writeOk = await _plcbll
                        .UpdateCurrentValueByVariableNameAsync(
                            new PLCValueUpdateDTO { VariableName = "Conv2_Run", Value = targetStatus ? 1 : 0 })
                        .ConfigureAwait(false);

                    int cur = await _plcbll
                        .GetCurrentValueByVariableNameAsync(
                            new PLCValueUpdateDTO { VariableName = "Conv2_Run" })
                        .ConfigureAwait(false);

                    return (writeOk, cur);
                }).ConfigureAwait(true);

                bool currentStatus = current == 1;
                _isConv2On = currentStatus;
                btn_Conv2.IconSvg = currentStatus ? IconSvg.switch_IconSvg_Success
                                                  : IconSvg.switch_IconSvg_Default;

                if (ok && currentStatus == targetStatus)
                {

                }
                // AntdUI.Message.success(this.FindForm(), $"Conv2已{(targetStatus ? "启用" : "禁用")}");
                else
                {

                }
                // AntdUI.Message.warn(this.FindForm(), "写入成功，状态未同步");
            }
            catch (Exception ex)
            {
                // AntdUI.Message.error(this.FindForm(), $"Conv2操作失败：{ex.Message}");
                _isConv2On = originalStatus;
                btn_Conv2.IconSvg = originalStatus ? IconSvg.switch_IconSvg_Success
                                                   : IconSvg.switch_IconSvg_Default;
            }
            finally
            {
                btn_Conv2.Enabled = true;
            }
        }
        #endregion

        #region 3号传送带的开关
        private async void btn_Conv3_Click(object sender, EventArgs e)
        {
            bool originalStatus = _isConv3On;
            bool targetStatus = !originalStatus;
            btn_Conv3.Enabled = false;

            try
            {

                var (ok, current) = await Task.Run(async () =>
                {
                    bool writeOk = await _plcbll
                        .UpdateCurrentValueByVariableNameAsync(
                            new PLCValueUpdateDTO { VariableName = "Conv3_Run", Value = targetStatus ? 1 : 0 })
                        .ConfigureAwait(false);

                    int cur = await _plcbll
                        .GetCurrentValueByVariableNameAsync(
                            new PLCValueUpdateDTO { VariableName = "Conv3_Run" })
                        .ConfigureAwait(false);

                    return (writeOk, cur);
                }).ConfigureAwait(true);

                bool currentStatus = current == 1;
                _isConv3On = currentStatus;
                btn_Conv3.IconSvg = currentStatus ? IconSvg.switch_IconSvg_Success
                                                  : IconSvg.switch_IconSvg_Default;

                if (ok && currentStatus == targetStatus)
                {

                }
                   // AntdUI.Message.success(this.FindForm(), $"Conv3已{(targetStatus ? "启用" : "禁用")}");
                else
                {

                }
                   // AntdUI.Message.warn(this.FindForm(), "写入成功，状态未同步");
            }
            catch (Exception ex)
            {
               // AntdUI.Message.error(this.FindForm(), $"Conv3操作失败：{ex.Message}");
                _isConv3On = originalStatus;
                btn_Conv3.IconSvg = originalStatus ? IconSvg.switch_IconSvg_Success
                                                   : IconSvg.switch_IconSvg_Default;
            }
            finally
            {
                btn_Conv3.Enabled = true;
            }
        }
        #endregion

        #region 4号传送带的开关
        private async void btn_Conv4_Click(object sender, EventArgs e)
        {
            if (_isAutoControllingConv4ByDetection)
            {
                AntdUI.Notification.warn(this.FindForm(), "自动控制中",
                    "Conveyor 4当前正根据检测结果自动控制，请等待自动控制结束");
                return;
            }

            bool originalStatus = _isConv4On;
            bool targetStatus = !originalStatus;
            btn_Conv4.Enabled = false;

            try
            {
                var (ok, current) = await Task.Run(async () =>
                {
                    bool writeOk = await _plcbll
                        .UpdateCurrentValueByVariableNameAsync(
                            new PLCValueUpdateDTO { VariableName = "Conv4_Run", Value = targetStatus ? 1 : 0 })
                        .ConfigureAwait(false);

                    int cur = await _plcbll
                        .GetCurrentValueByVariableNameAsync(
                            new PLCValueUpdateDTO { VariableName = "Conv4_Run" })
                        .ConfigureAwait(false);

                    return (writeOk, cur);
                }).ConfigureAwait(true);

                bool currentStatus = current == 1;
                _isConv4On = currentStatus;
                btn_Conv4.IconSvg = currentStatus ? IconSvg.switch_IconSvg_Success
                                                  : IconSvg.switch_IconSvg_Default;

                // 如果是手动打开，同时设置速度为固定值
                if (currentStatus)
                {
                    await _plcbll.UpdateCurrentValueByVariableNameAsync(new PLCValueUpdateDTO
                    {
                        VariableName = "Conv4_SpeedSet",
                        Value = _fixedConv4Speed
                    });
                    _conv4Speed = _fixedConv4Speed;
                    slider_Conv4.Value = _fixedConv4Speed;

                    await AddLog(LogType.PLCCommunication,
                        $"手动开启Conveyor 4，速度设置为{_fixedConv4Speed}",
                        LogStatu.Success);
                }
                else
                {
                    // 如果是手动关闭，同时设置速度为0
                    await _plcbll.UpdateCurrentValueByVariableNameAsync(new PLCValueUpdateDTO
                    {
                        VariableName = "Conv4_SpeedSet",
                        Value = 0
                    });
                    _conv4Speed = 0;
                    slider_Conv4.Value = 0;

                    await AddLog(LogType.PLCCommunication,
                        "手动关闭Conveyor 4",
                        LogStatu.Success);
                }
            }
            catch (Exception ex)
            {
                AntdUI.Message.error(this.FindForm(), $"Conv4操作失败：{ex.Message}");
                _isConv4On = originalStatus;
                btn_Conv4.IconSvg = originalStatus ? IconSvg.switch_IconSvg_Success
                                                   : IconSvg.switch_IconSvg_Default;
            }
            finally
            {
                btn_Conv4.Enabled = true;
            }
        }
        #endregion

        #region 5号传送带的开关
        private async void btn_Conv5_Click(object sender, EventArgs e)
        {
            bool originalStatus = _isConv5On;
            bool targetStatus = !originalStatus;
            btn_Conv5.Enabled = false;

            try
            {

                var (ok, current) = await Task.Run(async () =>
                {
                    bool writeOk = await _plcbll
                        .UpdateCurrentValueByVariableNameAsync(
                            new PLCValueUpdateDTO { VariableName = "Conv5_Run", Value = targetStatus ? 1 : 0 })
                        .ConfigureAwait(false);

                    int cur = await _plcbll
                        .GetCurrentValueByVariableNameAsync(
                            new PLCValueUpdateDTO { VariableName = "Conv5_Run" })
                        .ConfigureAwait(false);

                    return (writeOk, cur);
                }).ConfigureAwait(true);

                bool currentStatus = current == 1;
                _isConv5On = currentStatus;
                btn_Conv5.IconSvg = currentStatus ? IconSvg.switch_IconSvg_Success
                                                  : IconSvg.switch_IconSvg_Default;

                if (ok && currentStatus == targetStatus)
                {

                }
                   // AntdUI.Message.success(this.FindForm(), $"Conv5已{(targetStatus ? "启用" : "禁用")}");
                else
                {

                }
                   // AntdUI.Message.warn(this.FindForm(), "写入成功，状态未同步");
            }
            catch (Exception ex)
            {
                //AntdUI.Message.error(this.FindForm(), $"Conv5操作失败：{ex.Message}");
                _isConv5On = originalStatus;
                btn_Conv5.IconSvg = originalStatus ? IconSvg.switch_IconSvg_Success
                                                   : IconSvg.switch_IconSvg_Default;
            }
            finally
            {
                btn_Conv5.Enabled = true;
            }
        }
        #endregion

        #region 更新传送带速度
        private async Task ExecuteSpeedUpdateAsync(bool isEnabled, string variableName, int targetSpeed, string convName)
        {
            if (!isEnabled)
            {
              await  AddLog(LogType.PLCCommunication, $"{convName} 未运行或 PLC 未开启，无法调整速度", LogStatu.Warning);
                return;
            }

            try
            {
                await _plcbll.UpdateCurrentValueByVariableNameAsync(new PLCValueUpdateDTO
                {
                    VariableName = variableName,
                    Value = targetSpeed
                });
              await  AddLog(LogType.PLCCommunication, $"{convName} 速度已调整为：{targetSpeed}", LogStatu.Success);
            }
            catch (Exception ex)
            {
              await  AddLog(LogType.PLCCommunication, $"{convName} 速度调整失败：{ex.Message}", LogStatu.Error);
                AntdUI.Notification.error(this.FindForm(), "PLC 通信", $"{convName} 速度同步失败：{ex.Message}");
            }
        }
        #endregion

        #region 1号传送带数据更新事件
        private void slider_Conv1_ValueChanged(object sender, IntEventArgs e)
        {
            _conv1Speed = e.Value;
        }
        #endregion

        #region 2号传送带数据更新事件
        private void slider_Conv2_ValueChanged(object sender, IntEventArgs e)
        {
           

            _conv2Speed = e.Value;
        }
        #endregion

        #region 3号传送带数据更新事件
        private void slider_Conv3_ValueChanged(object sender, IntEventArgs e)
        {
            _conv3Speed = e.Value;
        }
        #endregion

        #region 4号传送带数据更新事件
        private void slider_Conv4_ValueChanged(object sender, IntEventArgs e)
        {
            if (_isAutoControllingConv4ByDetection)
            {
                return;
            }
            _conv4Speed = e.Value;
        }
        #endregion

        #region 5号传送带数据更新事件
        private void slider_Conv5_ValueChanged(object sender, IntEventArgs e)
        {
            _conv5Speed = e.Value;
        }
        #endregion

        #region 1号传送带滑块松开更新数值事件
        private async void slider_Conv1_MouseUp(object sender, MouseEventArgs e)
        {
            await ExecuteSpeedUpdateAsync(
               isEnabled: _isConv1On && switch_PLC.Checked,
               variableName: "Conv1_SpeedSet",
               targetSpeed: _conv1Speed,
               convName: "Conv1"
             ).ConfigureAwait(false);
        }
        #endregion

        #region 2号传送带滑块松开更新数值事件
        private async void slider_Conv2_MouseUp(object sender, MouseEventArgs e)
        {
          
            await ExecuteSpeedUpdateAsync(
             isEnabled: _isConv2On && switch_PLC.Checked,
             variableName: "Conv2_SpeedSet",
             targetSpeed: _conv2Speed,
             convName: "Conv2"
           ).ConfigureAwait(false);
        }
        #endregion

        #region 3号传送带滑块松开更新数值事件
        private async void slider_Conv3_MouseUp(object sender, MouseEventArgs e)
        {
            await ExecuteSpeedUpdateAsync(
             isEnabled: _isConv3On && switch_PLC.Checked,
             variableName: "Conv3_SpeedSet",
             targetSpeed: _conv3Speed,
             convName: "Conv3"
           ).ConfigureAwait(false);
        }
        #endregion

        #region 4号传送带滑块松开更新数值事件
        private async void slider_Conv4_MouseUp(object sender, MouseEventArgs e)
        {
            if (_isAutoControllingConv4ByDetection)
            {
                AntdUI.Notification.info(this.FindForm(), "自动控制中",
                    "速度当前正根据检测结果自动控制，手动调整无效");
                return;
            }
            await ExecuteSpeedUpdateAsync(
             isEnabled: _isConv4On && switch_PLC.Checked,
             variableName: "Conv4_SpeedSet",
             targetSpeed: _conv4Speed,
             convName: "Conv4"
           ).ConfigureAwait(false);
        }
        #endregion

        #region 5号传送带滑块松开更新数值事件
        private async void slider_Conv5_MouseUp(object sender, MouseEventArgs e)
        {
            await ExecuteSpeedUpdateAsync(
             isEnabled: _isConv5On && switch_PLC.Checked,
             variableName: "Conv5_SpeedSet",
             targetSpeed: _conv5Speed,
             convName: "Conv5"
           ).ConfigureAwait(false);
        }
        #endregion

        #region 选中PCB类型更新事件(屎山)
        private void select_PCBType_SelectedIndexChanged(object sender, IntEventArgs e)
        {
            var selectedPCBName = select_PCBType.Text;
            if (string.IsNullOrWhiteSpace(selectedPCBName) || _currentPCB == null || !_currentPCB.Any())
            {
                label_PlannedQuantity.Text = "0/0";
                progress_WorkOrder.Steps = 0;
                progress_WorkOrder.Text = "0.00%";
                progress_WorkOrder.Value = 0;
                //  _currentPCBDefect = new List<MesPCBDefect>();
                // UpdateDefectStatisticsTable();
                _defectBindingList.Clear();
                table_DefectStatistics.Refresh();
                return;
            }

            // 更新当前选中的PCB
            var selectedPCB = _currentPCB
                .FirstOrDefault(wop => string.Equals(wop.mesPCBs.PCBName, selectedPCBName, StringComparison.OrdinalIgnoreCase));

            _currentSelectedPCB = selectedPCB;
            _defectBindingList.Clear();
            table_DefectStatistics.Refresh();

            if (selectedPCB != null)
            {
                int planQuantity = selectedPCB.PlanQuantity;
                int completeQuantity = (int)selectedPCB.CompleteQuantity;

                label_PlannedQuantity.Text = $"{completeQuantity}/{planQuantity}";
                progress_WorkOrder.Steps = planQuantity;

                if (planQuantity > 0)
                {
                    double progressValue = (double)completeQuantity / planQuantity * 100;
                    progress_WorkOrder.Text = $"{progressValue:F2}%";
                    progress_WorkOrder.Value = (float)((double)completeQuantity / planQuantity);
                }
                else
                {
                    progress_WorkOrder.Text = "0.00%";
                    progress_WorkOrder.Value = 0;
                }
            }
            else
            {
                label_PlannedQuantity.Text = "0/0";
                progress_WorkOrder.Text = "0.00%";
                progress_WorkOrder.Value = 0;
            }
            PCBSyncRequested?.Invoke(this, _currentSelectedPCB);
        }

        #endregion

        #region 工单检测更新事件（核心逻辑）
        private void CollectionControl_WorkOrderDetectionUpdated(object sender, WorkOrderUpdateDTO e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<WorkOrderUpdateDTO>(UpdateWorkOrderCoreInfo), e);
            }
            else
            {
                UpdateWorkOrderCoreInfo(e);
            }
        }
        #endregion

        #region 工单核心信息更新方法的核心逻辑
        private void UpdateWorkOrderCoreInfo(WorkOrderUpdateDTO dto)
        {
            // 校验：必须有当前工单和选中的PCB类型，且检测的PCB类型与当前选中一致
            if (dto == null  || !_currentSelectedPCB.PCBCode.Equals(dto.TargetPCBType))
            {
                AntdUI.Notification.warn(this.FindForm(), "更新失败", "无有效工单或PCB类型不匹配");
                return;
            }

            // 1. 找到当前选中的PCB，累加检测数（合格才累加完成数，可按需修改）
            var selectedPCB = _currentPCB.FirstOrDefault(wop =>
                wop.mesPCBs.PCBName.Equals(select_PCBType.Text));
            if (selectedPCB == null)
            {
                AntdUI.Notification.warn(this.FindForm(), "更新失败", "未找到当前PCB类型");
                return;
            }

            // 累加已完成数量（仅合格产品计入，如需所有检测都计入，去掉IsQualified判断）
            if (dto.IsQualified)
            {
                selectedPCB.CompleteQuantity++;
            }

            // 2. 更新「检测数UI」（已完成数量/计划数量）
            int complete = (int)selectedPCB.CompleteQuantity;
            int plan = selectedPCB.PlanQuantity;
            label_PlannedQuantity.Text = $"{complete}/{plan}";
            progress_WorkOrder.Value = plan > 0 ? (float)complete / plan : 0;
            progress_WorkOrder.Text = plan > 0 ? $"{(float)complete / plan:P2}" : "0.00%";

            // 3. 自动更新「工单状态」（基于完成数推导，无需额外传递）
            UpdateWorkOrderStatusByProgress(complete, plan);
            if (_currentWorkOrder != null)
            {
                _currentWorkOrder.OrderStatus = dto.OrderStatus;
                switch (_currentWorkOrder.OrderStatus)
                {
                    case OrderStatu.Pending:
                        label_PlannedQuantity.Badge = "待处理";
                        label_PlannedQuantity.BadgeBack = Color.Orange;
                        break;
                    case OrderStatu.Processing:
                        label_PlannedQuantity.Badge = "处理中";
                        label_PlannedQuantity.BadgeBack = Color.Blue;
                        break;
                    case OrderStatu.Paused:
                        label_PlannedQuantity.Badge = "已暂停";
                        label_PlannedQuantity.BadgeBack = Color.Gray;
                        break;
                    case OrderStatu.Completed:
                        label_PlannedQuantity.Badge = "已完成";
                        label_PlannedQuantity.BadgeBack = Color.Green;
                        break;
                    case OrderStatu.Canceled:
                        label_PlannedQuantity.Badge = "已取消";
                        label_PlannedQuantity.BadgeBack = Color.Red;
                        break;
                    default:
                        label_PlannedQuantity.Badge = "未知状态";
                        label_PlannedQuantity.BadgeBack = Color.LightGray;
                        break;
                }
            }

        }
        #endregion

        #region 添加工单和PCB更新方法
        public void UpdateWorkOrderDisplay(WorkOrderResponse workOrder)
        {
            if (workOrder == null) return;

            // 更新当前工单和PCB列表
            _currentWorkOrder = workOrder;
            _currentPCB = workOrder.WorkOrderPCBs ?? new List<MesWorkOrderPCB>();
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<WorkOrderResponse>(UpdateWorkOrderControls), workOrder);
            }
            else
            {
                UpdateWorkOrderControls(workOrder);
            }
            WorkOrderSyncRequested?.Invoke(this, workOrder);
        }
        #endregion

        #region 更新选择的PCB方法
        public void UpdateSelectedPCB(MesWorkOrderPCB pcb)
        {
            if (pcb == null) return;

            _currentSelectedPCB = pcb;
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<MesWorkOrderPCB>(UpdateSelectedPCBCore), pcb);
            }
            else
            {
                UpdateSelectedPCBCore(pcb);
            }
            PCBSyncRequested?.Invoke(this, pcb);
        }
        #endregion

        #region 更新选择的PCB方法核心逻辑
        private void UpdateSelectedPCBCore(MesWorkOrderPCB pcb)
        {
            _currentSelectedPCB = pcb;

            // 更新PCB选择下拉框
            if (pcb != null && !string.IsNullOrEmpty(pcb.mesPCBs?.PCBName))
            {
                string pcbName = pcb.mesPCBs.PCBName;
                // 查找并选中对应的PCB
                for (int i = 0; i < select_PCBType.Items.Count; i++)
                {
                    if (select_PCBType.Items[i].ToString() == pcb.mesPCBs.PCBName)
                    {
                        select_PCBType.SelectedIndex = i;
                        break;
                    }
                }
            }
            _defectBindingList.Clear();
            table_DefectStatistics.Refresh();
            // _currentPCBDefect = pcb.mesPCBs.mesPCBDefects.ToList();
            //  UpdateDefectStatisticsTable();
            if (pcb != null)
            {
                label_PlannedQuantity.Text = $"{pcb.CompleteQuantity}/{pcb.PlanQuantity}";
                progress_WorkOrder.Steps = pcb.PlanQuantity;

                if (pcb.PlanQuantity > 0)
                {
                    double progressValue = (double)pcb.CompleteQuantity / pcb.PlanQuantity * 100;
                    progress_WorkOrder.Text = $"{progressValue:F2}%";
                    progress_WorkOrder.Value = (float)((double)pcb.CompleteQuantity / pcb.PlanQuantity);
                }
                else
                {
                    progress_WorkOrder.Text = "0.00%";
                    progress_WorkOrder.Value = 0;
                }
            }
        }

        #endregion

        #region 根据完成数和计划数推导工单状态的核心逻辑
        private void UpdateWorkOrderStatusByProgress(int complete, int plan)
        {
            if (plan == 0)
            {
                _currentWorkOrder.OrderStatus = OrderStatu.Pending;
            }
            else if (complete == 0)
            {
                _currentWorkOrder.OrderStatus = OrderStatu.Pending;
            }
            else if (complete < plan)
            {
                _currentWorkOrder.OrderStatus = OrderStatu.Processing;
            }
            else // complete ≥ plan
            {
                _currentWorkOrder.OrderStatus = OrderStatu.Completed;
            }

            if (_currentWorkOrder != null)
            {
                switch (_currentWorkOrder.OrderStatus)
                {
                    case OrderStatu.Pending:
                        label_PlannedQuantity.Badge = "待处理";
                        label_PlannedQuantity.BadgeBack = Color.Orange;
                        break;
                    case OrderStatu.Processing:
                        label_PlannedQuantity.Badge = "处理中";
                        label_PlannedQuantity.BadgeBack = Color.Blue;
                        break;
                    case OrderStatu.Paused:
                        label_PlannedQuantity.Badge = "已暂停";
                        label_PlannedQuantity.BadgeBack = Color.Gray;
                        break;
                    case OrderStatu.Completed:
                        label_PlannedQuantity.Badge = "已完成";
                        label_PlannedQuantity.BadgeBack = Color.Green;
                        break;
                    case OrderStatu.Canceled:
                        label_PlannedQuantity.Badge = "已取消";
                        label_PlannedQuantity.BadgeBack = Color.Red;
                        break;
                    default:
                        label_PlannedQuantity.Badge = "未知状态";
                        label_PlannedQuantity.BadgeBack = Color.LightGray;
                        break;
                }
            }
        }
        #endregion
    }
}
