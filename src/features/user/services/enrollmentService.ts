import apiClient from "@/shared/services/api/config/axios";
import { getEnrollment } from "@/shared/services/api/services/enrollment/enrollment";
import { getCourse } from "@/shared/services/api/services/course/course";
import type { EnrolledCourse, GetEnrollmentsResult } from "../types/enrollmentService.types";
import type {
  GetApiV1EnrollmentParams,
  EnrollmentDetailDto,
  PaginatedEnrollmentsDto,
} from "@/shared/services/api/models";
import { ENROLLMENT_PAGE_SIZE } from "@/features/user/constants/constants";

export type { EnrolledCourse, GetEnrollmentsResult };

const enrollmentApi = getEnrollment(apiClient);
const courseApi = getCourse(apiClient);

function mapDtoToCourse(dto: EnrollmentDetailDto): EnrolledCourse {
  return {
    id: dto.id!,
    courseId: dto.course_id!,
    title: dto.course_title!,
    category: dto.course_category?.name ?? "",
    enrolledOn: dto.enrolled_on ?? "",
    completedOn: dto.completed_on ?? null,
    progress: dto.progress_percentage ?? 0,
    status: dto.enrollment_status ?? false,
    thumbnail: (dto as Record<string, unknown>).thumbnail_img as string ?? "",
  };
}

export async function getEnrollments(
  params?: GetApiV1EnrollmentParams,
): Promise<GetEnrollmentsResult> {
  const response = await enrollmentApi.getApiV1Enrollment(params);
  const data: PaginatedEnrollmentsDto = response.data;
  return {
    courses: (data.data ?? []).map(mapDtoToCourse),
    total: data.total ?? 0,
    page: data.page ?? 1,
    pageSize: data.page_size ?? params?.pageSize ?? ENROLLMENT_PAGE_SIZE,
  };
}

export async function enrollInCourse(courseId: string): Promise<void> {
  await courseApi.postApiV1CourseCourseIdEnrollment(courseId);
}
