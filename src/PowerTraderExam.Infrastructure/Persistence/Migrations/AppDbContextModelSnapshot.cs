using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PowerTraderExam.Infrastructure.Persistence;

#nullable disable

namespace PowerTraderExam.Infrastructure.Persistence.Migrations;

[DbContext(typeof(AppDbContext))]
partial class AppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasAnnotation("ProductVersion", "6.0.29")
            .HasAnnotation("Relational:MaxIdentifierLength", 64);

        modelBuilder.Entity("PowerTraderExam.Domain.Entities.Candidate", b =>
        {
            b.Property<long>("Id").ValueGeneratedOnAdd().HasColumnType("bigint");
            b.Property<long>("BatchId").HasColumnType("bigint");
            b.Property<string>("CandidateNo").IsRequired().HasMaxLength(64).HasColumnType("varchar(64)");
            b.Property<DateTime>("CreatedAt").HasColumnType("datetime(6)");
            b.Property<string>("IdCard").HasMaxLength(32).HasColumnType("varchar(32)");
            b.Property<string>("Name").IsRequired().HasMaxLength(128).HasColumnType("varchar(128)");
            b.Property<string>("OrgName").HasMaxLength(256).HasColumnType("varchar(256)");
            b.Property<string>("StudentId").IsRequired().HasMaxLength(64).HasColumnType("varchar(64)");
            b.Property<int>("SyncStatus").HasColumnType("int");
            b.HasKey("Id");
            b.HasIndex("BatchId", "CandidateNo").IsUnique();
            b.HasIndex("BatchId", "SyncStatus");
            b.ToTable("candidates");
        });

        modelBuilder.Entity("PowerTraderExam.Domain.Entities.ExamBatch", b =>
        {
            b.Property<long>("Id").ValueGeneratedOnAdd().HasColumnType("bigint");
            b.Property<string>("BatchCode").IsRequired().HasMaxLength(64).HasColumnType("varchar(64)");
            b.Property<string>("BatchName").IsRequired().HasMaxLength(256).HasColumnType("varchar(256)");
            b.Property<DateTime>("CreatedAt").HasColumnType("datetime(6)");
            b.Property<DateTime?>("ExamDate").HasColumnType("datetime(6)");
            b.Property<int>("Status").HasColumnType("int");
            b.HasKey("Id");
            b.HasIndex("BatchCode").IsUnique();
            b.ToTable("exam_batches");
        });
    }
}
