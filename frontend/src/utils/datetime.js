// The workout time is the user's local time and must never be converted to UTC:
// that would move a late-evening workout into the wrong week. It stays a plain
// "YYYY-MM-DDTHH:mm" string from the form all the way to the database.

export function toInputValue(date) {
  const pad = (number) => String(number).padStart(2, "0");

  return (
    `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}` +
    `T${pad(date.getHours())}:${pad(date.getMinutes())}`
  );
}

/** Cuts an API value like "2026-09-08T18:00:00" down to what the input expects. */
export function apiValueToInput(value) {
  return value ? value.slice(0, 16) : "";
}

const DATE_FORMAT = new Intl.DateTimeFormat("sr-Latn-BA", {
  day: "numeric",
  month: "long",
  year: "numeric",
  hour: "2-digit",
  minute: "2-digit",
});

export function formatDateTime(value) {
  return DATE_FORMAT.format(new Date(value));
}

export function formatDuration(minutes) {
  const hours = Math.floor(minutes / 60);
  const rest = minutes % 60;

  if (hours === 0) return `${rest} min`;
  if (rest === 0) return `${hours} h`;

  return `${hours} h ${rest} min`;
}

/** "2026-09-01T00:00:00" -> "01.09." */
export function formatDayMonth(value) {
  const date = new Date(value);
  const pad = (number) => String(number).padStart(2, "0");

  return `${pad(date.getDate())}.${pad(date.getMonth() + 1)}.`;
}

export function currentMonthValue() {
  const now = new Date();

  return `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, "0")}`;
}
