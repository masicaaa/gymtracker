import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import { toFormError } from "../api/client";
import FormField from "../components/FormField";

const EMPTY_FORM = { fullName: "", email: "", password: "" };

export default function RegisterPage() {
  const { register } = useAuth();
  const navigate = useNavigate();

  const [form, setForm] = useState(EMPTY_FORM);
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
      await register(form);
      navigate("/", { replace: true });
    } catch (error) {
      const { fieldErrors: errors, generalError: message } = toFormError(error);
      setFieldErrors(errors);
      setGeneralError(message);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="page-center">
      <form className="card" onSubmit={handleSubmit} noValidate>
        <h1>Registracija</h1>
        <p className="subtitle">Napravite nalog i počnite da bilježite treninge.</p>

        {generalError && <div className="alert-error">{generalError}</div>}

        <FormField
          label="Ime i prezime"
          name="fullName"
          value={form.fullName}
          onChange={handleChange}
          error={fieldErrors.fullName}
          autoComplete="name"
        />

        <FormField
          label="Email"
          name="email"
          type="email"
          value={form.email}
          onChange={handleChange}
          error={fieldErrors.email}
          autoComplete="email"
        />

        <FormField
          label="Lozinka"
          name="password"
          type="password"
          value={form.password}
          onChange={handleChange}
          error={fieldErrors.password}
          autoComplete="new-password"
        />

        <button type="submit" className="button-primary" disabled={isSubmitting}>
          {isSubmitting ? "Registracija u toku..." : "Registruj se"}
        </button>

        <p className="form-footer">
          Već imate nalog? <Link to="/prijava">Prijavite se</Link>
        </p>
      </form>
    </div>
  );
}
