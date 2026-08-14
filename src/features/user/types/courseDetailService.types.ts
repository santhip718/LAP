import type { LapTopic, LapContent } from "@/shared/types/ui.types";
import type { EnrolledCourse } from "./enrollmentService.types";

export interface CourseDetail {
  id: string;
  title: string;
  category: { id: string; name: string };
  difficultyLevel: { id: string; name: string };
  durationMinute: number;
  overallRating: number;
  thumbnailImgPath: string;
  status: boolean;
  description: string;
  createdByUser: {
    id: string;
    fullName: string;
    email: string;
    roles: string[];
  };
  topics: LapTopic[];
  enrollmentCount: number;
  assessmentTitle: string;
  totalMark: number;
  passingMark: number;
}

export interface CourseCanvasProps {
  content: LapContent | null;
}

export interface CourseHeroProps {
  course: CourseDetail;
  durationLabel: string;
  isEnrolled: boolean;
  canResume: boolean;
  courseId: string | undefined;
  onEnroll: (id: string) => void;
  onRateClick: () => void;
}

export interface EnrolledCourseCardProps {
  course: EnrolledCourse;
}
