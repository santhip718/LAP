import { useState } from "react";
import { useNavigate } from "react-router-dom";
import Typography from "@mui/material/Typography";
import LapTooltip from "@/shared/components/ui/LapTooltip/LapTooltip";
import type { CourseCardProps } from "../../types/courseService.types";
import { BUTTON_LABELS, ICONS, BESTSELLER_LABEL } from "./CourseCard.constants";
import "./CourseCard.css";

export default function CourseCard({
  course,
  enrollment,
  onEnroll,
}: CourseCardProps) {
  const navigate = useNavigate();
  const isEnrolled = enrollment !== undefined;
  const canResume = isEnrolled && enrollment.status === true;
  const [imgError, setImgError] = useState(false);
  const showFallback = !course.image || imgError;

  const handleClick = () => {
    if (canResume) {
      navigate(`/course-overview/${course.id}`);
    } else if (!isEnrolled && onEnroll) {
      onEnroll(course.id);
    }
  };

  return (
    <div
      className="coursecard"
      onClick={() => navigate(`/course-overview/${course.id}`)}
    >
      <div className="coursecard-img">
        {showFallback ? (
          <div className="coursecard-img-fallback">
            <span className="material-symbols-outlined">{ICONS.FALLBACK}</span>
          </div>
        ) : (
          <img
            src={course.image}
            alt={course.alt}
            loading="lazy"
            onError={() => setImgError(true)}
          />
        )}
        {course.isBestseller && (
          <span className="coursecard-badge">{BESTSELLER_LABEL}</span>
        )}
      </div>
      <div className="coursecard-body">
        <LapTooltip
          text={course.title}
          variant="h6"
          className="coursecard-title"
        />
        <Typography variant="body2" className="coursecard-subtitle">
          {course.category}
        </Typography>
        <div className="coursecard-footer">
          <div className="coursecard-stats">
            <Typography
              variant="caption"
              component="span"
              className="coursecard-stat"
            >
              <span className="material-symbols-outlined">
                {ICONS.SCHEDULE}
              </span>
              {course.duration}
            </Typography>
            <Typography
              variant="caption"
              component="span"
              className="coursecard-stat"
            >
              <span className="material-symbols-outlined">{ICONS.STAR}</span>
              {course.rating}
            </Typography>
          </div>
          <button
            className={`coursecard-pill ${canResume ? "coursecard-pill-resume" : ""} ${isEnrolled && !canResume ? "coursecard-pill-enrolled" : ""}`}
            disabled={isEnrolled && !canResume}
            onClick={(e) => {
              e.stopPropagation();
              handleClick();
            }}
          >
            {canResume
              ? BUTTON_LABELS.resume
              : isEnrolled
                ? BUTTON_LABELS.requested
                : BUTTON_LABELS.enroll}
          </button>
        </div>
      </div>
    </div>
  );
}
