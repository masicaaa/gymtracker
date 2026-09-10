using GymTracker.Application.Interfaces;
using GymTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Infrastructure.Persistence;

public class WorkoutRepository : IWorkoutRepository
{
    private readonly GymTrackerDbContext _context;

    public WorkoutRepository(GymTrackerDbContext context)
    {
        _context = context;
    }

    public Task<Workout?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        return _context.Workouts
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<Workout>> GetForUserAsync(
        Guid userId,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Workouts
            .AsNoTracking()
            .Where(w => w.UserId == userId);

        if (from.HasValue)
        {
            query = query.Where(w => w.PerformedAt >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(w => w.PerformedAt < to.Value);
        }

        return await query
            .OrderByDescending(w => w.PerformedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Workout workout, CancellationToken cancellationToken = default)
    {
        await _context.Workouts.AddAsync(workout, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Workout workout, CancellationToken cancellationToken = default)
    {
        _context.Workouts.Update(workout);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Workout workout, CancellationToken cancellationToken = default)
    {
        _context.Workouts.Remove(workout);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
