import apiClient from "@/shared/services/api/config/axios";
import { getCourse } from "@/shared/services/api/services/course/course";
import type { CourseDetail } from "../types/courseDetailService.types";
import type {
  CourseOverviewDto,
  CourseOverviewMetaTopicDto,
  CourseContentProgressDto,
} from "@/shared/services/api/models";
import type {
  LapTopic,
  LapContent,
} from "@/shared/types/ui.types";
import { CONTENT_TYPES } from "@/features/user/constants/constants";

export type { CourseDetail };

const courseApi = getCourse(apiClient);

export async function getCourseProgress(id: string): Promise<number> {
  try {
    const response = await courseApi.getApiV1CourseIdProgress(id);
    return response.data.progress_percentage ?? 0;
  } catch {
    return 0;
  }
}

function mapContent(dto: CourseContentProgressDto): LapContent {
  return {
    id: dto.id!,
    title: dto.title!,
    contentType: {
      id: dto.content_type?.id ?? "",
      name:
        dto.content_type?.name === CONTENT_TYPES.PDF
          ? CONTENT_TYPES.PDF
          : CONTENT_TYPES.VIDEO,
    },
    videoUrl: dto.video_url ?? undefined,
    pdfFilePath: dto.pdf_file_path ?? undefined,
    durationMinute: dto.meta_duration_minute ?? 0,
    sequenceOrder: dto.sequence_order ?? 0,
  };
}

function mapTopic(dto: CourseOverviewMetaTopicDto): LapTopic {
  return {
    id: dto.id!,
    name: dto.name!,
    sequenceOrder: dto.sequence_order ?? 0,
    metaSequenceOrder: dto.meta_sequence_order ?? dto.sequence_order ?? 0,
    durationMinute: dto.duration_minute ?? 0,
    contents: ((dto.contents ?? []) as unknown as CourseContentProgressDto[])
      .map(mapContent)
      .sort(
        (a: LapContent, b: LapContent) => a.sequenceOrder - b.sequenceOrder,
      ),
  };
}

export async function getCourseOverview(id: string): Promise<CourseDetail> {
  const response = await courseApi.getApiV1CourseIdOverview(id);
  const dto: CourseOverviewDto = response.data;
  return {
    id: dto.id!,
    title: dto.title!,
    category: {
      id: dto.category?.id ?? "",
      name: dto.category?.name ?? "",
    },
    difficultyLevel: {
      id: dto.difficulty_level?.id ?? "",
      name: dto.difficulty_level?.name ?? "",
    },
    durationMinute: dto.duration_minute ?? 0,
    overallRating: dto.overall_rating ?? 0,
    thumbnailImgPath: dto.thumbnail_img ?? "",
    status: !dto.is_drafted,
    description: dto.description!,
    createdByUser: {
      id: dto.created_by_user!.id!,
      fullName: dto.created_by_user!.full_name!,
      email: dto.created_by_user!.email!,
      roles: dto.created_by_user?.roles ?? [],
    },
    topics: (dto.topic ?? [])
      .map(mapTopic)
      .sort((a: LapTopic, b: LapTopic) => a.metaSequenceOrder - b.metaSequenceOrder),
    enrollmentCount: dto.enrollment_count ?? 0,
    assessmentTitle: dto.assessment_title!,
    totalMark: dto.total_mark ?? 0,
    passingMark: dto.passing_mark ?? 0,
  };
}
