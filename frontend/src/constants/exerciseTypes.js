export const EXERCISE_TYPES = [
  { value: "Cardio", label: "Kardio" },
  { value: "Strength", label: "Trening snage" },
  { value: "Flexibility", label: "Fleksibilnost" },
  { value: "Other", label: "Ostalo" },
];

export function exerciseTypeLabel(value) {
  return EXERCISE_TYPES.find((type) => type.value === value)?.label ?? value;
}
