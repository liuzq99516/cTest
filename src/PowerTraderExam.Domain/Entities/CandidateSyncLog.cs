namespace PowerTraderExam.Domain.Entities;

public class CandidateSyncLog
{
    public long Id { get; set; }
    public long CandidateId { get; set; }
    public string ServerId { get; set; } = string.Empty;
    public DateTime SyncedAt { get; set; } = DateTime.UtcNow;
    public string? Remark { get; set; }

    public Candidate Candidate { get; set; } = null!;
}
