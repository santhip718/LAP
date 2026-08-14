import type { AssessmentOverviewDto } from '@/shared/services/api/models/assessmentOverviewDto';
import type { Column } from '@/shared/types';
import Box from '@mui/material/Box';
import Tooltip from '@mui/material/Tooltip';
import IconButton from '@mui/material/IconButton';
import DescriptionOutlinedIcon from '@mui/icons-material/DescriptionOutlined';
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import DeleteIcon from '@mui/icons-material/Delete';
import { ASSESSMENT_TABLE } from './assessmentTable.constants';
import { capitalizeFirst } from '@/shared/utils/stringUtils';

// ── Status helpers ─────────────────────────────────────────────────────────────

export type AssessmentStatus = 'Active' | 'Draft' | 'Inactive';

export function getAssessmentStatus(
  assessment: AssessmentOverviewDto,
): AssessmentStatus {
  if (assessment.passing_mark == null || assessment.total_mark == null)
    return 'Draft';
  if (
    assessment.total_mark === 0 ||
    assessment.duration_minute == null ||
    assessment.duration_minute <= 0
  ) {
    return 'Inactive';
  }
  return 'Active';
}

// ── Filter options ─────────────────────────────────────────────────────────────

export const assessmentFilterOptions: { label: string; value: string }[] = [
  { label: 'All Status', value: 'all' },
  { label: 'Active', value: 'Active' },
  { label: 'Draft', value: 'Draft' },
  { label: 'Inactive', value: 'Inactive' },
];

// ── Column factory ─────────────────────────────────────────────────────────────

/**
 * Builds the assessment column definitions.
 * @param onDelete - Called with the selected row when the delete icon is clicked.
 *                   The caller is responsible for showing the confirmation dialog.
 */
export function buildAssessmentColumns(
  onDelete: (row: AssessmentOverviewDto) => void,
): Column<AssessmentOverviewDto>[] {
  return [
    // ── Assessment title ───────────────────────────────────────────────────────
    {
      key: 'title',
      label: 'Assessment',
      sortable: true,
      // raw string value — default sort works fine, no sortValue needed
      render: (value: unknown) => (
        <Box sx={{
          display: 'flex',
          alignItems: 'center',
          gap: { xs: '6px', sm: '8px' },
          minHeight: { xs: '28px', sm: '32px' },
        }}>
          <DescriptionOutlinedIcon sx={{
            color: 'var(--secondary)',
            fontSize: { xs: 14, sm: 16 },
            flexShrink: 0,
          }} />
          <span style={{
            fontWeight: 600,
            color: 'var(--on-surface)',
            fontSize: 'inherit',
            lineHeight: 1.3,
            wordBreak: 'break-word',
          }}>
            {capitalizeFirst(value as string) || ASSESSMENT_TABLE.UNTITLED}
          </span>
        </Box>
      ),
    },

    // ── Course (nested object) ─────────────────────────────────────────────────
    {
      key: 'course',
      label: 'Course',
      sortable: true,
      // course is { title, ... } — extract title so the generic comparator works
      sortValue: (row: AssessmentOverviewDto) => row.course?.title ?? null,
      render: (value: unknown) => {
        const course = value as AssessmentOverviewDto['course'];
        return (
          <span style={{ color: 'var(--on-surface-variant)' }}>{capitalizeFirst(course?.title) || ASSESSMENT_TABLE.EM_DASH}</span>
        );
      },
    },

    // ── Duration ──────────────────────────────────────────────────────────────
    {
      key: 'duration_minute',
      label: 'Duration',
      sortable: true,
      // numeric field — default sort works fine
      render: (value: unknown) => (
        <Box
          sx={{
            display: 'inline-flex',
            alignItems: 'center',
            gap: '4px',
            bgcolor: 'var(--badge-duration-bg, #EEF2FF)',
            padding: '2px 8px',
            borderRadius: '20px',
            fontSize: '12px',
            fontWeight: 600,
            color: 'var(--secondary)',
            whiteSpace: 'nowrap',
          }}
        >
          <AccessTimeIcon sx={{ fontSize: 13 }} />
          {value != null ? `${value} min` : ASSESSMENT_TABLE.EM_DASH}
        </Box>
      ),
    },

    // ── Passing Mark ──────────────────────────────────────────────────────────
    {
      key: 'passing_mark',
      label: 'Passing Mark',
      sortable: true,
      mobileHidden: true,
      render: (value: unknown) => (
        <Box
          sx={{
            display: 'inline-flex',
            alignItems: 'center',
            gap: '4px',
            bgcolor: 'var(--badge-passing-bg, #FEF3C7)',
            padding: '2px 8px',
            borderRadius: '20px',
            fontSize: '12px',
            fontWeight: 600,
            color: 'var(--badge-passing-text, #D97706)',
            whiteSpace: 'nowrap',
          }}
        >
          <span className="material-symbols-outlined" style={{ fontSize: 14 }}>target</span>
          {value != null ? String(value) : ASSESSMENT_TABLE.EM_DASH}
        </Box>
      ),
    },

    // ── Total Mark ────────────────────────────────────────────────────────────
    {
      key: 'total_mark',
      label: 'Total Mark',
      sortable: true,
      mobileHidden: true,
      render: (value: unknown) => (
        <Box
          sx={{
            display: 'inline-flex',
            alignItems: 'center',
            gap: '4px',
            bgcolor: 'var(--badge-total-bg, #DCFCE7)',
            padding: '2px 8px',
            borderRadius: '20px',
            fontSize: '12px',
            fontWeight: 600,
            color: 'var(--badge-total-text, #16A34A)',
            whiteSpace: 'nowrap',
          }}
        >
          <span className="material-symbols-outlined" style={{ fontSize: 14 }}>grade</span>
          {value != null ? String(value) : ASSESSMENT_TABLE.EM_DASH}
        </Box>
      ),
    },

    // ── Actions ───────────────────────────────────────────────────────────────
    {
      key: 'actions',
      label: 'Actions',
      sortable: false,
      thClassName: 'cm-cell-center',
      className: 'cm-cell-center',
      render: (_value: unknown, row: AssessmentOverviewDto) => (
        <Tooltip title="Delete assessment" arrow>
          <IconButton
            size="small"
            onClick={(e) => {
              e.stopPropagation(); // don't fire row-click navigation
              onDelete(row);
            }}
            aria-label="Delete assessment"
            sx={{
              width: 30,
              height: 30,
              borderRadius: '6px',
              color: 'var(--outline)',
              transition: 'all 0.2s ease',
              '&:hover': {
                color: 'var(--error)',
                bgcolor: 'var(--surface)',
              },
            }}
          >
            <DeleteIcon fontSize="small" />
          </IconButton>
        </Tooltip>
      ),
    },
  ];
}

// ── Legacy static export ───────────────────────────────────────────────────────
// Kept for any external consumers that import assessmentColumns directly.
// Prefer buildAssessmentColumns(onDelete) for pages that handle deletion.
export const assessmentColumns = buildAssessmentColumns(() => undefined);
