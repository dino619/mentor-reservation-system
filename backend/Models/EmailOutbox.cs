using System.ComponentModel.DataAnnotations;

namespace MentorReservation.Api.Models;

public class EmailOutbox
{
    public int Id { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(160)]
    public string RecipientEmail { get; set; } = string.Empty;

    [Required]
    [MaxLength(180)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string Body { get; set; } = string.Empty;

    public EmailOutboxStatus Status { get; set; } = EmailOutboxStatus.Pending;

    [MaxLength(1000)]
    public string? ErrorMessage { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SentAt { get; set; }
}
