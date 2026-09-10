using GymTracker.Application.Dtos;

namespace GymTracker.Application.Interfaces;

public interface IWorkoutService
{
    Task<IReadOnlyList<WorkoutDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<WorkoutDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<WorkoutDto> CreateAsync(WorkoutRequest request, CancellationToken cancellationToken = default);

    Task<WorkoutDto> UpdateAsync(Guid id, WorkoutRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
