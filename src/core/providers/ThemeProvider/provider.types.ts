import type { ReactNode } from "react";

export type ThemeMode = "light" | "dark";

export interface ThemeContextType {
  mode: ThemeMode;
  toggleTheme: () => void;
  setMode: (mode: ThemeMode) => void;
}

export interface AppThemeProviderProps {
  children: ReactNode;
}
