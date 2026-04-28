using System.ComponentModel.DataAnnotations;

namespace MentorReservation.Api.Models;

public class AppUser
{
    public int Id { get; set; }

    [Required]
    [MaxLength(120)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(160)]
    public string Email { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public MentorProfile? MentorProfile { get; set; }
    public List<MentorshipRequest> StudentRequests { get; set; } = [];
}
