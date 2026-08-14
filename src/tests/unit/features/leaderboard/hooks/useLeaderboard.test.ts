import { renderHook, waitFor, act } from "@testing-library/react";
import { useLeaderboard } from "@/features/leaderboard/hooks/useLeaderboard";
import { getOverallLeaderboard } from "@/features/leaderboard/services/leaderboardService";
import type { LeaderboardUser } from "@/features/leaderboard/types/leaderboard.types";

jest.mock("@/features/leaderboard/services/leaderboardService", () => ({
  getOverallLeaderboard: jest.fn(),
}));

const mockUsers: LeaderboardUser[] = [
  { user_id: "1", full_name: "Alice", overall_weighted_score: 95, rank: 1, tier_awarded: "Gold" },
  { user_id: "2", full_name: "Bob", overall_weighted_score: 85, rank: 2, tier_awarded: "Silver" },
];

beforeEach(() => {
  jest.clearAllMocks();
});

describe("useLeaderboard", () => {
  it("starts with loading true", () => {
    (getOverallLeaderboard as jest.Mock).mockReturnValue(new Promise(() => {}));
    const { result } = renderHook(() => useLeaderboard());
    expect(result.current.loading).toBe(true);
    expect(result.current.leaderboard).toEqual([]);
    expect(result.current.error).toBeNull();
  });

  it("fetches leaderboard and returns data", async () => {
    (getOverallLeaderboard as jest.Mock).mockResolvedValue(mockUsers);
    const { result } = renderHook(() => useLeaderboard(10));
    await waitFor(() => expect(result.current.loading).toBe(false));
    expect(result.current.leaderboard).toEqual(mockUsers);
    expect(result.current.error).toBeNull();
    expect(getOverallLeaderboard).toHaveBeenCalledWith(10);
  });

  it("uses default page size of 25", async () => {
    (getOverallLeaderboard as jest.Mock).mockResolvedValue([]);
    renderHook(() => useLeaderboard());
    await waitFor(() => expect(getOverallLeaderboard).toHaveBeenCalledWith(25));
  });

  it("sets error when fetch fails", async () => {
    (getOverallLeaderboard as jest.Mock).mockRejectedValue(new Error("Network error"));
    const { result } = renderHook(() => useLeaderboard());
    await waitFor(() => expect(result.current.loading).toBe(false));
    expect(result.current.leaderboard).toEqual([]);
    expect(result.current.error).toEqual(new Error("Network error"));
  });

  it("refetch re-fetches data", async () => {
    (getOverallLeaderboard as jest.Mock).mockResolvedValue(mockUsers);
    const { result } = renderHook(() => useLeaderboard());
    await waitFor(() => expect(result.current.loading).toBe(false));

    (getOverallLeaderboard as jest.Mock).mockResolvedValue([mockUsers[0]]);
    act(() => result.current.refetch());
    await waitFor(() => expect(result.current.leaderboard).toEqual([mockUsers[0]]));
  });
});
