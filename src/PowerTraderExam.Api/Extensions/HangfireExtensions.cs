using Hangfire;
using Hangfire.MySql;
using Microsoft.Extensions.Options;
using PowerTraderExam.Api.Hangfire;
using PowerTraderExam.Application.Interfaces;
using PowerTraderExam.Application.Options;

namespace PowerTraderExam.Api.Extensions;

public static class HangfireExtensions
{
    public static IServiceCollection AddHangfireServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<HangfireOptions>(configuration.GetSection(HangfireOptions.SectionName));

        var connectionString = configuration.GetConnectionString("Default")
            ?? "Server=localhost;Port=3306;Database=power_trader_exam;User=root;Password=root;CharSet=utf8mb4;Allow User Variables=true;";

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseStorage(new MySqlStorage(connectionString, new MySqlStorageOptions
            {
                TablesPrefix = "Hangfire",
                PrepareSchemaIfNecessary = true
            })));

        services.AddHangfireServer();
        services.AddSingleton<HangfireDashboardAuthorizationFilter>();

        return services;
    }

    public static WebApplication UseHangfireApp(this WebApplication app)
    {
        var hangfireOptions = app.Services.GetRequiredService<IOptions<HangfireOptions>>().Value;
        var javaPushOptions = app.Services.GetRequiredService<IOptions<JavaPushOptions>>().Value;

        if (hangfireOptions.EnableDashboard)
        {
            app.UseMiddleware<HangfireDashboardUiMiddleware>();

            var authFilter = app.Services.GetRequiredService<HangfireDashboardAuthorizationFilter>();
            app.UseHangfireDashboard(hangfireOptions.DashboardPath, new DashboardOptions
            {
                Authorization = new[] { authFilter },
                DashboardTitle = hangfireOptions.DashboardTitle,
                AppPath = "/swagger",
                DisplayStorageConnectionString = false,
                DarkModeEnabled = false
            });
        }

        if (javaPushOptions.EnableBackgroundPush)
        {
            RecurringJob.AddOrUpdate<IScorePushJob>(
                "成绩推送重试",
                job => job.ExecuteAsync(CancellationToken.None),
                ToMinuteIntervalCron(javaPushOptions.RetryIntervalMinutes));
        }
        else
        {
            RecurringJob.RemoveIfExists("成绩推送重试");
        }

        return app;
    }

    private static string ToMinuteIntervalCron(int minutes)
    {
        var interval = minutes < 1 ? 1 : minutes;
        return $"*/{interval} * * * *";
    }
}
