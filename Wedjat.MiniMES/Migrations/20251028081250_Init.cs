using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wedjat.MiniMES.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CompanyEmployee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    WorkId = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "员工工号（业务唯一）")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "员工姓名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "员工密码（建议加密存储）")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间（自动生成）"),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间（修改时自动更新）")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyEmployee", x => x.Id);
                    table.UniqueConstraint("AK_CompanyEmployee_WorkId", x => x.WorkId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MesPCB",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PCBCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "PCB编号（业务唯一）")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PCBName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "PCB名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MesPCB", x => x.Id);
                    table.UniqueConstraint("AK_MesPCB_PCBCode", x => x.PCBCode);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MesWorkOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    WorkOrderCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "工单编号（业务唯一）")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OrderStatus = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, comment: "工单状态（Pending/Processing/Completed/Canceled）")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MesWorkOrder", x => x.Id);
                    table.UniqueConstraint("AK_MesWorkOrder_WorkOrderCode", x => x.WorkOrderCode);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MesPCBDefect",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PCBCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "关联的PCB编号（逻辑外键）")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DefectCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "缺陷编码（唯一标识）")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DefectName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "缺陷名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false, comment: "缺陷描述")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MesPCBDefect", x => x.Id);
                    table.UniqueConstraint("AK_MesPCBDefect_DefectCode", x => x.DefectCode);
                    table.ForeignKey(
                        name: "FK_MesPCBDefect_MesPCB_PCBCode",
                        column: x => x.PCBCode,
                        principalTable: "MesPCB",
                        principalColumn: "PCBCode");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MesPCBInspection",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    WorkOrderCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "关联的工单编号（逻辑外键）")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PCBCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "关联的PCB编号（逻辑外键）")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InspectionCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "检测编号（业务唯一）")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PCBSN = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "单个PCB的二维码标识（唯一追溯）")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InspectionTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "检测时间"),
                    IsQualified = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "是否合格（true=合格，false=不合格）"),
                    WorkId = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "检测员工号（关联CompanyEmployee.WorkId）")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProductLine = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "产线名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MesPCBInspection", x => x.Id);
                    table.UniqueConstraint("AK_MesPCBInspection_InspectionCode", x => x.InspectionCode);
                    table.ForeignKey(
                        name: "FK_MesPCBInspection_CompanyEmployee_WorkId",
                        column: x => x.WorkId,
                        principalTable: "CompanyEmployee",
                        principalColumn: "WorkId");
                    table.ForeignKey(
                        name: "FK_MesPCBInspection_MesPCB_PCBCode",
                        column: x => x.PCBCode,
                        principalTable: "MesPCB",
                        principalColumn: "PCBCode");
                    table.ForeignKey(
                        name: "FK_MesPCBInspection_MesWorkOrder_WorkOrderCode",
                        column: x => x.WorkOrderCode,
                        principalTable: "MesWorkOrder",
                        principalColumn: "WorkOrderCode");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MesWorkOrderPCB",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    WorkOrderCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "关联的工单编号（逻辑外键）")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PCBCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "关联的PCB编号（逻辑外键）")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PlanQuantity = table.Column<int>(type: "int", nullable: false, comment: "计划生产数量"),
                    CompleteQuantity = table.Column<int>(type: "int", nullable: true, defaultValue: 0, comment: "已完成数量（默认0）"),
                    QualifiedQuantity = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "合格数量（默认0）"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MesWorkOrderPCB", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MesWorkOrderPCB_MesPCB_PCBCode",
                        column: x => x.PCBCode,
                        principalTable: "MesPCB",
                        principalColumn: "PCBCode");
                    table.ForeignKey(
                        name: "FK_MesWorkOrderPCB_MesWorkOrder_WorkOrderCode",
                        column: x => x.WorkOrderCode,
                        principalTable: "MesWorkOrder",
                        principalColumn: "WorkOrderCode");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MesInspectionDefect",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    InspectionCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "关联的检测编号（逻辑外键）")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DefectCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "关联的缺陷编码（逻辑外键）")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Count = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "缺陷数量（默认0）"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MesInspectionDefect", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MesInspectionDefect_MesPCBDefect_DefectCode",
                        column: x => x.DefectCode,
                        principalTable: "MesPCBDefect",
                        principalColumn: "DefectCode");
                    table.ForeignKey(
                        name: "FK_MesInspectionDefect_MesPCBInspection_InspectionCode",
                        column: x => x.InspectionCode,
                        principalTable: "MesPCBInspection",
                        principalColumn: "InspectionCode");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyEmployee_WorkId",
                table: "CompanyEmployee",
                column: "WorkId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MesInspectionDefect_DefectCode",
                table: "MesInspectionDefect",
                column: "DefectCode");

            migrationBuilder.CreateIndex(
                name: "IX_MesInspectionDefect_InspectionCode",
                table: "MesInspectionDefect",
                column: "InspectionCode");

            migrationBuilder.CreateIndex(
                name: "IX_MesPCB_PCBCode",
                table: "MesPCB",
                column: "PCBCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MesPCBDefect_PCBCode_DefectCode",
                table: "MesPCBDefect",
                columns: new[] { "PCBCode", "DefectCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MesPCBInspection_InspectionCode",
                table: "MesPCBInspection",
                column: "InspectionCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MesPCBInspection_PCBCode",
                table: "MesPCBInspection",
                column: "PCBCode");

            migrationBuilder.CreateIndex(
                name: "IX_MesPCBInspection_WorkId",
                table: "MesPCBInspection",
                column: "WorkId");

            migrationBuilder.CreateIndex(
                name: "IX_MesPCBInspection_WorkOrderCode",
                table: "MesPCBInspection",
                column: "WorkOrderCode");

            migrationBuilder.CreateIndex(
                name: "IX_MesWorkOrder_WorkOrderCode",
                table: "MesWorkOrder",
                column: "WorkOrderCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MesWorkOrderPCB_PCBCode",
                table: "MesWorkOrderPCB",
                column: "PCBCode");

            migrationBuilder.CreateIndex(
                name: "IX_MesWorkOrderPCB_WorkOrderCode",
                table: "MesWorkOrderPCB",
                column: "WorkOrderCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MesInspectionDefect");

            migrationBuilder.DropTable(
                name: "MesWorkOrderPCB");

            migrationBuilder.DropTable(
                name: "MesPCBDefect");

            migrationBuilder.DropTable(
                name: "MesPCBInspection");

            migrationBuilder.DropTable(
                name: "CompanyEmployee");

            migrationBuilder.DropTable(
                name: "MesPCB");

            migrationBuilder.DropTable(
                name: "MesWorkOrder");
        }
    }
}
