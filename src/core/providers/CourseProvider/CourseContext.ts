import { createContext } from "react";
import type { CourseContextValue } from "./course.types";

export const CourseContext = createContext<CourseContextValue | null>(null);
