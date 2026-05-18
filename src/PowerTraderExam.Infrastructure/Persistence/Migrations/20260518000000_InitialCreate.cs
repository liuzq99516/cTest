using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PowerTraderExam.Infrastructure.Persistence.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "exam_batches",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                BatchCode = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                BatchName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                ExamDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                Status = table.Column<int>(type: "int", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_exam_batches", x => x.Id))
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "exam_servers",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                ServerId = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                ServerName = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                IsEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_exam_servers", x => x.Id))
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "candidates",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                BatchId = table.Column<long>(type: "bigint", nullable: false),
                CandidateNo = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                StudentId = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                IdCard = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                OrgName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                SyncStatus = table.Column<int>(type: "int", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_candidates", x => x.Id);
                table.ForeignKey(
                    name: "FK_candidates_exam_batches_BatchId",
                    column: x => x.BatchId,
                    principalTable: "exam_batches",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "candidate_sync_logs",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                CandidateId = table.Column<long>(type: "bigint", nullable: false),
                ServerId = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                SyncedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                Remark = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_candidate_sync_logs", x => x.Id);
                table.ForeignKey(
                    name: "FK_candidate_sync_logs_candidates_CandidateId",
                    column: x => x.CandidateId,
                    principalTable: "candidates",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "exam_scores",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                CandidateId = table.Column<long>(type: "bigint", nullable: false),
                StudentId = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Subject = table.Column<string>(type: "varchar(16)", maxLength: 16, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Situation = table.Column<string>(type: "varchar(16)", maxLength: 16, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Score = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                AnswerUrl = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                DetailJson = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                SourceServerId = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                ScoredAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                PushStatus = table.Column<int>(type: "int", nullable: false),
                PushedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_exam_scores", x => x.Id);
                table.ForeignKey(
                    name: "FK_exam_scores_candidates_CandidateId",
                    column: x => x.CandidateId,
                    principalTable: "candidates",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "score_push_logs",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                ExamScoreId = table.Column<long>(type: "bigint", nullable: false),
                AttemptNo = table.Column<int>(type: "int", nullable: false),
                Success = table.Column<bool>(type: "tinyint(1)", nullable: false),
                RequestBody = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                SignHeader = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                TimestampHeader = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                ResponseBody = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                ErrorMessage = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_score_push_logs", x => x.Id);
                table.ForeignKey(
                    name: "FK_score_push_logs_exam_scores_ExamScoreId",
                    column: x => x.ExamScoreId,
                    principalTable: "exam_scores",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateIndex(name: "IX_exam_batches_BatchCode", table: "exam_batches", column: "BatchCode", unique: true);
        migrationBuilder.CreateIndex(name: "IX_exam_servers_ServerId", table: "exam_servers", column: "ServerId", unique: true);
        migrationBuilder.CreateIndex(name: "IX_candidates_BatchId_CandidateNo", table: "candidates", columns: new[] { "BatchId", "CandidateNo" }, unique: true);
        migrationBuilder.CreateIndex(name: "IX_candidates_BatchId_SyncStatus", table: "candidates", columns: new[] { "BatchId", "SyncStatus" });
        migrationBuilder.CreateIndex(name: "IX_candidate_sync_logs_CandidateId_ServerId", table: "candidate_sync_logs", columns: new[] { "CandidateId", "ServerId" }, unique: true);
        migrationBuilder.CreateIndex(name: "IX_exam_scores_CandidateId_Subject", table: "exam_scores", columns: new[] { "CandidateId", "Subject" }, unique: true);
        migrationBuilder.CreateIndex(name: "IX_exam_scores_PushStatus_ScoredAt", table: "exam_scores", columns: new[] { "PushStatus", "ScoredAt" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "score_push_logs");
        migrationBuilder.DropTable(name: "candidate_sync_logs");
        migrationBuilder.DropTable(name: "exam_scores");
        migrationBuilder.DropTable(name: "exam_servers");
        migrationBuilder.DropTable(name: "candidates");
        migrationBuilder.DropTable(name: "exam_batches");
    }
}
