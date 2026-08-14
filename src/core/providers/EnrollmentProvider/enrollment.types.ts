import type { EnrolledCourse } from "@/features/user/services/enrollmentService";

export interface EnrollmentContextValue {
  enrolledCourses: Record<string, EnrolledCourse>;
  loading: boolean;
  enroll: (courseId: string) => Promise<void>;
}

export interface EnrollmentProviderProps {
  children: React.ReactNode;
}
