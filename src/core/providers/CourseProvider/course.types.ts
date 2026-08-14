import type { Course, FilterValues } from "@/features/user/types/courseService.types";

export interface CourseContextValue {
  courses: Course[];
  total: number;
  page: number;
  allLoaded: boolean;
  loading: boolean;
  filters: FilterValues;
  initialized: boolean;
  loadInitial: (f?: FilterValues) => Promise<void>;
  loadMore: () => Promise<void>;
  setFilters: (f: FilterValues) => void;
}

export interface CourseProviderProps {
  children: React.ReactNode;
}

export interface CacheEntry {
  courses: Course[];
  total: number;
  page: number;
  allLoaded: boolean;
}
