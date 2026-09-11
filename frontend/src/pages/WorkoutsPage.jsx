import { useCallback, useEffect, useState } from "react";
import { workoutApi } from "../api/workoutApi";
import WorkoutForm from "../components/WorkoutForm";
import WorkoutList from "../components/WorkoutList";

export default function WorkoutsPage() {
  const [workouts, setWorkouts] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [loadError, setLoadError] = useState("");
  const [actionError, setActionError] = useState("");

  const [editing, setEditing] = useState(null);
  const [isFormOpen, setIsFormOpen] = useState(false);

  const load = useCallback(async () => {
    setIsLoading(true);
    setLoadError("");
    setActionError("");

    try {
      setWorkouts(await workoutApi.getAll());
    } catch {
      setLoadError("Treninge nije moguće učitati. Provjerite da li je server pokrenut.");
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    load();
  }, [load]);

  const closeForm = () => {
    setIsFormOpen(false);
    setEditing(null);
  };

  const handleSave = async (values) => {
    if (editing) {
      await workoutApi.update(editing.id, values);
    } else {
      await workoutApi.create(values);
    }

    closeForm();
    await load();
  };

  const handleDelete = async (workout) => {
    const confirmed = window.confirm(
      "Da li ste sigurni da želite obrisati ovaj trening? Ova radnja se ne može poništiti."
    );

    if (!confirmed) return;

    try {
      await workoutApi.remove(workout.id);
      await load();
    } catch {
      setActionError("Trening nije moguće obrisati. Pokušajte ponovo.");
    }
  };

  return (
    <>
      <div className="page-title">
        <h1>Moji treninzi</h1>

        {!isFormOpen && (
          <button
            type="button"
            className="button-primary compact"
            onClick={() => {
              setEditing(null);
              setIsFormOpen(true);
            }}
          >
            Dodaj trening
          </button>
        )}
      </div>

      {isFormOpen && (
        <WorkoutForm
          key={editing?.id ?? "new"}
          workout={editing}
          onSave={handleSave}
          onCancel={closeForm}
        />
      )}

      {isLoading && <p className="empty-state">Učitavanje...</p>}
      {loadError && <div className="alert-error">{loadError}</div>}
      {actionError && <div className="alert-error">{actionError}</div>}

      {!isLoading && !loadError && (
        <WorkoutList
          workouts={workouts}
          onEdit={(workout) => {
            setEditing(workout);
            setIsFormOpen(true);
          }}
          onDelete={handleDelete}
        />
      )}
    </>
  );
}
