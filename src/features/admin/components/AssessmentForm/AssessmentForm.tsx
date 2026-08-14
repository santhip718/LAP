import { useState, useRef, useEffect } from "react";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import { useForm } from "react-hook-form";
import LapInput from "@/shared/components/ui/LapInput/LapInput";
import LapButton from "@/shared/components/ui/LapButton/LapButton";
import { feedbackService } from "@/shared/services/feedback/feedbackService";
import { getAssessment } from "@/shared/services/api/services/assessment/assessment";
import { getCourse } from "@/shared/services/api/services/course/course";
import apiClient from "@/shared/services/api/apiClient";
import type { PostApiV1AssessmentBody } from "@/shared/services/api/models";
import type { UpdateAssessmentRequestDto } from "@/shared/services/api/models";
import type { CourseSummaryDto } from "@/shared/services/api/models/courseSummaryDto";
import { ASSESSMENT_FORM as T, ALLOWED_EXTENSIONS, MAX_FILE_SIZE } from "./AssessmentForm.constants";
import type { AssessmentFormProps, FormData } from "./AssessmentForm.types";
import { exportAssessmentTemplate } from "@/features/admin/services/adminService";
import "./AssessmentForm.css";

// ── Component ─────────────────────────────────────────────────────────────────

export default function AssessmentForm({
  courseId,
  onSuccess,
  onCancel,
  initialData,
}: AssessmentFormProps) {
  const [courses, setCourses] = useState<CourseSummaryDto[]>([]);
  const [selectedCourseId, setSelectedCourseId] = useState(courseId);
  const [coursesLoading, setCoursesLoading] = useState(false);
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [fileError, setFileError] = useState<string | null>(null);
  const [isConverting, setIsConverting] = useState(false);
  const fileInputRef = useRef<HTMLInputElement | null>(null);
  const [isDragOver, setIsDragOver] = useState(false);

  const [templateLoading, setTemplateLoading] = useState(false);

  const handleDownloadTemplate = async () => {
    setTemplateLoading(true);
    try {
      await exportAssessmentTemplate();
      feedbackService.showToast(T.TOAST_TEMPLATE_DOWNLOADED, "success");
    } catch {
      feedbackService.showToast(T.TOAST_TEMPLATE_ERROR, "error");
    } finally {
      setTemplateLoading(false);
    }
  };

  const isEdit = !!initialData;

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<FormData>({
    defaultValues: {
      title: "",
      description: "",
      passingMark: undefined,
      durationMinute: undefined,
    },
  });

  useEffect(() => {
    if (initialData) {
      reset({
        title: initialData.title ?? "",
        description: initialData.description ?? "",
        passingMark: initialData.passing_mark ?? undefined,
        durationMinute: initialData.duration_minute ?? undefined,
      });
    }
  }, [initialData, reset]);

  useEffect(() => {
    setSelectedCourseId(courseId);
  }, [courseId]);

  useEffect(() => {
    if (isEdit || courseId) return;
    // eslint-disable-next-line react-hooks/set-state-in-effect
    setCoursesLoading(true);
    const api = getCourse(apiClient);
    api.getApiV1Course()
      .then((res) => {
        const raw = res.data;
        const list = Array.isArray(raw) ? raw : ((raw as Record<string, unknown>)?.data as CourseSummaryDto[] ?? []);
        setCourses(list);
      })
      .catch(() => {})
      .finally(() => setCoursesLoading(false));
  }, [isEdit, courseId]);

  const validateSelectedFile = (file: File | null): string | null => {
    if (isEdit && !file) return null;
    if (!file) return T.VALIDATION_FILE_REQUIRED;
    const ext = "." + file.name.split(".").pop()?.toLowerCase();
    if (!(ALLOWED_EXTENSIONS as readonly string[]).includes(ext)) {
      return T.VALIDATION_FILE_TYPE;
    }
    if (file.size > MAX_FILE_SIZE) {
      return T.VALIDATION_FILE_SIZE;
    }
    return null;
  };

  const setFile = (file: File | null) => {
    setSelectedFile(file);
    setFileError(validateSelectedFile(file));
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFile(e.target.files?.[0] ?? null);
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    setIsDragOver(false);
    const file = e.dataTransfer.files?.[0] ?? null;
    setFile(file);
  };

  const handleDragOver = (e: React.DragEvent) => {
    e.preventDefault();
    setIsDragOver(true);
  };

  const handleDragLeave = () => {
    setIsDragOver(false);
  };

  const removeFile = () => {
    setSelectedFile(null);
    if (!isEdit) setFileError(T.VALIDATION_FILE_REQUIRED);
    else setFileError(null);
    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
  };

  const onFormSubmit = async (data: FormData) => {
    const fileErr = validateSelectedFile(selectedFile);
    if (fileErr) {
      setFileError(fileErr);
      return;
    }

    setIsConverting(true);
    try {
      if (isEdit && initialData?.id) {
        const payload: UpdateAssessmentRequestDto = {
          title: data.title || null,
          description: data.description || null,
          ...(initialData.total_mark != null && initialData.total_mark > 0
            ? { total_mark: initialData.total_mark }
            : {}),
          passing_mark: data.passingMark,
          duration_minute: data.durationMinute,
        };

        const api = getAssessment(apiClient);
        await api.putApiV1AssessmentId(initialData.id, payload);

        feedbackService.showToast(T.TOAST_UPDATED, "success");
      } else {
        const payload: PostApiV1AssessmentBody = {
          CourseId: selectedCourseId || courseId || undefined,
          Title: data.title,
          Description: data.description,
          PassingMark: data.passingMark,
          DurationMinute: data.durationMinute,
          QuestionFile: selectedFile ?? undefined,
        };

        const api = getAssessment(apiClient);
        await api.postApiV1Assessment(payload);

        feedbackService.showToast(T.TOAST_CREATED, "success");
      }

      onSuccess();
    } catch (err: unknown) {
      const message =
        err instanceof Error ? err.message : T.TOAST_ERROR;
      feedbackService.showToast(message, "error");
    } finally {
      setIsConverting(false);
    }
  };

  const isBusy = isSubmitting || isConverting;

  return (
    <Box
      component="form"
      className="assessment-form"
      onSubmit={handleSubmit(onFormSubmit)}
      noValidate
      id="assessment-form"
    >
      {/* Title */}
      <div className="assessment-form-field">
        <LapInput
          label={`${T.LABEL_TITLE} *`}
          placeholder={T.PLACEHOLDER_TITLE}
          error={errors.title?.message}
          {...register("title", { required: T.VALIDATION_TITLE_REQUIRED })}
        />
      </div>

      {/* Description */}
      <div className="assessment-form-field">
        <Typography variant="caption" component="label" className="assessment-form-label">
          {T.LABEL_DESCRIPTION} <Typography variant="caption" component="span" sx={{ color: "var(--error)" }}>*</Typography>
        </Typography>
        <textarea
          className={`assessment-form-textarea${errors.description ? " assessment-form-textarea--error" : ""}`}
          placeholder={T.PLACEHOLDER_DESCRIPTION}
          rows={3}
          {...register("description", { required: T.VALIDATION_DESCRIPTION_REQUIRED })}
        />
        {errors.description && (
          <Typography variant="caption" className="assessment-form-error">
            <span className="material-symbols-outlined" style={{ fontSize: "0.875rem" }}>error</span>
            {errors.description.message}
          </Typography>
        )}
      </div>

      {/* Course selector (create mode only) */}
      {!isEdit && !courseId && (
        <div className="assessment-form-field">
          <Typography variant="caption" component="label" className="assessment-form-label">
            {T.LABEL_COURSE} <Typography variant="caption" component="span" sx={{ color: "var(--error)" }}>*</Typography>
          </Typography>
          <select
            className="assessment-form-input"
            value={selectedCourseId}
            onChange={(e) => setSelectedCourseId(e.target.value)}
            disabled={coursesLoading}
          >
            <option value="">
              {coursesLoading ? T.COURSES_LOADING : T.COURSE_PLACEHOLDER}
            </option>
            {courses.map((c) => (
              <option key={c.id} value={c.id}>
                {c.title ?? "Untitled"}
              </option>
            ))}
          </select>
        </div>
      )}

      {/* Two-column: Passing Mark + Duration */}
      <div className="assessment-form-row">
        <div className="assessment-form-field">
          <LapInput
            label={`${T.LABEL_PASSING_MARK} *`}
            htmlType="number"
            placeholder={T.PLACEHOLDER_PASSING_MARK}
            error={errors.passingMark?.message}
            {...register("passingMark", {
              required: T.VALIDATION_PASSING_MARK_REQUIRED,
              min: { value: 0, message: T.VALIDATION_PASSING_MARK_MIN },
              valueAsNumber: true,
            })}
          />
        </div>

        <div className="assessment-form-field">
          <div className="assessment-form-number-wrapper">
            <LapInput
              label={`${T.LABEL_DURATION} *`}
              htmlType="number"
              placeholder={T.PLACEHOLDER_DURATION}
              error={errors.durationMinute?.message}
              rightElement={<Typography variant="caption" className="assessment-form-number-suffix">{T.DURATION_SUFFIX}</Typography>}
              {...register("durationMinute", {
                required: T.VALIDATION_DURATION_REQUIRED,
                min: { value: 1, message: T.VALIDATION_DURATION_MIN },
                valueAsNumber: true,
              })}
            />
          </div>
        </div>
      </div>

      {/* Template Download (create only) */}
      {!isEdit && (
        <div className="assessment-form-field">
          <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
            <Typography variant="caption" className="assessment-form-label" sx={{ mb: 0 }}>
              {T.LABEL_DOWNLOAD_TEMPLATE}
            </Typography>
            <LapButton
              type="outline"
              loading={templateLoading}
              onClick={handleDownloadTemplate}
              style={{ fontSize: "0.75rem", padding: "0.25rem 0.625rem", minHeight: "unset", lineHeight: "1.25rem", fontWeight: 600 }}
            >
              {T.BTN_DOWNLOAD_TEMPLATE}
            </LapButton>
          </Box>
        </div>
      )}

      {/* File Upload (create only) */}
      {!isEdit && (
        <div className="assessment-form-field">
          <Typography variant="caption" component="label" className="assessment-form-label">
            {T.LABEL_QUESTION_FILE}{" "}
            <Typography variant="caption" component="span" sx={{ color: "var(--error)" }}>*</Typography>
          </Typography>
          {!selectedFile ? (
            <div
              className={`assessment-form-file-dropzone${isDragOver ? " assessment-form-file-dropzone--active" : ""}`}
              onDrop={handleDrop}
              onDragOver={handleDragOver}
              onDragLeave={handleDragLeave}
            >
              <input
                ref={fileInputRef}
                type="file"
                accept=".xlsx,.xls"
                onChange={handleFileChange}
                className="assessment-form-file-hidden"
              />
              <div className="assessment-form-file-dropzone-icon">
                <span className="material-symbols-outlined">cloud_upload</span>
              </div>
              <Typography variant="body1" className="assessment-form-file-dropzone-text">{T.FILE_DROP_TEXT}</Typography>
              <Typography variant="body2" className="assessment-form-file-dropzone-hint">
                {T.FILE_BROWSE_HINT} <span>{T.FILE_BROWSE_LINK}</span>
              </Typography>
              <Typography variant="caption" className="assessment-form-file-dropzone-format">{T.FILE_FORMAT_HINT}</Typography>
            </div>
          ) : (
            <div className="assessment-form-file-preview">
              <div className="assessment-form-file-preview-left">
                <div className="assessment-form-file-preview-icon">
                  <span className="material-symbols-outlined">description</span>
                </div>
                <div>
                  <Typography variant="body1" className="assessment-form-file-preview-name">{selectedFile.name}</Typography>
                  <Typography variant="caption" className="assessment-form-file-preview-status">{T.FILE_UPLOADED_STATUS}</Typography>
                </div>
              </div>
              <LapButton type="ghost" icon={<span className="material-symbols-outlined">delete</span>} onClick={removeFile} />
            </div>
          )}
          {fileError && (
            <Typography variant="caption" className="assessment-form-error">
              <span className="material-symbols-outlined" style={{ fontSize: "0.875rem" }}>error</span>
              {fileError}
            </Typography>
          )}
        </div>
      )}

      {/* Footer Actions */}
      <div className="assessment-form-footer">
        <LapButton type="outline" onClick={onCancel} disabled={isBusy}>
          {T.BTN_CANCEL}
        </LapButton>
        <LapButton type="primary" htmlType="submit" disabled={isBusy} loading={isBusy}>
          {isBusy ? T.BTN_PROCESSING : isEdit ? T.BTN_UPDATE : T.BTN_CREATE}
        </LapButton>
      </div>
    </Box>
  );
}
