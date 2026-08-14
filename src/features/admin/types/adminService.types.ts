import type { CourseSummaryDto } from "@/shared/services/api/models";

export interface AdminCourseSummaryModel {
  totalCourses: number;
  publishedCourses: number;
  draftCourses: number;
  totalEnrollments: number;
  activeStudents: number;
}

export interface AssessmentModel {
  id: string;
  title: string;
  description: string | null;
  totalMark: number;
  passingMark: number;
  durationMinute: number;
  course: CourseSummaryDto | undefined;
}

export interface AssessmentQuestionModel {
  id: string;
  assessmentId: string;
  metaTopicId: string;
  questionType: { id: string; name: string };
  questionText: string;
  optionList: string[];
  answer: string;
  weight: number;
}

export interface GetAssessmentsResult {
  data: AssessmentModel[];
}

export interface GetAssessmentQuestionsResult {
  data: AssessmentQuestionModel[];
}

export interface GetUsersResult {
  data: UserAdminModel[];
  total: number;
  page: number;
  pageSize: number;
}

export interface UserAdminModel {
  id: string;
  fullName: string;
  email: string;
  mobileNumber: string;
  designation: string;
  gender: string;
  roles: string[];
  dateCreated: string;
  profileImage: string | null;
}

export interface GetEnrollmentsResult {
  data: EnrollmentAdminModel[];
  total: number;
  page: number;
  pageSize: number;
}

export interface EnrollmentAdminModel {
  id: string;
  userId: string;
  courseId: string;
  enrolledOn: string;
  completedOn: string | null;
  progressPercentage: number;
  enrollmentStatus: boolean;
  courseTitle: string;
  userFullName: string;
}

export interface GetCoursesResult {
  data: CourseSummaryDto[];
  total: number;
  page: number;
  pageSize: number;
}
