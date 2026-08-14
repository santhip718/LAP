export interface EnrollmentItem {
  id: string;
  userId: string;
  courseId: string;
  courseTitle: string;
  userFullName: string;
  category: string;
  enrollmentStatus: boolean;
  enrolledOn: string;
}

export interface EnrollmentListResult {
  enrollments: EnrollmentItem[];
  total: number;
  page: number;
  pageSize: number;
}

export interface EnrollmentFilters {
  courseName?: string;
  categoryId?: string;
}

export interface UseEnrollmentsResult {
  enrollments: EnrollmentItem[];
  total: number;
  loading: boolean;
  error: string | null;
  refreshing: boolean;
  refresh: () => void;
  setFilters: (filters: EnrollmentFilters) => void;
  filters: EnrollmentFilters;
}
