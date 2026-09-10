namespace GymTracker.Application.Dtos;

public record MonthProgressDto(
    int Year,
    int Month,
    int WorkoutCount,
    int TotalDurationMinutes,
    int TotalCaloriesBurned,
    IReadOnlyList<WeekSummaryDto> Weeks);
