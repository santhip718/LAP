export const DEFAULT_TAB = "overview";

export const TAB_DEFINITIONS = [
  { id: "overview", label: "Overview", icon: "info" },
  { id: "discussions", label: "Discussions", icon: "forum" },
  { id: "ratings", label: "Ratings", icon: "star" },
  { id: "leaderboard", label: "Leaderboard", icon: "leaderboard" },
  { id: "history", label: "History", icon: "history" },
];

export const UNLOCK_THRESHOLD = 75;

export const MAX_ATTEMPTS = 3;

export const SUBMIT_TOAST = "Review submitted successfully";

export const SUBMIT_ERROR_TOAST = "Failed to submit review";

export const REVIEW_MODAL_TITLE = "Rate this Course";

export const ERROR_MESSAGE = "Failed to load course details.";

export const CURRICULUM_HEADING = "Curriculum";

export const NO_CONTENT_LABELS = {
  NO_CURRICULUM: "No curriculum",
  NO_CONTENT: "No curriculum content available for this course.",
};

export const HISTORY_EMPTY_LABELS = {
  ICON: "history",
  TITLE: "No Assessment History",
  MESSAGE: "You haven't attempted this assessment yet.",
};

export const HISTORY_TITLE = "Assessment History";

export const MODAL_PROPS = {
  maxWidth: "sm" as const,
};

export const DIVIDER_SPACING = 2;
