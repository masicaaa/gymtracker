const BASE_URL = import.meta.env.VITE_API_URL;

export const TOKEN_KEY = "gymtracker.token";

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

  if (response.status === 401 && token) {
    localStorage.removeItem(TOKEN_KEY);
    window.location.href = "/prijava";
    return null;
  }

  const data = response.status === 204 ? null : await response.json().catch(() => null);

  if (!response.ok) {
    throw new ApiError(response.status, data);
  }

  return data;
}

function toFieldErrors(errors) {
  if (!errors) return null;

  return Object.fromEntries(
    Object.entries(errors).map(([field, messages]) => [
      field.charAt(0).toLowerCase() + field.slice(1),
      messages.join(" "),
    ])
  );
}

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

export function toFormError(error) {
  if (error instanceof ApiError) {
    if (error.fieldErrors) {
      return { fieldErrors: error.fieldErrors, generalError: "" };
    }

    return { fieldErrors: {}, generalError: error.message };
  }

  return { fieldErrors: {}, generalError: "Server nije dostupan. Pokušajte ponovo." };
}
