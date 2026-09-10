using FluentValidation;
using GymTracker.Application.Dtos;
using GymTracker.Application.Exceptions;
using GymTracker.Application.Interfaces;
using GymTracker.Domain.Entities;

namespace GymTracker.Application.Services;

public class WorkoutService : IWorkoutService
{
    private readonly IWorkoutRepository _workoutRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<WorkoutRequest> _validator;

    public WorkoutService(
        IWorkoutRepository workoutRepository,
        ICurrentUserService currentUser,
        IValidator<WorkoutRequest> validator)
    {
        _workoutRepository = workoutRepository;
        _currentUser = currentUser;
        _validator = validator;
    }

    public async Task<IReadOnlyList<WorkoutDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var workouts = await _workoutRepository.GetForUserAsync(
            _currentUser.UserId,
            cancellationToken: cancellationToken);

        return workouts.Select(ToDto).ToList();
    }

    public async Task<WorkoutDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var workout = await FindOwnedWorkoutAsync(id, cancellationToken);

        return ToDto(workout);
    }

    public async Task<WorkoutDto> CreateAsync(
        WorkoutRequest request,
        CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var workout = new Workout
        {
            UserId = _currentUser.UserId,
            ExerciseType = request.ExerciseType,
            PerformedAt = request.PerformedAt,
            DurationMinutes = request.DurationMinutes,
            CaloriesBurned = request.CaloriesBurned,
            Intensity = request.Intensity,
            Fatigue = request.Fatigue,
            Notes = Normalize(request.Notes)
        };

        await _workoutRepository.AddAsync(workout, cancellationToken);

        return ToDto(workout);
    }

    public async Task<WorkoutDto> UpdateAsync(
        Guid id,
        WorkoutRequest request,
        CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var workout = await FindOwnedWorkoutAsync(id, cancellationToken);

        workout.ExerciseType = request.ExerciseType;
        workout.PerformedAt = request.PerformedAt;
        workout.DurationMinutes = request.DurationMinutes;
        workout.CaloriesBurned = request.CaloriesBurned;
        workout.Intensity = request.Intensity;
        workout.Fatigue = request.Fatigue;
        workout.Notes = Normalize(request.Notes);

        await _workoutRepository.UpdateAsync(workout, cancellationToken);

        return ToDto(workout);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var workout = await FindOwnedWorkoutAsync(id, cancellationToken);

        await _workoutRepository.DeleteAsync(workout, cancellationToken);
    }

    // The single place ownership is checked - used by get, update and delete.
    private async Task<Workout> FindOwnedWorkoutAsync(Guid id, CancellationToken cancellationToken)
    {
        var workout = await _workoutRepository.GetByIdAsync(id, _currentUser.UserId, cancellationToken);

        return workout ?? throw new WorkoutNotFoundException(id);
    }

    private static string? Normalize(string? notes)
    {
        var trimmed = notes?.Trim();

        return string.IsNullOrEmpty(trimmed) ? null : trimmed;
    }

    private static WorkoutDto ToDto(Workout workout) => new(
        workout.Id,
        workout.ExerciseType,
        workout.PerformedAt,
        workout.DurationMinutes,
        workout.CaloriesBurned,
        workout.Intensity,
        workout.Fatigue,
        workout.Notes);
}
