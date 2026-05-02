using System.ComponentModel.DataAnnotations;

namespace MentorReservation.Api.Models;

public class MentorProfile
{
    public int Id { get; set; }

    public int? UserId { get; set; }
    public AppUser? User { get; set; }

    [Required]
    [MaxLength(80)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(80)]
    public string? Title { get; set; }

    [EmailAddress]
    [MaxLength(160)]
    public string? Email { get; set; }

    [MaxLength(500)]
    public string? ProfileUrl { get; set; }

    [MaxLength(180)]
    public string? Laboratory { get; set; }

    [MaxLength(80)]
    public string? Office { get; set; }

    [MaxLength(80)]
    public string? Phone { get; set; }

    public int MaxStudents { get; set; } = 5;

    public bool IsAvailable { get; set; } = true;

    [Required]
    [MaxLength(120)]
    public string Source { get; set; } = "Manual";

    [MaxLength(240)]
    public string? SourceExternalId { get; set; }

    public DateTime? ImportedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public string FullName => $"{FirstName} {LastName}".Trim();

    public List<MentorshipRequest> Requests { get; set; } = [];
    public List<MentorResearchArea> MentorResearchAreas { get; set; } = [];
}
