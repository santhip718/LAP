import { useState, useEffect } from "react";
import { useForm } from "react-hook-form";
import Typography from "@mui/material/Typography";
import LapButton from "@/shared/components/ui/LapButton/LapButton";
import type {
  ReviewFormProps,
  ReviewFormValues,
} from "../../types/reviewService.types";
import {
  RATING_OPTIONS,
  MAX_REVIEW_LENGTH,
  VALIDATION_MESSAGES,
  TITLE_LABELS,
  PLACEHOLDER,
  BUTTON_LABELS,
  ICONS,
} from "./ReviewForm.constants";
import "./ReviewForm.css";

export default function ReviewForm({
  onSubmit,
  initialValues,
  onCancel,
}: ReviewFormProps) {
  const [rating, setRating] = useState(initialValues?.rating ?? 0);
  const [hoverRating, setHoverRating] = useState(0);
  const [ratingError, setRatingError] = useState(false);
  const {
    register,
    handleSubmit,
    reset,
    formState: { isSubmitting, errors },
  } = useForm<ReviewFormValues>();

  useEffect(() => {
    if (initialValues) {
      setRating(initialValues.rating);
      reset({ reviewText: initialValues.reviewText });
    }
  }, [initialValues, reset]);

  const isEditing = !!initialValues;

  const onFormSubmit = async (data: ReviewFormValues) => {
    if (rating === 0) {
      setRatingError(true);
      return;
    }
    setRatingError(false);
    await onSubmit({ rating, reviewText: data.reviewText });
    if (!isEditing) {
      setRating(0);
    }
  };

  return (
    <form className="co-form" onSubmit={handleSubmit(onFormSubmit)}>
      <Typography variant="body1" sx={{ mb: 1.5 }} className="rf-title">
        {isEditing ? TITLE_LABELS.EDIT : TITLE_LABELS.NEW}
      </Typography>

      <div style={{ marginBottom: 16 }}>
        <div className="co-stars">
          {RATING_OPTIONS.map((star) => (
            <button
              key={star}
              type="button"
              className={`co-star-btn ${(hoverRating || rating) >= star ? "co-star-active" : ""}`}
              onClick={() => {
                setRating(star);
                setRatingError(false);
              }}
              onMouseEnter={() => setHoverRating(star)}
              onMouseLeave={() => setHoverRating(0)}
            >
              <span className="material-symbols-outlined co-star-icon">
                {ICONS.STAR}
              </span>
            </button>
          ))}
        </div>
        {ratingError && (
          <Typography
            variant="caption"
            color="error"
            sx={{ display: "block", mt: 0.5 }}
          >
            {VALIDATION_MESSAGES.selectRating}
          </Typography>
        )}
      </div>

      <textarea
        className={`co-textarea ${errors.reviewText ? "co-textarea-error" : ""}`}
        placeholder={PLACEHOLDER}
        rows={4}
        {...register("reviewText", {
          required: VALIDATION_MESSAGES.required,
          maxLength: {
            value: MAX_REVIEW_LENGTH,
            message: VALIDATION_MESSAGES.tooLong,
          },
        })}
      />
      {errors.reviewText && (
        <Typography
          variant="caption"
          color="error"
          sx={{ mt: 0.5, display: "block" }}
        >
          {errors.reviewText.message}
        </Typography>
      )}

      <div
        style={{
          display: "flex",
          justifyContent: "flex-end",
          gap: 12,
          marginTop: 20,
        }}
      >
        {isEditing && onCancel && (
          <LapButton type="outline" onClick={onCancel}>
            {BUTTON_LABELS.CANCEL}
          </LapButton>
        )}
        <LapButton type="primary" htmlType="submit" loading={isSubmitting}>
          {isSubmitting
            ? BUTTON_LABELS.SAVING
            : isEditing
              ? BUTTON_LABELS.SAVE_CHANGES
              : BUTTON_LABELS.SUBMIT_FEEDBACK}
        </LapButton>
      </div>
    </form>
  );
}
