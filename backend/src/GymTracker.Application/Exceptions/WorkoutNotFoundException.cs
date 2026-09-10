namespace GymTracker.Application.Exceptions;

public class WorkoutNotFoundException : Exception
{
    public WorkoutNotFoundException(Guid id)
        : base("Trening nije pronađen.")
    {
        WorkoutId = id;
    }

    public Guid WorkoutId { get; }
}
