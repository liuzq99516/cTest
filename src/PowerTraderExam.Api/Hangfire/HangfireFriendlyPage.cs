using System.Net;
using System.Text;
using System.Text.Encodings.Web;

namespace PowerTraderExam.Api.Hangfire;

internal static class HangfireFriendlyPage
{
    public static bool IsHangfirePath(PathString path) =>
        path.StartsWithSegments("/hangfire", StringComparison.OrdinalIgnoreCase);

    public static async Task WriteAsync(HttpContext context, string title, string message, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        context.Response.Clear();
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "text/html; charset=utf-8";

        var encodedTitle = HtmlEncoder.Default.Encode(title);
        var encodedMessage = HtmlEncoder.Default.Encode(message);

        var html = new StringBuilder()
            .Append("<!DOCTYPE html><html lang=\"zh-CN\"><head><meta charset=\"utf-8\" />")
            .Append("<title>").Append(encodedTitle).Append("</title>")
            .Append("<style>")
            .Append("body{font-family:-apple-system,BlinkMacSystemFont,\"Segoe UI\",sans-serif;margin:40px;background:#f8f9fa;color:#212529;}")
            .Append(".card{max-width:720px;margin:0 auto;background:#fff;border:1px solid #dee2e6;border-radius:8px;padding:24px;}")
            .Append("h1{margin-top:0;font-size:22px;}p{line-height:1.7;margin:12px 0;}")
            .Append("a{color:#0d6efd;text-decoration:none;}a:hover{text-decoration:underline;}")
            .Append("</style></head><body><div class=\"card\">")
            .Append("<h1>").Append(encodedTitle).Append("</h1>")
            .Append("<p>").Append(encodedMessage).Append("</p>")
            .Append("<p><a href=\"/hangfire\">返回后台任务监控</a> · <a href=\"/swagger\">打开 Swagger</a></p>")
            .Append("</div></body></html>")
            .ToString();

        await context.Response.WriteAsync(html, context.RequestAborted);
    }
}
