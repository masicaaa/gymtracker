using GymTracker.Application.Dtos;
using GymTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymTracker.Api.Controllers;

[ApiController]
[Route("api/progress")]
[Authorize]
public class ProgressController : ControllerBase
{
    private readonly IProgressService _progressService;

    public ProgressController(IProgressService progressService)
    {
        _progressService = progressService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(MonthProgressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MonthProgressDto>> GetMonthProgress(
        [FromQuery] MonthProgressRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await _progressService.GetMonthProgressAsync(request, cancellationToken));
    }
}
