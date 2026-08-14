import { useCallback, useEffect, useRef, useState } from "react";
import { courseService } from "../services/courseService";
import type { ForumMessage, UseForumMessagesResult } from "../types";
import { courseOverviewStrings, courseDiscussionStrings } from "../pages/CourseOverview/CourseOverview.constants";

export function useForumMessages(courseId: string | undefined): UseForumMessagesResult {
  const [messages, setMessages] = useState<ForumMessage[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [sending, setSending] = useState(false);
  const isMountedRef = useRef(true);

  const fetchMessages = useCallback(async () => {
    if (!courseId) {
      if (isMountedRef.current) {
        setError(courseOverviewStrings.error.courseIdMissing);
        setLoading(false);
      }
      return;
    }

    setLoading(true);
    setError(null);

    try {
      const result = await courseService.getForumMessages(courseId);
      if (isMountedRef.current) {
        setMessages(result);
      }
    } catch (err) {
      console.error("Failed to load forum messages:", err);
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
        await courseService.postForumMessage(courseId, messageText);
        await fetchMessages();
      } catch (err) {
        console.error("Failed to send message:", err);
        throw err;
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
