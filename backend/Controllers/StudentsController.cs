using MentorReservation.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace MentorReservation.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController(RequestService requests) : ControllerBase
{
    [HttpGet("{id:int}/requests")]
    public async Task<IActionResult> GetStudentRequests(int id)
    {
        try
        {
            return Ok(await requests.GetStudentRequestsAsync(id));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
    }
}
