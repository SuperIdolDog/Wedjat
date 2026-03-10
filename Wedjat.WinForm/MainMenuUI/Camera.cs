using AntdUI;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wedjat.Driver;
using Wedjat.Driver.Model;
using Wedjat.Helper;
using Wedjat.Model.Config;
using Wedjat.Model.Entity;
using Wedjat.WinForm.CameraMenuUI;


namespace Wedjat.WinForm.MainMenuUI
{

    public partial class Camera : UserControl
    {
        public event EventHandler<bool> Sensor2TriggerForwarded;
        public event EventHandler<DeviceSwitchStatusChangedEventArgs> SwitchStatusChanged;
        public event EventHandler<PCBInspectionSavedEventArgs> InspectionSaved;
        public event Action<CollectionControl> CollectionControlReady;

        private MVSDriver _cameradriver;
        private CollectionControl _cc;

        public BindingList<PCBInspection> _inspections = new BindingList<PCBInspection>();

        #region 页面切换方法
        private void SwitchPage(string key, Func<UserControl> create)
        {
            if (panel_CameraMenu.Controls.Count > 0)
                panel_CameraMenu.Controls[0].Visible = false;

            if (!_pages.TryGetValue(key, out var uc))
            {
                uc = create();
                uc.Dock = DockStyle.Fill;
                _pages[key] = uc;
                panel_CameraMenu.Controls.Add(uc);
            }

            uc.Visible = true;
            uc.BringToFront();
        }
        #endregion

        public CollectionControl CurrentCollectionControl => _cc;
        public CollectionControl CreateCollectionControl()
        {
            var cc = new CollectionControl(_cameradriver);
            // 订阅图像导入事件（显示原始图像）
            cc.ImageImported += CollectionControl_ImageImported;
            // 订阅检测结果事件（叠加缺陷标注）
            cc.DetectionCompleted += CollectionControl_DetectionCompleted;
            cc.InspectionSaved += CollectionControl_InspectionSaved;
            return cc;
        }
        public Camera(MVSDriver cameradriver)
        {

            InitializeComponent();
            _cameradriver = cameradriver;
            SwitchPage("CollectionControl", () =>
            {
                var cc = CreateCollectionControl();
                _cc = cc;

                return cc;
            });
            _cameradriver.ImageLoaded += CameraHandler_ImageUpdated;
           

            table_Log.Columns = new AntdUI.ColumnCollection
        {
            new AntdUI.Column("InspectionCode", "检测编号"),
            new AntdUI.Column("WorkOrderCode", "工单号"),
            new AntdUI.Column("PCBCode", "PCB类型"),
            new AntdUI.Column("PCBSN", "序列号"),
            new AntdUI.Column("InspectionTime", "检测时间"),
            new AntdUI.Column("IsQualified", "是否合格")
            {
                Render = (value, record, rowIndex) =>
                {
                    var inspection = record as PCBInspection;
                    string text = inspection?.IsQualified == true ? "合格" : "不合格";
                    var type = inspection?.IsQualified == true ? AntdUI.TTypeMini.Success : AntdUI.TTypeMini.Error;
                    return new AntdUI.CellTag(text, type);
                }
            },
            new AntdUI.Column("TotalDefectCount", "缺陷数量")
        };

            table_Log.Binding(_inspections);
        }
       
        private readonly Dictionary<string, UserControl> _pages = new Dictionary<string, UserControl>();

        public void ForwardSensor2Trigger(bool isTriggered)
        {
            // 确保 CollectionControl 已初始化（切换页面时 _cc 会更新）
            if (_cc == null || _cc.IsDisposed) return;

            // 调用子级的公开方法，间接触发硬触发
            _cc.TriggerSensor2Trigger(isTriggered);
        }

        #region 接收CollectionControl的检测结果并添加到表格
        private void CollectionControl_InspectionSaved(object sender, PCBInspectionSavedEventArgs e)
        {
            
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    AddInspectionToTable(e.Inspection);
                }));
            }
            else
            {
                AddInspectionToTable(e.Inspection);
            }
        }
        #endregion

        #region 添加检测数据到表格
        private void AddInspectionToTable(PCBInspection inspection)
        {
            // 插入到列表开头，最新数据显示在最上面
            _inspections.Insert(0, inspection);

            // 限制显示的最大记录数,防止内存占用过高，实际应用中可以根据需要调整这个数值
            if (_inspections.Count > 1000)
            {
                _inspections.RemoveAt(_inspections.Count - 1);
            }
            // 刷新表格显示
            table_Log.Invalidate();
        }

        #endregion

        #region 相机序列框下拉事件
        private void menu_camera_SelectChanged(object sender, AntdUI.MenuSelectEventArgs e)
        {
            string key = (sender as AntdUI.Menu).SelectItem.Name;
            Func<UserControl> create;
            switch (key)
            {
                case "CollectionControl":
                    create = () =>
                    {
                        var cc = new CollectionControl(_cameradriver);
                        _cc = cc;
                        return cc;
                    };
                    break;
                case "AdvancedParameters":
                    create = () => new AdvancedPar(_cameradriver);
                    break;
                case "AlgorithmModule":
                    create = () => new AlgorithmModule(_cameradriver);
                    break;
                default:
                    throw new NotImplementedException();
            }
            SwitchPage(key, create);
        }
        #endregion

        #region 刷新相机列表并更新下拉框
        private async Task RefreshCameraList()
        {
            Invoke(new Action(() =>
            {
                select_DeviceSerialNumber.Items.Clear();
                select_DeviceSerialNumber.Text = null;
            }));
            List<string> cameraSerials = new List<string>();
            string errorMsg = string.Empty;
            try
            {
                // 耗时操作：相机枚举（后台线程执行）
                await Task.Run(() =>
                {
                    var cameraList = _cameradriver.EnumDevices(out errorMsg);
                    if (string.IsNullOrEmpty(errorMsg))
                    {
                        cameraSerials = cameraList.Select(c => c.SerialNumber).ToList();
                    }
                }).ConfigureAwait(false);

                // UI线程更新下拉框
                Invoke(new Action(() =>
                {
                    if (!string.IsNullOrEmpty(errorMsg))
                    {
                        AntdUI.Message.error(this.FindForm(), errorMsg);
                        return;
                    }
                    foreach (var serial in cameraSerials)
                    {
                        select_DeviceSerialNumber.Items.Add(serial);
                    }
                    if (select_DeviceSerialNumber.Items.Count > 0)
                    {
                        select_DeviceSerialNumber.SelectedIndex = 0;
                    }
                }));

                await LogEventHelper.PushLog(LogType.SystemOperation, "相机搜索成功!", LogStatu.Success).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await LogEventHelper.PushLog(LogType.SystemOperation, $"相机枚举失败{ex.Message}", LogStatu.Failure).ConfigureAwait(false);
                Invoke(new Action(() =>
                {
                    AntdUI.Message.error(this.FindForm(), "相机枚举失败：" + ex.Message);
                }));
            }
        }
        #endregion

        #region Camera加载事件
        private async void Camera_Load(object sender, EventArgs e)
        {
          await  RefreshCameraList().ConfigureAwait(false);
            if (_cameradriver != null && _cameradriver.IsConnect)
            {
                try
                {
                    // 尝试设置彩色图像格式
                    bool success = _cameradriver.SetImageFormat("RGB8");
                    if (success)
                    {
                       // Console.WriteLine("已设置图像格式为RGB8");
                    }
                    else
                    {
                        //Console.WriteLine("设置RGB8格式失败，尝试其他格式");

                        // 获取支持的格式
                        var formats = _cameradriver.GetImageFormats();
                       // Console.WriteLine($"支持的格式: {string.Join(", ", formats)}");

                        // 尝试设置BayerRG8格式
                        if (formats.Contains("BayerRG8"))
                        {
                            _cameradriver.SetImageFormat("BayerRG8");
                           // Console.WriteLine("已设置图像格式为BayerRG8");
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Console.WriteLine($"设置图像格式异常: {ex.Message}");
                }
            }
        }
        #endregion

        #region 搜索相机
        private async void button_searchCamera_Click(object sender, EventArgs e)
        {
            await RefreshCameraList().ConfigureAwait(false);
        }
        #endregion
      
        #region 选择性打开相机
        private async void switch_camera_CheckedChanged(object sender, AntdUI.BoolEventArgs e)
        {

            if (sender is Switch swit)
            {
                if (swit.Checked)
                {
                    var serial = select_DeviceSerialNumber.Text;
                    if (string.IsNullOrEmpty(serial))
                    {
                        Invoke(new Action(() =>
                        {
                            AntdUI.Message.warn(this.FindForm(), "请先从下拉框选择相机！");
                            swit.Checked = false;
                        }));
                        return;
                    }
                    if (_cameradriver.OpenDevice(serial))
                    {
                        _cameradriver.CloseDevice();
                    }

                    bool openSuccess = await Task.Run(() =>
                    {
                        return _cameradriver.OpenDevice(serial);
                    }).ConfigureAwait(false);

                    Invoke(new Action(async () =>
                    {
                        if (openSuccess)
                        {
                           await LogEventHelper.PushLog(LogType.SystemStartupShutdown, "相机打开成功!", LogStatu.Success).ConfigureAwait(false);
                            AntdUI.Message.success(this.FindForm(), "相机打开成功");
                            await Task.Delay(500);
                            try
                            {
                               // Console.WriteLine("尝试设置默认图像格式...");
                                bool formatSuccess = _cameradriver.SetImageFormat("BayerRG8");
                                if (formatSuccess)
                                {
                                   // Console.WriteLine("已设置图像格式为 BayerRG8");
                                }
                                else
                                {
                                    //Console.WriteLine("设置 BayerRG8 失败，尝试其他Bayer格式");
                                    var formats = _cameradriver.GetImageFormats();
                                    if (formats.Count > 0)
                                    {
                                        _cameradriver.SetImageFormat(formats[0]);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                               // Console.WriteLine($"设置图像格式异常: {ex.Message}");
                            }
                            if (_cc != null)
                            {
                                // 延迟一小段时间，确保相机完全初始化
                                await Task.Delay(200);
                                _cc.TriggerControlEvent(true);
                                _cc.RefreshImageFormats();
                            }
                        }
                        else
                        {
                           await LogEventHelper.PushLog(LogType.SystemStartupShutdown, "相机打开失败!", LogStatu.Failure).ConfigureAwait(false);
                            AntdUI.Message.error(this.FindForm(), "相机打开失败，请查看控制台输出");
                            swit.Checked = false;
                            _cc?.TriggerControlEvent(false);
                        }
                    }));
                }
                else
                {
                    await Task.Run(() => _cameradriver.CloseDevice()).ConfigureAwait(false);

                    Invoke(new Action(async () =>
                    {
                       await LogEventHelper.PushLog(LogType.SystemStartupShutdown, "相机已关闭!", LogStatu.System).ConfigureAwait(false);
                        AntdUI.Message.info(this.FindForm(), "相机已关闭");
                        _cc?.TriggerControlEvent(false);
                        _cc.RefreshImageFormats(true);
                    }));
                }
            }
            SwitchStatusChanged?.Invoke(this, new DeviceSwitchStatusChangedEventArgs(DeviceSwitchType.Camera_Switch, e.Value));
        }
        #endregion

        #region 更新相机开关状态
        public void UpdateCameraSwitch(bool isOn)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<bool>(UpdateCameraSwitch), isOn);
                return;
            }
            if (switch_camera.Checked != isOn)
                switch_camera.Checked = isOn;
        }
        #endregion
  
        #region 海康控件图像更新事件
        private void CameraHandler_ImageUpdated(Bitmap bitmap)
        {
           //($"CameraHandler_ImageUpdated: 收到图像 {bitmap?.Width}x{bitmap?.Height}, PixelFormat={bitmap?.PixelFormat}");
            if (hWindow_Final1.InvokeRequired)
            {
                hWindow_Final1.Invoke(new Action<Bitmap>(ShowCameraImage), bitmap);
            }
            else
            {
                ShowCameraImage(bitmap);
            }
        }
        #endregion

        #region 海康控件图像传入事件
        private void CollectionControl_ImageImported(object sender, HObject image)
        {
            if (hWindow_Final1.InvokeRequired)
            {
                hWindow_Final1.Invoke(new Action<HObject>(ShowImportedImage), image);
            }
            else
            {
                ShowImportedImage(image);
            }
        }
        #endregion

        #region 海康控件显示传入的图片
        private void ShowImportedImage(HObject image)
        {
            if (image == null || !image.IsInitialized() || hWindow_Final1 == null)
                return;

            try
            {
                hWindow_Final1.ClearWindow(); // 清空残留
                hWindow_Final1.HobjectToHimage(image); // 直接显示原始图像
            }
            catch (Exception ex)
            {
                AntdUI.Notification.error(this.FindForm(), "导入图像显示失败", ex.Message);
            }
            // 不释放image：检测时还要用，最后在检测完成后释放
        }
        #endregion

        #region 海康控件显示检测结果
        private   void CollectionControl_DetectionCompleted(object sender, DetectionResultEventArgs e)
        {
            if (hWindow_Final1.InvokeRequired)
            {
                hWindow_Final1.BeginInvoke(new Action<DetectionResultEventArgs>(ShowDetectionResult), e);
            }
            else
            {
                ShowDetectionResult(e);
            }
        }
        #endregion

        #region  显示检测结果的方法
        private async void ShowDetectionResult(DetectionResultEventArgs e)
        {
            if (e.OriginalImage == null || !e.OriginalImage.IsInitialized() || hWindow_Final1 == null)
                return;

            try
            {
                hWindow_Final1.ClearWindow();
                hWindow_Final1.HobjectToHimage(e.OriginalImage);

                // 2. 叠加缺失缺陷（红色）
                //if (e.MissingCount.I > 0 && e.MissingRegions != null && e.MissingRegions.IsInitialized())
                //{
                //    hWindow_Final1.DispObj(e.MissingRegions, "red");
                //}

                //// 3. 叠加多余缺陷（黄色）
                //if (e.ExtraCount.I > 0 && e.ExtraRegions != null && e.ExtraRegions.IsInitialized())
                //{
                //    hWindow_Final1.DispObj(e.ExtraRegions, "yellow");
                //}

                // 4. 显示检测结果提示
                string resultMsg = e.MissingCount.I > 0 //|| e.ExtraCount.I > 0
                    ? $"发现{e.MissingCount.I}处缺失缺陷"//{e.ExtraCount.I}处多余缺陷"
                    : "未发现缺陷，PCB合格";
               await LogEventHelper.PushLog(LogType.InspectionOperation, resultMsg, LogStatu.System).ConfigureAwait(false);
                AntdUI.Notification.info(this.FindForm(), "检测结果", resultMsg);
            }
            catch (Exception ex)
            {
                AntdUI.Notification.error(this.FindForm(), "检测结果显示失败", ex.Message);
            }
            
        }
        #endregion

        #region 显示图片的方法
        private void ShowCameraImage(Bitmap bitmap)
        {
            if (bitmap == null || hWindow_Final1 == null)
                return;

            HObject hImage = null;
            try
            {
                // 调试信息
               // Console.WriteLine($"接收到Bitmap: {bitmap.Width}x{bitmap.Height}, PixelFormat: {bitmap.PixelFormat}");

                // 1. 检查图像格式
                if (bitmap.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    // 黑白图像
                    //Console.WriteLine("检测到8位灰度图像");
                    hImage = BitmapToHObject(bitmap);
                }
                else if (bitmap.PixelFormat == PixelFormat.Format24bppRgb ||
                         bitmap.PixelFormat == PixelFormat.Format32bppArgb)
                {
                    // 彩色图像
                   // Console.WriteLine("检测到彩色图像");
                    hImage = BitmapToHObject(bitmap);
                }
                else
                {
                    // 其他格式，尝试转换
                   // Console.WriteLine($"不支持的格式: {bitmap.PixelFormat}，尝试转换");
                    using (Bitmap converted = ConvertTo24bppRgb(bitmap))
                    {
                        hImage = BitmapToHObject(converted);
                    }
                }

                if (hImage == null || !hImage.IsInitialized())
                {
                    AntdUI.Notification.error(this.FindForm(), "图像转换失败", "无法将相机图像转为HALCON格式");
                    return;
                }

                // 2. 用自定义控件显示
                hWindow_Final1.HobjectToHimage(hImage);

                // 3. 获取并显示图像信息
                HTuple width, height, channels;
                HOperatorSet.GetImageSize(hImage, out width, out height);
                HOperatorSet.CountChannels(hImage, out channels);
               // Console.WriteLine($"显示图像: {width.I}x{height.I}, 通道数: {channels.I}");
            }
            catch (Exception ex)
            {
                AntdUI.Notification.error(this.FindForm(), "相机图像显示失败", ex.Message);
              //  Console.WriteLine($"图像显示异常: {ex.Message}");
            }
            finally
            {
                hImage?.Dispose(); // 释放HObject资源
                bitmap.Dispose();  // 释放驱动传递的Bitmap资源
            }
        }
        #endregion

        #region 将图片进行格式转换（用于适配海康控件的图片显示格式）
        /// <summary>
        /// Bitmap转HObject（适配24位RGB和8位灰度，与相机驱动输出格式匹配）
        /// </summary>
        private HObject BitmapToHObject(Bitmap bitmap)
        {
            HObject hImage = null;
            try
            {
               // Console.WriteLine($"转换Bitmap: {bitmap.Width}x{bitmap.Height}, PixelFormat: {bitmap.PixelFormat}");

                // 处理32位ARGB格式：转换为24位RGB格式
                Bitmap bitmapToUse = bitmap;
                bool needDispose = false;

                if (bitmap.PixelFormat == PixelFormat.Format32bppArgb)
                {
                   // Console.WriteLine("转换32位ARGB到24位RGB");
                    // 创建24位RGB格式的Bitmap
                    bitmapToUse = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format24bppRgb);
                    using (Graphics g = Graphics.FromImage(bitmapToUse))
                    {
                        g.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height);
                    }
                    needDispose = true;
                }

                // 锁定Bitmap像素数据
                Rectangle rect = new Rectangle(0, 0, bitmapToUse.Width, bitmapToUse.Height);
                BitmapData bmpData = bitmapToUse.LockBits(
                    rect,
                    ImageLockMode.ReadOnly,
                    bitmapToUse.PixelFormat
                );

              //  Console.WriteLine($"锁定Bitmap数据: {bitmapToUse.PixelFormat}");

                // 根据像素格式创建HObject
                if (bitmapToUse.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    // 彩色图：RGB格式
                   // Console.WriteLine("创建RGB图像");
                    HOperatorSet.GenImageInterleaved(
                        out hImage,
                        new HTuple(bmpData.Scan0),
                        "rgb",
                        bitmapToUse.Width,
                        bitmapToUse.Height,
                        0,
                        "byte",
                        bitmapToUse.Width,
                        bitmapToUse.Height,
                        0, 0, -1, 0
                    );
                }
                else if (bitmapToUse.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    // 灰度图：单通道
                   // Console.WriteLine("创建灰度图像");
                    HOperatorSet.GenImage1(
                        out hImage,
                        "byte",
                        bitmapToUse.Width,
                        bitmapToUse.Height,
                        new HTuple(bmpData.Scan0)
                    );
                }
                else
                {
                    throw new Exception($"不支持的像素格式：{bitmapToUse.PixelFormat}");
                }

                bitmapToUse.UnlockBits(bmpData);

                // 清理临时创建的Bitmap
                if (needDispose)
                {
                    bitmapToUse.Dispose();
                }

                // 验证生成的HObject
                if (hImage != null && hImage.IsInitialized())
                {
                    HTuple width, height;
                    HOperatorSet.GetImageSize(hImage, out width, out height);
                  //  Console.WriteLine($"HObject创建成功: {width.I}x{height.I}");
                }
                else
                {
                    //Console.WriteLine("HObject创建失败");
                }

                return hImage;
            }
            catch (Exception ex)
            {
                AntdUI.Notification.error(this.FindForm(), "格式转换失败", ex.Message);
               // Console.WriteLine($"BitmapToHObject异常: {ex.Message}");
                hImage?.Dispose();
                return null;
            }
        }
        #endregion

        #region 位图格式转换方法
        /// <summary>
        /// 将任意格式的Bitmap转换为24位RGB格式
        /// </summary>
        private Bitmap ConvertTo24bppRgb(Bitmap source)
        {
            try
            {
               // Console.WriteLine($"转换到24位RGB: {source.Width}x{source.Height}, {source.PixelFormat}");

                // 如果已经是24位RGB格式，直接返回副本
                if (source.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    //Console.WriteLine("已经是24位RGB格式");
                    return new Bitmap(source);
                }

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

               // Console.WriteLine("转换完成");
                return target;
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"ConvertTo24bppRgb异常: {ex.Message}");
                throw;
            }
        }
        #endregion

    }
}
