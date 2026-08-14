import { ROUTES } from "@/shared/constants/routes";

export const SCROLL_THRESHOLD = 20;

export const NAV_ITEMS = {
  student: [
    { label: "Dashboard", to: ROUTES.DISCOVER },
    { label: "My Courses", to: ROUTES.MY_COURSES },
    { label: "Assessment History", to: ROUTES.ASSESSMENT_HISTORY },
    { label: "Leaderboard", to: ROUTES.LEADERBOARD },
    { label: "Profile", to: ROUTES.PROFILE },
  ],
  admin: [
    { label: "Dashboard", to: ROUTES.DASHBOARD },
    { label: "Course", to: ROUTES.ADMIN_COURSES },
    { label: "Assessment", to: ROUTES.ADMIN_ASSESSMENTS },
    { label: "Enrollments", to: ROUTES.ADMIN_ENROLLMENTS },
    { label: "Leaderboard", to: ROUTES.LEADERBOARD },
    { label: "Profile", to: ROUTES.PROFILE },
  ],
} as const;

export const NAVBAR_LABELS = {
  LOGOUT: "Logout",
  SIGN_IN: "Sign In",
  LOGO_ALT: "Propel Logo",
  TOGGLE_MENU: "Toggle menu",
} as const;
