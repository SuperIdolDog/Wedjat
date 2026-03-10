using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wedjat.Model.Entity
{
    public class ImageDataEntity
    {
        /// <summary>
        /// 图像Bitmap（用于UI显示）
        /// </summary>
        public Bitmap ImageBitmap { get; set; }

        /// <summary>
        /// 帧信息（宽度、高度、像素类型等）
        /// </summary>
        public FrameInfo FrameInfo { get; set; }

        /// <summary>
        /// 采集时间
        /// </summary>
        public DateTime CaptureTime { get; set; } = DateTime.Now;
    }
    public class FrameInfo
    {
        public uint Width { get; set; }
        public uint Height { get; set; }
        public string PixelType { get; set; } // 像素类型（如Mono8、RGB8）
        public uint FrameLength { get; set; } // 帧数据长度
    }
}
