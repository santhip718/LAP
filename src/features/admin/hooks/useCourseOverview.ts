import { useCallback, useEffect, useRef, useState } from "react";
import { courseService } from "../services/courseService";
import type { CourseOverviewItem, UseCourseOverviewResult } from "../types";
import { courseOverviewStrings } from "../pages/CourseOverview/CourseOverview.constants";

export function useCourseOverview(courseId: string | undefined): UseCourseOverviewResult {
  const [course, setCourse] = useState<CourseOverviewItem | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const isMountedRef = useRef(true);

  const fetchCourse = useCallback(async () => {
    if (!courseId) {
      if (isMountedRef.current) {
        setError(courseOverviewStrings.error.courseIdMissing);
        setLoading(false);
      }
      return;
    }

    setLoading(true);
    setError(null);

    try {
      const result = await courseService.getCourseOverview(courseId);
      if (isMountedRef.current) {
        setCourse(result);
      }
    } catch (err) {
      console.error("Failed to load course overview:", err);
      if (isMountedRef.current) {
        setError(courseOverviewStrings.error.loadFailed);
        setCourse(null);
      }
    } finally {
      if (isMountedRef.current) {
        setLoading(false);
      }
    }
  }, [courseId]);

  useEffect(() => {
    isMountedRef.current = true;
    queueMicrotask(() => {
      void fetchCourse();
    });
    return () => {
      isMountedRef.current = false;
    };
  }, [fetchCourse]);

  return {
    course,
    loading,
    error,
    refresh: fetchCourse,
  };
}
