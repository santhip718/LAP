import "@testing-library/jest-dom";
import { render, screen, fireEvent } from "@testing-library/react";
import AssessmentResult from "@/features/user/pages/AssessmentResult/AssessmentResult";

const mockNavigate = jest.fn();
const mockLocationState: Record<string, unknown> = {};

jest.mock("react-router-dom", () => ({
  useParams: () => ({ courseId: "course-1" }),
  useNavigate: () => mockNavigate,
  useLocation: () => ({ state: mockLocationState }),
}));

describe("AssessmentResult", () => {
  beforeEach(() => {
    jest.clearAllMocks();
    Object.assign(mockLocationState, {
      total_question: 10,
      correct_answer: 7,
      score: 70,
      weighted_score: 70,
      passed: true,
      completed_on: "2025-06-15T10:30:00Z",
      duration_taken_minutes: 15,
      weak_topic: [{ topic_name: "React Hooks", average_score: 50 }],
      answers: [
        {
          question_id: "q1",
          question_text: "What is JSX?",
          selected_answer: "A syntax extension",
          is_correct: true,
          obtained_score: 10,
        },
        {
          question_id: "q2",
          question_text: "What is state?",
          selected_answer: "A prop",
          is_correct: false,
          obtained_score: 0,
        },
      ],
    });
  });

  it("shows not available when no result data", () => {
    Object.keys(mockLocationState).forEach(
      (k) => delete (mockLocationState as Record<string, unknown>)[k],
    );
    render(<AssessmentResult />);
    expect(screen.getByText("Result data not available.")).toBeInTheDocument();
  });

  it("renders passed summary for passed assessment", async () => {
    render(<AssessmentResult />);
    expect(screen.getByText("Assessment Passed!")).toBeInTheDocument();
    expect(screen.getAllByText("check_circle").length).toBeGreaterThanOrEqual(1);
  });

  it("renders failed summary for failed assessment", () => {
    mockLocationState.passed = false;
    render(<AssessmentResult />);
    expect(screen.getByText("Assessment Not Passed")).toBeInTheDocument();
    expect(screen.getAllByText("cancel").length).toBeGreaterThanOrEqual(1);
  });

  it("shows total score", () => {
    render(<AssessmentResult />);
    expect(screen.getByText("70")).toBeInTheDocument();
  });

  it("shows accuracy percentage", () => {
    render(<AssessmentResult />);
    expect(screen.getByText("70%")).toBeInTheDocument();
  });

  it("shows time taken", () => {
    render(<AssessmentResult />);
    expect(screen.getByText("15 min")).toBeInTheDocument();
  });

  it("shows weak topics", () => {
    render(<AssessmentResult />);
    expect(screen.getByText("React Hooks")).toBeInTheDocument();
    expect(screen.getByText("50%")).toBeInTheDocument();
  });

  it("shows answer review toggle", () => {
    render(<AssessmentResult />);
    expect(screen.getByText("Review Your Answers")).toBeInTheDocument();
    fireEvent.click(screen.getByText("Review Your Answers"));
    expect(screen.getByText("Hide Answer Review")).toBeInTheDocument();
  });

  it("shows answers in review panel", () => {
    render(<AssessmentResult />);
    fireEvent.click(screen.getByText("Review Your Answers"));
    expect(screen.getByText("What is JSX?")).toBeInTheDocument();
    expect(screen.getByText("What is state?")).toBeInTheDocument();
  });

  it("shows back to course button", () => {
    render(<AssessmentResult />);
    const buttons = screen.getAllByText("Back to Course");
    expect(buttons.length).toBeGreaterThanOrEqual(2);
  });
});
