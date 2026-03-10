using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wedjat.Model.DTO;

namespace Wedjat.Model.Config
{
    public class WorkOrderReceivedEventArgs:EventArgs
    {
        /// <summary>
        /// Mes传递的数据
        /// </summary>
        public WorkOrderResponse  WorkOrder { get; set; }

        /// 用户当前选中的PCB（界面动态状态）
        /// </summary>
        public WorkOrderResponse.MesWorkOrderPCB CurrentSelectedPCB { get; set; }
    }
}
