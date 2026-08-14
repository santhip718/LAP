import { useCallback, useEffect, useRef, useState } from "react";
import { enrollmentService } from "../services/enrollmentService";
import type { EnrollmentFilters, UseEnrollmentsResult, EnrollmentItem } from "../types";
import { ENROLLMENT_PAGE_SIZE, enrollmentStrings } from "../pages/EnrollmentManagement/EnrollManagement.constants";

export function useEnrollments(): UseEnrollmentsResult {
  const [enrollments, setEnrollments] = useState<EnrollmentItem[]>([]);
  const [total, setTotal] = useState(0);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [refreshing, setRefreshing] = useState(false);
  const [filters, setFilters] = useState<EnrollmentFilters>({});
  const isMountedRef = useRef(true);

  const fetchEnrollments = useCallback(async (isRefresh = false, currentFilters?: EnrollmentFilters) => {
    if (isRefresh) {
      setRefreshing(true);
    } else {
      setLoading(true);
    }
    setError(null);

    try {
      const result = await enrollmentService.getEnrollments({
        page: 1,
        pageSize: ENROLLMENT_PAGE_SIZE,
        ...currentFilters,
      });
      if (isMountedRef.current) {
        setEnrollments(result.enrollments);
        setTotal(result.total);
      }
    } catch (err) {
      console.error("Failed to load enrollments:", err);
      if (isMountedRef.current) {
        setError(enrollmentStrings.error.loadFailed);
        setEnrollments([]);
      }
    } finally {
      if (isMountedRef.current) {
        setLoading(false);
        setRefreshing(false);
      }
    }
  }, []);

  const refresh = useCallback(() => {
    void fetchEnrollments(true, filters);
  }, [fetchEnrollments, filters]);

  const handleSetFilters = useCallback((newFilters: EnrollmentFilters) => {
    setFilters(newFilters);
  }, []);

  useEffect(() => {
    isMountedRef.current = true;
    queueMicrotask(() => {
      void fetchEnrollments(false, filters);
    });
    return () => {
      isMountedRef.current = false;
    };
  }, [fetchEnrollments, filters]);

  return {
    enrollments,
    total,
    loading,
    error,
    refreshing,
    refresh,
    setFilters: handleSetFilters,
    filters,
  };
}
