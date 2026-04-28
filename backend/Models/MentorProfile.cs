using System.ComponentModel.DataAnnotations;

namespace MentorReservation.Api.Models;

public class MentorProfile
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public AppUser User { get; set; } = null!;

    [Required]
    [MaxLength(160)]
    public string Laboratory { get; set; } = string.Empty;

    public List<string> ResearchAreas { get; set; } = [];

    public int MaxStudents { get; set; } = 5;

    public List<MentorshipRequest> Requests { get; set; } = [];
}
