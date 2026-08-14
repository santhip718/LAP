import React from "react";
import IconButton from "@mui/material/IconButton";
import LightModeIcon from "@mui/icons-material/LightMode";
import DarkModeIcon from "@mui/icons-material/DarkMode";
import { useAppTheme } from "@/core/providers/ThemeProvider";
import { THEME_TOGGLE } from "./ThemeToggle.constants";
import type { LapThemeToggleProps } from "@/shared/types/ui.types";

export const LapThemeToggle: React.FC<LapThemeToggleProps> = ({ className }) => {
  const { mode, toggleTheme } = useAppTheme();
 
  return (
    <IconButton
      onClick={toggleTheme}
      aria-label={THEME_TOGGLE.ARIA_LABEL}
      className={className}
      sx={{ color: "var(--on-primary)" }}
    >
      {mode === "dark" ? <LightModeIcon /> : <DarkModeIcon />}
    </IconButton>
  );
};
 