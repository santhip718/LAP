import React, { useState, useMemo } from "react";
import { ThemeProvider as MuiThemeProvider } from "@mui/material/styles";
import CssBaseline from "@mui/material/CssBaseline";
import { ThemeContext } from "./ThemeContext";
import type { ThemeMode, AppThemeProviderProps } from "./provider.types";
import { THEME_STORAGE_KEY, DEFAULT_MODE } from "./provider.constants";
import { createAppTheme } from "@/core/theme";

export const AppThemeProvider: React.FC<AppThemeProviderProps> = ({
  children,
}) => {
  const [mode, setModeState] = useState<ThemeMode>(() => {
    const saved = localStorage.getItem(THEME_STORAGE_KEY);
    if (saved === "light" || saved === "dark") {
      return saved;
    }
    return DEFAULT_MODE;
  });

  const setMode = (newMode: ThemeMode) => {
    setModeState(newMode);
    localStorage.setItem(THEME_STORAGE_KEY, newMode);
  };

  const toggleTheme = () => {
    setMode(mode === "light" ? "dark" : "light");
  };

  const theme = useMemo(() => createAppTheme(mode), [mode]);

  return (
    <ThemeContext.Provider value={{ mode, toggleTheme, setMode }}>
      <MuiThemeProvider theme={theme}>
        <CssBaseline />
        {children}
      </MuiThemeProvider>
    </ThemeContext.Provider>
  );
};
