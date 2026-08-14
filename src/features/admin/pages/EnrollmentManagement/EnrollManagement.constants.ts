export const DEFAULT_PAGE_SIZE = 10;
export const ENROLLMENT_PAGE_SIZE = 100;

export const FALLBACK_EMPTY = "—";
export const FALLBACK_COURSE = "Unknown Course";
export const FALLBACK_USER = "Unknown User";

export const enrollmentStrings = {
  pageTitle: "Enrollments",
  pageSubtitle: "Review and manage pending enrollment requests.",
  table: {
    title: "Pending Enrollment Requests",
    searchPlaceholder: "Search by course name",
    emptyState: "No pending enrollments.",
    emptyStateMessage: "There are no pending enrollment requests at this time.",
    loading: "Loading enrollments...",
    errorRetry: "Retry",
    pendingCount: "{count} pending",
  },
  filters: {
    allCategories: "All Categories",
    categoryLabel: "Category",
  },
  columns: {
    user: "User",
    course: "Course",
    category: "Category",
    enrolledOn: "Requested On",
    actions: "",
  },
  actions: {
    accept: "Accept",
    accepting: "Accepting...",
  },
  success: {
    accepted: "Enrollment accepted successfully!",
  },
  error: {
    loadFailed: "Failed to load enrollment requests. Please try again.",
    acceptFailed: "Failed to accept enrollment. Please try again.",
  },
  ariaLabels: {
    refresh: "Refresh",
  },
} as const;
