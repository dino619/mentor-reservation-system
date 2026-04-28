namespace MentorReservation.Api.DTOs;

public record MentorDto(
    int Id,
    int UserId,
    string FullName,
    string Email,
    string Laboratory,
    IReadOnlyList<string> ResearchAreas,
    int MaxStudents,
    int CurrentAcceptedStudents,
    int AvailableSlots);
