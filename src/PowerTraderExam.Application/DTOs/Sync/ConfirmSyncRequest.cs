namespace PowerTraderExam.Application.DTOs.Sync;

public class ConfirmSyncRequest
{
    public List<long> CandidateIds { get; set; } = new();
    public string? Remark { get; set; }
}
