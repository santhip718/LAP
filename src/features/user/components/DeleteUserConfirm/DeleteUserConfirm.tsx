import { useState } from "react";
import Typography from "@mui/material/Typography";
import LapModalDialog from "../../../../shared/components/feedback/LapModalDialog/LapModalDialog";
import LapButton from "../../../../shared/components/ui/LapButton/LapButton";
import { userService } from "../../services/userService";
import { feedbackService } from "../../../../shared/services/feedback/feedbackService";
import { deleteUserStrings } from "../../utils/constants";
import { userProfileStrings } from "../../pages/UserProfile/constants";
import type { DeleteUserConfirmProps } from "../../types";
import "./DeleteUserConfirm.css";

export default function DeleteUserConfirm({
  open,
  onClose,
  onSuccess,
  userId,
  userName,
}: DeleteUserConfirmProps) {
  const [deleting, setDeleting] = useState(false);

  const handleDelete = async () => {
    if (!userId) return;
    setDeleting(true);
    try {
      await userService.deleteUser(userId);
      feedbackService.showToast(deleteUserStrings.success, "success");
      handleClose();
      onSuccess();
    } catch {
      feedbackService.showToast(deleteUserStrings.error, "error");
    } finally {
      setDeleting(false);
    }
  };

  const handleClose = () => {
    if (!deleting) onClose();
  };

  return (
    <LapModalDialog open={open} onClose={handleClose} title={deleteUserStrings.title} maxWidth="xs">
      <div className="du-content">
        <div className="du-icon-wrap">
          <span className="material-symbols-outlined du-icon">warning</span>
        </div>
        <Typography variant="body2" className="du-message">
          {deleteUserStrings.message}
        </Typography>
        {userName && (
          <Typography variant="body2" className="du-user-name">{userProfileStrings.userLabel}<strong>{userName}</strong></Typography>
        )}
        <div className="du-actions">
          <LapButton
            type="ghost"
            onClick={handleClose}
            disabled={deleting}
          >
            {deleteUserStrings.cancelLabel}
          </LapButton>
          <LapButton
            type="primary"
            onClick={handleDelete}
            loading={deleting}
          >
            {deleting ? userProfileStrings.deleting : deleteUserStrings.confirmLabel}
          </LapButton>
        </div>
      </div>
    </LapModalDialog>
  );
}
