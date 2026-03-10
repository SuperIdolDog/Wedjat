using AntdUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vanara.PInvoke;
using Wedjat.BLL;
using Wedjat.DAL;
using Wedjat.Driver;
using Wedjat.Model.Entity;

namespace Wedjat.WinForm.MainMenuUI
{
    public partial class DataBase : UserControl
    {

        private readonly DatabaseBLL _dbBLL;
        private readonly PCBBLL _pcbBLL;
        private readonly PCBDefectTypeBLL _pcbDefectTypeBLL;
        private readonly PCBDefectDetailBLL _pcbDefectDetailBLL;
        private readonly PCBInspectionBLL _pcbInspectionBLL;
        private readonly PLCLogBLL _plcLogBLL;
        private readonly PLCSlaveVariableBLL _plcSlaveVariableBLL;
        private readonly ScannerDataBLL _scannerDataBLL;
        private readonly SysLogBLL _sysLogBLL;
        private readonly WorkOrderBLL _workOrderBLL;
        private readonly WorkOrderPCBBLL _workOrderPcbBLL;
        private readonly NModbusDriver _nModbusDriver;

        private string _currentTableName;
        
        public DataBase()
        {
            InitializeComponent();
            _nModbusDriver = new NModbusDriver();
            _dbBLL=new DatabaseBLL();
            _pcbBLL=new PCBBLL();
            _pcbDefectTypeBLL=new PCBDefectTypeBLL();
            _pcbDefectDetailBLL=new PCBDefectDetailBLL();
            _pcbInspectionBLL=new PCBInspectionBLL();
            _plcLogBLL=new PLCLogBLL();
            _plcSlaveVariableBLL=new PLCSlaveVariableBLL(_nModbusDriver);
            _scannerDataBLL=new ScannerDataBLL();
            _sysLogBLL=new SysLogBLL();
            _workOrderBLL=new WorkOrderBLL();
            _workOrderPcbBLL=new WorkOrderPCBBLL();

            InitControls();
            table1.FixedHeader = true;
            table1.ColumnDragSort = true;
            table1.Bordered = true;
            table1.EnableHeaderResizing = true;
          
        }
        #region 初始化控件
        private void InitControls()
        {
            pagination1.PageSizeOptions = new[] { 10, 20, 30, 50, 100 };
            pagination1.Current = 1;
            pagination1.PageSize = 20;
            pagination1.ValueChanged += async (sender, e) =>
            {
                await LoadTableData(e.Current, e.PageSize);
            };

            // 表格基础配置
            table1.Bordered = true;
            table1.FixedHeader = true;
           // table1.EditMode = TEditMode.Click;
            table1.CellFocusedStyle = TableCellFocusedStyle.Solid;

            // 绑定下拉框选择变更事件（关键：表名切换时加载数据）
            select_DataBase.SelectedValueChanged += async (sender, e) =>
            {
                await select_DataBase_SelectedValueChangedAsync(sender, e);
            };
        }
        #endregion

        #region 获取数据表名称
        public async Task<List<string>> TableNames()
        {
         return await  _dbBLL.QueryAllTableNamesAsync().ConfigureAwait(false);
        }
        #endregion

        #region DataBase界面加载事件
        private async void DataBase_Load(object sender, EventArgs e)
        {
            try
            {
                var tables = await TableNames().ConfigureAwait(false);
                this.Invoke((Action)(() =>
                {
                    select_DataBase.Items.Clear();
                    select_DataBase.Items.AddRange(tables.ToArray());
                    select_DataBase.SelectedIndex = 0;
                }));
            }
            catch (Exception ex)
            {
                AntdUI.Notification.error(this.FindForm(), "加载失败", ex.Message);
            }
        }
        #endregion
  
        #region 根据数据表加载数据事件
        private async Task LoadTableData(int pageIndex, int pageSize)
        {
            if (string.IsNullOrEmpty(_currentTableName))
                throw new Exception("未选择数据表");

            try
            {
                List<object> tableData = new List<object>();
                long totalCount = 0;
                switch (_currentTableName)
                {
                    case "PCB":
                        {
                            var (data, total) = await _pcbBLL.GetPageListAsync(pageIndex, pageSize, null, null, true).ConfigureAwait(false);
                            tableData = data.Cast<object>().ToList();
                            totalCount = total;
                        }
                        break;

                    case "PCBDefectType":
                        {
                            var (data, total) = await _pcbDefectTypeBLL.GetPageListAsync(pageIndex, pageSize, null, null, true).ConfigureAwait(false);
                            tableData = data.Cast<object>().ToList();
                            totalCount = total;
                        }
                        break;

                    case "PCBDefectDetail":
                        {
                            var (data, total) = await _pcbDefectDetailBLL.GetPageListAsync(pageIndex, pageSize, null, null, true).ConfigureAwait(false);
                            tableData = data.Cast<object>().ToList();
                            totalCount = total;
                        }
                        break;

                    case "PCBInspection":
                        {
                            var (data, total) = await _pcbInspectionBLL.GetPageListAsync(pageIndex, pageSize, null, null, true).ConfigureAwait(false);
                            tableData = data.Cast<object>().ToList();
                            totalCount = total;
                        }
                        break;

                    case "PLCLog":
                        {
                            var (data, total) = await _plcLogBLL.GetPageListAsync(pageIndex, pageSize, null, null, true).ConfigureAwait(false);
                            tableData = data.Cast<object>().ToList();
                            totalCount = total;
                        }
                        break;

                    case "PLCSlaveVariable":
                        {
                            var (data, total) = await _plcSlaveVariableBLL.GetPageListAsync(pageIndex, pageSize, null, null, true).ConfigureAwait(false);
                            tableData = data.Cast<object>().ToList();
                            totalCount = total;
                        }
                        break;

                    case "ScannerData":
                        {
                            var (data, total) = await _scannerDataBLL.GetPageListAsync(pageIndex, pageSize, null, null, true).ConfigureAwait(false);
                            tableData = data.Cast<object>().ToList();
                            totalCount = total;
                        }
                        break;

                    case "SysLog":
                        {
                            var (data, total) = await _sysLogBLL.GetPageListAsync(pageIndex, pageSize, null, null, true).ConfigureAwait(false);
                            tableData = data.Cast<object>().ToList();
                            totalCount = total;
                        }
                        break;

                    case "WorkOrder":
                        {
                            var (data, total) = await _workOrderBLL.GetPageListAsync(pageIndex, pageSize, null, null, true).ConfigureAwait(false);
                            tableData = data.Cast<object>().ToList();
                            totalCount = total;
                        }
                        break;

                    case "WorkOrderPCB":
                        {
                            var (data, total) = await _workOrderPcbBLL.GetPageListAsync(pageIndex, pageSize, null, null, true).ConfigureAwait(false);
                            tableData = data.Cast<object>().ToList();
                            totalCount = total;
                        }
                        break;

                    default:
                        throw new Exception($"未配置表【{_currentTableName}】的数据加载逻辑");
                }

                Invoke(new Action(() =>
                {
                    table1.DataSource = tableData;
                    pagination1.Total = (int)totalCount;
                    pagination1.Current = pageIndex;

                    if (pageIndex == 1)
                        AntdUI.Notification.info(FindForm(), "加载成功", $"共{totalCount}条数据", TAlignFrom.Top);
                }));
            }
            catch (TargetInvocationException ex)
            {
                var innerEx = ex.InnerException ?? ex;
                Debug.WriteLine($"分页查询失败：{innerEx.Message}");
                AntdUI.Notification.error(FindForm(), "查询失败", innerEx.Message);
            }
        }
        #endregion

        #region 初始化数据表各列信息
        private void GenerateTableColumns()
        {
            table1.Columns.Clear();
            var columns = new ColumnCollection();
            columns.Add(new ColumnCheck("Checked")
                .SetFixed()
                .SetAlign(ColumnAlign.Center));
            columns.Add(new Column("Id", "编号").SetAlign(ColumnAlign.Center));
            switch (_currentTableName)
            {
                case "PCB":
                    columns.Add(new Column("PCBCode", "PCB编码")
                       .SetAlign(ColumnAlign.Left));
                    columns.Add(new Column("PCBName", "PCB名称")
                        .SetAlign(ColumnAlign.Left)
                        .SetDefaultFilter(typeof(string)));
                    break;

                case "PCBDefectType":
                    columns.Add(new Column("PCBCode", "PCB编码")
                          .SetAlign(ColumnAlign.Left)
                          .SetDefaultFilter(typeof(string)));
                    columns.Add(new Column("DefectTypeCode", "缺陷编码")
                         .SetAlign(ColumnAlign.Left)
                         .SetDefaultFilter(typeof(string)));
                    columns.Add(new Column("DefectName", "缺陷名称")
                        .SetAlign(ColumnAlign.Left)
                        .SetDefaultFilter(typeof(string)));
                    columns.Add(new Column("Description", "缺陷描述")
                        .SetAlign(ColumnAlign.Left));
                    break;

                case "PCBDefectDetail":
                    columns.Add(new Column("InspectionCode", "检测记录编码")
                        .SetAlign(ColumnAlign.Left)
                        .SetDefaultFilter(typeof(string)));
                    columns.Add(new Column("DefectTypeCode", "缺陷类型编码")
                        .SetAlign(ColumnAlign.Left)
                        .SetDefaultFilter(typeof(string)));
                    columns.Add(new Column("DefectTypeName", "缺陷类型名称")
                        .SetAlign(ColumnAlign.Left)
                        .SetDefaultFilter(typeof(string)));
                    columns.Add(new Column("DefectCount", "缺陷数量")
                        .SetAlign(ColumnAlign.Right)
                        .SetDefaultFilter(typeof(int)));
                    columns.Add(new Column("DefectPosition", "缺陷位置")
                        .SetAlign(ColumnAlign.Left));
                    columns.Add(new Column("Remark", "备注")
                        .SetAlign(ColumnAlign.Left)
                        .SetDefaultFilter(typeof(string)));
                    break;

                case "PCBInspection":
                    columns.Add(new Column("WorkOrderCode", "工单号")
                         .SetAlign(ColumnAlign.Left));
                    columns.Add(new Column("PCBCode", "PCB编码")
                        .SetAlign(ColumnAlign.Left));
                    columns.Add(new Column("PCBSN", "PCB序列号")
                        .SetAlign(ColumnAlign.Left));
                    columns.Add(new Column("Image", "检测图片路径")
                        .SetAlign(ColumnAlign.Left));
                    columns.Add(new Column("InspectionTime", "检测时间")
                        .SetAlign(ColumnAlign.Center)
                        .SetDisplayFormat("yyyy-MM-dd HH:mm:ss")
                        .SetDefaultFilter(typeof(DateTime)));
                    columns.Add(new Column("IsQualified", "是否合格", ColumnAlign.Center)
                    {
                        Render = (value, record, rowIndex) =>
                        {
                            var pi = record as PCBInspection;
                            if (pi == null)
                                return new CellTag("未知数据", AntdUI.TTypeMini.Info);
                            bool statu = pi.IsQualified;

                            string text;
                            AntdUI.TTypeMini type;
                            switch (statu)
                            {
                                case true:
                                    text = "合格";
                                    type = AntdUI.TTypeMini.Success;
                                    break;
                                case false:
                                    text = "不良";
                                    type = AntdUI.TTypeMini.Error;
                                    break;
                                default:
                                    text = "未知状态";
                                    type = AntdUI.TTypeMini.Default;
                                    break;
                            }

                            return new CellTag(text, type);
                        }
                    }.SetDefaultFilter(typeof(Enum)));

                    columns.Add(new Column("InspectorWorkId", "检测员工号")
                        .SetAlign(ColumnAlign.Left));
                    columns.Add(new Column("ProductLine", "产线")
                        .SetAlign(ColumnAlign.Left));
                    columns.Add(new Column("TotalDefectCount", "总缺陷数量")
                        .SetAlign(ColumnAlign.Right));
                    break;

                case "PLCLog":
                    columns.Add(new Column("SlaveAddress", "从站地址")
                        .SetAlign(ColumnAlign.Right));
                    columns.Add(new Column("CommunicationType", "通信类型")
                        .SetAlign(ColumnAlign.Left));
                    columns.Add(new Column("AddressInfo", "地址信息")
                        .SetAlign(ColumnAlign.Right));
                    columns.Add(new Column("Data", "通信数据")
                        .SetAlign(ColumnAlign.Left));
                    columns.Add(new Column("IsSuccess", "通信状态", ColumnAlign.Center)
                    {
                        Render = (value, record, rowIndex) =>
                        {
                            var log = record as PLCLog;
                            if (log == null)
                                return new CellTag("未知数据", AntdUI.TTypeMini.Info);
                            bool statu = log.IsSuccess;
                            string text;
                            AntdUI.TTypeMini type;
                            switch (statu)
                            {
                                case true:
                                    text = "通信成功";
                                    type = AntdUI.TTypeMini.Success;
                                    break;
                                case false:
                                    text = "通信失败";
                                    type = AntdUI.TTypeMini.Error;
                                    break;
                                default:
                                    text = "未知状态";
                                    type = AntdUI.TTypeMini.Default;
                                    break;
                            }

                            return new CellTag(text, type);
                        }
                    }.SetDefaultFilter(typeof(Enum)));
                      
                    columns.Add(new Column("ErrorMessage", "错误信息")
                        .SetAlign(ColumnAlign.Left));
                    columns.Add(new Column("ConnectionType", "连接类型")
                        .SetAlign(ColumnAlign.Left).SetDefaultFilter(typeof(string)));
                    columns.Add(new Column("ConnectionParams", "连接参数")
                        .SetAlign(ColumnAlign.Left));
                    break;

                case "PLCSlaveVariable":
                    columns.Add(new Column("SlaveAddress", "从站地址")
                         .SetAlign(ColumnAlign.Right).SetDefaultFilter(typeof(string)));
                    columns.Add(new Column("DataBlockAddress", "数据块地址")
                        .SetAlign(ColumnAlign.Left).SetDefaultFilter(typeof(string)));
                    columns.Add(new Column("VariableName", "变量名称")
                        .SetAlign(ColumnAlign.Left));
                    columns.Add(new Column("CurrentValue", "当前值")
                       .SetAlign(ColumnAlign.Right));
                    columns.Add(new Column("Description", "变量描述")
                        .SetAlign(ColumnAlign.Left));
                    columns.Add(new Column("DataType", "数据类型")
                        .SetAlign(ColumnAlign.Left).SetDefaultFilter(typeof(int)));
                    columns.Add(new Column("ModbusAddress", "Modbus地址")
                        .SetAlign(ColumnAlign.Right));
                    columns.Add(new Column("ModbusType", "Modbus类型")
                        .SetAlign(ColumnAlign.Left));
                    columns.Add(new ColumnSwitch("IsWritable", "是否可写")
                        .SetAlign(ColumnAlign.Center).SetDefaultFilter(typeof(bool)));
                    columns.Add(new Column("DataLength", "数据长度")
                        .SetAlign(ColumnAlign.Left));
                    break;

                case "ScannerData":
                    columns.Add(new Column("CodeContent", "二维码内容")
                         .SetAlign(ColumnAlign.Left).SetDefaultFilter(typeof(string)));
                    columns.Add(new Column("WorkOrderNo", "工单号")
                        .SetAlign(ColumnAlign.Left));
                    columns.Add(new Column("ScanTime", "扫描时间")
                        .SetAlign(ColumnAlign.Center)
                        .SetDisplayFormat("yyyy-MM-dd HH:mm:ss")
                        .SetDefaultFilter(typeof(DateTime)));
                    columns.Add(new Column("Statu", "扫描状态", ColumnAlign.Center)
                    {
                        Render = (value, record, rowIndex) =>
                        {
                            var sd = record as ScannerData;
                            if (sd == null)
                                return new CellTag("未知数据", AntdUI.TTypeMini.Info);
                            ScanStatus status = sd.Statu;
                            string text;
                            AntdUI.TTypeMini type;
                            switch (status)
                            {
                                case ScanStatus.Success:
                                    text = "扫码成功";
                                    type = AntdUI.TTypeMini.Success;
                                    break;
                                case ScanStatus.Failed:
                                    text = "扫码失败";
                                    type = AntdUI.TTypeMini.Default;
                                    break;
                                case ScanStatus.WorkOrderNotFound:
                                    text = "工单不存在";
                                    type = AntdUI.TTypeMini.Warn;
                                    break;
                                case ScanStatus.MESApiException:
                                    text = "MES接口异常";
                                    type = AntdUI.TTypeMini.Error;
                                    break;
                                default:
                                    text = "未定义状态";
                                    type = AntdUI.TTypeMini.Primary;
                                    break;
                            }

                            return new CellTag(text, type);
                        }
                    }.SetDefaultFilter(typeof(Enum)));
                    columns.Add(new Column("OperatorWorkId", "操作员工号")
                        .SetAlign(ColumnAlign.Left));
                    break;

                case "SysLog":
                    columns.Add(new AntdUI.Column("LogType", "日志类型", ColumnAlign.Center)
                    {
                        Render = (value, record, rowIndex) =>
                        {

                            var log = record as SysLog;
                            if (log == null)
                                return new CellTag("未知数据", AntdUI.TTypeMini.Info);
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
                                case LogType.ScanOperation:
                                    text = "扫码日志";

                                    break;
                                case LogType.InspectionOperation:
                                    text = "检测日志";

                                    break;
                                case LogType.PLCCommunication:
                                    text = "PLC日志";

                                    break;
                                case LogType.MESApiCall:
                                    text = "MesAPI日志";
                                    break;
                                case LogType.DatabaseOperation:
                                    text = "数据库日志";
                                    break;
                                case LogType.SystemStartupShutdown:
                                    text = "系统状态";
                                    break;
                                case LogType.ConfigurationChange:
                                    text = "配置变更";
                                    break;
                                default:
                                    text = "未定义状态";

                                    break;
                            }

                            return new AntdUI.CellTag(text);
                        }
                    }.SetFixed().SetDefaultFilter(typeof(Enum)));

                    columns.Add(new Column("Detail", "日志详情")
                        .SetAlign(ColumnAlign.Left));
                    columns.Add(new AntdUI.Column("LogStatu", "状态", ColumnAlign.Center)
                    {
                        Render = (value, record, rowIndex) =>
                        {

                            var log = record as SysLog;
                            if (log == null)
                                return new CellTag("未知数据", AntdUI.TTypeMini.Info);
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
                    }.SetFixed().SetDefaultFilter(typeof(Enum)));
                    break;

                case "WorkOrder":
                    columns.Add(new Column("WorkOrderCode", "工单编号")
                        .SetAlign(ColumnAlign.Left).SetDefaultFilter(typeof(string)));
                    columns.Add(new Column("PlanQuantity", "计划数量")
                        .SetAlign(ColumnAlign.Right));
                    columns.Add(new Column("CompleteQuantity", "完成数量")
                        .SetAlign(ColumnAlign.Right));
                    columns.Add(new Column("OrderStatus", "工单状态", ColumnAlign.Center)
                    {
                        Render = (value, record, rowIndex) =>
                        {

                            var wo = record as WorkOrder;
                            if (wo == null)
                                return new CellTag("未知数据", AntdUI.TTypeMini.Info);
                            OrderStatu orderStatu = wo.OrderStatus;
                            string text;
                            AntdUI.TTypeMini type;
                            switch (orderStatu)
                            {
                                case OrderStatu.Pending:
                                    text = "待处理";
                                    type = AntdUI.TTypeMini.Warn;
                                    break;
                                case OrderStatu.Processing:
                                    text = "处理中";
                                    type = AntdUI.TTypeMini.Primary;
                                    break;
                                case OrderStatu.Paused:
                                    text = "已暂停";
                                    type = AntdUI.TTypeMini.Error;
                                    break;
                                case OrderStatu.Completed:
                                    text = "已完成";
                                    type = AntdUI.TTypeMini.Success;
                                    break;
                                case OrderStatu.Canceled:
                                    text = "已取消";
                                    type = AntdUI.TTypeMini.Default;
                                    break;
                                default:
                                    text = "未定义状态";
                                    type = AntdUI.TTypeMini.Info;
                                    break;
                            }
                            return new AntdUI.CellTag(text, type);
                        }
                    }.SetDefaultFilter(typeof(Enum)));
                    break;

                case "WorkOrderPCB":
                    columns.Add(new Column("WorkOrderCode", "工单号")
                        .SetAlign(ColumnAlign.Left).SetDefaultFilter(typeof(string)));
                    columns.Add(new Column("PCBCode", "PCB编码")
                        .SetAlign(ColumnAlign.Left));
                    break;

                default:
                    throw new Exception($"未配置表【{_currentTableName}】的字段");

            }
            columns.Add(new ColumnSwitch("IsDeleted", "是否软删")
               .SetFixed()
               .SetAlign(ColumnAlign.Center)
               .SetDefaultFilter(typeof(bool)));
            columns.Add(new Column("CreateTime", "创建时间").SetAlign(ColumnAlign.Center).SetDisplayFormat("yyyy-MM-dd HH:mm:ss").SetDefaultFilter(typeof(DateTime)));
            columns.Add(new Column("UpdateTime", "更新时间").SetAlign(ColumnAlign.Center).SetDisplayFormat("yyyy-MM-dd HH:mm:ss").SetDefaultFilter(typeof(DateTime)));
            table1.Columns = columns;
        }
        #endregion

        #region 导出当前数据为Excel事件
        private async void button_ExportExcel_Click(object sender, EventArgs e)
        {
            if (table1.DataSource == null)
            {
                AntdUI.Notification.warn(this.FindForm(), "导出失败", "无数据可导出");
                return;
            }

            var dia = new SaveFileDialog()
            {
                Filter = "(Excel文件)|*.xlsx"
            };
            if (dia.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            var loading = table1.Spin(AntdUI.Localization.Get("Loading", "正在导出数据..."), config =>
            {
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

            });
            try
            {
                // 后台线程执行导出（非UI操作）
                await Task.Run(() =>
                {
                    MiniExcelLibs.MiniExcel.SaveAs(dia.FileName, table1.DataSource, overwriteFile: true);
                }).ConfigureAwait(false);

                // 导出成功（切换回UI线程提示）
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        AntdUI.Notification.success(this.FindForm(), "导出成功", $"文件已保存至：{dia.FileName}");
                    }));
                }
                else
                {
                    AntdUI.Notification.success(this.FindForm(), "导出成功", $"文件已保存至：{dia.FileName}");
                }
            }
            catch (Exception ex)
            {
                // 导出失败提示
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        AntdUI.Notification.error(this.FindForm(), "导出失败", ex.Message);
                    }));
                }
                else
                {
                    AntdUI.Notification.error(this.FindForm(), "导出失败", ex.Message);
                }
            }
            finally
            {
                // 关闭加载提示（必须UI线程）
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => loading.Dispose()));
                }
                else
                {
                    loading.Dispose();
                }
            }

        }
        #endregion

        #region 切换数据表事件
        private async Task select_DataBase_SelectedValueChangedAsync(object sender, ObjectNEventArgs e)
        {
            if (e.Value == null) return;

            _currentTableName = e.Value.ToString();
            try
            {
             

                await table1.Spin(AntdUI.Localization.Get("Loading", "正在加载数据..."), config =>
                {
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
                   
                }, async () =>
                {
                    try
                    {
                        GenerateTableColumns();
                        await LoadTableData(1, pagination1.PageSize).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        AntdUI.Notification.error(this.FindForm(), "数据加载失败", ex.Message, TAlignFrom.Top);
                    }
                });
            }
            catch (Exception ex)
            {
                AntdUI.Notification.error(this.FindForm(), "加载失败", $"加载{_currentTableName}数据时出错：{ex.Message}", TAlignFrom.Top);
            }
        }
        #endregion
    }
}
