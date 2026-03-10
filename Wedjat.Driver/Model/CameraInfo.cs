using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wedjat.Model.Entity
{
    /// <summary>
    /// 相机设备信息模型
    /// </summary>
   public class CameraInfo
    {
        //相机索引
        public int Index {  get; set; }
        //序列号
        public string SerialNumber {  get; set; }
        //设备类型
        public string DeviceType {  get; set; }
    }
}
