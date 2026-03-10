using MvCamCtrl.NET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wedjat.Model.Entity;
using static MvCamCtrl.NET.MyCamera;

namespace Wedjat.Driver
{
    // 给海康控件传递参数的委托
    public delegate void ImageEventHandler(Bitmap bitmap);

    public class MVSDriver
    {
        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        private static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        // 给海康控件传递参数的事件
        public event ImageEventHandler ImageLoaded;

        // 设备列表
        private MyCamera.MV_CC_DEVICE_INFO_LIST m_stDeviceList;
        // 相机对象
        private MyCamera m_MyCamera = null;

        // 是否为触发模式
        public bool isTriggerMode { get; private set; }
        // 相机图像
        private Bitmap m_bitmap = null;
        // 读写图像时锁定
        private readonly object BufForDriverLock = new object();
        private readonly object BufForImageLock = new object();

        // 驱动字节大小
        private UInt32 m_nBufSizeForDriver = 0;
        // 用于从驱动获取图像的缓存
        private IntPtr m_BufForDriver = IntPtr.Zero;

        // 转换后图像缓冲区
        private IntPtr m_ConvertDstBuf = IntPtr.Zero;
        private UInt32 m_nConvertDstBufLen = 0;
        private PixelFormat m_bitmapPixelFormat = PixelFormat.DontCare;

        // 取像线程
        private Thread m_hReceiveThread = null;
        // 相机是否连接
        public bool IsConnect { get; private set; } = false;
        // 是否开始采集
        public bool isGrabbing { get; private set; } = false;

        // 帧信息
        private MyCamera.MV_FRAME_OUT_INFO_EX m_stFrameInfo = new MyCamera.MV_FRAME_OUT_INFO_EX();
        // 相机基本信息
        private CameraInfo cameraInfo = new CameraInfo();

        // 显示窗口句柄（如果需要直接显示）
        public IntPtr DisplayHandle { get; set; } = IntPtr.Zero;

        #region 枚举海康工业相机
        /// <summary>
        /// 枚举海康相机(GIGE USB3)
        /// </summary>
        public List<CameraInfo> EnumDevices(out string errorMsg)
        {
            var cameraList = new List<CameraInfo>();

            // 枚举设备
            m_stDeviceList.nDeviceNum = 0;
            int nRet = MV_CC_EnumDevices_NET
                (MV_GIGE_DEVICE | MV_USB_DEVICE,
                ref m_stDeviceList);

            if (nRet != 0)
            {
                errorMsg = "枚举HK相机设备失败";
                return cameraList;
            }
            else
            {
                // 解析设备列表，转换为CameraInfo
                for (int i = 0; i < m_stDeviceList.nDeviceNum; i++)
                {
                    // 获取SDK设备信息结构体
                    var deviceInfo = (MV_CC_DEVICE_INFO)Marshal.PtrToStructure(
                        m_stDeviceList.pDeviceInfo[i],
                        typeof(MV_CC_DEVICE_INFO)
                    );

                    // 解析序列号和设备类型
                    string serialNumber = string.Empty;
                    string deviceType = string.Empty;

                    if (deviceInfo.nTLayerType == MV_GIGE_DEVICE)
                    {
                        // GIGE设备解析
                        var gigeInfo = (MV_GIGE_DEVICE_INFO_EX)ByteToStruct(
                            deviceInfo.SpecialInfo.stGigEInfo,
                            typeof(MV_GIGE_DEVICE_INFO_EX)
                        );
                        serialNumber = gigeInfo.chSerialNumber;
                        deviceType = "GIGE";
                        Console.WriteLine($"GIGE设备[{i}]序列号：{serialNumber}");
                    }
                    else if (deviceInfo.nTLayerType == MV_USB_DEVICE)
                    {
                        // USB设备解析
                        var usbInfo = (MV_USB3_DEVICE_INFO_EX)ByteToStruct(
                            deviceInfo.SpecialInfo.stUsb3VInfo,
                            typeof(MV_USB3_DEVICE_INFO_EX)
                        );
                        serialNumber = usbInfo.chSerialNumber;
                        deviceType = "USB";
                        Console.WriteLine($"USB设备[{i}]序列号：{serialNumber}");
                    }

                    // 添加到列表
                    cameraList.Add(new CameraInfo
                    {
                        Index = i,
                        SerialNumber = serialNumber,
                        DeviceType = deviceType
                    });
                }
            }

            if (cameraList.Count == 0)
            {
                errorMsg = "HK相机为空";
            }
            else
            {
                errorMsg = string.Empty; // 枚举成功，无错误
            }

            // 打印列表中的实际数据
            Console.WriteLine($"枚举到{cameraList.Count}个设备：");
            foreach (var cam in cameraList)
            {
                Console.WriteLine($"cameraList[{cam.Index}]：{cam.SerialNumber}");
            }

            return cameraList;
        }
        #endregion

        #region 获取相机序列号
        private int GetDeviceIndex(string CameraID)
        {
            for (int i = 0; i < m_stDeviceList.nDeviceNum; i++)
            {
                MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(
                    m_stDeviceList.pDeviceInfo[i],
                    typeof(MyCamera.MV_CC_DEVICE_INFO));

                if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                {
                    MyCamera.MV_GIGE_DEVICE_INFO_EX gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO_EX)
                        MyCamera.ByteToStruct(device.SpecialInfo.stGigEInfo,
                        typeof(MyCamera.MV_GIGE_DEVICE_INFO_EX));
                    if (gigeInfo.chSerialNumber == CameraID)
                        return i;
                }
                else if (device.nTLayerType == MyCamera.MV_USB_DEVICE)
                {
                    MyCamera.MV_USB3_DEVICE_INFO_EX usb3Info = (MyCamera.MV_USB3_DEVICE_INFO_EX)
                        MyCamera.ByteToStruct(device.SpecialInfo.stUsb3VInfo,
                        typeof(MyCamera.MV_USB3_DEVICE_INFO_EX));
                    if (usb3Info.chSerialNumber == CameraID)
                        return i;
                }
            }
            return -1;
        }
        #endregion

        #region 打开相机
        public bool OpenDevice(string serial)
        {
            // 打开相机流程
            // 1.获取所有相机
            // 2.根据相机序列号，看该序列号对应的相机id是否存在，确定该相机是否存在
            int camIdx = GetDeviceIndex(serial);

            if (camIdx == -1)
            {
                Console.WriteLine("找不到该序列号的相机");
                return false;
            }

            // 3.获取相机信息
            MyCamera.MV_CC_DEVICE_INFO device =
               (MyCamera.MV_CC_DEVICE_INFO)
               Marshal.PtrToStructure(m_stDeviceList.pDeviceInfo[camIdx],
               typeof(MyCamera.MV_CC_DEVICE_INFO));

            // 4.建立设备对象
            if (null == m_MyCamera)
            {
                // 创建相机实例
                m_MyCamera = new MyCamera();
                if (null == m_MyCamera)
                {
                    Console.WriteLine("相机初始化失败");
                    return false;
                }
            }

            // 5.根据相机信息创建相机
            int nRet = m_MyCamera.MV_CC_CreateDevice_NET(ref device);
            if (MyCamera.MV_OK != nRet)
            {
                Console.WriteLine($"创建设备失败,失败代码:{nRet}");
                return false;
            }

            // 6.打开设备
            nRet = m_MyCamera.MV_CC_OpenDevice_NET();
            // 如果打开失败，则销毁相机设备
            if (MyCamera.MV_OK != nRet)
            {
                m_MyCamera.MV_CC_DestroyDevice_NET();
                Console.WriteLine($"创建设备失败,失败代码:{nRet}");
                return false;
            }

            IsConnect = true;

            // 7.探测网络最佳包大小,并进行设置(只对GigE相机有效)
            if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
            {
                // 最优网络包大小
                int nPacketSize = m_MyCamera.MV_CC_GetOptimalPacketSize_NET();
                if (nPacketSize > 0)
                {
                    nRet = m_MyCamera.MV_CC_SetIntValueEx_NET("GevSCPSPacketSize", (uint)nPacketSize);
                    if (nRet != MyCamera.MV_OK)
                    {
                        Console.WriteLine($"设置包大小失败，失败代码{nRet}");
                    }
                }
                else
                {
                    Console.WriteLine($"设置包大小失败，失败代码{nPacketSize}");
                }
            }

            // 8.设置为连续采集模式
            m_MyCamera.MV_CC_SetEnumValue_NET("AcquisitionMode",
                (uint)MyCamera.MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS);
            m_MyCamera.MV_CC_SetEnumValue_NET("TriggerMode",
                (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);

            // 9.尝试设置相机为彩色格式
            TrySetColorFormat();

            // 10.注册异常回调函数
            m_MyCamera.MV_CC_RegisterExceptionCallBack_NET(cbException, IntPtr.Zero);

            return true;
        }
        #endregion

        #region 关闭相机
        public void CloseDevice()
        {
            // 取流标志位清零
            if (isGrabbing)
            {
                StopGrab();
            }

            if (m_BufForDriver != IntPtr.Zero)
            {
                Marshal.Release(m_BufForDriver);
                m_BufForDriver = IntPtr.Zero;
            }

            if (m_ConvertDstBuf != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(m_ConvertDstBuf);
                m_ConvertDstBuf = IntPtr.Zero;
            }

            if (m_bitmap != null)
            {
                m_bitmap.Dispose();
                m_bitmap = null;
            }

            if (m_MyCamera != null)
            {
                m_MyCamera.MV_CC_CloseDevice_NET();
                m_MyCamera.MV_CC_DestroyDevice_NET();
                m_MyCamera = null;
            }

            IsConnect = false;
        }
        #endregion

        #region 硬触发设置
        public bool SetHardwareTrigger(string lineName = "Line0", int edgeType = 0, uint debounceTime = 250)
        {
            try
            {
                // 设置触发模式为ON
                int nRet = m_MyCamera.MV_CC_SetEnumValue_NET("TriggerMode",
                    (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON);
                if (nRet != MyCamera.MV_OK)
                {
                    Console.WriteLine($"设置触发模式失败，错误码: {nRet}");
                    return false;
                }

                // 设置触发源为线路输入
                uint triggerSource = GetTriggerSourceFromLineName(lineName);
                if (triggerSource == uint.MaxValue)
                {
                    Console.WriteLine($"不支持的触发线路: {lineName}");
                    return false;
                }

                nRet = m_MyCamera.MV_CC_SetEnumValue_NET("TriggerSource", triggerSource);
                if (nRet != MyCamera.MV_OK)
                {
                    Console.WriteLine($"设置触发源失败，错误码: {nRet}");
                    return false;
                }

                // 设置触发边沿
                nRet = m_MyCamera.MV_CC_SetEnumValue_NET("TriggerActivation", (uint)edgeType);
                if (nRet != MyCamera.MV_OK)
                {
                    Console.WriteLine($"设置触发边沿失败，错误码: {nRet}");
                    return false;
                }

                // 设置线路防抖时间
                nRet = m_MyCamera.MV_CC_SetIntValue_NET("LineDebouncerTime", debounceTime);
                if (nRet != MyCamera.MV_OK)
                {
                    Console.WriteLine($"设置防抖时间失败，错误码: {nRet}（可选参数）");
                }

                // 设置触发延迟（可选）
                nRet = m_MyCamera.MV_CC_SetFloatValue_NET("TriggerDelay", 0.0f);
                if (nRet != MyCamera.MV_OK)
                {
                    Console.WriteLine($"设置触发延迟失败，错误码: {nRet}（可选参数）");
                }

                isTriggerMode = true;
                Console.WriteLine($"硬触发设置成功 - 线路:{lineName}, 边沿:{edgeType}, 防抖:{debounceTime}μs");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"设置硬触发异常: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 根据线路名称获取触发源枚举值
        /// </summary>
        private uint GetTriggerSourceFromLineName(string lineName)
        {
            switch (lineName.ToLower())
            {
                case "line0":
                    return (uint)MyCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE0;
                case "line1":
                    return (uint)MyCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE1;
                case "line2":
                    return (uint)MyCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE2;
                case "line3":
                    return (uint)MyCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE3;
                default:
                    return uint.MaxValue;
            }
        }
        #endregion

        #region 设置触发模式
        public bool SetMode(CameraWorkMode mode, string triggerType = "Software", string lineName = "Line0", int edgeType = 0)
        {
            if (mode == CameraWorkMode.Continuous)
            {
                // 设置连续模式
                int nRet = m_MyCamera.MV_CC_SetEnumValue_NET("TriggerMode",
                    (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);
                if (nRet == MyCamera.MV_OK)
                {
                    isTriggerMode = false;
                }
                return true;
            }
            // 触发模式
            else if (mode == CameraWorkMode.Trigger)
            {
                if (triggerType.ToLower() == "hardware")
                {
                    // 硬触发
                    return SetHardwareTrigger(lineName, edgeType, 250);
                }
                else
                {
                    // 软触发
                    int nRet = m_MyCamera.MV_CC_SetEnumValue_NET("TriggerMode",
                      (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON);
                    if (nRet == MyCamera.MV_OK)
                    {
                        isTriggerMode = true;
                    }
                    // 设置软触发源
                    m_MyCamera.MV_CC_SetEnumValue_NET("TriggerSource",
                        (uint)MyCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE);
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region 开始采集前的必要操作（Demo方法）
        /// <summary>
        /// 开始采集前的必要操作（仿照Demo）
        /// </summary>
        private bool NecessaryOperBeforeGrab()
        {
            try
            {
                // 获取图像宽
                MyCamera.MVCC_INTVALUE_EX stWidth = new MyCamera.MVCC_INTVALUE_EX();
                int nRet = m_MyCamera.MV_CC_GetIntValueEx_NET("Width", ref stWidth);
                if (MyCamera.MV_OK != nRet)
                {
                    Console.WriteLine($"获取图像宽度失败，错误码: {nRet}");
                    return false;
                }

                // 获取图像高
                MyCamera.MVCC_INTVALUE_EX stHeight = new MyCamera.MVCC_INTVALUE_EX();
                nRet = m_MyCamera.MV_CC_GetIntValueEx_NET("Height", ref stHeight);
                if (MyCamera.MV_OK != nRet)
                {
                    Console.WriteLine($"获取图像高度失败，错误码: {nRet}");
                    return false;
                }

                // 获取像素格式
                MyCamera.MVCC_ENUMVALUE stPixelFormat = new MyCamera.MVCC_ENUMVALUE();
                nRet = m_MyCamera.MV_CC_GetEnumValue_NET("PixelFormat", ref stPixelFormat);
                if (MyCamera.MV_OK != nRet)
                {
                    Console.WriteLine($"获取像素格式失败，错误码: {nRet}");
                    return false;
                }

                Console.WriteLine($"相机信息: 宽度={stWidth.nCurValue}, 高度={stHeight.nCurValue}, 像素格式={stPixelFormat.nCurValue}");

                // 判断是否为单色格式
                bool isMono = IsMono(stPixelFormat.nCurValue);

                if (isMono)
                {
                    m_bitmapPixelFormat = PixelFormat.Format8bppIndexed;

                    // 释放之前的转换缓冲区
                    if (m_ConvertDstBuf != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(m_ConvertDstBuf);
                        m_ConvertDstBuf = IntPtr.Zero;
                    }

                    // Mono8为单通道，每个像素1字节
                    m_nConvertDstBufLen = (UInt32)(stWidth.nCurValue * stHeight.nCurValue);
                    m_ConvertDstBuf = Marshal.AllocHGlobal((Int32)m_nConvertDstBufLen);

                    if (IntPtr.Zero == m_ConvertDstBuf)
                    {
                        Console.WriteLine("分配单色图像缓冲区失败");
                        return false;
                    }

                    Console.WriteLine($"创建单色图像缓冲区: {m_nConvertDstBufLen} 字节");
                }
                else
                {
                    // 彩色图像使用BGR8格式（Windows Bitmap格式）
                    m_bitmapPixelFormat = PixelFormat.Format24bppRgb;

                    // 释放之前的转换缓冲区
                    if (m_ConvertDstBuf != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(m_ConvertDstBuf);
                        m_ConvertDstBuf = IntPtr.Zero;
                    }

                    // BGR8为三通道，每个像素3字节
                    m_nConvertDstBufLen = (UInt32)(3 * stWidth.nCurValue * stHeight.nCurValue);
                    m_ConvertDstBuf = Marshal.AllocHGlobal((Int32)m_nConvertDstBufLen);

                    if (IntPtr.Zero == m_ConvertDstBuf)
                    {
                        Console.WriteLine("分配彩色图像缓冲区失败");
                        return false;
                    }

                    Console.WriteLine($"创建彩色图像缓冲区: {m_nConvertDstBufLen} 字节");
                }

                // 创建新的Bitmap
                if (m_bitmap != null)
                {
                    m_bitmap.Dispose();
                    m_bitmap = null;
                }

                m_bitmap = new Bitmap((Int32)stWidth.nCurValue, (Int32)stHeight.nCurValue, m_bitmapPixelFormat);

                // 如果是8位灰度图，设置调色板
                if (PixelFormat.Format8bppIndexed == m_bitmapPixelFormat)
                {
                    ColorPalette palette = m_bitmap.Palette;
                    for (int i = 0; i < palette.Entries.Length; i++)
                    {
                        palette.Entries[i] = Color.FromArgb(i, i, i);
                    }
                    m_bitmap.Palette = palette;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"开始采集前准备失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 判断是否为单色格式（Demo方法）
        /// </summary>
        private bool IsMono(UInt32 enPixelType)
        {
            switch (enPixelType)
            {
                case (UInt32)MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono1p:
                case (UInt32)MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono2p:
                case (UInt32)MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono4p:
                case (UInt32)MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8:
                case (UInt32)MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8_Signed:
                case (UInt32)MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10:
                case (UInt32)MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10_Packed:
                case (UInt32)MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12:
                case (UInt32)MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12_Packed:
                case (UInt32)MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono14:
                case (UInt32)MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono16:
                    return true;
                default:
                    return false;
            }
        }
        #endregion

        #region 开始采集（使用Demo方法）
        public bool StartGrab()
        {
            if (m_MyCamera == null)
            {
                Console.WriteLine("错误：相机未打开");
                return false;
            }

            if (isGrabbing)
            {
                Console.WriteLine("已在采集状态");
                return false;
            }

            // 1. 开始采集前的必要操作
            if (!NecessaryOperBeforeGrab())
            {
                Console.WriteLine("开始采集前准备失败");
                return false;
            }

            // 2. 标志位置true
            isGrabbing = true;

            // 3. 启动取像线程
            m_hReceiveThread = new Thread(ReceiveThreadProcess);
            m_hReceiveThread.Start();

            // 4. 开始采集
            int nRet = m_MyCamera.MV_CC_StartGrabbing_NET();
            if (MyCamera.MV_OK != nRet)
            {
                isGrabbing = false;
                if (m_hReceiveThread != null && m_hReceiveThread.IsAlive)
                {
                    m_hReceiveThread.Join();
                }
                Console.WriteLine($"开始采集失败，错误码: {nRet}");
                return false;
            }

            Console.WriteLine("开始采集成功");
            return true;
        }

        // 兼容旧方法名
        public bool StratGrab()
        {
            return StartGrab();
        }
        #endregion

        #region 取像线程（核心 - 使用Demo方法）
        private void ReceiveThreadProcess()
        {
            MyCamera.MV_FRAME_OUT stFrameOut = new MyCamera.MV_FRAME_OUT();
            MyCamera.MV_PIXEL_CONVERT_PARAM stConvertParam = new MyCamera.MV_PIXEL_CONVERT_PARAM();

            while (isGrabbing)
            {
                try
                {
                    // 获取图像缓冲区
                    int nRet = m_MyCamera.MV_CC_GetImageBuffer_NET(ref stFrameOut, 1000);

                    if (nRet == MyCamera.MV_OK)
                    {
                        lock (BufForDriverLock)
                        {
                            // 更新帧信息
                            m_stFrameInfo = stFrameOut.stFrameInfo;

                            Console.WriteLine($"获取图像成功: 格式={m_stFrameInfo.enPixelType}, " +
                                $"宽度={m_stFrameInfo.nWidth}, 高度={m_stFrameInfo.nHeight}, " +
                                $"帧号={m_stFrameInfo.nFrameNum}");

                            // 准备图像缓冲区
                            if (m_BufForDriver == IntPtr.Zero || m_stFrameInfo.nFrameLen > m_nBufSizeForDriver)
                            {
                                if (m_BufForDriver != IntPtr.Zero)
                                {
                                    Marshal.Release(m_BufForDriver);
                                    m_BufForDriver = IntPtr.Zero;
                                }

                                m_nBufSizeForDriver = m_stFrameInfo.nFrameLen;
                                m_BufForDriver = Marshal.AllocHGlobal((Int32)m_nBufSizeForDriver);

                                if (m_BufForDriver == IntPtr.Zero)
                                {
                                    Console.WriteLine("分配图像缓冲区失败");
                                    m_MyCamera.MV_CC_FreeImageBuffer_NET(ref stFrameOut);
                                    continue;
                                }
                            }

                            // 复制图像数据
                            CopyMemory(m_BufForDriver, stFrameOut.pBufAddr, m_stFrameInfo.nFrameLen);

                            // 像素格式转换
                            stConvertParam.nWidth = m_stFrameInfo.nWidth;
                            stConvertParam.nHeight = m_stFrameInfo.nHeight;
                            stConvertParam.enSrcPixelType = m_stFrameInfo.enPixelType;
                            stConvertParam.pSrcData = m_BufForDriver;
                            stConvertParam.nSrcDataLen = m_stFrameInfo.nFrameLen;
                            stConvertParam.pDstBuffer = m_ConvertDstBuf;
                            stConvertParam.nDstBufferSize = m_nConvertDstBufLen;

                            // 根据像素格式设置目标格式
                            if (PixelFormat.Format8bppIndexed == m_bitmapPixelFormat)
                            {
                                stConvertParam.enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8;
                            }
                            else
                            {
                                // 彩色图像转换为BGR8（Windows Bitmap格式）
                                stConvertParam.enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_BGR8_Packed;
                            }

                            // 执行像素格式转换
                            nRet = m_MyCamera.MV_CC_ConvertPixelType_NET(ref stConvertParam);
                            if (nRet != MyCamera.MV_OK)
                            {
                                Console.WriteLine($"像素格式转换失败: {nRet}");
                                m_MyCamera.MV_CC_FreeImageBuffer_NET(ref stFrameOut);
                                continue;
                            }

                            // 保存Bitmap数据
                            lock (BufForImageLock)
                            {
                                if (m_bitmap != null)
                                {
                                    BitmapData bitmapData = m_bitmap.LockBits(
                                        new Rectangle(0, 0, m_bitmap.Width, m_bitmap.Height),
                                        ImageLockMode.WriteOnly,
                                        m_bitmap.PixelFormat);

                                    CopyMemory(bitmapData.Scan0, m_ConvertDstBuf,
                                        (uint)(bitmapData.Stride * m_bitmap.Height));

                                    m_bitmap.UnlockBits(bitmapData);

                                    // 触发图像加载事件
                                    if (ImageLoaded != null)
                                    {
                                        // 创建副本避免线程问题
                                        Bitmap copy = new Bitmap(m_bitmap);
                                        ImageLoaded.Invoke(copy);
                                    }
                                }
                            }

                            // 如果需要直接显示到窗口
                            if (DisplayHandle != IntPtr.Zero)
                            {
                                MyCamera.MV_DISPLAY_FRAME_INFO stDisplayInfo = new MyCamera.MV_DISPLAY_FRAME_INFO();
                                stDisplayInfo.hWnd = DisplayHandle;
                                stDisplayInfo.pData = m_BufForDriver;
                                stDisplayInfo.nDataLen = m_stFrameInfo.nFrameLen;
                                stDisplayInfo.nWidth = m_stFrameInfo.nWidth;
                                stDisplayInfo.nHeight = m_stFrameInfo.nHeight;
                                stDisplayInfo.enPixelType = m_stFrameInfo.enPixelType;
                                m_MyCamera.MV_CC_DisplayOneFrame_NET(ref stDisplayInfo);
                            }
                        }

                        // 释放图像缓冲区
                        m_MyCamera.MV_CC_FreeImageBuffer_NET(ref stFrameOut);
                    }
                    else if (nRet == MyCamera.MV_E_ABNORMAL_IMAGE)
                    {
                        // 异常图像，继续尝试
                        continue;
                    }
                    else if (IsTimeoutError(nRet))
                    {
                        // 超时错误，继续尝试
                        if (isTriggerMode)
                        {
                            Thread.Sleep(5);
                        }
                        continue;
                    }
                    else
                    {
                        // 其他错误
                        Console.WriteLine($"获取图像失败，错误码: {nRet}");
                        if (isTriggerMode)
                        {
                            Thread.Sleep(5);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"取像线程异常: {ex.Message}");
                    if (isGrabbing)
                    {
                        Thread.Sleep(100);
                    }
                }
            }

            Console.WriteLine("取像线程结束");
        }

        /// <summary>
        /// 判断是否为超时错误
        /// </summary>
        private bool IsTimeoutError(int errorCode)
        {
            // 海康SDK的超时错误码可能是-14，但不同版本可能不同
            // 这里我们检查常见的超时错误码
            return errorCode == -14 || errorCode == -7; // -7通常是MV_E_TIMEOUT，-14是另一种表示
        }
        #endregion

        #region 抓拍
        public bool SnapImage()
        {
            if (m_MyCamera == null)
            {
                Console.WriteLine("相机未打开");
                return false;
            }

            // 触发命令
            int nRet = m_MyCamera.MV_CC_SetCommandValue_NET("TriggerSoftware");
            if (MyCamera.MV_OK != nRet)
            {
                Console.WriteLine($"单张采集触发失败,失败代码:{nRet}");
                return false;
            }
            return true;
        }
        #endregion

        #region 停止采集
        public bool StopGrab()
        {
            if (!isGrabbing)
            {
                return true;
            }

            // 标志位设置为false
            isGrabbing = false;

            // 停止采集
            int nRet = m_MyCamera.MV_CC_StopGrabbing_NET();
            if (nRet != MyCamera.MV_OK)
            {
                Console.WriteLine($"停止采集失败,失败代码:{nRet}");
            }

            // 等待取像线程结束
            if (m_hReceiveThread != null && m_hReceiveThread.IsAlive)
            {
                m_hReceiveThread.Join(1000);
                if (m_hReceiveThread.IsAlive)
                {
                    try
                    {
                        m_hReceiveThread.Abort();
                    }
                    catch { }
                }
                m_hReceiveThread = null;
            }

            Console.WriteLine("停止采集成功");
            return true;
        }
        #endregion

        #region 设备状态异常回调
        private void cbException(uint nMsgType, IntPtr pUser)
        {
            Console.WriteLine($"设备异常: {nMsgType}");

            if (nMsgType == MyCamera.MV_EXCEPTION_DEV_DISCONNECT)
            {
                // 设备断开连接
                IsConnect = false;
                isGrabbing = false;

                // 关闭设备
                CloseDevice();

                Console.WriteLine("设备断开连接");
            }
        }
        #endregion

        #region 曝光
        public float GetExposure()
        {
            if (m_MyCamera == null || !IsConnect)
                return 0;

            MyCamera.MVCC_FLOATVALUE stParam = new MyCamera.MVCC_FLOATVALUE();
            int nRet = m_MyCamera.MV_CC_GetFloatValue_NET("ExposureTime", ref stParam);
            if (MyCamera.MV_OK == nRet)
            {
                return stParam.fCurValue;
            }
            return 0;
        }

        public bool SetExposure(float exposureTime)
        {
            if (m_MyCamera == null || !IsConnect)
                return false;

            int nRet = m_MyCamera.MV_CC_SetFloatValue_NET("ExposureTime", exposureTime);
            return nRet == MyCamera.MV_OK;
        }
        #endregion

        #region 帧率
        public float GetFrameRate()
        {
            if (m_MyCamera == null || !IsConnect)
                return 0;

            MyCamera.MVCC_FLOATVALUE stParam = new MyCamera.MVCC_FLOATVALUE();
            int nRet = m_MyCamera.MV_CC_GetFloatValue_NET("ResultingFrameRate", ref stParam);
            if (MyCamera.MV_OK == nRet)
            {
                return stParam.fCurValue;
            }
            return 0;
        }

        public bool SetFrameRate(float frameRate)
        {
            if (m_MyCamera == null || !IsConnect)
                return false;

            int nRet = m_MyCamera.MV_CC_SetBoolValue_NET("AcquisitionFrameRateEnable", true);
            if (nRet != MyCamera.MV_OK)
            {
                return false;
            }

            nRet = m_MyCamera.MV_CC_SetFloatValue_NET("AcquisitionFrameRate", frameRate);
            return nRet == MyCamera.MV_OK;
        }
        #endregion

        #region 增益
        public float GetGain()
        {
            if (m_MyCamera == null || !IsConnect)
                return 0;

            MyCamera.MVCC_FLOATVALUE stParam = new MyCamera.MVCC_FLOATVALUE();
            int nRet = m_MyCamera.MV_CC_GetFloatValue_NET("Gain", ref stParam);
            if (MyCamera.MV_OK == nRet)
            {
                return stParam.fCurValue;
            }
            return 0;
        }

        public bool SetGain(float gain)
        {
            if (m_MyCamera == null || !IsConnect)
                return false;

            int nRet = m_MyCamera.MV_CC_SetFloatValue_NET("Gain", gain);
            return nRet == MyCamera.MV_OK;
        }
        #endregion

        #region 获取最新采集的图像
        public Bitmap GetLatestFrame()
        {
            lock (BufForImageLock)
            {
                if (m_bitmap == null)
                    return null;

                // 返回副本，避免检测时修改原始图像
                return new Bitmap(m_bitmap);
            }
        }
        #endregion

        #region 尝试设置彩色格式
        private void TrySetColorFormat()
        {
            try
            {
                Console.WriteLine("尝试设置相机为彩色格式...");

                // 获取相机支持的格式
                var formats = GetImageFormats();
                Console.WriteLine("相机支持的格式:");
                foreach (var format in formats)
                {
                    Console.WriteLine($"  - {format}");
                }

                // 优先尝试设置BayerRG8格式（彩色相机常用）
                if (formats.Contains("BayerRG8"))
                {
                    Console.WriteLine("尝试设置格式为 BayerRG8");
                    SetImageFormat("BayerRG8");
                }
                else if (formats.Contains("RGB8"))
                {
                    Console.WriteLine("尝试设置格式为 RGB8");
                    SetImageFormat("RGB8");
                }
                else if (formats.Contains("BayerBG8"))
                {
                    Console.WriteLine("尝试设置格式为 BayerBG8");
                    SetImageFormat("BayerBG8");
                }
                else if (formats.Contains("BGR8"))
                {
                    Console.WriteLine("尝试设置格式为 BGR8");
                    SetImageFormat("BGR8");
                }

                // 获取并显示当前格式
                string currentFormat = GetCurrentFormat();
                Console.WriteLine($"当前相机格式: {currentFormat}");

                // 判断相机类型
                if (currentFormat.Contains("Mono") || currentFormat.Contains("MONO"))
                {
                    Console.WriteLine("警告：相机当前设置为黑白格式！");
                    Console.WriteLine("请检查相机是否为彩色相机硬件");
                }
                else if (currentFormat.Contains("Bayer") || currentFormat.Contains("RGB") ||
                         currentFormat.Contains("BGR") || currentFormat.Contains("YUV"))
                {
                    Console.WriteLine("相机已设置为彩色格式");
                }

                // 获取传感器类型（如果支持）
                try
                {
                    MyCamera.MVCC_ENUMVALUE sensorType = new MyCamera.MVCC_ENUMVALUE();
                    int nRet = m_MyCamera.MV_CC_GetEnumValue_NET("SensorType", ref sensorType);
                    if (nRet == MyCamera.MV_OK)
                    {
                        Console.WriteLine($"传感器类型代码: {sensorType.nCurValue}");
                        // 通常0表示彩色，1表示黑白
                        if (sensorType.nCurValue == 1)
                        {
                            Console.WriteLine("警告：相机硬件是黑白传感器，无法输出彩色图像！");
                        }
                        else if (sensorType.nCurValue == 0)
                        {
                            Console.WriteLine("传感器类型: 彩色");
                        }
                        else
                        {
                            Console.WriteLine($"未知传感器类型: {sensorType.nCurValue}");
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("无法获取传感器类型信息");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"设置彩色格式失败: {ex.Message}");
            }
        }
        #endregion

        #region 图像格式设置
        /// <summary>
        /// 获取相机支持的图像格式列表
        /// </summary>
        public List<string> GetImageFormats()
        {
            var formats = new List<string>();

            if (m_MyCamera == null || !IsConnect)
                return formats;

            try
            {
                // 获取当前像素格式
                MyCamera.MVCC_ENUMVALUE stParam = new MyCamera.MVCC_ENUMVALUE();
                int nRet = m_MyCamera.MV_CC_GetEnumValue_NET("PixelFormat", ref stParam);

                if (nRet == MyCamera.MV_OK)
                {
                    Console.WriteLine($"当前像素格式: {stParam.nCurValue}");
                    Console.WriteLine($"支持的枚举值数量: {stParam.nSupportedNum}");

                    // 遍历所有支持的像素格式值
                    for (uint i = 0; i < stParam.nSupportedNum; i++)
                    {
                        try
                        {
                            uint pixelFormatValue = stParam.nSupportValue[i];
                            string formatName = GetPixelFormatName(pixelFormatValue);
                            if (!string.IsNullOrEmpty(formatName) && formatName != "Unknown")
                            {
                                Console.WriteLine($"支持格式[{i}]: {formatName} (值: {pixelFormatValue})");
                                if (!formats.Contains(formatName))
                                {
                                    formats.Add(formatName);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"获取枚举项 {i} 失败: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取图像格式失败: {ex.Message}");
            }

            return formats;
        }

        /// <summary>
        /// 设置图像格式
        /// </summary>
        public bool SetImageFormat(string format)
        {
            if (m_MyCamera == null || !IsConnect)
                return false;

            try
            {
                // 根据格式名称设置对应的枚举值
                uint pixelFormatValue = GetPixelFormatValue(format);

                if (pixelFormatValue == 0)
                {
                    Console.WriteLine($"不支持的图像格式: {format}");
                    return false;
                }

                int nRet = m_MyCamera.MV_CC_SetEnumValue_NET("PixelFormat", pixelFormatValue);

                if (nRet == MyCamera.MV_OK)
                {
                    Console.WriteLine($"图像格式已设置为: {format}");
                    return true;
                }
                else
                {
                    Console.WriteLine($"设置图像格式失败: {nRet}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"设置图像格式异常: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 获取当前图像格式
        /// </summary>
        public string GetCurrentFormat()
        {
            if (m_MyCamera == null || !IsConnect)
                return string.Empty;

            try
            {
                MyCamera.MVCC_ENUMVALUE stParam = new MyCamera.MVCC_ENUMVALUE();
                int nRet = m_MyCamera.MV_CC_GetEnumValue_NET("PixelFormat", ref stParam);

                if (nRet == MyCamera.MV_OK)
                {
                    return GetPixelFormatName(stParam.nCurValue);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取当前格式失败: {ex.Message}");
            }

            return string.Empty;
        }

        private uint GetPixelFormatValue(string formatName)
        {
            // 根据格式名称返回对应的枚举值（不区分大小写）
            string upperFormat = formatName.ToUpper();

            switch (upperFormat)
            {
                // 单色（灰度）格式
                case "MONO8":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8;
                case "MONO10":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10;
                case "MONO10_PACKED":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10_Packed;
                case "MONO12":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12;
                case "MONO12_PACKED":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12_Packed;
                case "MONO16":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono16;

                // RGB格式
                case "RGB8":
                case "RGB8_PACKED":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed;
                case "RGB10":
                case "RGB10_PACKED":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB10_Packed;
                case "RGB12":
                case "RGB12_PACKED":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB12_Packed;

                // BGR格式
                case "BGR8":
                case "BGR8_PACKED":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_BGR8_Packed;
                case "BGR10":
                case "BGR10_PACKED":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_BGR10_Packed;
                case "BGR12":
                case "BGR12_PACKED":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_BGR12_Packed;

                // Bayer RG格式
                case "BAYERRG8":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG8;
                case "BAYERRG10":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10;
                case "BAYERRG10_PACKED":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10_Packed;
                case "BAYERRG12":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12;
                case "BAYERRG12_PACKED":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12_Packed;

                // Bayer GR格式
                case "BAYERGR8":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR8;
                case "BAYERGR10":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10;
                case "BAYERGR10_PACKED":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10_Packed;
                case "BAYERGR12":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12;
                case "BAYERGR12_PACKED":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12_Packed;

                // Bayer GB格式
                case "BAYERGB8":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB8;
                case "BAYERGB10":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10;
                case "BAYERGB10_PACKED":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10_Packed;
                case "BAYERGB12":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12;
                case "BAYERGB12_PACKED":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12_Packed;

                // Bayer BG格式
                case "BAYERBG8":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG8;
                case "BAYERBG10":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10;
                case "BAYERBG10_PACKED":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10_Packed;
                case "BAYERBG12":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12;
                case "BAYERBG12_PACKED":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12_Packed;

                // YUV格式
                case "YUV422_PACKED":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_Packed;
                case "YUV422_YUYV_PACKED":
                    return (uint)MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_YUYV_Packed;

                // 如果没有匹配，尝试解析为数字
                default:
                    // 尝试将格式名称作为数字解析
                    if (uint.TryParse(formatName, out uint numericValue))
                    {
                        return numericValue;
                    }

                    Console.WriteLine($"未知的像素格式: {formatName}");
                    return 0; // 返回0表示未知格式
            }
        }

        private string GetPixelFormatName(uint pixelFormat)
        {
            // 根据枚举值返回格式名称
            MyCamera.MvGvspPixelType pixelType = (MyCamera.MvGvspPixelType)pixelFormat;

            switch (pixelType)
            {
                // 单色（灰度）格式
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8:
                    return "Mono8";
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10:
                    return "Mono10";
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10_Packed:
                    return "Mono10_Packed";
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12:
                    return "Mono12";
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12_Packed:
                    return "Mono12_Packed";
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono16:
                    return "Mono16";

                // RGB格式
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed:
                    return "RGB8";
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB10_Packed:
                    return "RGB10";
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB12_Packed:
                    return "RGB12";

                // BGR格式
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BGR8_Packed:
                    return "BGR8";
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BGR10_Packed:
                    return "BGR10";
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BGR12_Packed:
                    return "BGR12";

                // Bayer RG格式
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG8:
                    return "BayerRG8";
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10:
                    return "BayerRG10";
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10_Packed:
                    return "BayerRG10_Packed";
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12:
                    return "BayerRG12";
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12_Packed:
                    return "BayerRG12_Packed";

                // Bayer GR格式
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR8:
                    return "BayerGR8";
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10:
                    return "BayerGR10";
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10_Packed:
                    return "BayerGR10_Packed";
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12:
                    return "BayerGR12";
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12_Packed:
                    return "BayerGR12_Packed";

                // Bayer GB格式
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB8:
                    return "BayerGB8";
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10:
                    return "BayerGB10";
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10_Packed:
                    return "BayerGB10_Packed";
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12:
                    return "BayerGB12";
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12_Packed:
                    return "BayerGB12_Packed";

                // Bayer BG格式
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG8:
                    return "BayerBG8";
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10:
                    return "BayerBG10";
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10_Packed:
                    return "BayerBG10_Packed";
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12:
                    return "BayerBG12";
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12_Packed:
                    return "BayerBG12_Packed";

                // YUV格式
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_Packed:
                    return "YUV422_Packed";
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_YUYV_Packed:
                    return "YUV422_YUYV_Packed";

                // 未知格式
                default:
                    // 尝试获取枚举名称
                    string enumName = Enum.GetName(typeof(MyCamera.MvGvspPixelType), pixelFormat);
                    if (!string.IsNullOrEmpty(enumName))
                    {
                        // 从枚举名称中提取格式名称（移除前缀）
                        if (enumName.StartsWith("PixelType_Gvsp_"))
                        {
                            return enumName.Substring("PixelType_Gvsp_".Length);
                        }
                        return enumName;
                    }
                    return $"未知格式({pixelFormat})";
            }
        }
        #endregion

        #region 辅助方法：ByteToStruct
        private object ByteToStruct(byte[] bytes, Type type)
        {
            int size = Marshal.SizeOf(type);
            if (size > bytes.Length)
            {
                return null;
            }

            IntPtr structPtr = Marshal.AllocHGlobal(size);
            Marshal.Copy(bytes, 0, structPtr, size);
            object obj = Marshal.PtrToStructure(structPtr, type);
            Marshal.FreeHGlobal(structPtr);
            return obj;
        }
        #endregion
    }
}