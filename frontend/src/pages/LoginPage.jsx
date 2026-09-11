import { useState } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import { toFormError } from "../api/client";
import FormField from "../components/FormField";

export default function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const [form, setForm] = useState({ email: "", password: "" });
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
      await login(form);

      const target = location.state?.from?.pathname ?? "/";
      navigate(target, { replace: true });
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
        <h1>Prijava</h1>
        <p className="subtitle">Dobrodošli nazad.</p>

        {generalError && <div className="alert-error">{generalError}</div>}

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
          autoComplete="current-password"
        />

        <button type="submit" className="button-primary" disabled={isSubmitting}>
          {isSubmitting ? "Prijava u toku..." : "Prijavi se"}
        </button>

        <p className="form-footer">
          Nemate nalog? <Link to="/registracija">Registrujte se</Link>
        </p>
      </form>
    </div>
  );
}
