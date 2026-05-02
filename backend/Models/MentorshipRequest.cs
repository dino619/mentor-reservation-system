using System.ComponentModel.DataAnnotations;

namespace MentorReservation.Api.Models;

public class MentorshipRequest
{
    public int Id { get; set; }

    public int StudentId { get; set; }
    public AppUser Student { get; set; } = null!;

    public int MentorProfileId { get; set; }
    public MentorProfile MentorProfile { get; set; } = null!;

    [Required]
    [MaxLength(180)]
    public string ProposedTitle { get; set; } = string.Empty;

    [Required]
    [MaxLength(3000)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(1500)]
    public string? OptionalMessage { get; set; }

    public RequestStatus Status { get; set; } = RequestStatus.Pending;

    [MaxLength(1500)]
    public string? MentorResponse { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DecidedAt { get; set; }
}
