namespace PowerTraderExam.Application.Options;

public class CandidateSyncOptions
{
    public const string SectionName = "CandidateSync";

    /// <summary>单次拉取请求的最大人数。</summary>
    public int MaxPullCount { get; set; } = 200;

    public int DefaultPullCount { get; set; } = 200;

    /// <summary>单次 confirm 请求的最大人数。</summary>
    public int MaxConfirmCount { get; set; } = 200;

    /// <summary>每个考试服务器在同一批次内累计最多可同步的考生数。</summary>
    public int MaxSyncPerBatchPerServer { get; set; } = 200;
}
