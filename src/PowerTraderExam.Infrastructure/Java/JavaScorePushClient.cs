using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PowerTraderExam.Application.DTOs.Java;
using PowerTraderExam.Application.Options;
using PowerTraderExam.Infrastructure.Security;

namespace PowerTraderExam.Infrastructure.Java;

public class JavaScorePushClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JavaPushOptions _options;
    private readonly ILogger<JavaScorePushClient> _logger;

    public JavaScorePushClient(
        IHttpClientFactory httpClientFactory,
        IOptions<JavaPushOptions> options,
        ILogger<JavaScorePushClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<(bool Success, string ResponseBody, string RequestBody, string Sign, string Timestamp)> PushAsync(
        IReadOnlyList<JavaGradeCallbackItemDto> items,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_options.PublicKeyPem))
        {
            throw new InvalidOperationException("JavaPush:PublicKeyPem is not configured.");
        }

        var requestBody = JavaJsonSerializer.SerializeGradeItems(items);
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        var plainText = requestBody + timestamp;
        var sign = RsaSignHelper.CreateSign(plainText, _options.PublicKeyPem);

        var client = _httpClientFactory.CreateClient("JavaPush");
        var url = $"{_options.BaseUrl.TrimEnd('/')}{_options.PushPath}";
        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
        request.Headers.TryAddWithoutValidation("sign", sign);
        request.Headers.TryAddWithoutValidation("timestamp", timestamp);

        try
        {
            var response = await client.SendAsync(request, ct);
            var responseBody = await response.Content.ReadAsStringAsync(ct);
            var success = response.IsSuccessStatusCode;
            if (!success)
            {
                _logger.LogWarning("Java push failed: {StatusCode} {Body}", response.StatusCode, responseBody);
            }

            return (success, responseBody, requestBody, sign, timestamp);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Java push exception");
            return (false, ex.Message, requestBody, sign, timestamp);
        }
    }
}
