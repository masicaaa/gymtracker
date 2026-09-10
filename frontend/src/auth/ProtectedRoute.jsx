import { Navigate, Outlet, useLocation } from "react-router-dom";
import { useAuth } from "./AuthContext";

// Signed-out visitors go to the sign-in page, remembering where they wanted to go.
export default function ProtectedRoute() {
  const { isAuthenticated } = useAuth();
  const location = useLocation();

  if (!isAuthenticated) {
    return <Navigate to="/prijava" state={{ from: location }} replace />;
  }

  return <Outlet />;
}
