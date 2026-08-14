import { useCallback, useEffect, useMemo, useState } from "react";
import { useEnrollments } from "../../hooks/useEnrollments";
import { enrollmentService } from "../../services/enrollmentService";
import type { EnrollmentItem } from "../../types";
import { referenceDataService } from "../../../../shared/services/referenceDataService";
import type { RefTerm } from "../../../../shared/services/referenceDataService";
import LapDataTable, { type Column } from "../../../../shared/components/ui/LapDataTable/LapDataTable";
import LapButton from "../../../../shared/components/ui/LapButton/LapButton";
import { feedbackService } from "../../../../shared/services/feedback/feedbackService";
import { enrollmentStrings, FALLBACK_EMPTY } from "./EnrollManagement.constants";
import Typography from "@mui/material/Typography";
import "./EnrollmentManagement.css";

const formatDate = (dateStr: string) => {
  if (!dateStr) return FALLBACK_EMPTY;
  try {
    return new Date(dateStr).toLocaleDateString("en-US", {
      year: "numeric",
      month: "short",
      day: "numeric",
    });
  } catch {
    return dateStr;
  }
};

export default function EnrollmentManagement() {
  const { enrollments, loading, error, refreshing, refresh, setFilters } = useEnrollments();
  const [acceptingId, setAcceptingId] = useState<string | null>(null);
  const [search, setSearch] = useState("");
  const [categories, setCategories] = useState<RefTerm[]>([]);
  const [selectedCategoryId, setSelectedCategoryId] = useState("");

  useEffect(() => {
    referenceDataService.getCategories().then(setCategories).catch(() => {});
  }, []);

  const handleSearchChange = useCallback((value: string) => {
    setSearch(value);
    setFilters({ courseName: value || undefined, categoryId: selectedCategoryId || undefined });
  }, [setFilters, selectedCategoryId]);

  const handleCategoryChange = useCallback((categoryId: string) => {
    setSelectedCategoryId(categoryId);
    setFilters({ courseName: search || undefined, categoryId: categoryId || undefined });
  }, [setFilters, search]);

  const handleAccept = useCallback(async (id: string) => {
    setAcceptingId(id);
    try {
      await enrollmentService.acceptEnrollment(id);
      feedbackService.showToast(enrollmentStrings.success.accepted, "success");
      refresh();
    } catch (err) {
      console.error("Failed to accept enrollment:", err);
      feedbackService.showToast(enrollmentStrings.error.acceptFailed, "error");
    } finally {
      setAcceptingId(null);
    }
  }, [refresh]);

  const pendingEnrollments = useMemo(
    () => enrollments.filter((e) => !e.enrollmentStatus),
    [enrollments],
  );

  const columns = useMemo<Column<EnrollmentItem>[]>(
    () => [
      {
        key: "userFullName",
        label: enrollmentStrings.columns.user,
        sortable: true,
        render: (_: unknown, row: EnrollmentItem) => (
          <div className="em-user-cell">
            <span className="material-symbols-outlined em-user-icon">person</span>
            <span className="em-user-name">{row.userFullName}</span>
          </div>
        ),
      },
      {
        key: "courseTitle",
        label: enrollmentStrings.columns.course,
        sortable: true,
        render: (value: unknown) => (
          <span className="em-course-title">{String(value)}</span>
        ),
      },
      {
        key: "category",
        label: enrollmentStrings.columns.category,
        sortable: true,
        render: (value: unknown) => (
          <Typography variant="caption" className="em-badge">{String(value)}</Typography>
        ),
      },
      {
        key: "enrolledOn",
        label: enrollmentStrings.columns.enrolledOn,
        sortable: true,
        render: (value: unknown) => (
          <Typography variant="body2" className="em-date">{formatDate(String(value))}</Typography>
        ),
      },
      {
        key: "actions",
        label: "",
        className: "em-cell-right",
        render: (_: unknown, row: EnrollmentItem) => (
          <div className="em-actions-cell">
            <LapButton
              type="primary"
              loading={acceptingId === row.id}
              disabled={!!acceptingId}
              onClick={() => handleAccept(row.id)}
            >
              {acceptingId === row.id
                ? enrollmentStrings.actions.accepting
                : enrollmentStrings.actions.accept}
            </LapButton>
          </div>
        ),
      },
    ],
    [handleAccept, acceptingId],
  );

  return (
    <div className="em-page">
      <main className="em-main">
        <div className="em-header">
          <div>
            <Typography variant="h2" className="em-title">{enrollmentStrings.pageTitle}</Typography>
            <Typography variant="body1" className="em-subtitle">{enrollmentStrings.pageSubtitle}</Typography>
          </div>
          <button
            className="em-icon-btn"
            type="button"
            onClick={refresh}
            disabled={refreshing}
            aria-label={enrollmentStrings.ariaLabels.refresh}
          >
            <span className="material-symbols-outlined">
              {refreshing ? "progress_activity" : "refresh"}
            </span>
          </button>
        </div>

        <div className="em-table-card">
          <div className="em-table-header">
            <div className="em-table-header-left">
              <Typography variant="h5" className="em-table-title">{enrollmentStrings.table.title}</Typography>
              <Typography variant="caption" className={`em-count-badge ${pendingEnrollments.length > 0 ? 'em-badge-success' : 'em-badge-danger'}`}>{enrollmentStrings.table.pendingCount.replace("{count}", String(pendingEnrollments.length))}</Typography>
            </div>
            <div className="em-table-actions">
              <label className="em-search">
                <span className="material-symbols-outlined">search</span>
                <input
                  type="search"
                  value={search}
                  onChange={(e) => handleSearchChange(e.target.value)}
                  placeholder={enrollmentStrings.table.searchPlaceholder}
                />
              </label>
              <div className="em-filter">
                <label className="em-filter-label">{enrollmentStrings.filters.categoryLabel}</label>
                <select
                  className="em-filter-select"
                  value={selectedCategoryId}
                  onChange={(e) => handleCategoryChange(e.target.value)}
                >
                  <option value="">{enrollmentStrings.filters.allCategories}</option>
                  {categories.map((cat) => (
                    <option key={cat.id} value={cat.id}>{cat.name}</option>
                  ))}
                </select>
              </div>
            </div>
          </div>

          {error && (
            <div className="em-error-state">
              <span className="material-symbols-outlined">error</span>
              <span>{error}</span>
              <button type="button" onClick={refresh}>
                {enrollmentStrings.table.errorRetry}
              </button>
            </div>
          )}

          {loading ? (
            <div className="em-loading-state">
              <span className="material-symbols-outlined">progress_activity</span>
              <span>{enrollmentStrings.table.loading}</span>
            </div>
          ) : !error && pendingEnrollments.length === 0 ? (
            <div className="em-empty-state">
              <span className="material-symbols-outlined">how_to_reg</span>
              <Typography variant="h6">{enrollmentStrings.table.emptyState}</Typography>
              <Typography variant="body2">{enrollmentStrings.table.emptyStateMessage}</Typography>
            </div>
          ) : !error ? (
            <LapDataTable<EnrollmentItem>
              columns={columns}
              data={pendingEnrollments}
              enableInfiniteScroll
            />
          ) : null}
        </div>
      </main>
    </div>
  );
}
