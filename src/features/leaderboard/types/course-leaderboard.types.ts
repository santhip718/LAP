import type { LeaderboardUser } from './leaderboard.types';

export interface CourseLeaderboardPageProps {
  courseId?: string;
}

export interface UseCourseLeaderboardResult {
  leaderboard: LeaderboardUser[];
  loading: boolean;
  error: Error | null;
  refetch: () => void;
}
