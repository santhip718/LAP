export interface AuthContextValue {
  isAuthenticated: boolean;
  isAdmin: boolean;
  isStudent: boolean;
  checkAuth: () => void;
}
