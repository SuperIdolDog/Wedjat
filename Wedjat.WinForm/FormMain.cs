using AntdUI;
using HalconControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wedjat.BLL;
using Wedjat.Driver;
using Wedjat.Helper;
using Wedjat.Model.Config;
using Wedjat.Model.DTO;
using Wedjat.Model.Entity;
using Wedjat.WinForm.CameraMenuUI;
using Wedjat.WinForm.MainMenuUI;

namespace Wedjat.WinForm
{
    public partial class FormMain : Window
    {
        private readonly MVSDriver _cameraDriver;
        private CpuInfo cpuInfo;
        private readonly Dictionary<string, UserControl> _pages = new Dictionary<string, UserControl>();
        private Scanner _scanner;
        private MainMenuUI.Console _console;
        private PLC _plc;
        private Camera _camera;
        private DataBase _database;
        private DataReport _datareport;
        bool setcolor = false;
        private bool _isSyncing = false;
        private CancellationTokenSource _cpuCts;

        private void SwitchPage(string key, Func<UserControl> create)
        {
            if (panelContent.Controls.Count > 0)
            {
                var currentUc = panelContent.Controls[0];
                currentUc.Visible = false;
            }
            if (!_pages.TryGetValue(key, out var uc))
            {
                uc = create();
                uc.Dock = DockStyle.Fill;
                _pages[key] = uc;
                panelContent.Controls.Add(uc);
                if (key == "UCScanner")
                    _scanner = uc as Scanner;
                if (key == "UCCamera")
                    _camera = uc as Camera;
                if (key == "UCConsole")
                    _console = uc as MainMenuUI.Console;
                if (key == "UCPLC")
                    _plc = uc as PLC;
                if (key == "UCReport")
                    _datareport = uc as DataReport;
                if (key == "UCDatabase")
                    _database = uc as DataBase;
            }
            uc.Visible = true;
            uc.BringToFront();
        }

        public FormMain()
        {
            InitializeComponent();
            _cameraDriver = new MVSDriver();
            SwitchPage("UCCamera", () => new Camera(_cameraDriver));
            SwitchPage("UCPLC", () => new PLC());
            SwitchPage("UCScanner", () => new Scanner());
            SwitchPage("UCDatabase", () => new DataBase());
            SwitchPage("UCReport", () => new DataReport());
            SwitchPage("UCConsole", () => new MainMenuUI.Console(_cameraDriver, _plc._modbusDriver));

            var globals = new AntdUI.SelectItem[] {
                new AntdUI.SelectItem("中文","zh-CN"),
                new AntdUI.SelectItem("English","en-US")
            };
            btn_global.Items.AddRange(globals);
            cpuInfo = new CpuInfo();
            _cpuCts = new CancellationTokenSource();
            Task.Run(() => GetCPU(_cpuCts.Token), _cpuCts.Token);
        }

        #region 获取上位机CPU使用详情
        private async void GetCPU(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    cpuInfo.GetCpuInfo(out bool isSuccess, out string errorMsg);
                    this.BeginInvoke(() =>
                    {
                        label_CPU.Text = isSuccess ? cpuInfo.ToString() : $"CPU获取失败：{errorMsg}";
                    });
                }
                catch (Exception ex)
                {
                    AntdUI.Notification.error(this, "CPU监控异常", ex.Message);
                }
                await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
            }
        }
        #endregion

        #region 切换主题
        private void btn_Theme_Click(object sender, EventArgs e)
        {
            if (setcolor)
            {
                var color = AntdUI.Style.Db.Primary;
                AntdUI.Config.IsDark = !AntdUI.Config.IsDark;
                Dark = AntdUI.Config.IsDark;
                AntdUI.Style.SetPrimary(color);
            }
            else
            {
                AntdUI.Config.IsDark = !AntdUI.Config.IsDark;
                Dark = AntdUI.Config.IsDark;
            }

            btn_Theme.Toggle = Dark;
            if (Dark)
            {
                BackColor = Color.Black;
                ForeColor = Color.White;
            }
            else
            {
                BackColor = Control.DefaultBackColor;
                ForeColor = Color.Black;
            }
            OnSizeChanged(e);
        }
        #endregion

        #region 显示/隐藏Menu
        private void btn_Accordion_Click(object sender, EventArgs e)
        {
            bool isNowCollapsed = sys_Menu.Collapsed;
            sys_Menu.Collapsed = !isNowCollapsed;
            if (isNowCollapsed)
            {
                sys_Menu.Visible = true;
            }
            else
            {
                sys_Menu.Visible = false;
            }
        }
        #endregion

        #region Menu选择展示界面事件
        private void sys_Menu_SelectChanged(object sender, MenuSelectEventArgs e)
        {
            string key = (sender as AntdUI.Menu).SelectItem.Name;
            Func<UserControl> create;
            switch (key)
            {
                case "UCConsole":
                    create = () => _pages.ContainsKey("UCConsole") ? _pages["UCConsole"] : new MainMenuUI.Console(_cameraDriver, _plc._modbusDriver);
                    break;
                case "UCCamera":
                    create = () => _pages.ContainsKey("UCCamera") ? _pages["UCCamera"] : new Camera(_cameraDriver);
                    break;
                case "UCPLC":
                    create = () => _pages.ContainsKey("UCPLC") ? _pages["UCPLC"] : new PLC();
                    break;
                case "UCScanner":
                    create = () => _pages.ContainsKey("UCScanner") ? _pages["UCScanner"] : new Scanner();
                    break;
                case "UCDatabase":
                    create = () => _pages.ContainsKey("UCDatabase") ? _pages["UCDatabase"] : new DataBase();
                    break;
                case "UCReport":
                    create = () => _pages.ContainsKey("UCReport") ? _pages["UCReport"] : new DataReport();
                    break;
                default:
                    throw new NotImplementedException();
            }
            SwitchPage(key, create);
        }
        #endregion

        #region 主窗体加载事件
        private void FormMain_Load(object sender, EventArgs e)
        {
           
            // 设置工单接收事件
            _scanner.WorkOrderReceived += _console.OnWorkOrderReceived;
            _scanner.WorkOrderReceived += OnScannerWorkOrderReceived;

            // 设置设备开关状态同步
            SetupDeviceSwitchSync();

            // 设置工单和PCB双向同步
            SetupWorkOrderAndPCBSync();

            // 设置传感器触发同步
            SetupSensorTriggerSync();

            // 设置CollectionControl关系
            if (_camera != null && _camera.CurrentCollectionControl != null && _console != null)
            {
                _console.SetCollectionControl(_camera.CurrentCollectionControl);
            }
            else
            {
                AntdUI.Notification.warn(this, "初始化警告", "未获取到CollectionControl实例，处理后图像无法显示");
            }
        }

        /// <summary>
        /// 设置设备开关状态同步
        /// </summary>
        private void SetupDeviceSwitchSync()
        {
            _console.SwitchStatusChanged += (s, args) =>
            {
                switch (args.SwitchType)
                {
                    case DeviceSwitchType.Camera_Switch:
                        _camera.UpdateCameraSwitch(args.IsOn);
                        break;
                    case DeviceSwitchType.PLC_Switch:
                        _plc.UpdatePLCSwitch(DeviceSwitchType.PLC_Switch, args.IsOn);
                        break;
                    case DeviceSwitchType.Scanner_Switch:
                        _scanner.UpdateScannerSwitch(args.IsOn);
                        break;
                }
            };

            _camera.SwitchStatusChanged += (s, args) =>
            {
                _console.UpdateSwitchStatus(args.SwitchType, args.IsOn);
            };

            _plc.SwitchStatusChanged += (s, args) =>
            {
                _console.UpdateSwitchStatus(args.SwitchType, args.IsOn);
            };

            _scanner.SwitchStatusChanged += (s, args) =>
            {
                _console.UpdateSwitchStatus(args.SwitchType, args.IsOn);
            };
        }

        /// <summary>
        /// 设置工单和PCB双向同步
        /// </summary>
        private void SetupWorkOrderAndPCBSync()
        {
            // Scanner → Console & CollectionControl 同步
            _scanner.WorkOrderSyncRequested += (s, workOrder) =>
            {
                if (_isSyncing) return;
                _isSyncing = true;

                try
                {
                    _console.UpdateWorkOrderDisplay(workOrder);
                    _camera.CurrentCollectionControl.UpdateCurrentWorkOrder(workOrder);

                    // 启用采集控制
                    _camera.CurrentCollectionControl.TriggerControlEvent(true);

                    AntdUI.Notification.success(this, "工单同步成功",
                        $"工单号：{workOrder.WorkOrderCode} 已同步到所有界面");
                }
                finally
                {
                    _isSyncing = false;
                }
            };

            _scanner.PCBSyncRequested += (s, pcb) =>
            {
                if (_isSyncing) return;
                _isSyncing = true;

                try
                {
                    _console.UpdateSelectedPCB(pcb);
                    _camera.CurrentCollectionControl.UpdateSelectedPCB(pcb);

                    AntdUI.Notification.info(this, "PCB同步成功",
                        $"PCB型号：{pcb.mesPCBs.PCBName} 已同步到所有界面");
                }
                finally
                {
                    _isSyncing = false;
                }
            };

            // Console → Scanner & CollectionControl 同步（反向同步）
            _console.WorkOrderSyncRequested += (s, workOrder) =>
            {
                if (_isSyncing) return;
                _isSyncing = true;

                try
                {
                    _scanner.UpdateWorkOrderDisplay(workOrder);
                    _camera.CurrentCollectionControl.UpdateCurrentWorkOrder(workOrder);
                }
                finally
                {
                    _isSyncing = false;
                }
            };

            _console.PCBSyncRequested += (s, pcb) =>
            {
                if (_isSyncing) return;
                _isSyncing = true;

                try
                {
                    _scanner.UpdateSelectedPCB(pcb);
                    _camera.CurrentCollectionControl.UpdateSelectedPCB(pcb);
                }
                finally
                {
                    _isSyncing = false;
                }
            };
        }

        /// <summary>
        /// 设置传感器触发同步
        /// </summary>
        private void SetupSensorTriggerSync()
        {
            _console.Sensor2TriggerChanged += (s, isTriggered) =>
            {
                // 线程安全：确保在 UI 线程调用 Camera 的转发方法
                if (_camera.InvokeRequired)
                {
                    _camera.Invoke(new Action(() =>
                        _camera.ForwardSensor2Trigger(isTriggered)
                    ));
                }
                else
                {
                    _camera.ForwardSensor2Trigger(isTriggered);
                }
            };
        }

        /// <summary>
        /// 原有的Scanner工单接收事件处理
        /// </summary>
        private void OnScannerWorkOrderReceived(object sender, WorkOrderReceivedEventArgs e)
        {
            // 1. 校验必要数据（避免空值）
            if (e.WorkOrder == null || e.CurrentSelectedPCB == null)
            {
                AntdUI.Notification.warn(this, "数据异常", "工单或PCB信息为空，无法同步");
                return;
            }

            // 2. 找到Camera中的CollectionControl实例
            var collectionControl = _camera?.CurrentCollectionControl;
            if (collectionControl == null)
            {
                AntdUI.Notification.warn(this, "控件未就绪", "未找到CollectionControl，无法同步工单");
                return;
            }

            // 3. 将Scanner的数据传递给CollectionControl（核心步骤）
            collectionControl.CurrentWorkOrder = e.WorkOrder;
            collectionControl.CurrentSelectedPCB = e.CurrentSelectedPCB;

            // 4. 可选：自动启用采集功能（用户无需手动点击"开始采集"）
            collectionControl.TriggerControlEvent(true);

            // 5. 提示用户同步成功
            AntdUI.Notification.success(this, "工单同步成功",
                $"工单号：{e.WorkOrder.WorkOrderCode}\nPCB型号：{e.CurrentSelectedPCB.mesPCBs.PCBName}");
        }
        #endregion
       
        #region 窗体关闭事件
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            _cpuCts.Cancel(); // 取消CPU监控线程
            _cpuCts.Dispose();

        }
        #endregion

      
    }
}