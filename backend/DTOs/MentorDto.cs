namespace MentorReservation.Api.DTOs;

public record MentorDto(
    int Id,
    int? UserId,
    string FirstName,
    string LastName,
    string FullName,
    string? Title,
    string? Email,
    string? ProfileUrl,
    string? Laboratory,
    string? Office,
    string? Phone,
    IReadOnlyList<string> ResearchAreas,
    int MaxStudents,
    int CurrentAcceptedStudents,
    int AvailableSlots,
    bool IsAvailable,
    string Source,
    DateTime? ImportedAt);
