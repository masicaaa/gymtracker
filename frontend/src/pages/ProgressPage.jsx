import { useEffect, useState } from "react";
import { progressApi } from "../api/progressApi";
import { toFormError } from "../api/client";
import { currentMonthValue, formatDayMonth, formatDuration } from "../utils/datetime";

const COMPLETE_MONTH = /^\d{4}-(0[1-9]|1[0-2])$/;

function splitMonthValue(value) {
  const [year, month] = value.split("-");

  return { year: Number(year), month: Number(month) };
}

function formatAverage(value) {
  return value === null ? "—" : value.toFixed(1);
}

export default function ProgressPage() {
  const [monthValue, setMonthValue] = useState(currentMonthValue);
  const [progress, setProgress] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    if (!COMPLETE_MONTH.test(monthValue)) {
      setIsLoading(false);
      return undefined;
    }

    let ignore = false;
    const { year, month } = splitMonthValue(monthValue);

    setIsLoading(true);
    setError("");

    progressApi
      .getMonth(year, month)
      .then((data) => {
        if (!ignore) setProgress(data);
      })
      .catch((error) => {
        if (!ignore) {
          const { generalError } = toFormError(error);
          setError(generalError || "Podatke za izabrani mjesec nije moguće učitati.");
        }
      })
      .finally(() => {
        if (!ignore) setIsLoading(false);
      });

    return () => {
      ignore = true;
    };
  }, [monthValue]);

  return (
    <>
      <div className="page-title">
        <h1>Napredak</h1>

        <label className="month-picker">
          Mjesec
          <input
            type="month"
            value={monthValue}
            onChange={(event) => setMonthValue(event.target.value)}
          />
        </label>
      </div>

      {isLoading && <p className="empty-state">Učitavanje...</p>}
      {error && <div className="alert-error">{error}</div>}

      {!isLoading && !error && progress && (
        <>
          <dl className="month-summary">
            <div>
              <dt>Treninga u mjesecu</dt>
              <dd>{progress.workoutCount}</dd>
            </div>
            <div>
              <dt>Ukupno trajanje</dt>
              <dd>{formatDuration(progress.totalDurationMinutes)}</dd>
            </div>
            <div>
              <dt>Ukupno kalorija</dt>
              <dd>{progress.totalCaloriesBurned}</dd>
            </div>
          </dl>

          <div className="table-scroll">
            <table className="progress-table">
              <thead>
                <tr>
                  <th>Sedmica</th>
                  <th>Period</th>
                  <th className="numeric">Treninga</th>
                  <th className="numeric">Ukupno trajanje</th>
                  <th className="numeric">Prosj. težina</th>
                  <th className="numeric">Prosj. umor</th>
                </tr>
              </thead>
              <tbody>
                {progress.weeks.map((week) => (
                  <tr key={week.weekNumber} className={week.workoutCount === 0 ? "is-empty" : undefined}>
                    <td>{week.weekNumber}.</td>
                    <td className="period">
                      {formatDayMonth(week.startDate)} – {formatDayMonth(week.endDate)}
                    </td>
                    <td className="numeric">{week.workoutCount}</td>
                    <td className="numeric">
                      {week.totalDurationMinutes === 0 ? "—" : formatDuration(week.totalDurationMinutes)}
                    </td>
                    <td className="numeric">{formatAverage(week.averageIntensity)}</td>
                    <td className="numeric">{formatAverage(week.averageFatigue)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          {progress.workoutCount === 0 && (
            <p className="empty-state">U ovom mjesecu nema zabilježenih treninga.</p>
          )}
        </>
      )}
    </>
  );
}
