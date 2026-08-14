export const PAGE_SIZE = 10;

export const INITIAL_PAGE = 1;

export const TOAST_MESSAGES = {
  deleteSuccess: "Review deleted",
  deleteError: "Failed to delete review",
  updateSuccess: "Review updated",
  updateError: "Failed to update review",
};

export const DELETE_CONFIRM_CONFIG = {
  title: "Delete Review?",
  message:
    "Are you sure you want to delete your review? This action cannot be undone.",
  confirmLabel: "Delete",
  cancelLabel: "Cancel",
};

export const SECTION_TITLES = {
  YOUR_REVIEW: "Your Review",
  OTHER_REVIEWS: "Other Reviews",
  COMMUNITY_REVIEWS: "Community Reviews",
};

export const NO_CONTENT_LABELS = {
  NO_REVIEW: "No review yet",
  SHARE: "Share your experience with this course.",
  NO_OTHER_REVIEWS: "No other reviews",
  NO_OTHER_REVIEWS_MSG: "There are no other reviews for this course yet.",
};

export const LOADING_LABEL = "Loading more reviews...";

export const END_LABEL = "You've reached the end";

export const EDIT_MODAL_TITLE = "Edit Your Review";

export const EDIT_MODAL_PROPS = {
  maxWidth: "sm" as const,
};
