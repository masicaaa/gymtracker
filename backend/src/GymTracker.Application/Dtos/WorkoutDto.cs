using GymTracker.Domain.Enums;

namespace GymTracker.Application.Dtos;

public record WorkoutDto(
    Guid Id,
    ExerciseType ExerciseType,
    DateTime PerformedAt,
    int DurationMinutes,
    int CaloriesBurned,
    int Intensity,
    int Fatigue,
    string? Notes);
