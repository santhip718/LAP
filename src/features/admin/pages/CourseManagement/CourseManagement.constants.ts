export const DEFAULT_PAGE_SIZE = 10;
export const DEBOUNCE_DELAY = 400;

export const deleteCourseStrings = {
  title: "Delete Course",
  message: (courseTitle: string) => `Are you sure you want to delete "${courseTitle}"? This action cannot be undone.`,
  confirmLabel: "Delete",
  cancelLabel: "Cancel",
  success: "Course deleted successfully.",
  error: "Failed to delete course. Please try again.",
} as const;

export const editCourseStrings = {
  title: "Edit Course",
  message: "This course was created by another admin. Are you sure you want to make changes?",
  confirmLabel: "Continue",
  cancelLabel: "Cancel",
} as const;

export const courseManagementStrings = {
  error: {
    loadFailed: "Failed to load course data. Please try again.",
  },
  ariaLabels: {
    editCourse: "Edit course",
    deleteCourse: "Delete course",
  },
} as const;

export const courseListingStrings = {
  pageTitle: "Courses",
  pageSubtitle: "Manage courses, syllabus, and student progress.",
  addCourseButton: "Add Course",
  stats: {
    totalCourses: "Total Courses",
    published: "Published",
    activeStudents: "Active Students",
    enrollments: "Enrollments",
  },
  table: {
    title: "All Courses",
    activeBadge: "Active",
    searchPlaceholder: "Search courses",
    filterAll: "All statuses",
    filterPublished: "Published",
    filterDraft: "Draft",
    emptyStateLoading: "Loading courses...",
    emptyStateLoadingMore: "Loading more courses...",
    errorRetry: "Retry",
  },
  columns: {
    title: "Course Title",
    category: "Category",
    difficulty: "Difficulty",
    duration: "Duration",
    rating: "Rating",
    status: "Status",
    actions: "",
  },
  status: {
    drafted: "Draft",
    published: "Published",
  },
} as const;

export const createCourseModalStrings = {
  title: "Create New Course",
  editTitle: "Edit Course",
  loadingReferenceData: "Loading reference data...",
  loadingCourseData: "Loading course data...",
  success: {
    draftSaved: "Course draft saved successfully!",
    published: "Course published successfully!",
    updated: "Course updated successfully!",
  },
  error: {
    draftFailed: "Failed to save draft. Please try again.",
    publishFailed: "Failed to publish course. Please try again.",
    updateFailed: "Failed to update course. Please try again.",
    loadFailed: "Failed to load course data. Please try again.",
    unexpectedError: "An unexpected error occurred",
    saveFailed: "Failed to save course",
  },
} as const;

export const basicInfoSectionStrings = {
  cardTitle: "Basic Information",
  cardSubtitle: "Provide the core details about your course",
  labels: {
    courseTitle: "Course Title",
    description: "Description",
    category: "Category",
    subcategory: "Subcategory",
    difficultyLevel: "Difficulty Level",
    durationHours: "Duration (hours)",
    thumbnailImage: "Thumbnail Image",
  },
  placeholders: {
    courseTitle: "e.g., Advanced React Development",
    description: "Describe what students will learn...",
    category: "Select category",
    subcategory: "Select subcategory",
    difficultyLevel: "Select difficulty",
    durationHours: "e.g., 10",
  },
  validation: {
    titleRequired: "Title is required",
    descriptionRequired: "Description is required",
    categoryRequired: "Category is required",
    difficultyRequired: "Difficulty level is required",
    durationRequired: "Duration is required",
    durationMin: "Minimum 1 hour",
  },
  dropzone: {
    dragDropText: "Drag & drop or ",
    browseLink: "browse",
    hint: "PNG, JPG up to 5MB",
    replaceText: "Click to Replace Image",
  },
  requiredIndicator: "*",
} as const;

export const contentSectionStrings = {
  cardTitle: "Course Content",
  cardSubtitle: "Define the meta topics, content types, and materials for your course",
  labels: {
    metaTopicName: "Meta Topic Name",
    contentTitle: "Content Title",
    metaTopicOrder: "Meta Topic Order",
    metaTopicDuration: "Meta Topic Duration (mins)",
    sequenceOrder: "Sequence Order",
    contentType: "Content Type",
    uploadFile: "Upload File",
    videoUrl: "Video URL",
  },
  placeholders: {
    metaTopicName: "e.g., Introduction to React",
    contentTitle: "e.g., React Basics Video",
    metaTopicOrder: "e.g., 1",
    metaTopicDuration: "e.g., 45",
    sequenceOrder: "e.g., 1",
    contentType: "Select type",
    videoUrl: "e.g., https://www.youtube.com/watch?v=...",
  },
  validation: {
    metaTopicNameRequired: "Meta topic name is required",
    contentTitleRequired: "Content title is required",
    metaTopicOrderRequired: "Meta topic order is required",
    metaTopicOrderMin: "Must be at least 1",
    metaTopicDurationRequired: "Meta topic duration is required",
    metaTopicDurationMin: "Must be at least 1 minute",
    sequenceOrderRequired: "Sequence order is required",
    sequenceOrderMin: "Must be at least 1",
    contentTypeRequired: "Content type is required",
    pdfFileRequired: "PDF file is required",
    videoUrlRequired: "Video URL is required",
    videoUrlInvalid: "Please enter a valid URL",
  },
  dropzone: {
    pdfPlaceholder: "Upload PDF (e.g. syllabus, notes)",
    pdfUploadIcon: "upload_file",
    pdfCheckIcon: "check_circle",
  },
  addTopicButton: "Add Content Meta Topic",
  topicLabel: "Meta Topic",
  removeButtonAriaLabel: "Remove meta topic",
  confirmRemoveTitle: "Delete Content",
  confirmRemoveMessage: "Are you sure you want to delete this meta topic? The content will be permanently removed.",
} as const;

export const formActionsStrings = {
  cancel: "Cancel",
  saveAsDraft: "Save as Draft",
  updateDraft: "Update Draft",
  publishCourse: "Publish Course",
  updateAndPublish: "Update & Publish",
  saving: "Saving...",
  publishIcon: "rocket_launch",
  cancelIcon: "close",
} as const;
