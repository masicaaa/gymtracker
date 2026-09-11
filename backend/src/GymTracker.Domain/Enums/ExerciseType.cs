namespace GymTracker.Domain.Enums;

// stored as int - don't renumber, the db has the old values
public enum ExerciseType
{
    Cardio = 1,
    Strength = 2,
    Flexibility = 3,
    Other = 4
}
