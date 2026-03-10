using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wedjat.DAL;
using Wedjat.Model.Entity;

namespace Wedjat.BLL
{
    public class WorkOrderBLL : BaseBLL<WorkOrder>
    {
        private WorkOrderDAL _workOrderDal;
        public WorkOrderBLL():base() 
        {
            _workOrderDal = new WorkOrderDAL();
        }
    }
}
