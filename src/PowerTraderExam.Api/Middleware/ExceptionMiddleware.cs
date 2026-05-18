using System.Net;
using System.Text.Json;
using PowerTraderExam.Api.Hangfire;
using PowerTraderExam.Application.Common;

namespace PowerTraderExam.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteErrorAsync(context, ex);
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, Exception ex)
    {
        if (HangfireFriendlyPage.IsHangfirePath(context.Request.Path))
        {
            await HangfireFriendlyPage.WriteAsync(
                context,
                "暂时无法加载任务列表",
                "后台任务页面加载失败，请检查数据库连接与 Hangfire 配置后重试。",
                HttpStatusCode.OK);
            return;
        }

        var (status, code, message) = ex switch
        {
            KeyNotFoundException => (HttpStatusCode.NotFound, 404, ex.Message),
            ArgumentException => (HttpStatusCode.BadRequest, 400, ex.Message),
            InvalidOperationException => (HttpStatusCode.BadRequest, 400, ex.Message),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, 401, ex.Message),
            _ => (HttpStatusCode.InternalServerError, 500, "服务器内部错误")
        };

        context.Response.StatusCode = (int)status;
        context.Response.ContentType = "application/json";
        var payload = ApiResponse<object>.Fail(code, message);
        await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}
