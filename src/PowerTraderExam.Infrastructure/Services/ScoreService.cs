using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using PowerTraderExam.Application.Common;
using PowerTraderExam.Application.DTOs.Scores;
using PowerTraderExam.Application.Interfaces;
using PowerTraderExam.Domain.Entities;
using PowerTraderExam.Domain.Enums;
using PowerTraderExam.Infrastructure.Persistence;

namespace PowerTraderExam.Infrastructure.Services;

public class ScoreService : IScoreService
{
    private readonly AppDbContext _db;
    private readonly IJavaScorePushService _pushService;

    public ScoreService(AppDbContext db, IJavaScorePushService pushService)
    {
        _db = db;
        _pushService = pushService;
    }

    public async Task<long> SubmitScoreAsync(string serverId, SubmitScoreRequest request, CancellationToken ct = default)
    {
        if (!Enum.TryParse<ExamSubject>(request.Subject, true, out var subject))
        {
            throw new ArgumentException($"无效的 subject: {request.Subject}");
        }

        if (!Enum.TryParse<ExamSituation>(request.Situation, true, out var situation))
        {
            throw new ArgumentException($"无效的 situation: {request.Situation}");
        }

        var candidate = await _db.Candidates.FirstOrDefaultAsync(x => x.StudentId == request.StudentId, ct)
            ?? await _db.Candidates.FirstOrDefaultAsync(x => x.CandidateNo == request.StudentId, ct);

        if (candidate == null)
        {
            throw new KeyNotFoundException($"未找到考生: {request.StudentId}");
        }

        var synced = await _db.CandidateSyncLogs
            .AnyAsync(x => x.CandidateId == candidate.Id && x.ServerId == serverId, ct);
        if (!synced)
        {
            throw new InvalidOperationException("该考生尚未在本考试服务器完成同步确认，无法回写成绩。");
        }

        var score = await _db.ExamScores
            .FirstOrDefaultAsync(x => x.CandidateId == candidate.Id && x.Subject == subject, ct);

        if (score == null)
        {
            score = new ExamScore
            {
                CandidateId = candidate.Id,
                StudentId = request.StudentId,
                Subject = subject,
                Situation = situation,
                Score = request.Score,
                AnswerUrl = request.AnswerUrl,
                DetailJson = request.DetailJson,
                SourceServerId = serverId,
                ScoredAt = request.ScoredAt ?? DateTime.UtcNow,
                PushStatus = ScorePushStatus.NotPushed
            };
            _db.ExamScores.Add(score);
        }
        else
        {
            score.StudentId = request.StudentId;
            score.Situation = situation;
            score.Score = request.Score;
            score.AnswerUrl = request.AnswerUrl;
            score.DetailJson = request.DetailJson;
            score.SourceServerId = serverId;
            score.ScoredAt = request.ScoredAt ?? DateTime.UtcNow;
            score.PushStatus = ScorePushStatus.NotPushed;
            score.PushedAt = null;
            score.UpdatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(ct);
        return score.Id;
    }

    public async Task<PagedResult<ScoreDto>> QueryScoresAsync(
        string? batchCode,
        string? subject,
        string? pushStatus,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = _db.ExamScores
            .Include(x => x.Candidate)
            .ThenInclude(x => x.Batch)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(batchCode))
        {
            query = query.Where(x => x.Candidate.Batch.BatchCode == batchCode);
        }

        if (!string.IsNullOrWhiteSpace(subject) && Enum.TryParse<ExamSubject>(subject, true, out var sub))
        {
            query = query.Where(x => x.Subject == sub);
        }

        if (!string.IsNullOrWhiteSpace(pushStatus) && Enum.TryParse<ScorePushStatus>(pushStatus, true, out var ps))
        {
            query = query.Where(x => x.PushStatus == ps);
        }

        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(x => x.ScoredAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ScoreDto
            {
                Id = x.Id,
                StudentId = x.StudentId,
                CandidateNo = x.Candidate.CandidateNo,
                Name = x.Candidate.Name,
                BatchCode = x.Candidate.Batch.BatchCode,
                Subject = x.Subject.ToString(),
                Situation = x.Situation.ToString(),
                Score = x.Score,
                AnswerUrl = x.AnswerUrl,
                PushStatus = x.PushStatus.ToString(),
                ScoredAt = x.ScoredAt,
                PushedAt = x.PushedAt
            })
            .ToListAsync(ct);

        return new PagedResult<ScoreDto> { Items = items, Page = page, PageSize = pageSize, Total = total };
    }

    public async Task<byte[]> ExportScoresAsync(
        string? batchCode,
        string? subject,
        string? pushStatus,
        DateTime? from,
        DateTime? to,
        CancellationToken ct = default)
    {
        var query = _db.ExamScores
            .Include(x => x.Candidate)
            .ThenInclude(x => x.Batch)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(batchCode))
            query = query.Where(x => x.Candidate.Batch.BatchCode == batchCode);
        if (!string.IsNullOrWhiteSpace(subject) && Enum.TryParse<ExamSubject>(subject, true, out var sub))
            query = query.Where(x => x.Subject == sub);
        if (!string.IsNullOrWhiteSpace(pushStatus) && Enum.TryParse<ScorePushStatus>(pushStatus, true, out var ps))
            query = query.Where(x => x.PushStatus == ps);
        if (from.HasValue)
            query = query.Where(x => x.ScoredAt >= from.Value);
        if (to.HasValue)
            query = query.Where(x => x.ScoredAt <= to.Value);

        var rows = await query.OrderBy(x => x.Id).ToListAsync(ct);

        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("成绩");
        sheet.Cell(1, 1).Value = "批次";
        sheet.Cell(1, 2).Value = "准考证号";
        sheet.Cell(1, 3).Value = "学员ID";
        sheet.Cell(1, 4).Value = "姓名";
        sheet.Cell(1, 5).Value = "科目";
        sheet.Cell(1, 6).Value = "状态";
        sheet.Cell(1, 7).Value = "分数";
        sheet.Cell(1, 8).Value = "答卷URL";
        sheet.Cell(1, 9).Value = "考试时间";
        sheet.Cell(1, 10).Value = "推送状态";

        var rowIndex = 2;
        foreach (var s in rows)
        {
            sheet.Cell(rowIndex, 1).Value = s.Candidate.Batch.BatchCode;
            sheet.Cell(rowIndex, 2).Value = s.Candidate.CandidateNo;
            sheet.Cell(rowIndex, 3).Value = s.StudentId;
            sheet.Cell(rowIndex, 4).Value = s.Candidate.Name;
            sheet.Cell(rowIndex, 5).Value = s.Subject.ToString();
            sheet.Cell(rowIndex, 6).Value = s.Situation.ToString();
            sheet.Cell(rowIndex, 7).Value = s.Score;
            sheet.Cell(rowIndex, 8).Value = s.AnswerUrl;
            sheet.Cell(rowIndex, 9).Value = s.ScoredAt;
            sheet.Cell(rowIndex, 10).Value = s.PushStatus.ToString();
            rowIndex++;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public Task<PushResultDto> PushScoresAsync(PushScoresRequest request, CancellationToken ct = default) =>
        _pushService.PushPendingScoresAsync(request.BatchCode, request.Subject, request.ScoreIds, ct);
}
