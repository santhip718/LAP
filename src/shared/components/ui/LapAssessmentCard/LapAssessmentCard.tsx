import { useNavigate } from "react-router-dom";
import Typography from "@mui/material/Typography";
import type { LapAssessmentCardProps } from "@/shared/types/ui.types";
import {
  BUTTON_LABELS,
  PROGRESS_MARKER_LABEL,
  EMPTY_ICON,
  TITLE_LABELS,
  META_TEMPLATES,
} from "./LapAssessmentCard.constants";
import "./LapAssessmentCard.css";

export default function LapAssessmentCard({
  assessment,
  canAccessAssessment,
  canResume = false,
  completionPercent,
  courseId,
  attemptsUsed = 0,
  maxAttempts = 3,
}: LapAssessmentCardProps) {
  const navigate = useNavigate();
  const maxReached = maxAttempts > 0 && attemptsUsed >= maxAttempts;
  const canAttempt = canAccessAssessment && !maxReached;

  if (!assessment) {
    if (!canResume) return null;
    return (
      <div className="co-assessment-section co-assessment-empty">
        <div className="co-assessment-header">
          <div className="co-assessment-header-left">
            <span className="material-symbols-outlined co-assessment-icon co-assessment-icon-empty">
              {EMPTY_ICON}
            </span>
            <div>
              <Typography variant="h6" className="co-assessment-title">
                {TITLE_LABELS.ASSESSMENT}
              </Typography>
              <Typography variant="body2" className="co-assessment-meta">
                {TITLE_LABELS.NO_ASSESSMENT}
              </Typography>
            </div>
          </div>
        </div>
      </div>
    );
  }

  const getButtonLabel = () => {
    if (maxReached) return BUTTON_LABELS.MAX_ATTEMPTS_REACHED;
    if (!canResume) return BUTTON_LABELS.ENROLL_TO_ACCESS;
    if (canAccessAssessment) return BUTTON_LABELS.BEGIN_ASSESSMENT;
    return META_TEMPLATES.COMPLETION_PCT(completionPercent);
  };

  return (
    <div className="co-assessment-section">
      <div className="co-assessment-header">
        <div className="co-assessment-header-left">
          <span className="material-symbols-outlined co-assessment-icon">
            {EMPTY_ICON}
          </span>
          <div>
            <Typography variant="h6" className="co-assessment-title">
              {assessment.title}
            </Typography>
            <Typography variant="body2" className="co-assessment-meta">
              {assessment.totalMark}
              {META_TEMPLATES.POINTS_SUFFIX}
              {META_TEMPLATES.POINTS_SEPARATOR}
              {assessment.durationMinute}
              {META_TEMPLATES.MIN_SUFFIX}
            </Typography>
          </div>
        </div>
        <button
          className={`co-assessment-btn ${!canAttempt ? "co-assessment-btn-disabled" : ""}`}
          disabled={!canAttempt}
          onClick={() =>
            canAttempt && navigate(`/course-overview/${courseId}/assessment`)
          }
        >
          {getButtonLabel()}
        </button>
      </div>
      {canResume && !canAccessAssessment && !maxReached && (
        <div className="co-assessment-progress">
          <div className="co-assessment-progress-header">
            <Typography
              variant="caption"
              className="co-assessment-progress-label"
            >
              {TITLE_LABELS.COURSE_PROGRESS}
            </Typography>
            <Typography variant="body2" className="co-assessment-progress-pct">
              {completionPercent}%
            </Typography>
          </div>
          <div className="co-assessment-progress-track">
            <div
              className="co-assessment-progress-fill"
              style={{ width: `${Math.min(completionPercent, 75)}%` }}
            />
            <div
              className="co-assessment-progress-marker"
              style={{ left: "75%" }}
            >
              <Typography
                variant="caption"
                className="co-assessment-progress-marker-label"
              >
                {PROGRESS_MARKER_LABEL}
              </Typography>
            </div>
          </div>
          <Typography variant="caption" className="co-assessment-locked-msg">
            {100 - completionPercent}
            {META_TEMPLATES.PROGRESS_UNLOCK}
          </Typography>
        </div>
      )}
      {canAccessAssessment && !maxReached && (
        <div className="co-assessment-progress">
          <div className="co-assessment-progress-header">
            <Typography
              variant="caption"
              className="co-assessment-progress-label"
            >
              {TITLE_LABELS.COURSE_PROGRESS}
            </Typography>
            <Typography
              variant="body2"
              className="co-assessment-progress-pct co-assessment-progress-pct-done"
            >
              {completionPercent}%
            </Typography>
          </div>
          <div className="co-assessment-progress-track">
            <div
              className="co-assessment-progress-fill co-assessment-progress-fill-done"
              style={{ width: "100%" }}
            />
          </div>
          <Typography variant="caption" className="co-assessment-unlocked-msg">
            {TITLE_LABELS.ASSESSMENT_UNLOCKED}
          </Typography>
        </div>
      )}
      {maxReached && attemptsUsed > 0 && (
        <div className="co-assessment-progress">
          <Typography variant="caption" className="co-assessment-locked-msg">
            {META_TEMPLATES.ATTEMPTS_USED(attemptsUsed, maxAttempts)}
          </Typography>
        </div>
      )}
    </div>
  );
}
