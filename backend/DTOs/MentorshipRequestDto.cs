using MentorReservation.Api.Models;

namespace MentorReservation.Api.DTOs;

public record MentorshipRequestDto(
    int Id,
    int StudentId,
    string StudentName,
    string StudentEmail,
    string? StudentEnrollmentNumber,
    StudyProgram? StudentStudyProgram,
    int MentorProfileId,
    string MentorName,
    string? MentorEmail,
    string ProposedTitle,
    string Description,
    string? OptionalMessage,
    RequestStatus Status,
    string? MentorResponse,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? DecidedAt);
