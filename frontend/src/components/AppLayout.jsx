import { NavLink, Outlet } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";

export default function AppLayout() {
  const { user, logout } = useAuth();

  return (
    <>
      <header className="app-header">
        <div className="header-left">
          <span className="brand">GymTracker</span>

          <nav className="main-nav">
            <NavLink to="/" end>
              Treninzi
            </NavLink>
            <NavLink to="/napredak">Napredak</NavLink>
          </nav>
        </div>

        <div className="user-area">
          <span>{user.fullName}</span>
          <button type="button" className="button-link" onClick={logout}>
            Odjava
          </button>
        </div>
      </header>

      <main className="app-content">
        <Outlet />
      </main>
    </>
  );
}
