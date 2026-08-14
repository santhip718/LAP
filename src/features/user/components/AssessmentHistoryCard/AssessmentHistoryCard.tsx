import { memo } from "react";
import Typography from "@mui/material/Typography";
import type { AssessmentHistoryCardProps } from "../../types/assessmentService.types";
import {
  FALLBACK_DATE,
  FALLBACK_TITLE,
  TIME_FORMAT_OPTIONS,
  ICON_NAME,
  STATUS_LABELS,
  STAT_ICONS,
} from "./AssessmentHistoryCard.constants";
import "./AssessmentHistoryCard.css";

const formatDate = (dateStr?: string) => {
  if (!dateStr) return FALLBACK_DATE;
  return new Date(dateStr).toLocaleDateString();
};

const formatTime = (dateStr?: string) => {
  if (!dateStr) return "";
  return new Date(dateStr).toLocaleTimeString([], {
    ...TIME_FORMAT_OPTIONS,
  });
};

function AssessmentHistoryCard({ item, onClick }: AssessmentHistoryCardProps) {
  const passed = item.passed;

  return (
    <div
      className="ah-card"
      onClick={() => onClick?.(item.course_id, item.assessment_id)}
    >
      <div className="ah-card-body">
        <div className="ah-card-top">
          <div className="ah-card-icon">
            <span className="material-symbols-outlined">{ICON_NAME}</span>
          </div>
          <span
            className={`ah-card-status ${passed ? "ah-status-passed" : "ah-status-failed"}`}
          >
            {passed ? STATUS_LABELS.PASSED : STATUS_LABELS.FAILED}
          </span>
        </div>
        <div className="ah-card-header">
          <Typography variant="h6" className="ah-card-title">
            {item.assessment_title ?? FALLBACK_TITLE}
          </Typography>
        </div>
        {item.course_title && (
          <Typography variant="body2" className="ah-card-course">
            {item.course_title}
          </Typography>
        )}
        <div className="ah-card-stats">
          <div className="ah-card-stat">
            <span className="material-symbols-outlined">
              {STAT_ICONS.TROPHY}
            </span>
            <div className="ah-card-stat-value">{item.score ?? 0}</div>
          </div>
          <div className="ah-card-stat">
            <span className="material-symbols-outlined">
              {STAT_ICONS.CALENDAR}
            </span>
            <div className="ah-card-stat-value">
              {formatDate(item.attempted_on)}
            </div>
          </div>
          <div className="ah-card-stat">
            <span className="material-symbols-outlined">
              {STAT_ICONS.SCHEDULE}
            </span>
            <div className="ah-card-stat-value">
              {formatTime(item.attempted_on)}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default memo(AssessmentHistoryCard);
