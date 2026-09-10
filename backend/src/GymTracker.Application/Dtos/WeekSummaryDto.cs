namespace GymTracker.Application.Dtos;

// Dates are already cut to the chosen month.
// The averages are null for a week without workouts - there is nothing to average.
public record WeekSummaryDto(
    int WeekNumber,
    DateTime StartDate,
    DateTime EndDate,
    int WorkoutCount,
    int TotalDurationMinutes,
    int TotalCaloriesBurned,
    double? AverageIntensity,
    double? AverageFatigue);
