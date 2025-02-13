using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DemoWebApplication.Models;

public partial class test1Context : DbContext
{
    public test1Context(DbContextOptions<test1Context> options)
        : base(options)
    {
    }

    public virtual DbSet<answers> answers { get; set; }

    public virtual DbSet<evaluations> evaluations { get; set; }

    public virtual DbSet<questions> questions { get; set; }

    public virtual DbSet<users> users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<answers>(entity =>
        {
            entity.HasKey(e => e.answer_id).HasName("PRIMARY");

            entity.HasIndex(e => e.question_id, "question_id");

            entity.HasIndex(e => e.user_id, "user_id");
            
            entity.HasIndex(e => e.survey_id, "survey_id");  // 新增 survey_id 索引

            entity.Property(e => e.answer).HasMaxLength(255);
            entity.Property(e => e.answered_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");

            // 設定 survey_id 的關聯
            entity.Property(e => e.survey_id)  // 新增 survey_id
                .HasMaxLength(255)
                .IsRequired();
            
            entity.HasOne(d => d.question).WithMany(p => p.answers)
                .HasForeignKey(d => d.question_id)
                .HasConstraintName("answers_ibfk_2");

            entity.HasOne(d => d.user).WithMany(p => p.answers)
                .HasForeignKey(d => d.user_id)
                .HasConstraintName("answers_ibfk_1");
        });

        modelBuilder.Entity<evaluations>(entity =>
        {
            entity.HasKey(e => e.evaluation_id).HasName("PRIMARY");

            entity.HasIndex(e => e.user_id, "user_id");

            entity.Property(e => e.evaluated_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.recommendation).HasMaxLength(255);

            entity.HasOne(d => d.user).WithMany(p => p.evaluations)
                .HasForeignKey(d => d.user_id)
                .HasConstraintName("evaluations_ibfk_1");
        });

        modelBuilder.Entity<questions>(entity =>
        {
            entity.HasKey(e => e.question_id).HasName("PRIMARY");

            entity.HasIndex(e => e.created_by, "created_by");

            entity.Property(e => e.category).HasMaxLength(255);
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.difficulty_level)
                .HasMaxLength(255)
                .HasDefaultValueSql("'Medium'");
            entity.Property(e => e.question_text).HasMaxLength(255);
            entity.Property(e => e.updated_at)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");

            entity.HasOne(d => d.created_byNavigation).WithMany(p => p.questions)
                .HasForeignKey(d => d.created_by)
                .HasConstraintName("questions_ibfk_1");
        });

        modelBuilder.Entity<users>(entity =>
        {
            entity.HasKey(e => e.user_id).HasName("PRIMARY");

            entity.HasIndex(e => e.email, "email").IsUnique();

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.password_hash).HasMaxLength(255);
            entity.Property(e => e.updated_at)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.username).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
