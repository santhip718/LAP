import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import {
  getAssessmentOverview,
  type AssessmentOverview,
} from "../../services/assessmentService";
import { getCourseProgress } from "@/features/user/services/courseDetailService";
import { useEnrollment } from "@/core/providers/EnrollmentProvider";
import Typography from "@mui/material/Typography";
import {
  UNLOCK_THRESHOLD,
  LOADING_LABELS,
  SECTION_LABELS,
  RULES,
  STAT_LABELS,
  MISC,
  ICONS,
  getTestRoute,
} from "./AssessmentOverview.constants";
import "./AssessmentOverview.css";

export default function AssessmentOverviewPage() {
  const { courseId } = useParams<{ courseId: string }>();
  const navigate = useNavigate();
  const [assessment, setAssessment] = useState<AssessmentOverview | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(false);
  const [completionPercent, setCompletionPercent] = useState(0);
  const [agreed, setAgreed] = useState(false);
  const { enrolledCourses } = useEnrollment();
  const enrollment = courseId ? enrolledCourses[courseId] : undefined;
  const isEnrolled = enrollment !== undefined;
  const canResume = isEnrolled && enrollment?.status === true;

  useEffect(() => {
    let cancelled = false;
    (async () => {
      if (!courseId) {
        setError(true);
        setLoading(false);
        return;
      }
      try {
        const data = await getAssessmentOverview(courseId);
        if (cancelled) return;
        if (!data) {
          setError(true);
        } else {
          setAssessment(data);
        }

        if (canResume) {
          try {
            const pct = await getCourseProgress(courseId);
            if (cancelled) return;
            setCompletionPercent(pct);
          } catch {
            console.error("Failed to load progress");
          }
        }
      } catch {
        if (!cancelled) setError(true);
      } finally {
        if (!cancelled) setLoading(false);
      }
    })();
    return () => {
      cancelled = true;
    };
  }, [courseId, canResume]);

  if (loading) {
    return (
      <div className="ao-loading">
        <Typography variant="body2">{LOADING_LABELS.LOADING}</Typography>
      </div>
    );
  }

  if (error || !assessment) {
    return (
      <div className="ao-loading">
        <Typography variant="body2">{LOADING_LABELS.NOT_FOUND}</Typography>
      </div>
    );
  }

  const canAccess = canResume && completionPercent >= UNLOCK_THRESHOLD;

  const handleBegin = () => {
    if (canAccess && agreed) {
      navigate(getTestRoute(courseId!));
    }
  };

  const passingPct =
    assessment.totalMark > 0
      ? Math.round((assessment.passingMark / assessment.totalMark) * 100)
      : 0;

  const difficultyName = assessment.course?.difficultyLevel?.name ?? null;

  return (
    <div className="ao-page">
      <div className="ao-card">
        <div className="ao-header text-center border-b border-gray-50">
          <div className="ao-header-icon">
            <span className="material-symbols-outlined text-4xl">
              {ICONS.DIFFICULTY}
            </span>
          </div>
          <Typography variant="h3" className="ao-title">
            {assessment.title.charAt(0).toUpperCase() +
              assessment.title.slice(1)}
          </Typography>
          <Typography variant="body1" className="ao-subtitle">
            {MISC.SUBTITLE}
          </Typography>
        </div>

        <div className="ao-body">
          <div className="ao-body-left">
            <Typography variant="h5" className="ao-section-title">
              <span className="material-symbols-outlined text-primary">
                {ICONS.RULES}
              </span>
              {SECTION_LABELS.RULES_TITLE}
            </Typography>
            <ul className="ao-rules">
              {RULES.map((rule, i) => (
                <li key={i} className="ao-rule">
                  <div className="ao-rule-icon">
                    <span className="material-symbols-outlined text-sm">
                      {rule.icon}
                    </span>
                  </div>
                  <div>
                    <Typography variant="body2" className="ao-rule-title">
                      {rule.label}
                    </Typography>
                    <Typography variant="body2" className="ao-rule-desc">
                      {rule.description}
                    </Typography>
                  </div>
                </li>
              ))}
            </ul>
          </div>

          <div className="ao-body-right">
            <Typography variant="h5" className="ao-section-title">
              {SECTION_LABELS.STATS_TITLE}
            </Typography>
            <div className="ao-stats">
              <div className="ao-stat">
                <div className="ao-stat-left">
                  <span className="material-symbols-outlined text-orange-500">
                    {ICONS.TRENDING_UP}
                  </span>
                  <Typography variant="body2" className="ao-stat-label">
                    {MISC.DIFFICULTY}
                  </Typography>
                </div>
                <Typography variant="body1" className="ao-stat-value">
                  {difficultyName ?? MISC.FALLBACK_DIFFICULTY}
                </Typography>
              </div>
              {STAT_LABELS.map((stat, i) => (
                <div key={i} className="ao-stat">
                  <div className="ao-stat-left">
                    <span className="material-symbols-outlined">
                      {stat.icon}
                    </span>
                    <Typography variant="body2" className="ao-stat-label">
                      {stat.label}
                    </Typography>
                  </div>
                  <Typography variant="body1" className="ao-stat-value">
                    {i === 1
                      ? `${assessment.durationMinute} ${stat.suffix}`
                      : i === 2
                        ? `${assessment.passingMark}`
                        : i === 3
                          ? `${passingPct}`
                          : assessment.totalMark}
                  </Typography>
                </div>
              ))}
            </div>

            {!canResume && (
              <div className="ao-info-box ao-info-box-error">
                <span className="material-symbols-outlined">{ICONS.LOCK}</span>
                <Typography variant="body2">
                  {MISC.ENROLLMENT_REQUIRED}
                </Typography>
              </div>
            )}

            {canResume && completionPercent < UNLOCK_THRESHOLD && (
              <div className="ao-info-box">
                <span className="material-symbols-outlined">{ICONS.INFO}</span>
                <div>
                  <Typography variant="body2">
                    {MISC.COMPLETION_MSG_PREFIX}
                    <Typography component="strong" variant="body2">
                      {completionPercent}%
                    </Typography>{" "}
                    {MISC.COMPLETION_MSG_SUFFIX}
                    {UNLOCK_THRESHOLD}
                    {MISC.COMPLETION_MSG_UNLOCK}
                  </Typography>
                  <div className="ao-mini-progress">
                    <div
                      className="ao-mini-progress-fill"
                      style={{
                        width: `${Math.min(completionPercent, UNLOCK_THRESHOLD)}%`,
                      }}
                    />
                  </div>
                </div>
              </div>
            )}
          </div>
        </div>

        <div className="ao-footer">
          <label className="ao-agree-label">
            <input
              type="checkbox"
              className="ao-checkbox"
              checked={agreed}
              onChange={(e) => setAgreed(e.target.checked)}
            />
            <Typography variant="body2">{MISC.CHECKBOX_LABEL}</Typography>
          </label>

          <button
            className={`ao-btn-primary ${!canAccess || !agreed ? "ao-btn-disabled" : ""}`}
            onClick={handleBegin}
            disabled={!canAccess || !agreed}
          >
            {!canResume
              ? MISC.BUTTON_ENROLL
              : completionPercent < UNLOCK_THRESHOLD
                ? `${completionPercent}${MISC.BUTTON_COMPLETED_SUFFIX}`
                : MISC.BUTTON_BEGIN}
            <span className="material-symbols-outlined">{ICONS.PLAY}</span>
          </button>

          <button className="ao-btn-back" onClick={() => navigate(-1)}>
            {MISC.BUTTON_BACK}
          </button>
        </div>
      </div>
    </div>
  );
}
