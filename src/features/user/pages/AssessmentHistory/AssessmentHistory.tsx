import { useCallback } from "react";
import { useNavigate } from "react-router-dom";
import Typography from "@mui/material/Typography";
import { getAssessmentHistory } from "../../services/assessmentService";
import { getCurrentUser } from "@/features/auth/utils/authUtils";
import type { AssessmentHistoryItemDto } from "@/shared/services/api/models";
import { useInfiniteScroll } from "@/shared/hooks";
import LapNoContent from "@/shared/components/ui/LapNoContent/LapNoContent";
import LapSpinnerv1 from "@/shared/components/ui/LapSpinnerv1/LapSpinnerv1";
import AssessmentHistoryCard from "../../components/AssessmentHistoryCard/AssessmentHistoryCard";
import {
  PAGE_SIZE,
  INITIAL_PAGE,
  EMPTY_ICON,
  PAGE_LABELS,
  EMPTY_LABELS,
  FOOTER_LABELS,
  BUTTON_LABEL,
  ROUTES,
} from "./AssessmentHistory.constants";
import "./AssessmentHistory.css";

export default function AssessmentHistory() {
  const navigate = useNavigate();

  const fetchHistoryData = useCallback(
    async (page: number): Promise<AssessmentHistoryItemDto[]> => {
      const user = getCurrentUser();
      if (!user?.id) return [];
      const data = await getAssessmentHistory(user.id, {
        pageNumber: page,
        pageSize: PAGE_SIZE,
      });
      return data;
    },
    [],
  );

  const { items = [], loading, hasMore, sentinelRef } =
    useInfiniteScroll<AssessmentHistoryItemDto>({
      fetchFn: fetchHistoryData,
      initialPage: INITIAL_PAGE,
    });

  const isEmpty = items.length === 0 && !loading;
  const loadingFirst = items.length === 0 && loading;
  const showEnd = !hasMore && items.length > 0;

  return (
    <div className="assessment-history">
      <main className="assessment-history-main">
        <div className="assessment-history-header">
          <Typography variant="h3">{PAGE_LABELS.TITLE}</Typography>
          <Typography variant="body1">{PAGE_LABELS.SUBTITLE}</Typography>
        </div>

        {loadingFirst ? (
          <div className="assessment-history-loading-wrap">
            <LapSpinnerv1 />
          </div>
        ) : isEmpty ? (
          <LapNoContent
            icon={EMPTY_ICON}
            title={EMPTY_LABELS.TITLE}
            message={EMPTY_LABELS.MESSAGE}
          >
            <button
              className="assessment-history-browse"
              onClick={() => navigate(ROUTES.MY_COURSES)}
            >
              {BUTTON_LABEL}
            </button>
          </LapNoContent>
        ) : (
          <>
            <div className="assessment-history-grid">
              {items.map((item) => (
                <AssessmentHistoryCard
                  key={item.assessment_history_id}
                  item={item}
                />
              ))}
            </div>

            {loading && items.length > 0 && (
              <div className="assessment-history-more">
                <span>{FOOTER_LABELS.LOADING}</span>
              </div>
            )}

            {showEnd && (
              <div className="assessment-history-end">
                <span>{FOOTER_LABELS.END}</span>
              </div>
            )}
          </>
        )}

        <div ref={sentinelRef} className="assessment-history-sentinel" />
      </main>
    </div>
  );
}
