import "./ProfileCard.css";
import { PROFILE_CARD } from "./ProfileCard.constants";

export interface LapProfileCardProps {
  name: string;
  title: string;
  avatarUrl: string;
  isVerified?: boolean;
  followerCount: number;
  postCount: number;
  onFollow?: () => void;
}

export default function LapProfileCard({
  name,
  title,
  avatarUrl,
  isVerified = false,
  followerCount,
  postCount,
  onFollow,
}: LapProfileCardProps) {
  return (
    <div className="profilecard">
      <img className="profilecard-avatar" src={avatarUrl} alt={name} loading="lazy" />
      <div className="profilecard-body">
        <div className="profilecard-name-row">
          <h3 className="profilecard-name">{name}</h3>
          {isVerified && (
            <svg className="profilecard-verified" viewBox="0 0 24 24" width="18" height="18" fill="none">
              <circle cx="12" cy="12" r="12" fill={PROFILE_CARD.VERIFIED_COLOR} />
              <path d="M9 12.5l2 2 4-5" stroke={PROFILE_CARD.CHECK_COLOR} strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round" />
            </svg>
          )}
        </div>
        <p className="profilecard-title">{title}</p>
        <div className="profilecard-footer">
          <div className="profilecard-stats">
            <span className="profilecard-stat">
              <span className="material-symbols-outlined profilecard-stat-icon">group</span>
              <span className="profilecard-stat-number">{followerCount}</span>
            </span>
            <span className="profilecard-stat">
              <span className="material-symbols-outlined profilecard-stat-icon">check_box</span>
              <span className="profilecard-stat-number">{postCount}</span>
            </span>
          </div>
          <button className="profilecard-follow" onClick={onFollow}>
            {PROFILE_CARD.FOLLOW_LABEL}
          </button>
        </div>
      </div>
    </div>
  );
}
