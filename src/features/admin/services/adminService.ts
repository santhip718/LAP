import apiClient from "@/shared/services/api/apiClient";
import { getAssessment } from "@/shared/services/api/services/assessment/assessment";
import { getCourse } from "@/shared/services/api/services/course/course";
import { getUser } from "@/shared/services/api/services/user/user";
import { getEnrollment } from "@/shared/services/api/services/enrollment/enrollment";

import type {
  AssessmentOverviewDto,
  QuestionDto,
  UpdateQuestionRequestDto,
  UpdateAssessmentRequestDto,
  PostApiV1AssessmentBody,
  AssessmentResultResponseDto,
  SuccessResponse,
  AdminCourseSummaryDto,
  GetApiV1CourseParams,
  PaginatedCoursesDto,
  CourseSummaryDto,
  GetApiV1UserParams,
  PaginatedUsersDto,
  UserDetailDto,
  UserEnrichedDto,
  UpdateUserRequestDto,
  GetApiV1EnrollmentParams,
  PaginatedEnrollmentsDto,
  EnrollmentDetailDto,
  UpdateEnrollmentRequestDto,
  PostApiV1CourseBody,
  PutApiV1CourseCourseIdBody,
} from "@/shared/services/api/models";

import type {
  AdminCourseSummaryModel,
  UserAdminModel,
  GetUsersResult,
  EnrollmentAdminModel,
  GetEnrollmentsResult,
  GetCoursesResult,
} from "../types/adminService.types";

const assessmentApi = getAssessment(apiClient);
const courseApi = getCourse(apiClient);
const userApi = getUser(apiClient);
const enrollmentApi = getEnrollment(apiClient);



function mapUserDetailDtoToModel(dto: UserDetailDto): UserAdminModel {
  return {
    id: dto.id!,
    fullName: dto.full_name ?? "",
    email: dto.email ?? "",
    mobileNumber: dto.mobile_number ?? "",
    designation: dto.designation?.name ?? "",
    gender: dto.gender?.name ?? "",
    roles: dto.roles ?? [],
    dateCreated: dto.date_created ?? "",
    profileImage: dto.profile_image ?? null,
  };
}

function mapEnrollmentDtoToModel(dto: EnrollmentDetailDto): EnrollmentAdminModel {
  return {
    id: dto.id!,
    userId: dto.user_id ?? "",
    courseId: dto.course_id ?? "",
    enrolledOn: dto.enrolled_on ?? "",
    completedOn: dto.completed_on ?? null,
    progressPercentage: dto.progress_percentage ?? 0,
    enrollmentStatus: dto.enrollment_status ?? false,
    courseTitle: dto.course_title ?? "",
    userFullName: dto.user_full_name ?? "",
  };
}

function extractArray<T>(raw: unknown): T[] {
  if (Array.isArray(raw)) return raw;
  if (raw && typeof raw === "object" && "data" in raw) {
    const nested = (raw as Record<string, unknown>).data;
    if (Array.isArray(nested)) return nested as T[];
  }
  return [];
}

function mapAdminSummaryDto(dto: AdminCourseSummaryDto): AdminCourseSummaryModel {
  return {
    totalCourses: dto.total_courses ?? 0,
    publishedCourses: dto.published_courses ?? 0,
    draftCourses: dto.draft_courses ?? 0,
    totalEnrollments: dto.total_enrollments ?? 0,
    activeStudents: dto.active_students ?? 0,
  };
}

export async function getAssessments(
  pageNumber?: number,
  pageSize?: number,
): Promise<AssessmentOverviewDto[]> {
  const response = await assessmentApi.getApiV1Assessment({
    pageNumber,
    pageSize,
  });
  const rawData = response.data;
  return extractArray<AssessmentOverviewDto>(rawData);
}

export async function getAssessmentById(
  id: string,
): Promise<AssessmentOverviewDto | null> {
  const items = await getAssessments();
  return items.find((a) => a.id === id) ?? null;
}

export async function getAssessmentResult(
  id: string,
): Promise<AssessmentResultResponseDto> {
  const response = await assessmentApi.getApiV1AssessmentIdResult(id);
  return response.data;
}

export async function getAssessmentQuestions(
  id: string,
): Promise<QuestionDto[]> {
  const response = await assessmentApi.getApiV1AssessmentIdQuestion(id);
  return extractArray<QuestionDto>(response.data);
}

export async function createAssessment(
  payload: PostApiV1AssessmentBody,
): Promise<SuccessResponse> {
  const response = await assessmentApi.postApiV1Assessment(payload);
  return response.data;
}

export async function updateAssessment(
  id: string,
  payload: UpdateAssessmentRequestDto,
): Promise<SuccessResponse> {
  const response = await assessmentApi.putApiV1AssessmentId(id, payload);
  return response.data;
}

export async function deleteAssessment(id: string): Promise<SuccessResponse> {
  const response = await assessmentApi.deleteApiV1AssessmentId(id);
  return response.data;
}

export async function updateQuestion(
  id: string,
  payload: UpdateQuestionRequestDto,
): Promise<SuccessResponse> {
  const response = await assessmentApi.putApiV1AssessmentQuestionId(id, payload);
  return response.data;
}

export async function deleteQuestion(id: string): Promise<SuccessResponse> {
  const response = await assessmentApi.deleteApiV1AssessmentQuestionId(id);
  return response.data;
}

export async function exportAssessmentTemplate(): Promise<void> {
  const response = await assessmentApi.getApiV1AssessmentExportTemplate({
    responseType: "blob",
  });
  const blob = new Blob([response.data as unknown as BlobPart], {
    type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
  });
  const url = URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.href = url;
  link.download = "Assessment_Import_Template.xlsx";
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
  URL.revokeObjectURL(url);
}

export async function getCourseAdminSummary(): Promise<AdminCourseSummaryModel> {
  const response = await courseApi.getApiV1CourseAdminSummary();
  return mapAdminSummaryDto(response.data);
}

export async function getCourses(
  params?: GetApiV1CourseParams,
): Promise<GetCoursesResult> {
  const response = await courseApi.getApiV1Course(params);
  const data: PaginatedCoursesDto = response.data;
  return {
    data: (data.data ?? []).map((dto) => dto as CourseSummaryDto),
    total: data.total ?? 0,
    page: data.page ?? 1,
    pageSize: data.page_size ?? params?.pageSize ?? 10,
  };
}

export async function createCourse(
  payload: PostApiV1CourseBody,
): Promise<SuccessResponse> {
  const response = await courseApi.postApiV1Course(payload);
  return response.data;
}

export async function updateCourse(
  courseId: string,
  payload: PutApiV1CourseCourseIdBody,
): Promise<SuccessResponse> {
  const response = await courseApi.putApiV1CourseCourseId(courseId, payload);
  return response.data;
}

export async function deleteCourse(courseId: string): Promise<SuccessResponse> {
  const response = await courseApi.deleteApiV1CourseCourseId(courseId);
  return response.data;
}

export async function getUsers(
  params?: GetApiV1UserParams,
): Promise<GetUsersResult> {
  const response = await userApi.getApiV1User(params);
  const data: PaginatedUsersDto = response.data;
  return {
    data: (data.data ?? []).map(mapUserDetailDtoToModel),
    total: data.total ?? 0,
    page: data.page ?? 1,
    pageSize: data.page_size ?? params?.pageSize ?? 10,
  };
}

export async function getUserById(id: string): Promise<UserEnrichedDto> {
  const response = await userApi.getApiV1UserId(id);
  return response.data;
}

export async function updateUser(
  id: string,
  payload: UpdateUserRequestDto,
): Promise<void> {
  await userApi.putApiV1UserId(id, payload);
}

export async function deleteUser(id: string): Promise<SuccessResponse> {
  const response = await userApi.deleteApiV1UserId(id);
  return response.data;
}

export async function uploadProfileImage(
  payload: FormData | { file: Blob },
): Promise<SuccessResponse> {
  const body = payload instanceof FormData
    ? { file: payload.get("file") as Blob }
    : payload;
  const response = await userApi.postApiV1UserProfileImage(body);
  return response.data;
}

export async function getEnrollments(
  params?: GetApiV1EnrollmentParams,
): Promise<GetEnrollmentsResult> {
  const response = await enrollmentApi.getApiV1Enrollment(params);
  const data: PaginatedEnrollmentsDto = response.data;
  return {
    data: (data.data ?? []).map(mapEnrollmentDtoToModel),
    total: data.total ?? 0,
    page: data.page ?? 1,
    pageSize: data.page_size ?? params?.pageSize ?? 10,
  };
}

export async function updateEnrollment(
  id: string,
  payload: UpdateEnrollmentRequestDto,
): Promise<SuccessResponse> {
  const response = await enrollmentApi.putApiV1EnrollmentId(id, payload);
  return response.data;
}
