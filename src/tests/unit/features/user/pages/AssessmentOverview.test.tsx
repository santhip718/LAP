import "@testing-library/jest-dom";
import { render, screen, fireEvent } from "@testing-library/react";
import AssessmentOverviewPage from "@/features/user/pages/AssessmentOverview/AssessmentOverview";

const mockNavigate = jest.fn();
jest.mock("react-router-dom", () => ({
  useParams: () => ({ courseId: "course-1" }),
  useNavigate: () => mockNavigate,
}));

const mockGetAssessmentOverview = jest.fn().mockResolvedValue({
  id: "assess-1",
  title: "React Basics Test",
  totalMark: 100,
  passingMark: 60,
  durationMinute: 30,
  course: {
    id: "course-1",
    title: "React Fundamentals",
    difficultyLevel: { id: "lvl-1", name: "Intermediate" },
  },
});

jest.mock("@/features/user/services/assessmentService", () => ({
  getAssessmentOverview: (...args: unknown[]) =>
    mockGetAssessmentOverview(...args),
}));

const mockGetCourseProgress = jest.fn().mockResolvedValue(80);

jest.mock("@/features/user/services/courseDetailService", () => ({
  getCourseProgress: (...args: unknown[]) => mockGetCourseProgress(...args),
}));

const mockEnrollmentState = { enrolledCourses: {} as Record<string, unknown> };

jest.mock("@/core/providers/EnrollmentProvider", () => ({
  useEnrollment: () => mockEnrollmentState,
}));

describe("AssessmentOverviewPage", () => {
  beforeEach(() => {
    jest.clearAllMocks();
    mockGetAssessmentOverview.mockResolvedValue({
      id: "assess-1",
      title: "React Basics Test",
      totalMark: 100,
      passingMark: 60,
      durationMinute: 30,
      course: {
        id: "course-1",
        title: "React Fundamentals",
        difficultyLevel: { id: "lvl-1", name: "Intermediate" },
      },
    });
    mockGetCourseProgress.mockResolvedValue(80);
    mockEnrollmentState.enrolledCourses = {
      "course-1": { id: "enroll-1", courseId: "course-1", status: true },
    };
  });

  it("shows loading state initially", () => {
    render(<AssessmentOverviewPage />);
    expect(screen.getByText("Loading assessment...")).toBeInTheDocument();
  });

  it("renders assessment title after loading", async () => {
    render(<AssessmentOverviewPage />);
    expect(await screen.findByText("React Basics Test")).toBeInTheDocument();
  });

  it("renders rules section", async () => {
    render(<AssessmentOverviewPage />);
    expect(
      await screen.findByText("Quiz Rules & Instructions"),
    ).toBeInTheDocument();
  });

  it("shows difficulty level", async () => {
    render(<AssessmentOverviewPage />);
    expect(await screen.findByText("Intermediate")).toBeInTheDocument();
  });

  it("shows total time", async () => {
    render(<AssessmentOverviewPage />);
    expect(await screen.findByText("30 Mins")).toBeInTheDocument();
  });

  it("shows passing score percentage", async () => {
    render(<AssessmentOverviewPage />);
    expect(await screen.findByText("60")).toBeInTheDocument();
  });

  it("shows total points", async () => {
    render(<AssessmentOverviewPage />);
    expect(await screen.findByText("100")).toBeInTheDocument();
  });

  it("begin button is disabled until checkbox is checked", async () => {
    render(<AssessmentOverviewPage />);
    const btn = await screen.findByText("Begin Assessment");
    expect(btn).toBeDisabled();

    fireEvent.click(screen.getByRole("checkbox"));
    expect(btn).not.toBeDisabled();
  });

  it("navigates to test route on begin", async () => {
    render(<AssessmentOverviewPage />);
    const checkbox = await screen.findByRole("checkbox");
    fireEvent.click(checkbox);
    fireEvent.click(screen.getByText("Begin Assessment"));
    expect(mockNavigate).toHaveBeenCalledWith(
      "/course-content/course-1/assessment/test",
    );
  });

  it("shows enrollment required when not enrolled", async () => {
    mockEnrollmentState.enrolledCourses = {};

    render(<AssessmentOverviewPage />);
    expect(
      await screen.findByText(
        "You need to be enrolled with active status to take this assessment.",
      ),
    ).toBeInTheDocument();
  });

  it("shows progress info when below unlock threshold", async () => {
    mockGetCourseProgress.mockResolvedValue(50);

    render(<AssessmentOverviewPage />);
    expect(await screen.findByText(/of the course/i)).toBeInTheDocument();
  });

  it("shows error state when assessment not found", async () => {
    mockGetAssessmentOverview.mockResolvedValue(null);

    render(<AssessmentOverviewPage />);
    expect(
      await screen.findByText("Assessment not found."),
    ).toBeInTheDocument();
  });
});
