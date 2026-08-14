import apiClient from "../../../shared/services/api/apiClient";
import { getCourse } from "../../../shared/services/api/services/course/course";
import { getCourseContent } from "../../../shared/services/api/services/course-content/course-content";
import { env } from "../../../core/config/env";
import type { CourseOverviewDto } from "../../../shared/services/api/models/courseOverviewDto";
import type { CourseOverviewMetaTopicDto } from "../../../shared/services/api/models/courseOverviewMetaTopicDto";
import type { CourseSummaryDto } from "../../../shared/services/api/models/courseSummaryDto";
import type { ForumMessageDto } from "../../../shared/services/api/models/forumMessageDto";
import type { GetApiV1CourseParams } from "../../../shared/services/api/models/getApiV1CourseParams";
import type { PostApiV1CourseBody } from "../../../shared/services/api/models/postApiV1CourseBody";
import type { PostApiV1CourseContentBody } from "../../../shared/services/api/models/postApiV1CourseContentBody";
import type { PutApiV1CourseContentIdBody } from "../../../shared/services/api/models/putApiV1CourseContentIdBody";
import type { PutApiV1CourseCourseIdBody } from "../../../shared/services/api/models/putApiV1CourseCourseIdBody";
import type { CreateCourseForm } from '../types/courseFormTypes';

const STORAGE_KEY_PREFIX = 'opencode_deleted_content_';

function getDeletedContentIds(courseId: string): string[] {
  try {
    const raw = sessionStorage.getItem(`${STORAGE_KEY_PREFIX}${courseId}`);
    return raw ? JSON.parse(raw) : [];
  } catch { return []; }
}

function addDeletedContentIds(courseId: string, ids: string[]): void {
  if (ids.length === 0) return;
  try {
    const existing = getDeletedContentIds(courseId);
    const updated = [...new Set([...existing, ...ids])];
    sessionStorage.setItem(`${STORAGE_KEY_PREFIX}${courseId}`, JSON.stringify(updated));
  } catch { /* sessionStorage unavailable */ }
}

function removeDeletedContentIds(courseId: string, ids: string[]): void {
  if (ids.length === 0) return;
  try {
    const existing = getDeletedContentIds(courseId);
    const set = new Set(ids);
    const updated = existing.filter(id => !set.has(id));
    sessionStorage.setItem(`${STORAGE_KEY_PREFIX}${courseId}`, JSON.stringify(updated));
  } catch { /* sessionStorage unavailable */ }
}
import type {
  CreateCourseRequest,
  AdminCourseListItem,
  AdminCourseListResult,
  AdminCourseSummary,
  CourseOverviewTopic,
  CourseOverviewItem,
  ForumMessage,
  CourseEditData,
} from '../types/courseServiceTypes';
import { DEFAULT_PAGE_SIZE } from '../pages/CourseManagement/CourseManagement.constants';
import {
  FALLBACK_COURSE_TITLE,
  FALLBACK_CATEGORY,
  FALLBACK_NOT_SET,
  FALLBACK_UNKNOWN,
  FALLBACK_TOPIC_TITLE,
  FALLBACK_CONTENT_TITLE,
  FALLBACK_USER,
  DEFAULT_TOPIC_DURATION,
  DEFAULT_SEQUENCE_ORDER,
  DEFAULT_DURATION_HOURS,
  HOURS_TO_MINUTES,
  courseServiceStrings,
} from '../utils/constants';

const courseApi = getCourse(apiClient);
const courseContentApi = getCourseContent(apiClient);

export function resolveThumbnailUrl(path: string | undefined | null): string | undefined {
  if (!path) return undefined;
  if (path.startsWith("http://") || path.startsWith("https://") || path.startsWith("data:")) return path;
  const baseUrl = env.apiBaseUrl ?? "";
  return `${baseUrl}${path.startsWith("/") ? "" : "/"}${path}`;
}

type LiveCourseSummaryDto = CourseSummaryDto & {
  thumbnail_img?: string | null;
};

const DEFAULT_TOPIC = {
  name: "",
  contentTitle: "",
  metaTopicOrder: DEFAULT_SEQUENCE_ORDER,
  metaTopicDuration: DEFAULT_TOPIC_DURATION,
  sequenceOrder: DEFAULT_SEQUENCE_ORDER,
  contentTypeId: "",
  videoUrl: "",
  contentFile: undefined as File | undefined,
};

export function getDefaultCourseFormValues(
  editCourse?: CourseEditData | null,
  contentTypes?: any[],
): CreateCourseForm {
  if (editCourse) {
    return {
      title: editCourse.title,
      description: editCourse.description || "",
      categoryId: editCourse.categoryId,
      subCategoryId: editCourse.subCategoryId || "",
      difficultyLevelId: editCourse.difficultyLevelId,
      durationHours: editCourse.durationHours || DEFAULT_DURATION_HOURS,
      thumbnailFile: undefined,
      topics: editCourse.topics.length > 0
        ? editCourse.topics.map((t) => {
            let resolvedId = t.contentTypeId || "";
            if (contentTypes && contentTypes.length > 0) {
              const term = contentTypes.find(
                (ct) =>
                  ct.name?.toLowerCase() === t.contentTypeName?.toLowerCase() ||
                  ct.name?.toLowerCase() === t.contentTypeId?.toLowerCase() ||
                  ct.id === t.contentTypeId
              );
              if (term) {
                resolvedId = term.id;
              }
            }
            return {
              name: t.name,
              originalName: t.name,
              contentTitle: t.contentTitle,
              metaTopicOrder: t.metaTopicOrder,
              metaTopicDuration: t.metaTopicDuration,
              sequenceOrder: t.sequenceOrder,
              contentTypeId: resolvedId,
              videoUrl: t.videoUrl || "",
              contentFile: undefined,
              existingPdfUrl: t.pdfFilePath || undefined,
              contentId: t.contentId || undefined,
            };
          })
        : [{ ...DEFAULT_TOPIC }],
    };
  }

  return {
    title: "",
    description: "",
    categoryId: "",
    subCategoryId: "",
    difficultyLevelId: "",
    durationHours: DEFAULT_DURATION_HOURS,
    thumbnailFile: undefined,
    topics: [{ ...DEFAULT_TOPIC }],
  };
}

export function buildCreateCoursePayload(
  data: CreateCourseForm,
  isDrafted: boolean,
  topicCount: number,
  deletedContentIds?: string[],
): CreateCourseRequest {
  return {
    title: data.title,
    description: data.description,
    categoryId: data.categoryId,
    subCategoryId: data.subCategoryId || undefined,
    difficultyLevelId: data.difficultyLevelId,
    durationHours: Number(data.durationHours),
    thumbnailFile: data.thumbnailFile,
    isDrafted,
    deletedContentIds,
    topics: Array.from({ length: Math.min(topicCount, data.topics.length) }).map((_, i) => {
      const topic = data.topics[i];
      return {
        name: topic?.name ?? "",
        originalName: topic?.originalName,
        contentTitle: topic?.contentTitle ?? "",
        metaTopicOrder: Number(topic?.metaTopicOrder ?? i + 1),
        metaTopicDuration: Number(topic?.metaTopicDuration ?? DEFAULT_TOPIC_DURATION),
        sequenceOrder: Number(topic?.sequenceOrder ?? DEFAULT_SEQUENCE_ORDER),
        contentTypeId: topic?.contentTypeId ?? "",
        videoUrl: topic?.videoUrl || undefined,
        contentFile: topic?.contentFile,
        contentId: topic?.contentId,
      };
    }),
  };
}

const firstFile = (file?: File | FileList): File | undefined => {
  if (!file) return undefined;
  if (file instanceof File) return file;
  return file.length > 0 ? file[0] : undefined;
};

const mapCourseSummary = (course: LiveCourseSummaryDto): AdminCourseListItem => ({
  id: course.id ?? "",
  title: course.title ?? FALLBACK_COURSE_TITLE,
  category: course.category?.name ?? FALLBACK_CATEGORY,
  difficulty: course.difficulty_level?.name ?? FALLBACK_NOT_SET,
  durationMinute: course.duration_minute ?? 0,
  rating: course.overall_rating ?? 0,
  thumbnailUrl: resolveThumbnailUrl(course.thumbnail_img),
  isDrafted: course.is_drafted ?? false,
});

export const courseService = {
  async getAdminCourses(
    params: Omit<GetApiV1CourseParams, "page" | "pageSize"> & { page?: number; pageSize?: number } = {},
  ): Promise<AdminCourseListResult> {
    const { data } = await courseApi.getApiV1Course({
      ...params,
      page: params.page ?? 1,
      pageSize: params.pageSize ?? DEFAULT_PAGE_SIZE,
    });

    const courses = (data.data ?? []).map((course) =>
      mapCourseSummary(course as LiveCourseSummaryDto),
    );

    return {
      courses,
      total: data.total ?? courses.length,
      page: data.page ?? 1,
      pageSize: data.page_size ?? DEFAULT_PAGE_SIZE,
    };
  },

  async getAdminCourseSummary(): Promise<AdminCourseSummary> {
    const { data } = await courseApi.getApiV1CourseAdminSummary();

    return {
      totalCourses: data.total_courses ?? 0,
      publishedCourses: data.published_courses ?? 0,
      draftCourses: data.draft_courses ?? 0,
      activeStudents: data.active_students ?? 0,
      totalEnrollments: data.total_enrollments ?? 0,
    };
  },

  async createCourse(input: CreateCourseRequest) {
    const courseBody: PostApiV1CourseBody = {
      title: input.title,
      description: input.description,
      category_id: input.categoryId,
      sub_category_id: input.subCategoryId,
      difficulty_level_id: input.difficultyLevelId,
      duration_minute: Number(input.durationHours) * HOURS_TO_MINUTES,
      thumbnail_img: firstFile(input.thumbnailFile),
      is_drafted: input.isDrafted,
    };
    const courseResponse = await courseApi.postApiV1Course(courseBody);
    const courseId = courseResponse.data?.id;

    if (!courseId) {
      throw new Error(courseServiceStrings.error.courseIdNotReturned);
    }

    const contentPromises = input.topics.map((topic) => {
      const contentBody: PostApiV1CourseContentBody = {
        course_id: courseId,
        meta_topic: topic.name,
        meta_topic_order: Number(topic.metaTopicOrder),
        meta_duration_minute: Number(topic.metaTopicDuration),
        title: topic.contentTitle,
        content_type_id: topic.contentTypeId,
        video_url: topic.videoUrl || undefined,
        pdf_file: firstFile(topic.contentFile),
        sequence_order: Number(topic.sequenceOrder),
      };
      return courseContentApi.postApiV1CourseContent(contentBody);
    });

    await Promise.all(contentPromises);
    return { id: courseId };
  },

  async getCourseOverview(courseId: string): Promise<CourseOverviewItem> {
    const { data } = await courseApi.getApiV1CourseIdOverview(courseId);
    return mapCourseOverview(data);
  },

  async getForumMessages(courseId: string): Promise<ForumMessage[]> {
    const { data } = await courseApi.getApiV1CourseCourseIdForumMessage(courseId);
    return (data ?? []).map(mapForumMessage);
  },

  async postForumMessage(courseId: string, messageText: string): Promise<void> {
    await courseApi.postApiV1CourseCourseIdForumMessage(courseId, { message_text: messageText });
  },

  async getCourseForEdit(courseId: string): Promise<CourseEditData> {
    const cacheBuster = { params: { _t: Date.now() } };
    const [overviewRes, contentRes] = await Promise.all([
      courseApi.getApiV1CourseIdOverview(courseId, cacheBuster),
      courseApi.getApiV1CourseIdContent(courseId, cacheBuster),
    ]);

    type ContentInfo = { contentTypeId: string; contentTypeName: string; videoUrl?: string; pdfFilePath?: string; contentId?: string; metaSequenceOrder?: number; metaDurationMinute?: number };
    const contentTypeMap = new Map<string, ContentInfo>();
    const topicsList = (contentRes.data as any)?.topic ?? contentRes.data?.topic ?? [];
    for (const topic of topicsList) {
      const firstContent = (topic.contents ?? [])[0];
      if (topic.id) {
        const ct = firstContent?.content_type;
        let contentTypeName = "";
        let contentTypeId = "";
        if (typeof ct === "string") {
          contentTypeName = ct;
        } else if (ct && typeof ct === "object") {
          contentTypeName = (ct as any).name ?? "";
          contentTypeId = (ct as any).id ?? "";
        }
        contentTypeMap.set(topic.id, {
          contentTypeId,
          contentTypeName,
          videoUrl: firstContent?.video_url ?? undefined,
          pdfFilePath: firstContent?.pdf_file_path ?? undefined,
          contentId: firstContent?.id ?? undefined,
          metaSequenceOrder: topic.meta_sequence_order,
          metaDurationMinute: topic.meta_duration_minute,
        });
      }
    }

    return mapCourseForEdit(overviewRes.data, contentTypeMap);
  },

  async updateCourse(courseId: string, input: CreateCourseRequest): Promise<void> {
    const thumbnailFile = firstFile(input.thumbnailFile);

    const courseBody: PutApiV1CourseCourseIdBody = {
      title: input.title,
      description: input.description,
      category_id: input.categoryId,
      sub_category_id: input.subCategoryId,
      difficulty_level_id: input.difficultyLevelId,
      duration_minute: Number(input.durationHours) * HOURS_TO_MINUTES,
      is_drafted: input.isDrafted,
    };
    if (thumbnailFile) {
      courseBody.thumbnail_img = thumbnailFile;
    }
    await courseApi.putApiV1CourseCourseId(courseId, courseBody);

    for (const contentId of input.deletedContentIds ?? []) {
      let deleted = false;
      try {
        await courseContentApi.deleteApiV1CourseContentId(contentId);
        deleted = true;
      } catch (err: any) {
        if (err?.response?.status === 404) {
          deleted = true; // already deleted
        }
      }
      if (deleted) {
        addDeletedContentIds(courseId, [contentId]);
      }
    }

    for (const topic of input.topics) {
      const createBody = () => {
        const body: PostApiV1CourseContentBody = {
          course_id: courseId,
          meta_topic: topic.name,
          meta_topic_order: Number(topic.metaTopicOrder),
          meta_duration_minute: Number(topic.metaTopicDuration),
          title: topic.contentTitle,
          content_type_id: topic.contentTypeId,
          video_url: topic.videoUrl ?? '',
          pdf_file: firstFile(topic.contentFile),
          sequence_order: Number(topic.sequenceOrder),
        };
        return body;
      };

      if (topic.contentId) {
        try {
          await courseContentApi.putApiV1CourseContentId(topic.contentId, createBody());
          removeDeletedContentIds(courseId, [topic.contentId]);
        } catch {
          // Content not found (previously deleted / stale overview data). Skip this topic.
          addDeletedContentIds(courseId, [topic.contentId]);
        }
      } else {
        await courseContentApi.postApiV1CourseContent(createBody());
      }
    }
  },

  async deleteCourse(courseId: string): Promise<void> {
    await courseApi.deleteApiV1CourseCourseId(courseId);
  },
};

const mapCourseOverview = (dto: CourseOverviewDto): CourseOverviewItem => {
  const activeTopics = (dto.topic ?? []).filter(t => {
    const hasContent = (t.contents ?? []).length > 0;
    if (!hasContent) return false;
    const firstContentId = (t.contents ?? [])[0]?.id;
    if (!firstContentId) return true;
    return !getDeletedContentIds(dto.id ?? "").includes(firstContentId);
  });

  // Group topics with the same name and merge their contents
  const grouped = new Map<string, CourseOverviewMetaTopicDto>();
  for (const t of activeTopics) {
    const key = t.name ?? "";
    if (grouped.has(key)) {
      const existing = grouped.get(key)!;
      existing.contents = [...(existing.contents ?? []), ...(t.contents ?? [])];
      existing.meta_duration_minute = (existing.meta_duration_minute ?? 0) + (t.meta_duration_minute ?? 0);
    } else {
      grouped.set(key, { ...t });
    }
  }

  return {
    id: dto.id ?? "",
    title: dto.title ?? FALLBACK_COURSE_TITLE,
    description: dto.description ?? "",
    category: dto.category?.name ?? FALLBACK_CATEGORY,
    subCategory: dto.sub_category?.name ?? "",
    difficulty: dto.difficulty_level?.name ?? FALLBACK_NOT_SET,
    durationMinute: dto.duration_minute ?? 0,
    rating: dto.overall_rating ?? 0,
    thumbnailUrl: resolveThumbnailUrl(dto.thumbnail_img),
    isDrafted: dto.is_drafted ?? false,
    createdBy: dto.created_by_user?.full_name ?? FALLBACK_UNKNOWN,
    dateCreated: dto.date_created ?? "",
    dateUpdated: dto.date_updated ?? "",
    enrollmentCount: dto.enrollment_count ?? 0,
    assessmentTitle: dto.assessment_title ?? "",
    totalMark: dto.total_mark ?? 0,
    passingMark: dto.passing_mark ?? 0,
    topics: Array.from(grouped.values()).map(mapCourseOverviewTopic),
  };
};

const mapCourseOverviewTopic = (topic: CourseOverviewMetaTopicDto): CourseOverviewTopic => ({
  id: topic.id ?? "",
  name: topic.name ?? FALLBACK_TOPIC_TITLE,
  sequenceOrder: topic.meta_sequence_order ?? 0,
  durationMinute: topic.meta_duration_minute ?? 0,
  contents: (topic.contents ?? []).map((c) => ({
    id: c.id ?? "",
    metaTopicId: c.meta_topic_id ?? "",
    sequenceOrder: c.sequence_order ?? 0,
    title: c.title ?? FALLBACK_CONTENT_TITLE,
  })),
});

const mapForumMessage = (dto: ForumMessageDto): ForumMessage => ({
  id: dto.id ?? "",
  courseId: dto.course_id ?? "",
  userId: dto.user_id ?? "",
  userFullName: dto.user_full_name ?? FALLBACK_USER,
  messageText: dto.message_text ?? "",
  dateCreated: dto.date_created ?? "",
});

const mapCourseForEdit = (
  dto: CourseOverviewDto,
  contentTypeMap: Map<string, { contentTypeId: string; contentTypeName: string; videoUrl?: string; pdfFilePath?: string; contentId?: string; metaSequenceOrder?: number; metaDurationMinute?: number }> = new Map(),
): CourseEditData => {
  const topics = (dto.topic ?? []).filter(t => {
    const hasContent = (t.contents ?? []).length > 0;
    if (!hasContent) return false;
    const firstContentId = (t.contents ?? [])[0]?.id;
    if (!firstContentId) return true;
    return !getDeletedContentIds(dto.id ?? "").includes(firstContentId);
  }).flatMap((t) => {
    const contentInfo = contentTypeMap.get(t.id ?? "");
    return (t.contents ?? []).map((content, index) => ({
      name: t.name ?? "",
      contentTitle: content.title ?? t.name ?? "",
      metaTopicOrder: content.sequence_order ?? contentInfo?.metaSequenceOrder ?? t.meta_sequence_order ?? 0,
      metaTopicDuration: contentInfo?.metaDurationMinute ?? t.meta_duration_minute ?? 0,
      sequenceOrder: content.sequence_order ?? t.meta_sequence_order ?? 0,
      contentTypeId: contentInfo?.contentTypeId ?? "",
      contentTypeName: contentInfo?.contentTypeName ?? "",
      videoUrl: contentInfo?.videoUrl,
      pdfFilePath: contentInfo?.pdfFilePath,
      contentId: content.id ?? contentInfo?.contentId,
    }));
  });

  return {
    id: dto.id ?? "",
    title: dto.title ?? "",
    description: dto.description ?? "",
    categoryId: dto.category?.id ?? "",
    subCategoryId: dto.sub_category?.id ?? "",
    difficultyLevelId: dto.difficulty_level?.id ?? "",
    durationHours: Math.round((dto.duration_minute ?? 0) / 60),
    thumbnailUrl: resolveThumbnailUrl(dto.thumbnail_img),
    isDrafted: dto.is_drafted ?? false,
    createdBy: dto.created_by_user?.full_name ?? FALLBACK_UNKNOWN,
    topics,
  };
};
