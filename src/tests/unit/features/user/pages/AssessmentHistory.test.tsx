import "@testing-library/jest-dom";
import { render, screen } from "@testing-library/react";
import AssessmentHistory from "@/features/user/pages/AssessmentHistory/AssessmentHistory";

const mockNavigate = jest.fn();
jest.mock("react-router-dom", () => ({
  useNavigate: () => mockNavigate,
}));

jest.mock("@/features/auth/utils/authUtils", () => ({
  getCurrentUser: jest.fn().mockReturnValue({ id: "user-1", name: "John" }),
}));

const mockItems: Array<{ assessment_history_id: string; assessment_title: string; passed: boolean; score: number; course_title: string; attempted_on: string; course_id: string; assessment_id: string }> = [];

const mockUseInfiniteScrollConfig = {
  items: mockItems,
  loading: false,
  hasMore: false,
  sentinelRef: { current: null },
};

const mockUseInfiniteScroll = jest.fn();
mockUseInfiniteScroll.mockReturnValue(mockUseInfiniteScrollConfig);

jest.mock("@/shared/hooks", () => ({
  useInfiniteScroll: (...args: unknown[]) => mockUseInfiniteScroll(...args),
}));

jest.mock(
  "@/shared/components/ui/LapSpinnerv1/LapSpinnerv1",
  () => () => <div data-testid="spinner">Loading...</div>,
);

jest.mock(
  "@/shared/components/ui/LapNoContent/LapNoContent",
  () => ({ title, message, icon, children }: { title: string; message: string; icon?: string; children?: React.ReactNode }) => (
    <div data-testid="no-content">
      {icon && <span data-testid="empty-icon">{icon}</span>}
      <div data-testid="empty-title">{title}</div>
      <div data-testid="empty-message">{message}</div>
      {children}
    </div>
  ),
);

jest.mock(
  "@/features/user/components/AssessmentHistoryCard/AssessmentHistoryCard",
  () => ({ item }: { item: { assessment_history_id: string; assessment_title: string } }) => (
    <div data-testid="history-card">{item.assessment_title}</div>
  ),
);

describe("AssessmentHistory", () => {
  beforeEach(() => {
    jest.clearAllMocks();
    mockItems.length = 0;
    mockUseInfiniteScrollConfig.loading = false;
    mockUseInfiniteScrollConfig.hasMore = false;
    mockUseInfiniteScroll.mockReturnValue(mockUseInfiniteScrollConfig);
  });

  it("shows loading spinner on first load", () => {
    mockUseInfiniteScroll.mockReturnValue({
      items: [],
      loading: true,
      hasMore: false,
      sentinelRef: { current: null },
    });
    render(<AssessmentHistory />);
    expect(screen.getByTestId("spinner")).toBeInTheDocument();
  });

  it("shows empty state when no items", () => {
    render(<AssessmentHistory />);
    expect(screen.getByTestId("no-content")).toBeInTheDocument();
    expect(screen.getByTestId("empty-title")).toHaveTextContent(
      "No assessments completed",
    );
    expect(screen.getByTestId("empty-message")).toHaveTextContent(
      "Complete an assessment to see your history here.",
    );
  });

  it("renders page title and subtitle", () => {
    render(<AssessmentHistory />);
    expect(screen.getByText("Assessment History")).toBeInTheDocument();
    expect(
      screen.getByText(
        "Review your past assessment results and track your progress.",
      ),
    ).toBeInTheDocument();
  });

  it("renders assessment history cards", () => {
    mockItems.push(
      {
        assessment_history_id: "h1",
        assessment_title: "Test 1",
        passed: true,
        score: 90,
        course_title: "Course 1",
        attempted_on: "2025-01-01T00:00:00Z",
        course_id: "c1",
        assessment_id: "a1",
      },
      {
        assessment_history_id: "h2",
        assessment_title: "Test 2",
        passed: false,
        score: 40,
        course_title: "Course 2",
        attempted_on: "2025-02-01T00:00:00Z",
        course_id: "c2",
        assessment_id: "a2",
      },
    );
    render(<AssessmentHistory />);
    expect(screen.getByText("Test 1")).toBeInTheDocument();
    expect(screen.getByText("Test 2")).toBeInTheDocument();
  });

  it("shows end message when no more items", () => {
    mockItems.push({
      assessment_history_id: "h1",
      assessment_title: "Test 1",
      passed: true,
      score: 90,
      course_title: "Course 1",
      attempted_on: "2025-01-01T00:00:00Z",
      course_id: "c1",
      assessment_id: "a1",
    });
    render(<AssessmentHistory />);
    expect(screen.getByText("You've reached the end")).toBeInTheDocument();
  });

  it("shows loading more indicator when hasMore is true", () => {
    mockItems.push({
      assessment_history_id: "h1",
      assessment_title: "Test 1",
      passed: true,
      score: 90,
      course_title: "Course 1",
      attempted_on: "2025-01-01T00:00:00Z",
      course_id: "c1",
      assessment_id: "a1",
    });
    mockUseInfiniteScrollConfig.loading = true;
    mockUseInfiniteScrollConfig.hasMore = true;
    render(<AssessmentHistory />);
    expect(screen.getByText("Loading more...")).toBeInTheDocument();
  });

  it("shows My Courses button in empty state", () => {
    render(<AssessmentHistory />);
    expect(screen.getByText("My Courses")).toBeInTheDocument();
  });
});
