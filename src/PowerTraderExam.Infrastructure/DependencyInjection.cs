using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PowerTraderExam.Application.Interfaces;
using PowerTraderExam.Application.Options;
using PowerTraderExam.Infrastructure.Jobs;
using PowerTraderExam.Infrastructure.Java;
using PowerTraderExam.Infrastructure.Persistence;
using PowerTraderExam.Infrastructure.Services;

namespace PowerTraderExam.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ApiKeyOptions>(configuration.GetSection(ApiKeyOptions.SectionName));
        services.Configure<CandidateSyncOptions>(configuration.GetSection(CandidateSyncOptions.SectionName));
        services.Configure<JavaPushOptions>(configuration.GetSection(JavaPushOptions.SectionName));

        var connectionString = configuration.GetConnectionString("Default")
            ?? "Server=localhost;Port=3306;Database=power_trader_exam;User=root;Password=root;CharSet=utf8mb4;";

        var serverVersion = ServerVersion.Parse("8.0.36-mysql");
        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connectionString, serverVersion, mySqlOptions =>
                mySqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddHttpClient("JavaPush", (sp, client) =>
        {
            var opts = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<JavaPushOptions>>().Value;
            client.Timeout = TimeSpan.FromSeconds(opts.TimeoutSeconds);
        });

        services.AddScoped<IBatchService, BatchService>();
        services.AddScoped<ISyncService, SyncService>();
        services.AddScoped<IScoreService, ScoreService>();
        services.AddScoped<IJavaScorePushService, JavaScorePushService>();
        services.AddScoped<IScorePushJob, ScorePushJob>();
        services.AddSingleton<JavaScorePushClient>();

        return services;
    }
}
