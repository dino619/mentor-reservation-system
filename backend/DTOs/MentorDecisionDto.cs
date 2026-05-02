using System.ComponentModel.DataAnnotations;

namespace MentorReservation.Api.DTOs;

public class MentorDecisionDto
{
    [StringLength(1500)]
    public string? Response { get; set; }

    // Kept for compatibility with the first prototype frontend.
    [StringLength(1500)]
    public string? Comment { get; set; }

    public string? EffectiveResponse => string.IsNullOrWhiteSpace(Response) ? Comment : Response;
}
