using GymTracker.Application.Dtos;
using GymTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymTracker.Api.Controllers;

[ApiController]
[Route("api/workouts")]
[Authorize]
public class WorkoutsController : ControllerBase
{
    private readonly IWorkoutService _workoutService;

    public WorkoutsController(IWorkoutService workoutService)
    {
        _workoutService = workoutService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<WorkoutDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<WorkoutDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _workoutService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(WorkoutDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkoutDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        return Ok(await _workoutService.GetByIdAsync(id, cancellationToken));
    }

    [HttpPost]
    [ProducesResponseType(typeof(WorkoutDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WorkoutDto>> Create(
        WorkoutRequest request,
        CancellationToken cancellationToken)
    {
        var workout = await _workoutService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = workout.Id }, workout);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(WorkoutDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkoutDto>> Update(
        Guid id,
        WorkoutRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await _workoutService.UpdateAsync(id, request, cancellationToken));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _workoutService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }
}
