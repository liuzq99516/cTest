using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PowerTraderExam.Infrastructure.Persistence;

/// <summary>
/// 供 dotnet ef 设计时创建 DbContext（读取 Api 项目的 appsettings）。
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var apiProjectPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "PowerTraderExam.Api");
        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiProjectPath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("Default")
            ?? "Server=localhost;Port=3306;Database=power_trader_exam;User=root;Password=root;CharSet=utf8mb4;Allow User Variables=true;";

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        var serverVersion = ServerVersion.Parse("8.0.36-mysql");
        optionsBuilder.UseMySql(
            connectionString,
            serverVersion,
            mySqlOptions => mySqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));

        return new AppDbContext(optionsBuilder.Options);
    }
}
