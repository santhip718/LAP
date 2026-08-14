import apiClient from "../../../shared/services/api/apiClient";
import { getEnrollment } from "../../../shared/services/api/services/enrollment/enrollment";
import type { EnrollmentDetailDto } from "../../../shared/services/api/models/enrollmentDetailDto";
import type { GetApiV1EnrollmentParams } from "../../../shared/services/api/models/getApiV1EnrollmentParams";
import { DEFAULT_PAGE_SIZE, FALLBACK_COURSE, FALLBACK_USER, FALLBACK_EMPTY } from "../pages/EnrollmentManagement/EnrollManagement.constants";

const enrollmentApi = getEnrollment(apiClient);
import type { EnrollmentItem, EnrollmentListResult } from "../types";

const mapEnrollment = (dto: EnrollmentDetailDto): EnrollmentItem => ({
  id: dto.id ?? "",
  userId: dto.user_id ?? "",
  courseId: dto.course_id ?? "",
  courseTitle: dto.course_title ?? FALLBACK_COURSE,
  userFullName: dto.user_full_name ?? FALLBACK_USER,
  category: dto.course_category?.name ?? FALLBACK_EMPTY,
  enrollmentStatus: dto.enrollment_status ?? false,
  enrolledOn: dto.enrolled_on ?? "",
});

export const enrollmentService = {
  async getEnrollments(
    params: GetApiV1EnrollmentParams = {},
  ): Promise<EnrollmentListResult> {
    const { data } = await enrollmentApi.getApiV1Enrollment({
      ...params,
      page: params.page ?? 1,
      pageSize: params.pageSize ?? DEFAULT_PAGE_SIZE,
    });

    const enrollments = (data.data ?? []).map(mapEnrollment);

    return {
      enrollments,
      total: data.total ?? enrollments.length,
      page: data.page ?? 1,
      pageSize: data.page_size ?? DEFAULT_PAGE_SIZE,
    };
  },

  async acceptEnrollment(id: string): Promise<void> {
    await enrollmentApi.putApiV1EnrollmentId(id, {
      enrollment_status: true,
    });
  },
};
