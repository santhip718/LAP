import { createContext } from "react";
import type { ThemeContextType } from "./provider.types";

export const ThemeContext = createContext<ThemeContextType | null>(null);
