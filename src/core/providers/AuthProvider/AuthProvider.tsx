import { useState, useCallback, useEffect, type ReactNode } from "react";
import { tokenService } from "@/shared/services/storage/tokenService";
import { hasRole } from "@/features/auth/utils/authUtils";
import { AuthContext } from "./AuthContext";
import type { AuthContextValue } from "./auth.types";
import { USER_ROLES } from "@/shared/constants/roles";
import { STORAGE_KEYS } from "@/shared/constants/storage";

function computeAuthState() {
  const authenticated = !!tokenService.getAccessToken();
  return {
    isAuthenticated: authenticated,
    isAdmin: authenticated && hasRole(USER_ROLES.ADMIN),
    isStudent: authenticated && hasRole(USER_ROLES.STUDENT),
  };
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [state, setState] = useState(computeAuthState);

  const checkAuth = useCallback(() => {
    setState(computeAuthState());
  }, []);

  useEffect(() => {
    const handleStorage = (e: StorageEvent) => {
      if (e.key === STORAGE_KEYS.ACCESS_TOKEN || e.key === STORAGE_KEYS.REFRESH_TOKEN) {
        setState(computeAuthState());
      }
    };
    window.addEventListener("storage", handleStorage);
    return () => window.removeEventListener("storage", handleStorage);
  }, []);

  const value: AuthContextValue = {
    ...state,
    checkAuth,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
