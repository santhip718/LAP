import Typography from "@mui/material/Typography";
import { useNavigate } from "react-router-dom";
import LapTooltip from "@/shared/components/ui/LapTooltip/LapTooltip";
import type { CourseHeroProps } from "../../types/courseDetailService.types";
import fallbackThumb from "@/assets/images/thumb-fallback.jpg";
import {
  BUTTON_LABELS,
  ICONS,
  STAT_LABELS,
  RATE_BUTTON_LABEL,
} from "./CourseHero.constants";
import "./CourseHero.css";

export default function CourseHero({
  course,
  durationLabel,
  isEnrolled,
  canResume,
  courseId,
  onEnroll,
  onRateClick,
}: CourseHeroProps) {
  const navigate = useNavigate();

  const handleButtonClick = () => {
    if (isEnrolled && canResume) {
      if (course.topics.length > 0 && course.topics[0].contents.length > 0) {
        navigate(`/course-content/${courseId}`);
      }
    } else if (!isEnrolled) {
      if (courseId) {
        onEnroll(courseId);
      }
    }
  };

  const sentenceCase = (text: string) => {
    if (!text) return text;
    return text.charAt(0).toUpperCase() + text.slice(1).toLowerCase();
  };

  return (
    <section className="co-hero">
      <div className="co-hero-media">
        <img
          src={course.thumbnailImgPath || fallbackThumb}
          alt={course.title}
          className="co-hero-img"
          onError={(e) => {
            e.currentTarget.src = fallbackThumb;
          }}
        />
        <div className="co-hero-overlay">
          <span className="material-symbols-outlined co-play-icon">
            {ICONS.PLAY}
          </span>
        </div>
      </div>
      <div className="co-hero-content">
        <LapTooltip
          text={course.title}
          variant="h3"
          className="co-hero-title"
        />
        <LapTooltip
          text={sentenceCase(course.description)}
          variant="body2"
          className="co-hero-desc"
          maxLines={3}
        />
        <div className="co-hero-stats">
          <Typography variant="body2" className="co-hero-stat">
            <span className="material-symbols-outlined">{ICONS.GROUP}</span>
            {course.enrollmentCount.toLocaleString()} {STAT_LABELS.STUDENTS}
          </Typography>
          <Typography variant="body2" className="co-hero-stat">
            <span className="material-symbols-outlined">{ICONS.SCHEDULE}</span>
            {durationLabel} {STAT_LABELS.TOTAL}
          </Typography>
          <Typography variant="body2" className="co-hero-stat">
            <span className="material-symbols-outlined co-star-filled">
              {ICONS.STAR}
            </span>
            {course.overallRating}
            {STAT_LABELS.RATING_SUFFIX}
            <Typography variant="caption" className="co-hero-review-count">
              {STAT_LABELS.REVIEWS}
            </Typography>
          </Typography>
        </div>
        <div className="co-hero-actions">
          <button
            className="co-btn-primary"
            onClick={handleButtonClick}
            disabled={isEnrolled && !canResume}
          >
            {isEnrolled
              ? canResume
                ? BUTTON_LABELS.resume
                : BUTTON_LABELS.requested
              : BUTTON_LABELS.enroll}
          </button>
          <button className="co-btn-outline" onClick={onRateClick}>
            {RATE_BUTTON_LABEL}
          </button>
        </div>
      </div>
    </section>
  );
}
