import { useState, useRef, useEffect } from "react";
import Typography from "@mui/material/Typography";
import { useForumMessages } from "../../../hooks/useForumMessages";
import { feedbackService } from "../../../services/feedback/feedbackService";
import { courseDiscussionStrings } from "./LapCourseDiscussion.constants";
import type { LapCourseDiscussionProps } from "./LapCourseDiscussion.types";
import { AVATAR_COLORS, timeStrings } from "./LapCourseDiscussion.constants";
import "./LapCourseDiscussion.css";

const formatMessageDate = (dateStr: string) => {
  if (!dateStr) return "";
  try {
    const date = new Date(dateStr);
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMins / 60);
    const diffDays = Math.floor(diffHours / 24);

    if (diffMins < 1) return timeStrings.justNow;
    if (diffMins < 60) return `${diffMins}${timeStrings.minutesAgo}`;
    if (diffHours < 24) return `${diffHours}${timeStrings.hoursAgo}`;
    if (diffDays < 7) return `${diffDays}${timeStrings.daysAgo}`;

    return date.toLocaleDateString("en-US", {
      month: "short",
      day: "numeric",
      year: date.getFullYear() !== now.getFullYear() ? "numeric" : undefined,
    });
  } catch {
    return dateStr;
  }
};

const getInitials = (name: string) => {
  return name
    .split(" ")
    .map((part) => part.charAt(0))
    .join("")
    .toUpperCase()
    .slice(0, 2);
};

const getAvatarColor = (name: string) => {
  let hash = 0;
  for (let i = 0; i < name.length; i++) {
    hash = name.charCodeAt(i) + ((hash << 5) - hash);
  }
  return AVATAR_COLORS[Math.abs(hash) % AVATAR_COLORS.length];
};

export default function LapCourseDiscussion({
  courseId,
}: LapCourseDiscussionProps) {
  const { messages, loading, error, refresh, sendMessage, sending } =
    useForumMessages(courseId);
  const [newMessage, setNewMessage] = useState("");
  const messagesEndRef = useRef<HTMLDivElement>(null);
  const textareaRef = useRef<HTMLTextAreaElement>(null);

  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

  const handleSend = async () => {
    const trimmed = newMessage.trim();
    if (!trimmed || sending) return;

    try {
      await sendMessage(trimmed);
      setNewMessage("");
      if (textareaRef.current) {
        textareaRef.current.style.height = "auto";
      }
    } catch {
      feedbackService.showToast(
        courseDiscussionStrings.error.sendFailed,
        "error",
      );
    }
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLTextAreaElement>) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      handleSend();
    }
  };

  const handleTextareaInput = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
    setNewMessage(e.target.value);
    const textarea = e.target;
    textarea.style.height = "auto";
    textarea.style.height = `${Math.min(textarea.scrollHeight, 120)}px`;
  };

  if (loading) {
    return (
      <div className="cd-container">
        <div className="cd-loading">
          <span className="material-symbols-outlined">progress_activity</span>
          <span>{courseDiscussionStrings.loading}</span>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="cd-container">
        <div className="cd-error">
          <span className="material-symbols-outlined">error</span>
          <span>{error}</span>
          <button type="button" onClick={refresh} className="cd-retry-btn">
            {courseDiscussionStrings.retry}
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="cd-container">
      <div className="cd-messages">
        {messages.length === 0 ? (
          <div className="cd-empty">
            <span className="material-symbols-outlined cd-empty-icon">
              forum
            </span>
            <Typography variant="h6" className="cd-empty-title">
              {courseDiscussionStrings.empty.title}
            </Typography>
            <Typography variant="body2" className="cd-empty-subtitle">
              {courseDiscussionStrings.empty.subtitle}
            </Typography>
          </div>
        ) : (
          messages.map((msg) => (
            <div key={msg.id} className="cd-message">
              <div
                className="cd-avatar"
                style={{ backgroundColor: getAvatarColor(msg.userFullName) }}
              >
                {getInitials(msg.userFullName)}
              </div>
              <div className="cd-message-body">
                <div className="cd-message-header">
                  <Typography
                    variant="caption"
                    component="span"
                    className="cd-message-author"
                  >
                    {msg.userFullName}
                  </Typography>
                  <Typography
                    variant="caption"
                    component="span"
                    className="cd-message-time"
                  >
                    {formatMessageDate(msg.dateCreated)}
                  </Typography>
                </div>
                <Typography variant="body2" className="cd-message-text">
                  {msg.messageText}
                </Typography>
              </div>
            </div>
          ))
        )}
        <div ref={messagesEndRef} />
      </div>

      <div className="cd-input-area">
        <div className="cd-input-wrapper">
          <textarea
            ref={textareaRef}
            className="cd-textarea"
            placeholder={courseDiscussionStrings.input.placeholder}
            value={newMessage}
            onChange={handleTextareaInput}
            onKeyDown={handleKeyDown}
            rows={1}
            disabled={sending}
          />
          <button
            type="button"
            className="cd-send-btn"
            onClick={handleSend}
            disabled={!newMessage.trim() || sending}
            aria-label={courseDiscussionStrings.input.sendAriaLabel}
          >
            <span className="material-symbols-outlined">
              {sending ? "progress_activity" : "send"}
            </span>
          </button>
        </div>
        <Typography variant="caption" className="cd-input-hint">
          {courseDiscussionStrings.input.hint}
        </Typography>
      </div>
    </div>
  );
}
