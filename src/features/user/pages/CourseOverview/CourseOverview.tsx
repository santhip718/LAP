import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import Divider from "@mui/material/Divider";
import Typography from "@mui/material/Typography";
import CourseHero from "@/features/user/components/CourseHero/CourseHero";
import LapSidebar from "@/shared/components/ui/LapSidebar/LapSidebar";
import LapCourseLayout from "@/shared/components/layout/LapCourseLayout/LapCourseLayout";
import LapCurriculumAccordion from "@/shared/components/ui/LapCurriculumAccordion/LapCurriculumAccordion";
import LapSpinnerv1 from "@/shared/components/ui/LapSpinnerv1/LapSpinnerv1";
import LapCourseDiscussion from "@/shared/components/ui/LapCourseDiscussion/LapCourseDiscussion";
import RatingsView from "../../components/RatingsView/RatingsView";
import CourseLeaderboardPage from "@/features/leaderboard/pages/course-leaderboard/CourseLeaderboardPage";
import LapAssessmentCard from "@/shared/components/ui/LapAssessmentCard/LapAssessmentCard";
import LapNoContent from "@/shared/components/ui/LapNoContent/LapNoContent";
import AssessmentHistoryCard from "../../components/AssessmentHistoryCard/AssessmentHistoryCard";
import {
  getCourseOverview,
  getCourseProgress,
  type CourseDetail,
} from "../../services/courseDetailService";
import { getCourseContent } from "../../services/courseContentService";
import {
  getAssessmentOverview,
  getAssessmentAttemptInfo,
  getAssessmentAttempts,
  type AssessmentOverview,
} from "../../services/assessmentService";
import type { AssessmentHistoryItemDto } from "@/shared/services/api/models";
import { useEnrollment } from "@/core/providers/EnrollmentProvider";
import { submitReview } from "../../services/reviewService";
import type { ReviewData } from "../../types/reviewService.types";
import { feedbackService } from "@/shared/services/feedback";
import { extractErrorMessage } from "@/shared/utils/apiErrorUtils";
import LapModalDialog from "@/shared/components/feedback/LapModalDialog/LapModalDialog";
import ReviewForm from "../../components/ReviewForm/ReviewForm";
import {
  DEFAULT_TAB,
  TAB_DEFINITIONS,
  UNLOCK_THRESHOLD,
  MAX_ATTEMPTS,
  SUBMIT_TOAST,
  SUBMIT_ERROR_TOAST,
  REVIEW_MODAL_TITLE,
  ERROR_MESSAGE,
  CURRICULUM_HEADING,
  NO_CONTENT_LABELS,
  HISTORY_EMPTY_LABELS,
  HISTORY_TITLE,
  MODAL_PROPS,
  DIVIDER_SPACING,
} from "./CourseOverview.constants";
import "./CourseOverview.css";

export default function CourseOverview() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [course, setCourse] = useState<CourseDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(false);
  const [modalOpen, setModalOpen] = useState(false);
  const [activeTab, setActiveTab] = useState(DEFAULT_TAB);
  const [isSidebarCollapsed, setIsSidebarCollapsed] = useState(false);
  const [isMobileOpen, setIsMobileOpen] = useState(false);
  const [assessment, setAssessment] = useState<AssessmentOverview | null>(null);
  const [reviewRefreshKey, setReviewRefreshKey] = useState(0);
  const [completionPercent, setCompletionPercent] = useState(0);
  const [attemptsUsed, setAttemptsUsed] = useState(0);
  const [historyItems, setHistoryItems] = useState<AssessmentHistoryItemDto[]>([]);
  const { enrolledCourses, enroll } = useEnrollment();
  const enrollment = id ? enrolledCourses[id] : undefined;
  const isEnrolled = enrollment !== undefined;
  const canResume = isEnrolled && enrollment?.status === true;
  const canAccessAssessment = canResume && completionPercent >= UNLOCK_THRESHOLD;

  useEffect(() => {
    let cancelled = false;
    (async () => {
      if (!id) {
        setError(true);
        setLoading(false);
        return;
      }
      try {
        const courseData = await getCourseOverview(id);
        if (cancelled) return;
        if (isEnrolled) {
          try {
            const contentData = await getCourseContent(id);
            courseData.topics = courseData.topics.map((topic) => {
              const progressTopic = contentData.topics.find(
                (t) => t.id === topic.id,
              );
              if (progressTopic) {
                return {
                  ...topic,
                  isCompleted: progressTopic.isCompleted,
                  contents: topic.contents.map((content) => {
                    const progressContent = progressTopic.contents.find(
                      (c) => c.id === content.id,
                    );
                    return {
                      ...content,
                      isCompleted: progressContent?.isCompleted ?? false,
                    };
                  }),
                };
              }
              return topic;
            });
          } catch {
            console.error("Failed to load progress");
          }
        }
        if (isEnrolled) {
          try {
            const pct = await getCourseProgress(id);
            if (!cancelled) setCompletionPercent(pct);
          } catch {
            console.error("Failed to load course progress");
          }
        }
        if (courseData.assessmentTitle) {
          try {
            const asmData = await getAssessmentOverview(id);
            if (!cancelled) setAssessment(asmData);
            if (asmData) {
              try {
                const info = await getAssessmentAttemptInfo(asmData.id);
                if (!cancelled && info) setAttemptsUsed(info.attemptsUsed);
              } catch {
                console.error("Failed to load assessment attempts");
              }
              try {
                const attempts = await getAssessmentAttempts(asmData.id);
                if (!cancelled) {
                  const items = attempts.map((a, i) => ({
                    ...a,
                    course_id: id,
                    course_title: courseData.title,
                    assessment_history_id: a.assessment_history_id ?? `${i}`,
                  }));
                  setHistoryItems(items);
                }
              } catch {
                console.error("Failed to load assessment history");
              }
            }
          } catch {
            console.error("Failed to load assessment overview");
          }
        }
        if (!cancelled) setCourse(courseData);
      } catch {
        if (!cancelled) setError(true);
      } finally {
        if (!cancelled) setLoading(false);
      }
    })();
    return () => {
      cancelled = true;
    };
  }, [id, isEnrolled]);

  if (loading) {
    return <LapSpinnerv1 />;
  }

  if (error || !course) {
    return (
      <div
        style={{
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          minHeight: "50vh",
        }}
      >
        <Typography>{ERROR_MESSAGE}</Typography>
      </div>
    );
  }

  const totalLessons = course.topics.reduce(
    (sum, t) => sum + t.contents.length,
    0,
  );
  const topicCount = course.topics.length;
  const hours = Math.floor(course.durationMinute / 60);
  const mins = course.durationMinute % 60;
  const durationLabel = `${hours}h${mins > 0 ? ` ${mins}m` : ""}`;

  const links = TAB_DEFINITIONS;

  const handleSubmitReview = async (data: ReviewData) => {
    if (!id) return;
    try {
      await submitReview(id, data);
      feedbackService.showToast(SUBMIT_TOAST, "success");
      setModalOpen(false);
      setReviewRefreshKey((k) => k + 1);
    } catch (err: unknown) {
      feedbackService.showToast(extractErrorMessage(err, SUBMIT_ERROR_TOAST), "error");
    }
  };

  const renderContent = () => {
    switch (activeTab) {
      case "discussions":
        return <LapCourseDiscussion courseId={course.id} />;
      case "ratings":
        return (
          <RatingsView courseId={course.id} refreshKey={reviewRefreshKey} />
        );
      case "leaderboard":
        return <CourseLeaderboardPage courseId={id} />;
      case "overview":
      default:
        return (
          <main className="co-main">
            <CourseHero
              course={course}
              durationLabel={durationLabel}
              isEnrolled={isEnrolled}
              canResume={canResume}
              courseId={id}
              onEnroll={enroll}
              onRateClick={() => setModalOpen(true)}
            />

            <div className="co-grid">
              <div className="co-grid-left" id="curriculum">
                <Divider sx={{ my: DIVIDER_SPACING }} />
                <Typography variant="h5" className="co-curriculum-heading">
                  {CURRICULUM_HEADING}
                  <Typography variant="body2" component="span" className="co-curriculum-meta">
                    {topicCount} Topics &bull; {totalLessons} Lessons &bull;{" "}
                    {durationLabel}
                  </Typography>
                </Typography>
                {course.topics.length > 0 ? (
                  <LapCurriculumAccordion
                    topics={course.topics}
                    defaultExpanded={[course.topics[0]?.id ?? ""].filter(
                      Boolean,
                    )}
                    showCompletion={false}
                    disabled
                  />
                ) : (
                  <LapNoContent
                    title={NO_CONTENT_LABELS.NO_CURRICULUM}
                    message={NO_CONTENT_LABELS.NO_CONTENT}
                  />
                )}

                <Divider sx={{ my: DIVIDER_SPACING }} />
                <LapAssessmentCard
                  assessment={assessment}
                  canAccessAssessment={canAccessAssessment}
                  canResume={canResume}
                  completionPercent={completionPercent}
                  courseId={id!}
                  attemptsUsed={attemptsUsed}
                  maxAttempts={MAX_ATTEMPTS}
                />
              </div>
            </div>
          </main>
        );
      case "history":
        return (
          <main className="co-main">
            <Typography variant="h4" sx={{ mb: 3, px: 2 }}>
              {HISTORY_TITLE}
            </Typography>
            {historyItems.length > 0 ? (
              <div className="co-history-grid">
                {historyItems.map((item) => (
                  <AssessmentHistoryCard
                    key={item.assessment_history_id}
                    item={item}
                    onClick={(courseId, assessmentId) =>
                      navigate(`/course-overview/${courseId}/assessment`)
                    }
                  />
                ))}
              </div>
            ) : (
              <LapNoContent
                icon={HISTORY_EMPTY_LABELS.ICON}
                title={HISTORY_EMPTY_LABELS.TITLE}
                message={HISTORY_EMPTY_LABELS.MESSAGE}
              />
            )}
          </main>
        );
    }
  };

  return (
    <>
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
              {links.map((link) => (
                <button
                  key={link.id}
                  className={`co-sidebar-link${activeTab === link.id ? " co-sidebar-link--active" : ""}`}
                  onClick={() => {
                    setActiveTab(link.id);
                    setIsMobileOpen(false);
                  }}
                  title={isSidebarCollapsed ? link.label : ""}
                >
                  <span className="material-symbols-outlined">{link.icon}</span>
                  <Typography variant="body2" component="span" className="co-sidebar-link-label">{link.label}</Typography>
                </button>
              ))}
            </nav>
          </LapSidebar>
        }
      >
        {renderContent()}
      </LapCourseLayout>

      <LapModalDialog
        open={modalOpen}
        onClose={() => setModalOpen(false)}
        title={REVIEW_MODAL_TITLE}
        maxWidth={MODAL_PROPS.maxWidth}
      >
        <ReviewForm onSubmit={handleSubmitReview} />
      </LapModalDialog>
    </>
  );
}
