import { useCallback, useEffect, useRef, useState } from "react";
import { forumService } from "../services/forum/forumService";
import type { UseForumMessagesResult } from "../components/ui/LapCourseDiscussion/LapCourseDiscussion.types";
import { courseDiscussionStrings, courseIdMissingError } from "../components/ui/LapCourseDiscussion/LapCourseDiscussion.constants";

export function useForumMessages(courseId: string | undefined): UseForumMessagesResult {
  const [messages, setMessages] = useState<UseForumMessagesResult["messages"]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [sending, setSending] = useState(false);
  const isMountedRef = useRef(true);

  const fetchMessages = useCallback(async () => {
    if (!courseId) {
      if (isMountedRef.current) {
        setError(courseIdMissingError);
        setLoading(false);
      }
      return;
    }

    setLoading(true);
    setError(null);

    try {
      const result = await forumService.getForumMessages(courseId);
      if (isMountedRef.current) {
        setMessages(result);
      }
    } catch {
      if (isMountedRef.current) {
        setError(courseDiscussionStrings.error.loadFailed);
        setMessages([]);
      }
    } finally {
      if (isMountedRef.current) {
        setLoading(false);
      }
    }
  }, [courseId]);

  const sendMessage = useCallback(
    async (messageText: string) => {
      if (!courseId) return;

      setSending(true);
      try {
        await forumService.postForumMessage(courseId, messageText);
        await fetchMessages();
      } catch {
        throw new Error(courseDiscussionStrings.error.sendFailed);
      } finally {
        if (isMountedRef.current) {
          setSending(false);
        }
      }
    },
    [courseId, fetchMessages],
  );

  useEffect(() => {
    isMountedRef.current = true;
    queueMicrotask(() => {
      void fetchMessages();
    });
    return () => {
      isMountedRef.current = false;
    };
  }, [fetchMessages]);

  return {
    messages,
    loading,
    error,
    refresh: fetchMessages,
    sendMessage,
    sending,
  };
}
