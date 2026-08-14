import type { CourseOverviewItem, ForumMessage } from "./courseServiceTypes";
import type { CourseStatusFilter } from "./statusTypes";

export interface UseCourseOverviewResult {
  course: CourseOverviewItem | null;
  loading: boolean;
  error: string | null;
  refresh: () => void;
}

export interface UseAdminCoursesParams {
  search: string;
  status: CourseStatusFilter;
}

export interface UseForumMessagesResult {
  messages: ForumMessage[];
  loading: boolean;
  error: string | null;
  refresh: () => void;
  sendMessage: (messageText: string) => Promise<void>;
  sending: boolean;
}
