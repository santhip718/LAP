export { default as AdminDashboard } from "./pages/AdminDashboard/AdminDashboard";
export { default as CourseManagement } from "./pages/CourseManagement/CourseManagement";
export { default as CourseOverview } from "./pages/CourseOverview/CourseOverview";
export { default as EnrollmentManagement } from "./pages/EnrollmentManagement/EnrollmentManagement";
export { default as AssessmentManagement } from "./pages/AssessmentManagement/AssessmentManagement";
export { default as AssessmentOverview } from "./pages/AssessmentOverview/AssessmentOverview";
export { default as CreateCourseModal } from "./components/CreateCourseModal/CreateCourseModal";
export { default as LapCourseDiscussion } from "@/shared/components/ui/LapCourseDiscussion/LapCourseDiscussion";
export { default as StatCard } from "./components/StatCard/StatCard";
export { courseService } from "./services/courseService";
export { enrollmentService } from "./services/enrollmentService";
export * from "./services/adminService";
export { useAdminCourses } from "./hooks/useAdminCourses";
export { useCourseOverview } from "./hooks/useCourseOverview";
export { useEnrollments } from "./hooks/useEnrollments";
export { useForumMessages } from "@/shared/hooks/useForumMessages";
export { useReferenceData } from "./hooks/useReferenceData";
export { useAssessments } from "./hooks/useAssessments";
export type {
  AdminCourseListItem,
  AdminCourseSummary,
  CourseOverviewItem,
  CourseEditData,
  CreateCourseRequest,
  CourseStatusFilter,
  EnrollmentItem,
  EnrollmentListResult,
  EnrollmentFilters,
  UseEnrollmentsResult,
  CreateCourseModalProps,
  CourseOverviewTopic,
} from "./types";
export type { ForumMessage, LapCourseDiscussionProps } from "@/shared/components/ui/LapCourseDiscussion/LapCourseDiscussion.types";
