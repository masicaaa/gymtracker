using GymTracker.Application.Interfaces;
using GymTracker.Domain.Entities;

namespace GymTracker.Application.Tests.Fakes;

public class FakeWorkoutRepository : IWorkoutRepository
{
    private readonly List<Workout> _workouts;

    public FakeWorkoutRepository(params Workout[] workouts)
    {
        _workouts = workouts.ToList();
    }

    public Task<IReadOnlyList<Workout>> GetForUserAsync(
        Guid userId,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default)
    {
        var result = _workouts
            .Where(w => w.UserId == userId)
            .Where(w => !from.HasValue || w.PerformedAt >= from.Value)
            .Where(w => !to.HasValue || w.PerformedAt < to.Value)
            .OrderByDescending(w => w.PerformedAt)
            .ToList();

        return Task.FromResult<IReadOnlyList<Workout>>(result);
    }

    // the progress tests never reach these, throwing makes an accidental call obvious
    public Task<Workout?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task AddAsync(Workout workout, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task UpdateAsync(Workout workout, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task DeleteAsync(Workout workout, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();
}
