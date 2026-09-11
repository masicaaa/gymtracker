using GymTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymTracker.Infrastructure.Persistence.Configurations;

public class WorkoutConfiguration : IEntityTypeConfiguration<Workout>
{
    public void Configure(EntityTypeBuilder<Workout> builder)
    {
        builder.ToTable("workouts", t =>
        {
            t.HasCheckConstraint("ck_workouts_intensity", "`Intensity` BETWEEN 1 AND 10");
            t.HasCheckConstraint("ck_workouts_fatigue", "`Fatigue` BETWEEN 1 AND 10");
            t.HasCheckConstraint("ck_workouts_duration", "`DurationMinutes` > 0");
        });

        builder.HasKey(w => w.Id);

        builder.Property(w => w.ExerciseType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(w => w.PerformedAt)
            .IsRequired();

        builder.Property(w => w.Notes)
            .HasMaxLength(1000);

        builder.HasOne(w => w.User)
            .WithMany()
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(w => new { w.UserId, w.PerformedAt });
    }
}
