import type { UseFormRegister, FieldErrors, UseFormWatch, UseFieldArrayReturn } from "react-hook-form";
import type { RefTerm } from "../../../shared/services/referenceDataService";
import type { CreateCourseForm } from "./courseFormTypes";
import type { CourseEditData } from "./courseServiceTypes";

export interface BasicInfoSectionProps {
  register: UseFormRegister<CreateCourseForm>;
  errors: FieldErrors<CreateCourseForm>;
  watch: UseFormWatch<CreateCourseForm>;
  categories: RefTerm[];
  subcategories: RefTerm[];
  difficultyLevels: RefTerm[];
  thumbnailPreview: string | null;
}

export interface CreateCourseModalProps {
  open: boolean;
  onClose: () => void;
  onSuccess?: () => void;
  editCourse?: CourseEditData | null;
}

export interface ContentSectionProps {
  register: UseFormRegister<CreateCourseForm>;
  errors: FieldErrors<CreateCourseForm>;
  watch: UseFormWatch<CreateCourseForm>;
  contentTypes: RefTerm[];
  fields: UseFieldArrayReturn<CreateCourseForm, "topics", "id">["fields"];
  onAddTopic: () => void;
  onRemoveTopic: (index: number) => void;
}

export interface FormActionsProps {
  isSubmitting: boolean;
  submittingAction?: "draft" | "publish" | null;
  isDisabled?: boolean;
  isEditMode?: boolean;
  isDrafted?: boolean;
  onSubmitDraft: () => void;
  onSubmitPublish: () => void;
  onCancel: () => void;
}

export interface CourseDiscussionProps {
  courseId: string;
}
