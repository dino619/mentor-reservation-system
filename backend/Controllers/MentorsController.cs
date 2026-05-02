using MentorReservation.Api.Models;
using MentorReservation.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace MentorReservation.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MentorsController(MentorService mentors, MentorImportService imports) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMentors([FromQuery] string? search)
    {
        var result = await mentors.GetMentorsAsync(search);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetMentor(int id)
    {
        var result = await mentors.GetMentorAsync(id);
        return result is null ? NotFound(new { message = "Mentor was not found." }) : Ok(result);
    }

    [HttpGet("{id:int}/requests")]
    public async Task<IActionResult> GetMentorRequests(int id)
    {
        try
        {
            return Ok(await mentors.GetMentorRequestsAsync(id));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
    }

    [HttpPost("import")]
    public async Task<IActionResult> ImportMentors()
    {
        var result = await imports.ImportAsync();

        if (result.Status == ImportRunStatus.Failed)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
