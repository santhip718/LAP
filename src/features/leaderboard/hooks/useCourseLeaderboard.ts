import { useState, useEffect, useCallback, useRef } from 'react';
import { getCourseLeaderboard } from '../services/courseLeaderboardService';
import { DEFAULT_PAGE_SIZE } from '@/features/leaderboard/constants/course-leaderboard.constants';
import type { UseCourseLeaderboardResult } from '@/features/leaderboard/types/course-leaderboard.types';
import type { LeaderboardUser } from '../types/leaderboard.types';

export function useCourseLeaderboard(
  courseId: string,
  pageSize = DEFAULT_PAGE_SIZE
): UseCourseLeaderboardResult {
  const [leaderboard, setLeaderboard] = useState<LeaderboardUser[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<Error | null>(null);
  const abortRef = useRef<AbortController | null>(null);

  const fetchLeaderboard = useCallback(() => {
    abortRef.current?.abort();
    const controller = new AbortController();
    abortRef.current = controller;

    setLoading(true);
    setError(null);

    getCourseLeaderboard(courseId, pageSize)
      .then((data) => {
        if (!controller.signal.aborted) {
          setLeaderboard(data);
          setLoading(false);
        }
      })
      .catch((err: unknown) => {
        if (!controller.signal.aborted) {
          setError(err instanceof Error ? err : new Error(String(err)));
          setLoading(false);
        }
      });
  }, [courseId, pageSize]);

  useEffect(() => {
    fetchLeaderboard();
    return () => {
      abortRef.current?.abort();
    };
  }, [fetchLeaderboard]);

  return { leaderboard, loading, error, refetch: fetchLeaderboard };
}
