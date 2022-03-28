using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Hire360WebAPI.Models
{
    public partial class Hire360Context : DbContext
    {
        public Hire360Context()
        {
        }

        public Hire360Context(DbContextOptions<Hire360Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Candidate> Candidates { get; set; } = null!;
        public virtual DbSet<HumanResource> HumanResources { get; set; } = null!;
        public virtual DbSet<Job> Jobs { get; set; } = null!;
        public virtual DbSet<JobApplied> JobApplieds { get; set; } = null!;
        public virtual DbSet<Qualification> Qualifications { get; set; } = null!;
        public virtual DbSet<Skill> Skills { get; set; } = null!;
        public virtual DbSet<SkillSet> SkillSets { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("ConnectionString");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Candidate>(entity =>
            {
                entity.ToTable("Candidate");

                entity.HasIndex(e => e.CandidateEmail, "UQ__Candidat__CFAF32FE7CC04222")
                    .IsUnique();

                entity.Property(e => e.CandidateId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Active)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('ACTIVE')");

                entity.Property(e => e.CandidateCity)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CandidateDescription)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CandidateEmail)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CandidateName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CandidatePassword)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CandidatePhoneNumber)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CandidatePhotoUrl)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CandidateResume)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.CandidateTotalExp)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HumanResource>(entity =>
            {
                entity.HasKey(e => e.Hrid)
                    .HasName("PK__HumanRes__14F5A1B5BED2565A");

                entity.ToTable("HumanResource");

                entity.HasIndex(e => e.Hremail, "UQ__HumanRes__73F839FEA7A4101A")
                    .IsUnique();

                entity.Property(e => e.Hrid)
                    .HasColumnName("HRId")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Active)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('ACTIVE')");

                entity.Property(e => e.City)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CompanyName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Hrdescription)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("HRDescription");

                entity.Property(e => e.Hremail)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("HREmail");

                entity.Property(e => e.Hrname)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("HRName");

                entity.Property(e => e.Hrpassword)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("HRPassword");

                entity.Property(e => e.HrphoneNumber)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("HRPhoneNumber");

                entity.Property(e => e.UserRole).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.ToTable("Job");

                entity.Property(e => e.JobId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Active)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('ACTIVE')");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Hrid).HasColumnName("HRId");

                entity.Property(e => e.JobCity)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.JobDescription)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.JobTitle)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Hr)
                    .WithMany(p => p.Jobs)
                    .HasForeignKey(d => d.Hrid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Job__HRId__7A672E12");
            });

            modelBuilder.Entity<JobApplied>(entity =>
            {
                entity.ToTable("JobApplied");

                entity.Property(e => e.JobAppliedId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Active)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('ACTIVE')");

                entity.Property(e => e.AppliedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Candidate)
                    .WithMany(p => p.JobApplieds)
                    .HasForeignKey(d => d.CandidateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__JobApplie__Candi__01142BA1");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobApplieds)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__JobApplie__JobId__00200768");
            });

            modelBuilder.Entity<Qualification>(entity =>
            {
                entity.ToTable("Qualification");

                entity.Property(e => e.QualificationId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Active)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('ACTIVE')");

                entity.Property(e => e.DegreeName)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.YearOfGraduation)
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.HasOne(d => d.Candidate)
                    .WithMany(p => p.Qualifications)
                    .HasForeignKey(d => d.CandidateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Qualifica__Candi__74AE54BC");
            });

            modelBuilder.Entity<Skill>(entity =>
            {
                entity.ToTable("Skill");

                entity.Property(e => e.SkillId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Active)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('ACTIVE')");

                entity.Property(e => e.SkillLevel)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Candidate)
                    .WithMany(p => p.Skills)
                    .HasForeignKey(d => d.CandidateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Skill__Candidate__6EF57B66");

                entity.HasOne(d => d.SkillSet)
                    .WithMany(p => p.Skills)
                    .HasForeignKey(d => d.SkillSetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Skill__SkillSetI__6FE99F9F");
            });

            modelBuilder.Entity<SkillSet>(entity =>
            {
                entity.ToTable("SkillSet");

                entity.Property(e => e.SkillSetId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Active)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('ACTIVE')");

                entity.Property(e => e.SkillSetName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
