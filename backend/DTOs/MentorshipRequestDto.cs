using MentorReservation.Api.Models;

namespace MentorReservation.Api.DTOs;

public record MentorshipRequestDto(
    int Id,
    int StudentId,
    string StudentName,
    string StudentEmail,
    int MentorId,
    string MentorName,
    string MentorEmail,
    string ProposedTitle,
    string Description,
    string? OptionalMessage,
    RequestStatus Status,
    string? MentorComment,
    DateTime CreatedAt,
    DateTime UpdatedAt);
