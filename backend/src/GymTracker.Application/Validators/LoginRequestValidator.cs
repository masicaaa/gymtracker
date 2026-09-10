using FluentValidation;
using GymTracker.Application.Dtos;

namespace GymTracker.Application.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    // Deliberately no format or length rules here: sign-in must not hint
    // at what a valid password looks like. Only "you left it empty".
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email je obavezan.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Lozinka je obavezna.");
    }
}
