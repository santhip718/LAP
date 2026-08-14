import LapModalDialog from "../../../../shared/components/feedback/LapModalDialog/LapModalDialog";
import RegisterForm from "../../../auth/components/RegisterForm";
import { editUserStrings } from "../../utils/constants";
import type { EditUserModalProps } from "../../types";

export default function EditUserModal({ open, onClose, onSuccess, user }: EditUserModalProps) {
  if (!user) return null;

  return (
    <LapModalDialog open={open} onClose={onClose} title={editUserStrings.title} maxWidth="sm">
      <RegisterForm
        mode="edit"
        initialData={{
          id: user.id,
          fullName: user.fullName,
          email: user.email,
          mobileNumber: user.mobileNumber,
          designationId: user.designationId,
          genderId: user.genderId,
          roles: user.roles,
        }}
        onSuccess={onSuccess}
        onClose={onClose}
      />
    </LapModalDialog>
  );
}
