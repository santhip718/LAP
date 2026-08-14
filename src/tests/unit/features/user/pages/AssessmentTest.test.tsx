import "@testing-library/jest-dom";
import { render, screen, fireEvent, act } from "@testing-library/react";
import AssessmentTest from "@/features/user/pages/AssessmentTest/AssessmentTest";

const mockNavigate = jest.fn();
jest.mock("react-router-dom", () => ({
  useParams: () => ({ courseId: "course-1" }),
  useNavigate: () => mockNavigate,
}));

const mockGetAssessmentOverview = jest.fn();
const mockGetAssessmentQuestions = jest.fn();
const mockSubmitAssessment = jest.fn();

jest.mock("@/features/user/services/assessmentService", () => ({
  getAssessmentOverview: (...args: unknown[]) => mockGetAssessmentOverview(...args),
  getAssessmentQuestions: (...args: unknown[]) => mockGetAssessmentQuestions(...args),
  submitAssessment: (...args: unknown[]) => mockSubmitAssessment(...args),
}));

const mockShowConfirm = jest.fn();
const mockShowToast = jest.fn();

jest.mock("@/shared/services/feedback", () => ({
  feedbackService: {
    showConfirm: (...args: unknown[]) => mockShowConfirm(...args),
    showToast: (...args: unknown[]) => mockShowToast(...args),
  },
}));

jest.mock("@/shared/components/ui/LapSpinnerv1/LapSpinnerv1", () => () => (
  <div data-testid="spinner">Loading...</div>
));

const overviewData = {
  id: "asm-001",
  title: "React Basics Test",
  description: "Answer all questions thoroughly.",
  totalMark: 100,
  passingMark: 60,
  durationMinute: 30,
  course: { id: "course-1", title: "React", difficultyLevel: { id: "lvl-1", name: "Intermediate" } },
};

const questionsData = [
  {
    id: "q-001",
    assessmentId: "asm-001",
    metaTopicId: "mt-001",
    questionType: { id: "MCQ", name: "MCQ" },
    questionText: "What is JSX?",
    optionList: ["Option A", "Option B", "Option C"],
    weight: 10,
  },
  {
    id: "q-002",
    assessmentId: "asm-001",
    metaTopicId: "mt-002",
    questionType: { id: "TrueFalse", name: "TrueFalse" },
    questionText: "React is a framework.",
    optionList: ["True", "False"],
    weight: 5,
  },
  {
    id: "q-003",
    assessmentId: "asm-001",
    metaTopicId: "mt-003",
    questionType: { id: "FillInBlank", name: "FillInBlank" },
    questionText: "___ is a state management library.",
    optionList: [],
    weight: 5,
  },
  {
    id: "q-004",
    assessmentId: "asm-001",
    metaTopicId: "mt-004",
    questionType: { id: "Essay", name: "Essay" },
    questionText: "Write an essay.",
    optionList: [],
    weight: 20,
  },
];

const submitResult = {
  total_question: 4,
  correct_answer: 3,
  weighted_score: 20,
  passed: true,
  completed_on: "2025-06-15T10:30:00Z",
  duration_taken_minutes: 15,
  weak_topic: [],
  answers: [],
  status: "Assessment passed!",
};

async function loadAssessment() {
  mockGetAssessmentOverview.mockResolvedValue(overviewData);
  mockGetAssessmentQuestions.mockResolvedValue(questionsData);
  render(<AssessmentTest />);
  await screen.findByText("React Basics Test");
}

describe("AssessmentTest", () => {
  beforeEach(() => {
    jest.clearAllMocks();
    jest.useRealTimers();
    mockGetAssessmentOverview.mockResolvedValue(overviewData);
    mockGetAssessmentQuestions.mockResolvedValue(questionsData);
    mockSubmitAssessment.mockResolvedValue(submitResult);
    mockShowConfirm.mockResolvedValue(true);
  });

  it("shows loading spinner initially", () => {
    mockGetAssessmentOverview.mockReturnValue(new Promise(() => {}));
    render(<AssessmentTest />);
    expect(screen.getByTestId("spinner")).toBeInTheDocument();
  });

  it("shows not available when assessment is null", async () => {
    mockGetAssessmentOverview.mockResolvedValue(null);
    render(<AssessmentTest />);
    expect(await screen.findByText("Assessment not available.")).toBeInTheDocument();
  });

  it("shows not available on error", async () => {
    mockGetAssessmentOverview.mockRejectedValue(new Error("fail"));
    render(<AssessmentTest />);
    expect(await screen.findByText("Assessment not available.")).toBeInTheDocument();
  });

  it("renders assessment title and description after loading", async () => {
    await loadAssessment();
    expect(screen.getByText("React Basics Test")).toBeInTheDocument();
    expect(screen.getByText(/Answer all questions/i)).toBeInTheDocument();
  });

  it("renders page title", async () => {
    await loadAssessment();
    expect(screen.getByText("Course Assessment")).toBeInTheDocument();
  });

  it("renders question count and total marks", async () => {
    await loadAssessment();
    expect(screen.getByText("4 Questions")).toBeInTheDocument();
    expect(screen.getByText("100 Points")).toBeInTheDocument();
  });

  it("renders time limit", async () => {
    await loadAssessment();
    expect(screen.getByText("30 Minutes")).toBeInTheDocument();
  });

  it("renders quit button", async () => {
    await loadAssessment();
    expect(screen.getByText("Quit")).toBeInTheDocument();
  });

  it("renders all question types", async () => {
    await loadAssessment();
    expect(screen.getByText("Multiple Choice")).toBeInTheDocument();
    expect(screen.getByText("True / False")).toBeInTheDocument();
    expect(screen.getByText("Fill in the Blank")).toBeInTheDocument();
    expect(screen.getByText("Unsupported question type")).toBeInTheDocument();
  });

  it("shows MCQ options as radio buttons", async () => {
    await loadAssessment();
    expect(screen.getByText("Option A")).toBeInTheDocument();
    expect(screen.getByText("Option B")).toBeInTheDocument();
    expect(screen.getByText("Option C")).toBeInTheDocument();
  });

  it("selecting an answer updates answered count", async () => {
    await loadAssessment();
    expect(screen.getByText("0/4")).toBeInTheDocument();
    fireEvent.click(screen.getByLabelText("Option A"));
    expect(screen.getByText("1/4")).toBeInTheDocument();
  });

  it("selecting TrueFalse answer updates answered count", async () => {
    await loadAssessment();
    fireEvent.click(screen.getByText("True"));
    expect(await screen.findByText("1/4")).toBeInTheDocument();
  });

  it("typing in fill-in-blank updates answered count", async () => {
    await loadAssessment();
    const input = screen.getByPlaceholderText("Type your answer...");
    fireEvent.change(input, { target: { value: "Redux" } });
    expect(screen.getByText("1/4")).toBeInTheDocument();
  });

  it("submit button shows submitting state while submitting", async () => {
    mockSubmitAssessment.mockReturnValue(new Promise(() => {}));
    await loadAssessment();
    fireEvent.click(screen.getByText("Submit Assessment"));
    expect(await screen.findByText("Submitting...")).toBeInTheDocument();
  });

  it("navigates to result page after successful submit", async () => {
    await loadAssessment();
    fireEvent.click(screen.getByText("Submit Assessment"));
    await act(async () => {});
    expect(mockNavigate).toHaveBeenCalledWith(
      "/course-overview/course-1/assessment/result",
      expect.any(Object),
    );
  });

  it("shows success toast on submit", async () => {
    await loadAssessment();
    fireEvent.click(screen.getByText("Submit Assessment"));
    await act(async () => {});
    expect(mockShowToast).toHaveBeenCalledWith("Assessment passed!", "success", 5000);
  });

  it("shows error toast when submitAssessment throws", async () => {
    mockSubmitAssessment.mockRejectedValue(new Error("Network error"));
    await loadAssessment();
    fireEvent.click(screen.getByText("Submit Assessment"));
    await act(async () => {});
    expect(mockShowToast).toHaveBeenCalledWith(
      "Network error",
      "error",
    );
  });

  it("calls showConfirm when quit is clicked", async () => {
    await loadAssessment();
    fireEvent.click(screen.getByText("Quit"));
    expect(mockShowConfirm).toHaveBeenCalled();
  });

  it("navigates to overview when quit confirmed", async () => {
    mockShowConfirm.mockResolvedValue(true);
    await loadAssessment();
    fireEvent.click(screen.getByText("Quit"));
    await act(async () => {});
    expect(mockNavigate).toHaveBeenCalledWith("/course-overview/course-1");
  });

  it("does not navigate when quit cancelled", async () => {
    mockShowConfirm.mockResolvedValue(false);
    await loadAssessment();
    fireEvent.click(screen.getByText("Quit"));
    await act(async () => {});
    expect(mockNavigate).not.toHaveBeenCalled();
  });

  it("shows flag button and toggles on click", async () => {
    await loadAssessment();
    const flagBtns = screen.getAllByText("Flag for review");
    expect(flagBtns.length).toBeGreaterThanOrEqual(1);
    fireEvent.click(flagBtns[0]);
    expect(screen.getByText("Flagged")).toBeInTheDocument();
  });

  it("flagging a question updates text back on second click", async () => {
    await loadAssessment();
    const flagBtn = screen.getAllByText("Flag for review")[0];
    fireEvent.click(flagBtn);
    expect(screen.getByText("Flagged")).toBeInTheDocument();
    const flagBtnAgain = screen.getByText("Flagged");
    fireEvent.click(flagBtnAgain);
    expect(screen.getAllByText("Flag for review").length).toBeGreaterThanOrEqual(1);
  });

  it("shows weight for each question", async () => {
    await loadAssessment();
    expect(screen.getByText("Weight: 10 points")).toBeInTheDocument();
    expect(screen.getAllByText("Weight: 5 points").length).toBe(2);
  });

  it("shows submit section with answered count", async () => {
    await loadAssessment();
    expect(screen.getByText(/0 of 4 questions answered/)).toBeInTheDocument();
  });

  it("shows flagged count in submit section when flagged", async () => {
    await loadAssessment();
    const flagBtn = screen.getAllByText("Flag for review")[0];
    fireEvent.click(flagBtn);
    expect(screen.getByText(/0 of 4 questions answered \(1 flagged\)/)).toBeInTheDocument();
  });

  it("renders passing score percentage in stats", async () => {
    await loadAssessment();
    expect(screen.getByText(/60%/)).toBeInTheDocument();
  });

  it("shows digital clock when remainingTime is set", async () => {
    await loadAssessment();
    expect(await screen.findByText("30:00")).toBeInTheDocument();
  });
});

describe("AssessmentTest timer", () => {
  beforeEach(() => {
    jest.clearAllMocks();
    jest.useFakeTimers();
    mockGetAssessmentOverview.mockResolvedValue(overviewData);
    mockGetAssessmentQuestions.mockResolvedValue(questionsData);
    mockSubmitAssessment.mockResolvedValue(submitResult);
    mockShowConfirm.mockResolvedValue(true);
  });

  afterEach(() => {
    jest.useRealTimers();
  });

  it("counts down every second", async () => {
    render(<AssessmentTest />);
    await screen.findByText("React Basics Test");
    const clock = document.querySelector(".at-digital-clock-digits");
    expect(clock?.textContent).toBe("30:00");
    act(() => {
      jest.advanceTimersByTime(1000);
    });
    expect(clock?.textContent).toBe("29:59");
  });

  it("auto-submits when timer reaches zero", async () => {
    mockGetAssessmentOverview.mockResolvedValue({ ...overviewData, durationMinute: 0.0167 });
    render(<AssessmentTest />);
    await screen.findByText("React Basics Test");
    act(() => {
      jest.advanceTimersByTime(2000);
    });
    await act(async () => {});
    expect(mockSubmitAssessment).toHaveBeenCalled();
  });
});
