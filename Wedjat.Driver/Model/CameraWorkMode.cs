using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wedjat.Model.Entity
{
    /// <summary>
    /// 相机工作模式
    /// </summary>
    public enum CameraWorkMode
    {
        /// <summary>
        /// 连续采集模式（无触发）
        /// </summary>
        Continuous,
        /// <summary>
        /// 触发采集模式（默认软触发）
        /// </summary>
        Trigger
    }
}
