using System.ComponentModel.DataAnnotations;

namespace MentorReservation.Api.Models;

public class MentorImportRun
{
    public int Id { get; set; }

    [Required]
    [MaxLength(500)]
    public string SourceUrl { get; set; } = string.Empty;

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? FinishedAt { get; set; }
    public ImportRunStatus Status { get; set; }
    public int ImportedCount { get; set; }
    public int UpdatedCount { get; set; }
    public int SkippedCount { get; set; }

    [MaxLength(2000)]
    public string? ErrorMessage { get; set; }
}
