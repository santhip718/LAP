export const ASSESSMENT_FORM = {
  LABEL_TITLE: "Assessment Title",
  LABEL_DESCRIPTION: "Description",
  LABEL_PASSING_MARK: "Passing Mark",
  LABEL_DURATION: "Duration",
  LABEL_COURSE: "Course",
  LABEL_QUESTION_FILE: "Question File",
  COURSE_PLACEHOLDER: "-- Select a course --",
  COURSES_LOADING: "Loading courses...",
  LABEL_OPTIONAL_EDIT: "(optional for edit)",

  PLACEHOLDER_TITLE: "e.g., Advanced Java Fundamentals - Midterm",
  PLACEHOLDER_DESCRIPTION:
    "Briefly describe the learning outcomes of this assessment...",
  PLACEHOLDER_PASSING_MARK: "70",
  PLACEHOLDER_DURATION: "30",
  DURATION_SUFFIX: "min",

  FILE_DROP_TEXT: "Drop your question file here",
  FILE_BROWSE_HINT: "or",
  FILE_BROWSE_LINK: "browse files",
  FILE_FORMAT_HINT: "XLSX, XLS (max 10 MB)",
  FILE_UPLOADED_STATUS: "Uploaded successfully",

  VALIDATION_TITLE_REQUIRED: "Title is required",
  VALIDATION_DESCRIPTION_REQUIRED: "Description is required",
  VALIDATION_PASSING_MARK_REQUIRED: "Passing mark is required",
  VALIDATION_PASSING_MARK_MIN: "Minimum value is 0",
  VALIDATION_DURATION_REQUIRED: "Duration is required",
  VALIDATION_DURATION_MIN: "Minimum duration is 1 minute",
  VALIDATION_FILE_REQUIRED: "Question file is required",
  VALIDATION_FILE_TYPE: "Only .xlsx and .xls files are allowed",
  VALIDATION_FILE_SIZE: "File size must not exceed 10 MB",

  BTN_CANCEL: "Cancel",
  BTN_PROCESSING: "Processing...",
  BTN_CREATE: "Create Assessment",
  BTN_UPDATE: "Update Assessment",

  LABEL_DOWNLOAD_TEMPLATE: "Question File Template",
  BTN_DOWNLOAD_TEMPLATE: "Download Template",

  TOAST_CREATED: "Assessment created successfully",
  TOAST_UPDATED: "Assessment updated successfully",
  TOAST_ERROR: "Operation failed",
  TOAST_FILE_READ_ERROR: "Failed to read file",
  TOAST_TEMPLATE_DOWNLOADED: "Template downloaded successfully",
  TOAST_TEMPLATE_ERROR: "Failed to download template",
} as const;

export const ALLOWED_EXTENSIONS = [".xlsx", ".xls"] as const;
export const MAX_FILE_SIZE = 10 * 1024 * 1024;
