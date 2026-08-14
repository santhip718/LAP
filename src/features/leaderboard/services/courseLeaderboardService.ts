import apiClient from "@/shared/services/api/apiClient";
import { getCourse } from "@/shared/services/api/services/course/course";
import { DEFAULT_PAGE_SIZE } from "@/features/leaderboard/constants/course-leaderboard.constants";
import type { LeaderboardDto } from "@/shared/services/api/models";
import type { LeaderboardUser } from "@/features/leaderboard/types/leaderboard.types";

const courseApi = getCourse(apiClient);

function mapLeaderboardDto(dto: LeaderboardDto): LeaderboardUser {
  return {
    user_id: dto.user_id!,
    full_name: dto.full_name ?? "",
    overall_weighted_score: dto.overall_weighted_score ?? 0,
    rank: dto.rank ?? 0,
    tier_awarded: "",
  };
}

export async function getCourseLeaderboard(
  courseId: string,
  pageSize: number = DEFAULT_PAGE_SIZE,
): Promise<LeaderboardUser[]> {
  const { data } = await courseApi.getApiV1CourseCourseIdLeaderboard(courseId, {
    pageSize,
  });
  const items = Array.isArray(data) ? data : [];
  return items.map(mapLeaderboardDto);
}
