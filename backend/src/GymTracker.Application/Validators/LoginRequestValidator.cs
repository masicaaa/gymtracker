using FluentValidation;
using GymTracker.Application.Dtos;

namespace GymTracker.Application.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    // no format rules on purpose - login shouldn't hint what a valid password looks like
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email je obavezan.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Lozinka je obavezna.");
    }
}
