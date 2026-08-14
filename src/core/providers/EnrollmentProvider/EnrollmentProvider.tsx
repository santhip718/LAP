import {
  useCallback,
  useEffect,
  useMemo,
  useRef,
  useState,
  type ReactNode,
} from "react";
import {
  getEnrollments,
  enrollInCourse,
  type EnrolledCourse,
} from "@/features/user/services/enrollmentService";
import { useAuth } from "@/core/providers/AuthProvider/useAuth";
import { EnrollmentContext } from "./EnrollmentContext";

export function EnrollmentProvider({ children }: { children: ReactNode }) {
  const [enrolledCourses, setEnrolledCourses] = useState<
    Record<string, EnrolledCourse>
  >({});
  const [loading, setLoading] = useState(true);
  const { isAuthenticated } = useAuth();
  const prevAuth = useRef(isAuthenticated);

  const refresh = useCallback(async () => {
    try {
      const result = await getEnrollments();
      const coursesMap = result.courses.reduce<Record<string, EnrolledCourse>>(
        (accumulator: Record<string, EnrolledCourse>, course: EnrolledCourse) => {
          accumulator[course.courseId] = course;
          return accumulator;
        },
        {},
      );
      setEnrolledCourses(coursesMap);
    } catch {
      setEnrolledCourses({});
    }
  }, []);

  useEffect(() => {
    const justLoggedIn = isAuthenticated && !prevAuth.current;
    prevAuth.current = isAuthenticated;
    if (!isAuthenticated && !justLoggedIn) {
      setLoading(false);
      return;
    }
    refresh().finally(() => setLoading(false));
  }, [isAuthenticated, refresh]);

  const enroll = useCallback(
    async (courseId: string) => {
      setEnrolledCourses((prev) => ({
        ...prev,
        [courseId]: {
          id: "",
          courseId,
          thumbnail: "",
          title: "",
          category: "",
          enrolledOn: new Date().toISOString(),
          completedOn: null,
          progress: 0,
          status: false,
        },
      }));
      try {
        await enrollInCourse(courseId);
        await refresh();
      } catch {
        setEnrolledCourses((prev) => {
          const next = { ...prev };
          delete next[courseId];
          return next;
        });
      }
    },
    [refresh],
  );

  const value = useMemo(
    () => ({
      enrolledCourses,
      loading,
      enroll,
    }),
    [enrolledCourses, loading, enroll],
  );

  return (
    <EnrollmentContext.Provider value={value}>
      {children}
    </EnrollmentContext.Provider>
  );
}
