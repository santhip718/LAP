import { useCallback, useMemo, useState } from "react";
import Typography from "@mui/material/Typography";
import { useNavigate } from "react-router-dom";
import LapDataTable, {
  type Column,
} from "../../../../shared/components/ui/LapDataTable/LapDataTable";
import LapAddButton from "../../../../shared/components/ui/LapAddButton/LapAddButton";
import StatCard from "../../components/StatCard/StatCard";
import { useDebounce } from "../../../../shared/hooks/useDebounce";
import CreateCourseModal from "../../components/CreateCourseModal/CreateCourseModal";
import { courseService } from "../../services/courseService";
import { useAdminCourses } from "../../hooks/useAdminCourses";
import type { AdminCourseListItem, CourseEditData } from "../../types";
import { courseListingStrings, deleteCourseStrings, editCourseStrings, courseManagementStrings, DEBOUNCE_DELAY } from "./CourseManagement.constants";
import { FALLBACK_EMPTY } from "../../utils/constants";
import { feedbackService } from "../../../../shared/services/feedback/feedbackService";
import { getCurrentUser } from "@/features/auth/utils/authUtils";
import "./CourseManagement.css";

const getDifficultyColor = (difficulty: string) => {
  const normalized = difficulty.toLowerCase();
  if (normalized.includes("beginner")) return "var(--success)";
  if (normalized.includes("intermediate")) return "var(--secondary)";
  if (normalized.includes("expert") || normalized.includes("advanced")) {
    return "var(--danger)";
  }
  return "var(--primary)";
};

const formatDuration = (minutes: number) => {
  if (minutes < 60) return `${minutes}m`;
  const hours = Math.floor(minutes / 60);
  const remainingMinutes = minutes % 60;
  return remainingMinutes ? `${hours}h ${remainingMinutes}m` : `${hours}h`;
};

export default function CourseManagement() {
  const navigate = useNavigate();
  const [search, setSearch] = useState("");
  const debouncedSearch = useDebounce(search, DEBOUNCE_DELAY);
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [editCourse, setEditCourse] = useState<CourseEditData | null>(null);
  const [loadingEdit, setLoadingEdit] = useState(false);
  const {
    courses,
    totalCourses,
    summary,
    loading,
    summaryLoading,
    error,
    summaryError,
    refreshCourses,
    loadingMore,
    hasMore,
    loadMore,
  } = useAdminCourses({ search: debouncedSearch, status: "all" });

  const handleEdit = useCallback(async (courseId: string) => {
    setLoadingEdit(true);
    try {
      const data = await courseService.getCourseForEdit(courseId);
      const currentUser = getCurrentUser();
      if (currentUser?.name && data.createdBy && currentUser.name !== data.createdBy) {
        const confirmed = await feedbackService.showConfirm({
          title: editCourseStrings.title,
          message: editCourseStrings.message,
          confirmLabel: editCourseStrings.confirmLabel,
          cancelLabel: editCourseStrings.cancelLabel,
        });
        if (!confirmed) {
          setLoadingEdit(false);
          return;
        }
      }
      setEditCourse(data);
    } catch (err) {
      console.error("Failed to load course for editing:", err);
      feedbackService.showToast(courseManagementStrings.error.loadFailed, "error");
    } finally {
      setLoadingEdit(false);
    }
  }, []);

  const handleCloseEdit = useCallback(() => {
    setEditCourse(null);
  }, []);

  const handleDelete = useCallback(async (courseId: string, courseTitle: string) => {
    const confirmed = await feedbackService.showConfirm({
      title: deleteCourseStrings.title,
      message: deleteCourseStrings.message(courseTitle),
      confirmLabel: deleteCourseStrings.confirmLabel,
      cancelLabel: deleteCourseStrings.cancelLabel,
    });

    if (confirmed) {
      try {
        await courseService.deleteCourse(courseId);
        feedbackService.showToast(deleteCourseStrings.success, "success");
        refreshCourses();
      } catch (err) {
        console.error("Failed to delete course:", err);
        feedbackService.showToast(deleteCourseStrings.error, "error");
      }
    }
  }, [refreshCourses]);

  const columns = useMemo<Column<AdminCourseListItem>[]>(
    () => [
      {
        key: "title",
        label: courseListingStrings.columns.title,
        sortable: true,
        render: (_: unknown, row: AdminCourseListItem) => (
          <div className="cm-course-cell" onClick={() => navigate(`/admin/courses/${row.id}`)}>
            <div className="cm-course-thumb">
              {row.thumbnailUrl ? (
                <img src={row.thumbnailUrl} alt={row.title} />
              ) : (
                <span className="material-symbols-outlined">school</span>
              )}
            </div>
            <div>
              <div className="cm-course-name">{row.title}</div>
            </div>
          </div>
        ),
      },
      {
        key: "category",
        label: courseListingStrings.columns.category,
        sortable: true,
        render: (value: unknown) => (
          <span className="cm-badge">{String(value)}</span>
        ),
      },
      {
        key: "difficulty",
        label: courseListingStrings.columns.difficulty,
        sortable: true,
        render: (value: unknown) => {
          const difficulty = String(value);
          return (
            <div className="cm-difficulty">
              <span
                className="cm-difficulty-dot"
                style={{ background: getDifficultyColor(difficulty) }}
              />
              <span>{difficulty}</span>
            </div>
          );
        },
      },
      {
        key: "durationMinute",
        label: courseListingStrings.columns.duration,
        sortable: true,
        className: "cm-cell-center",
        thClassName: "cm-cell-center",
        render: (value: unknown) => (
          <span className="cm-duration">{formatDuration(Number(value))}</span>
        ),
      },
      {
        key: "rating",
        label: courseListingStrings.columns.rating,
        sortable: true,
        className: "cm-cell-center",
        thClassName: "cm-cell-center",
        render: (value: unknown) => (
          <div className="cm-rating">
            <span className="material-symbols-outlined cm-star">star</span>
            <span>{Number(value).toFixed(1)}</span>
          </div>
        ),
      },
      {
        key: "isDrafted",
        label: courseListingStrings.columns.status,
        sortable: true,
        className: "cm-cell-center",
        thClassName: "cm-cell-center",
        render: (value: unknown) => {
          const isDrafted = Boolean(value);
          return (
            <span
              className={`cm-status-badge ${
                isDrafted ? "cm-status-draft" : "cm-status-published"
              }`}
            >
              {isDrafted ? courseListingStrings.status.drafted : courseListingStrings.status.published}
            </span>
          );
        },
      },
      {
        key: "actions",
        label: "",
        className: "cm-cell-right",
        render: (_: unknown, row: AdminCourseListItem) => (
          <div className="cm-actions-cell">
            <button
              className="cm-edit-btn"
              type="button"
              aria-label={courseManagementStrings.ariaLabels.editCourse}
              onClick={(e) => {
                e.stopPropagation();
                handleEdit(row.id);
              }}
            >
              <span className="material-symbols-outlined">edit</span>
            </button>
            <button
              className="cm-delete-btn"
              type="button"
              aria-label={courseManagementStrings.ariaLabels.deleteCourse}
              onClick={(e) => {
                e.stopPropagation();
                handleDelete(row.id, row.title);
              }}
            >
              <span className="material-symbols-outlined">delete</span>
            </button>
          </div>
        ),
      },
    ],
    [handleEdit, handleDelete],
  );

  const activeCount = courses.filter((course) => !course.isDrafted).length;

  return (
    <div className="cm-page">
      <main className="cm-main">
        <div className="cm-header">
          <div>
            <Typography variant="h2" className="cm-title">{courseListingStrings.pageTitle}</Typography>
            <Typography variant="body1" className="cm-subtitle">{courseListingStrings.pageSubtitle}</Typography>
          </div>
          <LapAddButton label={courseListingStrings.addCourseButton} onClick={() => setIsCreateModalOpen(true)} />
        </div>

        <div className="cm-stats">
          <StatCard
            label={courseListingStrings.stats.totalCourses}
            value={summaryLoading ? FALLBACK_EMPTY : String(summary.totalCourses || totalCourses)}
          />
          <StatCard
            label={courseListingStrings.stats.published}
            value={summaryLoading ? FALLBACK_EMPTY : String(summary.publishedCourses)}
            trend={!summaryLoading ? { text: `${summary.draftCourses} draft`, icon: "school" } : undefined}
          />
          <StatCard
            label={courseListingStrings.stats.activeStudents}
            value={summaryLoading ? FALLBACK_EMPTY : String(summary.activeStudents)}
          />
          <StatCard
            label={courseListingStrings.stats.enrollments}
            value={summaryLoading ? FALLBACK_EMPTY : String(summary.totalEnrollments)}
          />
        </div>

        <div className="cm-table-header">
          <div className="cm-table-header-left">
            <Typography variant="h5" className="cm-table-title">{courseListingStrings.table.title}</Typography>
            <span className={`cm-active-badge ${activeCount > 0 ? 'cm-badge-success' : 'cm-badge-danger'}`}>{activeCount} {courseListingStrings.table.activeBadge}</span>
          </div>
          <div className="cm-table-actions">
            <label className="cm-search">
              <span className="material-symbols-outlined">search</span>
              <input
                type="search"
                value={search}
                onChange={(event) => setSearch(event.target.value)}
                placeholder={courseListingStrings.table.searchPlaceholder}
              />
            </label>
            <div className="cm-divider" />
            <button className="cm-icon-btn" type="button" onClick={refreshCourses}>
              <span className="material-symbols-outlined">refresh</span>
            </button>
          </div>
        </div>

        {error && (
          <div className="cm-state cm-error-state">
            <span className="material-symbols-outlined">error</span>
            <span>{error}</span>
            <button type="button" onClick={refreshCourses}>
              {courseListingStrings.table.errorRetry}
            </button>
          </div>
        )}

        {!error && summaryError && (
          <div className="cm-state cm-warning-state">
            <span className="material-symbols-outlined">info</span>
            <span>{summaryError}</span>
          </div>
        )}

        {loading ? (
          <div className="cm-state">
            <span className="material-symbols-outlined">progress_activity</span>
            <span>{courseListingStrings.table.emptyStateLoading}</span>
          </div>
        ) : (
          <>
            <LapDataTable<AdminCourseListItem>
              columns={columns}
              data={courses}
              enableInfiniteScroll
              onLoadMore={loadMore}
              hasMore={hasMore}
            />
            {loadingMore && (
              <div className="cm-state cm-loading-more">
                <span className="material-symbols-outlined">progress_activity</span>
                <span>{courseListingStrings.table.emptyStateLoadingMore}</span>
              </div>
            )}
          </>
        )}
      </main>

      <CreateCourseModal
        open={isCreateModalOpen}
        onClose={() => setIsCreateModalOpen(false)}
        onSuccess={refreshCourses}
      />

      <CreateCourseModal
        key={editCourse?.id ?? "edit"}
        open={!!editCourse}
        onClose={handleCloseEdit}
        onSuccess={refreshCourses}
        editCourse={editCourse}
      />
    </div>
  );
}
