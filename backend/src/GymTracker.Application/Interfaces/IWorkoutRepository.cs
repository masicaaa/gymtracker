using GymTracker.Domain.Entities;

namespace GymTracker.Application.Interfaces;

public interface IWorkoutRepository
{
    // Ownership is part of the lookup, so another user's workout simply cannot be found.
    Task<Workout?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Workout>> GetForUserAsync(
        Guid userId,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(Workout workout, CancellationToken cancellationToken = default);

    Task UpdateAsync(Workout workout, CancellationToken cancellationToken = default);

    Task DeleteAsync(Workout workout, CancellationToken cancellationToken = default);
}
