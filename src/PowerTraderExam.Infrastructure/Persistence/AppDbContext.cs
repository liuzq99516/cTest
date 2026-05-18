using Microsoft.EntityFrameworkCore;
using PowerTraderExam.Domain.Entities;

namespace PowerTraderExam.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<ExamBatch> ExamBatches => Set<ExamBatch>();
    public DbSet<Candidate> Candidates => Set<Candidate>();
    public DbSet<ExamServer> ExamServers => Set<ExamServer>();
    public DbSet<CandidateSyncLog> CandidateSyncLogs => Set<CandidateSyncLog>();
    public DbSet<ExamScore> ExamScores => Set<ExamScore>();
    public DbSet<ScorePushLog> ScorePushLogs => Set<ScorePushLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExamBatch>(e =>
        {
            e.ToTable("exam_batches");
            e.HasIndex(x => x.BatchCode).IsUnique();
            e.Property(x => x.BatchCode).HasMaxLength(64);
            e.Property(x => x.BatchName).HasMaxLength(256);
        });

        modelBuilder.Entity<Candidate>(e =>
        {
            e.ToTable("candidates");
            e.HasIndex(x => new { x.BatchId, x.CandidateNo }).IsUnique();
            e.HasIndex(x => new { x.BatchId, x.SyncStatus });
            e.Property(x => x.CandidateNo).HasMaxLength(64);
            e.Property(x => x.StudentId).HasMaxLength(64);
            e.Property(x => x.Name).HasMaxLength(128);
            e.Property(x => x.IdCard).HasMaxLength(32);
            e.Property(x => x.OrgName).HasMaxLength(256);
        });

        modelBuilder.Entity<ExamServer>(e =>
        {
            e.ToTable("exam_servers");
            e.HasIndex(x => x.ServerId).IsUnique();
            e.Property(x => x.ServerId).HasMaxLength(64);
            e.Property(x => x.ServerName).HasMaxLength(128);
        });

        modelBuilder.Entity<CandidateSyncLog>(e =>
        {
            e.ToTable("candidate_sync_logs");
            e.HasIndex(x => new { x.CandidateId, x.ServerId }).IsUnique();
            e.Property(x => x.ServerId).HasMaxLength(64);
        });

        modelBuilder.Entity<ExamScore>(e =>
        {
            e.ToTable("exam_scores");
            e.HasIndex(x => new { x.CandidateId, x.Subject }).IsUnique();
            e.HasIndex(x => new { x.PushStatus, x.ScoredAt });
            e.Property(x => x.StudentId).HasMaxLength(64);
            e.Property(x => x.AnswerUrl).HasMaxLength(1024);
            e.Property(x => x.SourceServerId).HasMaxLength(64);
            e.Property(x => x.Subject).HasConversion<string>().HasMaxLength(16);
            e.Property(x => x.Situation).HasConversion<string>().HasMaxLength(16);
        });

        modelBuilder.Entity<ScorePushLog>(e =>
        {
            e.ToTable("score_push_logs");
            e.Property(x => x.RequestBody).HasColumnType("longtext");
            e.Property(x => x.ResponseBody).HasColumnType("longtext");
        });
    }
}
