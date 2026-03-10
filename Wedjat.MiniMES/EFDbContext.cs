using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Wedjat.MiniMES.Model;

namespace Wedjat.MiniMES
{
    public class EFDbContext:DbContext
    {
        public EFDbContext(DbContextOptions<EFDbContext> options) : base(options)
        { 
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region 1. CompanyEmployee（员工表）
            modelBuilder.Entity<CompanyEmployee>(entity =>
            {
                // 主键
                entity.HasKey(e => e.Id);

                // 业务唯一约束：工号不可重复
                entity.HasIndex(e => e.WorkId)
                      .IsUnique()
                      .HasDatabaseName("IX_CompanyEmployee_WorkId");

                // 字段约束
                entity.Property(e => e.WorkId)
                      .HasMaxLength(50)
                      .IsRequired()
                      .HasComment("员工工号（业务唯一）");

                entity.Property(e => e.Name)
                      .HasMaxLength(100)
                      .IsRequired()
                      .HasComment("员工姓名");

                entity.Property(e => e.Password)
                      .HasMaxLength(200)
                      .IsRequired()
                      .HasComment("员工密码（建议加密存储）");

                // 审计字段说明
                entity.Property(e => e.CreateTime)
                      .HasComment("创建时间（自动生成）");

                entity.Property(e => e.UpdateTime)
                      .HasComment("更新时间（修改时自动更新）");
            });
            #endregion


            #region 2. MesPCB（PCB表）
            modelBuilder.Entity<MesPCB>(entity =>
            {
                entity.HasKey(p => p.Id);

                // 业务唯一约束：PCB编号不可重复
                entity.HasIndex(p => p.PCBCode)
                      .IsUnique()
                      .HasDatabaseName("IX_MesPCB_PCBCode");

                // 字段约束
                entity.Property(p => p.PCBCode)
                      .HasMaxLength(50)
                      .IsRequired()
                      .HasComment("PCB编号（业务唯一）");

                entity.Property(p => p.PCBName)
                      .HasMaxLength(100)
                      .IsRequired()
                      .HasComment("PCB名称");
            });
            #endregion


            #region 3. MesWorkOrder（工单表）
            modelBuilder.Entity<MesWorkOrder>(entity =>
            {
                entity.HasKey(o => o.Id);

                // 业务唯一约束：工单编号不可重复
                entity.HasIndex(o => o.WorkOrderCode)
                      .IsUnique()
                      .HasDatabaseName("IX_MesWorkOrder_WorkOrderCode");

                // 字段约束
                entity.Property(o => o.WorkOrderCode)
                      .HasMaxLength(50)
                      .IsRequired()
                      .HasComment("工单编号（业务唯一）");

                // 枚举类型映射为字符串（数据库存储"Pending"而非数字）
                entity.Property(o => o.OrderStatus)
                      .HasConversion<string>()
                      .HasMaxLength(20)
                      .IsRequired()
                      .HasComment("工单状态（Pending/Processing/Completed/Canceled）");
            });
            #endregion


            #region 4. MesPCBDefect（PCB缺陷类型表）
            modelBuilder.Entity<MesPCBDefect>(entity =>
            {
                entity.HasKey(d => d.Id);

                // 联合唯一约束：同一PCB下缺陷编码不可重复
                entity.HasIndex(d => new { d.PCBCode, d.DefectCode })
                      .IsUnique()
                      .HasDatabaseName("IX_MesPCBDefect_PCBCode_DefectCode");

                // 字段约束
                entity.Property(d => d.PCBCode)
                      .HasMaxLength(50)
                      .IsRequired()
                      .HasComment("关联的PCB编号（逻辑外键）");

                entity.Property(d => d.DefectCode)
                      .HasMaxLength(50)
                      .IsRequired()
                      .HasComment("缺陷编码（唯一标识）");

                entity.Property(d => d.DefectName)
                      .HasMaxLength(100)
                      .IsRequired()
                      .HasComment("缺陷名称");

                entity.Property(d => d.Description)
                      .HasMaxLength(500)
                      .HasComment("缺陷描述");

                // 关联 MesPCB（逻辑外键：PCBCode → MesPCB.PCBCode）
                entity.HasOne(d => d.PCB)
                      .WithMany(p => p.Defects)
                      .HasForeignKey(d => d.PCBCode)
                      .HasPrincipalKey(p => p.PCBCode) // 关联PCB的业务字段
                      .OnDelete(DeleteBehavior.NoAction) // 禁用级联删除
                      .HasConstraintName(null); // 不生成物理外键约束
            });
            #endregion


            #region 5. MesWorkOrderPCB（工单-PCB中间表）
            modelBuilder.Entity<MesWorkOrderPCB>(entity =>
            {
                entity.HasKey(wop => wop.Id);

                // 字段约束
                entity.Property(wop => wop.WorkOrderCode)
                      .HasMaxLength(50)
                      .IsRequired()
                      .HasComment("关联的工单编号（逻辑外键）");

                entity.Property(wop => wop.PCBCode)
                      .HasMaxLength(50)
                      .IsRequired()
                      .HasComment("关联的PCB编号（逻辑外键）");

                entity.Property(wop => wop.PlanQuantity)
                      .IsRequired()
                      .HasComment("计划生产数量");

                entity.Property(wop => wop.CompleteQuantity)
                      .HasDefaultValue(0)
                      .HasComment("已完成数量（默认0）");

                entity.Property(wop => wop.QualifiedQuantity)
                      .HasDefaultValue(0)
                      .HasComment("合格数量（默认0）");

                // 关联 MesWorkOrder（逻辑外键：WorkOrderCode → MesWorkOrder.WorkOrderCode）
                entity.HasOne(wop => wop.WorkOrder)
                      .WithMany(wo => wo.WorkOrderPCBs)
                      .HasForeignKey(wop => wop.WorkOrderCode)
                      .HasPrincipalKey(wo => wo.WorkOrderCode)
                      .OnDelete(DeleteBehavior.NoAction)
                      .HasConstraintName(null);

                // 关联 MesPCB（逻辑外键：PCBCode → MesPCB.PCBCode）
                entity.HasOne(wop => wop.PCB)
                      .WithMany(pcb => pcb.WorkOrderPCBs)
                      .HasForeignKey(wop => wop.PCBCode)
                      .HasPrincipalKey(pcb => pcb.PCBCode)
                      .OnDelete(DeleteBehavior.NoAction)
                      .HasConstraintName(null);
            });
            #endregion


            #region 6. MesPCBInspection（PCB质检记录表）
            modelBuilder.Entity<MesPCBInspection>(entity =>
            {
                entity.HasKey(i => i.Id);

                // 业务唯一约束：检测编号不可重复
                entity.HasIndex(i => i.InspectionCode)
                      .IsUnique()
                      .HasDatabaseName("IX_MesPCBInspection_InspectionCode");

                // 字段约束
                entity.Property(i => i.WorkOrderCode)
                      .HasMaxLength(50)
                      .IsRequired()
                      .HasComment("关联的工单编号（逻辑外键）");

                entity.Property(i => i.PCBCode)
                      .HasMaxLength(50)
                      .IsRequired()
                      .HasComment("关联的PCB编号（逻辑外键）");

                entity.Property(i => i.InspectionCode)
                      .HasMaxLength(50)
                      .IsRequired()
                      .HasComment("检测编号（业务唯一）");

                entity.Property(i => i.PCBSN)
                      .HasMaxLength(100)
                      .IsRequired()
                      .HasComment("单个PCB的二维码标识（唯一追溯）");

                entity.Property(i => i.InspectionTime)
                      .IsRequired()
                      .HasComment("检测时间");

                entity.Property(i => i.IsQualified)
                      .IsRequired()
                      .HasComment("是否合格（true=合格，false=不合格）");

                entity.Property(i => i.WorkId)
                      .HasMaxLength(50)
                      .IsRequired()
                      .HasComment("检测员工号（关联CompanyEmployee.WorkId）");

                entity.Property(i => i.ProductLine)
                      .HasMaxLength(50)
                      .IsRequired()
                      .HasComment("产线名称");

                // 关联 MesWorkOrder（逻辑外键：WorkOrderCode → 工单编号）
                entity.HasOne(i => i.WorkOrder)
                      .WithMany(wo => wo.Inspections)
                      .HasForeignKey(i => i.WorkOrderCode)
                      .HasPrincipalKey(wo => wo.WorkOrderCode)
                      .OnDelete(DeleteBehavior.NoAction)
                      .HasConstraintName(null);

                // 关联 MesPCB（逻辑外键：PCBCode → PCB编号）
                entity.HasOne(i => i.PCB)
                      .WithMany(pcb => pcb.Inspections)
                      .HasForeignKey(i => i.PCBCode)
                      .HasPrincipalKey(pcb => pcb.PCBCode)
                      .OnDelete(DeleteBehavior.NoAction)
                      .HasConstraintName(null);

                // 关联 CompanyEmployee（逻辑外键：InspectorWorkId → 员工工号）
                entity.HasOne(i => i.Inspector)
                      .WithMany(e => e.Inspections)
                      .HasForeignKey(i => i.WorkId)
                      .HasPrincipalKey(e => e.WorkId)
                      .OnDelete(DeleteBehavior.NoAction)
                      .HasConstraintName(null);
            });
            #endregion


            #region 7. MesInspectionDefect（质检缺陷项表）
            modelBuilder.Entity<MesInspectionDefect>(entity =>
            {
                entity.HasKey(id => id.Id);

                // 字段约束
                entity.Property(id => id.InspectionCode)
                      .HasMaxLength(50)
                      .IsRequired()
                      .HasComment("关联的检测编号（逻辑外键）");

                entity.Property(id => id.DefectCode)
                      .HasMaxLength(50)
                      .IsRequired()
                      .HasComment("关联的缺陷编码（逻辑外键）");

                entity.Property(id => id.Count)
                      .HasDefaultValue(0)
                      .IsRequired()
                      .HasComment("缺陷数量（默认0）");

                // 关联 MesPCBInspection（逻辑外键：InspectionCode → 检测编号）
                entity.HasOne(id => id.Inspection)
                      .WithMany(i => i.DetectedDefects)
                      .HasForeignKey(id => id.InspectionCode)
                      .HasPrincipalKey(i => i.InspectionCode)
                      .OnDelete(DeleteBehavior.NoAction)
                      .HasConstraintName(null);

                // 关联 MesPCBDefect（逻辑外键：DefectCode → 缺陷编码）
                entity.HasOne(id => id.Defect)
                      .WithMany(d => d.InspectionDefects)
                      .HasForeignKey(id => id.DefectCode)
                      .HasPrincipalKey(d => d.DefectCode)
                      .OnDelete(DeleteBehavior.NoAction)
                      .HasConstraintName(null);
            });
            #endregion
        }
        public DbSet<CompanyEmployee> CompanyEmployee { get; set; }
        public DbSet<MesWorkOrderPCB> MesWorkOrderPCB { get; set; }
        public DbSet<MesInspectionDefect> MesInspectionDefect { get; set; }
        public DbSet<MesPCB> MesPCB { get; set; }
        public DbSet<MesPCBDefect> MesPCBDefect { get; set; } 
        public DbSet<MesPCBInspection> MesPCBInspection { get; set; }
        public DbSet<MesWorkOrder> MesWorkOrder { get; set; }
    }
}
