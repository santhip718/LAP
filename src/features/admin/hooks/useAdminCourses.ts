import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { courseService } from "../services/courseService";
import type { AdminCourseListItem, AdminCourseSummary, UseAdminCoursesParams, CourseStatusFilter } from "../types";
import { DEFAULT_PAGE_SIZE } from "../pages/CourseManagement/CourseManagement.constants";
import { courseServiceStrings } from "../utils/constants";

const PAGE_SIZE = DEFAULT_PAGE_SIZE;

const EMPTY_SUMMARY: AdminCourseSummary = {
  totalCourses: 0,
  publishedCourses: 0,
  draftCourses: 0,
  activeStudents: 0,
  totalEnrollments: 0,
};

export function useAdminCourses({ search, status }: UseAdminCoursesParams) {
  const [courses, setCourses] = useState<AdminCourseListItem[]>([]);
  const [totalCourses, setTotalCourses] = useState(0);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [summary, setSummary] = useState<AdminCourseSummary>(EMPTY_SUMMARY);
  const [loading, setLoading] = useState(true);
  const [summaryLoading, setSummaryLoading] = useState(true);
  const [loadingMore, setLoadingMore] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [summaryError, setSummaryError] = useState<string | null>(null);

  const normalizedSearch = useMemo(() => search.trim(), [search]);
  const statusRef = useRef(status);
  const searchRef = useRef(normalizedSearch);
  const loadedCountRef = useRef(0);

  useEffect(() => {
    statusRef.current = status;
    searchRef.current = normalizedSearch;
  });

  useEffect(() => {
    let cancelled = false;

    const loadSummary = async () => {
      setSummaryLoading(true);
      setSummaryError(null);

      try {
        const result = await courseService.getAdminCourseSummary();
        if (!cancelled) {
          setSummary(result);
        }
      } catch (err) {
        console.error("Failed to load admin course summary:", err);
        if (!cancelled) {
          setSummaryError(courseServiceStrings.error.summaryMetricsFailed);
        }
      } finally {
        if (!cancelled) {
          setSummaryLoading(false);
        }
      }
    };

    queueMicrotask(() => {
      void loadSummary();
    });

    return () => {
      cancelled = true;
    };
  }, []);

  const fetchCourses = useCallback(async () => {
    setLoading(true);
    setError(null);
    setPage(1);
    setHasMore(true);
    loadedCountRef.current = 0;

    const statusParam = status === "all" ? undefined : status === "published";

    try {
      const courseResult = await courseService.getAdminCourses({
        search: normalizedSearch || undefined,
        status: statusParam,
        page: 1,
        pageSize: PAGE_SIZE,
      });

      setCourses(courseResult.courses);
      setTotalCourses(courseResult.total);
      setPage(1);
      loadedCountRef.current = courseResult.courses.length;
      setHasMore(courseResult.courses.length < courseResult.total);
    } catch (err) {
      console.error("Failed to load admin courses:", err);
      setError(courseServiceStrings.error.loadAdminCoursesFailed);
      setCourses([]);
      setTotalCourses(0);
      setHasMore(false);
      loadedCountRef.current = 0;
    } finally {
      setLoading(false);
    }
  }, [normalizedSearch, status]);

  const loadMore = useCallback(async () => {
    if (loadingMore || !hasMore || loading) return;
    setLoadingMore(true);

    const nextPage = page + 1;
    const currentSearch = searchRef.current;
    const currentStatus = statusRef.current;

    try {
      const statusParam =
        currentStatus === "all" ? undefined : currentStatus === "published";
      const courseResult = await courseService.getAdminCourses({
        search: currentSearch || undefined,
        status: statusParam,
        page: nextPage,
        pageSize: PAGE_SIZE,
      });

      setCourses((prev) => [...prev, ...courseResult.courses]);
      setPage(nextPage);
      loadedCountRef.current += courseResult.courses.length;
      setHasMore(loadedCountRef.current < courseResult.total);
    } catch (err) {
      console.error("Failed to load more courses:", err);
    } finally {
      setLoadingMore(false);
    }
  }, [loadingMore, hasMore, loading, page]);

  useEffect(() => {
    queueMicrotask(() => {
      void fetchCourses();
    });
  }, [fetchCourses]);

  return {
    courses,
    totalCourses,
    summary,
    loading,
    summaryLoading,
    loadingMore,
    error,
    summaryError,
    hasMore,
    loadMore,
    refreshCourses: fetchCourses,
  };
}
