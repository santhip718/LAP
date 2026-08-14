import apiClient from "@/shared/services/api/config/axios";
import { getCourse } from "@/shared/services/api/services/course/course";
import {
  COURSE_PAGINATION,
  MINUTES_PER_HOUR,
  DURATION_FORMATS,
  DEFAULT_IS_BESTSELLER,
} from "@/features/user/constants/constants";
import type { Course, GetCoursesResult } from "../types/courseService.types";
import type {
  GetApiV1CourseParams,
  CourseSummaryDto,
  PaginatedCoursesDto,
} from "@/shared/services/api/models";

const courseApi = getCourse(apiClient);

function formatDuration(minutes: number): string {
  const h = Math.floor(minutes / MINUTES_PER_HOUR);
  const m = minutes % MINUTES_PER_HOUR;
  if (h === 0) return `${m}${DURATION_FORMATS.MINUTES_SUFFIX}`;
  if (m === 0) return `${h}${DURATION_FORMATS.HOURS_SUFFIX}`;
  return `${h}${DURATION_FORMATS.HOURS_SUFFIX_SPACE}${m}${DURATION_FORMATS.MINUTES_SUFFIX}`;
}

function mapDtoToCourse(dto: CourseSummaryDto): Course {
  return {
    id: dto.id!,
    title: dto.title!,
    category: dto.category?.name ?? "",
    categoryId: dto.category?.id ?? "",
    duration: formatDuration(dto.duration_minute ?? 0),
    level: dto.difficulty_level?.name ?? "",
    rating: (dto.overall_rating ?? 0).toFixed(1),
    image: dto.thumbnail_img ?? "",
    alt: dto.title!,
    isBestseller: DEFAULT_IS_BESTSELLER,
  };
}

export type { GetCoursesResult };

export async function getRecommendedCourses(): Promise<Course[]> {
  const response = await courseApi.getApiV1CourseRecommendation();
  return (response.data ?? []).map((dto) => mapDtoToCourse(dto));
}

export async function getActiveCategories(): Promise<{ id: string; name: string }[]> {
  const { data } = await courseApi.getApiV1CourseActiveCategory();
  return (data ?? []).map((dto) => ({
    id: dto.id ?? "",
    name: dto.name ?? "",
  }));
}

export async function getCourses(
  params?: GetApiV1CourseParams,
): Promise<GetCoursesResult> {
  const response = await courseApi.getApiV1Course(params);
  const data: PaginatedCoursesDto = response.data;
  return {
    courses: (data.data ?? []).map((dto) => mapDtoToCourse(dto)),
    total: data.total ?? 0,
    page: data.page ?? 1,
    pageSize:
      data.page_size ?? params?.pageSize ?? COURSE_PAGINATION.DEFAULT_PAGE_SIZE,
  };
}
