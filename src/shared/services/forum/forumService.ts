import apiClient from "@/shared/services/api/config/axios";
import { getCourse } from "@/shared/services/api/services/course/course";
import type { ForumMessageDto } from "@/shared/services/api/models/forumMessageDto";
import type { CreateForumMessageRequestDto } from "@/shared/services/api/models/createForumMessageRequestDto";

const courseApi = getCourse(apiClient);

const FALLBACK_USER = "Unknown User";

const mapForumMessage = (dto: ForumMessageDto) => ({
  id: dto.id ?? "",
  courseId: dto.course_id ?? "",
  userId: dto.user_id ?? "",
  userFullName: dto.user_full_name ?? FALLBACK_USER,
  messageText: dto.message_text ?? "",
  dateCreated: dto.date_created ?? "",
});

export const forumService = {
  async getForumMessages(courseId: string) {
    const { data } = await courseApi.getApiV1CourseCourseIdForumMessage(courseId);
    return (data ?? []).map(mapForumMessage);
  },

  async postForumMessage(courseId: string, messageText: string) {
    const body: CreateForumMessageRequestDto = { message_text: messageText };
    await courseApi.postApiV1CourseCourseIdForumMessage(courseId, body);
  },
};
