namespace PowerTraderExam.Application.Options;

public class ApiKeyOptions
{
    public const string SectionName = "ApiKeys";

    public string Admin { get; set; } = string.Empty;
    public Dictionary<string, string> Servers { get; set; } = new();
}
