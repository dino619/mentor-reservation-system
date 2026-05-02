using MentorReservation.Api.Data;
using MentorReservation.Api.DTOs;
using MentorReservation.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MentorReservation.Api.Services;

public class MentorService(AppDbContext db)
{
    public async Task<List<MentorDto>> GetMentorsAsync(string? search)
    {
        var normalizedSearch = search?.Trim().ToLowerInvariant();

        var mentors = await db.MentorProfiles
            .AsNoTracking()
            .Include(profile => profile.Requests)
            .Include(profile => profile.MentorResearchAreas)
            .ThenInclude(item => item.ResearchArea)
            .OrderBy(profile => profile.LastName)
            .ThenBy(profile => profile.FirstName)
            .ToListAsync();

        if (!string.IsNullOrWhiteSpace(normalizedSearch))
        {
            mentors = mentors
                .Where(profile =>
                    profile.FullName.ToLowerInvariant().Contains(normalizedSearch) ||
                    (profile.Title ?? string.Empty).ToLowerInvariant().Contains(normalizedSearch) ||
                    (profile.Email ?? string.Empty).ToLowerInvariant().Contains(normalizedSearch) ||
                    (profile.Laboratory ?? string.Empty).ToLowerInvariant().Contains(normalizedSearch) ||
                    profile.MentorResearchAreas.Any(item =>
                        item.ResearchArea.Name.ToLowerInvariant().Contains(normalizedSearch)))
                .ToList();
        }

        return mentors.Select(ToMentorDto).ToList();
    }

    public async Task<MentorDto?> GetMentorAsync(int id)
    {
        var mentor = await db.MentorProfiles
            .AsNoTracking()
            .Include(profile => profile.Requests)
            .Include(profile => profile.MentorResearchAreas)
            .ThenInclude(item => item.ResearchArea)
            .SingleOrDefaultAsync(profile => profile.Id == id);

        return mentor is null ? null : ToMentorDto(mentor);
    }

    public async Task<List<MentorshipRequestDto>> GetMentorRequestsAsync(int mentorProfileId)
    {
        var exists = await db.MentorProfiles.AnyAsync(profile => profile.Id == mentorProfileId);

        if (!exists)
        {
            throw new KeyNotFoundException("Mentor was not found.");
        }

        return await db.MentorshipRequests
            .AsNoTracking()
            .Include(request => request.Student)
            .ThenInclude(student => student.StudentProfile)
            .Include(request => request.MentorProfile)
            .Where(request => request.MentorProfileId == mentorProfileId)
            .OrderByDescending(request => request.CreatedAt)
            .Select(request => ToRequestDto(request))
            .ToListAsync();
    }

    public static MentorDto ToMentorDto(MentorProfile profile)
    {
        var accepted = profile.Requests.Count(request => request.Status == RequestStatus.Accepted);
        var areas = profile.MentorResearchAreas
            .Select(item => item.ResearchArea.Name)
            .OrderBy(name => name)
            .ToList();

        return new MentorDto(
            profile.Id,
            profile.UserId,
            profile.FirstName,
            profile.LastName,
            profile.FullName,
            profile.Title,
            profile.Email,
            profile.ProfileUrl,
            profile.Laboratory,
            profile.Office,
            profile.Phone,
            areas,
            profile.MaxStudents,
            accepted,
            Math.Max(profile.MaxStudents - accepted, 0),
            profile.IsAvailable,
            profile.Source,
            profile.ImportedAt);
    }

    public static MentorshipRequestDto ToRequestDto(MentorshipRequest request) =>
        new(
            request.Id,
            request.StudentId,
            request.Student.FullName,
            request.Student.Email,
            request.Student.StudentProfile?.EnrollmentNumber,
            request.Student.StudentProfile?.StudyProgram,
            request.MentorProfileId,
            request.MentorProfile.FullName,
            request.MentorProfile.Email,
            request.ProposedTitle,
            request.Description,
            request.OptionalMessage,
            request.Status,
            request.MentorResponse,
            request.CreatedAt,
            request.UpdatedAt,
            request.DecidedAt);
}
