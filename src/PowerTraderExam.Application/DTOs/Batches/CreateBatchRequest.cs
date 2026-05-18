namespace PowerTraderExam.Application.DTOs.Batches;

public class CreateBatchRequest
{
    public string BatchCode { get; set; } = string.Empty;
    public string BatchName { get; set; } = string.Empty;
    public DateTime? ExamDate { get; set; }
}
