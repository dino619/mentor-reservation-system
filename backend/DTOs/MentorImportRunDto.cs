using MentorReservation.Api.Models;

namespace MentorReservation.Api.DTOs;

public record MentorImportRunDto(
    int Id,
    string SourceUrl,
    DateTime StartedAt,
    DateTime? FinishedAt,
    ImportRunStatus Status,
    int ImportedCount,
    int UpdatedCount,
    int SkippedCount,
    string? ErrorMessage);
