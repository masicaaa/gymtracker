import { useState } from "react";
import { EXERCISE_TYPES } from "../constants/exerciseTypes";
import { apiValueToInput, toInputValue } from "../utils/datetime";
import { toFormError } from "../api/client";
import FormField from "./FormField";

function emptyForm() {
  return {
    exerciseType: "Cardio",
    performedAt: toInputValue(new Date()),
    durationMinutes: "",
    caloriesBurned: "",
    intensity: 5,
    fatigue: 5,
    notes: "",
  };
}

function toForm(workout) {
  return {
    exerciseType: workout.exerciseType,
    performedAt: apiValueToInput(workout.performedAt),
    durationMinutes: String(workout.durationMinutes),
    caloriesBurned: String(workout.caloriesBurned),
    intensity: workout.intensity,
    fatigue: workout.fatigue,
    notes: workout.notes ?? "",
  };
}

// Used for both adding and editing - the difference is only whether a workout is passed in.
export default function WorkoutForm({ workout, onSave, onCancel }) {
  const isEditing = Boolean(workout);

  const [form, setForm] = useState(() => (workout ? toForm(workout) : emptyForm()));
  const [fieldErrors, setFieldErrors] = useState({});
  const [generalError, setGeneralError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleChange = (event) => {
    const { name, value } = event.target;
    setForm((current) => ({ ...current, [name]: value }));
  };

  const handleSubmit = async (event) => {
    event.preventDefault();

    setFieldErrors({});
    setGeneralError("");
    setIsSubmitting(true);

    try {
      await onSave({
        exerciseType: form.exerciseType,
        performedAt: form.performedAt,
        durationMinutes: Number(form.durationMinutes),
        caloriesBurned: Number(form.caloriesBurned),
        intensity: Number(form.intensity),
        fatigue: Number(form.fatigue),
        notes: form.notes.trim() === "" ? null : form.notes,
      });
    } catch (error) {
      const { fieldErrors: errors, generalError: message } = toFormError(error);
      setFieldErrors(errors);
      setGeneralError(message);
      setIsSubmitting(false);
    }
  };

  return (
    <form className="workout-form" onSubmit={handleSubmit} noValidate>
      <h2>{isEditing ? "Izmjena treninga" : "Novi trening"}</h2>

      {generalError && <div className="alert-error">{generalError}</div>}

      <div className="form-grid">
        <FormField label="Vrsta vježbe" name="exerciseType" error={fieldErrors.exerciseType}>
          <select
            id="exerciseType"
            name="exerciseType"
            value={form.exerciseType}
            onChange={handleChange}
          >
            {EXERCISE_TYPES.map((type) => (
              <option key={type.value} value={type.value}>
                {type.label}
              </option>
            ))}
          </select>
        </FormField>

        <FormField
          label="Datum i vrijeme"
          name="performedAt"
          type="datetime-local"
          value={form.performedAt}
          onChange={handleChange}
          error={fieldErrors.performedAt}
        />

        <FormField
          label="Trajanje (minuta)"
          name="durationMinutes"
          type="number"
          min="1"
          value={form.durationMinutes}
          onChange={handleChange}
          error={fieldErrors.durationMinutes}
        />

        <FormField
          label="Potrošene kalorije"
          name="caloriesBurned"
          type="number"
          min="0"
          value={form.caloriesBurned}
          onChange={handleChange}
          error={fieldErrors.caloriesBurned}
        />

        <FormField
          label={`Težina treninga: ${form.intensity} / 10`}
          name="intensity"
          error={fieldErrors.intensity}
        >
          <input
            id="intensity"
            name="intensity"
            type="range"
            min="1"
            max="10"
            value={form.intensity}
            onChange={handleChange}
          />
        </FormField>

        <FormField
          label={`Umor poslije treninga: ${form.fatigue} / 10`}
          name="fatigue"
          error={fieldErrors.fatigue}
        >
          <input
            id="fatigue"
            name="fatigue"
            type="range"
            min="1"
            max="10"
            value={form.fatigue}
            onChange={handleChange}
          />
        </FormField>
      </div>

      <FormField label="Bilješka (nije obavezno)" name="notes" error={fieldErrors.notes}>
        <textarea
          id="notes"
          name="notes"
          rows="3"
          value={form.notes}
          onChange={handleChange}
        />
      </FormField>

      <div className="form-actions">
        <button type="submit" className="button-primary" disabled={isSubmitting}>
          {isSubmitting ? "Čuvanje..." : isEditing ? "Sačuvaj izmjene" : "Sačuvaj trening"}
        </button>
        <button type="button" className="button-secondary" onClick={onCancel}>
          Odustani
        </button>
      </div>
    </form>
  );
}
