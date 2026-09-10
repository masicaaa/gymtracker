const BASE_URL = import.meta.env.VITE_API_URL;

export const TOKEN_KEY = "gymtracker.token";

// Every request goes through here: attaches the token and turns a failed status into an error.
async function request(path, options = {}) {
  const token = localStorage.getItem(TOKEN_KEY);

  const response = await fetch(`${BASE_URL}${path}`, {
    ...options,
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...options.headers,
    },
  });

  // A 401 while we were sending a token means the session expired.
  // A 401 without a token is simply a failed sign-in, handled by the page.
  if (response.status === 401 && token) {
    localStorage.removeItem(TOKEN_KEY);
    window.location.href = "/prijava";
    return null;
  }

  // 204 No Content has no body to read.
  const data = response.status === 204 ? null : await response.json().catch(() => null);

  if (!response.ok) {
    throw new ApiError(response.status, data);
  }

  return data;
}

// The API names fields "Email", forms use "email" - lowering the first letter here
// keeps that difference out of the components.
function toFieldErrors(errors) {
  if (!errors) return null;

  return Object.fromEntries(
    Object.entries(errors).map(([field, messages]) => [
      field.charAt(0).toLowerCase() + field.slice(1),
      messages.join(" "),
    ])
  );
}

// Carries the ProblemDetails body the API returned.
export class ApiError extends Error {
  constructor(status, problem) {
    super(problem?.detail || problem?.title || "Došlo je do greške.");
    this.status = status;
    this.fieldErrors = toFieldErrors(problem?.errors);
  }
}

export const api = {
  get: (path) => request(path),
  post: (path, body) => request(path, { method: "POST", body: JSON.stringify(body) }),
  put: (path, body) => request(path, { method: "PUT", body: JSON.stringify(body) }),
  del: (path) => request(path, { method: "DELETE" }),
};

// Written once so every form shows errors the same way.
export function toFormError(error) {
  if (error instanceof ApiError) {
    if (error.fieldErrors) {
      return { fieldErrors: error.fieldErrors, generalError: "" };
    }

    return { fieldErrors: {}, generalError: error.message };
  }

  // Not an ApiError means the request never reached the server.
  return { fieldErrors: {}, generalError: "Server nije dostupan. Pokušajte ponovo." };
}
