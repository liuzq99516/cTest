namespace PowerTraderExam.Application.Options;

public class CandidateSyncOptions
{
    public const string SectionName = "CandidateSync";

    public int MaxPullCount { get; set; } = 200;
    public int DefaultPullCount { get; set; } = 200;
    public int MaxConfirmCount { get; set; } = 200;
}
