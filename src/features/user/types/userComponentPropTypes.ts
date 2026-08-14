import type { UserDetail } from "./userServiceTypes";

export interface DeleteUserConfirmProps {
  open: boolean;
  onClose: () => void;
  onSuccess: () => void;
  userId: string | null;
  userName: string;
}

export interface EditUserModalProps {
  open: boolean;
  onClose: () => void;
  onSuccess: () => void;
  user: UserDetail | null;
}

export interface ViewUserModalProps {
  open: boolean;
  onClose: () => void;
  user: UserDetail | null;
}

export interface CreateUserModalProps {
  open: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

export interface ProfileImageUploadProps {
  currentImage: string | null;
  onUpload: (file: File) => Promise<string>;
  uploading: boolean;
}

