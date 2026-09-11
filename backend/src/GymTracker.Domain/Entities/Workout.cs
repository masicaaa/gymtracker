using GymTracker.Domain.Enums;

namespace GymTracker.Domain.Entities;

public class Workout
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }

    public ExerciseType ExerciseType { get; set; }

    public DateTime PerformedAt { get; set; }

    public int DurationMinutes { get; set; }

    public int CaloriesBurned { get; set; }

    public int Intensity { get; set; }

    public int Fatigue { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
}
