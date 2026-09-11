using System.Reflection;
using FluentValidation;
using GymTracker.Application.Interfaces;
using GymTracker.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GymTracker.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IWorkoutService, WorkoutService>();
        services.AddScoped<IProgressService, ProgressService>();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

        return services;
    }
}
