using System.ComponentModel.DataAnnotations;

namespace MentorReservation.Api.Models;

public class Notification
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public AppUser User { get; set; } = null!;

    [Required]
    [MaxLength(160)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Message { get; set; } = string.Empty;

    public NotificationType Type { get; set; } = NotificationType.General;

    public bool IsRead { get; set; }

    public int? RelatedRequestId { get; set; }
    public MentorshipRequest? RelatedRequest { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
