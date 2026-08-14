import React from "react";
import Accordion from "@mui/material/Accordion";
import AccordionSummary from "@mui/material/AccordionSummary";
import AccordionDetails from "@mui/material/AccordionDetails";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import Typography from "@mui/material/Typography";
import { CONTENT_TYPES } from "@/features/user/constants/constants";
import type {
  LapTopic,
  LapCurriculumAccordionProps,
} from "@/shared/types/ui.types";
import "./LapCurriculumAccordion.css";

export default function LapCurriculumAccordion({
  topics,
  defaultExpanded = [],
  onContentClick,
  showCompletion = true,
  disabled = false,
}: LapCurriculumAccordionProps) {
  return (
    <div className={`lap-ca-list${disabled ? " lap-ca-disabled" : ""}`}>
      {topics.map((topic, index) => {
        const isLocked = topic.contents.length === 0;
        return (
          <React.Fragment key={topic.id}>
            <Accordion
              defaultExpanded={defaultExpanded.includes(topic.id)}
              disableGutters
              className={`lap-ca-item ${index === 0 ? "lap-ca-item-first" : ""}`}
              sx={{
                borderRadius: "12px !important",
                overflow: "hidden",
                "&:before": { display: "none" },
                borderTop:
                  index === 0
                    ? `4px solid ${disabled ? "var(--outline-variant)" : "var(--primary-container)"}`
                    : "none",
                boxShadow: "--shadow-sm",
                opacity: disabled ? 0.8 : undefined,
              }}
            >
              <AccordionSummary
                expandIcon={
                  <ExpandMoreIcon
                    sx={{
                      color: disabled
                        ? "var(--outline-variant)"
                        : "var(--on-surface-variant)",
                    }}
                  />
                }
                sx={{
                  padding: "14px",
                  minHeight: "auto !important",
                  "&.Mui-expanded": { minHeight: "auto !important" },
                  "& .MuiAccordionSummary-content": {
                    margin: 0,
                    display: "flex",
                    alignItems: "center",
                    gap: "16px",
                  },
                  "&:hover": { background: "var(--surface-container-low)" },
                  boxShadow: "--shadow-sm",
                  cursor: disabled ? "default" : undefined,
                }}
              >
                <span className="lap-ca-num">
                  <Typography variant="caption" component="span">
                    {index + 1}
                  </Typography>
                </span>
                <Typography
                  variant="body2"
                  component="span"
                  className="lap-ca-title"
                >
                  {topic.name}
                </Typography>
                <Typography
                  variant="caption"
                  component="span"
                  sx={{
                    color: disabled
                      ? "var(--outline-variant)"
                      : "var(--on-surface-variant)",
                    whiteSpace: "nowrap",
                    ml: "auto",
                  }}
                >
                  {topic.durationMinute} min
                </Typography>
                {showCompletion && (
                  <span
                    className="material-symbols-outlined"
                    style={{
                      color:
                        topic.isCompleted ||
                        topic.contents.every((c) => c.isCompleted)
                          ? "#4CAF50"
                          : "#BDBDBD",
                      fontSize: "22px",
                    }}
                  >
                    {topic.isCompleted ||
                    topic.contents.every((c) => c.isCompleted)
                      ? "check_circle"
                      : "radio_button_unchecked"}
                  </span>
                )}
              </AccordionSummary>
              <AccordionDetails
                sx={{
                  padding: "14px",
                  paddingTop: "8px",
                  borderTop: "1px solid rgba(0,0,0,0.08)",
                  background: "var(--surface-container-lowest)",
                  opacity: disabled ? 0.7 : undefined,
                }}
              >
                {isLocked ? (
                  <Typography variant="body2" className="lap-ca-locked">
                    Locked until previous topic is complete.
                  </Typography>
                ) : (
                  <div className="lap-ca-items">
                    {topic.contents.map((content) => (
                      <div
                        key={content.id}
                        className="lap-ca-row"
                        onClick={
                          disabled
                            ? undefined
                            : (e) => {
                                e.stopPropagation();
                                onContentClick?.(content);
                              }
                        }
                        role={disabled ? undefined : "button"}
                        tabIndex={disabled ? undefined : 0}
                        onKeyDown={
                          disabled
                            ? undefined
                            : (e) => {
                                if (e.key === "Enter" || e.key === " ") {
                                  e.preventDefault();
                                  onContentClick?.(content);
                                }
                              }
                        }
                      >
                        <div className="lap-ca-row-left">
                          <span
                            className={`material-symbols-outlined lap-ca-icon ${content.contentType.name === CONTENT_TYPES.VIDEO ? "lap-ca-icon-video" : "lap-ca-icon-pdf"}`}
                          >
                            {content.contentType.name === CONTENT_TYPES.VIDEO
                              ? "play_circle"
                              : "picture_as_pdf"}
                          </span>
                          <Typography
                            variant="body2"
                            component="span"
                            className="lap-ca-content-title"
                          >
                            {content.title}
                          </Typography>
                        </div>
                        <div style={{ display: "flex", alignItems: "center" }}>
                          {showCompletion && (
                            <span
                              className="material-symbols-outlined"
                              style={{
                                color: content.isCompleted
                                  ? "#4CAF50"
                                  : "#BDBDBD",
                                fontSize: "20px",
                                marginRight: "8px",
                              }}
                            >
                              {content.isCompleted
                                ? "check_circle"
                                : "radio_button_unchecked"}
                            </span>
                          )}
                        </div>
                      </div>
                    ))}
                  </div>
                )}
              </AccordionDetails>
            </Accordion>
          </React.Fragment>
        );
      })}
    </div>
  );
}
