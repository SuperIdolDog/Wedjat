using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wedjat.Driver.Model
{
   
    public class DetectionResultEventArgs : EventArgs
    {
        public HObject OriginalImage { get; set; } // 原始图像
        public HObject MissingRegions { get; set; } // 缺失缺陷区域
        //public HObject ExtraRegions { get; set; } // 多余缺陷区域
        public HTuple MissingCount { get; set; } // 缺失数量
                                                 //  public HTuple ExtraCount { get; set; } // 多余数量
        public string PCBSN { get; set; }
        public HObject AnnotatedImage { get; set; }
        public byte[] AnnotatedImageBytes { get; set; }
    }
       
}

