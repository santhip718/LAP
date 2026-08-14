import apiClient from "@/shared/services/api/config/axios";
import { getReview } from "@/shared/services/api/services/review/review";
import type { CreateReviewRequestDto, UpdateReviewRequestDto, ReviewDto } from "@/shared/services/api/models";

const reviewApi = getReview(apiClient);

export async function submitReview(
  courseId: string,
  data: { rating: number; reviewText: string },
): Promise<ReviewDto> {
  const dto: CreateReviewRequestDto = {
    rating: data.rating,
    review_text: data.reviewText || null,
  };
  const response = await reviewApi.postApiV1ReviewCourseCourseId(courseId, dto);
  return response.data;
}

export async function getCourseReviews(
  courseId: string,
  page?: number,
  pageSize?: number,
): Promise<ReviewDto[]> {
  const params: Record<string, number> = {};
  if (page !== undefined) params.page = page;
  if (pageSize !== undefined) params.pageSize = pageSize;

  const response = await reviewApi.getApiV1ReviewCourseCourseId(courseId, Object.keys(params).length > 0 ? params : undefined);

  const body = response.data as unknown;
  if (Array.isArray(body)) return body;
  if (body && typeof body === "object") {
    const obj = body as Record<string, unknown>;
    if (Array.isArray(obj.data)) return obj.data as ReviewDto[];
    if (Array.isArray(obj.item)) return obj.item as ReviewDto[];
  }
  return [];
}

export async function getUserReviewForCourse(
  courseId: string,
  userId: string,
): Promise<ReviewDto> {
  const response = await reviewApi.getApiV1ReviewCourseCourseIdUserUserIdReview(courseId, userId);
  return response.data;
}

export async function updateReview(
  id: string,
  data: { rating?: number; reviewText?: string },
): Promise<ReviewDto> {
  const dto: UpdateReviewRequestDto = {
    rating: data.rating,
    review_text: data.reviewText || null,
  };
  const response = await reviewApi.putApiV1ReviewId(id, dto);
  return response.data;
}

export async function deleteReview(id: string): Promise<void> {
  await reviewApi.deleteApiV1ReviewId(id);
}
