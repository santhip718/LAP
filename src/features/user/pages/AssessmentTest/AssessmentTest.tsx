import { useEffect, useState, useMemo, useCallback } from "react";
import { useParams, useNavigate } from "react-router-dom";
import {
  getAssessmentOverview,
  getAssessmentQuestions,
  submitAssessment,
  type AssessmentOverview,
  type AssessmentQuestion,
} from "../../services/assessmentService";
import { feedbackService } from "@/shared/services/feedback";
import { extractErrorMessage } from "@/shared/utils/apiErrorUtils";
import type { SubmitAssessmentResponseDto } from "@/shared/services/api/models";
import type { Answers, Flagged } from "../../types/assessmentService.types";
import Typography from "@mui/material/Typography";
import LapSpinnerv1 from "@/shared/components/ui/LapSpinnerv1/LapSpinnerv1";
import {
  TIMER_INTERVAL_MS,
  QUESTION_TYPES,
  LEAVE_CONFIRM_CONFIG,
  QUIT_CONFIRM_CONFIG,
  SUBMIT_CONFIRM_CONFIG,
  SUBMIT_LABELS,
  PAGE_TITLE,
  QUIT_LABEL,
  STAT_LABELS,
  STAT_UNITS,
  QUESTION_TYPE_LABELS,
  UNSUPPORTED_QUESTION,
  FLAG_LABELS,
  PLACEHOLDER,
  WEIGHT_LABEL,
  POINT,
  POINTS,
  ERROR_TOAST_FALLBACK,
  TOAST_DURATION_MS,
  ICONS,
  getOverviewRoute,
  getResultRoute,
} from "./AssessmentTest.constants";
import "./AssessmentTest.css";

export default function AssessmentTest() {
  const { courseId } = useParams<{ courseId: string }>();
  const navigate = useNavigate();

  const [assessment, setAssessment] = useState<AssessmentOverview | null>(null);
  const [questions, setQuestions] = useState<AssessmentQuestion[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(false);
  const [answers, setAnswers] = useState<Answers>({});
  const [flagged, setFlagged] = useState<Flagged>({});
  const [scrollProgress, setScrollProgress] = useState(0);
  const [startedOn] = useState(() => new Date().toISOString());
  const [submitting, setSubmitting] = useState(false);
  const [remainingTime, setRemainingTime] = useState<number | null>(null);

  useEffect(() => {
    const handleBeforeUnload = (e: BeforeUnloadEvent) => {
      if (!submitting) {
        e.preventDefault();
        e.returnValue = "";
      }
    };
    globalThis.addEventListener("beforeunload", handleBeforeUnload);
    return () =>
      globalThis.removeEventListener("beforeunload", handleBeforeUnload);
  }, [submitting]);

  useEffect(() => {
    globalThis.history.pushState(null, "", globalThis.location.href);
    const handlePopState = async () => {
      const confirmed = await feedbackService.showConfirm(LEAVE_CONFIRM_CONFIG);
      if (confirmed) {
        globalThis.history.back();
      } else {
        globalThis.history.pushState(null, "", globalThis.location.href);
      }
    };
    globalThis.addEventListener("popstate", handlePopState);
    return () => globalThis.removeEventListener("popstate", handlePopState);
  }, []);

  useEffect(() => {
    if (assessment) {
      setRemainingTime(assessment.durationMinute * 60);
    }
  }, [assessment]);

  const handleSubmit = useCallback(async () => {
    if (!assessment || submitting) return;
    const confirmed = await feedbackService.showConfirm(SUBMIT_CONFIRM_CONFIG);
    if (!confirmed) return;
    setSubmitting(true);
    const formattedAnswers = Object.entries(answers).map(([qId, val]) => ({
      question_id: qId,
      selected_answer: val,
    }));
    let result: SubmitAssessmentResponseDto | null = null;
    try {
      result = await submitAssessment(
        assessment.id,
        formattedAnswers,
        startedOn,
      );
      if (result?.status) {
        feedbackService.showToast(result.status, "success", TOAST_DURATION_MS);
      }
    } catch (err: unknown) {
      feedbackService.showToast(
        extractErrorMessage(err, ERROR_TOAST_FALLBACK),
        "error",
      );
    }
    navigate(getResultRoute(courseId!), {
      state: result,
    });
  }, [assessment, answers, startedOn, courseId, navigate, submitting]);

  const handleQuit = useCallback(async () => {
    const confirmed = await feedbackService.showConfirm(QUIT_CONFIRM_CONFIG);
    if (confirmed) {
      navigate(getOverviewRoute(courseId!));
    }
  }, [courseId, navigate]);

  useEffect(() => {
    if (remainingTime === null || remainingTime <= 0) return;

    const timer = setInterval(() => {
      setRemainingTime((prev) => {
        if (prev === null || prev <= 1) {
          clearInterval(timer);
          if (prev !== null && prev <= 1) {
            handleSubmit();
          }
          return 0;
        }
        return prev - 1;
      });
    }, TIMER_INTERVAL_MS);

    return () => clearInterval(timer);
  }, [remainingTime, handleSubmit]);

  const formatTime = (seconds: number) => {
    const mins = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${String(mins).padStart(2, "0")}:${String(secs).padStart(2, "0")}`;
  };

  useEffect(() => {
    let cancelled = false;
    (async () => {
      if (!courseId) {
        setError(true);
        setLoading(false);
        return;
      }
      try {
        const asm = await getAssessmentOverview(courseId);
        if (cancelled) return;
        if (!asm) {
          setError(true);
          setLoading(false);
          return;
        }
        setAssessment(asm);
        const qs = await getAssessmentQuestions(asm.id);
        if (!cancelled) {
          setQuestions(qs);
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
  }, [courseId]);

  const handleScroll = useCallback(() => {
    const winScroll =
      document.body.scrollTop || document.documentElement.scrollTop;
    const height =
      document.documentElement.scrollHeight -
      document.documentElement.clientHeight;
    const scrolled = height > 0 ? (winScroll / height) * 100 : 0;
    setScrollProgress(scrolled);
  }, []);

  useEffect(() => {
    globalThis.addEventListener("scroll", handleScroll);
    return () => globalThis.removeEventListener("scroll", handleScroll);
  }, [handleScroll]);

  const handleAnswer = useCallback((questionId: string, value: string) => {
    setAnswers((prev) => ({ ...prev, [questionId]: value }));
  }, []);

  const handleFlag = useCallback((questionId: string) => {
    setFlagged((prev) => ({
      ...prev,
      [questionId]: !prev[questionId],
    }));
  }, []);

  const answeredCount = useMemo(() => Object.keys(answers).length, [answers]);

  const flaggedCount = useMemo(
    () => Object.values(flagged).filter(Boolean).length,
    [flagged],
  );

  if (loading) {
    return <LapSpinnerv1 />;
  }

  if (error || !assessment || questions.length === 0) {
    return (
      <div className="at-loading">
        <Typography variant="body2">Assessment not available.</Typography>
      </div>
    );
  }

  return (
    <div className="at-page">
      <header className="at-header">
        <div className="at-header-inner">
          <div className="at-header-left">
            <Typography variant="h6" className="at-header-title">
              {PAGE_TITLE}
            </Typography>
            <span className="at-header-divider" />
            <Typography variant="body2" className="at-header-subtitle">
              {assessment.title}
            </Typography>
          </div>
          <div className="at-header-right">
            <div className="at-progress-note">
              <span className="material-symbols-outlined at-progress-note-icon">
                {ICONS.CORRECT}
              </span>
              <span className="at-progress-note-text">
                {answeredCount}/{questions.length}
              </span>
            </div>
            {remainingTime !== null && (
              <div className="at-digital-clock">
                <span className="at-digital-clock-digits">
                  {formatTime(remainingTime)}
                </span>
              </div>
            )}
            <button className="at-btn-quit" onClick={handleQuit}>
              {QUIT_LABEL}
            </button>
          </div>
        </div>
      </header>

      <div className="at-progress-container">
        <div
          className="at-progress-bar"
          style={{ width: `${scrollProgress}%` }}
        />
      </div>

      <div className="at-content">
        <div className="at-content-inner">
          <section className="at-section" id="at-overview">
            <div className="at-instructions">
              <Typography variant="h3" className="at-instructions-title">
                Description
              </Typography>
              <Typography variant="body1" className="at-instructions-text">
                {assessment.description.charAt(0).toUpperCase() +
                  assessment.description.slice(1)}
              </Typography>
            </div>
            <div className="at-stats-grid">
              <div className="at-stat-card">
                <div className="at-stat-icon-wrapper">
                  <span className="material-symbols-outlined at-stat-icon">
                    {ICONS.HELP}
                  </span>
                </div>
                <div>
                  <Typography variant="body1" className="at-stat-value">
                    {questions.length} {STAT_UNITS.QUESTIONS}
                  </Typography>
                  <Typography variant="body2" className="at-stat-label">
                    {STAT_LABELS.TOTAL}
                  </Typography>
                </div>
              </div>
              <div className="at-stat-card">
                <div className="at-stat-icon-wrapper">
                  <span className="material-symbols-outlined at-stat-icon">
                    {ICONS.GRADE}
                  </span>
                </div>
                <div>
                  <Typography variant="body1" className="at-stat-value">
                    {assessment.totalMark} {STAT_UNITS.POINTS}
                  </Typography>
                  <Typography variant="body2" className="at-stat-label">
                    Passing score:{" "}
                    {Math.round(
                      (assessment.passingMark / assessment.totalMark) * 100,
                    )}
                    %
                  </Typography>
                </div>
              </div>
              <div className="at-stat-card">
                <div className="at-stat-icon-wrapper">
                  <span className="material-symbols-outlined at-stat-icon">
                    {ICONS.HISTORY}
                  </span>
                </div>
                <div>
                  <Typography variant="body1" className="at-stat-value">
                    {assessment.durationMinute} {STAT_UNITS.MINUTES}
                  </Typography>
                  <Typography variant="body2" className="at-stat-label">
                    {STAT_LABELS.TIME_LIMIT}
                  </Typography>
                </div>
              </div>
            </div>
          </section>

          <hr className="at-divider" />

          <section className="at-section" id="at-questions">
            {questions.map((q, index) => (
              <div
                key={q.id}
                className={`at-question-card ${flagged[q.id] ? "at-question-card-flagged" : ""}`}
                id={`q-${q.id}`}
              >
                <div className="at-question-header">
                  <div className="at-question-header-left">
                    <span className="at-qnum">{index + 1}</span>
                    <Typography variant="h6" className="at-question-title">
                      {QUESTION_TYPE_LABELS[q.questionType.name] ??
                        q.questionType.name}
                    </Typography>
                  </div>
                  <button
                    className={`at-flag-btn ${flagged[q.id] ? "at-flag-btn-active" : ""}`}
                    onClick={() => handleFlag(q.id)}
                  >
                    <span className="material-symbols-outlined at-flag-icon">
                      {ICONS.FLAG}
                    </span>
                    <Typography variant="caption" className="at-flag-label">
                      {flagged[q.id]
                        ? FLAG_LABELS.FLAGGED
                        : FLAG_LABELS.FLAG_REVIEW}
                    </Typography>
                  </button>
                </div>

                <Typography variant="body1" className="at-question-text">
                  {q.questionText}
                </Typography>

                {q.questionType.name === QUESTION_TYPES.MCQ ? (
                  <div className="at-options">
                    {q.optionList.map((opt, oi) => (
                      <label
                        key={oi}
                        className={`at-option ${answers[q.id] === opt ? "at-option-selected" : ""}`}
                      >
                        <input
                          type="radio"
                          name={`q-${q.id}`}
                          value={opt}
                          checked={answers[q.id] === opt}
                          onChange={() => handleAnswer(q.id, opt)}
                          className="at-radio"
                        />
                        <Typography variant="body1" className="at-option-text">
                          {opt}
                        </Typography>
                      </label>
                    ))}
                  </div>
                ) : q.questionType.name === QUESTION_TYPES.TrueFalse ? (
                  <div className="at-tf-group">
                    {q.optionList.map((opt, oi) => (
                      <label
                        key={oi}
                        className={`at-tf-option ${answers[q.id] === opt ? "at-tf-selected" : ""}`}
                      >
                        <input
                          type="radio"
                          name={`q-${q.id}`}
                          value={opt}
                          checked={answers[q.id] === opt}
                          onChange={() => handleAnswer(q.id, opt)}
                          className="at-tf-radio"
                        />
                        <span
                          className={`material-symbols-outlined at-tf-icon ${opt === "True" ? "at-tf-icon-true" : "at-tf-icon-false"}`}
                        >
                          {opt === "True" ? "check_circle" : "cancel"}
                        </span>
                        <Typography variant="h6" className="at-tf-label">
                          {opt}
                        </Typography>
                      </label>
                    ))}
                  </div>
                ) : q.questionType.name === QUESTION_TYPES.FillInBlank ? (
                  <div className="at-fib">
                    <input
                      type="text"
                      className="at-fib-input"
                      placeholder={PLACEHOLDER}
                      value={answers[q.id] || ""}
                      onChange={(e) => handleAnswer(q.id, e.target.value)}
                    />
                  </div>
                ) : (
                  <div className="at-unsupported">
                    <Typography variant="body2" className="at-unsupported-text">
                      {UNSUPPORTED_QUESTION}
                    </Typography>
                  </div>
                )}

                <div className="at-question-footer">
                  <Typography variant="caption" className="at-weight">
                    {WEIGHT_LABEL} {q.weight} {q.weight > 1 ? POINTS : POINT}
                  </Typography>
                </div>
              </div>
            ))}
          </section>

          <hr className="at-divider" />

          <section className="at-section" id="at-submit-section">
            <div className="at-submit-bar">
              <div className="at-submit-bar-info">
                <span className="material-symbols-outlined at-submit-bar-icon">
                  {ICONS.ASSIGNMENT}
                </span>
                <Typography variant="body2" className="at-submit-bar-text">
                  {answeredCount} of {questions.length} questions answered
                  {flaggedCount > 0 && ` (${flaggedCount} flagged)`}
                </Typography>
              </div>
              <button
                className="at-btn-submit"
                disabled={submitting}
                onClick={handleSubmit}
              >
                {submitting ? SUBMIT_LABELS.submitting : SUBMIT_LABELS.submit}
              </button>
            </div>
          </section>
        </div>
      </div>
    </div>
  );
}
