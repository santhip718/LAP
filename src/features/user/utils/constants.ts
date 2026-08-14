export const FALLBACK_USER_NAME = "Unknown User";
export const FALLBACK_EMPTY = "—";
export const FALLBACK_NAME = "User";
export const DEFAULT_PAGE_SIZE = 10;
export const SEARCH_PAGE_SIZE = 50;
export const CONTENT_TYPE_MULTIPART = "multipart/form-data";
export const STORAGE_KEY_PROFILE = "profileData";

export const userListStrings = {
  error: {
    loadFailed: "Failed to load users. Please try again.",
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

export const viewUserStrings = {
  title: "User Details",
  labels: {
    fullName: "Full Name",
    email: "Email",
    mobileNumber: "Mobile Number",
    designation: "Designation",
    gender: "Gender",
    currentTier: "Current Tier",
    roles: "Roles",
    dateCreated: "Date Created",
  },
} as const;

export const editUserStrings = {
  title: "Edit User",
  success: "User updated successfully!",
  error: "Failed to update user. Please try again.",
  labels: {
    fullName: "Full Name",
    email: "Email",
    mobileNumber: "Mobile Number",
    designation: "Designation",
    gender: "Gender",
    roles: "Roles",
  },
  placeholders: {
    fullName: "Enter full name",
    email: "Enter email address",
    mobileNumber: "Enter mobile number",
    designation: "Select designation",
    gender: "Select gender",
  },
  validation: {
    fullNameRequired: "Full name is required",
    emailRequired: "Email is required",
    emailInvalid: "Please enter a valid email",
  },
  loadingReferenceData: "Loading reference data...",
} as const;

export const deleteUserStrings = {
  title: "Delete User",
  message: "Are you sure you want to delete this user? This action cannot be undone.",
  confirmLabel: "Delete",
  cancelLabel: "Cancel",
  success: "User deleted successfully!",
  error: "Failed to delete user. Please try again.",
} as const;
