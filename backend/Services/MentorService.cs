using MentorReservation.Api.Data;
using MentorReservation.Api.DTOs;
using MentorReservation.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MentorReservation.Api.Services;

public class MentorService(AppDbContext db)
{
    public async Task<List<MentorDto>> GetMentorsAsync(string? search)
    {
        var normalizedSearch = search?.Trim().ToLower();

        var mentors = await db.MentorProfiles
            .AsNoTracking()
            .Include(profile => profile.User)
            .Include(profile => profile.Requests)
            .OrderBy(profile => profile.User.FullName)
            .ToListAsync();

        if (!string.IsNullOrWhiteSpace(normalizedSearch))
        {
            mentors = mentors
                .Where(profile =>
                    profile.User.FullName.ToLower().Contains(normalizedSearch) ||
                    profile.Laboratory.ToLower().Contains(normalizedSearch) ||
                    profile.ResearchAreas.Any(area => area.ToLower().Contains(normalizedSearch)))
                .ToList();
        }

        return mentors.Select(ToMentorDto).ToList();
    }

    public async Task<MentorDto?> GetMentorAsync(int id)
    {
        var mentor = await db.MentorProfiles
            .AsNoTracking()
            .Include(profile => profile.User)
            .Include(profile => profile.Requests)
            .SingleOrDefaultAsync(profile => profile.Id == id);

        return mentor is null ? null : ToMentorDto(mentor);
    }

    public async Task<List<MentorshipRequestDto>> GetMentorRequestsAsync(int mentorId)
    {
        var exists = await db.MentorProfiles.AnyAsync(profile => profile.Id == mentorId);

        if (!exists)
        {
            throw new KeyNotFoundException("Mentor was not found.");
        }

        return await db.MentorshipRequests
            .AsNoTracking()
            .Include(request => request.Student)
            .Include(request => request.Mentor)
            .ThenInclude(mentor => mentor.User)
            .Where(request => request.MentorId == mentorId)
            .OrderByDescending(request => request.CreatedAt)
            .Select(request => ToRequestDto(request))
            .ToListAsync();
    }

    private static MentorDto ToMentorDto(MentorProfile profile)
    {
        var accepted = profile.Requests.Count(request => request.Status == RequestStatus.Accepted);

        return new MentorDto(
            profile.Id,
            profile.UserId,
            profile.User.FullName,
            profile.User.Email,
            profile.Laboratory,
            profile.ResearchAreas,
            profile.MaxStudents,
            accepted,
            Math.Max(profile.MaxStudents - accepted, 0));
    }

    private static MentorshipRequestDto ToRequestDto(MentorshipRequest request) =>
        new(
            request.Id,
            request.StudentId,
            request.Student.FullName,
            request.Student.Email,
            request.MentorId,
            request.Mentor.User.FullName,
            request.Mentor.User.Email,
            request.ProposedTitle,
            request.Description,
            request.OptionalMessage,
            request.Status,
            request.MentorComment,
            request.CreatedAt,
            request.UpdatedAt);
}
