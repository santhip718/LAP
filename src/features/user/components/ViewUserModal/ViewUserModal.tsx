import Typography from "@mui/material/Typography";
import LapModalDialog from "../../../../shared/components/feedback/LapModalDialog/LapModalDialog";
import { viewUserStrings, FALLBACK_EMPTY, AVATAR_COLORS } from "../../utils/constants";
import type { UserDetail, ViewUserModalProps } from "../../types";
import "./ViewUserModal.css";

const formatDate = (dateStr: string) => {
  if (!dateStr) return FALLBACK_EMPTY;
  try {
    return new Date(dateStr).toLocaleDateString("en-US", {
      year: "numeric",
      month: "short",
      day: "numeric",
    });
  } catch {
    return dateStr;
  }
};

const getInitials = (name: string) => {
  return name
    .split(" ")
    .map((p) => p.charAt(0))
    .join("")
    .toUpperCase()
    .slice(0, 2);
};

const getAvatarColor = (name: string) => {
  let hash = 0;
  for (let i = 0; i < name.length; i++) {
    hash = name.charCodeAt(i) + ((hash << 5) - hash);
  }
  return AVATAR_COLORS[Math.abs(hash) % AVATAR_COLORS.length];
};

export default function ViewUserModal({ open, onClose, user }: ViewUserModalProps) {
  if (!user) return null;

  return (
    <LapModalDialog open={open} onClose={onClose} title={viewUserStrings.title} maxWidth="sm">
      <div className="vu-content">
        <div className="vu-header">
          {user.profileImage ? (
            <img src={user.profileImage} alt={user.fullName} className="vu-avatar" />
          ) : (
            <div
              className="vu-avatar vu-avatar-initials"
              style={{ backgroundColor: getAvatarColor(user.fullName) }}
            >
              {getInitials(user.fullName)}
            </div>
          )}
          <div>
            <Typography variant="h5" className="vu-name">{user.fullName}</Typography>
            <Typography variant="body2" className="vu-email">{user.email}</Typography>
          </div>
        </div>

        <div className="vu-fields">
          <div className="vu-field">
            <span className="vu-label">{viewUserStrings.labels.mobileNumber}</span>
            <Typography variant="body2" className="vu-value">{user.mobileNumber || FALLBACK_EMPTY}</Typography>
          </div>
          <div className="vu-field">
            <span className="vu-label">{viewUserStrings.labels.designation}</span>
            <Typography variant="body2" className="vu-value">{user.designation}</Typography>
          </div>
          <div className="vu-field">
            <span className="vu-label">{viewUserStrings.labels.gender}</span>
            <Typography variant="body2" className="vu-value">{user.gender}</Typography>
          </div>
          <div className="vu-field">
            <span className="vu-label">{viewUserStrings.labels.currentTier}</span>
            <Typography variant="body2" className="vu-value">{user.currentTier}</Typography>
          </div>
          <div className="vu-field">
            <span className="vu-label">{viewUserStrings.labels.roles}</span>
            <div className="vu-roles">
              {user.roles.length > 0 ? (
                user.roles.map((role) => (
                  <span key={role} className="vu-role-badge">{role}</span>
                ))
              ) : (
                <Typography variant="body2" className="vu-value">{FALLBACK_EMPTY}</Typography>
              )}
            </div>
          </div>
          <div className="vu-field">
            <span className="vu-label">{viewUserStrings.labels.dateCreated}</span>
            <Typography variant="body2" className="vu-value">{formatDate(user.dateCreated)}</Typography>
          </div>
        </div>
      </div>
    </LapModalDialog>
  );
}
