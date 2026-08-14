import apiClient from "@/shared/services/api/config/axios";
import { getCourse } from "@/shared/services/api/services/course/course";
import type { CourseContentResponseDto } from "@/shared/services/api/models/courseContentResponseDto";
import type { CourseContentProgressDto } from "@/shared/services/api/models/courseContentProgressDto";
import type { CourseTopicProgressDto } from "@/shared/services/api/models/courseTopicProgressDto";
import type { LapTopic, LapContent } from "@/shared/types/ui.types";
import {
  CONTENT_TYPES,
  CONTENT_TYPE_PDF_KEY,
} from "@/features/user/constants/constants";

const courseApi = getCourse(apiClient);

function mapContentType(
  name: string | null | undefined,
): typeof CONTENT_TYPES.VIDEO | typeof CONTENT_TYPES.PDF {
  const lower = name?.toLowerCase() ?? "";
  if (lower.includes(CONTENT_TYPE_PDF_KEY)) return CONTENT_TYPES.PDF;
  return CONTENT_TYPES.VIDEO;
}

function mapContent(dto: CourseContentProgressDto): LapContent {
  return {
    id: dto.id!,
    title: dto.title!,
    contentType: {
      id: dto.content_type?.id ?? "",
      name: mapContentType(dto.content_type?.name),
    },
    videoUrl: dto.video_url ?? undefined,
    pdfFilePath: dto.pdf_file_path ?? undefined,
    durationMinute: dto.meta_duration_minute ?? 0,
    sequenceOrder: dto.sequence_order ?? 0,
    isCompleted: dto.is_completed ?? false,
  };
}

function mapTopic(dto: CourseTopicProgressDto): LapTopic {
  const contents = (dto.contents ?? [])
    .map((c) => mapContent(c as CourseContentProgressDto))
    .sort((a, b) => a.sequenceOrder - b.sequenceOrder);
  return {
    id: dto.id!,
    name: dto.name!,
    sequenceOrder: dto.sequence_order ?? 0,
    metaSequenceOrder: dto.meta_sequence_order ?? dto.sequence_order ?? 0,
    durationMinute: dto.duration_minute ?? 0,
    contents: contents,
    isCompleted: contents.length > 0 && contents.every((c) => c.isCompleted),
  };
}

export async function getCourseContent(id: string): Promise<{
  topics: LapTopic[];
  thumbnailImg: string;
}> {
  const response = await courseApi.getApiV1CourseIdContent(id);
  const dto = response.data as CourseContentResponseDto & {
    topic?: CourseTopicProgressDto[] | null;
  };
  const rawTopics = dto.topic ?? dto.topic ?? [];
  return {
    topics: rawTopics
      .map((t) => mapTopic(t as CourseTopicProgressDto))
      .filter((t) => t.contents.length > 0)
      .sort((a, b) => a.metaSequenceOrder - b.metaSequenceOrder),
    thumbnailImg: dto.thumbnail_img ?? "",
  };
}
