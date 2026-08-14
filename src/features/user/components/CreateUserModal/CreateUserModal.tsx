import LapModalDialog from "../../../../shared/components/feedback/LapModalDialog/LapModalDialog";
import RegisterForm from "../../../auth/components/RegisterForm";
import { userProfileStrings } from "../../pages/UserProfile/constants";
import type { CreateUserModalProps } from "../../types";

export default function CreateUserModal({ open, onClose, onSuccess }: CreateUserModalProps) {
  return (
    <LapModalDialog open={open} onClose={onClose} title={userProfileStrings.createUserTitle} maxWidth="sm">
      <RegisterForm
        mode="create"
        onSuccess={onSuccess}
        onClose={onClose}
      />
    </LapModalDialog>
  );
}
