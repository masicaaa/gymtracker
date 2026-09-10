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

        // Finds every AbstractValidator in this project, so a new validator
        // only needs to be written - never registered by hand.
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // One message per field: stop at the first rule that fails, instead of
        // stacking "is required" together with every format rule.
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

        return services;
    }
}
