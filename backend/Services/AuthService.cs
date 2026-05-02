using MentorReservation.Api.Data;
using MentorReservation.Api.DTOs;
using MentorReservation.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MentorReservation.Api.Services;

public class AuthService(AppDbContext db, PasswordHasher<AppUser> passwordHasher)
{
    public async Task<UserDto> RegisterStudentAsync(RegisterStudentDto dto)
    {
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
        var enrollmentNumber = dto.EnrollmentNumber.Trim();

        if (await db.Users.AnyAsync(user => user.Email.ToLower() == normalizedEmail))
        {
            throw new InvalidOperationException("A user with this email already exists.");
        }

        if (await db.StudentProfiles.AnyAsync(profile => profile.EnrollmentNumber == enrollmentNumber))
        {
            throw new InvalidOperationException("A student with this enrollment number already exists.");
        }

        var now = DateTime.UtcNow;
        var user = new AppUser
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = normalizedEmail,
            Role = UserRole.Student,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        user.PasswordHash = passwordHasher.HashPassword(user, dto.Password);

        user.StudentProfile = new StudentProfile
        {
            EnrollmentNumber = enrollmentNumber,
            StudyProgram = dto.StudyProgram,
            YearOfStudy = dto.YearOfStudy,
            CreatedAt = now,
            UpdatedAt = now
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return ToUserDto(user);
    }

    public async Task<UserDto> LoginAsync(LoginDto dto)
    {
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
        var user = await db.Users.SingleOrDefaultAsync(item => item.Email.ToLower() == normalizedEmail);

        if (user is null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        return ToUserDto(user);
    }

    private static UserDto ToUserDto(AppUser user) =>
        new(user.Id, user.FirstName, user.LastName, user.FullName, user.Email, user.Role, user.IsActive);
}
