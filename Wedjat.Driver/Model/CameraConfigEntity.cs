using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wedjat.Model.Entity
{
    public class CameraConfigEntity
    {
        /// <summary>
        /// 相机序列号（唯一标识）
        /// </summary>
        public string SerialNumber { get; set; } = string.Empty;

        /// <summary>
        /// 曝光时间（ms）
        /// </summary>
        public float ExposureTime { get; set; } = 100;

        /// <summary>
        /// 增益
        /// </summary>
        public float Gain { get; set; } = 0;

        /// <summary>
        /// 帧率
        /// </summary>
        public float FrameRate { get; set; } = 30;

        /// <summary>
        /// 采集模式（连续/触发）
        /// </summary>
        public CameraAcquisitionMode AcquisitionMode { get; set; } = CameraAcquisitionMode.Continuous;

        /// <summary>
        /// 触发模式（软触发/硬触发）
        /// </summary>
        public CameraTriggerMode TriggerMode { get; set; } = CameraTriggerMode.Software;
    }

    public enum CameraAcquisitionMode
    {
        Continuous, // 连续模式
        Trigger     // 触发模式
    }
    public enum CameraTriggerMode
    {
        Software, // 软触发
        Hardware  // 硬触发（Line0）
    }
}
