export const BACK_BUTTON_LABEL = "Back to Course";

export const RESULT_NOT_AVAILABLE = "Result data not available.";

export const SUMMARY_LABELS = {
  PASSED: "Assessment Passed!",
  FAILED: "Assessment Not Passed",
};

export const MESSAGE_LABELS = {
  SUMMARY_PREFIX: "You answered ",
  SUMMARY_INFIX: " out of ",
  SUMMARY_SUFFIX: " questions correctly.",
  DATE_PREFIX: "Completed on ",
  QUESTION_PREFIX: "Q",
};

export const STAT_LABELS = {
  ACCURACY: "Accuracy",
  TIME_TAKEN: "Time Taken",
  TIER: "Tier",
  AREAS_TO_IMPROVE: "Areas to Improve",
  ANSWER_REVIEW: "Answer Review",
  YOUR_ANSWER: "Your answer:",
  SCORE: "Score:",
};

export const TOGGLE_LABELS = {
  HIDE: "Hide Answer Review",
  SHOW: "Review Your Answers",
};

export const FALLBACKS = {
  QUESTION_TEXT: "Question text not available",
  NO_ANSWER: "No answer",
  TOPIC_NAME: "General",
};

export const ICONS = {
  BACK: "arrow_back",
  PASSED: "check_circle",
  FAILED: "cancel",
  ACCURACY: "target",
  TIME: "schedule",
  TIER: "military_tech",
  WEAK: "trending_down",
  HIDE_REVIEW: "visibility_off",
  SHOW_REVIEW: "fact_check",
  CHEVRON: "expand_more",
  CORRECT: "check_circle",
  INCORRECT: "cancel",
};

export const SUFFIXES = {
  PERCENT: "%",
  POINTS: "pts",
  MINUTES: "min",
  TOTAL_SEPARATOR: "/",
};

export const getCourseOverviewRoute = (courseId: string) =>
  `/course-overview/${courseId}`;
