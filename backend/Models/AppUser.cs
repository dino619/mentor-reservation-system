using System.ComponentModel.DataAnnotations;

namespace MentorReservation.Api.Models;

public class AppUser
{
    public int Id { get; set; }

    [Required]
    [MaxLength(80)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(160)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public string FullName => $"{FirstName} {LastName}".Trim();

    public StudentProfile? StudentProfile { get; set; }
    public MentorProfile? MentorProfile { get; set; }
    public List<MentorshipRequest> StudentRequests { get; set; } = [];
    public List<Notification> Notifications { get; set; } = [];
}
