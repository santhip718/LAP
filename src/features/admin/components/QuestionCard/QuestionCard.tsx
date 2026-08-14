import { useState, useRef, useEffect } from "react";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import Typography from "@mui/material/Typography";
import type { QuestionDto } from "@/shared/services/api/models/questionDto";
import type { QuestionType } from "@/features/admin/components/QuestionCard/quiz.constants";
import "./QuestionCard.css";

interface QuestionCardProps {
  question: QuestionDto;
  index: number;
  questionType: QuestionType;
  labels: {
    QUESTION_PREFIX: string;
    MARK_SINGULAR: string;
    MARK_PLURAL: string;
    QUESTION_TYPE_DEFAULT: string;
    NO_QUESTION_TEXT: string;
    ANSWER_LABEL: string;
    MENU_ARIA_LABEL: string;
    MENU_PANEL_ARIA_LABEL: string;
    MENU_EDIT_QUESTION: string;
    MENU_DELETE_QUESTION: string;
    MENU_EDIT_ARIA: string;
    MENU_DELETE_ARIA: string;
  };
  onEdit: (question: QuestionDto) => void;
  onDelete: (questionId: string) => void;
}

export default function QuestionCard({
  question: q,
  index,
  questionType,
  labels: L,
  onEdit,
  onDelete,
}: QuestionCardProps) {
  const cardKey = q.id ?? String(index);
  const options = q.option_list ?? [];
  const isMCQ = questionType === "multiple-choice";
  const isTrueFalse = questionType === "true-false";
  const isFillIn = questionType === "short-answer";

  const [openMenu, setOpenMenu] = useState(false);
  const menuRef = useRef<HTMLDivElement | null>(null);

  useEffect(() => {
    const handleOutsideClick = (e: MouseEvent) => {
      if (menuRef.current && !menuRef.current.contains(e.target as Node)) {
        setOpenMenu(false);
      }
    };
    if (openMenu) {
      document.addEventListener("mousedown", handleOutsideClick);
    }
    return () => document.removeEventListener("mousedown", handleOutsideClick);
  }, [openMenu]);

  return (
    <Card variant="outlined" className="question-card">
      <CardContent>
        <div className="assessment-overview-question-header">
          <Typography
            variant="caption"
            className="assessment-overview-question-badge"
          >
            {L.QUESTION_PREFIX} {index + 1}
          </Typography>
          <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
            <Typography
              variant="caption"
              className="assessment-overview-question-type-badge"
            >
              {q.question_type?.name ?? L.QUESTION_TYPE_DEFAULT}
            </Typography>
            <Typography
              variant="caption"
              className="assessment-overview-question-weight"
            >
              {q.weight ?? 0} {q.weight !== 1 ? L.MARK_PLURAL : L.MARK_SINGULAR}
            </Typography>

            <div
              className="assessment-overview-question-menu-wrapper"
              ref={openMenu ? menuRef : null}
            >
              <button
                className={`assessment-overview-question-edit-btn${openMenu ? " is-active" : ""}`}
                aria-label={L.MENU_ARIA_LABEL}
                aria-haspopup="menu"
                aria-expanded={openMenu}
                title={L.MENU_ARIA_LABEL}
                onClick={() => setOpenMenu((prev) => !prev)}
              >
                <span className="material-symbols-outlined">edit</span>
              </button>

              {openMenu && (
                <div
                  className="assessment-overview-question-menu"
                  role="menu"
                  aria-label={L.MENU_PANEL_ARIA_LABEL}
                >
                  <button
                    className="assessment-overview-question-menu-item"
                    role="menuitem"
                    aria-label={L.MENU_EDIT_ARIA}
                    onClick={() => {
                      setOpenMenu(false);
                      onEdit(q);
                    }}
                  >
                    <span className="material-symbols-outlined">edit</span>
                    {L.MENU_EDIT_QUESTION}
                  </button>
                  <div
                    className="assessment-overview-question-menu-divider"
                    role="separator"
                  />
                  <button
                    className="assessment-overview-question-menu-item assessment-overview-question-menu-item--danger"
                    role="menuitem"
                    aria-label={L.MENU_DELETE_ARIA}
                    onClick={() => {
                      setOpenMenu(false);
                      if (q.id) onDelete(q.id);
                    }}
                  >
                    <span className="material-symbols-outlined">delete</span>
                    {L.MENU_DELETE_QUESTION}
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
          {q.question_text ?? L.NO_QUESTION_TEXT}
        </Typography>

        {(isMCQ || isTrueFalse) && options.length > 0 && (
          <div className="assessment-overview-options-grid">
            {options.map((opt, oi) => {
              const isCorrect = q.answer != null && opt === q.answer;
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
                  <Typography
                    variant="body2"
                    className="assessment-overview-option-text"
                  >
                    {opt}
                  </Typography>
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
            <Typography
              variant="caption"
              className="assessment-overview-answer-box-label"
            >
              {L.ANSWER_LABEL}
            </Typography>
            <Typography
              variant="body2"
              className="assessment-overview-answer-box-value"
            >
              {q.answer}
            </Typography>
          </div>
        )}
      </CardContent>
    </Card>
  );
}
