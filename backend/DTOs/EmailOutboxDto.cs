using MentorReservation.Api.Models;

namespace MentorReservation.Api.DTOs;

public record EmailOutboxDto(
    int Id,
    string RecipientEmail,
    string Subject,
    string Body,
    EmailOutboxStatus Status,
    string? ErrorMessage,
    DateTime CreatedAt,
    DateTime? SentAt);
