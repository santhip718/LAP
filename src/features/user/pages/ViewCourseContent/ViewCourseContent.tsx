import { useEffect, useState, useCallback } from "react";
import { useParams, useNavigate } from "react-router-dom";
import Typography from "@mui/material/Typography";
import CourseCanvas from "@/features/user/components/CourseCanvas/CourseCanvas";
import LapSidebar from "@/shared/components/ui/LapSidebar/LapSidebar";
import LapCourseLayout from "@/shared/components/layout/LapCourseLayout/LapCourseLayout";
import LapCurriculumAccordion from "@/shared/components/ui/LapCurriculumAccordion/LapCurriculumAccordion";
import type {
  LapContent,
  LapTopic,
} from "@/shared/types/ui.types";
import {
  getCourseOverview,
  getCourseProgress,
  type CourseDetail,
} from "@/features/user/services/courseDetailService";
import { getCourseContent } from "@/features/user/services/courseContentService";
import {
  getAssessmentOverview,
  getAssessmentAttemptInfo,
  type AssessmentOverview,
} from "@/features/user/services/assessmentService";
import apiClient from "@/shared/services/api/config/axios";
import { getCourseContent as getCourseContentApi } from "@/shared/services/api/services/course-content/course-content";
import { CONTENT_TYPES } from "@/features/user/constants/constants";
import LapAssessmentCard from "@/shared/components/ui/LapAssessmentCard/LapAssessmentCard";
import LapSpinnerv1 from "@/shared/components/ui/LapSpinnerv1/LapSpinnerv1";
import { feedbackService } from "@/shared/services/feedback";
import { extractErrorMessage } from "@/shared/utils/apiErrorUtils";
import {
  ASSESSMENT_UNLOCK_THRESHOLD,
  MAX_ATTEMPTS,
  SIDEBAR_WIDTH,
  SIDEBAR_STATUS,
  ERROR_BANNER,
  NAV_LABELS,
  COMPLETE_BUTTON,
  NAV_ICONS,
  COMPLETE_TOAST,
  COMPLETE_ERROR_TOAST,
  createFallbackCourse,
} from "./ViewCourseContent.constants";
import "./ViewCourseContent.css";

const courseContentApi = getCourseContentApi(apiClient);

function findContentById(
  topics: LapTopic[],
  id: string,
): LapContent | undefined {
  for (const topic of topics) {
    const found = topic.contents.find((c) => c.id === id);
    if (found) return found;
  }
  return undefined;
}

export default function ViewCourseContent() {
  const { courseId } = useParams<{ courseId: string }>();
  const navigate = useNavigate();
  const [course, setCourse] = useState<CourseDetail | null>(null);
  const [topics, setTopics] = useState<LapTopic[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(false);
  const [isSidebarCollapsed, setIsSidebarCollapsed] = useState(false);
  const [isMobileOpen, setIsMobileOpen] = useState(false);
  const [activeContent, setActiveContent] = useState<LapContent | null>(null);
  const [loadingContent, setLoadingContent] = useState(false);
  const [assessment, setAssessment] = useState<AssessmentOverview | null>(null);
  const [completionPercent, setCompletionPercent] = useState(0);
  const [attemptsUsed, setAttemptsUsed] = useState(0);
  const [prevContentId, setPrevContentId] = useState<string | null>(null);
  const [nextContentId, setNextContentId] = useState<string | null>(null);
  const [completing, setCompleting] = useState(false);

  const handleContentClick = useCallback(async (content: LapContent) => {
    setLoadingContent(true);
    try {
      const response = await courseContentApi.getApiV1CourseContentId(
        content.id,
      );
      const dto = response.data;
      const apiContentType = (dto.content_type ?? "").toLowerCase();
      const isPdfType = apiContentType.includes("pdf");

      setPrevContentId(dto.previous_content_id ?? null);
      setNextContentId(dto.next_content_id ?? null);

      setActiveContent({
        ...content,
        title: dto.title ?? content.title,
        contentType: {
          id: dto.content_type ?? content.contentType.id,
          name: isPdfType ? CONTENT_TYPES.PDF : CONTENT_TYPES.VIDEO,
        },
        videoUrl: dto.video_url ?? content.videoUrl,
        pdfBase64: dto.pdf_base64 ?? undefined,
        pdfFilePath: dto.pdf_base64 ? undefined : content.pdfFilePath,
        isCompleted: dto.is_completed ?? content.isCompleted,
      });
    } catch {
      console.error("Failed to load content");
    } finally {
      setLoadingContent(false);
    }
  }, []);

  const goToContent = useCallback(
    (id: string) => {
      const content = findContentById(topics, id);
      if (content) {
        handleContentClick(content);
      }
    },
    [topics, handleContentClick],
  );

  const markCompleted = useCallback(async () => {
    if (!activeContent?.id || completing) return;
    setCompleting(true);
    try {
      await courseContentApi.putApiV1CourseContentIdCompletionStatus(
        activeContent.id,
        { is_completed: true },
      );
      setActiveContent((prev) =>
        prev ? { ...prev, isCompleted: true } : prev,
      );
      feedbackService.showToast(COMPLETE_TOAST, "success");
      setTopics((prev) => {
        const updated = prev.map((topic) => ({
          ...topic,
          contents: topic.contents.map((c) =>
            c.id === activeContent.id ? { ...c, isCompleted: true } : c,
          ),
        }));
        const total = updated.reduce((s, t) => s + t.contents.length, 0);
        const done = updated.reduce(
          (s, t) => s + t.contents.filter((c) => c.isCompleted).length,
          0,
        );
        setCompletionPercent(total > 0 ? Math.round((done / total) * 100) : 0);
        return updated;
      });
    } catch (err: unknown) {
      feedbackService.showToast(
        extractErrorMessage(err, COMPLETE_ERROR_TOAST),
        "error",
      );
    } finally {
      setCompleting(false);
    }
  }, [activeContent, completing, courseContentApi]);

  useEffect(() => {
    let cancelled = false;
    (async () => {
      if (!courseId) {
        setError(true);
        setLoading(false);
        return;
      }
      try {
        const courseData = await getCourseOverview(courseId);
        if (cancelled) return;

        try {
          const pct = await getCourseProgress(courseId);
          if (!cancelled) setCompletionPercent(pct);
        } catch {
          console.error("Failed to load course progress");
        }

        if (courseData.assessmentTitle) {
          try {
            const asmData = await getAssessmentOverview(courseId);
            if (!cancelled) setAssessment(asmData);
            if (asmData) {
              try {
                const info = await getAssessmentAttemptInfo(asmData.id);
                if (!cancelled && info) setAttemptsUsed(info.attemptsUsed);
              } catch {
                console.error("Failed to load assessment attempts");
              }
            }
          } catch {
            console.error("Failed to load assessment overview");
          }
        }

        setCourse(courseData);
        setTopics(courseData.topics);
        try {
          const contentData = await getCourseContent(courseId);
          if (cancelled) return;
          if (contentData.topics.length > 0) {
            const updatedTopics = courseData.topics.map((topic) => {
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
            setTopics(updatedTopics);

            const firstContent = updatedTopics[0]?.contents[0];
            if (firstContent) {
              handleContentClick(firstContent);
            }
          }
        } catch {
          console.error("Failed to load content/progress");
        }
      } catch {
        if (!cancelled) setError(true);
      } finally {
        if (!cancelled) setLoading(false);
      }
    })();
    return () => {
      cancelled = true;
    };
  }, [courseId, handleContentClick]);

  const canAccessAssessment = completionPercent >= ASSESSMENT_UNLOCK_THRESHOLD;

  const defaultExpanded = topics.length > 0 ? [topics[0].id] : [];

  const displayCourse = course ?? createFallbackCourse();

  const sidebar = (
    <LapSidebar
      course={displayCourse}
      isCollapsed={isSidebarCollapsed}
      isMobileOpen={isMobileOpen}
      onToggleCollapse={() => setIsSidebarCollapsed((p) => !p)}
      onMobileClose={() => setIsMobileOpen(false)}
    >
      {loading ? (
        <Typography variant="body2" className="vcc-sidebar-status">
          {SIDEBAR_STATUS.LOADING}
        </Typography>
      ) : error ? (
        <Typography
          variant="body2"
          color="error"
          className="vcc-sidebar-status"
        >
          {SIDEBAR_STATUS.ERROR}
        </Typography>
      ) : topics.length === 0 && !assessment ? (
        <Typography variant="body2" className="vcc-sidebar-status">
          {SIDEBAR_STATUS.NO_CONTENT}
        </Typography>
      ) : (
        <>
          {topics.length > 0 && (
            <LapCurriculumAccordion
              topics={topics}
              defaultExpanded={defaultExpanded}
              onContentClick={handleContentClick}
              showCompletion
            />
          )}
          <LapAssessmentCard
            assessment={assessment}
            canAccessAssessment={canAccessAssessment}
            canResume
            completionPercent={completionPercent}
            courseId={courseId!}
            attemptsUsed={attemptsUsed}
            maxAttempts={MAX_ATTEMPTS}
          />
        </>
      )}
    </LapSidebar>
  );

  return (
    <LapCourseLayout
      sidebar={sidebar}
      isSidebarCollapsed={isSidebarCollapsed}
      isMobileOpen={isMobileOpen}
      onMobileToggle={() => setIsMobileOpen((p) => !p)}
      sidebarWidth={SIDEBAR_WIDTH}
    >
      {error && (
        <Typography variant="body2" className="vcc-error-banner">
          {ERROR_BANNER}
        </Typography>
      )}
      <div className="vcc-canvas-wrapper">
        {loadingContent ? (
          <LapSpinnerv1 />
        ) : (
          <CourseCanvas content={activeContent} />
        )}
      </div>

      {activeContent && (
        <div className="vcc-nav-bar">
          <div className="vcc-nav-left">
            <button
              className="vcc-nav-btn"
              disabled={!prevContentId}
              onClick={() => prevContentId && goToContent(prevContentId)}
            >
              <span className="material-symbols-outlined">
                {NAV_ICONS.PREVIOUS}
              </span>
              {NAV_LABELS.PREVIOUS}
            </button>
          </div>
          <div className="vcc-nav-center">
            <button
              className={`vcc-complete-btn ${activeContent.isCompleted ? "vcc-complete-btn-done" : ""}`}
              onClick={markCompleted}
              disabled={completing || activeContent.isCompleted}
            >
              <span className="material-symbols-outlined">
                {activeContent.isCompleted
                  ? NAV_ICONS.COMPLETED
                  : NAV_ICONS.UNCOMPLETED}
              </span>
              {activeContent.isCompleted
                ? COMPLETE_BUTTON.COMPLETED
                : completing
                  ? COMPLETE_BUTTON.MARKING
                  : COMPLETE_BUTTON.MARK_AS_COMPLETED}
            </button>
          </div>
          <div className="vcc-nav-right">
            <button
              className="vcc-nav-btn vcc-nav-btn-primary"
              disabled={!nextContentId}
              onClick={() => nextContentId && goToContent(nextContentId)}
            >
              {NAV_LABELS.NEXT}
              <span className="material-symbols-outlined">
                {NAV_ICONS.NEXT}
              </span>
            </button>
          </div>
        </div>
      )}
    </LapCourseLayout>
  );
}
