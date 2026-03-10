using AntdUI;
using HZH_Controls;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Wedjat.BLL;
using Wedjat.Driver;
using Wedjat.Helper;
using Wedjat.Model;
using Wedjat.Model.Config;
using Wedjat.Model.DTO;
using Wedjat.Model.Entity;
using static Wedjat.Model.DTO.WorkOrderResponse;

namespace Wedjat.WinForm.MainMenuUI
{

    public partial class Scanner : UserControl
    {
       
        public event EventHandler<DeviceSwitchStatusChangedEventArgs> SwitchStatusChanged;
        public event EventHandler<WorkOrderReceivedEventArgs> WorkOrderReceived;
        public event EventHandler<WorkOrderResponse> WorkOrderSyncRequested;
        public event EventHandler<MesWorkOrderPCB> PCBSyncRequested;
       
        public WorkOrderResponse CurrentWorkOrder;
        public WorkOrder workOrder;
        public MesWorkOrderPCB CurrentSelectedPCB;
        private BindingList<ScannerData> _scans = new BindingList<ScannerData>();
        private readonly RestClient _restClient;
        private ScannerDataBLL _scannerDatabll;
        private readonly WorkOrderBLL _workOrderBll;
        private readonly PCBBLL _pcbBll;
        private readonly PCBDefectTypeBLL _defectTypeBll;
        private readonly WorkOrderPCBBLL _workOrderPcbBll;
        NModbusDriver _modbusDriver;
        ScannerInfo scannerInfo = new ScannerInfo();
        IniService iniService = new IniService();
        private List<MesWorkOrderPCB> _currentPCB = new List<MesWorkOrderPCB>();
        private string scannerPath = Application.StartupPath + "\\scanner.ini";
        public Scanner()
        {
            InitializeComponent();
            _modbusDriver = new NModbusDriver();
            _scannerDatabll = new ScannerDataBLL();
            _workOrderBll = new WorkOrderBLL();
            _pcbBll = new PCBBLL();
            _defectTypeBll = new PCBDefectTypeBLL();
            _workOrderPcbBll = new WorkOrderPCBBLL();
            InitTable();
            scannerPage();
            workOrderPage();
            LoadIniConfigToScannerInfo();
            FillConfigToControls();
            _restClient = new RestClient("http://localhost:5059/");
            label_productLine.Text = LoginConfig.ProductLine;
            label_oper.Text = LoginConfig.Name;
            select_Serialport.Items.AddRange(SerialPort.GetPortNames());
            select_Serialport.SelectedValue = select_Serialport.Items[0].ToString().Trim();
            select_BaudRate.Items.AddRange(new string[] { "9600", "19200", "38400", "57600", "115200", "230400" });
            select_BaudRate.SelectedValue = select_BaudRate.Items[0].ToString().Trim();
            select_DataBits.Items.AddRange(new string[] { "7", "8" });
            select_DataBits.SelectedValue = select_DataBits.Items[1].ToString().Trim();
            select_StopBits.Items.AddRange(Enum.GetNames(typeof(StopBits)));
            select_StopBits.SelectedValue = select_StopBits.Items[1].ToString().Trim();
            select_Parity.Items.AddRange(Enum.GetNames(typeof(Parity)));
            select_Parity.SelectedValue = select_Parity.Items[0].ToString().Trim();
        }

        #region 更新工单信息显示
        public void UpdateWorkOrderDisplay(WorkOrderResponse workOrder)
        {
            if (workOrder == null) return;

            CurrentWorkOrder = workOrder;
            _currentPCB = workOrder.WorkOrderPCBs ?? new List<MesWorkOrderPCB>();

            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<WorkOrderResponse>(UpdateWorkOrderControls), workOrder);
            }
            else
            {
                UpdateWorkOrderControls(workOrder);
            }
        }
        #endregion

        #region 更新选中PCB信息显示
        public void UpdateSelectedPCB(MesWorkOrderPCB pcb)
        {
            if (pcb == null) return;

            CurrentSelectedPCB = pcb;

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


        #region 更新选中PCB信息显示核心方法
        private void UpdateSelectedPCBCore(MesWorkOrderPCB pcb)
        {
            // 更新PCB选择下拉框
            if (pcb != null && !string.IsNullOrEmpty(pcb.mesPCBs?.PCBName))
            {
                string pcbName = pcb.mesPCBs.PCBName;

                // 查找并选中对应的PCB
                for (int i = 0; i < select_PCBType.Items.Count; i++)
                {
                    if (select_PCBType.Items[i].ToString() == pcbName)
                    {
                        select_PCBType.SelectedIndex = i;
                        break;
                    }
                }

                // 更新完成数量显示
                if (pcb != null)
                {
                    label_plannedQuantity.Text = pcb.PlanQuantity.ToString();
                    label_completedQuantity.Text = pcb.CompleteQuantity.ToString();
                }
            }
        }
        #endregion

        #region 更新工单信息模块的方法
        private void UpdateWorkOrderControls(WorkOrderResponse workOrder)
        {
            if (workOrder == null) return;

            label_workOrderNo.Text = workOrder.WorkOrderCode;
            _currentPCB = workOrder.WorkOrderPCBs ?? new List<MesWorkOrderPCB>();
            var pcbNames = _currentPCB
                .Select(wop => wop.mesPCBs.PCBName)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .ToArray();

            select_PCBType.Items.Clear();
            select_PCBType.Items.AddRange(pcbNames);

            // 如果有选中的PCB，尝试保持选中状态
            if (CurrentSelectedPCB != null && pcbNames.Length > 0)
            {
                var currentPCBName = CurrentSelectedPCB.mesPCBs?.PCBName;
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
                    CurrentSelectedPCB = _currentPCB.FirstOrDefault(wop => wop.mesPCBs.PCBName == pcbNames[0]);
                }
            }
            else if (pcbNames.Length > 0)
            {
                select_PCBType.SelectedIndex = 0;
                CurrentSelectedPCB = _currentPCB.FirstOrDefault(wop => wop.mesPCBs.PCBName == pcbNames[0]);
            }

            // 更新工单状态显示
            if (workOrder != null)
            {
                switch (workOrder.OrderStatus)
                {
                    case OrderStatu.Pending:
                        label_workOrderStatu.Text = "待处理";
                        label_workOrderStatu.BackColor = Color.Orange;
                        break;
                    case OrderStatu.Processing:
                        label_workOrderStatu.Text = "处理中";
                        label_workOrderStatu.BackColor = Color.Blue;
                        break;
                    case OrderStatu.Paused:
                        label_workOrderStatu.Text = "已暂停";
                        label_workOrderStatu.BackColor = Color.Gray;
                        break;
                    case OrderStatu.Completed:
                        label_workOrderStatu.Text = "已完成";
                        label_workOrderStatu.BackColor = Color.Green;
                        break;
                    case OrderStatu.Canceled:
                        label_workOrderStatu.Text = "已取消";
                        label_workOrderStatu.BackColor = Color.Red;
                        break;
                    default:
                        label_workOrderStatu.Text = "未知状态";
                        label_workOrderStatu.BackColor = Color.LightGray;
                        break;
                }
            }
        }
        #endregion

        #region 扫码枪触发工单查询
        /// <summary>
        /// 从MES中获取工单
        /// </summary>
        /// <returns></returns>
        public async Task GetWorkOrderFromMesAsync()
        {
            string mesAPI = this.input_mesAPI.Text;
            string scannerCode = this.input_QRCode.Text;
            ScannerDataDTO dto = new ScannerDataDTO();
            if (string.IsNullOrEmpty(mesAPI))
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        AntdUI.Modal.open("提示", "mesAPI不能为空！", AntdUI.TType.Error);
                    }));
                }
                else
                {
                    AntdUI.Modal.open("提示", "mesAPI不能为空！", AntdUI.TType.Error);
                }
                return;
            }

            try
            {
                var request = new RestRequest(mesAPI, Method.Get);
                request.AddQueryParameter("scannerCode", scannerCode);
                var response = await Task.Run(() => _restClient.ExecuteAsync(request));
                ClearWorkOrder();
                if (mesAPI != scannerInfo.MesAPI)
                {
                  await  LogEventHelper.PushLog(LogType.MESApiCall, "MesAPI地址错误", LogStatu.Failure).ConfigureAwait(false);
                    AntdUI.Modal.open("错误", "MesAPI地址错误", AntdUI.TType.Error);
                    lbl_analysisResult.Text = ScanStatus.MESApiException.ToString();
                    return;
                }
                if (!response.IsSuccessful)
                {
                  await  LogEventHelper.PushLog(LogType.MESApiCall, $"HTTP 错误：{response.StatusDescription}", LogStatu.Failure).ConfigureAwait(false);
                    AntdUI.Modal.open("错误", $"HTTP 错误：{response.StatusDescription}", AntdUI.TType.Error);
                    lbl_analysisResult.Text = ScanStatus.WorkOrderNotFound.ToString();
                    return;
                }

                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<WorkOrderResponse>>(response.Content);

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    CurrentWorkOrder = apiResponse.Data;
                    WorkOrderSyncRequested?.Invoke(this, CurrentWorkOrder);
                    label_workOrderNo.Text = apiResponse.Data.WorkOrderCode;
                    if (apiResponse.Data.StartTime.HasValue)
                    {
                        label_startTime.Text = apiResponse.Data.StartTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
                       
                    }
                    else
                    {
                        label_startTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        label_startTime.Text = "未开始";
                    }

                    // 【修复】显示结束时间
                    if (apiResponse.Data.EndTime.HasValue)
                    {
                        label_endTime.Text = apiResponse.Data.EndTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    else
                    {
                        label_endTime.Text = "未结束";
                    }
                    var workOrderPCBs = apiResponse.Data.WorkOrderPCBs ?? new List<MesWorkOrderPCB>();
                    string[] pcbNames = workOrderPCBs
                        .Select(wop => wop.mesPCBs.PCBName)
                        .Where(name => !string.IsNullOrWhiteSpace(name))
                        .ToArray();

                    select_PCBType.Items.Clear();
                    select_PCBType.Items.AddRange(pcbNames);
                    _currentPCB = workOrderPCBs;
                   
                    lbl_analysisResult.Text = ScanStatus.Success.ToString();
                    
                    switch (apiResponse.Data.OrderStatus)
                    {
                        case OrderStatu.Pending:
                            label_workOrderStatu.Text = "待处理";
                            label_workOrderStatu.BackColor = Color.Orange;
                            break;
                        case OrderStatu.Processing:
                            label_workOrderStatu.Text = "处理中";
                             label_workOrderStatu.BackColor = Color.Blue;
                            break;
                        case OrderStatu.Paused:
                            label_workOrderStatu.Text = "已暂停";
                             label_workOrderStatu.BackColor = Color.Gray;
                            break;
                        case OrderStatu.Completed:
                            label_workOrderStatu.Text = "已完成";
                             label_workOrderStatu.BackColor = Color.Green;
                            break;
                        case OrderStatu.Canceled:
                            label_workOrderStatu.Text = "已取消";
                             label_workOrderStatu.BackColor = Color.Red;
                            break;
                        default:
                            label_workOrderStatu.Text = "未知状态";
                             label_workOrderStatu.BackColor = Color.LightGray;
                            break;
                    }
                    if (pcbNames.Length > 0)
                        select_PCBType.SelectedIndex = 0;
                    WorkOrderSyncRequested?.Invoke(this, CurrentWorkOrder);
                    //WorkOrderReceived?.Invoke(this, new WorkOrderReceivedEventArgs
                    //{
                    //    WorkOrder = apiResponse.Data,
                    //    CurrentSelectedPCB = CurrentSelectedPCB
                    //});
                    try
                    {
                        await SaveWorkOrderToDbAsync(apiResponse.Data);
                        AntdUI.Notification.success(this.FindForm(), "数据保存成功", "工单及关联数据已同步到数据库");
                    }
                    catch (Exception dbEx)
                    {
                        AntdUI.Notification.error(this.FindForm(), "数据保存失败", dbEx.Message);
                    }
                    await LogEventHelper.PushLog(LogType.MESApiCall, $"查询成功：{label_workOrderNo.Text}", LogStatu.Success).ConfigureAwait(false);
                }
                else
                {
                    string err = apiResponse?.Message
                              ?? (apiResponse?.Errors?.Any() == true ? string.Join("\n", apiResponse.Errors) : "未知错误");
                  await  LogEventHelper.PushLog(LogType.MESApiCall, err, LogStatu.Error).ConfigureAwait(false);
                    AntdUI.Modal.open("查询失败", err, AntdUI.TType.Error);

                    lbl_analysisResult.Text = ScanStatus.WorkOrderNotFound.ToString();
                }
            }

            catch (Exception ex)
            {
                AntdUI.Modal.open("异常", ex.Message, AntdUI.TType.Error);
                lbl_analysisResult.Text = ScanStatus.Failed.ToString();
                _currentPCB = new List<MesWorkOrderPCB>();
            }
            finally
            {
                gridPanel_workOrder.Show();
                await InsertLogRow(dto);
            }
        }
        #endregion

        #region 保存查询的的工单相关信息到数据库
        private async Task SaveWorkOrderToDbAsync(WorkOrderResponse mesWorkOrder)
        {
            if (mesWorkOrder == null || string.IsNullOrEmpty(mesWorkOrder.WorkOrderCode) || mesWorkOrder.WorkOrderPCBs == null)
                return;


          
            try
                {
               
                await Task.Run(async () =>
                {
                    #region 1. 处理工单主表（WorkOrder）
                    var existingWorkOrder = await _workOrderBll.GetModelAsync(w => w.WorkOrderCode == mesWorkOrder.WorkOrderCode);
                    

                    // 计算总计划数和总完成数（汇总所有PCB的数量）
                    int totalPlanQty = mesWorkOrder.WorkOrderPCBs.Sum(wop => wop.PlanQuantity);
                    int totalCompleteQty = mesWorkOrder.WorkOrderPCBs.Sum(wop => wop.CompleteQuantity ?? 0);

                    if (existingWorkOrder == null)
                    {
                        // 不存在：新增工单
                        workOrder = new WorkOrder
                        {
                            WorkOrderCode = mesWorkOrder.WorkOrderCode,
                            PlanQuantity = totalPlanQty,
                            CompleteQuantity = totalCompleteQty,
                            OrderStatus = mesWorkOrder.OrderStatus,
                            CreateTime = DateTime.Now,
                            UpdateTime = DateTime.Now,
                            StartTime = mesWorkOrder.StartTime,
                            EndTime = mesWorkOrder.EndTime
                        };
                        await _workOrderBll.InsertAsync(workOrder);
                        await LogEventHelper.PushLog(LogType.DatabaseOperation, $"新增工单：{workOrder.WorkOrderCode}", LogStatu.Success).ConfigureAwait(false);
                    }
                    else
                    {
                        // 已存在：更新状态和数量
                        existingWorkOrder.OrderStatus = mesWorkOrder.OrderStatus;
                        existingWorkOrder.PlanQuantity = totalPlanQty;
                        existingWorkOrder.CompleteQuantity = totalCompleteQty;
                        existingWorkOrder.UpdateTime = DateTime.Now;
                        await _workOrderBll.UpdateAsync(existingWorkOrder);
                        await LogEventHelper.PushLog(LogType.DatabaseOperation, $"更新工单：{existingWorkOrder.WorkOrderCode}", LogStatu.Success).ConfigureAwait(false);
                        workOrder = existingWorkOrder;
                    }
                    #endregion
                });

                    #region 2. 处理PCB表+缺陷类型表+工单-PCB关联表
                    foreach (var mesWorkOrderPcb in mesWorkOrder.WorkOrderPCBs)
                    {
                        var mesPcb = mesWorkOrderPcb.mesPCBs;
                        if (mesPcb == null || string.IsNullOrEmpty(mesPcb.PCBCode))
                            continue;

                        #region 2.1 处理PCB表
                        var existingPcb = await _pcbBll.GetModelAsync(p => p.PCBCode == mesPcb.PCBCode);
                        PCB pcb;

                        if (existingPcb == null)
                        {
                            // 不存在：新增PCB
                            pcb = new PCB
                            {
                                PCBCode = mesPcb.PCBCode,
                                PCBName = string.IsNullOrWhiteSpace(mesPcb.PCBName) ? mesPcb.PCBCode : mesPcb.PCBName,
                                CreateTime = DateTime.Now,
                                UpdateTime = DateTime.Now
                            };
                            await _pcbBll.InsertAsync(pcb);
                            await LogEventHelper.PushLog(LogType.DatabaseOperation, $"新增PCB：{pcb.PCBCode}", LogStatu.Success).ConfigureAwait(false);
                        }
                        else
                        {
                            // 已存在：更新名称（如有变更）
                            if (!string.IsNullOrWhiteSpace(mesPcb.PCBName) && existingPcb.PCBName != mesPcb.PCBName)
                            {
                                existingPcb.PCBName = mesPcb.PCBName;
                                existingPcb.UpdateTime = DateTime.Now;
                                await _pcbBll.UpdateAsync(existingPcb);
                                await LogEventHelper.PushLog(LogType.DatabaseOperation, $"更新PCB：{existingPcb.PCBCode}", LogStatu.Success).ConfigureAwait(false);
                            }
                            pcb = existingPcb;
                        }
                        #endregion

                        #region 2.2 处理PCB缺陷类型表
                        if (mesPcb.mesPCBDefects != null && mesPcb.mesPCBDefects.Any())
                        {
                            foreach (var mesDefect in mesPcb.mesPCBDefects)
                            {
                                if (string.IsNullOrEmpty(mesDefect.DefectCode))
                                    continue;

                                // 校验：同一PCB下是否已存在该缺陷类型
                                var existingDefect = await _defectTypeBll.GetModelAsync(d =>
                                    d.DefectTypeCode == mesDefect.DefectCode && d.PCBCode == pcb.PCBCode);

                                if (existingDefect == null)
                                {
                                    // 不存在：新增缺陷类型
                                    var defectType = new PCBDefectType
                                    {
                                        DefectTypeCode = mesDefect.DefectCode,
                                        PCBCode = pcb.PCBCode,
                                        DefectName = string.IsNullOrWhiteSpace(mesDefect.DefectName) ? mesDefect.DefectCode : mesDefect.DefectName,
                                        Description = string.Empty, // MES无描述字段，留空
                                        CreateTime = DateTime.Now,
                                        UpdateTime = DateTime.Now
                                    };
                                    await _defectTypeBll.InsertAsync(defectType);
                                    await LogEventHelper.PushLog(LogType.DatabaseOperation, $"新增缺陷类型：{defectType.DefectTypeCode}（PCB：{pcb.PCBCode}）", LogStatu.Success).ConfigureAwait(false);
                                }
                                else
                                {
                                    // 已存在：更新名称（如有变更）
                                    if (!string.IsNullOrWhiteSpace(mesDefect.DefectName) && existingDefect.DefectName != mesDefect.DefectName)
                                    {
                                        existingDefect.DefectName = mesDefect.DefectName;
                                        existingDefect.UpdateTime = DateTime.Now;
                                        await _defectTypeBll.UpdateAsync(existingDefect);
                                        await LogEventHelper.PushLog(LogType.DatabaseOperation, $"更新缺陷类型：{existingDefect.DefectTypeCode}", LogStatu.Success).ConfigureAwait(false);
                                    }
                                }
                            }
                        }
                        #endregion

                        #region 2.3 处理工单-PCB关联表（多对多）
                        var existingRelation = await _workOrderPcbBll.GetModelAsync(wp =>
                            wp.WorkOrderCode == workOrder.WorkOrderCode && wp.PCBCode == pcb.PCBCode);

                        if (existingRelation == null)
                        {
                            // 不存在：新增关联记录
                            var workOrderPcb = new WorkOrderPCB
                            {
                                WorkOrderCode = workOrder.WorkOrderCode,
                                PCBCode = pcb.PCBCode,
                                CreateTime = DateTime.Now,
                                UpdateTime = DateTime.Now
                            };
                            await _workOrderPcbBll.InsertAsync(workOrderPcb);
                            await LogEventHelper.PushLog(LogType.DatabaseOperation, $"新增工单-PCB关联：{workOrder.WorkOrderCode}-{pcb.PCBCode}", LogStatu.Success).ConfigureAwait(false);
                        }
                        #endregion
                    }
                    #endregion
                
                   
                }
                catch (Exception ex)
                {
                   
                    await LogEventHelper.PushLog(LogType.DatabaseOperation, $"工单数据入库失败：{ex.Message}", LogStatu.Failure).ConfigureAwait(false);
                    throw new Exception("数据库保存失败：" + ex.Message);
                }
            
        }
        #endregion

        #region 点击按钮查询工单信息
        /// <summary>
        /// 点击按钮查询工单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       public async void btn_FindWorkOrder_Click(object sender, EventArgs e)
        {
            
            await GetWorkOrderFromMesAsync();
        }
        #endregion

        #region 二维码内容变更时进行查询工单(相当于自动查询)
        /// <summary>
        /// 二维码内容变更时进行查询工单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void input_QRCode_TextChanged(object sender, EventArgs e)
        {
            if (checkbox_autoFindWorkOrder.Checked && !string.IsNullOrWhiteSpace(input_QRCode.Text.Trim()))
            {
                btn_FindWorkOrder.PerformClick();
            }
        }
        #endregion

        #region 清空工单信息
        /// <summary>
        /// 清空工单信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearWorkOrder()
        {
           
            _currentPCB.Clear();
            label_workOrderNo.Text = "NULL";
            select_PCBType.Items.Clear();
            label_plannedQuantity.Text = "0";
            label_completedQuantity.Text = "0";
            label_workOrderStatu.Text = "NULL";
        }
        #endregion

     

        #region 选中不同的PCB会更新工单状态（计划数量完成数量）
        private void select_PCBType_SelectedIndexChanged(object sender, IntEventArgs e)
        {
            string selectedPCBName = select_PCBType.Text.ToString();
            if (string.IsNullOrWhiteSpace(selectedPCBName) || _currentPCB == null)
            {
                label_plannedQuantity.Text = "0";
                label_completedQuantity.Text = "0";
                CurrentSelectedPCB = null;
                return;
            }

            CurrentSelectedPCB = _currentPCB
             .FirstOrDefault(wop => wop.mesPCBs.PCBName == selectedPCBName);

            if (CurrentSelectedPCB != null)
            {
                label_plannedQuantity.Text = CurrentSelectedPCB.PlanQuantity.ToString();
                label_completedQuantity.Text = CurrentSelectedPCB.CompleteQuantity.ToString();
             //   string pcbsn = GeneratePCBSN(CurrentSelectedPCB);
               // string pcbcode = CurrentSelectedPCB.mesPCBs.PCBName;
                PCBSyncRequested?.Invoke(this, CurrentSelectedPCB);
                //WorkOrderReceived?.Invoke(this, new WorkOrderReceivedEventArgs
                //{
                //    WorkOrder = new WorkOrderResponse
                //    {
                //        WorkOrderCode = label_workOrderNo.Text,
                //        OrderStatus = GetOrderStatusFromLabel(),
                //        WorkOrderPCBs = _currentPCB
                //    },
                //    CurrentSelectedPCB = CurrentSelectedPCB

                //});
            }
            else
            {
                label_plannedQuantity.Text = "0";
                label_completedQuantity.Text = "0";
            }
        }
        #endregion

        #region 从工单状态中获取OrderStatus枚举值
        private OrderStatu GetOrderStatusFromLabel()
        {
            string statusText = label_workOrderStatu.Text;
            switch (statusText)
            {
                case "待处理": return OrderStatu.Pending;
                case "处理中": return OrderStatu.Processing;
                case "已暂停": return OrderStatu.Paused;
                case "已完成": return OrderStatu.Completed;
                case "已取消": return OrderStatu.Canceled;
                default: return OrderStatu.Pending;
            }
        }
        #endregion

        #region 获取当前选中的PCB信息
        public MesWorkOrderPCB GetCurrentSelectedPCB()
        {
            string selectedPCBName = select_PCBType.Text.ToString();
            if (string.IsNullOrWhiteSpace(selectedPCBName) || _currentPCB == null)
                return null;

            return _currentPCB.FirstOrDefault(wop => wop.mesPCBs.PCBName == selectedPCBName);
        }
        #endregion

        #region 获取当前工单编号
        public string GetCurrentWorkOrderCode()
        {
            return label_workOrderNo.Text;
        }
        #endregion

        #region 初始化表格信息
        public void InitTable()
        {
            table_DataLog.Columns = new ColumnCollection() {
                new Column("ScanTime", "扫码时间", ColumnAlign.Center),
                new Column("CodeContent", "解析内容",ColumnAlign.Center),
                new Column("WorkOrderNo", "返回工单号",ColumnAlign.Center),
                new Column("Statu", "状态", ColumnAlign.Center)
                {
                      Render = (value, record, rowIndex) =>
                        {

                            var info = record as ScannerData;
                             ScanStatus status = info.Statu;
                            string text;
                             AntdUI.TTypeMini type;
                            switch (status)
                            {
                                case ScanStatus.Success:
                                    text = "查询成功";
                                    type=AntdUI.TTypeMini.Success;
                                    break;
                                case ScanStatus.MESApiException:
                                    text = "Mes接口异常";
                                    type=AntdUI.TTypeMini.Error;
                                    break;
                                case ScanStatus.Failed:
                                    text = "查询失败";
                                    type=AntdUI.TTypeMini.Default;
                                    break;
                               case ScanStatus.WorkOrderNotFound:
                                    text = "工单不存在";
                                    type=AntdUI.TTypeMini.Warn;
                                    break;
                                default:
                                    text = "未知";
                                    type=AntdUI.TTypeMini.Info;
                                    break;
                            }
                            return new AntdUI.CellTag(text,type);
                        }
                },
                new Column("OperatorWorkId", "操作人",ColumnAlign.Center)

            };

        }
        #endregion

        #region 从Ini配置信息中读取扫码枪信息
        private async void LoadIniConfigToScannerInfo()
        {
            if (!System.IO.File.Exists(scannerPath))
            { 
                AntdUI.Notification.info(this.FindForm(), "串口扫码枪", "未找到扫码枪配置文件，将使用默认配置");
                return;
            }

            try
            {
                scannerInfo = iniService.GetScannerInfoFromPath(scannerPath);
              await  LogEventHelper.PushLog(LogType.ConfigurationChange, "扫码枪配置读取成功！", LogStatu.Success).ConfigureAwait(false);
                AntdUI.Notification.success(this.FindForm(), "串口扫码枪", "扫码枪配置读取成功");
            }
            catch (Exception ex)
            {
             await   LogEventHelper.PushLog(LogType.ConfigurationChange, "扫码枪配置读取失败！", LogStatu.Failure).ConfigureAwait(false);
                AntdUI.Notification.error(this.FindForm(), "串口扫码枪", $"配置读取失败：{ex.Message}");
                // 读取失败时使用默认配置
                scannerInfo = new ScannerInfo()
                {
                    BaudRate = 9600,
                    DataBits = 8,
                    StopBits = StopBits.One,
                    Parity = Parity.None,
                    OverTime = 3000
                };
            }
        }
        #endregion

        #region 将扫码枪相关默认值显示再控件上
        private void FillConfigToControls()
        {
            if (!string.IsNullOrEmpty(scannerInfo.PortName) && select_Serialport.Items.Contains(scannerInfo.PortName))
            {
                select_Serialport.Text = scannerInfo.PortName;
            }
            else if (select_Serialport.Items.Count > 0)
            {
                select_Serialport.SelectedValue = select_Serialport.Items[0];
            }

            string baudStr = scannerInfo.BaudRate.ToString();
            if (select_BaudRate.Items.Contains(baudStr))
            {
                select_BaudRate.Text = baudStr;
            }
            else
            {
                select_BaudRate.Text = "9600"; 
            }

          
            string dataBitsStr = scannerInfo.DataBits.ToString();
            if (select_DataBits.Items.Contains(dataBitsStr))
            {
                select_DataBits.Text = dataBitsStr;
            }
            else
            {
                select_DataBits.Text = "8"; 
            }

            string stopBitsStr = scannerInfo.StopBits.ToString();
            if (select_StopBits.Items.Contains(stopBitsStr))
            {
                select_StopBits.Text = stopBitsStr;
            }
            else
            {
                select_StopBits.Text = "One"; 
            }

            string parityStr = scannerInfo.Parity.ToString();
            if (select_Parity.Items.Contains(parityStr))
            {
                select_Parity.Text = parityStr;
            }
            else
            {
                select_Parity.Text = "None"; 
            }

            input_mesAPI.Text = scannerInfo.MesAPI ?? "";
            inputNumber_overTime.Text = scannerInfo.OverTime.ToString();
        }
        #endregion

        #region 扫码前的样式
        public void scannerPage()
        {
            gridPanel1.Hide();
            AntdUI.Label panel = new AntdUI.Label();
            panel.Text = "请连接扫码枪设备以开始扫描";
            panel.Dock = DockStyle.Fill;
            panel.IconRatio = 2;
            panel.PrefixSvg = IconSvg.scanner_Device;
            panel.TextAlign = ContentAlignment.MiddleCenter;
            panel_scannerDataBorder.Controls.Add(panel);
        }
        public void workOrderPage()
        {
            gridPanel_workOrder.Hide();
            AntdUI.Label panel1 = new AntdUI.Label();
            panel1.Text = "请扫描二维码并查询工单信息";
            panel1.Dock = DockStyle.Fill;
            panel1.IconRatio = 2;
            panel1.PrefixSvg = IconSvg.scanner_WorkOrder;
            panel1.TextAlign = ContentAlignment.MiddleCenter;
            panel_ScannerWorkOrder.Controls.Add(panel1);
        }
        #endregion

        #region 扫码枪开关
        private async void btn_switchRTU_Click(object sender, EventArgs e)
        {


            if (btn_switchRTU.IconSvg == IconSvg.switch_IconSvg_Success || btn_switchRTU.IconSvg == IconSvg.switch_IconSvg_Fail)
            {
                _modbusDriver.DisConnectRTU();
                btn_switchRTU.IconSvg = IconSvg.switch_IconSvg_Default;
              await  LogEventHelper.PushLog(LogType.SystemStartupShutdown, "串口扫码枪断开连接！", LogStatu.System).ConfigureAwait(false);
                AntdUI.Notification.success(this.FindForm(), "串口扫码枪", "已断开RTU连接!");
                scannerPage();
                SwitchStatusChanged?.Invoke(this, new DeviceSwitchStatusChangedEventArgs(DeviceSwitchType.Scanner_Switch, false));
                return;
            }

            scannerInfo.PortName = select_Serialport.Text;
            scannerInfo.BaudRate = int.Parse(select_BaudRate.Text);
            scannerInfo.DataBits = int.Parse(select_DataBits.Text);
            scannerInfo.StopBits = (StopBits)Enum.Parse(typeof(StopBits), select_StopBits.Text.Trim(), true);
            scannerInfo.Parity = (Parity)Enum.Parse(typeof(Parity), select_Parity.Text.Trim(), true);

            bool statu = _modbusDriver.InitializeRTU(
                scannerInfo.PortName,
                scannerInfo.BaudRate,
                scannerInfo.Parity,
                scannerInfo.DataBits,
                scannerInfo.StopBits
            );

            if (statu)
            {
                btn_switchRTU.IconSvg = IconSvg.switch_IconSvg_Success;
              await  LogEventHelper.PushLog(LogType.SystemStartupShutdown, "串口扫码枪连接成功！",LogStatu.Success).ConfigureAwait(false);
                AntdUI.Notification.success(this.FindForm(), "串口扫码枪", "串口扫码枪连接成功！");
                _modbusDriver.CodeReceived += workOrder_CodeReceived;
                gridPanel1.Show();
                SwitchStatusChanged?.Invoke(this, new DeviceSwitchStatusChangedEventArgs(DeviceSwitchType.Scanner_Switch, true));
            }
            else
            {
                btn_switchRTU.IconSvg = IconSvg.switch_IconSvg_Fail;
               await LogEventHelper.PushLog(LogType.SystemStartupShutdown, "串口扫码枪连接失败！", LogStatu.Failure).ConfigureAwait(false);
                AntdUI.Modal.open("串口扫码枪连接失败！", AntdUI.TType.Error);
                SwitchStatusChanged?.Invoke(this, new DeviceSwitchStatusChangedEventArgs(DeviceSwitchType.Scanner_Switch, false));
            }
          
        }
        #endregion

        #region 更新工单状态(未实现)
        private void btn_updateStatu_Click(object sender, EventArgs e)
        {
            panel_ScannerWorkOrder.Spin(AntdUI.Localization.Get("Loading", "正在查询MES系统工单信息..."), config =>
            {
               
                Thread.Sleep(1000);

                if (AntdUI.Config.IsDark)
                {
                    config.Back = Color.Black;
                    config.Fore = Color.White;
                }
                else
                {
                    config.Back = Color.White;
                    config.Fore = Color.Black;
                }



                config.Value = null;
                config.Text = AntdUI.Localization.Get("PleaseWait", "请耐心等候...");
                Thread.Sleep(2000);
            }, () =>
            {
                System.Diagnostics.Debug.WriteLine("加载结束");
            });
        }
        #endregion

        #region 保存扫码枪信息到Ini
        private void btn_SaveRTU_Click(object sender, EventArgs e)
        {
            if (this.scannerInfo == null) this.scannerInfo = new ScannerInfo();
            this.scannerInfo.PortName = this.select_Serialport.Text.Trim();
            this.scannerInfo.BaudRate = Convert.ToInt32(
                this.select_BaudRate.Text.Trim());
            this.scannerInfo.Parity = (Parity)Enum.Parse(typeof(Parity),
                this.select_Parity.Text.Trim(), true);
            this.scannerInfo.DataBits = Convert.ToInt32(
                this.select_DataBits.Text.Trim());
            this.scannerInfo.StopBits = (StopBits)Enum.Parse(typeof(StopBits), this.select_StopBits.Text.Trim(), true);
            this.scannerInfo.MesAPI = this.input_mesAPI.Text.Trim();
            this.scannerInfo.OverTime = int.Parse(this.inputNumber_overTime.Text.Trim());
            bool result = iniService.SetScannerInfoToPath(scannerInfo, scannerPath);
            if (result)
            {
                AntdUI.Modal.open("配置信息保存成功" + scannerPath + "", "保存配置", AntdUI.TType.Success);
            }
            else
            {
                AntdUI.Modal.open("配置信息保存失败", "保存配置", AntdUI.TType.Error);
            }
        }
        #endregion

        #region Scanner窗体加载事件
        private void Scanner_Load(object sender, EventArgs e)
        {
            _modbusDriver = new NModbusDriver();
            _modbusDriver.CodeReceived += workOrder_CodeReceived;
        }
        #endregion

        #region Scanner工单信息接收
        private void workOrder_CodeReceived(string code)
        {
            if (code.Length == 0)
            {
                return;
            }
            if (input_QRCode.InvokeRequired)
            {
                input_QRCode.BeginInvoke(new Action<string>((receivedCode) =>
                {
                    input_QRCode.Text = receivedCode.Trim();
                    input_QRCode.SelectAll();
                    if (input_QRCode != null)
                    {
                        lbl_scanTime.Text = DateTime.Now.ToString();
                    }
                }), code);
            }
            else
            {

                input_QRCode.Text = code.Trim();
                input_QRCode.SelectAll();
            }
        }
        #endregion

        #region 将获取到的工单信息添加到Table中
        private async Task InsertLogRow(ScannerDataDTO dto)
        {
            dto.Result = label_workOrderNo.Text;
            dto.OperatorWorkId = LoginConfig.WorkId;
            dto.ScanTime = lbl_scanTime.Text.ToDate();
            dto.ScanContent = input_QRCode.Text.ToString();
            dto.ScanStatus = (ScanStatus)Enum.Parse(typeof(ScanStatus), lbl_analysisResult.Text);
            var scanData = new ScannerData
            {
                ScanTime = dto.ScanTime,
                CodeContent = dto.ScanContent,
                WorkOrderNo = dto.Result,
                Statu = dto.ScanStatus,
                OperatorWorkId = dto.OperatorWorkId
            };
            _scans.Add(scanData);
            table_DataLog.Binding(_scans);
            await _scannerDatabll.AddScanRecord(dto);
        }
        #endregion

        #region 更新Scanner开关按钮状态
        public void UpdateScannerSwitch(bool isOn)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<bool>(UpdateScannerSwitch), isOn);
                return;
            }

            if ((btn_switchRTU.IconSvg == IconSvg.switch_IconSvg_Success) == isOn)
                return;

            btn_switchRTU_Click(null, EventArgs.Empty);


            if (isOn && btn_switchRTU.IconSvg != IconSvg.switch_IconSvg_Success && btn_switchRTU.IconSvg != IconSvg.switch_IconSvg_Fail)
            {
                btn_switchRTU.IconSvg = IconSvg.switch_IconSvg_Fail; 
            }
        }

        #endregion
    }
}
