using MentorReservation.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MentorReservation.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<StudentProfile> StudentProfiles => Set<StudentProfile>();
    public DbSet<MentorProfile> MentorProfiles => Set<MentorProfile>();
    public DbSet<ResearchArea> ResearchAreas => Set<ResearchArea>();
    public DbSet<MentorResearchArea> MentorResearchAreas => Set<MentorResearchArea>();
    public DbSet<MentorshipRequest> MentorshipRequests => Set<MentorshipRequest>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<EmailOutbox> EmailOutbox => Set<EmailOutbox>();
    public DbSet<MentorImportRun> MentorImportRuns => Set<MentorImportRun>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasIndex(user => user.Email).IsUnique();
            entity.Property(user => user.Role).HasConversion<string>();
        });

        modelBuilder.Entity<StudentProfile>(entity =>
        {
            entity.HasIndex(profile => profile.EnrollmentNumber).IsUnique();
            entity.Property(profile => profile.StudyProgram).HasConversion<string>();

            entity.HasOne(profile => profile.User)
                .WithOne(user => user.StudentProfile)
                .HasForeignKey<StudentProfile>(profile => profile.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MentorProfile>(entity =>
        {
            entity.HasIndex(profile => profile.UserId).IsUnique();
            entity.HasIndex(profile => profile.Email);
            entity.HasIndex(profile => profile.ProfileUrl).IsUnique();
            entity.HasIndex(profile => profile.SourceExternalId);

            entity.HasOne(profile => profile.User)
                .WithOne(user => user.MentorProfile)
                .HasForeignKey<MentorProfile>(profile => profile.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ResearchArea>(entity =>
        {
            entity.HasIndex(area => area.Name).IsUnique();
        });

        modelBuilder.Entity<MentorResearchArea>(entity =>
        {
            entity.HasKey(item => new { item.MentorProfileId, item.ResearchAreaId });

            entity.HasOne(item => item.MentorProfile)
                .WithMany(profile => profile.MentorResearchAreas)
                .HasForeignKey(item => item.MentorProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(item => item.ResearchArea)
                .WithMany(area => area.MentorResearchAreas)
                .HasForeignKey(item => item.ResearchAreaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MentorshipRequest>(entity =>
        {
            entity.Property(request => request.Status).HasConversion<string>();

            entity.HasOne(request => request.Student)
                .WithMany(user => user.StudentRequests)
                .HasForeignKey(request => request.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(request => request.MentorProfile)
                .WithMany(profile => profile.Requests)
                .HasForeignKey(request => request.MentorProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(request => new { request.StudentId, request.MentorProfileId, request.Status });
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.Property(notification => notification.Type).HasConversion<string>();

            entity.HasOne(notification => notification.User)
                .WithMany(user => user.Notifications)
                .HasForeignKey(notification => notification.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(notification => notification.RelatedRequest)
                .WithMany()
                .HasForeignKey(notification => notification.RelatedRequestId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<EmailOutbox>(entity =>
        {
            entity.Property(email => email.Status).HasConversion<string>();
        });

        modelBuilder.Entity<MentorImportRun>(entity =>
        {
            entity.Property(run => run.Status).HasConversion<string>();
        });
    }
}
