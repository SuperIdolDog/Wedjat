using FreeSql;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wedjat.Model.Entity;

namespace Wedjat.Model
{
    public class AppDbContext:FreeSql.DbContext
    {
       
            private static readonly Lazy<IFreeSql> _sqliteLazy = new Lazy<IFreeSql>(() =>
            {
                string solutionDir = GetSolutionDirectory();
                // 在解决方案目录下创建 Wedjat 文件夹（或直接放数据库文件）
                string dbFolder = Path.Combine(solutionDir);
                // 确保文件夹存在，不存在则创建
                if (!Directory.Exists(dbFolder))
                {
                    Directory.CreateDirectory(dbFolder);
                }
                // 拼接数据库文件完整路径
                string dbPath = Path.Combine(dbFolder, "wedjetdb.db");

                Trace.WriteLine($"SQLite数据库路径：{dbPath}");

                return new FreeSqlBuilder()
                    .UseMonitorCommand(cmd => Trace.WriteLine($"Sql：{cmd.CommandText}"))
                    .UseAdoConnectionPool(true)
                    .UseAutoSyncStructure(true)
                    .UseConnectionString(DataType.Sqlite, $"Data Source={dbPath};") // 使用拼接的路径
                    .Build();
            });
        private static string GetSolutionDirectory()
        {
            // 1. 从当前程序集的路径（bin目录）向上查找解决方案目录
            string currentDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            // 2. 向上遍历目录，直到找到 .sln 文件（解决方案文件）或到达根目录
            while (!string.IsNullOrEmpty(currentDir))
            {
                // 检查当前目录是否存在 .sln 文件
                if (Directory.EnumerateFiles(currentDir, "*.sln").Any())
                {
                    return currentDir; // 找到解决方案目录
                }
                // 向上一级目录查找
                currentDir = Path.GetDirectoryName(currentDir);
            }
            // 3. 如果找不到 .sln 文件，退回到程序运行目录（bin目录）
            return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
        public static IFreeSql Sqlite => _sqliteLazy.Value;
            public AppDbContext() : base(Sqlite, new DbContextOptions())
            {

            }
            public static void SyncAllTables()
            {
                try
                {
                    var entityTypes = new List<Type>
                {
                    typeof(WorkOrder),
                    typeof(PCB),
                    typeof(PCBDefectType),
                    typeof(ScannerData),
                    typeof(PCBInspection),
                    typeof(PCBDefectDetail),
                    typeof(SysLog),
                    typeof(WorkOrderPCB),
                    typeof(PLCLog),
                    typeof(PLCSlaveVariable)
                };

                    // 手动同步所有实体的表结构
                    Sqlite.CodeFirst.SyncStructure(entityTypes.ToArray());
                    Trace.WriteLine("所有表结构同步完成");
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"表结构同步失败：{ex.Message}");
                    throw; // 抛出异常，让上层处理
                }
            }




            public DbSet<WorkOrder> WorkOrders { get; set; }
            public DbSet<PCB> PCBs { get; set; }
            public DbSet<PCBDefectType> PCBDefectTypes { get; set; }
            public DbSet<ScannerData> ScannerDatas { get; set; }
            public DbSet<PCBInspection> PCBInspections { get; set; }
            public DbSet<PCBDefectDetail> PCBDefectDetails { get; set; }
            public DbSet<SysLog> SysLogs { get; set; }
            public DbSet<PLCLog> PLCLogs { get; set; }
            public DbSet<PLCSlaveVariable> PLCSlaveVariables { get; set; }



        }
    }

