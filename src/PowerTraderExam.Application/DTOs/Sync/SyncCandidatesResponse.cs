namespace PowerTraderExam.Application.DTOs.Sync;

public class SyncCandidatesResponse
{
    public string BatchCode { get; set; } = string.Empty;
    public IReadOnlyList<SyncCandidateDto> Items { get; set; } = Array.Empty<SyncCandidateDto>();
    public int Count { get; set; }
    public int Limit { get; set; }
    public bool HasMore { get; set; }
    public int MaxSyncPerBatch { get; set; }
    public int SyncedCountInBatch { get; set; }
    public int RemainingQuota { get; set; }
}
