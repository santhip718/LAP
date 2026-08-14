import { useState, useEffect, useCallback, useRef } from 'react';
import type { AssessmentOverviewDto } from '@/shared/services/api/models/assessmentOverviewDto';
import { getAssessments, deleteAssessment as deleteAssessmentService } from '../services/adminService';

interface UseAssessmentsResult {
  items: AssessmentOverviewDto[];
  isLoading: boolean;
  error: Error | null;
  refetch: () => void;
  deleteAssessment: (id: string) => Promise<void>;
  isDeleting: boolean;
  loadMore: () => void;
  loadingMore: boolean;
  hasMore: boolean;
}

export function useAssessments(): UseAssessmentsResult {
  const [items, setItems] = useState<AssessmentOverviewDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [loadingMore, setLoadingMore] = useState(false);
  const [error, setError] = useState<Error | null>(null);
  const [isDeleting, setIsDeleting] = useState(false);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);

  const abortRef = useRef<AbortController | null>(null);
  const isMountedRef = useRef(true);

  const fetchAssessments = useCallback(() => {
    abortRef.current?.abort();
    const controller = new AbortController();
    abortRef.current = controller;

    setIsLoading(true);
    setError(null);
    setPage(1);

    getAssessments(1, 10)
      .then((data) => {
        if (!controller.signal.aborted && isMountedRef.current) {
          setItems(data);
          setHasMore(data.length === 10);
          setIsLoading(false);
        }
      })
      .catch((err: unknown) => {
        if (!controller.signal.aborted && isMountedRef.current) {
          setError(err instanceof Error ? err : new Error(String(err)));
          setIsLoading(false);
          setItems([]);
          setHasMore(false);
        }
      });
  }, []);

  const loadMore = useCallback(async () => {
    if (loadingMore || isLoading || !hasMore) return;
    setLoadingMore(true);

    try {
      const nextPage = page + 1;
      const data = await getAssessments(nextPage, 10);
      if (isMountedRef.current) {
        setItems((prev) => [...prev, ...data]);
        setPage(nextPage);
        setHasMore(data.length === 10);
      }
    } catch (err: unknown) {
      console.error("Failed to load more assessments:", err);
    } finally {
      if (isMountedRef.current) {
        setLoadingMore(false);
      }
    }
  }, [loadingMore, isLoading, hasMore, page]);

  const deleteAssessment = useCallback(async (id: string): Promise<void> => {
    setIsDeleting(true);
    try {
      await deleteAssessmentService(id);
    } finally {
      setIsDeleting(false);
    }
  }, []);

  useEffect(() => {
    isMountedRef.current = true;
    fetchAssessments();
    return () => {
      isMountedRef.current = false;
      abortRef.current?.abort();
    };
  }, [fetchAssessments]);

  return {
    items,
    isLoading,
    error,
    refetch: fetchAssessments,
    deleteAssessment,
    isDeleting,
    loadMore,
    loadingMore,
    hasMore,
  };
}
