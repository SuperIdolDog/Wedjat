using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wedjat.Model;
using Wedjat.Model.DTO;
using Wedjat.Model.Entity;

namespace Wedjat.DAL
{
    public class WorkOrderDAL : BaseDAL<WorkOrder>
    {
        public WorkOrderDAL() : base(AppDbContext.Sqlite)
        {
        }
        #region 添加扫码枪查询出来的工单
        public async Task<WorkOrder> AddWorkOrder(WorkOrderResponse dto)
        {
            var workOrder = new WorkOrder
            {
                WorkOrderCode = dto.WorkOrderCode,
                OrderStatus = dto.OrderStatus,
                PlanQuantity = dto.WorkOrderPCBs.Sum(wp => wp.PlanQuantity),
                CompleteQuantity = dto.WorkOrderPCBs.Sum(wp => wp.CompleteQuantity ?? 0),
               
            };
            long insertResult = await InsertModel(workOrder);
            if (insertResult < 0)
            {
                throw new Exception("获取的工单信息插入数据库失败");
            }
            return workOrder;
        }
        #endregion
    }
}
