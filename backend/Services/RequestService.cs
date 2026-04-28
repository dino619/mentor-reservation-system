using MentorReservation.Api.Data;
using MentorReservation.Api.DTOs;
using MentorReservation.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MentorReservation.Api.Services;

public class RequestService(AppDbContext db)
{
    public async Task<List<MentorshipRequestDto>> GetStudentRequestsAsync(int studentId)
    {
        var studentExists = await db.Users.AnyAsync(user => user.Id == studentId && user.Role == UserRole.Student);

        if (!studentExists)
        {
            throw new KeyNotFoundException("Student was not found.");
        }

        return await db.MentorshipRequests
            .AsNoTracking()
            .Include(request => request.Student)
            .Include(request => request.Mentor)
            .ThenInclude(mentor => mentor.User)
            .Where(request => request.StudentId == studentId)
            .OrderByDescending(request => request.CreatedAt)
            .Select(request => ToRequestDto(request))
            .ToListAsync();
    }

    public async Task<MentorshipRequestDto> CreateRequestAsync(CreateMentorshipRequestDto dto)
    {
        var student = await db.Users.SingleOrDefaultAsync(user => user.Id == dto.StudentId);

        if (student is null || student.Role != UserRole.Student)
        {
            throw new KeyNotFoundException("Student was not found.");
        }

        var mentor = await db.MentorProfiles
            .Include(profile => profile.User)
            .Include(profile => profile.Requests)
            .SingleOrDefaultAsync(profile => profile.Id == dto.MentorId);

        if (mentor is null)
        {
            throw new KeyNotFoundException("Mentor was not found.");
        }

        var acceptedCount = mentor.Requests.Count(request => request.Status == RequestStatus.Accepted);

        if (acceptedCount >= mentor.MaxStudents)
        {
            throw new InvalidOperationException("This mentor has no available supervision slots.");
        }

        var hasActiveRequest = await db.MentorshipRequests.AnyAsync(request =>
            request.StudentId == dto.StudentId &&
            request.MentorId == dto.MentorId &&
            (request.Status == RequestStatus.Pending || request.Status == RequestStatus.Accepted));

        if (hasActiveRequest)
        {
            throw new InvalidOperationException("You already have an active request with this mentor.");
        }

        var now = DateTime.UtcNow;
        var request = new MentorshipRequest
        {
            StudentId = dto.StudentId,
            MentorId = dto.MentorId,
            ProposedTitle = dto.ProposedTitle.Trim(),
            Description = dto.Description.Trim(),
            OptionalMessage = string.IsNullOrWhiteSpace(dto.OptionalMessage) ? null : dto.OptionalMessage.Trim(),
            Status = RequestStatus.Pending,
            CreatedAt = now,
            UpdatedAt = now
        };

        db.MentorshipRequests.Add(request);
        await db.SaveChangesAsync();

        return await GetRequestAsync(request.Id);
    }

    public async Task<MentorshipRequestDto> AcceptRequestAsync(int requestId, string? comment)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();
        var request = await FindRequestForUpdateAsync(requestId);

        if (request.Status != RequestStatus.Pending)
        {
            throw new InvalidOperationException("Only pending requests can be accepted.");
        }

        var acceptedCount = await db.MentorshipRequests.CountAsync(existing =>
            existing.MentorId == request.MentorId &&
            existing.Status == RequestStatus.Accepted);

        if (acceptedCount >= request.Mentor.MaxStudents)
        {
            throw new InvalidOperationException("The mentor has already reached the maximum number of accepted students.");
        }

        var studentAlreadyAccepted = await db.MentorshipRequests.AnyAsync(existing =>
            existing.StudentId == request.StudentId &&
            existing.Status == RequestStatus.Accepted);

        if (studentAlreadyAccepted)
        {
            throw new InvalidOperationException("This student already has an accepted mentorship request.");
        }

        request.Status = RequestStatus.Accepted;
        request.MentorComment = NormalizeComment(comment);
        request.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        await transaction.CommitAsync();

        return await GetRequestAsync(request.Id);
    }

    public async Task<MentorshipRequestDto> RejectRequestAsync(int requestId, string? comment)
    {
        var request = await FindRequestForUpdateAsync(requestId);

        if (request.Status != RequestStatus.Pending)
        {
            throw new InvalidOperationException("Only pending requests can be rejected.");
        }

        request.Status = RequestStatus.Rejected;
        request.MentorComment = NormalizeComment(comment);
        request.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return await GetRequestAsync(request.Id);
    }

    public async Task<MentorshipRequestDto> CancelRequestAsync(int requestId)
    {
        var request = await FindRequestForUpdateAsync(requestId);

        if (request.Status != RequestStatus.Pending)
        {
            throw new InvalidOperationException("Only pending requests can be cancelled.");
        }

        request.Status = RequestStatus.Cancelled;
        request.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return await GetRequestAsync(request.Id);
    }

    private async Task<MentorshipRequestDto> GetRequestAsync(int id)
    {
        var request = await db.MentorshipRequests
            .AsNoTracking()
            .Include(item => item.Student)
            .Include(item => item.Mentor)
            .ThenInclude(mentor => mentor.User)
            .SingleAsync(item => item.Id == id);

        return ToRequestDto(request);
    }

    private async Task<MentorshipRequest> FindRequestForUpdateAsync(int requestId)
    {
        var request = await db.MentorshipRequests
            .Include(item => item.Student)
            .Include(item => item.Mentor)
            .ThenInclude(mentor => mentor.User)
            .SingleOrDefaultAsync(item => item.Id == requestId);

        if (request is null)
        {
            throw new KeyNotFoundException("Request was not found.");
        }

        return request;
    }

    private static string? NormalizeComment(string? comment) =>
        string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();

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
