using MentorReservation.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace MentorReservation.Api.Controllers;

[ApiController]
[Route("api/import-runs")]
public class ImportRunsController(MentorImportService imports) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetImportRuns()
    {
        return Ok(await imports.GetRunsAsync());
    }
}
