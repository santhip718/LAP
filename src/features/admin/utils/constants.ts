export const FALLBACK_COURSE_TITLE = "Untitled course";
export const FALLBACK_CATEGORY = "Uncategorized";
export const FALLBACK_NOT_SET = "Not set";
export const FALLBACK_UNKNOWN = "Unknown";
export const FALLBACK_TOPIC_TITLE = "Untitled topic";
export const FALLBACK_CONTENT_TITLE = "Untitled content";
export const FALLBACK_USER = "Unknown User";
export const FALLBACK_EMPTY = "—";

export const DEFAULT_TOPIC_DURATION = 30;
export const DEFAULT_SEQUENCE_ORDER = 1;
export const DEFAULT_DURATION_HOURS = 1;
export const HOURS_TO_MINUTES = 60;

export const CONTENT_TYPE_VIDEO = "video";
export const CONTENT_TYPE_PDF = "pdf";
export const MIME_TYPE_PDF = "application/pdf";
export const ACCEPTED_IMAGE_TYPES = "image/png,image/jpeg,image/jpg";

export const courseServiceStrings = {
  error: {
    courseIdMissing: "Course ID is missing.",
    courseIdNotReturned: "Created course ID was not returned by the server.",
    loadOverviewFailed: "Failed to load course overview. Please try again.",
    loadDiscussionFailed: "Failed to load discussion messages. Please try again.",
    loadReferenceDataFailed: "Failed to load reference data. Please try again later.",
    loadAdminCoursesFailed: "Failed to load courses from the backend. Please try again.",
    summaryMetricsFailed: "Course summary metrics could not be loaded.",
  },
} as const;

export const AVATAR_COLORS = [
  "#4f46e5",
  "#0891b2",
  "#059669",
  "#d97706",
  "#dc2626",
  "#7c3aed",
  "#db2777",
  "#2563eb",
] as const;

export const timeStrings = {
  justNow: "Just now",
  minutesAgo: "m ago",
  hoursAgo: "h ago",
  daysAgo: "d ago",
} as const;

export const ASSESSMENTS_PAGE_SIZE = 20;

export const ASSESSMENT_STATUS = {
  ACTIVE: 'Active',
  DRAFT: 'Draft',
  CLOSED: 'Closed',
} as const;

export const dashboardStrings = {
  pageTitle: "Users",
  pageSubtitle: "Manage users, roles, and account details.",
  addUserButton: "Add User",
  table: {
    title: "All Users",
    searchPlaceholder: "Search by name or email",
    loading: "Loading users...",
    errorRetry: "Retry",
  },
  columns: {
    user: "User",
    email: "Email",
    roles: "Roles",
    actions: "",
  },
  error: {
    loadDetailFailed: "Failed to load user details.",
  },
  ariaLabels: {
    viewUser: "View user",
    editUser: "Edit user",
    deleteUser: "Delete user",
  },
} as const;
