import { useState, useMemo } from "react";
import Typography from "@mui/material/Typography";
import { useParams, useNavigate, useLocation } from "react-router-dom";
import type { SubmitAssessmentResponseDto } from "@/shared/services/api/models";
import { mapAssessmentResult } from "@/features/user/services/assessmentService";
import {
  BACK_BUTTON_LABEL,
  RESULT_NOT_AVAILABLE,
  SUMMARY_LABELS,
  MESSAGE_LABELS,
  STAT_LABELS,
  TOGGLE_LABELS,
  FALLBACKS,
  ICONS,
  SUFFIXES,
  getCourseOverviewRoute,
} from "./AssessmentResult.constants";
import "./AssessmentResult.css";

export default function AssessmentResult() {
  const { courseId } = useParams<{ courseId: string }>();
  const navigate = useNavigate();
  const location = useLocation();
  const stateData = location.state as Record<string, unknown> | null;
  const rawResult =
    stateData && "total_question" in stateData
      ? (stateData as SubmitAssessmentResponseDto)
      : null;
  const [showReview, setShowReview] = useState(false);

  const result = useMemo(() => rawResult ? mapAssessmentResult(rawResult) : null, [rawResult]);

  if (!result) {
    return (
      <div className="ar-loading">
        <Typography variant="body2">{RESULT_NOT_AVAILABLE}</Typography>
        <button
          className="ar-btn-back"
          onClick={() => navigate(getCourseOverviewRoute(courseId!))}
        >
          {BACK_BUTTON_LABEL}
        </button>
      </div>
    );
  }

  const {
    totalQuestion = 0,
    correctAnswer = 0,
    score = 0,
    passed = false,
    completedOn = "",
    durationTakenMinutes = 0,
    weakTopics = [],
    answers = [],
    tierAwarded,
  } = result;

  const accuracy =
    totalQuestion > 0 ? Math.round((correctAnswer / totalQuestion) * 100) : 0;

  return (
    <div className="ar-page">
      <div className="ar-header">
        <button
          className="ar-back-btn"
          onClick={() => navigate(getCourseOverviewRoute(courseId!))}
        >
          <span className="material-symbols-outlined">{ICONS.BACK}</span>
          {BACK_BUTTON_LABEL}
        </button>
      </div>

      <div className="ar-content">
        {/* ── Summary view ── */}
        <div
          className={`ar-summary-card ${passed ? "ar-summary-passed" : "ar-summary-failed"}`}
        >
          <div
            className={`ar-summary-icon-ring ${passed ? "ar-icon-passed" : "ar-icon-failed"}`}
          >
            <span className="material-symbols-outlined ar-summary-icon">
              {passed ? ICONS.PASSED : ICONS.FAILED}
            </span>
          </div>

          <Typography variant="h4" className="ar-summary-title">
            {passed ? SUMMARY_LABELS.PASSED : SUMMARY_LABELS.FAILED}
          </Typography>

          <div className="ar-summary-score">
            <Typography variant="h2" className="ar-score-value">
              {score}
            </Typography>
            <Typography variant="h5" className="ar-score-sep">
              /
            </Typography>
            <Typography variant="h5" className="ar-score-total">
              {totalQuestion}
            </Typography>
          </div>

          <Typography variant="body1" className="ar-summary-text">
            {MESSAGE_LABELS.SUMMARY_PREFIX}
            {correctAnswer}
            {MESSAGE_LABELS.SUMMARY_INFIX}
            {totalQuestion}
            {MESSAGE_LABELS.SUMMARY_SUFFIX}
          </Typography>

          <div className="ar-stat-grid">
            <div className="ar-stat-pill">
              <span className="material-symbols-outlined">
                {ICONS.ACCURACY}
              </span>
              <div>
                <Typography variant="caption" className="ar-stat-label">
                  {STAT_LABELS.ACCURACY}
                </Typography>
                <Typography variant="body2">
                  {accuracy}
                  {SUFFIXES.PERCENT}
                </Typography>
              </div>
            </div>
            <div className="ar-stat-pill">
              <span className="material-symbols-outlined">{ICONS.TIME}</span>
              <div>
                <Typography variant="caption" className="ar-stat-label">
                  {STAT_LABELS.TIME_TAKEN}
                </Typography>
                <Typography variant="body2">
                  {durationTakenMinutes} {SUFFIXES.MINUTES}
                </Typography>
              </div>
            </div>
            {tierAwarded && (
              <div className="ar-stat-pill">
                <span className="material-symbols-outlined">{ICONS.TIER}</span>
                <div>
                  <Typography variant="caption" className="ar-stat-label">
                    {STAT_LABELS.TIER}
                  </Typography>
                  <Typography variant="body2">{tierAwarded}</Typography>
                </div>
              </div>
            )}
          </div>

          {completedOn && (
            <Typography variant="caption" className="ar-summary-date">
              {MESSAGE_LABELS.DATE_PREFIX}
              {new Date(completedOn).toLocaleDateString()}
            </Typography>
          )}
        </div>

        {weakTopics.length > 0 && (
          <section className="ar-section">
            <Typography variant="h6" className="ar-section-title">
              {STAT_LABELS.AREAS_TO_IMPROVE}
            </Typography>
            <div className="ar-weak-list">
              {weakTopics.map((topic, i) => (
                <div key={i} className="ar-weak-item">
                  <div className="ar-weak-item-left">
                    <span className="material-symbols-outlined ar-weak-icon">
                      {ICONS.WEAK}
                    </span>
                    <Typography variant="body2" className="ar-weak-name">
                      {topic.topicName ?? FALLBACKS.TOPIC_NAME}
                    </Typography>
                  </div>
                  <span className="ar-weak-score-badge">
                    {Math.round(topic.averageScore ?? 0)}%
                  </span>
                </div>
              ))}
            </div>
          </section>
        )}

        {/* ── Toggle button ── */}
        {answers.length > 0 && (
          <button
            className="ar-review-toggle"
            onClick={() => setShowReview((p) => !p)}
            aria-expanded={showReview}
          >
            <span className="material-symbols-outlined">
              {showReview ? ICONS.HIDE_REVIEW : ICONS.SHOW_REVIEW}
            </span>
            {showReview ? TOGGLE_LABELS.HIDE : TOGGLE_LABELS.SHOW}
            <span
              className={`material-symbols-outlined ar-review-chevron ${showReview ? "ar-review-chevron-up" : ""}`}
            >
              {ICONS.CHEVRON}
            </span>
          </button>
        )}

        {/* ── Answer review (revealed on demand) ── */}
        <div
          className={`ar-review-panel ${showReview ? "ar-review-panel-open" : ""}`}
        >
          <div className="ar-review-panel-inner">
            <section className="ar-section">
              <Typography variant="h6" className="ar-section-title">
                {STAT_LABELS.ANSWER_REVIEW}
              </Typography>
              <div className="ar-answers-list">
                {answers.map((answer, index) => (
                  <div
                    key={answer.questionId ?? index}
                    className={`ar-answer-card ${answer.isCorrect ? "ar-answer-correct" : "ar-answer-wrong"}`}
                  >
                    <div className="ar-answer-header">
                      <span className="ar-answer-num">
                        {MESSAGE_LABELS.QUESTION_PREFIX}
                        {index + 1}
                      </span>
                      <span
                        className={`material-symbols-outlined ar-answer-icon ${answer.isCorrect ? "ar-answer-icon-correct" : "ar-answer-icon-wrong"}`}
                      >
                        {answer.isCorrect ? ICONS.CORRECT : ICONS.INCORRECT}
                      </span>
                    </div>
                    <Typography variant="body1" className="ar-answer-text">
                      {answer.questionText ?? FALLBACKS.QUESTION_TEXT}
                    </Typography>
                    <div className="ar-answer-details">
                      <div className="ar-answer-row">
                        <Typography variant="body2" className="ar-answer-label">
                          {STAT_LABELS.YOUR_ANSWER}
                        </Typography>
                        <Typography
                          variant="body2"
                          className={`ar-answer-value ${answer.isCorrect ? "ar-value-correct" : "ar-value-wrong"}`}
                        >
                          {answer.selectedAnswer ?? FALLBACKS.NO_ANSWER}
                        </Typography>
                      </div>
                      <div className="ar-answer-row">
                        <Typography variant="body2" className="ar-answer-label">
                          {STAT_LABELS.SCORE}
                        </Typography>
                        <Typography variant="body2" className="ar-answer-value">
                          {answer.obtainedScore ?? 0} {SUFFIXES.POINTS}
                        </Typography>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            </section>
          </div>
        </div>

        <div className="ar-actions">
          <button
            className="ar-btn-primary"
            onClick={() => navigate(getCourseOverviewRoute(courseId!))}
          >
            {BACK_BUTTON_LABEL}
          </button>
        </div>
      </div>
    </div>
  );
}
