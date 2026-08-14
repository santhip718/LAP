import LapButton from "../../../../shared/components/ui/LapButton/LapButton";
import { formActionsStrings } from "../../pages/CourseManagement/CourseManagement.constants";
import type { FormActionsProps } from "../../types";

export default function FormActions({
  isSubmitting,
  submittingAction = null,
  isDisabled = false,
  isEditMode = false,
  isDrafted = true,
  onSubmitDraft,
  onSubmitPublish,
  onCancel,
}: FormActionsProps) {
  const showDraftButton = isEditMode ? isDrafted : true;
  const disabled = isSubmitting || isDisabled;
  return (
    <div className="cc-actions">
      <LapButton
        type="ghost"
        onClick={onCancel}
        icon={<span className="material-symbols-outlined">{formActionsStrings.cancelIcon}</span>}
      >
        {formActionsStrings.cancel}
      </LapButton>

      <div className="cc-actions-right">
        {showDraftButton && (
          <LapButton
            type="outline"
            disabled={disabled}
            loading={isSubmitting && submittingAction === "draft"}
            onClick={onSubmitDraft}
          >
            {isEditMode ? formActionsStrings.updateDraft : formActionsStrings.saveAsDraft}
          </LapButton>
        )}
        <LapButton
          type="primary"
          disabled={disabled}
          loading={isSubmitting && submittingAction === "publish"}
          onClick={onSubmitPublish}
          icon={isSubmitting && submittingAction === "publish" ? undefined : <span className="material-symbols-outlined cc-btn-icon">{formActionsStrings.publishIcon}</span>}
        >
          {isEditMode ? formActionsStrings.updateAndPublish : formActionsStrings.publishCourse}
        </LapButton>
      </div>
    </div>
  );
}
