using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;

namespace Wedjat.MiniMES
{
    public class IEFDbContextDesignFactory :
        IDesignTimeDbContextFactory<EFDbContext>
    {
        public EFDbContext CreateDbContext(string[] args)
        {
            #region MySQL
            // 1. 构建配置
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
                .Build();

            // 2. 获取连接字符串
            string? connectionString = configuration.GetConnectionString("EFDbContextConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("未找到连接字符串 'EFDbContextConnection'");
            }

            // 3. 配置 MySQL 选项（正确方式）
            var optionsBuilder = new DbContextOptionsBuilder<EFDbContext>();

            // 定义 MySQL 版本（只需要定义一次）
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 39));

            // 基础 MySQL 配置 - 包含版本参数
            optionsBuilder.UseMySql(
                connectionString,
                serverVersion
            );

            // 4. 配置迁移历史表（必须包含版本参数）
            optionsBuilder.UseMySql(
                connectionString,
                serverVersion, // 这里添加了ServerVersion参数
                options => options.MigrationsHistoryTable("__EFMigrationsHistory")
            );

            return new EFDbContext(optionsBuilder.Options);
            #endregion
        }

    }
}
