import { useContext } from "react";
import { EnrollmentContext } from "./EnrollmentContext";

export function useEnrollment() {
  const context = useContext(EnrollmentContext);
  if (!context) {
    throw new Error("useEnrollment must be used within EnrollmentProvider");
  }
  return context;
}
