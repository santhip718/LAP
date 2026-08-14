import { FALLBACK_EMPTY } from "./constants";

export const getDifficultyColor = (difficulty: string): string => {
  const normalized = difficulty.toLowerCase();
  if (normalized.includes("beginner")) return "var(--success)";
  if (normalized.includes("intermediate")) return "var(--secondary)";
  if (normalized.includes("expert") || normalized.includes("advanced")) return "var(--danger)";
  return "var(--primary)";
};

export const formatDuration = (minutes: number): string => {
  if (minutes < 60) return `${minutes}m`;
  const hours = Math.floor(minutes / 60);
  const remainingMinutes = minutes % 60;
  return remainingMinutes ? `${hours}h ${remainingMinutes}m` : `${hours}h`;
};

export const formatDate = (dateStr: string): string => {
  if (!dateStr) return FALLBACK_EMPTY;
  try {
    return new Date(dateStr).toLocaleDateString("en-US", {
      year: "numeric",
      month: "short",
      day: "numeric",
    });
  } catch {
    return dateStr;
  }
};
