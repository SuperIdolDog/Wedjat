using AntdUI;
using HalconDotNet;
using HZH_Controls;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using Wedjat.BLL;
using Wedjat.Driver;
using Wedjat.Driver.Model;
using Wedjat.Helper;
using Wedjat.Model.Config;
using Wedjat.Model.DTO;
using Wedjat.Model.Entity;
using static Wedjat.Model.DTO.WorkOrderResponse;

namespace Wedjat.WinForm.CameraMenuUI
{
    public delegate void CollectionControlEventHandler(bool statu);
    public partial class CollectionControl : UserControl
    {
        //调用Webapi的RestClient
        private readonly RestClient _restClient;

        
        private PCBInspectionBLL _pcbInspectionBLL;
        private PCBDefectDetailBLL _defectDetailBLL;
        private WorkOrderBLL _workOrderBLL;


        public event EventHandler<HObject> ImageImported;
        public event EventHandler<DetectionResultEventArgs> DetectionCompleted;
        public event EventHandler<PCBInspectionSavedEventArgs> InspectionSaved;
        public event EventHandler<PCBDefectStatisticsEventArgs> DefectStatisticsUpdated;
        public event EventHandler<WorkOrderUpdateDTO> WorkOrderDetectionUpdated;
        public event CollectionControlEventHandler CollectionControlChanged;
        public event EventHandler<bool> Sensor2TriggerChanged;




        private readonly string _inspectimageSaveDir = Path.Combine(@"C:\Users\Lenovo\Desktop\CSharp项目\就业项目\Wedjat", "PCBInspectionImages");
        private readonly string _imageSaveDir = Path.Combine(@"C:\Users\Lenovo\Desktop\CSharp项目\就业项目\Wedjat", "PCBImages");

       private readonly MVSDriver _cameraDriver;

        private List<string> _imageFormats = new List<string>();

        private HTuple _matchRow = new HTuple();
        private HTuple _matchCol = new HTuple();
        private HTuple _matchAngle = new HTuple();


        private PcbTemplateResource currentTemplate;
        public WorkOrderResponse CurrentWorkOrder { get; set; }
        public MesWorkOrderPCB CurrentSelectedPCB { get; set; }

        public CollectionControl(MVSDriver cameradriver)
        {
            InitializeComponent();
            _cameraDriver = cameradriver;
            _restClient = new RestClient("http://localhost:5059/");
            _pcbInspectionBLL = new PCBInspectionBLL();
            _defectDetailBLL = new PCBDefectDetailBLL();
            _workOrderBLL = new WorkOrderBLL();
            CollectionControlChanged += CollectionControl_CollectionControlChanged;
            Sensor2TriggerChanged += OnSensor2HardTrigger;

        }

        #region 外部调用以触发采集控制事件的方法
        public void TriggerControlEvent(bool isEnable)
        {
            CollectionControlChanged?.Invoke(isEnable);
        }
        #endregion

        #region 外部调用以触发传感器2硬触发事件的方法
        private void CollectionControl_CollectionControlChanged(bool isEnable)
        {
            if (btn_StartGrab.InvokeRequired)
            {
                btn_StartGrab.Invoke(new Action<bool>(CollectionControl_CollectionControlChanged), isEnable);
                return;
            }
            btn_StartGrab.Enabled = isEnable;
            if (isEnable)
            {
                RefreshCameraParameters(true);
            }
        }
        #endregion

        #region 传感器2硬触发事件触发方法
        public void TriggerSensor2Trigger(bool isTriggered)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                    OnSensor2HardTrigger(this, isTriggered)
                ));
            }
            else
            {
                OnSensor2HardTrigger(this, isTriggered);
            }
        }
        #endregion

        #region 更新当前选中的工单的方法
        public void UpdateCurrentWorkOrder(WorkOrderResponse workOrder)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<WorkOrderResponse>(UpdateWorkOrderCore), workOrder);
            }
            else
            {
                UpdateWorkOrderCore(workOrder);
            }
        }
        #endregion

        #region 获取将要更新的工单的方法
        private void UpdateWorkOrderCore(WorkOrderResponse workOrder)
        {
            CurrentWorkOrder = workOrder;
        }
        #endregion

        #region 更新当前选中的PCB的方法
        public void UpdateSelectedPCB(MesWorkOrderPCB pcb)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<MesWorkOrderPCB>(UpdateSelectedPCBCore), pcb);
            }
            else
            {
                UpdateSelectedPCBCore(pcb);
            }
        }
        #endregion

        #region 获取将要更新的PCB的方法
        private void UpdateSelectedPCBCore(MesWorkOrderPCB pcb)
        {
            CurrentSelectedPCB = pcb;
        }
        #endregion

        #region 传感器状态硬触发拍照
        private async void OnSensor2HardTrigger(object sender, bool triggered)
        {
            if (!triggered) return;          // 只处理 0→1 上升沿
            if (IsDisposed) return;          // 控件已释放
            if (!_cameraDriver.isGrabbing)   // 必须已在取流
            {
                this.BeginInvoke((Action)(() =>
                    AntdUI.Notification.warn(this.FindForm(), "硬触发忽略", "相机未处于采集状态")));
                return;
            }

            await Task.Run(() =>       // 后台执行，防止卡定时器线程
            {
                try
                {
                   

                    // 2. 取最新帧
                    Bitmap bmp = _cameraDriver.GetLatestFrame();
                    if (bmp == null) throw new Exception("GetLatestFrame 返回空");

                    // 3. 转 Halcon 格式
                    //HObject hImg = BitmapToHImage(bmp);
                    using (Bitmap bmp24 = ConvertTo24bppRgb(bmp))
                    {
                        HObject hImg = BitmapToHImage(bmp24);
                        if (hImg == null || hImg.IsEmpty())
                            throw new Exception("Bitmap->HObject 失败");

                        // 4. 直接走现有识别流程（含模板匹配、缺陷检测、落库）
                        this.BeginInvoke((Action)(() => DetectAndNotify(hImg)));
                    }
                    
                }
                catch (Exception ex)
                {
                    // 写日志 + 弹窗
                    this.BeginInvoke((Action)(async () =>
                    {
                        await LogEventHelper.PushLog(LogType.PLCCommunication,
                            $"Sensor2硬触发识别异常：{ex.Message}", LogStatu.Failure);
                        AntdUI.Notification.error(this.FindForm(), "硬触发识别失败", ex.Message);
                    }));
                }
            });
        }
        #endregion

        #region 更新相机参数方法
        private void RefreshCameraParameters(bool forceRefresh = false)
        {
            if (_cameraDriver != null && _cameraDriver.IsConnect)
            {
                try
                {
                    float exposure = _cameraDriver.GetExposure();
                    float frameRate = _cameraDriver.GetFrameRate();
                    float gain = _cameraDriver.GetGain();
                    slider_Exposure.Value = (int)exposure;
                    slider_FrameRate.Value = (int)frameRate;
                    slider_Gain.Value = (int)gain;
                    slider_Exposure.ValueChanged += slider_Exposure_ValueChanged;
                    slider_FrameRate.ValueChanged += slider_FrameRate_ValueChanged;
                    slider_Gain.ValueChanged += slider_Gain_ValueChanged;
                    LoadImageFormats(forceRefresh);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"获取相机参数失败: {ex.Message}");
                    AntdUI.Notification.error(this.FindForm(), "获取参数失败", ex.Message);
                }
            }
            else
            {
                Console.WriteLine("请打开相机");
                select_Resolution.Items.Clear();
                select_Resolution.Items.Add("相机未连接");
                select_Resolution.SelectedIndex = 0;
                select_Resolution.Enabled = false;
            }
        }
        #endregion

        #region 开始采集
        private void btn_StartGrab_Click(object sender, EventArgs e)
        {
            if (_cameraDriver.StratGrab())
            {
                Form form = this.FindForm();
                AntdUI.Notification.success(this.FindForm(), "相机", "成功开始采集");
                btn_StartGrab.Enabled = false;
                btn_StopGrab.Enabled = true;
                button_TriggerExec.Enabled = true;
            }
        }
        #endregion

        #region 停止采集
        private async void btn_StopGrab_Click(object sender, EventArgs e)
        {
            btn_StopGrab.Enabled = false;
            btn_StartGrab.Enabled = false;
            button_TriggerExec.Enabled = false;
            try
            {
                // 使用Task.Run将耗时操作放到后台线程
                bool stopSuccess = await Task.Run(() =>
                {
                    // 执行相机停止采集（耗时操作在后台线程执行）
                    return _cameraDriver.StopGrab();
                });

                // 后台操作完成后，在主线程更新UI
                if (stopSuccess)
                {
                    AntdUI.Notification.success(this.FindForm(), "相机", "停止采集成功");
                    // 恢复按钮状态
                    btn_StartGrab.Enabled = true;
                    btn_StopGrab.Enabled = false;
                    button_TriggerExec.Enabled = false;
                }
                else
                {
                    AntdUI.Notification.error(this.FindForm(), "相机", "停止采集失败");
                    // 恢复按钮状态（允许重试）
                    btn_StopGrab.Enabled = true;
                    btn_StartGrab.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                // 捕获异常，避免程序崩溃
                AntdUI.Notification.error(this.FindForm(), "错误", $"停止采集异常: {ex.Message}");
                // 异常时恢复按钮状态
                btn_StopGrab.Enabled = true;
                btn_StartGrab.Enabled = false;
            }
        }
        #endregion

        #region 导入图片
        private void btn_Importimage_Click(object sender, EventArgs e)
        {


            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "图像文件|*.bmp;*.jpg;*.jpeg;*.png;*.tif|所有文件|*.*";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = fileDialog.FileName;
                HObject inputImage = null;
                try
                {
                    // 读取图像
                    HOperatorSet.ReadImage(out inputImage, fileName);
                    AntdUI.Notification.info(this.FindForm(), "图像导入成功", $"已加载图像：{Path.GetFileName(fileName)}");
                    DetectAndNotify(inputImage);
                    ImageImported?.Invoke(this, inputImage);
                    // 2. 直接执行模板对比检测（去掉hdev脚本检查，因为用的是reg模板）

                }
                catch (Exception ex)
                {
                    AntdUI.Notification.error(this.FindForm(), "图像导入失败", $"读取图像出错：{ex.Message}");
                }

            }
        }
        #endregion

        #region Halcon脚本检测相关方法

        //private async void DetectAndNotify(HObject originalImage)
        //{
        //    if (currentTemplate == null
        //       || currentTemplate.NccModelID == null
        //       || currentTemplate.NccModelID.Length == 0
        //       || currentTemplate.Region == null
        //       || currentTemplate.Region.IsEmpty())
        //    {
        //        AntdUI.Notification.warn(this.FindForm(), "检测失败", "请先加载有效的模板（包含.ncm和.reg文件）");
        //        return;
        //    }
        //    if (CurrentWorkOrder == null)
        //    {
        //        AntdUI.Notification.error(this.FindForm(), "未选择工单", "请先选择工单");
        //        return;
        //    }

        //    // 从数据库获取最新工单状态
        //   Expression<Func<WorkOrder, bool>> expression=(wo => wo.WorkOrderCode == CurrentWorkOrder.WorkOrderCode && !wo.IsDeleted);
        //    var currentWorkOrderFromDB = await _workOrderBLL.GetModelAsync(expression).ConfigureAwait(false);
        //    if (currentWorkOrderFromDB == null)
        //    {
        //        AntdUI.Notification.error(this.FindForm(), "工单不存在", "无法进行检测");
        //        return;
        //    }

        //    var status = currentWorkOrderFromDB.OrderStatus;
        //    if (status == OrderStatu.Canceled)
        //    {
        //        AntdUI.Notification.error(this.FindForm(), "工单已取消", "无法进行检测");
        //        return;
        //    }
        //    else if (status == OrderStatu.Completed)
        //    {
        //        AntdUI.Notification.error(this.FindForm(), "工单已完成", "无法进行检测");
        //        return;
        //    }
        //    else if (status == OrderStatu.Pending || status == OrderStatu.Paused)
        //    {
        //        // 更新工单状态为处理中
        //        Expression<Func<WorkOrder, WorkOrder>> updateColumns = w => new WorkOrder
        //        {
        //            OrderStatus = OrderStatu.Processing
        //        };
        //        Expression<Func<WorkOrder, bool>> whereCondition = w => w.WorkOrderCode == CurrentWorkOrder.WorkOrderCode;
        //        bool updateSuccess = await _workOrderBLL.UpdateAsync(updateColumns, whereCondition).ConfigureAwait(false);
        //        if (updateSuccess)
        //        {
        //            // 更新当前工单对象的状态，避免多次查询
        //            CurrentWorkOrder.OrderStatus = OrderStatu.Processing;
        //            AntdUI.Notification.info(this.FindForm(), "工单状态已更新", "工单状态已更新为处理中");
        //        }
        //        else
        //        {
        //            AntdUI.Notification.error(this.FindForm(), "工单状态更新失败", "无法将工单状态更新为处理中");
        //            return;
        //        }
        //    }
        //    string pcbsn = GeneratePCBSN();
        //    string imageSavePath = Path.Combine(_inspectimageSaveDir, $"{pcbsn}.png");
        //    if (!Directory.Exists(_inspectimageSaveDir))
        //        Directory.CreateDirectory(_inspectimageSaveDir);

        //    byte[] imageBytes = null;
        //    // 中间变量（需手动释放）
        //    HObject testGray = null;
        //    HObject testRegion = null;
        //    HObject matchedRegion = null;
        //    HObject missingRegions = null;
        //    HObject validMissing = null;
        //    HObject annotatedImage = null;

        //    try
        //    {
        //        // 1. 验证原始图像
        //        if (!IsValidHObject(originalImage))
        //        {
        //            AntdUI.Notification.error(this.FindForm(), "图像无效", "原始图像为空或未初始化");
        //            return;
        //        }

        //        // 2. 测试图像灰度化（用于NCC匹配）
        //        HOperatorSet.Rgb1ToGray(originalImage, out testGray);

        //        // 3. NCC模板匹配
        //        HTuple matchRow = new HTuple();
        //        HTuple matchCol = new HTuple();
        //        HTuple matchAngle = new HTuple();
        //        HTuple score = new HTuple();
        //        HOperatorSet.FindNccModel(
        //            testGray,
        //            currentTemplate.NccModelID,
        //            -0.1745, 0.349,
        //            0.8,
        //            1,
        //            0.5,
        //            "true", 0,
        //            out matchRow, out matchCol, out matchAngle, out score);

        //        // 校验匹配结果
        //        if (matchRow.Length == 0 || score[0].D < 0.8)
        //        {
        //            AntdUI.Notification.warn(this.FindForm(), "匹配失败", $"未找到有效匹配（得分：{score[0].D:0.2f}）");
        //            return;
        //        }

        //        // 存储定位结果
        //        _matchRow = matchRow;
        //        _matchCol = matchCol;
        //        _matchAngle = matchAngle;

        //        // 4. 生成变换矩阵
        //        HTuple homMat2D = new HTuple();
        //        HOperatorSet.VectorAngleToRigid(
        //            currentTemplate.RefRow, currentTemplate.RefCol, currentTemplate.RefAngle,
        //            matchRow[0], matchCol[0], matchAngle[0],
        //            out homMat2D);

        //        // 5. 模板区域对齐到测试图像
        //        HOperatorSet.AffineTransRegion(
        //            currentTemplate.Region,
        //            out matchedRegion,
        //            homMat2D,
        //            "nearest_neighbor");

        //        // 6. 调用预处理方法
        //        testRegion = PreprocessImage(originalImage);

        //        // 7. 差分检测缺陷
        //        HOperatorSet.Difference(matchedRegion, testRegion, out missingRegions);

        //        // 8. 过滤噪声
        //        HOperatorSet.SelectShape(missingRegions, out validMissing, "area", "and", 700, 99999);

        //        // 9. 统计结果
        //        HOperatorSet.CountObj(validMissing, out HTuple missingCount);

        //        // 10. 【关键修复】直接创建并保存标注图像
        //        annotatedImage = CreateAndSaveAnnotatedImageDirectly(originalImage, validMissing, missingCount, pcbsn, imageSavePath);

        //        // 11. 读取保存的图像字节流
        //        if (File.Exists(imageSavePath))
        //        {
        //            imageBytes = File.ReadAllBytes(imageSavePath);
        //            AntdUI.Notification.info(this.FindForm(), "标注图像已保存", $"路径：{imageSavePath}");
        //        }

        //        // 12. 创建检测结果事件参数
        //        var detectionResult = new DetectionResultEventArgs
        //        {
        //            OriginalImage = originalImage,
        //            AnnotatedImage = annotatedImage,
        //            MissingRegions = validMissing ?? new HObject(),
        //            MissingCount = missingCount,
        //            PCBSN = pcbsn,
        //        };

        //        // 13. 触发检测完成事件
        //        DetectionCompleted?.Invoke(this, detectionResult);

        //        // 14. 校验工单号和PCB编号
        //        string workOrderCode = CurrentWorkOrder?.WorkOrderCode ?? "未知工单";
        //        string pcbCode = CurrentSelectedPCB?.mesPCBs?.PCBCode ?? "未知PCB";

        //        // 15. 保存到数据库
        //        await SaveDetectionResultToDatabase(detectionResult, workOrderCode, pcbCode, pcbsn, imageBytes).ConfigureAwait(false);
        //        var updatedWorkOrder = await UpdateWorkOrderCompleteQuantityAsync(workOrderCode);
        //        await PushToMesAfterSave(detectionResult, workOrderCode, pcbCode, pcbsn, updatedWorkOrder.OrderStatus).ConfigureAwait(false);

        //        // 16. 记录日志和统计
        //        string inspectionResult = (missingCount == 0) ? "合格" : "不合格";
        //        LogStatu logStatus = inspectionResult == "合格" ? LogStatu.Success : LogStatu.Failure;
        //        await LogEventHelper.PushLog(LogType.InspectionOperation, $"{pcbsn}:检测{inspectionResult}", logStatus).ConfigureAwait(false);

        //        var defectStats = new PCBDefectStatisticsEventArgs
        //        {
        //            PCBSN = pcbsn,
        //            DefectName = missingCount == 0 ? "无缺陷" : "缺孔",
        //            DefectCount = missingCount.I
        //        };
        //        DefectStatisticsUpdated?.Invoke(this, defectStats);

        //        if (CurrentWorkOrder != null && CurrentSelectedPCB != null)
        //        {
        //            WorkOrderDetectionUpdated?.Invoke(this, new WorkOrderUpdateDTO
        //            {
        //                PCBSN = pcbsn,
        //                IsQualified = missingCount == 0,
        //                TargetPCBType = CurrentSelectedPCB.PCBCode ?? "未知PCB",
        //                OrderStatus = updatedWorkOrder?.OrderStatus ?? CurrentWorkOrder.OrderStatus
        //            });
        //        }

        //        AntdUI.Notification.success(this.FindForm(), "检测完成", $"找到{missingCount.I}个缺失缺陷");

        //    }
        //    catch (Exception ex)
        //    {
        //        AntdUI.Notification.error(this.FindForm(), "检测异常", ex.Message);
        //        await LogEventHelper.PushLog(LogType.InspectionOperation, $"检测异常：{ex.Message}", LogStatu.Error).ConfigureAwait(false);
        //    }
        //    finally
        //    {
        //        // 释放中间变量
        //        testGray?.Dispose();
        //        testRegion?.Dispose();
        //        matchedRegion?.Dispose();
        //        missingRegions?.Dispose();
        //        validMissing?.Dispose();
        //    }
        //}
        #region 加载图像格式
        private void LoadImageFormats(bool forceRefresh = false)
        {
            try
            {
                Console.WriteLine("开始加载图像格式...");

                // 清空下拉框
                select_Resolution.Items.Clear();
                _imageFormats.Clear();
                bool isCameraValid = false;
                try
                {
                    // 尝试调用相机方法，确认相机可用
                    isCameraValid = _cameraDriver != null && _cameraDriver.IsConnect;
                    Console.WriteLine($"相机有效性检查: {isCameraValid}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"相机检查异常: {ex.Message}");
                    isCameraValid = false;
                }
                if (_cameraDriver != null && _cameraDriver.IsConnect)
                {
                    // 获取相机支持的图像格式
                    _imageFormats = _cameraDriver.GetImageFormats();

                    Console.WriteLine($"获取到 {_imageFormats.Count} 种格式");

                    if (_imageFormats.Count > 0)
                    {
                        // 添加到下拉框
                        foreach (string format in _imageFormats)
                        {
                            select_Resolution.Items.Add(format);
                            Console.WriteLine($"添加格式到下拉框: {format}");
                        }

                        // 获取当前格式并选中
                        string currentFormat = _cameraDriver.GetCurrentFormat();
                        Console.WriteLine($"当前格式: {currentFormat}");

                        // 尝试找到并选中当前格式
                        bool found = false;
                        for (int i = 0; i < select_Resolution.Items.Count; i++)
                        {
                            if (select_Resolution.Items[i].ToString().Equals(currentFormat, StringComparison.OrdinalIgnoreCase))
                            {
                                select_Resolution.SelectedIndex = i;
                                found = true;
                                Console.WriteLine($"选中格式索引: {i}");
                                break;
                            }
                        }

                        // 如果没有找到当前格式，选中第一个
                        if (!found && select_Resolution.Items.Count > 0)
                        {
                            select_Resolution.SelectedIndex = 0;
                            Console.WriteLine("默认选中第一个格式");
                        }

                        // 启用下拉框
                        select_Resolution.Enabled = true;
                    }
                    else
                    {
                        select_Resolution.Items.Add("无可用格式");
                        select_Resolution.SelectedIndex = 0;
                        select_Resolution.Enabled = false;
                        Console.WriteLine("无可用格式");
                    }
                }
                else
                {
                    select_Resolution.Items.Add("相机未连接");
                    select_Resolution.SelectedIndex = 0;
                    select_Resolution.Enabled = false;
                    Console.WriteLine("相机未连接");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载图像格式失败: {ex.Message}");
                select_Resolution.Items.Clear();
                select_Resolution.Items.Add("加载失败");
                select_Resolution.SelectedIndex = 0;
                select_Resolution.Enabled = false;
            }
        }
        #endregion
        private async void DetectAndNotify(HObject originalImage)
        {
            if (currentTemplate == null
               || currentTemplate.NccModelID == null
               || currentTemplate.NccModelID.Length == 0
               || currentTemplate.Region == null
               || currentTemplate.Region.IsEmpty())
            {
                AntdUI.Notification.warn(this.FindForm(), "检测失败", "请先加载有效的模板（包含.ncm和.reg文件）");
                return;
            }
            if (CurrentWorkOrder == null)
            {
                AntdUI.Notification.error(this.FindForm(), "未选择工单", "请先选择工单");
                return;
            }

            // 从数据库获取最新工单状态
            Expression<Func<WorkOrder, bool>> expression = (wo => wo.WorkOrderCode == CurrentWorkOrder.WorkOrderCode && !wo.IsDeleted);
            var currentWorkOrderFromDB = await _workOrderBLL.GetModelAsync(expression).ConfigureAwait(false);
            if (currentWorkOrderFromDB == null)
            {
                AntdUI.Notification.error(this.FindForm(), "工单不存在", "无法进行检测");
                return;
            }

            var status = currentWorkOrderFromDB.OrderStatus;
            if (status == OrderStatu.Canceled)
            {
                AntdUI.Notification.error(this.FindForm(), "工单已取消", "无法进行检测");
                return;
            }
            else if (status == OrderStatu.Completed)
            {
                AntdUI.Notification.error(this.FindForm(), "工单已完成", "无法进行检测");
                return;
            }
            else if (status == OrderStatu.Pending || status == OrderStatu.Paused)
            {
                // 更新工单状态为处理中，并设置开始时间
                Expression<Func<WorkOrder, WorkOrder>> updateColumns = w => new WorkOrder
                {
                    OrderStatus = OrderStatu.Processing,
                    StartTime = DateTime.Now // 【新增】设置开始时间为当前时间
                };
                Expression<Func<WorkOrder, bool>> whereCondition = w => w.WorkOrderCode == CurrentWorkOrder.WorkOrderCode;
                bool updateSuccess = await _workOrderBLL.UpdateAsync(updateColumns, whereCondition).ConfigureAwait(false);
                if (updateSuccess)
                {
                    // 更新当前工单对象的状态和开始时间，避免多次查询
                    CurrentWorkOrder.OrderStatus = OrderStatu.Processing;
                    CurrentWorkOrder.StartTime = DateTime.Now; // 【新增】更新当前工单对象的开始时间
                    AntdUI.Notification.info(this.FindForm(), "工单状态已更新", "工单状态已更新为处理中，开始时间已记录");
                }
                else
                {
                    AntdUI.Notification.error(this.FindForm(), "工单状态更新失败", "无法将工单状态更新为处理中");
                    return;
                }
            }

            string pcbsn = GeneratePCBSN();
            string imageSavePath = Path.Combine(_inspectimageSaveDir, $"{pcbsn}.png");
            if (!Directory.Exists(_inspectimageSaveDir))
                Directory.CreateDirectory(_inspectimageSaveDir);

            byte[] imageBytes = null;
            // 中间变量（需手动释放）
            HObject testGray = null;
            HObject testRegion = null;
            HObject matchedRegion = null;
            HObject missingRegions = null;
            HObject validMissing = null;
            HObject annotatedImage = null;

            try
            {
                // 1. 验证原始图像
                if (!IsValidHObject(originalImage))
                {
                    AntdUI.Notification.error(this.FindForm(), "图像无效", "原始图像为空或未初始化");
                    return;
                }

                // 2. 测试图像灰度化（用于NCC匹配）
                HOperatorSet.Rgb1ToGray(originalImage, out testGray);

                // 3. NCC模板匹配
                HTuple matchRow = new HTuple();
                HTuple matchCol = new HTuple();
                HTuple matchAngle = new HTuple();
                HTuple score = new HTuple();
                HOperatorSet.FindNccModel(
                    testGray,
                    currentTemplate.NccModelID,
                    -0.1745, 0.349,
                    0.8,
                    1,
                    0.5,
                    "true", 0,
                    out matchRow, out matchCol, out matchAngle, out score);

                // 校验匹配结果
                if (matchRow.Length == 0 || score[0].D < 0.8)
                {
                    AntdUI.Notification.warn(this.FindForm(), "匹配失败", $"未找到有效匹配（得分：{score[0].D:0.2f}）");
                    return;
                }

                // 存储定位结果
                _matchRow = matchRow;
                _matchCol = matchCol;
                _matchAngle = matchAngle;

                // 4. 生成变换矩阵
                HTuple homMat2D = new HTuple();
                HOperatorSet.VectorAngleToRigid(
                    currentTemplate.RefRow, currentTemplate.RefCol, currentTemplate.RefAngle,
                    matchRow[0], matchCol[0], matchAngle[0],
                    out homMat2D);

                // 5. 模板区域对齐到测试图像
                HOperatorSet.AffineTransRegion(
                    currentTemplate.Region,
                    out matchedRegion,
                    homMat2D,
                    "nearest_neighbor");

                // 6. 调用预处理方法
                testRegion = PreprocessImage(originalImage);

                // 7. 差分检测缺陷
                HOperatorSet.Difference(matchedRegion, testRegion, out missingRegions);

                // 8. 过滤噪声
                // HOperatorSet.SelectShape(missingRegions, out validMissing, "area", "and", 1000, 1400);
                HOperatorSet.SelectShape(missingRegions, out validMissing,
                  new HTuple("area", "circularity"),  // 增加圆形度特征
                  new HTuple("and"),
                 new HTuple(1000, 0.2),
new HTuple(9900, 1.0));

                // 9. 统计结果
                HOperatorSet.CountObj(validMissing, out HTuple missingCount);

                // 10. 【关键修复】直接创建并保存标注图像
                annotatedImage = CreateAndSaveAnnotatedImageDirectly(originalImage, validMissing, missingCount, pcbsn, imageSavePath);

                // 11. 读取保存的图像字节流
                if (File.Exists(imageSavePath))
                {
                    imageBytes = File.ReadAllBytes(imageSavePath);
                    AntdUI.Notification.info(this.FindForm(), "标注图像已保存", $"路径：{imageSavePath}");
                }

                // 12. 创建检测结果事件参数
                var detectionResult = new DetectionResultEventArgs
                {
                    OriginalImage = originalImage,
                    AnnotatedImage = annotatedImage,
                    MissingRegions = validMissing ?? new HObject(),
                    MissingCount = missingCount,
                    PCBSN = pcbsn,
                };

                // 13. 触发检测完成事件
                DetectionCompleted?.Invoke(this, detectionResult);

                // 14. 校验工单号和PCB编号
                string workOrderCode = CurrentWorkOrder?.WorkOrderCode ?? "未知工单";
                string pcbCode = CurrentSelectedPCB?.mesPCBs?.PCBCode ?? "未知PCB";

                // 15. 保存到数据库
                await SaveDetectionResultToDatabase(detectionResult, workOrderCode, pcbCode, pcbsn, imageBytes).ConfigureAwait(false);

                // 【修改】更新工单完成数量，并检查是否需要设置结束时间
                var updatedWorkOrder = await UpdateWorkOrderCompleteQuantityWithEndTimeAsync(workOrderCode);

                await PushToMesAfterSave(detectionResult, workOrderCode, pcbCode, pcbsn, updatedWorkOrder.OrderStatus).ConfigureAwait(false);

                // 16. 记录日志和统计
                string inspectionResult = (missingCount == 0) ? "合格" : "不合格";
                LogStatu logStatus = inspectionResult == "合格" ? LogStatu.Success : LogStatu.Failure;
                await LogEventHelper.PushLog(LogType.InspectionOperation, $"{pcbsn}:检测{inspectionResult}", logStatus).ConfigureAwait(false);

                var defectStats = new PCBDefectStatisticsEventArgs
                {
                    PCBSN = pcbsn,
                    DefectName = missingCount == 0 ? "无缺陷" : "缺孔",
                    DefectCount = missingCount.I
                };
                DefectStatisticsUpdated?.Invoke(this, defectStats);

                if (CurrentWorkOrder != null && CurrentSelectedPCB != null)
                {
                    WorkOrderDetectionUpdated?.Invoke(this, new WorkOrderUpdateDTO
                    {
                        PCBSN = pcbsn,
                        IsQualified = missingCount == 0,
                        TargetPCBType = CurrentSelectedPCB.PCBCode ?? "未知PCB",
                        OrderStatus = updatedWorkOrder?.OrderStatus ?? CurrentWorkOrder.OrderStatus
                    });
                }

                AntdUI.Notification.success(this.FindForm(), "检测完成", $"找到{missingCount.I}个缺失缺陷");

            }
            catch (Exception ex)
            {
                AntdUI.Notification.error(this.FindForm(), "检测异常", ex.Message);
                await LogEventHelper.PushLog(LogType.InspectionOperation, $"检测异常：{ex.Message}", LogStatu.Error).ConfigureAwait(false);
            }
            finally
            {
                // 释放中间变量
                testGray?.Dispose();
                testRegion?.Dispose();
                matchedRegion?.Dispose();
                missingRegions?.Dispose();
                validMissing?.Dispose();
            }
        }

        // 【新增】更新工单完成数量并设置结束时间的方法
        private async Task<WorkOrder> UpdateWorkOrderCompleteQuantityWithEndTimeAsync(string workOrderCode)
        {
            if (string.IsNullOrEmpty(workOrderCode))
            {
                await LogEventHelper.PushLog(LogType.DatabaseOperation, "更新工单失败：工单号为空", LogStatu.Failure).ConfigureAwait(false);
                return null;
            }

            try
            {
                // 1. 先查询工单当前状态
                Expression<Func<WorkOrder, bool>> expression = w => w.WorkOrderCode == workOrderCode;
                var currentWorkOrder = await _workOrderBLL.GetModelAsync(expression).ConfigureAwait(false);
                if (currentWorkOrder == null)
                {
                    await LogEventHelper.PushLog(LogType.DatabaseOperation, $"更新工单失败：未找到工单号 {workOrderCode}", LogStatu.Failure).ConfigureAwait(false);
                    return null;
                }

                // 2. 校验计划数量
                if (currentWorkOrder.PlanQuantity <= 0)
                {
                    await LogEventHelper.PushLog(LogType.DatabaseOperation, $"更新工单失败：工单号 {workOrderCode} 计划数量为0", LogStatu.Warning).ConfigureAwait(false);
                    return currentWorkOrder;
                }

                // 3. 计算新的完成数量
                int newCompleteQuantity = currentWorkOrder.CompleteQuantity ?? 0;
                newCompleteQuantity += 1;

                // 4. 判断是否完成
                bool isCompleted = newCompleteQuantity >= currentWorkOrder.PlanQuantity;

                // 5. 构建更新字段表达式
                Expression<Func<WorkOrder, WorkOrder>> updateColumns = w => new WorkOrder
                {
                    CompleteQuantity = newCompleteQuantity,
                    OrderStatus = isCompleted ? OrderStatu.Completed : currentWorkOrder.OrderStatus,
                    EndTime = isCompleted ? DateTime.Now : currentWorkOrder.EndTime // 【新增】如果完成，设置结束时间
                };

                // 6. 构建查询条件表达式
                Expression<Func<WorkOrder, bool>> whereCondition = w =>
                    w.WorkOrderCode == workOrderCode && w.Id == currentWorkOrder.Id;

                // 7. 更新数据库
                bool updateSuccess = await _workOrderBLL.UpdateAsync(updateColumns, whereCondition).ConfigureAwait(false);

                if (updateSuccess)
                {
                    // 8. 查询更新后的最新数据
                    var updatedWorkOrder = await _workOrderBLL.GetModelAsync(expression).ConfigureAwait(false);

                    string statusChangeLog = $"工单更新成功：工单号 {workOrderCode}，完成数量 {newCompleteQuantity}，状态 {currentWorkOrder.OrderStatus} → {updatedWorkOrder.OrderStatus}";

                    if (isCompleted)
                    {
                        statusChangeLog += $"，结束时间已设置为：{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                    }

                    await LogEventHelper.PushLog(LogType.DatabaseOperation, statusChangeLog, LogStatu.Success).ConfigureAwait(false);

                    return updatedWorkOrder;
                }
                else
                {
                    await LogEventHelper.PushLog(LogType.DatabaseOperation,
                        $"工单更新失败：工单号 {workOrderCode}（无匹配记录或未修改）",
                        LogStatu.Failure).ConfigureAwait(false);
                    return currentWorkOrder;
                }
            }
            catch (Exception ex)
            {
                await LogEventHelper.PushLog(LogType.DatabaseOperation,
                    $"工单更新异常：{ex.Message}",
                    LogStatu.Error).ConfigureAwait(false);
                return null;
            }
        }

        private async Task PushToMesAfterSave(DetectionResultEventArgs detectionResult, string workOrderCode, string pcbCode, string pcbsn,OrderStatu workOrderStatus)
        {
            bool isQualified = detectionResult.MissingCount == 0;
            var inspectionRecord = await _pcbInspectionBLL.GetModelAsync(p => p.PCBSN == pcbsn).ConfigureAwait(false);
            var pushDto = new PcbInspectionPushDTO
            {
                // 基础必填字段
                InspectionCode = inspectionRecord.InspectionCode ?? inspectionRecord.Id.ToString(),
                WorkOrderCode = workOrderCode ?? "未知工单",
                PCBCode = pcbCode ?? "未知PCB",
                PCBSN = pcbsn,
                InspectionTime = DateTime.Now,
                IsQualified = isQualified, // 替换原InspectionResult→IsQualified
                WorkId = LoginConfig.WorkId ?? string.Empty, // 替换原OperatorWorkId→WorkId
                WorkName = LoginConfig.Name ?? "未知姓名", // 补充必填字段
                ProductLine = LoginConfig.ProductLine ?? "未知产线", // 补充必填字段
                WorkOrderStatus = workOrderStatus,
                DetectedDefects = new List<MesInspectionDefect>() // 初始化缺陷列表
            };
            if (detectionResult.MissingCount > 0)
            {
                var defect = new MesInspectionDefect
                {
                    // 关联检测记录ID（与顶层一致，MES 用于绑定缺陷和检测单）
                    InspectionCode = inspectionRecord.InspectionCode ?? inspectionRecord.Id.ToString(),
                    DefectCode = "PCBA0001missinghole",
                    DefectName = "缺孔",
                    Count = detectionResult.MissingCount.I
                };

                pushDto.DetectedDefects.Add(defect);
                // 调用推送方法（忽略推送结果，避免影响本地业务；如需重试可在此扩展）

            }
            else
            {
                // 合格时添加无缺陷记录，适配MES接口要求
                pushDto.DetectedDefects.Add(new MesInspectionDefect
                {
                    InspectionCode = inspectionRecord.Id.ToString(),
                    DefectCode = "nodefect",
                    DefectName = "无缺陷",
                    Count = 0
                });
            }
            await PushPcbInspectionResultToMesAsync(pushDto).ConfigureAwait(false);
        }
        /// <summary>
        /// 根据 PCBSN 更新数据库中的标注图
        /// </summary>
        /// <param name="pcbsn">PCB唯一编号（关联数据库记录）</param>
        /// <param name="markedImageBytes">标注后的图像字节数组（PNG格式）</param>
        public async Task UpdateMarkedImageToDatabase(string pcbsn, byte[] markedImageBytes)
        {
            if (string.IsNullOrEmpty(pcbsn) || markedImageBytes == null || markedImageBytes.Length == 0)
            {
                await LogEventHelper.PushLog(LogType.DatabaseOperation, "更新标注图失败：参数无效", LogStatu.Failure);
                return;
            }

            try
            {
                // 1. 根据 PCBSN 找到之前插入的检测记录
                var inspectionRecord = await _pcbInspectionBLL.GetModelAsync(
                    p => p.PCBSN == pcbsn
                    );

                if (inspectionRecord == null)
                {
                    await LogEventHelper.PushLog(LogType.DatabaseOperation, $"更新标注图失败：未找到 PCBSN={pcbsn} 的记录", LogStatu.Failure);
                    return;
                }

                // 2. 更新 Image 字段为标注后的图像字节数组
                inspectionRecord.Image = markedImageBytes;
                await _pcbInspectionBLL.UpdateAsync(inspectionRecord); // 保存更新

                await LogEventHelper.PushLog(LogType.DatabaseOperation, $"标注图更新成功：PCBSN={pcbsn}", LogStatu.Success);
            }
            catch (Exception ex)
            {
                await LogEventHelper.PushLog(LogType.DatabaseOperation, $"更新标注图失败：{ex.Message}", LogStatu.Error);
            }
        }

        #region 导入模板
        private List<PcbTemplateResource> allTemplates = new List<PcbTemplateResource>();
        private void btn_ImportModel_Click(object sender, EventArgs e)
        {

            try
            {
                // 1. 选择模板文件夹（默认路径设为用户指定的文件夹）
                AntdUI.FolderBrowserDialog fbd = new AntdUI.FolderBrowserDialog();
                fbd.DirectoryPath = @"C:\Users\Lenovo\Desktop\CSharp项目\就业项目\Wedjat\PCB"; // 默认路径
                DialogResult result = fbd.ShowDialog();
                if (result != DialogResult.OK)
                    return;
                string templateDir = fbd.DirectoryPath;

                // 2. 清空现有数据（避免重复加载）
                allTemplates.Clear();
                select_Model.Items.Clear();
                currentTemplate = null;
                _matchRow = new HTuple();
                _matchCol = new HTuple();
                _matchAngle = new HTuple();
                // 3. 扫描文件夹内的所有相关文件（.reg、.ncm、.tup）
                string[] regFiles = Directory.GetFiles(templateDir, "*.reg");    // 区域文件
                string[] ncmFiles = Directory.GetFiles(templateDir, "*.ncm");    // NCC模板文件
                string[] tupFiles = Directory.GetFiles(templateDir, "*.tup");    // 参考点文件

                if (regFiles.Length == 0 && ncmFiles.Length == 0 && tupFiles.Length == 0)
                {
                    AntdUI.Notification.warn(this.FindForm(), "无模板文件", "文件夹中未找到.reg、.ncm或.tup文件");
                    return;
                }

                // 4. 按文件名前缀分组（关联同一模板的不同文件）
                // 例如："template1.reg"、"template1.ncm"、"template1.tup"属于同一模板
                HashSet<string> templateNames = new HashSet<string>();
                foreach (var file in regFiles)
                    templateNames.Add(Path.GetFileNameWithoutExtension(file));
                foreach (var file in ncmFiles)
                    templateNames.Add(Path.GetFileNameWithoutExtension(file));
                foreach (var file in tupFiles)
                    templateNames.Add(Path.GetFileNameWithoutExtension(file));

                // 5. 批量读取每个模板的所有关联文件
                foreach (string name in templateNames)
                {
                    var template = new PcbTemplateResource { TemplateName = name };

                    // 读取.reg区域文件
                    string regPath = Path.Combine(templateDir, $"{name}.reg");
                    if (File.Exists(regPath))
                    {
                        HObject region;
                        HOperatorSet.ReadRegion(out region, regPath);
                        template.Region = region;
                    }

                    // 读取.ncm模板文件
                    string ncmPath = Path.Combine(templateDir, $"{name}.ncm");
                    if (File.Exists(ncmPath))
                    {
                        HTuple modelID;
                        HOperatorSet.ReadNccModel(ncmPath, out modelID);
                        template.NccModelID = modelID;
                    }

                    // 读取.tup参考点文件（格式：[行, 列, 角度]）
                    string tupPath = Path.Combine(templateDir, $"{name}.tup");
                    if (File.Exists(tupPath))
                    {
                        HTuple refData;
                        HOperatorSet.ReadTuple(tupPath, out refData);
                        if (refData.Length >= 3)
                        {
                            template.RefRow = refData[0];
                            template.RefCol = refData[1];
                            template.RefAngle = refData[2];
                        }
                    }

                    // 添加到模板列表
                    allTemplates.Add(template);
                    select_Model.Items.Add(name); // 下拉列表添加模板名称
                }

                // 6. 提示加载成功
                AntdUI.Notification.success(this.FindForm(), "模板批量加载成功",
                    $"共加载 {allTemplates.Count} 个模板，路径：{templateDir}");

                // 自动选中第一个模板（可选）
                if (select_Model.Items.Count > 0)
                    select_Model.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                AntdUI.Notification.error(this.FindForm(), "模板加载失败",
                    $"错误信息：{ex.Message}");
                // 清理已加载的资源
                foreach (var temp in allTemplates)
                {
                    if (temp.NccModelID.Length == 0)
                        HOperatorSet.ClearNccModel(temp.NccModelID);
                    temp.Region?.Dispose();
                }
                allTemplates.Clear();
                select_Model.Items.Clear();
                currentTemplate = null;
            }
        }

        #endregion

        #region 图像预处理
        private HObject PreprocessImage(HObject originalImage)
        {

            HObject templateMatchROI = null;
            HObject grayImage = null;
            HObject gaussGray = null;
            HObject markCandidates = null;
            HObject connectedRegions = null;
            HObject selectedMarks = null;
            HObject polygonRegion = null;
            HObject filledRegion = null;
            HObject imageROI = null;
            HObject grayROI = null;
            HObject filteredImage = null;
            HObject thresholdRegion = null;
            HObject connectedDefects = null;
            HObject selectedDefects = null;
            HObject erodedRegion = null;
            HObject dilatedRegion = null;

            try
            {
                // 1. 灰度化
                HOperatorSet.Rgb1ToGray(originalImage, out grayImage);

                HTuple matchRow = new HTuple();
                HTuple matchCol = new HTuple();
                HTuple matchAngle = new HTuple();
                HTuple score = new HTuple();

                if (_matchRow.Length == 0 || _matchCol.Length == 0 || _matchAngle.Length == 0)
                {
                    throw new Exception("匹配参数无效，请先进行模板匹配");
                }

                // 【修改1】模板匹配ROI大小调整为2000×1000（Halcon参数）
                HOperatorSet.GenRectangle2(out templateMatchROI,
                    _matchRow[0], _matchCol[0], _matchAngle[0],
                    2000, 1000);  // 原为1600, 800

                HOperatorSet.ReduceDomain(grayImage, templateMatchROI, out grayImage);

                // 【修改2】高斯滤波核大小调整为11（Halcon参数）
                HOperatorSet.GaussFilter(grayImage, out gaussGray, 11);  // 原为5

                // 2. 二值化找标点（阈值与Halcon一致100-255）
                HOperatorSet.Threshold(gaussGray, out markCandidates, 100, 255);

                // 3. 连通区域分析
                HOperatorSet.Connection(markCandidates, out connectedRegions);

                // 【修改3】筛选标点参数调整（增加圆形度筛选）
                // Halcon参数：面积6000-90000，圆形度0.4-1.0
                HOperatorSet.SelectShape(connectedRegions, out selectedMarks,
                    new HTuple("area", "circularity"),  // 增加圆形度特征
                    new HTuple("and"),
                    new HTuple(6000, 0.4),      // 最小面积6000，最小圆形度0.4
                    new HTuple(90000, 1.0));    // 最大面积90000，最大圆形度1.0

                // 校验标点数量（必须为4个才能生成四边形）
                HOperatorSet.CountObj(selectedMarks, out HTuple markCount);
                if (markCount.I != 4)
                {
                    throw new Exception($"标点数量异常，需4个实际检测到{markCount.I}个");
                }

                // 4. 获取标点中心坐标
                HOperatorSet.AreaCenter(selectedMarks, out _, out HTuple rowDetect, out HTuple colDetect);

                // 5. 构造四边形顶点（闭合多边形）
                HTuple rowPoints = new HTuple(new double[] {
            rowDetect[0].D,
            rowDetect[1].D,
            rowDetect[3].D,
            rowDetect[2].D,
            rowDetect[0].D
        });
                HTuple colPoints = new HTuple(new double[] {
            colDetect[0].D,
            colDetect[1].D,
            colDetect[3].D,
            colDetect[2].D,
            colDetect[0].D
        });

                // 6. 生成多边形ROI
                HOperatorSet.GenRegionPolygon(out polygonRegion, rowPoints, colPoints);

                // 7. 填充多边形
                HOperatorSet.FillUp(polygonRegion, out filledRegion);

                // 8. 提取ROI区域
                HOperatorSet.ReduceDomain(originalImage, filledRegion, out imageROI);

                // 9. ROI区域灰度化
                HOperatorSet.Rgb1ToGray(imageROI, out grayROI);

                // 【修改4】ROI区域高斯滤波核大小调整为11（Halcon参数）
                HOperatorSet.GaussFilter(grayROI, out filteredImage, 11);  // 保持11

                // 10. 二值化（阈值与Halcon一致100-255）
                HOperatorSet.Threshold(filteredImage, out thresholdRegion, 100, 255);

                // 11. 连通缺陷区域
                HOperatorSet.Connection(thresholdRegion, out connectedDefects);

                // 【修改5】筛选缺陷区域面积范围调整（Halcon参数0-8000）
                HOperatorSet.SelectShape(connectedDefects, out selectedDefects,
                    "area", "and", 0, 8000);  // 原为500, 6200

                // 【修改6】腐蚀操作半径调整为8（Halcon参数）
                HOperatorSet.ErosionCircle(selectedDefects, out erodedRegion, 8);  // 原为8，保持

                // 【修改7】膨胀操作半径调整为14（Halcon参数）
                HOperatorSet.DilationCircle(erodedRegion, out dilatedRegion, 14);  // 原为14，保持

                return dilatedRegion;
            }
            finally
            {
                grayImage?.Dispose();
                templateMatchROI?.Dispose();
                gaussGray?.Dispose();
                markCandidates?.Dispose();
                connectedRegions?.Dispose();
                selectedMarks?.Dispose();
                polygonRegion?.Dispose();
                filledRegion?.Dispose();
                imageROI?.Dispose();
                grayROI?.Dispose();
                filteredImage?.Dispose();
                thresholdRegion?.Dispose();
                connectedDefects?.Dispose();
                selectedDefects?.Dispose();
                erodedRegion?.Dispose();
                // 不释放返回对象dilatedRegion
                //  _matchRow.Dispose();
                // _matchCol.Dispose();
                // _matchAngle.Dispose();
                // _matchRow = new HTuple();
                // _matchCol = new HTuple();
                // _matchAngle = new HTuple();
            }
        }
        #endregion

        #region 触发采集
        private void button_TriggerExec_Click(object sender, EventArgs e)
        {
            if (!_cameraDriver.isGrabbing)
            {
                AntdUI.Notification.warn(this.FindForm(), "相机未采集", "请先点击【开始采集】");
                return;
            }

            try
            {

                bool triggerSuccess = _cameraDriver.SnapImage();
                if (!triggerSuccess)
                {
                    AntdUI.Notification.error(this.FindForm(), "采集失败", "软触发采集失败");
                    return;
                }


                Bitmap testBitmap = _cameraDriver.GetLatestFrame();
                if (testBitmap == null)
                {
                    AntdUI.Notification.error(this.FindForm(), "采集失败", "未获取到检测用图像");
                    return;
                }

                // 3. Bitmap转HObject，执行检测
                HObject testImage = null;
                using (Bitmap bmp24 = ConvertTo24bppRgb(testBitmap))
                {
                    testImage = BitmapToHImage(bmp24);
                }
                // HObject testImage = BitmapToHImage(testBitmap);
                if (testImage == null)
                {
                    AntdUI.Notification.error(this.FindForm(), "转换失败", "无法将采集图像转为HALCON格式");
                    return;
                }
                string triggerImgPath = SaveTriggerImage(testImage);
                // 4. 执行检测并传递结果
                DetectAndNotify(testImage);

            }
            catch (Exception ex)
            {
                AntdUI.Notification.error(this.FindForm(), "采集检测异常", ex.Message);
            }
        }
        #endregion

        #region 格式转换方法
        private HObject BitmapToHImage(Bitmap bitmap)
        {
            HObject hImage = null;
            try
            {
                // 如果图像是32位ARGB格式，先转换为24位RGB
                if (bitmap.PixelFormat == PixelFormat.Format32bppArgb)
                {
                    using (Bitmap bmp24 = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format24bppRgb))
                    {
                        using (Graphics g = Graphics.FromImage(bmp24))
                        {
                            g.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height);
                        }
                        bitmap = bmp24; // 使用转换后的位图
                    }
                }

                Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);

                if (bitmap.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    HOperatorSet.GenImageInterleaved(out hImage, new HTuple(bmpData.Scan0), "rgb",
                        bitmap.Width, bitmap.Height, 0, "byte", bitmap.Width, bitmap.Height, 0, 0, -1, 0);
                }
                else if (bitmap.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    HOperatorSet.GenImage1(out hImage, "byte", bitmap.Width, bitmap.Height, new HTuple(bmpData.Scan0));
                }
                else
                {
                    throw new Exception($"不支持的像素格式：{bitmap.PixelFormat}");
                }

                bitmap.UnlockBits(bmpData);
                return hImage;
            }
            catch (Exception ex)
            {
                AntdUI.Notification.error(this.FindForm(), "格式转换失败", ex.Message);
                hImage?.Dispose();
                return null;
            }
        }
        #endregion

        #region 模板选择
        //private void select_Model_SelectedIndexChanged(object sender, IntEventArgs e)
        //{
        //    if (select_Model.SelectedIndex < 0 || allTemplates.Count == 0)
        //    {
        //        currentTemplate = null;
        //        return;
        //    }
        //    currentTemplate = allTemplates[select_Model.SelectedIndex];
        //    AntdUI.Notification.info(this.FindForm(), "模板切换", $"当前使用模板：{currentTemplate.TemplateName}");
        //}
        private void select_Model_SelectedIndexChanged(object sender, IntEventArgs e)
        {
            if (select_Model.SelectedIndex < 0 ||  select_Model.SelectedIndex >= allTemplates.Count)
            {
                currentTemplate = null;
                // 清空匹配参数
                _matchRow = new HTuple();
                _matchCol = new HTuple();
                _matchAngle = new HTuple();
                return;
            }

            try
            {
                Console.WriteLine($"切换模板: 索引={select_Model.SelectedIndex}, 名称={allTemplates[select_Model.SelectedIndex].TemplateName}");
               
                // 清空之前的匹配参数
                _matchRow = new HTuple();
                _matchCol = new HTuple();
                _matchAngle = new HTuple();

                // 检查模板数据
                var newTemplate = allTemplates[select_Model.SelectedIndex];
                currentTemplate = newTemplate;
                // 验证模板数据
                if (newTemplate.NccModelID == null || newTemplate.NccModelID.Length == 0)
                {
                    Console.WriteLine("警告: NCC模型ID为空");
                    AntdUI.Notification.warn(this.FindForm(), "模板无效", "NCC模型ID为空");
                    currentTemplate = null;
                    return;
                }

                if (newTemplate.Region == null || newTemplate.Region.IsEmpty())
                {
                    Console.WriteLine("警告: 区域为空");
                    AntdUI.Notification.warn(this.FindForm(), "模板无效", "区域为空");
                    currentTemplate = null;
                    return;
                }


                Console.WriteLine($"模板切换成功: {currentTemplate.TemplateName}");
                AntdUI.Notification.info(this.FindForm(), "模板切换",
                    $"当前使用模板：{currentTemplate.TemplateName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"切换模板异常: {ex.Message}");
                AntdUI.Notification.error(this.FindForm(), "模板切换失败", ex.Message);
                currentTemplate = null;
            }
        }
        #endregion

        #region 选中采集模式(连续采集/触发采集)
        private async void radio_Mode_CheckedChanged(object sender, BoolEventArgs e)
        {
            if (sender is Radio radio && radio.Checked)
            {
                if (radio == radio_ContinuesMode)
                {
                    bool success = _cameraDriver.SetMode(CameraWorkMode.Continuous);
                    if (success)
                    {
                        // 连续模式：禁用触发相关控件
                        checkbox_softTri.Enabled = false;
                        checkbox_hardTri.Enabled = false;
                        button_TriggerExec.Enabled = false;

                        await LogEventHelper.PushLog(LogType.SystemOperation, "切换到连续模式", LogStatu.Success);

                    }
                    else
                    {
                        await LogEventHelper.PushLog(LogType.SystemOperation, "连续模式切换失败", LogStatu.Failure);
                        AntdUI.Notification.error(this.FindForm(), "相机模式", "连续模式切换失败");
                    }
                }
                else if (radio == radio_TriggerMode)
                {
                    bool success = _cameraDriver.SetMode(CameraWorkMode.Trigger);
                    if (success)
                    {
                        // 触发模式：启用触发相关控件
                        checkbox_softTri.Enabled = true;
                        checkbox_hardTri.Enabled = true;
                        button_TriggerExec.Enabled = true;

                        // 检查是否有选中的触发类型，如果没有则默认选择软触发
                        if (!checkbox_softTri.Checked && !checkbox_hardTri.Checked)
                        {
                            checkbox_softTri.Checked = true;
                            // 设置软触发
                            _cameraDriver.SetMode(CameraWorkMode.Trigger, "Software");
                        }
                        else
                        {
                            // 如果已经有选中的触发类型，确保相机设置正确
                            if (checkbox_softTri.Checked)
                            {
                                _cameraDriver.SetMode(CameraWorkMode.Trigger, "Software");
                            }
                            else if (checkbox_hardTri.Checked)
                            {
                                _cameraDriver.SetMode(CameraWorkMode.Trigger, "Hardware");


                            }
                        }

                        await LogEventHelper.PushLog(LogType.SystemOperation, "切换到触发模式", LogStatu.Success);
                        AntdUI.Notification.success(this.FindForm(), "相机模式", "已切换到触发模式");
                    }
                    else
                    {
                        await LogEventHelper.PushLog(LogType.SystemOperation, "触发模式切换失败", LogStatu.Failure);
                        AntdUI.Notification.error(this.FindForm(), "相机模式", "触发模式切换失败");
                    }
                }
            }
        }
        #endregion

      

        #region 设置滑块默认值
        private void SetDefaultSliderValues()
        {
            slider_Exposure.MinValue= 10;     // 10μs
            slider_Exposure.MaxValue = 100000; // 100ms
            slider_Exposure.Value = 10000;    // 10ms

            slider_FrameRate.MinValue = 1;     // 1fps
            slider_FrameRate.MaxValue = 200;   // 200fps
            slider_FrameRate.Value = 30;      // 30fps

            slider_Gain.MinValue = 0;          // 0dB
            slider_Gain.MaxValue = 48;         // 48dB
            slider_Gain.Value = 0;            // 0dB

        }
        #endregion

        #region 数据采集界面加载事件(未写)
        private void CollectionControl_Load(object sender, EventArgs e)
        {
            if (_cameraDriver != null && _cameraDriver.IsConnect)
            {
                RefreshCameraParameters();
                LoadImageFormats();
            }
            else
            {
                // 如果相机未连接，设置滑块到默认值
                SetDefaultSliderValues();
                select_Resolution.Items.Clear();
            }

        }
        #endregion

        #region 触发模式选择事件
        private async void checkbox_TriggerType_CheckedChanged(object sender, BoolEventArgs e)
        {
            if (!e.Value) return; // 只处理选中情况

            var checkbox = sender as AntdUI.Checkbox;
            if (checkbox == null) return;

            // 确保触发模式已选中
            if (!radio_TriggerMode.Checked)
            {
                checkbox.Checked = false;
                return;
            }

            string triggerType = "";
            string triggerName = "";

            if (checkbox == checkbox_softTri && checkbox.Checked)
            {
                checkbox_hardTri.Checked = false;
                triggerType = "Software";
                triggerName = "软触发";
            }
            else if (checkbox == checkbox_hardTri && checkbox.Checked)
            {
                checkbox_softTri.Checked = false;
                triggerType = "Hardware";
                triggerName = "硬触发";
            }
            else
            {
                return;
            }

            bool success = _cameraDriver.SetMode(CameraWorkMode.Trigger, triggerType);
            if (success)
            {
                await LogEventHelper.PushLog(LogType.SystemOperation, $"切换到{triggerName}", LogStatu.Success);

            }
            else
            {
                await LogEventHelper.PushLog(LogType.SystemOperation, $"{triggerName}设置失败", LogStatu.Failure);
                AntdUI.Notification.error(this.FindForm(), "触发类型", $"{triggerName}设置失败");

                if (checkbox == checkbox_softTri)
                {
                    checkbox_softTri.Checked = false;
                    checkbox_hardTri.Checked = true;
                }
                else
                {
                    checkbox_hardTri.Checked = false;
                    checkbox_softTri.Checked = true;
                }
            }
        }
        #endregion

        #region 生成PCBSN(代替PCB板上没有二维码)
        public string GeneratePCBSN()
        {
            string pcbCode = CurrentSelectedPCB?.PCBCode ?? "PCB_Unknow";
            string PCBSN = $"{pcbCode}_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}";
            return PCBSN;
        }
        #endregion
        #region 直接创建和保存标注图像（修复missingRegions问题）

        /// <summary>
        /// 直接创建并保存标注图像（修复missingRegions问题）
        /// </summary>
        private HObject CreateAndSaveAnnotatedImageDirectly(HObject originalImage, HObject missingRegions, HTuple missingCount, string pcbsn, string savePath)
        {
            HObject annotatedImage = null;

            try
            {
                Console.WriteLine($"=== 开始创建标注图像（修复版）===");
                Console.WriteLine($"PCBSN: {pcbsn}");
                Console.WriteLine($"缺陷数量: {missingCount.I}");

                // 1. 复制原始图像
                HOperatorSet.CopyImage(originalImage, out annotatedImage);

                // 2. 检查图像信息
                HTuple width, height, channels;
                HOperatorSet.GetImageSize(annotatedImage, out width, out height);
                HOperatorSet.CountChannels(annotatedImage, out channels);
                Console.WriteLine($"图像信息: {width.I}x{height.I}, {channels.I}通道");

                // 3. 确保是三通道图像
                if (channels.I == 1)
                {
                    HObject rgbImage = null;
                    HOperatorSet.Compose3(annotatedImage, annotatedImage, annotatedImage, out rgbImage);
                    annotatedImage.Dispose();
                    annotatedImage = rgbImage;
                    Console.WriteLine("已将单通道图像转换为三通道");
                }

                // 4. 修复：在调用IsValidHObject之前先检查基本状态
                bool hasValidMissingRegions = false;
                HObject validMissingRegions = null;

                if (missingRegions != null && missingRegions.IsInitialized() && !missingRegions.IsEmpty() && missingCount.I > 0)
                {
                    try
                    {
                        // 创建一个临时的区域副本，避免原始区域被释放的问题
                        HOperatorSet.CopyObj(missingRegions, out validMissingRegions, 1, -1);
                        hasValidMissingRegions = true;
                        Console.WriteLine("已创建有效的缺陷区域副本");

                        // 获取区域信息用于调试
                        HOperatorSet.AreaCenter(validMissingRegions, out HTuple area, out HTuple row, out HTuple col);
                        Console.WriteLine($"缺陷区域信息 - 总面积: {area.TupleSum()}, 区域数量: {row.Length}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"创建缺陷区域副本失败: {ex.Message}");
                        hasValidMissingRegions = false;
                    }
                }

                // 5. 使用修复后的区域绘制缺陷
                if (hasValidMissingRegions && validMissingRegions != null)
                {
                    Console.WriteLine($"开始绘制 {missingCount.I} 个缺陷区域");

                    try
                    {
                        // 方法1: 直接绘制缺陷区域轮廓
                        HObject contours = null;
                        HOperatorSet.GenContourRegionXld(validMissingRegions, out contours, "border");

                        if (contours != null && contours.IsInitialized() && !contours.IsEmpty())
                        {
                            HOperatorSet.PaintXld(contours, annotatedImage, out annotatedImage,
                                new HTuple(255, 0, 0)); // 红色轮廓
                            contours.Dispose();
                            Console.WriteLine("已绘制缺陷区域红色轮廓");
                        }

                        // 方法2: 绘制外接矩形
                        HTuple row1, col1, row2, col2;
                        HOperatorSet.SmallestRectangle1(validMissingRegions, out row1, out col1, out row2, out col2);

                        HObject missingRect;
                        HOperatorSet.GenRectangle1(out missingRect, row1, col1, row2, col2);

                        if (missingRect.IsInitialized())
                        {
                            // 绘制黄色外接矩形边框
                            HOperatorSet.PaintRegion(missingRect, annotatedImage, out annotatedImage,
                                new HTuple(255, 255, 0), "margin");
                            missingRect.Dispose();
                            Console.WriteLine("已绘制黄色外接矩形");
                        }

                        Console.WriteLine($"缺陷坐标范围: [{row1.D},{col1.D}] - [{row2.D},{col2.D}]");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"绘制缺陷区域失败: {ex.Message}");
                    }
                    finally
                    {
                        // 释放临时区域副本
                        validMissingRegions?.Dispose();
                    }
                }
                else
                {
                    Console.WriteLine($"跳过缺陷绘制: 有效区域={hasValidMissingRegions}, 缺陷数量={missingCount.I}");
                }

                // 6. 添加文本标注信息
                AddTextAnnotations(annotatedImage, missingCount.I, pcbsn, width, height);

                // 7. 保存图像
                SaveImageWithVerification(annotatedImage, savePath);

                Console.WriteLine($"标注图像创建完成: {savePath}");
                return annotatedImage;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"标注图像创建失败: {ex.Message}");

                // 失败时保存原始图像
                try
                {
                    SaveImageWithVerification(originalImage, savePath);
                    Console.WriteLine($"已保存原始图像: {savePath}");
                }
                catch (Exception ex2)
                {
                    Console.WriteLine($"原始图像保存失败: {ex2.Message}");
                }

                return originalImage;
            }
        }

        /// <summary>
        /// 改进的HObject有效性检查（避免GetImageSize错误）
        /// </summary>
        private bool IsValidHObject(HObject obj)
        {
            try
            {
                if (obj == null)
                {
                    Console.WriteLine("HObject为null");
                    return false;
                }
                if (!obj.IsInitialized())
                {
                    Console.WriteLine("HObject未初始化");
                    return false;
                }
                if (obj.IsEmpty())
                {
                    Console.WriteLine("HObject为空");
                    return false;
                }

                // 对于区域对象，不要调用GetImageSize，直接返回true
                // 只有图像对象才需要检查尺寸
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"检查HObject有效性时发生异常: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 添加文本标注信息
        /// </summary>
        private void AddTextAnnotations(HObject image, int missingCount, string pcbsn, HTuple width, HTuple height)
        {
            try
            {
                // 定义文本配置列表
                var textConfigs = new List<Tuple<string, int>>
        {
            new Tuple<string, int>($"PCB SN: {pcbsn}", 30),
            new Tuple<string, int>($"检测时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}", 60),
            new Tuple<string, int>($"缺陷数量: {missingCount}", 90),
            new Tuple<string, int>($"检测结果: {(missingCount == 0 ? "合格" : "不合格")}", 120)
        };

                // 背景参数
                int xStart = 20;
                int bgWidth = 300;
                int bgHeight = 25;
                int textMargin = 5;

                // 为每个文本添加背景
                foreach (var config in textConfigs)
                {
                    string text = config.Item1;
                    int y = config.Item2;

                    HObject textBg = null;
                    HOperatorSet.GenRectangle1(out textBg, y - textMargin, xStart - textMargin, y + bgHeight, xStart + bgWidth);
                    HOperatorSet.PaintRegion(textBg, image, out image, new HTuple(0, 0, 0), "fill");
                    textBg?.Dispose();
                }

                // 添加状态指示器
                AddStatusIndicator(image, missingCount, width, height);

                Console.WriteLine("文本标注添加完成");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"文本标注添加失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 添加状态指示器
        /// </summary>
        private void AddStatusIndicator(HObject image, int missingCount, HTuple width, HTuple height)
        {
            try
            {
                // 在右上角添加状态指示器
                int indicatorSize = 30;
                int margin = 20;
                int centerX = width.I - margin - indicatorSize / 2;
                int centerY = margin + indicatorSize / 2;

                HObject indicator = null;
                HOperatorSet.GenCircle(out indicator, centerY, centerX, indicatorSize);

                if (missingCount == 0)
                {
                    // 合格 - 绿色
                    HOperatorSet.PaintRegion(indicator, image, out image, new HTuple(0, 255, 0), "fill");
                }
                else
                {
                    // 不合格 - 红色
                    HOperatorSet.PaintRegion(indicator, image, out image, new HTuple(255, 0, 0), "fill");
                }

                indicator?.Dispose();
                Console.WriteLine("状态指示器添加完成");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"状态指示器添加失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 保存图像并验证
        /// </summary>
        private void SaveImageWithVerification(HObject image, string savePath)
        {
            try
            {
                if (!IsValidHObject(image))
                    throw new Exception("图像无效");

                // 确保目录存在
                string directory = Path.GetDirectoryName(savePath);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                // 保存图像
                HOperatorSet.WriteImage(image, "png", 0, savePath);

                // 验证文件
                if (!File.Exists(savePath))
                    throw new Exception("文件未创建");

                FileInfo fileInfo = new FileInfo(savePath);
                if (fileInfo.Length == 0)
                    throw new Exception("文件为空");

                Console.WriteLine($"图像保存成功: {savePath} ({fileInfo.Length} 字节)");
            }
            catch (Exception ex)
            {
                throw new Exception($"图像保存失败: {ex.Message}");
            }
        }

        #endregion
       
        #endregion

        #region 保存检测的图片
        private string SaveTriggerImage(HObject hobj, string pcbsn = null)
        {
            if (hobj == null || hobj.IsEmpty())
                throw new ArgumentNullException(nameof(hobj));

            // 1. 生成文件名
            string sn = pcbsn ?? GeneratePCBSN();
            string fileName = $"{sn}_Trigger.png";          // 可加时间戳区分同一 SN 多次触发
            string fullPath = Path.Combine(_imageSaveDir, fileName);

            // 2. 确保目录存在
            if (!Directory.Exists(_imageSaveDir))
                Directory.CreateDirectory(_imageSaveDir);

            // 3. 直接 Halcon 存盘
            HOperatorSet.WriteImage(hobj, "png", 0, fullPath);

            return fullPath;
        }
        #endregion

        #region 将检测结果添加到数据库
        public async Task SaveDetectionResultToDatabase(DetectionResultEventArgs detectionResult, string workOrderCode, string pcbCode, string pcbsn, byte[] imageBytes)
        {
            try
            {
                string inspectionCode = $"INSP_{DateTime.Now:yyyyMMdd}_{pcbsn}";
                var inspection = new PCBInspection
                {
                    PCBSN = pcbsn,
                    WorkOrderCode = workOrderCode,
                    PCBCode = pcbCode,
                    CreateTime = DateTime.Now,
                    TotalDefectCount = detectionResult.MissingCount.I,
                    IsQualified = detectionResult.MissingCount.I == 0,
                    Image = imageBytes,
                    ProductLine = LoginConfig.ProductLine,
                    InspectorWorkId = LoginConfig.WorkId,
                    InspectionTime = DateTime.Now,
                    InspectionCode = inspectionCode
                };

                await _pcbInspectionBLL.InsertAsync(inspection).ConfigureAwait(false);
               
                var defectDetail = new PCBDefectDetail
                {
                    InspectionCode = inspectionCode,
                    DefectTypeCode = "PCBA0001missinghole",
                    DefectTypeName = "缺孔",
                    DefectCount = detectionResult.MissingCount.I,
                    DefectPosition = "PCB板区域",
                    Remark = $"PCBSN:{pcbsn} 检测缺陷"
                };
                await _defectDetailBLL.InsertAsync(defectDetail).ConfigureAwait(false);

                await LogEventHelper.PushLog(LogType.SystemOperation, $"检测结果保存成功PCBSN:{pcbsn}", LogStatu.Success).ConfigureAwait(false);
                InspectionSaved?.Invoke(this, new PCBInspectionSavedEventArgs { Inspection = inspection });
            }
            catch (Exception ex)
            {
                await LogEventHelper.PushLog(LogType.SystemOperation, $"检测结果保存失败PCBSN:{pcbsn} {ex.Message}", LogStatu.Failure).ConfigureAwait(false);
            }
        }
        #endregion

        #region 推送PCB检测结果到MES
        private async Task<bool> PushPcbInspectionResultToMesAsync(PcbInspectionPushDTO pushDto)
        {
            // 【模仿Scanner】第一步：参数校验（必填项不能为空，和Scanner参数校验逻辑一致）
            if (pushDto == null)
            {
                await LogEventHelper.PushLog(LogType.MESApiCall, "PCB检测推送MES：数据为空", LogStatu.Failure).ConfigureAwait(false);
                return false;
            }
            //string jsonStr = JsonConvert.SerializeObject(pushDto, Formatting.Indented);

            //Console.WriteLine(jsonStr);
            try
            {
                var request = new RestRequest("api/Scada/PushPCBInspection", Method.Post)
                    .AddHeader("Content-Type", "application/json")
                    .AddJsonBody(pushDto);

                // 【模仿Scanner】异步调用（直接执行，不额外包装）
                var response = await _restClient.ExecuteAsync(request).ConfigureAwait(false);

                // 【模仿Scanner】HTTP响应校验
                if (!response.IsSuccessful)
                {
                    var errorMsg = $"HTTP状态码：{response.StatusCode}，描述：{response.StatusDescription}";
                    await LogEventHelper.PushLog(LogType.MESApiCall, $"PCB检测推送MES HTTP失败（PCBSN：{pushDto.PCBSN}）：{errorMsg}", LogStatu.Failure).ConfigureAwait(false);
                    AntdUI.Notification.error(this.FindForm(), "MES推送失败", errorMsg);
                    return false;
                }

                // 【模仿Scanner】空响应校验
                if (string.IsNullOrEmpty(response.Content))
                {
                    await LogEventHelper.PushLog(LogType.MESApiCall, $"PCB检测推送MES：返回空响应（PCBSN：{pushDto.PCBSN}）", LogStatu.Failure).ConfigureAwait(false);
                    AntdUI.Notification.error(this.FindForm(), "MES推送失败", "返回空响应");
                    return false;
                }

                // 【模仿Scanner】反序列化+业务结果处理（合并精简）
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<string>>(response.Content);
                if (apiResponse == null)
                {
                    var errorMsg = $"响应反序列化失败（响应内容：{response.Content}）";
                    await LogEventHelper.PushLog(LogType.MESApiCall, $"PCB检测推送MES（PCBSN：{pushDto.PCBSN}）：{errorMsg}", LogStatu.Error).ConfigureAwait(false);
                    AntdUI.Notification.error(this.FindForm(), "MES推送失败", "响应格式错误");
                    return false;
                }

                if (apiResponse.Success)
                {
                    var resultDesc = pushDto.IsQualified ? "合格" : "不合格"; // 注意：新DTO字段是IsQualified，原InspectionResult已废弃
                    await LogEventHelper.PushLog(LogType.MESApiCall, $"PCB检测推送MES成功（PCBSN：{pushDto.PCBSN}，结果：{resultDesc}）", LogStatu.Success).ConfigureAwait(false);
                    AntdUI.Notification.success(this.FindForm(), "MES推送成功", "缺陷数据已同步至MES");
                    return true;
                }
                else
                {
                    var errorMsg = apiResponse.Message ??
                       (apiResponse.Errors?.Any() == true ? string.Join("；", apiResponse.Errors) : "未知业务错误");
                    await LogEventHelper.PushLog(LogType.MESApiCall, $"PCB检测推送MES业务失败（PCBSN：{pushDto.PCBSN}）：{errorMsg}", LogStatu.Error).ConfigureAwait(false);
                    AntdUI.Notification.error(this.FindForm(), "MES推送失败", errorMsg);
                    return false;
                }
            }


            catch (Exception ex)
            {
                var errorMsg = $"未知异常：{ex.Message}";
                await LogEventHelper.PushLog(LogType.MESApiCall, $"PCB检测推送MES未知异常（PCBSN：{pushDto.PCBSN}）：{errorMsg}，堆栈：{ex.StackTrace}", LogStatu.Error).ConfigureAwait(false);
                AntdUI.Notification.error(this.FindForm(), "MES推送异常", errorMsg);
            }

            return false;

        }

        #endregion

        #region 刷新图像格式列表
        public void RefreshImageFormats(bool forceRefresh = false)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<bool>(RefreshImageFormats), forceRefresh);
            }
            else
            {
                LoadImageFormats(forceRefresh);
            }
        }
        #endregion

        #region 图像格式选择事件
        private async void select_Resolution_SelectedValueChanged(object sender, ObjectNEventArgs e)
        {
            if (_cameraDriver == null || !_cameraDriver.IsConnect)
            {
                AntdUI.Notification.warn(this.FindForm(), "相机未连接", "请先连接相机");
                return;
            }

            if (select_Resolution.SelectedIndex < 0 || select_Resolution.SelectedIndex >= _imageFormats.Count)
            {
                Console.WriteLine("选择的索引无效");
                return;
            }

            string selectedFormat = _imageFormats[select_Resolution.SelectedIndex];
            Console.WriteLine($"选择格式: {selectedFormat}");

            // 检查是否正在采集
            if (_cameraDriver.isGrabbing)
            {
                var result = MessageBox.Show(this.FindForm(),
                  "切换图像格式需要停止采集，是否继续？",
                  "提示",
                  MessageBoxButtons.YesNo,
                  MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                {
                    // 恢复之前的选中项
                    string currentFormat = _cameraDriver.GetCurrentFormat();
                    for (int i = 0; i < _imageFormats.Count; i++)
                    {
                        if (_imageFormats[i].Equals(currentFormat, StringComparison.OrdinalIgnoreCase))
                        {
                            select_Resolution.SelectedIndex = i;
                            break;
                        }
                    }
                    return;
                }
            }

            try
            {
                Console.WriteLine($"开始设置格式: {selectedFormat}");

                bool success = await Task.Run(() => _cameraDriver.SetImageFormat(selectedFormat));

                if (success)
                {
                    Console.WriteLine("格式设置成功");
                    AntdUI.Notification.success(this.FindForm(), "设置成功",
                        $"图像格式已切换为 {selectedFormat}");

                    // 记录日志
                    await LogEventHelper.PushLog(LogType.SystemOperation,
                        $"图像格式切换: {selectedFormat}",
                        LogStatu.Success);
                }
                else
                {
                    Console.WriteLine("格式设置失败");
                    AntdUI.Notification.error(this.FindForm(), "设置失败",
                        "无法切换图像格式");

                    // 恢复之前的选中项
                    string currentFormat = _cameraDriver.GetCurrentFormat();
                    for (int i = 0; i < _imageFormats.Count; i++)
                    {
                        if (_imageFormats[i].Equals(currentFormat, StringComparison.OrdinalIgnoreCase))
                        {
                            select_Resolution.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"设置图像格式异常: {ex.Message}");
                AntdUI.Notification.error(this.FindForm(), "设置异常",
                    $"切换失败: {ex.Message}");
            }
        }
        #endregion

        #region 增益数值修改事件
        private void slider_Gain_ValueChanged(object sender, IntEventArgs e)
        {
            slider_Gain.Value = e.Value;
           
        }
        #endregion

        #region 帧率数值修改事件
        private  void slider_FrameRate_ValueChanged(object sender, IntEventArgs e)
        {
            slider_FrameRate.Value = e.Value;
           
        }
        #endregion

        #region 曝光时间修改事件
        private void slider_Exposure_ValueChanged(object sender, IntEventArgs e)
        {
            slider_Exposure.Value = e.Value;

        }
        #endregion

        #region 滑块松手增益触发事件
        private async void slider_Exposure_MouseUp(object sender, MouseEventArgs e)
        {
            if (_cameraDriver != null && _cameraDriver.IsConnect)
            {
                try
                {
                    float exposureTime = (float)slider_Exposure.Value;
                    bool success = _cameraDriver.SetExposure(exposureTime);

                    if (success)
                    {
                        await LogEventHelper.PushLog(LogType.SystemOperation, $"设置曝光成功:{exposureTime} μs", LogStatu.Success);
                    }
                    else
                    {
                        await LogEventHelper.PushLog(LogType.SystemOperation, $"设置曝光失败:{exposureTime} μs", LogStatu.Failure);
                    }

                }
                catch (Exception ex)
                {
                    await LogEventHelper.PushLog(LogType.SystemOperation, $"设置曝光:{ex.Message} ", LogStatu.Error);

                }

            }
        }
        #endregion

        #region 滑块松手帧率触发事件
        private async void slider_FrameRate_MouseUp(object sender, MouseEventArgs e)
        {
            if (_cameraDriver != null && _cameraDriver.IsConnect)
            {
                try
                {
                    float frameRateValue = (float)slider_FrameRate.Value;
                    bool success = _cameraDriver.SetFrameRate(frameRateValue);

                    if (success)
                    {
                        await LogEventHelper.PushLog(LogType.SystemOperation, $"设置帧率成功:{frameRateValue} fps", LogStatu.Success);
                    }
                    else
                    {
                        await LogEventHelper.PushLog(LogType.SystemOperation, $"设置帧率失败:{frameRateValue} fps", LogStatu.Failure);
                    }

                }
                catch (Exception ex)
                {
                    await LogEventHelper.PushLog(LogType.SystemOperation, $"设置帧率:{ex.Message} ", LogStatu.Error);

                }

            }
        }
        #endregion

        #region 滑块松手增益触发事件
        private async void slider_Gain_MouseUp(object sender, MouseEventArgs e)
        {
            if (_cameraDriver != null && _cameraDriver.IsConnect)
            {
                try
                {
                    float gainValue = (float)slider_Gain.Value;
                    bool success = _cameraDriver.SetGain(gainValue);

                    if (success)
                    {
                        await LogEventHelper.PushLog(LogType.SystemOperation, $"设置增益成功:{gainValue} dB", LogStatu.Success);
                    }
                    else
                    {
                        await LogEventHelper.PushLog(LogType.SystemOperation, $"设置增益失败:{gainValue} dB", LogStatu.Failure);
                    }

                }
                catch (Exception ex)
                {
                    await LogEventHelper.PushLog(LogType.SystemOperation, $"设置增益:{ex.Message} ", LogStatu.Error);

                }

            }
        }
        #endregion

        #region 位图格式转换方法
        /// <summary>
        /// 将任意格式的Bitmap转换为24位RGB格式
        /// </summary>
        private Bitmap ConvertTo24bppRgb(Bitmap source)
        {
            // 如果已经是24位RGB格式，直接返回
            if (source.PixelFormat == PixelFormat.Format24bppRgb)
                return new Bitmap(source);

            // 创建新的24位RGB位图
            Bitmap target = new Bitmap(source.Width, source.Height, PixelFormat.Format24bppRgb);

            using (Graphics g = Graphics.FromImage(target))
            {
                // 设置高质量绘制参数
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                // 绘制图像（这将去除Alpha通道）
                g.DrawImage(source, 0, 0, source.Width, source.Height);
            }

            return target;
        }
        #endregion
    }

}
