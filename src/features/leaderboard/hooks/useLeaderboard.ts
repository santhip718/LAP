import { useState, useEffect, useCallback, useRef } from 'react';
import type { LeaderboardUser } from '../types/leaderboard.types';
import { getOverallLeaderboard } from '../services/leaderboardService';

interface UseLeaderboardResult {
  leaderboard: LeaderboardUser[];
  loading: boolean;
  error: Error | null;
  refetch: () => void;
}

export function useLeaderboard(pageSize = 25): UseLeaderboardResult {
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

    getOverallLeaderboard(pageSize)
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
  }, [pageSize]);

  useEffect(() => {
    fetchLeaderboard();
    return () => {
      abortRef.current?.abort();
    };
  }, [fetchLeaderboard]);

  return { leaderboard, loading, error, refetch: fetchLeaderboard };
}
