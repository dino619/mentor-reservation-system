using MentorReservation.Api.Data;
using MentorReservation.Api.DTOs;
using MentorReservation.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MentorReservation.Api.Services;

public class EmailOutboxService(AppDbContext db)
{
    public async Task CreateAsync(string? recipientEmail, string subject, string body)
    {
        if (string.IsNullOrWhiteSpace(recipientEmail))
        {
            return;
        }

        db.EmailOutbox.Add(new EmailOutbox
        {
            RecipientEmail = recipientEmail.Trim().ToLowerInvariant(),
            Subject = subject,
            Body = body,
            Status = EmailOutboxStatus.Pending,
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();
    }

    public async Task<List<EmailOutboxDto>> GetAllAsync()
    {
        return await db.EmailOutbox
            .AsNoTracking()
            .OrderByDescending(email => email.CreatedAt)
            .Select(email => new EmailOutboxDto(
                email.Id,
                email.RecipientEmail,
                email.Subject,
                email.Body,
                email.Status,
                email.ErrorMessage,
                email.CreatedAt,
                email.SentAt))
            .ToListAsync();
    }
}
