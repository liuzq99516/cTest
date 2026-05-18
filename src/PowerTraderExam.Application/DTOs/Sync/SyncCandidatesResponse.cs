namespace PowerTraderExam.Application.DTOs.Sync;

public class SyncCandidatesResponse
{
    public IReadOnlyList<SyncCandidateDto> Items { get; set; } = Array.Empty<SyncCandidateDto>();
    public int Count { get; set; }
    public int Limit { get; set; }
    public bool HasMore { get; set; }
}
