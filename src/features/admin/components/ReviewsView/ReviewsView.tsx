import Typography from "@mui/material/Typography";
import { useCallback, useEffect, useRef } from "react";
import type { ReviewDto } from "@/shared/services/api/models";
import { getCourseReviews } from "@/features/user/services/reviewService";
import { useInfiniteScroll } from "@/shared/hooks";
import LapReviewCard from "@/shared/components/ui/LapReviewCard/LapReviewCard";
import LapNoContent from "@/shared/components/ui/LapNoContent/LapNoContent";
import LapSpinnerv1 from "@/shared/components/ui/LapSpinnerv1/LapSpinnerv1";
import type { ReviewsViewProps } from "../../types/reviewsView.types";
import {
  PAGE_SIZE,
  INITIAL_PAGE,
  SECTION_TITLE,
  NO_CONTENT_LABELS,
  LOADING_LABEL,
  END_LABEL,
} from "./ReviewsView.constants";
import "./ReviewsView.css";

export default function ReviewsView({
  courseId,
  refreshKey,
}: ReviewsViewProps) {
  const fetchPage = useCallback(
    async (page: number) => {
      const data = await getCourseReviews(courseId, page, PAGE_SIZE);
      return data;
    },
    [courseId],
  );

  const { items, loading, hasMore, sentinelRef, reset } =
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

  return (
    <div className="rev-container">
      <div className="rev-section">
        <Typography variant="caption" className="rev-section-title">
          {SECTION_TITLE}
        </Typography>
        {items.length > 0 ? (
          <div className="rev-list">
            {items.map((review) => (
              <LapReviewCard key={review.id} review={review} />
            ))}
          </div>
        ) : !loading ? (
          <LapNoContent
            title={NO_CONTENT_LABELS.NO_REVIEWS}
            message={NO_CONTENT_LABELS.NO_REVIEWS_MSG}
          />
        ) : (
          <LapSpinnerv1 />
        )}

        {hasMore && (
          <div ref={sentinelRef} className="rev-sentinel">
            {loading && (
              <Typography variant="body2" className="rev-sentinel-text">
                {LOADING_LABEL}
              </Typography>
            )}
          </div>
        )}

        {!hasMore && items.length > 0 && (
          <div className="rev-end">
            <Typography variant="body2">{END_LABEL}</Typography>
          </div>
        )}
      </div>
    </div>
  );
}
