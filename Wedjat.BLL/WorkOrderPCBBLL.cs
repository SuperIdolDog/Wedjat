using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wedjat.DAL;
using Wedjat.Model.Entity;

namespace Wedjat.BLL
{
    public class WorkOrderPCBBLL : BaseBLL<WorkOrderPCB>
    {
        private readonly WorkOrderPCBDAL _workOrderPCBDAL;
        public WorkOrderPCBBLL() : base()
        {
            _workOrderPCBDAL=new WorkOrderPCBDAL();
        }
    }
}
