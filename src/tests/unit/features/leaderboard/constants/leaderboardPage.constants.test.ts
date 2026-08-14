import { LEADERBOARD_UI } from "@/features/leaderboard/pages/LeaderboardPage.constants";

describe("LeaderboardPage constants", () => {
  it("has correct PAGE_TITLE", () => {
    expect(LEADERBOARD_UI.PAGE_TITLE).toBe("Leaderboard");
  });

  it("has correct PAGE_SUBTITLE", () => {
    expect(LEADERBOARD_UI.PAGE_SUBTITLE).toBe("Track top learners and their overall weighted scores.");
  });

  it("has correct error constants", () => {
    expect(LEADERBOARD_UI.ERROR_TITLE).toBe("Error");
    expect(LEADERBOARD_UI.ERROR_MESSAGE).toBe("Unable to load leaderboard");
    expect(LEADERBOARD_UI.BTN_RETRY).toBe("Retry");
  });

  it("has correct empty state constants", () => {
    expect(LEADERBOARD_UI.EMPTY_ICON).toBe("\uD83C\uDFC6");
    expect(LEADERBOARD_UI.EMPTY_MESSAGE).toBe("No leaderboard data available");
  });

  it("has correct search and column constants", () => {
    expect(LEADERBOARD_UI.SEARCH_PLACEHOLDER).toBe("Search learners...");
    expect(LEADERBOARD_UI.COL_RANK).toBe("Rank");
    expect(LEADERBOARD_UI.COL_LEARNER).toBe("Learner");
    expect(LEADERBOARD_UI.COL_SCORE).toBe("Score");
  });
});
