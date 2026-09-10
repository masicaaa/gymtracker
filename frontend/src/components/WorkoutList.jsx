import { exerciseTypeLabel } from "../constants/exerciseTypes";
import { formatDateTime, formatDuration } from "../utils/datetime";

export default function WorkoutList({ workouts, onEdit, onDelete }) {
  if (workouts.length === 0) {
    return (
      <p className="empty-state">
        Još nema zabilježenih treninga. Dodajte prvi da biste mogli pratiti napredak.
      </p>
    );
  }

  return (
    <ul className="workout-list">
      {workouts.map((workout) => (
        <li key={workout.id} className="workout-item">
          <div className="workout-main">
            <div className="workout-title">
              <span className="workout-type">{exerciseTypeLabel(workout.exerciseType)}</span>
              <span className="workout-date">{formatDateTime(workout.performedAt)}</span>
            </div>

            <dl className="workout-stats">
              <div>
                <dt>Trajanje</dt>
                <dd>{formatDuration(workout.durationMinutes)}</dd>
              </div>
              <div>
                <dt>Kalorije</dt>
                <dd>{workout.caloriesBurned}</dd>
              </div>
              <div>
                <dt>Težina</dt>
                <dd>{workout.intensity} / 10</dd>
              </div>
              <div>
                <dt>Umor</dt>
                <dd>{workout.fatigue} / 10</dd>
              </div>
            </dl>

            {workout.notes && <p className="workout-notes">{workout.notes}</p>}
          </div>

          <div className="workout-actions">
            <button type="button" className="button-link" onClick={() => onEdit(workout)}>
              Izmijeni
            </button>
            <button type="button" className="button-link danger" onClick={() => onDelete(workout)}>
              Obriši
            </button>
          </div>
        </li>
      ))}
    </ul>
  );
}
