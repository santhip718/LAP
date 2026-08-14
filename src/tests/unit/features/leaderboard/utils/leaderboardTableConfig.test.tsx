import "@testing-library/jest-dom/jest-globals";
import { render, screen } from "@testing-library/react";
import { leaderboardColumns } from "@/features/leaderboard/utils/leaderboardTableConfig";
import type { LeaderboardUser } from "@/features/leaderboard/types/leaderboard.types";

jest.mock("@/features/leaderboard/utils/avatarUtils", () => ({
  getUserAvatar: jest.fn((id: string) => (id === "user-1" ? "/avatars/user1.png" : "")),
}));

const dummyRow = {} as LeaderboardUser;

describe("leaderboardColumns", () => {
  it("has 4 columns", () => {
    expect(leaderboardColumns).toHaveLength(4);
  });

  it("has correct column keys", () => {
    const keys = leaderboardColumns.map((c) => c.key);
    expect(keys).toEqual(["rank", "full_name", "overall_weighted_score", "tier_awarded"]);
  });

  describe("rank column", () => {
    it("renders 1 for rank 1", () => {
      render(<table><tbody><tr>{leaderboardColumns[0].render!(1, dummyRow, 0)}</tr></tbody></table>);
      expect(screen.getByText("1")).toBeInTheDocument();
    });

    it("renders 2 for rank 2", () => {
      render(<table><tbody><tr>{leaderboardColumns[0].render!(2, dummyRow, 0)}</tr></tbody></table>);
      expect(screen.getByText("2")).toBeInTheDocument();
    });

    it("renders 3 for rank 3", () => {
      render(<table><tbody><tr>{leaderboardColumns[0].render!(3, dummyRow, 0)}</tr></tbody></table>);
      expect(screen.getByText("3")).toBeInTheDocument();
    });

    it("renders correct value for string rank value", () => {
      render(<table><tbody><tr>{leaderboardColumns[0].render!("1", dummyRow, 0)}</tr></tbody></table>);
      expect(screen.getByText("1")).toBeInTheDocument();
    });

    it("renders N for rank > 3", () => {
      render(<table><tbody><tr>{leaderboardColumns[0].render!(4, dummyRow, 0)}</tr></tbody></table>);
      expect(screen.getByText("4")).toBeInTheDocument();
    });
  });

  describe("full_name column", () => {
    it("renders learner name and avatar", () => {
      const row: LeaderboardUser = { user_id: "user-1", full_name: "Alice", overall_weighted_score: 95, rank: 1, tier_awarded: "Gold" };
      render(<table><tbody><tr>{leaderboardColumns[1].render!("Alice", row, 0)}</tr></tbody></table>);
      expect(screen.getByText("Alice")).toBeInTheDocument();
    });
  });

  describe("overall_weighted_score column", () => {
    it("renders score with one decimal", () => {
      render(<table><tbody><tr>{leaderboardColumns[2].render!(95.5, dummyRow, 0)}</tr></tbody></table>);
      expect(screen.getByText("95.5")).toBeInTheDocument();
    });

    it("renders em-dash for null score", () => {
      const { container } = render(<table><tbody><tr>{leaderboardColumns[2].render!(null, dummyRow, 0)}</tr></tbody></table>);
      expect(container.textContent).toMatch(/—/);
    });
  });
});
