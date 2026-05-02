using MentorReservation.Api.Models;

namespace MentorReservation.Api.DTOs;

public record NotificationDto(
    int Id,
    int UserId,
    string Title,
    string Message,
    NotificationType Type,
    bool IsRead,
    int? RelatedRequestId,
    DateTime CreatedAt);
