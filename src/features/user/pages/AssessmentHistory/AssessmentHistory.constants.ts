export const PAGE_SIZE = 12;

export const INITIAL_PAGE = 1;

export const EMPTY_ICON = "assignment";

export const PAGE_LABELS = {
  TITLE: "Assessment History",
  SUBTITLE: "Review your past assessment results and track your progress.",
};

export const EMPTY_LABELS = {
  TITLE: "No assessments completed",
  MESSAGE: "Complete an assessment to see your history here.",
};

export const FOOTER_LABELS = {
  LOADING: "Loading more...",
  END: "You've reached the end",
};

export const BUTTON_LABEL = "My Courses";

export const ROUTES = {
  MY_COURSES: "/my-courses",
};

export const getResultRoute = (courseId: string) =>
  `/course-overview/${courseId}/assessment/result`;
