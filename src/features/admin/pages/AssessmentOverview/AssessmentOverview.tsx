import { useState, useEffect, useCallback, useRef } from "react";
import { useParams, useNavigate } from "react-router-dom";
import Typography from "@mui/material/Typography";
import LapModalDialog from "@/shared/components/feedback/LapModalDialog/LapModalDialog";
import LapSpinnerv1 from "@/shared/components/ui/LapSpinnerv1/LapSpinnerv1";
import LapInput from "@/shared/components/ui/LapInput/LapInput";
import LapButton from "@/shared/components/ui/LapButton/LapButton";
import AssessmentForm from "@/features/admin/components/AssessmentForm/AssessmentForm";
import {
  getAssessments,
  getAssessmentQuestions,
  deleteQuestion as deleteQuestionService,
  updateQuestion as updateQuestionService,
} from "@/features/admin/services/adminService";
import { feedbackService } from "@/shared/services/feedback/feedbackService";
import type { AssessmentOverviewDto } from "@/shared/services/api/models/assessmentOverviewDto";
import type { QuestionDto } from "@/shared/services/api/models/questionDto";
import type { UpdateQuestionRequestDto } from "@/shared/services/api/models";
import { ASSESSMENT_OVERVIEW as T } from "./AssessmentOverview.constants";
import type { QuestionType } from "@/features/admin/components/QuestionCard/quiz.constants";
import { capitalizeFirst } from "@/shared/utils/stringUtils";
import "./AssessmentOverview.css";

function getQuestionType(q: QuestionDto): QuestionType {
  const name = q.question_type?.name?.toLowerCase() ?? "";
  if (name.includes("true") || name.includes("false"))
    return "true-false" as QuestionType;
  if (name.includes("fill") || name.includes("short") || name.includes("blank"))
    return "short-answer" as QuestionType;
  return "multiple-choice" as QuestionType;
}

// ── Component ─────────────────────────────────────────────────────────────────

export default function AssessmentOverview() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [assessment, setAssessment] = useState<AssessmentOverviewDto | null>(
    null,
  );
  const [questions, setQuestions] = useState<QuestionDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [editModalOpen, setEditModalOpen] = useState(false);
  const [editQuestion, setEditQuestion] = useState<QuestionDto | null>(null);
  const [questionEditText, setQuestionEditText] = useState("");
  const [questionEditOptions, setQuestionEditOptions] = useState("");
  const [questionEditWeight, setQuestionEditWeight] = useState(1);
  const [questionEditAnswer, setQuestionEditAnswer] = useState("");
  const [isSavingQuestion, setIsSavingQuestion] = useState(false);

  const [openMenuId, setOpenMenuId] = useState<string | null>(null);
  const menuRef = useRef<HTMLDivElement | null>(null);

  const openQuestionEdit = (q: QuestionDto) => {
    setEditQuestion(q);
    setQuestionEditText(q.question_text ?? "");
    setQuestionEditOptions((q.option_list ?? []).join(", "));
    setQuestionEditWeight(q.weight ?? 1);
    setQuestionEditAnswer(q.answer ?? "");
  };

  const deleteQuestion = async (questionId: string) => {
    const confirmed = await feedbackService.showConfirm({
      title: T.CONFIRM_DELETE_TITLE,
      message: T.CONFIRM_DELETE_MESSAGE,
      confirmLabel: T.CONFIRM_DELETE_CONFIRM,
      cancelLabel: T.CONFIRM_DELETE_CANCEL,
    });
    if (!confirmed) return;
    try {
      await deleteQuestionService(questionId);
      feedbackService.showToast(T.TOAST_QUESTION_DELETED, "success");
      fetchData();
    } catch (err: unknown) {
      const message =
        err instanceof Error ? err.message : T.TOAST_QUESTION_DELETE_FAIL;
      feedbackService.showToast(message, "error");
    }
  };

  const saveQuestion = async () => {
    if (!editQuestion?.id) return;
    setIsSavingQuestion(true);
    try {
      const payload: UpdateQuestionRequestDto = {
        question_text: questionEditText || null,
        option_list: questionEditOptions
          .split(",")
          .map((s) => s.trim())
          .filter(Boolean),
        weight: questionEditWeight || null,
        answer: questionEditAnswer || null,
        question_type_id:
          editQuestion.question_type?.id ??
          ((editQuestion as Record<string, unknown>).question_type_id as
            | string
            | undefined),
        meta_topic_id:
          editQuestion.meta_topic_id ??
          ((editQuestion as Record<string, unknown>).meta_topic_id as
            | string
            | undefined),
      };
      await updateQuestionService(editQuestion.id, payload);
      feedbackService.showToast(T.TOAST_QUESTION_UPDATED, "success");
      setEditQuestion(null);
      fetchData();
    } catch (err: unknown) {
      const message =
        err instanceof Error ? err.message : T.TOAST_QUESTION_UPDATE_FAIL;
      feedbackService.showToast(message, "error");
    } finally {
      setIsSavingQuestion(false);
    }
  };

  const fetchData = useCallback(async () => {
    if (!id) return;
    setIsLoading(true);
    setError(null);

    try {
      const [assessmentsData, questionsData] = await Promise.all([
        getAssessments(),
        getAssessmentQuestions(id),
      ]);

      const found = assessmentsData.find((a) => a.id === id) as
        | AssessmentOverviewDto
        | undefined;
      if (!found) {
        setError(T.ERROR_NOT_FOUND);
        return;
      }

      setAssessment(found);
      setQuestions(questionsData);
    } catch (err: unknown) {
      const message =
        err instanceof Error ? err.message : "Failed to load assessment data";
      setError(message);
    } finally {
      setIsLoading(false);
    }
  }, [id]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  // Close the action menu when clicking outside
  useEffect(() => {
    const handleOutsideClick = (e: MouseEvent) => {
      if (menuRef.current && !menuRef.current.contains(e.target as Node)) {
        setOpenMenuId(null);
      }
    };
    if (openMenuId) {
      document.addEventListener("mousedown", handleOutsideClick);
    }
    return () => document.removeEventListener("mousedown", handleOutsideClick);
  }, [openMenuId]);

  const handleBack = () => {
    navigate("/admin/assessments");
  };

  // ── Loading ─────────────────────────────────────────────────────────────────

  if (isLoading) {
    return <LapSpinnerv1 />;
  }

  // ── Error ───────────────────────────────────────────────────────────────────

  if (error || !assessment) {
    return (
      <div className="assessment-overview">
        <main className="assessment-overview-main">
          <div className="assessment-overview-error">
            <span className="material-symbols-outlined assessment-overview-error-icon">
              error
            </span>
            <Typography variant="body1">
              {error || T.ERROR_NOT_FOUND}
            </Typography>
            <LapButton type="outline" onClick={handleBack}>
              {T.BTN_BACK}
            </LapButton>
          </div>
        </main>
      </div>
    );
  }

  const questionCount = questions.length;
  const marksLabel =
    questionCount === 1
      ? `1 ${T.MARK_SINGULAR}`
      : `${questionCount} ${T.SECTION_QUESTIONS}`;
  const totalMarks =
    assessment.total_mark ??
    questions.reduce((sum, q) => sum + (q.weight ?? 0), 0);

  return (
    <div className="assessment-overview">
      <main className="assessment-overview-main">
        {/* Header */}
        <header className="assessment-overview-header">
          <div className="assessment-overview-header-left">
            <Typography variant="h3" className="assessment-overview-title">
              {T.PAGE_TITLE}
            </Typography>
            <Typography
              variant="body1"
              className="assessment-overview-subtitle"
            >
              {T.PAGE_SUBTITLE}
            </Typography>
          </div>
          <div className="assessment-overview-header-actions">
            <LapButton type="outline" onClick={handleBack}>
              {T.BTN_BACK}
            </LapButton>
            <LapButton type="primary" onClick={() => setEditModalOpen(true)}>
              {T.BTN_EDIT_ASSESSMENT}
            </LapButton>
          </div>
        </header>

        {/* Summary */}
        <section>
          <Typography
            variant="h2"
            className="assessment-overview-section-title"
          >
            <span className="material-symbols-outlined">info</span>
            {T.SECTION_SUMMARY}
          </Typography>
          <div className="assessment-overview-summary-card">
            <div className="assessment-overview-summary-grid">
              <div className="assessment-overview-summary-field">
                <label className="assessment-overview-summary-label">
                  {T.LABEL_TITLE}
                </label>
                <Typography
                  variant="body1"
                  className="assessment-overview-summary-value assessment-overview-summary-value--semibold"
                >
                  {capitalizeFirst(assessment.title) || T.UNTITLED}
                </Typography>
              </div>
              <div className="assessment-overview-summary-field">
                <label className="assessment-overview-summary-label">
                  {T.LABEL_DURATION}
                </label>
                <Typography
                  variant="body1"
                  className="assessment-overview-summary-value assessment-overview-summary-value--semibold"
                >
                  {assessment.duration_minute != null
                    ? `${assessment.duration_minute} ${T.DURATION_SUFFIX}`
                    : T.DASH}
                </Typography>
              </div>
              <div className="assessment-overview-summary-field assessment-overview-summary-field--full">
                <label className="assessment-overview-summary-label">
                  {T.LABEL_DESCRIPTION}
                </label>
                <Typography
                  variant="body1"
                  className="assessment-overview-summary-value"
                  style={{ lineHeight: 1.6 }}
                >
                  {capitalizeFirst(assessment.description) || T.NO_DESCRIPTION}
                </Typography>
              </div>
              <div className="assessment-overview-summary-marks">
                <div className="assessment-overview-summary-marks-item">
                  <label>{T.LABEL_TOTAL_MARKS}</label>
                  <Typography variant="h4" className="text-primary">
                    {totalMarks}
                  </Typography>
                </div>
                <div className="assessment-overview-summary-marks-item">
                  <label>{T.LABEL_PASSING_MARKS}</label>
                  <Typography variant="h4" className="text-secondary">
                    {assessment.passing_mark ?? T.DASH}
                  </Typography>
                </div>
                <div className="assessment-overview-summary-marks-item">
                  <label>{T.LABEL_QUESTIONS_COUNT}</label>
                  <Typography variant="h4" className="text-primary-container">
                    {questionCount}
                  </Typography>
                </div>
              </div>
            </div>
          </div>
        </section>

        {/* Questions */}
        <section>
          <Typography
            variant="h2"
            className="assessment-overview-section-header"
          >
            {T.SECTION_QUESTIONS}
            <span className="assessment-overview-question-count">
              {marksLabel}
            </span>
          </Typography>

          {questions.length === 0 ? (
            <div
              className="assessment-overview-summary-card"
              style={{ textAlign: "center", padding: "48px" }}
            >
              <span
                className="material-symbols-outlined"
                style={{ fontSize: 48, opacity: 0.3, marginBottom: 8 }}
              >
                quiz
              </span>
              <Typography
                variant="body2"
                sx={{ color: "var(--on-surface-variant)", margin: 0 }}
              >
                No questions uploaded yet.
              </Typography>
            </div>
          ) : (
            <div className="assessment-overview-questions">
              {questions.map((q, idx) => {
                const options = q.option_list ?? [];
                const cardKey = q.id ?? String(idx);
                const questionType = getQuestionType(q);
                const isMCQ = questionType === "multiple-choice";
                const isTrueFalse = questionType === "true-false";
                const isFillIn = questionType === "short-answer";
                return (
                  <div
                    key={cardKey}
                    className="assessment-overview-question-card"
                  >
                    <div className="assessment-overview-question-header">
                      <span className="assessment-overview-question-badge">
                        {T.QUESTION_PREFIX} {idx + 1}
                      </span>
                      <div
                        style={{
                          display: "flex",
                          alignItems: "center",
                          gap: 8,
                        }}
                      >
                        <span className="assessment-overview-question-type-badge">
                          {q.question_type?.name ?? T.QUESTION_TYPE_DEFAULT}
                        </span>
                        <span className="assessment-overview-question-weight">
                          {q.weight ?? 0}{" "}
                          {q.weight !== 1 ? T.MARK_PLURAL : T.MARK_SINGULAR}
                        </span>

                        {/* Action menu trigger */}
                        <div
                          className="assessment-overview-question-menu-wrapper"
                          ref={openMenuId === cardKey ? menuRef : null}
                        >
                          <button
                            className={`assessment-overview-question-edit-btn${
                              openMenuId === cardKey ? " is-active" : ""
                            }`}
                            aria-label={T.MENU_ARIA_LABEL}
                            aria-haspopup="menu"
                            aria-expanded={openMenuId === cardKey}
                            title={T.MENU_ARIA_LABEL}
                            onClick={() =>
                              setOpenMenuId((prev) =>
                                prev === cardKey ? null : cardKey,
                              )
                            }
                          >
                            <span className="material-symbols-outlined">
                              edit
                            </span>
                          </button>

                          {/* Dropdown menu */}
                          {openMenuId === cardKey && (
                            <div
                              className="assessment-overview-question-menu"
                              role="menu"
                              aria-label={T.MENU_PANEL_ARIA_LABEL}
                            >
                              <button
                                className="assessment-overview-question-menu-item"
                                role="menuitem"
                                aria-label={T.MENU_EDIT_ARIA}
                                onClick={() => {
                                  setOpenMenuId(null);
                                  openQuestionEdit(q);
                                }}
                              >
                                <span className="material-symbols-outlined">
                                  edit
                                </span>
                                {T.MENU_EDIT_QUESTION}
                              </button>
                              <div
                                className="assessment-overview-question-menu-divider"
                                role="separator"
                              />
                              <button
                                className="assessment-overview-question-menu-item assessment-overview-question-menu-item--danger"
                                role="menuitem"
                                aria-label={T.MENU_DELETE_ARIA}
                                onClick={() => {
                                  setOpenMenuId(null);
                                  if (q.id) deleteQuestion(q.id);
                                }}
                              >
                                <span className="material-symbols-outlined">
                                  delete
                                </span>
                                {T.MENU_DELETE_QUESTION}
                              </button>
                            </div>
                          )}
                        </div>
                      </div>
                    </div>
                    <Typography
                      variant="body1"
                      className="assessment-overview-question-text"
                    >
                      {q.question_text ?? T.NO_QUESTION_TEXT}
                    </Typography>

                    {(isMCQ || isTrueFalse) && options.length > 0 && (
                      <div className="assessment-overview-options-grid">
                        {options.map((opt, oi) => {
                          const isCorrect =
                            q.answer != null && opt === q.answer;
                          return (
                            <div
                              key={oi}
                              className={`assessment-overview-option${isCorrect ? " is-correct" : ""}`}
                            >
                              <div
                                className={`assessment-overview-option-dot${isCorrect ? " is-correct" : ""}`}
                              >
                                {isCorrect && (
                                  <span className="assessment-overview-option-dot-inner" />
                                )}
                              </div>
                              <span className="assessment-overview-option-text">
                                {opt}
                              </span>
                              {isCorrect && (
                                <span className="material-symbols-outlined assessment-overview-option-check">
                                  check_circle
                                </span>
                              )}
                            </div>
                          );
                        })}
                      </div>
                    )}

                    {isFillIn && q.answer != null && q.answer !== "" && (
                      <div className="assessment-overview-answer-box">
                        <span className="assessment-overview-answer-box-label">
                          {T.ANSWER_LABEL}
                        </span>
                        <span className="assessment-overview-answer-box-value">
                          {q.answer}
                        </span>
                      </div>
                    )}
                  </div>
                );
              })}
            </div>
          )}

          {questions.length > 0 && (
            <div className="assessment-overview-questions-footer">
              <span className="assessment-overview-questions-footer-line" />
              <Typography variant="caption">
                {T.SHOWING_ALL_PREFIX} {questionCount}{" "}
                {questionCount !== 1 ? T.QUESTION_PLURAL : T.QUESTION_SINGULAR}
              </Typography>
              <span className="assessment-overview-questions-footer-line" />
            </div>
          )}
        </section>
      </main>

      {/* Edit Assessment Modal */}
      <LapModalDialog
        open={editModalOpen}
        onClose={() => setEditModalOpen(false)}
        title={T.MODAL_EDIT_TITLE}
        subtitle={T.MODAL_EDIT_SUBTITLE}
        size="md"
      >
        <AssessmentForm
          courseId=""
          initialData={assessment}
          onSuccess={() => {
            setEditModalOpen(false);
            fetchData();
          }}
          onCancel={() => setEditModalOpen(false)}
        />
      </LapModalDialog>

      {/* Edit Question Modal */}
      <LapModalDialog
        open={!!editQuestion}
        onClose={() => setEditQuestion(null)}
        title={`Edit ${T.QUESTION_PREFIX} ${editQuestion ? questions.indexOf(editQuestion) + 1 : ""}`}
        subtitle={T.MODAL_EDIT_QUESTION_SUBTITLE}
        size="md"
        actions={
          <>
            <LapButton
              type="outline"
              onClick={() => setEditQuestion(null)}
              disabled={isSavingQuestion}
            >
              {T.BTN_CANCEL}
            </LapButton>
            <LapButton
              type="primary"
              onClick={saveQuestion}
              disabled={isSavingQuestion}
              loading={isSavingQuestion}
            >
              {isSavingQuestion ? T.BTN_SAVING : T.BTN_SAVE_QUESTION}
            </LapButton>
          </>
        }
      >
        {(() => {
          const editType = editQuestion
            ? getQuestionType(editQuestion)
            : "multiple-choice";
          const isEditMCQ = editType === "multiple-choice";
          const isEditTrueFalse = editType === "true-false";
          const isEditFillIn = editType === "short-answer";
          return (
            <div className="assessment-overview-question-edit-form">
              <div className="assessment-form-field">
                <label className="assessment-form-label">
                  {T.EDIT_QUESTION_LABEL_TEXT}
                </label>
                <textarea
                  className="assessment-form-textarea"
                  rows={3}
                  value={questionEditText}
                  onChange={(e) => setQuestionEditText(e.target.value)}
                  placeholder={T.EDIT_QUESTION_PLACEHOLDER_TEXT}
                />
              </div>

              {isEditMCQ && (
                <div className="assessment-form-field">
                  <label className="assessment-form-label">
                    {T.EDIT_QUESTION_LABEL_OPTIONS}
                  </label>
                  <LapInput
                    value={questionEditOptions}
                    onChange={(e) => setQuestionEditOptions(e.target.value)}
                    placeholder={T.EDIT_QUESTION_PLACEHOLDER_OPTIONS}
                  />
                </div>
              )}

              {isEditMCQ && (
                <div className="assessment-form-field">
                  <label className="assessment-form-label">
                    {T.EDIT_QUESTION_LABEL_ANSWER}
                  </label>
                  <LapInput
                    value={questionEditAnswer}
                    onChange={(e) => setQuestionEditAnswer(e.target.value)}
                    placeholder={T.EDIT_QUESTION_PLACEHOLDER_ANSWER}
                  />
                </div>
              )}

              {isEditTrueFalse && (
                <div className="assessment-form-field">
                  <label className="assessment-form-label">
                    {T.EDIT_QUESTION_LABEL_ANSWER}
                  </label>
                  <div className="assessment-overview-true-false-group">
                    <label
                      className={`assessment-overview-true-false-option${questionEditAnswer === "True" ? " is-selected" : ""}`}
                    >
                      <input
                        type="radio"
                        name="tf-answer"
                        value="True"
                        checked={questionEditAnswer === "True"}
                        onChange={() => {
                          setQuestionEditAnswer("True");
                          setQuestionEditOptions("True, False");
                        }}
                      />
                      True
                    </label>
                    <label
                      className={`assessment-overview-true-false-option${questionEditAnswer === "False" ? " is-selected" : ""}`}
                    >
                      <input
                        type="radio"
                        name="tf-answer"
                        value="False"
                        checked={questionEditAnswer === "False"}
                        onChange={() => {
                          setQuestionEditAnswer("False");
                          setQuestionEditOptions("True, False");
                        }}
                      />
                      False
                    </label>
                  </div>
                </div>
              )}

              {isEditFillIn && (
                <div className="assessment-form-field">
                  <label className="assessment-form-label">
                    {T.EDIT_QUESTION_LABEL_ANSWER}
                  </label>
                  <LapInput
                    value={questionEditAnswer}
                    onChange={(e) => setQuestionEditAnswer(e.target.value)}
                    placeholder={T.EDIT_QUESTION_PLACEHOLDER_FILL_ANSWER}
                  />
                </div>
              )}

              <div className="assessment-form-field">
                <label className="assessment-form-label">
                  {T.EDIT_QUESTION_LABEL_WEIGHT}
                </label>
                <LapInput
                  htmlType="number"
                  min={1}
                  value={questionEditWeight}
                  onChange={(e) =>
                    setQuestionEditWeight(parseInt(e.target.value) || 1)
                  }
                  placeholder={T.EDIT_QUESTION_PLACEHOLDER_WEIGHT}
                  style={{ maxWidth: 200 }}
                />
              </div>
            </div>
          );
        })()}
      </LapModalDialog>
    </div>
  );
}
