export const courseDiscussionStrings = {
  loading: "Loading discussion...",
  error: {
    loadFailed: "Failed to load messages.",
    sendFailed: "Failed to send message. Please try again.",
  },
  retry: "Retry",
  empty: {
    title: "No messages yet",
    subtitle: "Start the discussion by sending the first message.",
  },
  input: {
    placeholder: "Type your message...",
    hint: "Press Enter to send, Shift+Enter for new line",
    sendAriaLabel: "Send message",
  },
} as const;

export const courseIdMissingError = "Course ID is missing.";

export const AVATAR_COLORS = [
  "#4f46e5",
  "#0891b2",
  "#059669",
  "#d97706",
  "#dc2626",
  "#7c3aed",
  "#db2777",
  "#2563eb",
] as const;

export const timeStrings = {
  justNow: "Just now",
  minutesAgo: "m ago",
  hoursAgo: "h ago",
  daysAgo: "d ago",
} as const;
