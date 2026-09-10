using GymTracker.Application.Dtos;

namespace GymTracker.Application.Interfaces;

public interface IProgressService
{
    Task<MonthProgressDto> GetMonthProgressAsync(
        MonthProgressRequest request,
        CancellationToken cancellationToken = default);
}
