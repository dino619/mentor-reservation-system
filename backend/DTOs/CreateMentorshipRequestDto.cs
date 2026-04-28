using System.ComponentModel.DataAnnotations;

namespace MentorReservation.Api.DTOs;

public class CreateMentorshipRequestDto
{
    [Required]
    public int StudentId { get; set; }

    [Required]
    public int MentorId { get; set; }

    [Required]
    [StringLength(180, MinimumLength = 5)]
    public string ProposedTitle { get; set; } = string.Empty;

    [Required]
    [StringLength(3000, MinimumLength = 20)]
    public string Description { get; set; } = string.Empty;

    [StringLength(1500)]
    public string? OptionalMessage { get; set; }
}
