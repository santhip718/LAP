import { REGEX } from "@/shared/constants/regex";

export const LOGIN_UI = {
  TITLE: "Welcome back",
  SUBTITLE: "Access your AI-powered learning workspace",
  EMAIL_LABEL: "Email Address",
  EMAIL_PLACEHOLDER: "name@institution.edu",
  PASSWORD_LABEL: "Password",
  PASSWORD_PLACEHOLDER: "\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022",
  SUBMIT_TEXT: "Sign In",
  SUBMITTING_TEXT: "Signing In...",
  ARROW_FORWARD_ICON: "arrow_forward",
  ANALYTICS_ICON: "analytics",
  VISIBILITY_OFF: "visibility_off",
  VISIBILITY: "visibility",
} as const;

export const LOGIN_VALIDATION = {
  EMAIL_REQUIRED: "Email is required",
  EMAIL_PATTERN: REGEX.EMAIL,
  EMAIL_INVALID: "Enter a valid email (e.g. name@domain.com)",
  PASSWORD_REQUIRED: "Password is required",
  PASSWORD_MIN_LENGTH: 8,
  PASSWORD_MIN_MESSAGE: "At least 8 characters",
  PASSWORD_MAX_LENGTH: 128,
  PASSWORD_MAX_MESSAGE: "At most 128 characters",
  PASSWORD_UPPER_MESSAGE: "Must include an uppercase letter",
  PASSWORD_LOWER_MESSAGE: "Must include a lowercase letter",
  PASSWORD_DIGIT_MESSAGE: "Must include a number",
} as const;
