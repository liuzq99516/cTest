namespace PowerTraderExam.Application.Options;

public class HangfireOptions
{
    public const string SectionName = "Hangfire";

    public string DashboardPath { get; set; } = "/hangfire";
    public bool EnableDashboard { get; set; } = true;
}
