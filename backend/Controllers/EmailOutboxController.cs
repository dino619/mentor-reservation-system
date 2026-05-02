using MentorReservation.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace MentorReservation.Api.Controllers;

[ApiController]
[Route("api/email-outbox")]
public class EmailOutboxController(EmailOutboxService emailOutbox) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetEmailOutbox()
    {
        return Ok(await emailOutbox.GetAllAsync());
    }
}
