namespace GymTracker.Application.Dtos;

public record WeekSummaryDto(
    int WeekNumber,
    DateTime StartDate,
    DateTime EndDate,
    int WorkoutCount,
    int TotalDurationMinutes,
    int TotalCaloriesBurned,
    double? AverageIntensity,
    double? AverageFatigue);
