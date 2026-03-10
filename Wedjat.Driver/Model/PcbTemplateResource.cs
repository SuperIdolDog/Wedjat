using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wedjat.Driver.Model
{
    public class PcbTemplateResource
    {
        public string TemplateName { get; set; } // 模板名称（文件名前缀）
        public HTuple NccModelID { get; set; }   // NCC模板ID（.ncm文件）
        public HObject Region { get; set; }      // 区域文件（.reg文件）
        public HTuple RefRow { get; set; }       // 参考点行（.tup文件）
        public HTuple RefCol { get; set; }       // 参考点列（.tup文件）
        public HTuple RefAngle { get; set; }     // 参考点角度（.tup文件）
    }
}
