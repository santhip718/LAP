import Typography from "@mui/material/Typography";
import type { LapReviewCardProps } from "@/shared/types/ui.types";
import "./LapReviewCard.css";

function getInitials(name: string | undefined | null): string {
  if (!name) return "?";
  return name
    .split(" ")
    .map((n) => n[0])
    .join("")
    .toUpperCase()
    .slice(0, 2);
}

function formatDate(dateStr: string | undefined | null): string {
  if (!dateStr) return "";
  const d = new Date(dateStr);
  return d.toLocaleDateString("en-US", {
    year: "numeric",
    month: "short",
    day: "numeric",
  });
}

function StarDisplay({ rating }: { rating: number }) {
  return (
    <div className="lap-review-card__stars">
      {[1, 2, 3, 4, 5].map((star) => (
        <span
          key={star}
          className={`material-symbols-outlined lap-review-card__star ${rating >= star ? "lap-review-card__star--filled" : ""}`}
          style={{ fontVariationSettings: rating >= star ? "'FILL' 1" : "'FILL' 0" }}
        >
          star
        </span>
      ))}
    </div>
  );
}

export default function LapReviewCard({
  review,
  isOwn = false,
  onEdit,
  onDelete,
}: LapReviewCardProps) {
  return (
    <div className={`lap-review-card ${isOwn ? "lap-review-card--own" : ""}`}>
      <div className="lap-review-card__top">
        <div className="lap-review-card__avatar" data-initials={getInitials(review.user_full_name)}>
          <Typography variant="body2" component="span">{getInitials(review.user_full_name)}</Typography>
        </div>
        <div className="lap-review-card__info">
          <div className="lap-review-card__info-top">
            <Typography variant="body2" component="span" className="lap-review-card__user-name">
              {review.user_full_name ?? "Anonymous"}
              {isOwn && <Typography variant="caption" component="span" className="lap-review-card__badge">You</Typography>}
            </Typography>
            <StarDisplay rating={review.rating ?? 0} />
          </div>
          <Typography variant="caption" component="span" className="lap-review-card__date">{formatDate(review.date_created)}</Typography>
        </div>
        {isOwn && (
          <div className="lap-review-card__actions">
            <button
              className="lap-review-card__action-btn"
              title="Edit review"
              onClick={() => onEdit?.(review)}
            >
              <span className="material-symbols-outlined">edit</span>
            </button>
            <button
              className="lap-review-card__action-btn lap-review-card__action-btn--danger"
              title="Delete review"
              onClick={() => review.id && onDelete?.(review.id)}
            >
              <span className="material-symbols-outlined">delete</span>
            </button>
          </div>
        )}
      </div>
      {review.review_text && (
        <Typography variant="body2" className="lap-review-card__text">{review.review_text}</Typography>
      )}
    </div>
  );
}
