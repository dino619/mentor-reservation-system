using MentorReservation.Api.Data;
using MentorReservation.Api.DTOs;
using MentorReservation.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MentorReservation.Api.Services;

public class RequestService(
    AppDbContext db,
    NotificationService notifications,
    EmailOutboxService emailOutbox)
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
            .ThenInclude(student => student.StudentProfile)
            .Include(request => request.MentorProfile)
            .Where(request => request.StudentId == studentId)
            .OrderByDescending(request => request.CreatedAt)
            .Select(request => MentorService.ToRequestDto(request))
            .ToListAsync();
    }

    public async Task<MentorshipRequestDto> CreateRequestAsync(CreateMentorshipRequestDto dto)
    {
        var mentorProfileId = dto.MentorProfileId ?? dto.MentorId;

        if (mentorProfileId is null)
        {
            throw new InvalidOperationException("Mentor profile is required.");
        }

        var student = await db.Users
            .Include(user => user.StudentProfile)
            .SingleOrDefaultAsync(user => user.Id == dto.StudentId);

        if (student is null || student.Role != UserRole.Student)
        {
            throw new KeyNotFoundException("Student was not found.");
        }

        var mentor = await db.MentorProfiles
            .Include(profile => profile.Requests)
            .SingleOrDefaultAsync(profile => profile.Id == mentorProfileId.Value);

        if (mentor is null)
        {
            throw new KeyNotFoundException("Mentor was not found.");
        }

        if (!mentor.IsAvailable)
        {
            throw new InvalidOperationException("This mentor is currently marked as unavailable.");
        }

        var acceptedCount = mentor.Requests.Count(request => request.Status == RequestStatus.Accepted);

        if (acceptedCount >= mentor.MaxStudents)
        {
            throw new InvalidOperationException("This mentor has no available supervision slots.");
        }

        var hasActiveRequest = await db.MentorshipRequests.AnyAsync(request =>
            request.StudentId == dto.StudentId &&
            request.MentorProfileId == mentorProfileId.Value &&
            (request.Status == RequestStatus.Pending || request.Status == RequestStatus.Accepted));

        if (hasActiveRequest)
        {
            throw new InvalidOperationException("You already have an active request with this mentor.");
        }

        var now = DateTime.UtcNow;
        var request = new MentorshipRequest
        {
            StudentId = dto.StudentId,
            MentorProfileId = mentorProfileId.Value,
            ProposedTitle = dto.ProposedTitle.Trim(),
            Description = dto.Description.Trim(),
            OptionalMessage = string.IsNullOrWhiteSpace(dto.OptionalMessage) ? null : dto.OptionalMessage.Trim(),
            Status = RequestStatus.Pending,
            CreatedAt = now,
            UpdatedAt = now
        };

        db.MentorshipRequests.Add(request);
        await db.SaveChangesAsync();

        if (mentor.UserId is not null)
        {
            await notifications.CreateAsync(
                mentor.UserId.Value,
                "New mentorship request",
                $"{student.FullName} submitted a mentorship request: {request.ProposedTitle}",
                NotificationType.RequestSubmitted,
                request.Id);
        }

        await emailOutbox.CreateAsync(
            mentor.Email,
            "New mentorship request",
            BuildRequestSubmittedEmail(student, request));

        return await GetRequestAsync(request.Id);
    }

    public async Task<MentorshipRequestDto> AcceptRequestAsync(int requestId, string? response)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();
        var request = await FindRequestForUpdateAsync(requestId);

        if (request.Status != RequestStatus.Pending)
        {
            throw new InvalidOperationException("Only pending requests can be accepted.");
        }

        var acceptedCount = await db.MentorshipRequests.CountAsync(existing =>
            existing.MentorProfileId == request.MentorProfileId &&
            existing.Status == RequestStatus.Accepted);

        if (acceptedCount >= request.MentorProfile.MaxStudents)
        {
            throw new InvalidOperationException("The mentor has already reached the maximum number of accepted students.");
        }

        // TODO: Confirm with the professor whether other pending requests should be cancelled after one acceptance.
        var studentAlreadyAccepted = await db.MentorshipRequests.AnyAsync(existing =>
            existing.StudentId == request.StudentId &&
            existing.Id != request.Id &&
            existing.Status == RequestStatus.Accepted);

        if (studentAlreadyAccepted)
        {
            throw new InvalidOperationException("This student already has an accepted mentorship request.");
        }

        var now = DateTime.UtcNow;
        request.Status = RequestStatus.Accepted;
        request.MentorResponse = NormalizeResponse(response);
        request.UpdatedAt = now;
        request.DecidedAt = now;

        await db.SaveChangesAsync();
        await transaction.CommitAsync();

        await notifications.CreateAsync(
            request.StudentId,
            "Your mentorship request was accepted",
            $"{request.MentorProfile.FullName} accepted your request: {request.ProposedTitle}",
            NotificationType.RequestAccepted,
            request.Id);

        await emailOutbox.CreateAsync(
            request.Student.Email,
            "Your mentorship request was accepted",
            BuildDecisionEmail(request, "accepted"));

        return await GetRequestAsync(request.Id);
    }

    public async Task<MentorshipRequestDto> RejectRequestAsync(int requestId, string? response)
    {
        var request = await FindRequestForUpdateAsync(requestId);

        if (request.Status != RequestStatus.Pending)
        {
            throw new InvalidOperationException("Only pending requests can be rejected.");
        }

        var now = DateTime.UtcNow;
        request.Status = RequestStatus.Rejected;
        request.MentorResponse = NormalizeResponse(response);
        request.UpdatedAt = now;
        request.DecidedAt = now;

        await db.SaveChangesAsync();

        await notifications.CreateAsync(
            request.StudentId,
            "Your mentorship request was rejected",
            $"{request.MentorProfile.FullName} rejected your request: {request.ProposedTitle}",
            NotificationType.RequestRejected,
            request.Id);

        await emailOutbox.CreateAsync(
            request.Student.Email,
            "Your mentorship request was rejected",
            BuildDecisionEmail(request, "rejected"));

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
            .ThenInclude(student => student.StudentProfile)
            .Include(item => item.MentorProfile)
            .SingleAsync(item => item.Id == id);

        return MentorService.ToRequestDto(request);
    }

    private async Task<MentorshipRequest> FindRequestForUpdateAsync(int requestId)
    {
        var request = await db.MentorshipRequests
            .Include(item => item.Student)
            .ThenInclude(student => student.StudentProfile)
            .Include(item => item.MentorProfile)
            .SingleOrDefaultAsync(item => item.Id == requestId);

        if (request is null)
        {
            throw new KeyNotFoundException("Request was not found.");
        }

        return request;
    }

    private static string? NormalizeResponse(string? response) =>
        string.IsNullOrWhiteSpace(response) ? null : response.Trim();

    private static string BuildRequestSubmittedEmail(AppUser student, MentorshipRequest request)
    {
        var profile = student.StudentProfile;

        return $"""
            New mentorship request

            Student: {student.FullName}
            Email: {student.Email}
            Enrollment number: {profile?.EnrollmentNumber ?? "N/A"}
            Study program: {profile?.StudyProgram.ToString() ?? "N/A"}

            Proposed thesis title:
            {request.ProposedTitle}

            Thesis description:
            {request.Description}

            Optional message:
            {request.OptionalMessage ?? "N/A"}
            """;
    }

    private static string BuildDecisionEmail(MentorshipRequest request, string decision)
    {
        return $"""
            Your mentorship request was {decision}.

            Mentor: {request.MentorProfile.FullName}
            Proposed thesis title: {request.ProposedTitle}

            Mentor response:
            {request.MentorResponse ?? "No additional response was provided."}
            """;
    }
}
