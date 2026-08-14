import { Navigate } from "react-router-dom";
import { isAuthenticated, getUserRoles, normalizeRole } from "@/features/auth/utils/authUtils";
import { USER_ROLES } from "@/shared/constants/roles";
import { ROUTES } from "@/shared/constants/routes";

interface PublicRouteProps {
  children: React.ReactNode;
}

export default function PublicRoute({ children }: PublicRouteProps) {
  if (isAuthenticated()) {
    const roles = getUserRoles().map(normalizeRole);
    if (roles.includes(USER_ROLES.ADMIN)) {
      return <Navigate to={ROUTES.DASHBOARD} replace />;
    }
    if (roles.includes(USER_ROLES.STUDENT)) {
      return <Navigate to={ROUTES.DISCOVER} replace />;
    }
    return <>{children}</>;
  }

  return <>{children}</>;
}
