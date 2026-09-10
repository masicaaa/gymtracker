/**
 * The API sends and accepts the enum by name ("Cardio").
 * The Serbian wording shown to the user lives only here.
 */
export const EXERCISE_TYPES = [
  { value: "Cardio", label: "Kardio" },
  { value: "Strength", label: "Trening snage" },
  { value: "Flexibility", label: "Fleksibilnost" },
  { value: "Other", label: "Ostalo" },
];

export function exerciseTypeLabel(value) {
  return EXERCISE_TYPES.find((type) => type.value === value)?.label ?? value;
}
