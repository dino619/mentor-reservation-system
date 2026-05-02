using MentorReservation.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MentorReservation.Api.Data;

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext db, PasswordHasher<AppUser> passwordHasher)
    {
        if (await db.Users.AnyAsync())
        {
            return;
        }

        var now = DateTime.UtcNow;
        var dino = CreateUser(passwordHasher, "Dino", "Džaferagić", "dino.dzaferagic@student.uni-lj.si", UserRole.Student, "Password123!");
        var tibor = CreateUser(passwordHasher, "Tibor", "Koderman", "tibor.koderman@student.uni-lj.si", UserRole.Student, "Password123!");
        var jerneja = CreateUser(passwordHasher, "Jerneja", "Krajcar", "jerneja.krajcar@student.uni-lj.si", UserRole.Student, "Password123!");
        var maja = CreateUser(passwordHasher, "Maja", "Vidmar", "maja.vidmar@student.uni-lj.si", UserRole.Student, "Password123!");
        var mentorDemo = CreateUser(passwordHasher, "Demo", "Mentor", "mentor.demo@fri.uni-lj.si", UserRole.Mentor, "Password123!");
        var admin = CreateUser(passwordHasher, "Demo", "Admin", "admin@mentor-reservation.local", UserRole.Admin, "Password123!");

        dino.StudentProfile = new StudentProfile
        {
            EnrollmentNumber = "63240001",
            StudyProgram = StudyProgram.UNI,
            YearOfStudy = 3,
            CreatedAt = now,
            UpdatedAt = now
        };

        tibor.StudentProfile = new StudentProfile
        {
            EnrollmentNumber = "63240002",
            StudyProgram = StudyProgram.UNI,
            YearOfStudy = 3,
            CreatedAt = now,
            UpdatedAt = now
        };

        jerneja.StudentProfile = new StudentProfile
        {
            EnrollmentNumber = "63240003",
            StudyProgram = StudyProgram.UNI,
            YearOfStudy = 3,
            CreatedAt = now,
            UpdatedAt = now
        };

        maja.StudentProfile = new StudentProfile
        {
            EnrollmentNumber = "63240004",
            StudyProgram = StudyProgram.VSS,
            YearOfStudy = 3,
            CreatedAt = now,
            UpdatedAt = now
        };

        db.Users.AddRange(dino, tibor, jerneja, maja, mentorDemo, admin);
        await db.SaveChangesAsync();
    }

    private static AppUser CreateUser(
        PasswordHasher<AppUser> passwordHasher,
        string firstName,
        string lastName,
        string email,
        UserRole role,
        string password)
    {
        var now = DateTime.UtcNow;
        var user = new AppUser
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email.ToLowerInvariant(),
            Role = role,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        user.PasswordHash = passwordHasher.HashPassword(user, password);
        return user;
    }
}
