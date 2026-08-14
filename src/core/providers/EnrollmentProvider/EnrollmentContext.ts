import { createContext } from "react";
import type { EnrollmentContextValue } from "./enrollment.types";

export const EnrollmentContext = createContext<EnrollmentContextValue | null>(null);
