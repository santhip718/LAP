import "@testing-library/jest-dom";
import { render, screen, fireEvent } from "@testing-library/react";
import LapAssessmentCard from "@/shared/components/ui/LapAssessmentCard/LapAssessmentCard";

const mockNavigate = jest.fn();
jest.mock("react-router-dom", () => ({
  useNavigate: () => mockNavigate,
}));

const assessment = {
  id: "asm-1",
  title: "Final Test",
  description: "Complete assessment",
  totalMark: 100,
  passingMark: 60,
  durationMinute: 30,
  courseId: "c1",
  course: {
    id: "c1",
    title: "React",
    category: { id: "cat-1", name: "Programming" },
    difficultyLevel: { id: "lvl-1", name: "Intermediate" },
    durationMinute: 120,
    overallRating: 4.5,
    thumbnailImg: "",
    isDrafted: false,
  },
};

describe("LapAssessmentCard", () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  it("returns null when no assessment and canResume is false", () => {
    const { container } = render(
      <LapAssessmentCard
        assessment={null}
        canAccessAssessment={false}
        canResume={false}
        completionPercent={0}
        courseId="c1"
      />,
    );
    expect(container.innerHTML).toBe("");
  });

  it("shows empty state when no assessment but canResume is true", () => {
    render(
      <LapAssessmentCard
        assessment={null}
        canAccessAssessment={false}
        canResume
        completionPercent={0}
        courseId="c1"
      />,
    );
    expect(screen.getByText("Assessment")).toBeInTheDocument();
    expect(
      screen.getByText("No assessment available for this course."),
    ).toBeInTheDocument();
  });

  it("renders assessment title and metadata", () => {
    render(
      <LapAssessmentCard
        assessment={assessment}
        canAccessAssessment={false}
        canResume
        completionPercent={50}
        courseId="c1"
      />,
    );
    expect(screen.getByText("Final Test")).toBeInTheDocument();
    expect(screen.getByText(/100 Points/)).toBeInTheDocument();
  });

  it("shows Enroll to Access when not enrolled", () => {
    render(
      <LapAssessmentCard
        assessment={assessment}
        canAccessAssessment={false}
        canResume={false}
        completionPercent={0}
        courseId="c1"
      />,
    );
    expect(screen.getByText("Enroll to Access")).toBeInTheDocument();
  });

  it("shows progress percentage when enrolled but below threshold", () => {
    render(
      <LapAssessmentCard
        assessment={assessment}
        canAccessAssessment={false}
        canResume
        completionPercent={50}
        courseId="c1"
      />,
    );
    expect(screen.getByText("50% Completed")).toBeInTheDocument();
  });

  it("shows Begin Assessment when enrolled and above threshold", () => {
    render(
      <LapAssessmentCard
        assessment={assessment}
        canAccessAssessment
        canResume
        completionPercent={80}
        courseId="c1"
      />,
    );
    expect(screen.getByText("Begin Assessment")).toBeInTheDocument();
  });

  it("navigates to assessment when Begin Assessment clicked", () => {
    render(
      <LapAssessmentCard
        assessment={assessment}
        canAccessAssessment
        canResume
        completionPercent={80}
        courseId="c1"
      />,
    );
    fireEvent.click(screen.getByText("Begin Assessment"));
    expect(mockNavigate).toHaveBeenCalledWith("/course-overview/c1/assessment");
  });

  it("disables button when canAccessAssessment is false", () => {
    render(
      <LapAssessmentCard
        assessment={assessment}
        canAccessAssessment={false}
        canResume
        completionPercent={30}
        courseId="c1"
      />,
    );
    expect(screen.getByText("30% Completed").closest("button")).toBeDisabled();
  });

  it("shows progress bar when enrolled but below threshold", () => {
    render(
      <LapAssessmentCard
        assessment={assessment}
        canAccessAssessment={false}
        canResume
        completionPercent={50}
        courseId="c1"
      />,
    );
    expect(screen.getByText("Course Progress")).toBeInTheDocument();
    expect(screen.getByText("50%").closest(".co-assessment-progress-pct")).toBeInTheDocument();
  });

  it("shows 100% progress bar when assessment is unlocked", () => {
    render(
      <LapAssessmentCard
        assessment={assessment}
        canAccessAssessment
        canResume
        completionPercent={80}
        courseId="c1"
      />,
    );
    expect(screen.getByText("Assessment unlocked!")).toBeInTheDocument();
  });

  it("shows locked message with percentage remaining", () => {
    render(
      <LapAssessmentCard
        assessment={assessment}
        canAccessAssessment={false}
        canResume
        completionPercent={50}
        courseId="c1"
      />,
    );
    expect(screen.getByText(/50% more to unlock/)).toBeInTheDocument();
  });
});
