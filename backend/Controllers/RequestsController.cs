using MentorReservation.Api.DTOs;
using MentorReservation.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace MentorReservation.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RequestsController(RequestService requests) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateRequest(CreateMentorshipRequestDto dto)
    {
        try
        {
            var created = await requests.CreateRequestAsync(dto);
            return Created($"/api/requests/{created.Id}", created);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ex.Message });
        }
    }

    [HttpPost("{id:int}/accept")]
    public async Task<IActionResult> AcceptRequest(int id, MentorDecisionDto dto)
    {
        try
        {
            return Ok(await requests.AcceptRequestAsync(id, dto.Comment));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ex.Message });
        }
    }

    [HttpPost("{id:int}/reject")]
    public async Task<IActionResult> RejectRequest(int id, MentorDecisionDto dto)
    {
        try
        {
            return Ok(await requests.RejectRequestAsync(id, dto.Comment));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ex.Message });
        }
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> CancelRequest(int id)
    {
        try
        {
            return Ok(await requests.CancelRequestAsync(id));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ex.Message });
        }
    }
}
