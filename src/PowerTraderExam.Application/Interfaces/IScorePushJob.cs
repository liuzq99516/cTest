namespace PowerTraderExam.Application.Interfaces;

public interface IScorePushJob
{
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}
