import apiClient from '@/shared/services/api/apiClient';
import { getLeaderboard } from '@/shared/services/api/services/leaderboard/leaderboard';
import type { LeaderboardDto } from '@/shared/services/api/models';
import type { LeaderboardUser } from '../types/leaderboard.types';

const leaderboardApi = getLeaderboard(apiClient);

function mapLeaderboardDto(dto: LeaderboardDto): LeaderboardUser {
  return {
    user_id: dto.user_id ?? '',
    full_name: dto.full_name ?? '',
    overall_weighted_score: dto.overall_weighted_score ?? 0,
    rank: dto.rank ?? 0,
    tier_awarded: '',
  };
}

export async function getOverallLeaderboard(
  pageSize: number = 25,
): Promise<LeaderboardUser[]> {
  const { data } = await leaderboardApi.getApiV1LeaderboardOverall({
    pageSize,
  });
  const items = Array.isArray(data) ? data : [];
  return items.map(mapLeaderboardDto);
}
