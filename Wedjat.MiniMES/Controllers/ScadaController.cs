using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wedjat.MiniMES.DTO;
using Wedjat.MiniMES.Model;

namespace Wedjat.MiniMES.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class ScadaController :ControllerBase
    {
        private readonly EFDbContext _dbContext;
        public ScadaController(EFDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region 登录
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="workId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public async Task<IActionResult> UserLogin(string workId,string password)
        {
            try
            {
                if (string.IsNullOrEmpty(workId) || string.IsNullOrEmpty(password))
                {
                    return BadRequest(ApiResponse<string>.ErrorResult("工号和密码不能为空"));
                }
                var employee = await _dbContext.CompanyEmployee.FirstOrDefaultAsync(e => e.WorkId == workId && e.Password == password);
                if (employee == null)
                {
                    return Unauthorized(ApiResponse<string>.ErrorResult("工号或密码错误，请重新输入"));
                }
                var userData = new
                {
                    workId = employee.WorkId,
                    name = employee.Name
                };
                return Ok(ApiResponse<object>.SuccessResult(userData, "登录成功"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResult($"服务器内部错误，请稍后重试:{ex.Message}")
                );
            }
           
        }
        #endregion

        #region 根据扫码枪解析的工单号调用MESAPI获取工单
        /// <summary>
        /// 根据扫码枪解析的工单号调用MESAPI获取工单
        /// </summary>
        /// <param name="scannerCode"></param>
        /// <returns></returns>
        [HttpGet("GetWorkOrder")]
        public async Task<IActionResult> ScannerGetWorkOrder(string scannerCode)
        {
            try
            {
                if (string.IsNullOrEmpty(scannerCode))
                {
                    return BadRequest(ApiResponse<string>.ErrorResult("扫码枪获取到的二维码不能为空！"));
                }

                var workOrder = await _dbContext.MesWorkOrder
                    .Include(wo => wo.WorkOrderPCBs) 
                        .ThenInclude(wop => wop.PCB) 
                            .ThenInclude(pcb => pcb!.Defects) 
                    .FirstOrDefaultAsync(wo => wo.WorkOrderCode == scannerCode && !wo.IsDeleted);

                if (workOrder == null)
                {
                    return NotFound(ApiResponse<string>.ErrorResult("MES中不存在此工单或工单已删除"));
                }

                var dto = new MesToScada
                {
                    Id = workOrder.Id,
                    WorkOrderCode = workOrder.WorkOrderCode,
                    OrderStatus = workOrder.OrderStatus,
                    WorkOrderPCBs = workOrder.WorkOrderPCBs.Select(wop => new DTO.MesWorkOrderPCB
                    {
                        PCBCode = wop.PCBCode,
                        PlanQuantity = wop.PlanQuantity,
                        CompleteQuantity = wop.CompleteQuantity ?? 0,
                        QualifiedQuantity = wop.QualifiedQuantity,
                        mesPCBs = new DTO.MesPCB
                        {
                            PCBCode=wop.PCB!.PCBCode,
                            PCBName = wop.PCB!.PCBName,
                            mesPCBDefects = wop.PCB.Defects.Select(d => new DTO.MesPCBDefect
                            {
                                DefectCode = d.DefectCode,
                                DefectName = d.DefectName
                            }).ToList()
                        }
                    }).ToList()
                };

                return Ok(ApiResponse<MesToScada>.SuccessResult(dto, "工单数据获取成功"));
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, ApiResponse<string>.ErrorResult($"服务器内部错误，请稍后重试:{ex.Message}"));
            }
        }
        #endregion

        #region 上位机将PCB检测信息推送到Mes
        [HttpPost("PushPCBInspection")]
        public async Task<IActionResult> ScadaPushPCBInspection([FromBody] ScadaToMes toMes)
        {
            try
            {
                // 修复问题1：逻辑颠倒 → 改为“toMes == null”时返回不能为空
                if (toMes == null)
                {
                    return BadRequest(ApiResponse<string>.ErrorResult("上位机推送的PCB检测信息不能为空"));
                }

                var errorMsg = new List<string>();
                if (string.IsNullOrWhiteSpace(toMes.InspectionCode)) errorMsg.Add("检测编号不能为空");
                if (string.IsNullOrWhiteSpace(toMes.WorkOrderCode)) errorMsg.Add("工单编号不能为空");
                if (string.IsNullOrWhiteSpace(toMes.PCBSN)) errorMsg.Add("PCB唯一序列号不能为空");
                if (string.IsNullOrWhiteSpace(toMes.WorkId)) errorMsg.Add("操作员工号不能为空");
                if (string.IsNullOrWhiteSpace(toMes.ProductLine)) errorMsg.Add("产线不能为空");
                if (!toMes.IsQualified && (toMes.DetectedDefects == null || toMes.DetectedDefects.Count == 0))
                {
                    errorMsg.Add("检测不合格时，缺陷列表不能为空");
                }

                // 修复问题4：校验缺陷列表中的 InspectionCode（若为必填）
                if (!toMes.IsQualified && toMes.DetectedDefects != null && toMes.DetectedDefects.Any())
                {
                    foreach (var defect in toMes.DetectedDefects)
                    {
                        if (string.IsNullOrWhiteSpace(defect.InspectionCode))
                        {
                            errorMsg.Add("缺陷列表中的检测编号（InspectionCode）不能为空");
                        }
                        if (string.IsNullOrWhiteSpace(defect.DefectCode))
                        {
                            errorMsg.Add("缺陷列表中的缺陷编码（DefectCode）不能为空");
                        }
                        if (defect.Count <= 0)
                        {
                            errorMsg.Add($"缺陷编码【{defect.DefectCode}】的数量必须大于0");
                        }
                    }
                }

                if (errorMsg.Any())
                {
                    return BadRequest(ApiResponse<string>.ErrorResult(string.Join("；", errorMsg)));
                }

                var workOrder = await _dbContext.MesWorkOrder.FirstOrDefaultAsync(wo => wo.WorkOrderCode == toMes.WorkOrderCode && !wo.IsDeleted);
                if (workOrder == null)
                {
                    return NotFound(ApiResponse<string>.ErrorResult($"MES中不存在工单【{toMes.WorkOrderCode}】或工单已删除"));
                }

                var pcb = await _dbContext.MesPCB.FirstOrDefaultAsync(p => p.PCBCode == toMes.PCBCode && !p.IsDeleted);
                if (pcb == null)
                {
                    return NotFound(ApiResponse<string>.ErrorResult($"MES中不存在PCB型号【{toMes.PCBCode}】或PCB已删除"));
                }

                var existsRecord = await _dbContext.MesPCBInspection.AnyAsync(r => r.PCBSN == toMes.PCBSN && r.InspectionCode == toMes.InspectionCode);
                if (existsRecord)
                {
                    return Conflict(ApiResponse<string>.ErrorResult($"PCB序列号【{toMes.PCBSN}】的检测记录（编号：{toMes.InspectionCode}）已存在，无需重复推送"));
                }

                var workOrderPcb = await _dbContext.MesWorkOrderPCB.FirstOrDefaultAsync(wp => wp.WorkOrderCode == toMes.WorkOrderCode && wp.PCBCode == toMes.PCBCode && !wp.IsDeleted);
                if (workOrderPcb == null)
                {
                    return NotFound(ApiResponse<string>.ErrorResult($"MES中不存在工单【{toMes.WorkOrderCode}】与PCB型号【{toMes.PCBCode}】的关联记录（MesWorkOrderPCB）"));
                }

                var inspection = new MesPCBInspection
                {
                    InspectionCode = toMes.InspectionCode,
                    WorkOrderCode = toMes.WorkOrderCode,
                    PCBCode = toMes.PCBCode,
                    PCBSN = toMes.PCBSN,
                    InspectionTime = toMes.InspectionTime,
                    IsQualified = toMes.IsQualified,
                    WorkId = toMes.WorkId,
                    ProductLine = toMes.ProductLine
                };
                await _dbContext.MesPCBInspection.AddAsync(inspection);

                if (!toMes.IsQualified && toMes.DetectedDefects!.Any())
                {
                    foreach (var defect in toMes.DetectedDefects!)
                    {
                        var mesDefect = await _dbContext.MesPCBDefect.FirstOrDefaultAsync(d => d.DefectCode == defect.DefectCode && !d.IsDeleted);
                        if (mesDefect == null)
                        {
                            return BadRequest(ApiResponse<string>.ErrorResult($"MES缺陷库中不存在缺陷编码【{defect.DefectCode}】（缺陷名称：{defect.DefectName}）"));
                        }

                        var defectRecord = new Model.MesInspectionDefect
                        {
                            InspectionCode = defect.InspectionCode,
                            DefectCode = defect.DefectCode,
                            Count = defect.Count,
                        };
                        await _dbContext.MesInspectionDefect.AddAsync(defectRecord);
                    }
                }


                if (toMes.IsQualified)
                {
        
                    workOrderPcb.QualifiedQuantity += 1;
                }
                else
                {
                
                    workOrderPcb.CompleteQuantity += 1;
                }
                _dbContext.MesWorkOrderPCB.Update(workOrderPcb);
                workOrder.OrderStatus = toMes.WorkOrderStatus;
                await UpdateWorkOrderStatusAsync(workOrder);
                await _dbContext.SaveChangesAsync();
                return Ok(ApiResponse<string>.SuccessResult(
                    $"PCB检测信息推送成功！\n" +
                    $"检测编号：{toMes.InspectionCode} | PCB序列号：{toMes.PCBSN} | 检测结果：{(toMes.IsQualified ? "合格（计入好板）" : "不合格（计入次品）")}\n" +
                    $"工单【{toMes.WorkOrderCode}】-PCB【{toMes.PCBCode}】更新后：\n" +
                    $"好板数量：{workOrderPcb.QualifiedQuantity} | 次品数量：{workOrderPcb.CompleteQuantity - workOrderPcb.QualifiedQuantity}",
                    "推送成功并更新数量"
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResult($"服务器内部错误，请稍后重试:{ex.Message}"));
            }
        }
        #endregion
        /// <summary>
        /// 更新工单状态
        /// </summary>
        /// <param name="workOrder">工单对象</param>
        /// <returns></returns>
        private async Task UpdateWorkOrderStatusAsync(MesWorkOrder workOrder)
        {
            if (workOrder == null) throw new ArgumentNullException(nameof(workOrder));

            // 计算完成数量和计划数量的比例
            int totalQuantity = workOrder.WorkOrderPCBs.Sum(wp => wp.PlanQuantity);
            int completeQuantity = workOrder.Inspections.Sum(i => i.IsQualified ? 1 : 0);

            // 更新工单状态
            if (completeQuantity >= totalQuantity)
            {
                workOrder.OrderStatus = OrderStatus.Completed;
            }
            else if (workOrder.OrderStatus == OrderStatus.Processing)
            {
                return;
            }
            else
            {
                workOrder.OrderStatus = OrderStatus.Processing;
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
