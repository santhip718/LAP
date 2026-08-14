import "@testing-library/jest-dom/jest-globals";
import { render, screen } from "@testing-library/react";
import LeaderboardPage from "@/features/leaderboard/pages/LeaderboardPage";

const mockUseLeaderboard = jest.fn();

jest.mock("@/features/leaderboard/hooks/useLeaderboard", () => ({
  useLeaderboard: (...args: unknown[]) => mockUseLeaderboard(...args),
}));

jest.mock("@/shared/components/ui/LapSpinnerv1/LapSpinnerv1", () => () => <div>Loading spinner</div>);
jest.mock("@/shared/components/feedback/LapErrorBoundary/LapErrorBoundary", () => ({ children }: { children: React.ReactNode }) => <>{children}</>);
jest.mock("@/shared/components/ui/LapLeaderboardStats/LeaderboardStats", () => () => <div>LeaderboardStats</div>);
jest.mock("@/shared/components/ui/LapLeaderboardPodium/LeaderboardPodium", () => () => <div>LeaderboardPodium</div>);
jest.mock("@/shared/components/ui/LapNoContent/LapNoContent", () => ({ title, message, children }: { title?: string; message?: string; children?: React.ReactNode }) => (
  <div>
    <div>{title}</div>
    <div>{message}</div>
    {children}
  </div>
));
jest.mock("@/features/leaderboard/utils/leaderboardTableConfig", () => ({
  leaderboardColumns: [],
}));

jest.mock("@/shared/components/ui/LapDataTable/LapDataTable", () => () => <div>LapDataTable</div>);

beforeEach(() => {
  jest.clearAllMocks();
});

describe("LeaderboardPage", () => {
  it("renders spinner while loading", () => {
    mockUseLeaderboard.mockReturnValue({ leaderboard: [], loading: true, error: null, refetch: jest.fn() });
    render(<LeaderboardPage />);
    expect(screen.getByText("Loading spinner")).toBeInTheDocument();
  });

  it("renders error state", () => {
    mockUseLeaderboard.mockReturnValue({ leaderboard: [], loading: false, error: new Error("Fail"), refetch: jest.fn() });
    render(<LeaderboardPage />);
    expect(screen.getByText("Error")).toBeInTheDocument();
    expect(screen.getByText("Unable to load leaderboard")).toBeInTheDocument();
    expect(screen.getByText("Retry")).toBeInTheDocument();
  });

  it("renders empty state", () => {
    mockUseLeaderboard.mockReturnValue({ leaderboard: [], loading: false, error: null, refetch: jest.fn() });
    render(<LeaderboardPage />);
    expect(screen.getByText("Leaderboard")).toBeInTheDocument();
    expect(screen.getByText("No leaderboard data available")).toBeInTheDocument();
  });

  it("renders normal state with data", () => {
    const mockData = [{ user_id: "1", full_name: "Alice", overall_weighted_score: 95, rank: 1, tier_awarded: "Gold" }];
    mockUseLeaderboard.mockReturnValue({ leaderboard: mockData, loading: false, error: null, refetch: jest.fn() });
    render(<LeaderboardPage />);
    expect(screen.getByText("Leaderboard")).toBeInTheDocument();
    expect(screen.getByText("LeaderboardStats")).toBeInTheDocument();
    expect(screen.getByText("LeaderboardPodium")).toBeInTheDocument();
    expect(screen.getByText("LapDataTable")).toBeInTheDocument();
  });
});
