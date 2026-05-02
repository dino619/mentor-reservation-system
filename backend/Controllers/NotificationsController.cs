using MentorReservation.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace MentorReservation.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController(NotificationService notifications) : ControllerBase
{
    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetUserNotifications(int userId)
    {
        return Ok(await notifications.GetForUserAsync(userId));
    }

    [HttpPost("{id:int}/read")]
    public async Task<IActionResult> MarkRead(int id)
    {
        try
        {
            return Ok(await notifications.MarkReadAsync(id));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
    }
}
