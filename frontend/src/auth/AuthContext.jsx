import { createContext, useCallback, useContext, useMemo, useState } from "react";
import { TOKEN_KEY } from "../api/client";
import { authApi } from "../api/authApi";

const USER_KEY = "gymtracker.user";

const AuthContext = createContext(null);

function readStoredUser() {
  try {
    const raw = localStorage.getItem(USER_KEY);
    return raw ? JSON.parse(raw) : null;
  } catch {
    return null;
  }
}

export function AuthProvider({ children }) {
  const [user, setUser] = useState(readStoredUser);

  const storeSession = useCallback((response) => {
    const loggedInUser = {
      id: response.userId,
      email: response.email,
      fullName: response.fullName,
    };

    localStorage.setItem(TOKEN_KEY, response.token);
    localStorage.setItem(USER_KEY, JSON.stringify(loggedInUser));
    setUser(loggedInUser);

    return loggedInUser;
  }, []);

  const login = useCallback(
    async (credentials) => storeSession(await authApi.login(credentials)),
    [storeSession]
  );

  const register = useCallback(
    async (data) => storeSession(await authApi.register(data)),
    [storeSession]
  );

  const logout = useCallback(() => {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    setUser(null);
  }, []);

  const value = useMemo(
    () => ({ user, isAuthenticated: user !== null, login, register, logout }),
    [user, login, register, logout]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);

  if (context === null) {
    throw new Error("useAuth must be used inside an AuthProvider.");
  }

  return context;
}
