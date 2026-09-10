using FluentValidation;
using GymTracker.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GymTracker.Api.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Validation failures carry a list of field errors, so they get their own shape.
        if (exception is ValidationException validationException)
        {
            var errors = validationException.Errors
                .GroupBy(failure => failure.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(failure => failure.ErrorMessage).ToArray());

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            await httpContext.Response.WriteAsJsonAsync(
                new ValidationProblemDetails(errors)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Podaci nisu ispravni",
                    Instance = httpContext.Request.Path
                },
                cancellationToken);

            return true;
        }

        var (statusCode, title) = exception switch
        {
            EmailAlreadyExistsException => (StatusCodes.Status409Conflict, "Email je već registrovan"),
            InvalidCredentialsException => (StatusCodes.Status401Unauthorized, "Prijava nije uspjela"),
            WorkoutNotFoundException => (StatusCodes.Status404NotFound, "Trening nije pronađen"),
            _ => (StatusCodes.Status500InternalServerError, "Neočekivana greška")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception");
        }

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            // Known exceptions carry a safe message; unknown ones must not leak details.
            Detail = statusCode == StatusCodes.Status500InternalServerError
                ? "Došlo je do greške. Pokušajte ponovo."
                : exception.Message,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}
