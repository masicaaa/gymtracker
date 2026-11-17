using GymTracker.Application.Dtos;
using GymTracker.Application.Services;
using GymTracker.Application.Tests.Fakes;
using GymTracker.Application.Validators;
using GymTracker.Domain.Entities;
using GymTracker.Domain.Enums;

namespace GymTracker.Application.Tests.Services;

public class ProgressServiceTests
{
    private static readonly Guid UserId = Guid.NewGuid();

    private static ProgressService CreateService(params Workout[] workouts) =>
        new(new FakeWorkoutRepository(workouts),
            new FakeCurrentUserService(UserId),
            new MonthProgressRequestValidator());

    private static Workout Workout(DateTime performedAt, int duration = 60, int intensity = 5, int fatigue = 5) =>
        new()
        {
            UserId = UserId,
            ExerciseType = ExerciseType.Cardio,
            PerformedAt = performedAt,
            DurationMinutes = duration,
            CaloriesBurned = 300,
            Intensity = intensity,
            Fatigue = fatigue
        };

    [Fact]
    public async Task Month_starting_midweek_has_a_short_first_week()
    {
        // 1 Sep 2026 is a Tuesday, so the first week runs out on Sunday the 6th
        var service = CreateService();

        var progress = await service.GetMonthProgressAsync(new MonthProgressRequest(2026, 9));

        var firstWeek = progress.Weeks[0];

        Assert.Equal(new DateTime(2026, 9, 1), firstWeek.StartDate);
        Assert.Equal(new DateTime(2026, 9, 6), firstWeek.EndDate);
    }

    [Fact]
    public async Task Month_starting_on_monday_has_a_full_first_week()
    {
        // 1 Jun 2026 is a Monday, the edge case where "days until next Monday" comes out zero
        var service = CreateService();

        var progress = await service.GetMonthProgressAsync(new MonthProgressRequest(2026, 6));

        var firstWeek = progress.Weeks[0];

        Assert.Equal(new DateTime(2026, 6, 1), firstWeek.StartDate);
        Assert.Equal(new DateTime(2026, 6, 7), firstWeek.EndDate);
    }

    [Fact]
    public async Task Weeks_add_up_to_the_month_total()
    {
        var service = CreateService(
            Workout(new DateTime(2026, 9, 2, 18, 0, 0), duration: 45),
            Workout(new DateTime(2026, 9, 5, 9, 30, 0), duration: 60),
            Workout(new DateTime(2026, 9, 9, 19, 0, 0), duration: 30),
            Workout(new DateTime(2026, 9, 30, 7, 0, 0), duration: 90));

        var progress = await service.GetMonthProgressAsync(new MonthProgressRequest(2026, 9));

        Assert.Equal(4, progress.WorkoutCount);
        Assert.Equal(4, progress.Weeks.Sum(w => w.WorkoutCount));
        Assert.Equal(225, progress.Weeks.Sum(w => w.TotalDurationMinutes));
        Assert.Equal(225, progress.TotalDurationMinutes);
    }

    [Fact]
    public async Task Week_without_workouts_has_no_average_instead_of_zero()
    {
        // the only workout lands in the first week, so the second one stays empty
        var service = CreateService(
            Workout(new DateTime(2026, 9, 2, 18, 0, 0), intensity: 8, fatigue: 7));

        var progress = await service.GetMonthProgressAsync(new MonthProgressRequest(2026, 9));

        var weekWithWorkout = progress.Weeks[0];
        var emptyWeek = progress.Weeks[1];

        Assert.Equal(8, weekWithWorkout.AverageIntensity);
        Assert.Equal(7, weekWithWorkout.AverageFatigue);

        Assert.Equal(0, emptyWeek.WorkoutCount);
        Assert.Null(emptyWeek.AverageIntensity);
        Assert.Null(emptyWeek.AverageFatigue);
    }

    [Fact]
    public async Task Workout_at_midnight_belongs_to_the_week_that_starts_then()
    {
        // 7 Sep is a Monday, midnight has to land in the week that opens, not the one that closes
        var service = CreateService(
            Workout(new DateTime(2026, 9, 7, 0, 0, 0)));

        var progress = await service.GetMonthProgressAsync(new MonthProgressRequest(2026, 9));

        Assert.Equal(0, progress.Weeks[0].WorkoutCount);
        Assert.Equal(1, progress.Weeks[1].WorkoutCount);
        Assert.Equal(1, progress.Weeks.Sum(w => w.WorkoutCount));
    }
}
