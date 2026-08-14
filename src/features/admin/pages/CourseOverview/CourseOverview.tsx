import { useState } from "react";
import { useParams, useNavigate, useLocation } from "react-router-dom";
import Typography from "@mui/material/Typography";
import { useCourseOverview } from "../../hooks/useCourseOverview";
import LapCourseDiscussion from "@/shared/components/ui/LapCourseDiscussion/LapCourseDiscussion";
import CourseLeaderboardPage from "@/features/leaderboard/pages/course-leaderboard/CourseLeaderboardPage";
import ReviewsView from "../../components/ReviewsView/ReviewsView";
import LapSidebar from "@/shared/components/ui/LapSidebar/LapSidebar";
import LapCourseLayout from "@/shared/components/layout/LapCourseLayout/LapCourseLayout";
import LapButton from "../../../../shared/components/ui/LapButton/LapButton";
import LapSpinnerv1 from "../../../../shared/components/ui/LapSpinnerv1/LapSpinnerv1";
import LapNoContent from "../../../../shared/components/ui/LapNoContent/LapNoContent";
import { courseOverviewStrings } from "./CourseOverview.constants";
import {
  getDifficultyColor,
  formatDuration,
  formatDate,
} from "../../utils/helpers";
import "./CourseOverview.css";

type TabId = "overview" | "discussion" | "reviews" | "leaderboard";

export default function CourseOverview() {
  const { courseId } = useParams<{ courseId: string }>();
  const navigate = useNavigate();
  const { pathname } = useLocation();
  const { course, loading, error, refresh } = useCourseOverview(courseId);

  const [isSidebarCollapsed, setIsSidebarCollapsed] = useState(false);
  const [isMobileOpen, setIsMobileOpen] = useState(false);

  let activeTab: TabId = "overview";
  if (pathname.endsWith("/leaderboard")) {
    activeTab = "leaderboard";
  } else if (pathname.endsWith("/discussion")) {
    activeTab = "discussion";
  } else if (pathname.endsWith("/reviews")) {
    activeTab = "reviews";
  }

  if (loading) {
    return (
      <div className="co-page">
        <main className="co-main">
          <LapSpinnerv1 />
        </main>
      </div>
    );
  }

  if (error || !course) {
    return (
      <div className="co-page">
        <main className="co-main">
          <LapNoContent
            icon="error"
            title={courseOverviewStrings.error.notFound}
            message={error || courseOverviewStrings.error.notFound}
          >
            <div className="co-error-actions">
              <LapButton type="outline" onClick={refresh}>
                {courseOverviewStrings.retry}
              </LapButton>
              <LapButton
                type="ghost"
                onClick={() => navigate("/admin/courses")}
              >
                {courseOverviewStrings.backToList}
              </LapButton>
            </div>
          </LapNoContent>
        </main>
      </div>
    );
  }

  return (
    <LapCourseLayout
      isSidebarCollapsed={isSidebarCollapsed}
      isMobileOpen={isMobileOpen}
      onMobileToggle={() => setIsMobileOpen((p) => !p)}
      sidebar={
        <LapSidebar
          course={course}
          onToggleCollapse={() => setIsSidebarCollapsed((p) => !p)}
          isCollapsed={isSidebarCollapsed}
          isMobileOpen={isMobileOpen}
          onMobileClose={() => setIsMobileOpen(false)}
        >
          <nav className="co-sidebar-nav">
            <button
              className={`co-sidebar-link${activeTab === "overview" ? " co-sidebar-link--active" : ""}`}
              onClick={() => {
                navigate(`/admin/courses/${courseId}`);
                setIsMobileOpen(false);
              }}
              title={isSidebarCollapsed ? courseOverviewStrings.tabs.overview : ""}
            >
              <span className="material-symbols-outlined">info</span>
              <Typography
                variant="body2"
                component="span"
                className="co-sidebar-link-label"
              >
                {courseOverviewStrings.tabs.overview}
              </Typography>
            </button>

            <button
              className={`co-sidebar-link${activeTab === "discussion" ? " co-sidebar-link--active" : ""}`}
              onClick={() => {
                navigate(`/admin/courses/${courseId}/discussion`);
                setIsMobileOpen(false);
              }}
              title={isSidebarCollapsed ? courseOverviewStrings.tabs.discussion : ""}
            >
              <span className="material-symbols-outlined">forum</span>
              <Typography
                variant="body2"
                component="span"
                className="co-sidebar-link-label"
              >
                {courseOverviewStrings.tabs.discussion}
              </Typography>
            </button>

            <button
              className={`co-sidebar-link${activeTab === "reviews" ? " co-sidebar-link--active" : ""}`}
              onClick={() => {
                navigate(`/admin/courses/${courseId}/reviews`);
                setIsMobileOpen(false);
              }}
              title={isSidebarCollapsed ? courseOverviewStrings.tabs.reviews : ""}
            >
              <span className="material-symbols-outlined">star</span>
              <Typography
                variant="body2"
                component="span"
                className="co-sidebar-link-label"
              >
                {courseOverviewStrings.tabs.reviews}
              </Typography>
            </button>

            <button
              className={`co-sidebar-link${activeTab === "leaderboard" ? " co-sidebar-link--active" : ""}`}
              onClick={() => {
                navigate(`/admin/courses/${courseId}/leaderboard`);
                setIsMobileOpen(false);
              }}
              title={isSidebarCollapsed ? courseOverviewStrings.tabs.leaderboard : ""}
            >
              <span className="material-symbols-outlined">leaderboard</span>
              <Typography
                variant="body2"
                component="span"
                className="co-sidebar-link-label"
              >
                {courseOverviewStrings.tabs.leaderboard}
              </Typography>
            </button>
          </nav>
        </LapSidebar>
      }
    >
      <main className="co-content">
        {activeTab === "overview" && (
          <div className="co-overview">
            <div className="co-hero">
              <div className="co-hero-thumb">
                {course.thumbnailUrl ? (
                  <img src={course.thumbnailUrl} alt={course.title} />
                ) : (
                  <span className="material-symbols-outlined">school</span>
                )}
              </div>
              <div className="co-hero-info">
                <div className="co-hero-top">
                  <span className="co-meta-prefix">
                    {courseOverviewStrings.meta.coursePrefix}
                  </span>
                  <span
                    className={`co-status-badge ${course.isDrafted ? "co-status-draft" : "co-status-published"}`}
                  >
                    {course.isDrafted
                      ? courseOverviewStrings.labels.draft
                      : courseOverviewStrings.labels.published}
                  </span>
                </div>
                <Typography variant="h5" className="co-title">
                  {course.title}
                </Typography>
                <div className="co-hero-meta">
                  <span className="co-hero-meta-item">
                    <span className="material-symbols-outlined">
                      {courseOverviewStrings.icons.folder}
                    </span>
                    {course.category}
                  </span>
                  {course.subCategory && (
                    <span className="co-hero-meta-item">
                      <span className="material-symbols-outlined">
                        {courseOverviewStrings.icons.info}
                      </span>
                      {course.subCategory}
                    </span>
                  )}
                  <span className="co-hero-meta-item">
                    <span
                      className="co-difficulty-dot"
                      style={{
                        background: getDifficultyColor(course.difficulty),
                      }}
                    />
                    {course.difficulty}
                  </span>
                  <span className="co-hero-meta-item">
                    <span className="material-symbols-outlined">
                      {courseOverviewStrings.icons.schedule}
                    </span>
                    {formatDuration(course.durationMinute)}
                  </span>
                  <span className="co-hero-meta-item">
                    <span className="material-symbols-outlined co-star-icon">
                      {courseOverviewStrings.icons.star}
                    </span>
                    {course.rating.toFixed(1)}
                  </span>
                </div>
              </div>
            </div>

            <div className="co-grid">
              <div className="co-main-col">
                {course.description && (
                  <section className="co-card">
                    <Typography variant="h6" className="co-card-title">
                      <span className="material-symbols-outlined">
                        {courseOverviewStrings.icons.info}
                      </span>
                      {courseOverviewStrings.sections.details}
                    </Typography>
                    <Typography variant="body1" className="co-description">
                      {course.description}
                    </Typography>
                  </section>
                )}

                <section className="co-card">
                  <Typography variant="h6" className="co-card-title">
                    <span className="material-symbols-outlined">
                      {courseOverviewStrings.icons.school}
                    </span>
                    {courseOverviewStrings.sections.content}
                  </Typography>
                  {course.topics.length === 0 ? (
                    <Typography variant="body2" className="co-empty">
                      {courseOverviewStrings.labels.noTopics}
                    </Typography>
                  ) : (
                    <div className="co-topics">
                      {course.topics.map((topic) => (
                        <div key={topic.id} className="co-topic">
                          <div className="co-topic-header">
                            <span className="co-topic-order">
                              {topic.sequenceOrder}
                            </span>
                            <div className="co-topic-info">
                              <h3 className="co-topic-name">{topic.name}</h3>
                              <span className="co-topic-meta">
                                {formatDuration(topic.durationMinute)}
                                {topic.contents.length > 0 && (
                                  <>
                                    {" "}
                                    · {topic.contents.length}{" "}
                                    {courseOverviewStrings.meta.contentCount
                                      .replace(
                                        "{count}",
                                        String(topic.contents.length),
                                      )
                                      .split(" ")
                                      .slice(1)
                                      .join(" ")}
                                  </>
                                )}
                              </span>
                            </div>
                          </div>
                          {topic.contents.length > 0 && (
                            <div className="co-contents">
                              {topic.contents.map((content) => (
                                <div
                                  key={content.id}
                                  className="co-content-item"
                                >
                                  <span className="co-content-seq">
                                    {content.sequenceOrder}
                                  </span>
                                  <span className="co-content-title">
                                    {content.title}
                                  </span>
                                </div>
                              ))}
                            </div>
                          )}
                        </div>
                      ))}
                    </div>
                  )}
                </section>
              </div>

              <aside className="co-side-col">
                <section className="co-card co-card-side">
                  <Typography variant="h6" className="co-card-title">
                    <span className="material-symbols-outlined">
                      {courseOverviewStrings.icons.calendarMonth}
                    </span>
                    {courseOverviewStrings.sections.meta}
                  </Typography>
                  <div className="co-detail-list">
                    <div className="co-detail-row">
                      <span className="co-detail-label">
                        {courseOverviewStrings.labels.instructor}
                      </span>
                      <span className="co-detail-value">
                        {course.createdBy}
                      </span>
                    </div>
                    <div className="co-detail-row">
                      <span className="co-detail-label">
                        {courseOverviewStrings.labels.enrollments}
                      </span>
                      <span className="co-detail-value">
                        {course.enrollmentCount}
                      </span>
                    </div>
                    <div className="co-detail-row">
                      <span className="co-detail-label">
                        {courseOverviewStrings.labels.createdDate}
                      </span>
                      <span className="co-detail-value">
                        {formatDate(course.dateCreated)}
                      </span>
                    </div>
                  </div>
                </section>

                <section className="co-card co-card-side">
                  <Typography variant="h6" className="co-card-title">
                    <span className="material-symbols-outlined">
                      {courseOverviewStrings.icons.assessment}
                    </span>
                    {courseOverviewStrings.sections.assessment}
                  </Typography>
                  {course.assessmentTitle ? (
                    <div className="co-detail-list">
                      <div className="co-detail-row">
                        <span className="co-detail-label">
                          {courseOverviewStrings.labels.assessmentTitle}
                        </span>
                        <span className="co-detail-value">
                          {course.assessmentTitle}
                        </span>
                      </div>
                      {course.totalMark > 0 && (
                        <div className="co-detail-row">
                          <span className="co-detail-label">
                            {courseOverviewStrings.labels.totalMark}
                          </span>
                          <span className="co-detail-value">
                            {course.totalMark}
                          </span>
                        </div>
                      )}
                      {course.passingMark > 0 && (
                        <div className="co-detail-row">
                          <span className="co-detail-label">
                            {courseOverviewStrings.labels.passingMark}
                          </span>
                          <span className="co-detail-value">
                            {course.passingMark}
                          </span>
                        </div>
                      )}
                    </div>
                  ) : (
                    <div
                      className="co-no-assessment"
                      style={{
                        display: "flex",
                        flexDirection: "column",
                        gap: "0.75rem",
                      }}
                    >
                      <Typography
                        variant="body2"
                        sx={{ color: "var(--on-surface-variant)" }}
                      >
                        No assessment created for this course.
                      </Typography>
                      <LapButton
                        type="primary"
                        onClick={() =>
                          navigate(`/admin/courses/${courseId}/assessments/new`)
                        }
                        style={{ width: "100%" }}
                      >
                        Create Assessment
                      </LapButton>
                    </div>
                  )}
                </section>
              </aside>
            </div>
          </div>
        )}

        {activeTab === "discussion" && courseId && (
          <div className="co-discussion">
            <div className="co-discussion-header">
              <Typography variant="h5" className="co-discussion-title">
                {courseOverviewStrings.tabs.discussion}
              </Typography>
              <Typography variant="body2" className="co-discussion-subtitle">
                {courseOverviewStrings.discussion.subtitle}
              </Typography>
            </div>
            <div className="co-discussion-content">
              <LapCourseDiscussion courseId={courseId} />
            </div>
          </div>
        )}

        {activeTab === "reviews" && courseId && (
          <div
            className="co-reviews"
            style={{ padding: "1.5rem var(--gutter) 3rem" }}
          >
            <ReviewsView courseId={courseId} />
          </div>
        )}

        {activeTab === "leaderboard" && courseId && (
          <div
            className="co-leaderboard"
            style={{ padding: "1.5rem var(--gutter) 3rem" }}
          >
            <CourseLeaderboardPage courseId={courseId} />
          </div>
        )}
      </main>
    </LapCourseLayout>
  );
}
