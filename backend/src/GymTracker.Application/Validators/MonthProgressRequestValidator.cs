using FluentValidation;
using GymTracker.Application.Dtos;

namespace GymTracker.Application.Validators;

public class MonthProgressRequestValidator : AbstractValidator<MonthProgressRequest>
{
    public MonthProgressRequestValidator()
    {
        RuleFor(x => x.Year)
            .InclusiveBetween(2000, 2100).WithMessage("Godina mora biti između 2000. i 2100.");

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12).WithMessage("Mjesec mora biti između 1 i 12.");
    }
}
