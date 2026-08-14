export const BUTTON_LABELS = {
  ENROLL_TO_ACCESS: "Enroll to Access",
  BEGIN_ASSESSMENT: "Begin Assessment",
  MAX_ATTEMPTS_REACHED: "Max Attempts Reached",
} as const;

export const PROGRESS_MARKER_LABEL = "75% required";

export const EMPTY_ICON = "assignment";

export const TITLE_LABELS = {
  ASSESSMENT: "Assessment",
  COURSE_PROGRESS: "Course Progress",
  ASSESSMENT_UNLOCKED: "Assessment unlocked!",
  NO_ASSESSMENT: "No assessment available for this course.",
} as const;

export const META_TEMPLATES = {
  POINTS_SEPARATOR: " \u2022 ",
  POINTS_SUFFIX: " Points",
  MIN_SUFFIX: " min",
  PROGRESS_UNLOCK: "% more to unlock the assessment",
  ATTEMPTS_USED: (used: number, max: number) =>
    `You have used ${used} of ${max} attempts.`,
  COMPLETION_PCT: (pct: number) => `${pct}% Completed`,
} as const;
