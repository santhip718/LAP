import "@testing-library/jest-dom/jest-globals";
import "@testing-library/jest-dom";
import { render, screen } from "@testing-library/react";
import AssessmentManagement from "@/features/admin/pages/AssessmentManagement/AssessmentManagement";
import type { AssessmentOverviewDto } from "@/shared/services/api/models/assessmentOverviewDto";

const mockUseAssessments = jest.fn();
const mockNavigate = jest.fn();

jest.mock("react-router-dom", () => ({
  useNavigate: () => mockNavigate,
}));

jest.mock("@/features/admin/hooks/useAssessments", () => ({
  useAssessments: (...args: unknown[]) => mockUseAssessments(...args),
}));

jest.mock("@/shared/components/ui/LapSpinnerv1/LapSpinnerv1", () => () => <div>Loading spinner</div>);
jest.mock("@/shared/components/feedback/LapErrorBoundary/LapErrorBoundary", () => ({ children }: { children: React.ReactNode }) => <>{children}</>);
jest.mock("@/shared/components/ui/LapDataTable/LapDataTable", () => () => <div>LapDataTable</div>);
jest.mock("@/shared/components/feedback/LapModalDialog/LapModalDialog", () => ({ open, title, children }: { open: boolean; title?: string; children?: React.ReactNode }) =>
  open ? <div><div>{title}</div><div>{children}</div></div> : null
);
jest.mock("@/features/admin/components/AssessmentForm/AssessmentForm", () => () => <div>AssessmentForm</div>);
jest.mock("@/features/admin/utils/assessmentTableConfig", () => ({
  buildAssessmentColumns: jest.fn(() => []),
}));
jest.mock("@/shared/services/feedback/feedbackService", () => ({
  feedbackService: { showToast: jest.fn() },
}));

beforeEach(() => {
  jest.clearAllMocks();
});

describe("AssessmentManagement", () => {
  it("renders spinner while loading", () => {
    mockUseAssessments.mockReturnValue({ items: [], isLoading: true, error: null, refetch: jest.fn(), deleteAssessment: jest.fn(), isDeleting: false, loadMore: jest.fn(), hasMore: false });
    render(<AssessmentManagement />);
    expect(screen.getByText("Loading spinner")).toBeInTheDocument();
  });

  it("renders error state", () => {
    mockUseAssessments.mockReturnValue({ items: [], isLoading: false, error: new Error("API error"), refetch: jest.fn(), deleteAssessment: jest.fn(), isDeleting: false, loadMore: jest.fn(), hasMore: false });
    render(<AssessmentManagement />);
    expect(screen.getByText("API error")).toBeInTheDocument();
    expect(screen.getByText("Try Again")).toBeInTheDocument();
  });

  it("renders empty state", () => {
    mockUseAssessments.mockReturnValue({ items: [], isLoading: false, error: null, refetch: jest.fn(), deleteAssessment: jest.fn(), isDeleting: false, loadMore: jest.fn(), hasMore: false });
    render(<AssessmentManagement />);
    expect(screen.getByText("Assessment Management")).toBeInTheDocument();
    expect(screen.getByText("No assessments found")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /create assessment/i })).toBeInTheDocument();
  });

  it("renders normal state with data", () => {
    const mockItems: AssessmentOverviewDto[] = [
      { id: "1", title: "Quiz 1", total_mark: 100, passing_mark: 50, duration_minute: 60 },
    ];
    mockUseAssessments.mockReturnValue({ items: mockItems, isLoading: false, error: null, refetch: jest.fn(), deleteAssessment: jest.fn(), isDeleting: false, loadMore: jest.fn(), hasMore: false });
    render(<AssessmentManagement />);
    expect(screen.getByText("Assessment Management")).toBeInTheDocument();
    expect(screen.getByText("LapDataTable")).toBeInTheDocument();
  });
});
