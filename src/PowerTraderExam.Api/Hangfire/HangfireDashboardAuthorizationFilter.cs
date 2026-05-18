using Hangfire.Dashboard;
using Microsoft.Extensions.Options;
using PowerTraderExam.Application.Options;

namespace PowerTraderExam.Api.Hangfire;

public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    private readonly IWebHostEnvironment _environment;
    private readonly ApiKeyOptions _apiKeyOptions;

    public HangfireDashboardAuthorizationFilter(
        IWebHostEnvironment environment,
        IOptions<ApiKeyOptions> apiKeyOptions)
    {
        _environment = environment;
        _apiKeyOptions = apiKeyOptions.Value;
    }

    public bool Authorize(DashboardContext context)
    {
        if (_environment.IsDevelopment())
        {
            return true;
        }

        var http = context.GetHttpContext();
        var apiKey = http.Request.Headers["X-Api-Key"].FirstOrDefault();
        return !string.IsNullOrEmpty(_apiKeyOptions.Admin)
            && string.Equals(apiKey, _apiKeyOptions.Admin, StringComparison.Ordinal);
    }
}
