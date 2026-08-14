import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import {
  AuthProvider,
  AppThemeProvider,
  EnrollmentProvider,
  CourseProvider,
} from "@/core/providers";
import "./index.css";
import App from "./App.tsx";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <AuthProvider>
      <AppThemeProvider>
        <EnrollmentProvider>
          <CourseProvider>
            <App />
          </CourseProvider>
        </EnrollmentProvider>
      </AppThemeProvider>
    </AuthProvider>
  </StrictMode>,
);
