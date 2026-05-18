using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PowerTraderExam.Application.DTOs.Java;
using PowerTraderExam.Application.DTOs.Scores;
using PowerTraderExam.Application.Interfaces;
using PowerTraderExam.Application.Options;
using PowerTraderExam.Domain.Enums;
using PowerTraderExam.Infrastructure.Java;
using PowerTraderExam.Infrastructure.Persistence;

namespace PowerTraderExam.Infrastructure.Services;

public class JavaScorePushService : IJavaScorePushService
{
    private readonly AppDbContext _db;
    private readonly JavaScorePushClient _client;
    private readonly JavaPushOptions _options;

    public JavaScorePushService(
        AppDbContext db,
        JavaScorePushClient client,
        IOptions<JavaPushOptions> options)
    {
        _db = db;
        _client = client;
        _options = options.Value;
    }

    public async Task<PushResultDto> PushPendingScoresAsync(
        string? batchCode,
        string? subject,
        IReadOnlyList<long>? scoreIds,
        CancellationToken ct = default)
    {
        var query = _db.ExamScores
            .Include(x => x.Candidate)
            .ThenInclude(x => x.Batch)
            .Where(x => x.PushStatus == ScorePushStatus.NotPushed || x.PushStatus == ScorePushStatus.Failed);

        if (scoreIds is { Count: > 0 })
        {
            query = query.Where(x => scoreIds.Contains(x.Id));
        }

        if (!string.IsNullOrWhiteSpace(batchCode))
        {
            query = query.Where(x => x.Candidate.Batch.BatchCode == batchCode);
        }

        if (!string.IsNullOrWhiteSpace(subject) && Enum.TryParse<ExamSubject>(subject, true, out var sub))
        {
            query = query.Where(x => x.Subject == sub);
        }

        var pending = await query.OrderBy(x => x.Id).ToListAsync(ct);
        var result = new PushResultDto();

        foreach (var chunk in pending.Chunk(_options.BatchSize))
        {
            foreach (var score in chunk)
            {
                score.PushStatus = ScorePushStatus.Pushing;
            }
            await _db.SaveChangesAsync(ct);

            var items = chunk.Select(s => new JavaGradeCallbackItemDto
            {
                StudentId = s.StudentId,
                Subject = s.Subject.ToString(),
                Situation = s.Situation.ToString(),
                Score = s.Score,
                AnswerUrl = s.AnswerUrl
            }).ToList();

            var (success, responseBody, requestBody, sign, timestamp) =
                await _client.PushAsync(items, ct);

            var attemptBase = await _db.ScorePushLogs
                .Where(x => chunk.Select(c => c.Id).Contains(x.ExamScoreId))
                .Select(x => x.AttemptNo)
                .DefaultIfEmpty(0)
                .MaxAsync(ct);

            foreach (var score in chunk)
            {
                var attempt = attemptBase + 1;
                _db.ScorePushLogs.Add(new Domain.Entities.ScorePushLog
                {
                    ExamScoreId = score.Id,
                    AttemptNo = attempt,
                    Success = success,
                    RequestBody = requestBody,
                    SignHeader = sign,
                    TimestampHeader = timestamp,
                    ResponseBody = responseBody,
                    ErrorMessage = success ? null : responseBody
                });

                if (success)
                {
                    score.PushStatus = ScorePushStatus.Pushed;
                    score.PushedAt = DateTime.UtcNow;
                    result.SuccessCount++;
                }
                else
                {
                    score.PushStatus = ScorePushStatus.Failed;
                    result.FailedCount++;
                }
            }

            await _db.SaveChangesAsync(ct);
        }

        return result;
    }
}
