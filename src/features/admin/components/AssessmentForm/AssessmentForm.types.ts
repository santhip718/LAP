import type { AssessmentOverviewDto } from "@/shared/services/api/models/assessmentOverviewDto";

export interface AssessmentFormProps {
  courseId: string;
  onSuccess: () => void;
  onCancel: () => void;
  initialData?: AssessmentOverviewDto;
}

export interface FormData {
  title: string;
  description: string;
  passingMark: number;
  durationMinute: number;
}
