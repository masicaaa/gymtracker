using GymTracker.Application.Interfaces;

namespace GymTracker.Application.Tests.Fakes;

public class FakeCurrentUserService : ICurrentUserService
{
    public FakeCurrentUserService(Guid userId)
    {
        UserId = userId;
    }

    public Guid UserId { get; }
}
