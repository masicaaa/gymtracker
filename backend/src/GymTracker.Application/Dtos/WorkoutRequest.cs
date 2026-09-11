using GymTracker.Domain.Enums;

namespace GymTracker.Application.Dtos;

public record WorkoutRequest(
    ExerciseType ExerciseType,
    DateTime PerformedAt,
    int DurationMinutes,
    int CaloriesBurned,
    int Intensity,
    int Fatigue,
    string? Notes);
