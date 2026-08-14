import { Navigate } from "react-router-dom";
import { USER_ROLES } from "@/shared/constants/roles";
import { ROUTES } from "@/shared/constants/routes";
import {
  isAuthenticated,
  getUserRoles,
  normalizeRole,
} from "../../features/auth/utils/authUtils";

interface ProtectedRouteProps {
  allowedRoles: string[];
  children: React.ReactNode;
}

export default function ProtectedRoute({
  allowedRoles,
  children,
}: ProtectedRouteProps) {
  if (!isAuthenticated()) {
    return <Navigate to={ROUTES.HOME} replace />;
  }
  const roles = getUserRoles().map(normalizeRole);
  const allowed = allowedRoles.map(normalizeRole);
  const isAdmin = roles.includes(USER_ROLES.ADMIN);
  const isStudent = roles.includes(USER_ROLES.STUDENT);

  const hasAccess = allowed.some((role) => roles.includes(role));

  if (hasAccess) {
    return <>{children}</>;
  }

  if (isAdmin) {
    return <Navigate to={ROUTES.DASHBOARD} replace />;
  }

  if (isStudent) {
    return <Navigate to={ROUTES.DISCOVER} replace />;
  }

  return <Navigate to={ROUTES.HOME} replace />;
}
