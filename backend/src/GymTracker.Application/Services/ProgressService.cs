using FluentValidation;
using GymTracker.Application.Dtos;
using GymTracker.Application.Interfaces;
using GymTracker.Domain.Entities;

namespace GymTracker.Application.Services;

public class ProgressService : IProgressService
{
    // Monday, as in ISO 8601 and as people here count weeks.
    private const DayOfWeek FirstDayOfWeek = DayOfWeek.Monday;

    private readonly IWorkoutRepository _workoutRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<MonthProgressRequest> _validator;

    public ProgressService(
        IWorkoutRepository workoutRepository,
        ICurrentUserService currentUser,
        IValidator<MonthProgressRequest> validator)
    {
        _workoutRepository = workoutRepository;
        _currentUser = currentUser;
        _validator = validator;
    }

    public async Task<MonthProgressDto> GetMonthProgressAsync(
        MonthProgressRequest request,
        CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var monthStart = new DateTime(request.Year, request.Month, 1);
        var monthEnd = monthStart.AddMonths(1); // exclusive: the 1st of the next month

        var workouts = await _workoutRepository.GetForUserAsync(
            _currentUser.UserId,
            monthStart,
            monthEnd,
            cancellationToken);

        var weeks = BuildWeeks(monthStart, monthEnd, workouts);

        return new MonthProgressDto(
            request.Year,
            request.Month,
            workouts.Count,
            workouts.Sum(w => w.DurationMinutes),
            workouts.Sum(w => w.CaloriesBurned),
            weeks);
    }

    // Walks the month week by week, cutting every week to the month:
    // a week running from the previous month starts on the 1st instead.
    private static List<WeekSummaryDto> BuildWeeks(
        DateTime monthStart,
        DateTime monthEnd,
        IReadOnlyList<Workout> workouts)
    {
        var weeks = new List<WeekSummaryDto>();

        var segmentStart = monthStart;
        var weekNumber = 1;

        while (segmentStart < monthEnd)
        {
            var nextWeekStart = StartOfNextWeek(segmentStart);
            var segmentEnd = nextWeekStart < monthEnd ? nextWeekStart : monthEnd;

            var inSegment = workouts
                .Where(w => w.PerformedAt >= segmentStart && w.PerformedAt < segmentEnd)
                .ToList();

            weeks.Add(new WeekSummaryDto(
                WeekNumber: weekNumber,
                StartDate: segmentStart,
                EndDate: segmentEnd.AddDays(-1), // last day that belongs to this week
                WorkoutCount: inSegment.Count,
                TotalDurationMinutes: inSegment.Sum(w => w.DurationMinutes),
                TotalCaloriesBurned: inSegment.Sum(w => w.CaloriesBurned),
                AverageIntensity: Average(inSegment, w => w.Intensity),
                AverageFatigue: Average(inSegment, w => w.Fatigue)));

            segmentStart = segmentEnd;
            weekNumber++;
        }

        return weeks;
    }

    private static DateTime StartOfNextWeek(DateTime date)
    {
        var daysUntilNext = ((int)FirstDayOfWeek - (int)date.DayOfWeek + 7) % 7;

        // Already on a Monday means a whole week ahead, not zero days.
        if (daysUntilNext == 0)
        {
            daysUntilNext = 7;
        }

        return date.Date.AddDays(daysUntilNext);
    }

    // Null, not zero: zero would read as "very easy workouts" instead of "no workouts".
    private static double? Average(IReadOnlyCollection<Workout> workouts, Func<Workout, int> selector)
    {
        if (workouts.Count == 0)
        {
            return null;
        }

        return Math.Round(workouts.Average(selector), 1);
    }
}
