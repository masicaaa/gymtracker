using FluentValidation;
using GymTracker.Application.Dtos;

namespace GymTracker.Application.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email je obavezan.")
            .EmailAddress().WithMessage("Email adresa nije ispravna.")
            .MaximumLength(256).WithMessage("Email može imati najviše 256 znakova.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Lozinka je obavezna.")
            .MinimumLength(8).WithMessage("Lozinka mora imati najmanje 8 znakova.")
            .Matches("[A-Za-z]").WithMessage("Lozinka mora sadržati bar jedno slovo.")
            .Matches("[0-9]").WithMessage("Lozinka mora sadržati bar jednu cifru.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Ime i prezime je obavezno.")
            .MaximumLength(100).WithMessage("Ime i prezime može imati najviše 100 znakova.");
    }
}
