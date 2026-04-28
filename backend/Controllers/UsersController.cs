using MentorReservation.Api.Models;
using MentorReservation.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace MentorReservation.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(UserService users) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] UserRole? role)
    {
        var result = await users.GetUsersAsync(role);
        return Ok(result);
    }
}
