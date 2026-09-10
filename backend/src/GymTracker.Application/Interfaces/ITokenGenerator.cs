using GymTracker.Domain.Entities;

namespace GymTracker.Application.Interfaces;

public interface ITokenGenerator
{
    string GenerateToken(User user);
}
