import { useState, useCallback, useEffect, useMemo, useRef } from "react";
import { useForm, useWatch, useFieldArray } from "react-hook-form";
import { courseService, getDefaultCourseFormValues, buildCreateCoursePayload } from "../../services/courseService";
import { useReferenceData } from "../../hooks/useReferenceData";
import type { CreateCourseForm } from "../../types/courseFormTypes";
import type { CourseEditData, CreateCourseModalProps } from "../../types";
import BasicInfoSection from "../BasicInfoSection/BasicInfoSection";
import ContentSection from "../ContentSection/ContentSection";
import FormActions from "../FormActions/FormActions";
import LapModalDialog from "../../../../shared/components/feedback/LapModalDialog/LapModalDialog";
import { feedbackService } from "../../../../shared/services/feedback/feedbackService";
import { createCourseModalStrings, contentSectionStrings } from "../../pages/CourseManagement/CourseManagement.constants";
import "./CreateCourseModal.css";

export default function CreateCourseModal({
  open,
  onClose,
  onSuccess,
  editCourse,
}: CreateCourseModalProps) {
  const [serverError, setServerError] = useState("");
  const [submitting, setSubmitting] = useState(false);
  const [submittingAction, setSubmittingAction] = useState<"draft" | "publish" | null>(null);
  const deletedContentIdsRef = useRef<string[]>([]);
  const { categories, subcategories, difficultyLevels, contentTypes, loading, error: refDataError } =
    useReferenceData();

  const isEditMode = !!editCourse;

  const {
    register,
    handleSubmit,
    watch,
    getValues,
    control,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<CreateCourseForm>({
    defaultValues: getDefaultCourseFormValues(editCourse, contentTypes),
  });

  const { fields, append, remove } = useFieldArray({
    control,
    name: 'topics',
  });

  useEffect(() => {
    if (open && !loading) {
      reset(getDefaultCourseFormValues(editCourse, contentTypes));
      deletedContentIdsRef.current = [];
      setServerError("");
    }
  }, [open, editCourse, reset, loading, contentTypes]);

  const addTopic = useCallback(() => {
    append({
      name: "",
      contentTitle: "",
      metaTopicOrder: 1,
      metaTopicDuration: 10,
      sequenceOrder: 1,
      contentTypeId: "",
      videoUrl: "",
      contentFile: undefined,
    });
  }, [append]);

  const removeTopic = useCallback(async (index: number) => {
    const confirmed = await feedbackService.showConfirm({
      title: contentSectionStrings.confirmRemoveTitle,
      message: contentSectionStrings.confirmRemoveMessage,
    });
    if (!confirmed) return;
    const topic = getValues(`topics.${index}`);
    if (topic?.contentId) {
      deletedContentIdsRef.current = [...deletedContentIdsRef.current, topic.contentId!];
    }
    remove(index);
  }, [getValues, remove]);

  const thumbnailFile = useWatch({ control, name: "thumbnailFile" });

  const selectedThumbnailFile = useMemo(
    () =>
      thumbnailFile instanceof FileList && thumbnailFile.length > 0
        ? thumbnailFile[0]
        : thumbnailFile instanceof File
          ? thumbnailFile
          : undefined,
    [thumbnailFile],
  );

  const thumbnailPreview = useMemo(
    () =>
      selectedThumbnailFile
        ? URL.createObjectURL(selectedThumbnailFile)
        : editCourse?.thumbnailUrl || null,
    [selectedThumbnailFile, editCourse?.thumbnailUrl],
  );

  useEffect(
    () => () => {
      if (thumbnailPreview && selectedThumbnailFile) {
        URL.revokeObjectURL(thumbnailPreview);
      }
    },
    [thumbnailPreview, selectedThumbnailFile],
  );

  const handleClose = useCallback(() => {
    deletedContentIdsRef.current = [];
    setServerError("");
    setSubmitting(false);
    setSubmittingAction(null);
    onClose();
  }, [onClose]);

  const saveCourse = useCallback(async (data: CreateCourseForm, isDrafted: boolean) => {
    setSubmitting(true);
    setSubmittingAction(isDrafted ? "draft" : "publish");
    setServerError("");
    try {
      const payload = buildCreateCoursePayload(data, isDrafted, data.topics.length, deletedContentIdsRef.current);
      if (isEditMode && editCourse) {
        await courseService.updateCourse(editCourse.id, payload);
      } else {
        await courseService.createCourse(payload);
      }
      handleClose();
      if (onSuccess) onSuccess();
      feedbackService.showToast(
        isEditMode ? createCourseModalStrings.success.updated : createCourseModalStrings.success.draftSaved,
        "success",
      );
    } catch (err: unknown) {
      setSubmitting(false);
      setSubmittingAction(null);
      const msg =
        (err as { response?: { data?: { message?: string } } })?.response?.data?.message ||
        (err as { response?: { data?: string } })?.response?.data ||
        (err as Error)?.message ||
        createCourseModalStrings.error.unexpectedError;
      console.error("Failed to save course:", err, "Server response:", msg);
      setServerError(typeof msg === "string" ? msg : createCourseModalStrings.error.saveFailed);
    }
  }, [isEditMode, editCourse, handleClose, onSuccess]);

  const onSubmitDraft = handleSubmit((data) => saveCourse(data, true));
  const onSubmitPublish = handleSubmit((data) => saveCourse(data, false));

  return (
    <LapModalDialog
      open={open}
      onClose={handleClose}
      title={isEditMode ? createCourseModalStrings.editTitle : createCourseModalStrings.title}
      maxWidth="md"
    >
      {loading ? (
        <div className="cc-loading">
          {createCourseModalStrings.loadingReferenceData}
        </div>
      ) : (
        <div className="cc-modal-content">
          {refDataError && <div className="cc-server-error">{refDataError}</div>}

          <form className="cc-form-container" noValidate>
            <BasicInfoSection
              register={register}
              errors={errors}
              watch={watch}
              categories={categories}
              subcategories={subcategories}
              difficultyLevels={difficultyLevels}
              thumbnailPreview={thumbnailPreview}
            />

            <ContentSection
              register={register}
              errors={errors}
              watch={watch}
              contentTypes={contentTypes}
              fields={fields}
              onAddTopic={addTopic}
              onRemoveTopic={removeTopic}
            />

            {serverError && <div className="cc-server-error">{serverError}</div>}

            <FormActions
              isSubmitting={submitting || isSubmitting}
              submittingAction={submittingAction}
              isEditMode={isEditMode}
              isDrafted={editCourse?.isDrafted}
              onSubmitDraft={onSubmitDraft}
              onSubmitPublish={onSubmitPublish}
              onCancel={handleClose}
            />
          </form>
        </div>
      )}
    </LapModalDialog>
  );
}
