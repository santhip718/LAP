import { useUserProfile } from "../../hooks/useUserProfile";
import ProfileImageUpload from "../../components/ProfileImageUpload/ProfileImageUpload";
import LapSpinnerv1 from "../../../../shared/components/ui/LapSpinnerv1/LapSpinnerv1";
import LapNoContent from "../../../../shared/components/ui/LapNoContent/LapNoContent";
import LapButton from "../../../../shared/components/ui/LapButton/LapButton";
import { userProfileStrings } from "./constants";
import { FALLBACK_EMPTY } from "../../utils/constants";
import Typography from "@mui/material/Typography";
import "./UserProfile.css";

export default function UserProfile() {
  const { profile, loading, error, refresh, uploadImage, uploading } = useUserProfile();

  if (loading) {
    return (
      <div className="up-page">
        <main className="up-main">
          <LapSpinnerv1 />
        </main>
      </div>
    );
  }

  if (error || !profile) {
    return (
      <div className="up-page">
        <main className="up-main">
          <LapNoContent
            icon="error"
            title={userProfileStrings.errorTitle}
            message={error || userProfileStrings.error}
          >
            <LapButton type="outline" onClick={refresh}>
              {userProfileStrings.retry}
            </LapButton>
          </LapNoContent>
        </main>
      </div>
    );
  }

  return (
    <div className="up-page">
      <main className="up-main">
        <Typography variant="h3" className="up-title">{userProfileStrings.title}</Typography>

        <div className="up-card">
          <ProfileImageUpload
            currentImage={profile.profileImage}
            onUpload={uploadImage}
            uploading={uploading}
          />
        </div>

        <div className="up-card">
          <Typography variant="h5" className="up-card-title">
            <span className="material-symbols-outlined">person</span>
            {userProfileStrings.sections.personalInfo}
          </Typography>
          <div className="up-fields">
            <div className="up-field">
              <span className="up-label">{userProfileStrings.labels.fullName}</span>
              <Typography variant="body2" className="up-value">{profile.fullName}</Typography>
            </div>
            <div className="up-field">
              <span className="up-label">{userProfileStrings.labels.email}</span>
              <Typography variant="body2" className="up-value">{profile.email}</Typography>
            </div>
            <div className="up-field">
              <span className="up-label">{userProfileStrings.labels.mobileNumber}</span>
              <Typography variant="body2" className="up-value">{profile.mobileNumber || FALLBACK_EMPTY}</Typography>
            </div>
          </div>
        </div>

        <div className="up-card">
          <Typography variant="h5" className="up-card-title">
            <span className="material-symbols-outlined">badge</span>
            {userProfileStrings.sections.workDetails}
          </Typography>
          <div className="up-fields">
            <div className="up-field">
              <span className="up-label">{userProfileStrings.labels.designation}</span>
              <Typography variant="body2" className="up-value">{profile.designation}</Typography>
            </div>
            <div className="up-field">
              <span className="up-label">{userProfileStrings.labels.gender}</span>
              <Typography variant="body2" className="up-value">{profile.gender}</Typography>
            </div>
            <div className="up-field">
              <span className="up-label">{userProfileStrings.labels.currentTier}</span>
              <Typography variant="body2" className="up-value">{profile.currentTier}</Typography>
            </div>
            <div className="up-field">
              <span className="up-label">{userProfileStrings.labels.roles}</span>
              <div className="up-roles">
                {profile.roles.length > 0 ? (
                  profile.roles.map((role) => (
                    <span key={role} className="up-role-badge">{role}</span>
                  ))
                ) : (
                  <Typography variant="body2" className="up-value">{FALLBACK_EMPTY}</Typography>
                )}
              </div>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
}
