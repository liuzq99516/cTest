namespace PowerTraderExam.Domain.Entities;

public class ExamBatch
{
    public long Id { get; set; }
    public string BatchCode { get; set; } = string.Empty;
    public string BatchName { get; set; } = string.Empty;
    public DateTime? ExamDate { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Candidate> Candidates { get; set; } = new List<Candidate>();
}
