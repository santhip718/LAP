export const UNLOCK_THRESHOLD = 75;

export const MAX_ATTEMPTS = 3;

export const QUESTION_COUNT = 10;

export const LOADING_LABELS = {
  LOADING: "Loading assessment...",
  NOT_FOUND: "Assessment not found.",
};

export const SECTION_LABELS = {
  RULES_TITLE: "Quiz Rules & Instructions",
  STATS_TITLE: "Quick Stats",
};

export const RULES = [
  {
    icon: "timer",
    label: "Timed Questions",
    description: "Assessment has a time limit.",
  },
  { icon: "block", label: "No Backtracking", description: "Once you submit." },
  {
    icon: "auto_mode",
    label: "Auto-Submit",
    description: "The quiz will automatically end.",
  },
];

export const STAT_LABELS = [
  { icon: "list_alt", label: "Questions", suffix: "Items" },
  { icon: "schedule", label: "Total Time", suffix: "Mins" },
  { icon: "grade", label: "Passing Score", suffix: "%" },
];

export const MISC = {
  DIFFICULTY: "Difficulty",
  SUBTITLE: "Validate your expertise and demonstrate your knowledge.",
  ENROLLMENT_REQUIRED:
    "You need to be enrolled with active status to take this assessment.",
  CHECKBOX_LABEL: "I have read and agree to the rules and instructions",
  BUTTON_ENROLL: "Enrollment Required",
  BUTTON_BEGIN: "Begin Assessment",
  BUTTON_BACK: "Back to Course",
  FALLBACK_DIFFICULTY: "—",
  COMPLETION_MSG_PREFIX: "You have completed ",
  COMPLETION_MSG_SUFFIX: " of the course. You need at least ",
  COMPLETION_MSG_UNLOCK: "% to unlock this assessment.",
  BUTTON_COMPLETED_SUFFIX: "% Completed",
};

export const ICONS = {
  DIFFICULTY: "terminal",
  RULES: "gavel",
  PLAY: "play_circle",
  LOCK: "lock",
  INFO: "info",
  TRENDING_UP: "trending_up",
};

export const getTestRoute = (courseId: string) =>
  `/course-content/${courseId}/assessment/test`;
