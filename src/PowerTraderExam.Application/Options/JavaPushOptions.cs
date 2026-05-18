namespace PowerTraderExam.Application.Options;

public class JavaPushOptions
{
    public const string SectionName = "JavaPush";

    public string BaseUrl { get; set; } = "http://127.0.0.1:8080";
    public string PushPath { get; set; } = "/student/grade/callback/score";
    public string PublicKeyPem { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
    public int BatchSize { get; set; } = 50;
    public int MaxRetry { get; set; } = 5;
    public int RetryIntervalMinutes { get; set; } = 10;
    public bool EnableBackgroundPush { get; set; } = true;
}
