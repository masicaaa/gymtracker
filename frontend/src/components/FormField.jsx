export default function FormField({ label, name, error, children, ...inputProps }) {
  return (
    <div className="field">
      <label htmlFor={name}>{label}</label>

      {children ?? (
        <input
          id={name}
          name={name}
          className={error ? "has-error" : undefined}
          {...inputProps}
        />
      )}

      {error && <span className="field-error">{error}</span>}
    </div>
  );
}
