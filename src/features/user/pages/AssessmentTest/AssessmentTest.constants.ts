export const TIMER_INTERVAL_MS = 1000;

export const QUESTION_TYPES = {
  MCQ: "MCQ",
  TrueFalse: "TrueFalse",
  FillInBlank: "FillInBlank",
} as const;

export const LEAVE_CONFIRM_CONFIG = {
  title: "Leave Assessment?",
  message: "Your answers will not be saved. Are you sure you want to leave?",
  confirmLabel: "Leave",
  cancelLabel: "Stay",
};

export const QUIT_CONFIRM_CONFIG = {
  title: "Quit Assessment?",
  message: "Your progress will not be saved. Are you sure you want to quit?",
  confirmLabel: "Quit",
  cancelLabel: "Continue Assessment",
};

export const SUBMIT_CONFIRM_CONFIG = {
  title: "Submit Assessment?",
  message:
    "Once submitted, you cannot change your answers. Are you sure you want to submit?",
  confirmLabel: "Submit",
  cancelLabel: "Review Answers",
};

export const SUBMIT_LABELS = {
  submitting: "Submitting...",
  submit: "Submit Assessment",
};

export const PAGE_TITLE = "Course Assessment";

export const QUIT_LABEL = "Quit";

export const STAT_LABELS = {
  TOTAL: "Total assessment volume",
  TIME_LIMIT: "Time limit",
};

export const STAT_UNITS = {
  QUESTIONS: "Questions",
  POINTS: "Points",
  MINUTES: "Minutes",
};

export const QUESTION_TYPE_LABELS: Record<string, string> = {
  MCQ: "Multiple Choice",
  TrueFalse: "True / False",
  FillInBlank: "Fill in the Blank",
};

export const UNSUPPORTED_QUESTION = "Unsupported question type";

export const FLAG_LABELS = {
  FLAGGED: "Flagged",
  FLAG_REVIEW: "Flag for review",
};

export const PLACEHOLDER = "Type your answer...";

export const WEIGHT_LABEL = "Weight:";
export const POINT = "point";
export const POINTS = "points";

export const ERROR_TOAST_FALLBACK = "An error occurred while submitting. Please try again.";

export const TOAST_DURATION_MS = 5000;

export const ICONS = {
  CORRECT: "check_circle",
  HELP: "help",
  GRADE: "grade",
  HISTORY: "history",
  FLAG: "flag",
  ASSIGNMENT: "assignment",
};

export const getOverviewRoute = (courseId: string) =>
  `/course-overview/${courseId}`;

export const getResultRoute = (courseId: string) =>
  `/course-overview/${courseId}/assessment/result`;
