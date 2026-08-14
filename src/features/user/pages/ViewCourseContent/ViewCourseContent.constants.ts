export const ASSESSMENT_UNLOCK_THRESHOLD = 75;

export const MAX_ATTEMPTS = 3;

export const SIDEBAR_WIDTH = 340;

export const SIDEBAR_STATUS = {
  LOADING: "Loading curriculum...",
  ERROR: "Failed to load curriculum.",
  NO_CONTENT: "No curriculum content available.",
};

export const ERROR_BANNER =
  "Could not load course data. The curriculum may be incomplete.";

export const NAV_LABELS = {
  PREVIOUS: "Previous",
  NEXT: "Next",
};

export const COMPLETE_BUTTON = {
  COMPLETED: "Completed",
  MARKING: "Marking...",
  MARK_AS_COMPLETED: "Mark as Completed",
};

export const NAV_ICONS = {
  PREVIOUS: "chevron_left",
  NEXT: "chevron_right",
  COMPLETED: "check_circle",
  UNCOMPLETED: "radio_button_unchecked",
};

export const COMPLETE_TOAST = "Content marked as complete";

export const COMPLETE_ERROR_TOAST = "Failed to mark as complete";

export const createFallbackCourse = () => ({
  id: "",
  title: "Course Content",
  category: { id: "", name: "" },
  difficultyLevel: { id: "", name: "" },
  durationMinute: 0,
  overallRating: 0,
  thumbnailImgPath: "",
  status: true,
  description: "",
  createdByUser: { id: "", fullName: "", email: "", roles: [] },
  topics: [],
  enrollmentCount: 0,
  assessmentTitle: "",
  totalMark: 0,
  passingMark: 0,
});
