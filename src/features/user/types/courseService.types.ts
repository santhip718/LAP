import type { EnrolledCourse } from "./enrollmentService.types";

export interface Course {
  id: string;
  title: string;
  category: string;
  categoryId: string;
  duration: string;
  level: string;
  rating: string;
  image: string;
  alt: string;
  isBestseller?: boolean;
}

export interface FilterValues {
  search?: string;
  categoryId?: string;
  difficultyLevelId?: string;
}

export interface GetCoursesResult {
  courses: Course[];
  total: number;
  page: number;
  pageSize: number;
}

export interface CourseCardProps {
  course: Course;
  enrollment?: EnrolledCourse;
  onEnroll?: (courseId: string) => void;
}

export interface FilterBarProps {
  onFilterChange: (filters: FilterValues) => void;
}
