import Typography from "@mui/material/Typography";
import { useState, useCallback, useEffect, useMemo, useRef } from "react";
import type { ReviewDto } from "@/shared/services/api/models";
import {
  getCourseReviews,
  deleteReview,
  updateReview,
} from "../../services/reviewService";
import { getCurrentUser } from "@/features/auth/utils/authUtils";
import { useEnrollment } from "@/core/providers/EnrollmentProvider";
import { feedbackService } from "@/shared/services/feedback";
import { extractErrorMessage } from "@/shared/utils/apiErrorUtils";
import { useInfiniteScroll } from "@/shared/hooks";
import LapReviewCard from "@/shared/components/ui/LapReviewCard/LapReviewCard";
import LapNoContent from "@/shared/components/ui/LapNoContent/LapNoContent";
import LapSpinnerv1 from "@/shared/components/ui/LapSpinnerv1/LapSpinnerv1";
import LapModalDialog from "@/shared/components/feedback/LapModalDialog/LapModalDialog";
import type {
  RatingsViewProps,
  ReviewData,
} from "../../types/reviewService.types";
import ReviewForm from "../ReviewForm/ReviewForm";
import {
  PAGE_SIZE,
  INITIAL_PAGE,
  TOAST_MESSAGES,
  DELETE_CONFIRM_CONFIG,
  SECTION_TITLES,
  NO_CONTENT_LABELS,
  LOADING_LABEL,
  END_LABEL,
  EDIT_MODAL_TITLE,
  EDIT_MODAL_PROPS,
} from "./RatingsView.constants";
import "./RatingsView.css";

export default function RatingsView({
  courseId,
  refreshKey,
}: RatingsViewProps) {
  const [editingReview, setEditingReview] = useState<ReviewDto | null>(null);
  const [editModalOpen, setEditModalOpen] = useState(false);
  const [deleting, setDeleting] = useState<string | null>(null);
  const { enrolledCourses } = useEnrollment();
  const enrollment = courseId ? enrolledCourses[courseId] : undefined;
  const isEnrolled = enrollment !== undefined && enrollment?.status === true;

  const currentUser = useMemo(() => getCurrentUser(), []);

  const fetchPage = useCallback(
    async (page: number) => {
      const data = await getCourseReviews(courseId, page, PAGE_SIZE);
      return data;
    },
    [courseId],
  );

  const { items, loading, hasMore, sentinelRef, reset, updateItem } =
    useInfiniteScroll<ReviewDto>({
      fetchFn: fetchPage,
      initialPage: INITIAL_PAGE,
    });

  const refreshKeyRef = useRef(refreshKey);
  useEffect(() => {
    if (refreshKey !== undefined && refreshKey !== refreshKeyRef.current) {
      refreshKeyRef.current = refreshKey;
      reset();
    }
  }, [refreshKey, reset]);

  const { userReview, otherReviews } = useMemo(() => {
    const user = currentUser
      ? items.find(
          (r) =>
            r.user_full_name === currentUser.name ||
            r.user_id === currentUser.id,
        )
      : undefined;
    const others = currentUser ? items.filter((r) => r.id !== user?.id) : items;
    return { userReview: user ?? null, otherReviews: others };
  }, [items, currentUser]);

  const handleDelete = async (id: string) => {
    if (deleting) return;
    setDeleting(id);
    try {
      await deleteReview(id);
      feedbackService.showToast(TOAST_MESSAGES.deleteSuccess, "success");
      reset();
    } catch (err: unknown) {
      feedbackService.showToast(
        extractErrorMessage(err, TOAST_MESSAGES.deleteError),
        "error",
      );
    } finally {
      setDeleting(null);
    }
  };

  const startEdit = (review: ReviewDto) => {
    setEditingReview(review);
    setEditModalOpen(true);
  };

  const cancelEdit = () => {
    setEditingReview(null);
    setEditModalOpen(false);
  };

  const saveEdit = async (data: ReviewData) => {
    if (!editingReview?.id) return;
    try {
      await updateReview(editingReview.id, {
        rating: data.rating,
        reviewText: data.reviewText,
      });
      updateItem((r) => r.id === editingReview.id, {
        rating: data.rating,
        review_text: data.reviewText,
      } as Partial<ReviewDto>);
      feedbackService.showToast(TOAST_MESSAGES.updateSuccess, "success");
      cancelEdit();
    } catch (err: unknown) {
      feedbackService.showToast(
        extractErrorMessage(err, TOAST_MESSAGES.updateError),
        "error",
      );
    }
  };

  return (
    <>
      <div className="rv-container">
        {isEnrolled && (
          <div className="rv-section">
            <Typography variant="caption" className="rv-section-title">
              {SECTION_TITLES.YOUR_REVIEW}
            </Typography>
            {userReview ? (
              <LapReviewCard
                review={userReview}
                isOwn
                onEdit={startEdit}
                onDelete={async (id) => {
                  const confirmed = await feedbackService.showConfirm(
                    DELETE_CONFIRM_CONFIG,
                  );
                  if (confirmed) handleDelete(id);
                }}
              />
            ) : (
              <LapNoContent
                title={NO_CONTENT_LABELS.NO_REVIEW}
                message={NO_CONTENT_LABELS.SHARE}
              />
            )}
          </div>
        )}

        {/* Other reviews */}
        <div className="rv-section">
          <Typography variant="caption" className="rv-section-title">
            {otherReviews.length > 0
              ? SECTION_TITLES.OTHER_REVIEWS
              : SECTION_TITLES.COMMUNITY_REVIEWS}
          </Typography>
          {otherReviews.length > 0 ? (
            <div className="rv-list">
              {otherReviews.map((review) => (
                <LapReviewCard key={review.id} review={review} />
              ))}
            </div>
          ) : !loading ? (
            <LapNoContent
              title={NO_CONTENT_LABELS.NO_OTHER_REVIEWS}
              message={NO_CONTENT_LABELS.NO_OTHER_REVIEWS_MSG}
            />
          ) : items.length === 0 ? (
            <LapSpinnerv1 />
          ) : null}

          {/* Sentinel for infinite scroll */}
          {hasMore && (
            <div ref={sentinelRef} className="rv-sentinel">
              {loading && (
                <Typography variant="body2" className="rv-sentinel-text">
                  {LOADING_LABEL}
                </Typography>
              )}
            </div>
          )}

          {!hasMore && otherReviews.length > 0 && (
            <div className="rv-end">
              <Typography variant="body2">{END_LABEL}</Typography>
            </div>
          )}
        </div>
      </div>

      <LapModalDialog
        open={editModalOpen}
        onClose={cancelEdit}
        title={EDIT_MODAL_TITLE}
        maxWidth={EDIT_MODAL_PROPS.maxWidth}
      >
        {editingReview && (
          <ReviewForm
            onSubmit={saveEdit}
            initialValues={{
              rating: editingReview.rating ?? 0,
              reviewText: editingReview.review_text ?? "",
            }}
            onCancel={cancelEdit}
          />
        )}
      </LapModalDialog>
    </>
  );
}
