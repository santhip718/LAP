import type { ButtonHTMLAttributes, InputHTMLAttributes, ReactNode } from "react";
import type { TypographyProps } from "@mui/material/Typography";
import type { CourseDetail } from "@/features/user/services/courseDetailService";
import type { AssessmentOverview } from "@/features/user/services/assessmentService";
import type { ReviewDto } from "@/shared/services/api/models";

export type LapButtonType =
  | "primary"
  | "secondary"
  | "outline"
  | "ghost"
  | "logout"
  | "register"
  | "home"
  | "nav";

export interface LapButtonProps
  extends Omit<ButtonHTMLAttributes<HTMLButtonElement>, "type"> {
  type?: LapButtonType;
  htmlType?: "button" | "submit" | "reset";
  loading?: boolean;
  icon?: ReactNode;
  children?: ReactNode;
  fullWidth?: boolean;
}

export type LapInputType = "default" | "search" | "ghost";

export interface LapInputProps
  extends Omit<InputHTMLAttributes<HTMLInputElement>, "className" | "type"> {
  type?: LapInputType;
  htmlType?: InputHTMLAttributes<HTMLInputElement>["type"];
  label?: string;
  error?: string;
  rightElement?: ReactNode;
}

export type SortDirection = "asc" | "desc";

export interface LapColumn<T> {
  key: string;
  label: string;
  sortable?: boolean;
  render?: (value: unknown, row: T, index: number) => React.ReactNode;
  className?: string;
  thClassName?: string;
}

export interface LapDataTableProps<T> {
  columns: LapColumn<T>[];
  data: T[];
  pageSize?: number;
  pageSizeOptions?: number[];
  onRowClick?: (row: T) => void;
}

export interface LapContent {
  id: string;
  title: string;
  contentType: {
    id: string;
    name: "Video" | "Pdf";
  };
  videoUrl?: string;
  pdfFilePath?: string;
  pdfBase64?: string;
  durationMinute: number;
  sequenceOrder: number;
  isCompleted?: boolean;
}

export interface LapTopic {
  id: string;
  name: string;
  sequenceOrder: number;
  metaSequenceOrder: number;
  durationMinute: number;
  contents: LapContent[];
  isCompleted?: boolean;
}

export interface LapCurriculumAccordionProps {
  topics: LapTopic[];
  defaultExpanded?: string[];
  onContentClick?: (content: LapContent) => void;
  showCompletion?: boolean;
  disabled?: boolean;
}

export interface LapAssessmentCardProps {
  assessment: AssessmentOverview | null;
  canAccessAssessment: boolean;
  canResume?: boolean;
  completionPercent: number;
  courseId: string;
  attemptsUsed?: number;
  maxAttempts?: number;
}

export interface LapAddButtonProps {
  onClick?: () => void;
  label?: string;
  icon?: string;
}

export interface LapSidebarProps {
  course: any;
  onToggleCollapse: () => void;
  isCollapsed: boolean;
  isMobileOpen: boolean;
  onMobileClose: () => void;
  children?: ReactNode;
}

export interface LapNoContentProps {
  icon?: string;
  title: string;
  message: string;
  children?: ReactNode;
  className?: string;
}

export interface LapReviewCardProps {
  review: ReviewDto;
  isOwn?: boolean;
  onEdit?: (review: ReviewDto) => void;
  onDelete?: (id: string) => void;
}

export interface LapTooltipProps extends TypographyProps {
  text: string;
  maxLines?: number;
}

export interface LapThemeToggleProps {
  className?: string;
}
