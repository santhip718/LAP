export interface EnrolledCourse {
  id: string;
  courseId: string;
  title: string;
  category: string;
  enrolledOn: string;
  completedOn: string | null;
  progress: number;
  status: boolean;
  thumbnail: string;
}

export interface GetEnrollmentsResult {
  courses: EnrolledCourse[];
  total: number;
  page: number;
  pageSize: number;
}
