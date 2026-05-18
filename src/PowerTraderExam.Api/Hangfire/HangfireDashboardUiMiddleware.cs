using System.Globalization;
using System.Text;
using Hangfire.Dashboard.Resources;

namespace PowerTraderExam.Api.Hangfire;

/// <summary>
/// 为 Hangfire Dashboard 注入中文空状态样式，并固定使用 zh 资源。
/// </summary>
public sealed class HangfireDashboardUiMiddleware
{
    private const string AssetsPrefix = "/hangfire-assets/";
    private const string InjectMarkup =
        "<link rel=\"stylesheet\" href=\"/hangfire-assets/dashboard.css\" />" +
        "<script src=\"/hangfire-assets/dashboard.js\" defer></script>";

    private readonly RequestDelegate _next;

    public HangfireDashboardUiMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/hangfire", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        Strings.Culture = CultureInfo.GetCultureInfo("zh");

        if (!HttpMethods.IsGet(context.Request.Method)
            || context.Request.Path.StartsWithSegments(AssetsPrefix, StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var originalBody = context.Response.Body;
        await using var buffer = new MemoryStream();
        context.Response.Body = buffer;

        try
        {
            await _next(context);

            if (context.Response.StatusCode != StatusCodes.Status200OK
                || !IsHtml(context.Response.ContentType))
            {
                buffer.Position = 0;
                await buffer.CopyToAsync(originalBody);
                return;
            }

            buffer.Position = 0;
            var html = await new StreamReader(buffer, Encoding.UTF8).ReadToEndAsync();
            if (html.Contains("/hangfire-assets/dashboard.css", StringComparison.Ordinal))
            {
                buffer.Position = 0;
                await buffer.CopyToAsync(originalBody);
                return;
            }

            var headIndex = html.IndexOf("</head>", StringComparison.OrdinalIgnoreCase);
            if (headIndex >= 0)
            {
                html = html.Insert(headIndex, InjectMarkup);
            }

            context.Response.Body = originalBody;
            context.Response.ContentLength = null;
            await context.Response.WriteAsync(html, Encoding.UTF8, context.RequestAborted);
        }
        finally
        {
            context.Response.Body = originalBody;
        }
    }

    private static bool IsHtml(string? contentType) =>
        contentType?.Contains("text/html", StringComparison.OrdinalIgnoreCase) == true;
}
