using System.ComponentModel.DataAnnotations;

namespace MentorReservation.Api.DTOs;

public class MentorDecisionDto
{
    [StringLength(1500)]
    public string? Comment { get; set; }
}
