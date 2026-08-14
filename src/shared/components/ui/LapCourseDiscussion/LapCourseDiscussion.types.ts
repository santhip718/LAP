export interface ForumMessage {
  id: string;
  courseId: string;
  userId: string;
  userFullName: string;
  messageText: string;
  dateCreated: string;
}

export interface UseForumMessagesResult {
  messages: ForumMessage[];
  loading: boolean;
  error: string | null;
  refresh: () => void;
  sendMessage: (messageText: string) => Promise<void>;
  sending: boolean;
}

export interface LapCourseDiscussionProps {
  courseId: string;
}
