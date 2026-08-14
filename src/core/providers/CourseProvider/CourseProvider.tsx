import {
  useCallback,
  useEffect,
  useMemo,
  useRef,
  useState,
  type ReactNode,
} from "react";
import type { Course, FilterValues } from "@/features/user/types/courseService.types";
import {
  getCourses,
  type GetCoursesResult,
} from "@/features/user/services/courseService";
import { CourseContext } from "./CourseContext";
import type { CacheEntry } from "./course.types";
import { PAGE_SIZE } from "./course.constants";

const getFilterKey = (f: FilterValues) =>
  `${f.search || ""}|${f.categoryId || ""}|${f.difficultyLevelId || ""}`;

export function CourseProvider({ children }: { children: ReactNode }) {
  const [courses, setCourses] = useState<Course[]>([]);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [allLoaded, setAllLoaded] = useState(false);
  const [loading, setLoading] = useState(false);
  const [filters, setFiltersState] = useState<FilterValues>({});
  const [initialized, setInitialized] = useState(false);
  const cacheRef = useRef<Record<string, CacheEntry>>({});

  const loadGuardRef = useRef(false);
  const filtersRef = useRef(filters);
  useEffect(() => { filtersRef.current = filters; }, [filters]);

  const loadInitial = useCallback(async (f?: FilterValues) => {
    const fParams = f ?? {};
    const key = getFilterKey(fParams);
    filtersRef.current = fParams;
    setFiltersState(fParams);

    if (cacheRef.current[key]) {
      const cached = cacheRef.current[key];
      setCourses(cached.courses);
      setTotal(cached.total);
      setPage(cached.page);
      setAllLoaded(cached.allLoaded);
      setInitialized(true);
      return;
    }

    setLoading(true);
    setAllLoaded(false);
    try {
      const result: GetCoursesResult = await getCourses({
        page: 1,
        pageSize: PAGE_SIZE,
        search: fParams.search,
        categoryId: fParams.categoryId,
        difficultyLevelId: fParams.difficultyLevelId,
        status: true,
      });
      setCourses(result.courses);
      setTotal(result.total);
      setPage(result.page);
      const isAllLoaded =
        result.courses.length === 0 || result.total <= PAGE_SIZE;
      setAllLoaded(isAllLoaded);
      setInitialized(true);
      cacheRef.current[key] = {
        courses: result.courses,
        total: result.total,
        page: result.page,
        allLoaded: isAllLoaded,
      };
    } catch {
      setAllLoaded(true);
      setInitialized(true);
    } finally {
      setLoading(false);
    }
  }, []);

  const loadMore = useCallback(async () => {
    if (loadGuardRef.current || allLoaded || loading) return;
    loadGuardRef.current = true;
    setLoading(true);
    const nextPage = page + 1;
    const currentFilters = filtersRef.current;
    const key = getFilterKey(currentFilters);
    try {
      const result: GetCoursesResult = await getCourses({
        page: nextPage,
        pageSize: PAGE_SIZE,
        search: currentFilters.search,
        categoryId: currentFilters.categoryId,
        difficultyLevelId: currentFilters.difficultyLevelId,
        status: true,
      });
      const updatedCourses = [...courses, ...result.courses];
      const isAllLoaded =
        result.courses.length === 0 || nextPage * PAGE_SIZE >= result.total;

      setCourses(updatedCourses);
      setPage(result.page);
      setAllLoaded(isAllLoaded);

      cacheRef.current[key] = {
        courses: updatedCourses,
        total: result.total,
        page: result.page,
        allLoaded: isAllLoaded,
      };
    } catch {
      setAllLoaded(true);
    } finally {
      setLoading(false);
      loadGuardRef.current = false;
    }
  }, [allLoaded, loading, page, courses]);

  const setFilters = useCallback(
    (f: FilterValues) => {
      loadInitial(f);
    },
    [loadInitial],
  );

  useEffect(() => { loadInitial(); }, [loadInitial]);

  const value = useMemo(
    () => ({
      courses,
      total,
      page,
      allLoaded,
      loading,
      filters,
      initialized,
      loadInitial,
      loadMore,
      setFilters,
    }),
    [
      courses,
      total,
      page,
      allLoaded,
      loading,
      filters,
      initialized,
      loadInitial,
      loadMore,
      setFilters,
    ],
  );

  return (
    <CourseContext.Provider value={value}>{children}</CourseContext.Provider>
  );
}
