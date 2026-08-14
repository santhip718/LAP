import { useMemo, useCallback, useState, type ReactNode } from 'react';
import { useNavigate } from 'react-router-dom';
import type { AssessmentOverviewDto } from '@/shared/services/api/models/assessmentOverviewDto';
import LapErrorBoundary from '@/shared/components/feedback/LapErrorBoundary/LapErrorBoundary';
import LapDataTable from '@/shared/components/ui/LapDataTable/LapDataTable';
import LapModalDialog from '@/shared/components/feedback/LapModalDialog/LapModalDialog';
import LapSpinnerv1 from '@/shared/components/ui/LapSpinnerv1/LapSpinnerv1';
import LapButton from '@/shared/components/ui/LapButton/LapButton';
import AssessmentForm from '@/features/admin/components/AssessmentForm/AssessmentForm';
import { useAssessments } from '@/features/admin/hooks/useAssessments';
import { buildAssessmentColumns } from '@/features/admin/utils/assessmentTableConfig';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import { feedbackService } from '@/shared/services/feedback/feedbackService';
import { ASSESSMENT_MANAGEMENT as T } from './AssessmentManagement.constants';
import './AssessmentManagement.css';

// ── Page component ─────────────────────────────────────────────────────────────

export default function AssessmentManagement() {
  const { items, isLoading, error, refetch, deleteAssessment, isDeleting, loadMore, hasMore } =
    useAssessments();
  const navigate = useNavigate();

  // ── Modal state ──────────────────────────────────────────────────────────────
  const [createModalOpen, setCreateModalOpen] = useState(false);
  const [deleteTarget, setDeleteTarget] = useState<AssessmentOverviewDto | null>(
    null,
  );

  const handleRowClick = useCallback(
    (row: AssessmentOverviewDto) => {
      if (row.id) {
        navigate(`/admin/assessments/${row.id}`);
      }
    },
    [navigate],
  );

  // ── Delete flow ──────────────────────────────────────────────────────────────

  const handleDeleteClick = useCallback((row: AssessmentOverviewDto) => {
    setDeleteTarget(row);
  }, []);

  const handleDeleteConfirm = useCallback(async () => {
    if (!deleteTarget?.id) return;
    try {
      await deleteAssessment(deleteTarget.id);
      feedbackService.showToast(T.TOAST_DELETE_SUCCESS, 'success');
      setDeleteTarget(null);
      refetch();
    } catch (err: unknown) {
      const message =
        err instanceof Error ? err.message : T.TOAST_DELETE_ERROR;
      feedbackService.showToast(message, 'error');
      // Modal stays open so the user can retry or cancel
    }
  }, [deleteTarget, deleteAssessment, refetch]);

  const handleDeleteCancel = useCallback(() => {
    if (!isDeleting) setDeleteTarget(null);
  }, [isDeleting]);

  // Build columns with the real delete callback (memoised so reference is stable)
  const assessmentColumns = useMemo(
    () => buildAssessmentColumns(handleDeleteClick),
    [handleDeleteClick],
  );

  // ── Page content ─────────────────────────────────────────────────────────────

  let content: ReactNode;

  if (isLoading) {
    content = <LapSpinnerv1 />;
  } else if (error) {
    content = (
      <Box className="assessment-management">
        <Box component="main" className="assessment-management-main">
          <Typography color="error">
            {error.message || T.ERROR_LOAD}
          </Typography>
          <LapButton type="outline" onClick={refetch}>
            {T.BTN_RETRY}
          </LapButton>
        </Box>
      </Box>
    );
  } else if (items.length === 0) {
    content = (
      <Box className="assessment-management">
        <Box component="main" className="assessment-management-main">
          <Box className="assessment-management-header">
            <Box>
              <Typography
                variant="h1"
                className="assessment-management-title"
                sx={{
                  fontSize: { xs: '22px', sm: '32px', md: '40px' },
                  lineHeight: { xs: '30px', sm: '40px', md: '48px' },
                  wordBreak: 'break-word',
                  overflowWrap: 'break-word',
                  whiteSpace: 'normal',
                }}
              >
                {T.PAGE_TITLE}
              </Typography>
              <Typography
                variant="body1"
                className="assessment-management-subtitle"
              >
                {T.PAGE_SUBTITLE}
              </Typography>
            </Box>
          </Box>
          <Box className="assessment-management-empty">
            <Box className="assessment-management-empty-icon">📄</Box>
            <Typography className="assessment-management-empty-title">
              No assessments found
            </Typography>
            <Typography className="assessment-management-empty-text">
              Create your first assessment to get started.
            </Typography>
            <LapButton type="primary" onClick={() => setCreateModalOpen(true)}>
              {T.BTN_CREATE}
            </LapButton>
          </Box>
        </Box>
      </Box>
    );
  } else {
    content = (
      <Box className="assessment-management">
        <Box component="main" className="assessment-management-main">
          {/* Header */}
          <Box className="assessment-management-header">
            <Box>
              <Typography
                variant="h1"
                className="assessment-management-title"
                sx={{
                  fontSize: { xs: '22px', sm: '32px', md: '40px' },
                  lineHeight: { xs: '30px', sm: '40px', md: '48px' },
                  wordBreak: 'break-word',
                  overflowWrap: 'break-word',
                  whiteSpace: 'normal',
                }}
              >
                {T.PAGE_TITLE}
              </Typography>
              <Typography
                variant="body1"
                className="assessment-management-subtitle"
              >
                {T.PAGE_SUBTITLE}
              </Typography>
            </Box>
            <LapButton type="primary" onClick={() => setCreateModalOpen(true)}>
              {T.BTN_CREATE}
            </LapButton>
          </Box>

          {/* Data table */}
          <LapDataTable<AssessmentOverviewDto>
            columns={assessmentColumns}
            data={items}
            pageSize={10}
            enableInfiniteScroll
            onLoadMore={loadMore}
            hasMore={hasMore}
            enableSearch
            searchPlaceholder={T.SEARCH_PLACEHOLDER}
            searchKeys={['title', 'course']}
            onRowClick={handleRowClick}
          />
        </Box>
      </Box>
    );
  }

  return (
    <LapErrorBoundary>
      {content}

      {/* Create Assessment Modal */}
      <LapModalDialog
        open={createModalOpen}
        onClose={() => setCreateModalOpen(false)}
        title={T.MODAL_CREATE_TITLE}
        maxWidth="md"
      >
        <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
          {T.MODAL_CREATE_SUBTITLE}
        </Typography>
        <AssessmentForm
          courseId=""
          onSuccess={() => {
            setCreateModalOpen(false);
            refetch();
          }}
          onCancel={() => setCreateModalOpen(false)}
        />
      </LapModalDialog>

      {/* Delete Confirmation Modal */}
      <LapModalDialog
        open={!!deleteTarget}
        onClose={handleDeleteCancel}
        title={T.MODAL_DELETE_TITLE}
        maxWidth="sm"
      >
        <Box>
          <Typography variant="body2" className="assessment-delete-modal-message" sx={{ mb: 3 }}>
            {T.MODAL_DELETE_MESSAGE}
          </Typography>
          <Box sx={{ display: 'flex', justifyContent: 'flex-end', gap: 2 }}>
            <LapButton type="outline" onClick={handleDeleteCancel} disabled={isDeleting}>
              {T.BTN_DELETE_CANCEL}
            </LapButton>
            <LapButton type="logout" onClick={handleDeleteConfirm} disabled={isDeleting} loading={isDeleting}>
              {isDeleting ? T.BTN_DELETE_DELETING : T.BTN_DELETE_CONFIRM}
            </LapButton>
          </Box>
        </Box>
      </LapModalDialog>
    </LapErrorBoundary>
  );
}
