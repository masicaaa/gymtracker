using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GymTracker.Application.Interfaces;

namespace GymTracker.Api.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;

            var value = user?.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? user?.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (!Guid.TryParse(value, out var userId))
            {
                throw new InvalidOperationException(
                    "The current request has no authenticated user.");
            }

            return userId;
        }
    }
}
