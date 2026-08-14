export interface CreateCourseRequest {
  title: string;
  description: string;
  categoryId: string;
  subCategoryId?: string;
  difficultyLevelId: string;
  durationHours: number;
  thumbnailFile?: File | FileList;
  isDrafted: boolean;
  topics: import("./courseFormTypes").CourseTopicFormInput[];
  deletedContentIds?: string[];
}

export interface AdminCourseListItem {
  id: string;
  title: string;
  category: string;
  difficulty: string;
  durationMinute: number;
  rating: number;
  thumbnailUrl?: string;
  isDrafted: boolean;
}

export interface AdminCourseListResult {
  courses: AdminCourseListItem[];
  total: number;
  page: number;
  pageSize: number;
}

export interface AdminCourseSummary {
  totalCourses: number;
  publishedCourses: number;
  draftCourses: number;
  activeStudents: number;
  totalEnrollments: number;
}

export interface ForumMessage {
  id: string;
  courseId: string;
  userId: string;
  userFullName: string;
  messageText: string;
  dateCreated: string;
}

export interface CourseEditData {
  id: string;
  title: string;
  description: string;
  categoryId: string;
  subCategoryId: string;
  difficultyLevelId: string;
  durationHours: number;
  thumbnailUrl?: string;
  isDrafted: boolean;
  createdBy: string;
  topics: CourseEditTopic[];
}

export interface CourseEditTopic {
  name: string;
  contentTitle: string;
  metaTopicOrder: number;
  metaTopicDuration: number;
  sequenceOrder: number;
  contentTypeId: string;
  contentTypeName?: string;
  videoUrl?: string;
  pdfFilePath?: string;
  contentId?: string;
}

export interface CourseOverviewTopic {
  id: string;
  name: string;
  sequenceOrder: number;
  durationMinute: number;
  contents: CourseOverviewContent[];
}

export interface CourseOverviewContent {
  id: string;
  metaTopicId: string;
  sequenceOrder: number;
  title: string;
}

export interface CourseOverviewItem {
  id: string;
  title: string;
  description: string;
  category: string;
  subCategory: string;
  difficulty: string;
  durationMinute: number;
  rating: number;
  thumbnailUrl?: string;
  isDrafted: boolean;
  createdBy: string;
  dateCreated: string;
  dateUpdated: string;
  enrollmentCount: number;
  assessmentTitle: string;
  totalMark: number;
  passingMark: number;
  topics: CourseOverviewTopic[];
}
