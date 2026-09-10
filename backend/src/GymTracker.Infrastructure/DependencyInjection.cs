using GymTracker.Application.Interfaces;
using GymTracker.Infrastructure.Persistence;
using GymTracker.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GymTracker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is missing from configuration.");

        // The version is declared instead of auto-detected, because auto-detection
        // opens a connection at startup and fails before the database is created.
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 36));

        services.AddDbContext<GymTrackerDbContext>(options =>
            options.UseMySql(connectionString, serverVersion));

        // Contract (Application) -> implementation (Infrastructure).
        // Scoped: one instance per HTTP request, matching the DbContext lifetime.
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IWorkoutRepository, WorkoutRepository>();

        // Reads the "Jwt" section of appsettings into JwtOptions once, at startup.
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        // Stateless helpers: one shared instance is enough.
        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
        services.AddSingleton<ITokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
