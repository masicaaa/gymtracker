using GymTracker.Domain.Enums;

namespace GymTracker.Application.Dtos;

// Used for both create and update - the fields are the same, so the rules cannot drift apart.
// No UserId here on purpose: the owner comes from the token, never from the request.
public record WorkoutRequest(
    ExerciseType ExerciseType,
    DateTime PerformedAt,
    int DurationMinutes,
    int CaloriesBurned,
    int Intensity,
    int Fatigue,
    string? Notes);
