using MentorReservation.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MentorReservation.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<MentorProfile> MentorProfiles => Set<MentorProfile>();
    public DbSet<MentorshipRequest> MentorshipRequests => Set<MentorshipRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasIndex(user => user.Email).IsUnique();
            entity.Property(user => user.Role).HasConversion<string>();
        });

        modelBuilder.Entity<MentorProfile>(entity =>
        {
            entity.HasOne(profile => profile.User)
                .WithOne(user => user.MentorProfile)
                .HasForeignKey<MentorProfile>(profile => profile.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(profile => profile.ResearchAreas)
                .HasColumnType("text[]");
        });

        modelBuilder.Entity<MentorshipRequest>(entity =>
        {
            entity.Property(request => request.Status).HasConversion<string>();

            entity.HasOne(request => request.Student)
                .WithMany(user => user.StudentRequests)
                .HasForeignKey(request => request.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(request => request.Mentor)
                .WithMany(profile => profile.Requests)
                .HasForeignKey(request => request.MentorId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(request => new { request.StudentId, request.MentorId, request.Status });
        });
    }
}
