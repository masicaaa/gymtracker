using FluentValidation;
using GymTracker.Application.Dtos;

namespace GymTracker.Application.Validators;

public class WorkoutRequestValidator : AbstractValidator<WorkoutRequest>
{
    public WorkoutRequestValidator()
    {
        RuleFor(x => x.ExerciseType)
            .IsInEnum().WithMessage("Vrsta vježbe nije ispravna.");

        RuleFor(x => x.PerformedAt)
            .NotEmpty().WithMessage("Datum i vrijeme treninga su obavezni.")
            // Five minutes of slack: the user's clock is never exactly the server's.
            .LessThanOrEqualTo(_ => DateTime.Now.AddMinutes(5))
            .WithMessage("Trening ne može biti u budućnosti.");

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0).WithMessage("Trajanje mora biti veće od nule.")
            .LessThanOrEqualTo(1440).WithMessage("Trajanje ne može biti duže od 24 sata.");

        RuleFor(x => x.CaloriesBurned)
            .GreaterThanOrEqualTo(0).WithMessage("Kalorije ne mogu biti negativne.")
            .LessThanOrEqualTo(10000).WithMessage("Unesena vrijednost kalorija je nerealna.");

        RuleFor(x => x.Intensity)
            .InclusiveBetween(1, 10).WithMessage("Težina treninga mora biti između 1 i 10.");

        RuleFor(x => x.Fatigue)
            .InclusiveBetween(1, 10).WithMessage("Umor mora biti između 1 i 10.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Bilješka može imati najviše 1000 znakova.");
    }
}
