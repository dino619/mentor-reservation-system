using MentorReservation.Api.Data;
using MentorReservation.Api.DTOs;
using MentorReservation.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MentorReservation.Api.Services;

public class NotificationService(AppDbContext db)
{
    public async Task CreateAsync(
        int userId,
        string title,
        string message,
        NotificationType type,
        int? relatedRequestId = null)
    {
        db.Notifications.Add(new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            RelatedRequestId = relatedRequestId,
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();
    }

    public async Task<List<NotificationDto>> GetForUserAsync(int userId)
    {
        return await db.Notifications
            .AsNoTracking()
            .Where(notification => notification.UserId == userId)
            .OrderByDescending(notification => notification.CreatedAt)
            .Select(notification => ToDto(notification))
            .ToListAsync();
    }

    public async Task<NotificationDto> MarkReadAsync(int id)
    {
        var notification = await db.Notifications.SingleOrDefaultAsync(item => item.Id == id);

        if (notification is null)
        {
            throw new KeyNotFoundException("Notification was not found.");
        }

        notification.IsRead = true;
        await db.SaveChangesAsync();

        return ToDto(notification);
    }

    private static NotificationDto ToDto(Notification notification) =>
        new(
            notification.Id,
            notification.UserId,
            notification.Title,
            notification.Message,
            notification.Type,
            notification.IsRead,
            notification.RelatedRequestId,
            notification.CreatedAt);
}
