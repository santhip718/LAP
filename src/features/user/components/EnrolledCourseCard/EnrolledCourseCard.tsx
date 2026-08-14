import { useNavigate } from "react-router-dom";
import Typography from "@mui/material/Typography";
import type { EnrolledCourse } from "../../services/enrollmentService";
import LapTooltip from "@/shared/components/ui/LapTooltip/LapTooltip";
import type { EnrolledCourseCardProps } from "../../types/courseDetailService.types";
import {
  BUTTON_LABELS,
  BADGE_LABELS,
  PROGRESS_SUFFIX,
  ICONS,
  COMPLETED_PREFIX,
  ENTROLLED,
} from "./EnrolledCourseCard.constants";
import "./EnrolledCourseCard.css";

export default function EnrolledCourseCard({
  course,
}: EnrolledCourseCardProps) {
  const navigate = useNavigate();
  const canResume = course.status;

  return (
    <div
      className={`enrolled-card${!canResume ? " enrolled-card-disabled" : ""}`}
      onClick={() => navigate(`/course-overview/${course.courseId}`)}
    >
      <div className="enrolled-card-img-wrap">
        {course.thumbnail ? (
          <img
            className="enrolled-card-img"
            src={course.thumbnail}
            alt={course.title}
            loading="lazy"
          />
        ) : (
          <div className="enrolled-card-img-placeholder">
            <span className="material-symbols-outlined">{ICONS.FALLBACK}</span>
          </div>
        )}
        <span
          className={`enrolled-card-badge ${course.status ? "badge-active" : "badge-inactive"}`}
        >
          {course.status ? BADGE_LABELS.ACTIVE : BADGE_LABELS.INACTIVE}
        </span>
      </div>

      <div className="enrolled-card-body">
        <LapTooltip
          text={course.title}
          variant="h6"
          className="enrolled-card-title"
        />
        {course.category && (
          <Typography variant="caption" className="enrolled-card-cat">
            {course.category}
          </Typography>
        )}

        <div className="enrolled-card-progress">
          <div className="enrolled-card-progress-bar">
            <div
              className="enrolled-card-progress-fill"
              style={{ width: `${course.progress}%` }}
            />
          </div>
          <Typography
            variant="caption"
            className="enrolled-card-progress-label"
          >
            {course.progress}
            {PROGRESS_SUFFIX}
          </Typography>
        </div>

        <div className="enrolled-card-footer">
          <div className="enrolled-card-dates">
            <Typography variant="caption" className="enrolled-card-date">
              {ENTROLLED} {new Date(course.enrolledOn).toLocaleDateString()}
            </Typography>
          </div>
          <button
            className={`enrolled-card-resume${!canResume ? " enrolled-card-resume-disabled" : ""}`}
            disabled={!canResume}
            onClick={(e) => {
              if (!canResume) return;
              e.stopPropagation();
              navigate(`/course-overview/${course.courseId}`);
            }}
          >
            {canResume ? BUTTON_LABELS.resume : BUTTON_LABELS.notStarted}
          </button>
        </div>
      </div>
    </div>
  );
}
