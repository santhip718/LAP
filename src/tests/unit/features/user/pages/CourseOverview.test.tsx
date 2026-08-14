import "@testing-library/jest-dom";
import { render, screen, fireEvent, act } from "@testing-library/react";
import CourseOverview from "@/features/user/pages/CourseOverview/CourseOverview";

const mockNavigate = jest.fn();
jest.mock("react-router-dom", () => ({
  useParams: () => ({ id: "course-1" }),
  useNavigate: () => mockNavigate,
}));

const mockGetCourseOverview = jest.fn();
const mockGetCourseContent = jest.fn();
const mockGetCourseProgress = jest.fn();
const mockGetAssessmentOverview = jest.fn();
const mockGetAssessmentAttempts = jest.fn();
const mockSubmitReview = jest.fn();

jest.mock("@/features/user/services/courseDetailService", () => ({
  getCourseOverview: (...args: unknown[]) => mockGetCourseOverview(...args),
  getCourseProgress: (...args: unknown[]) => mockGetCourseProgress(...args),
}));

jest.mock("@/features/user/services/courseContentService", () => ({
  getCourseContent: (...args: unknown[]) => mockGetCourseContent(...args),
}));

jest.mock("@/features/user/services/assessmentService", () => ({
  getAssessmentOverview: (...args: unknown[]) => mockGetAssessmentOverview(...args),
  getAssessmentAttemptInfo: jest.fn().mockResolvedValue({ attemptsUsed: 0, maxAttempts: 3 }),
  getAssessmentAttempts: (...args: unknown[]) => mockGetAssessmentAttempts(...args),
}));

jest.mock("@/features/user/services/reviewService", () => ({
  submitReview: (...args: unknown[]) => mockSubmitReview(...args),
}));

const mockShowToast = jest.fn();
jest.mock("@/shared/services/feedback", () => ({
  feedbackService: { showToast: (...args: unknown[]) => mockShowToast(...args) },
}));

const mockEnroll = jest.fn();
const mockEnrollmentState: { enrolledCourses: Record<string, unknown>; loading: boolean; enroll: jest.Mock } = {
  enrolledCourses: {},
  loading: false,
  enroll: mockEnroll,
};

jest.mock("@/core/providers/EnrollmentProvider", () => ({
  useEnrollment: () => mockEnrollmentState,
}));

jest.mock("@/shared/components/ui/LapSpinnerv1/LapSpinnerv1", () => () => <div data-testid="spinner">Loading...</div>);

jest.mock("@/features/user/components/CourseHero/CourseHero", () => {
  const MockCourseHero = ({ course, durationLabel, isEnrolled, onRateClick }: { course: { title: string }; durationLabel: string; isEnrolled: boolean; onRateClick: () => void }) => (
    <div data-testid="course-hero">
      <span data-testid="hero-title">{course.title}</span>
      <span data-testid="hero-duration">{durationLabel}</span>
      <span data-testid="hero-enrolled">{String(isEnrolled)}</span>
      <button data-testid="rate-btn" onClick={onRateClick}>Rate</button>
    </div>
  );
  return MockCourseHero;
});

jest.mock("@/shared/components/ui/LapSidebar/LapSidebar", () => ({ children }: { children: React.ReactNode }) => <div data-testid="sidebar">{children}</div>);

jest.mock("@/shared/components/layout/LapCourseLayout/LapCourseLayout", () => ({ children, sidebar }: { children: React.ReactNode; sidebar: React.ReactNode }) => (
  <div data-testid="layout"><div data-testid="layout-sidebar">{sidebar}</div><div data-testid="layout-content">{children}</div></div>
));

jest.mock("@/shared/components/ui/LapCurriculumAccordion/LapCurriculumAccordion", () => ({ topics, disabled }: { topics: Array<{ id: string; name: string }>; disabled?: boolean }) => (
  <div data-testid="accordion" data-disabled={disabled}>
    {topics.map((t) => <span key={t.id} data-testid="accordion-topic">{t.name}</span>)}
  </div>
));

jest.mock("@/shared/components/ui/LapCourseDiscussion/LapCourseDiscussion", () => () => <div data-testid="discussions">Discussions</div>);

jest.mock("@/features/user/components/RatingsView/RatingsView", () => ({ courseId, refreshKey }: { courseId: string; refreshKey: number }) => (
  <div data-testid="ratings"><span data-testid="ratings-course">{courseId}</span><span data-testid="ratings-key">{refreshKey}</span></div>
));

jest.mock("@/features/leaderboard/pages/course-leaderboard/CourseLeaderboardPage", () => ({ courseId }: { courseId?: string }) => (
  <div data-testid="leaderboard"><span data-testid="leaderboard-course">{courseId}</span></div>
));

jest.mock("@/shared/components/ui/LapAssessmentCard/LapAssessmentCard", () => ({ courseId }: { courseId: string }) => <div data-testid="assessment-card">{courseId}</div>);

jest.mock("@/features/user/components/AssessmentHistoryCard/AssessmentHistoryCard", () => ({ item }: { item: { assessment_title?: string | null; score?: number } }) => (
  <div data-testid="history-card"><span>{item.assessment_title}</span><span>{item.score}</span></div>
));

jest.mock("@/shared/components/ui/LapNoContent/LapNoContent", () => ({ title, message }: { title: string; message: string }) => (
  <div data-testid="no-content"><div data-testid="nc-title">{title}</div><div data-testid="nc-message">{message}</div></div>
));

jest.mock("@/shared/components/feedback/LapModalDialog/LapModalDialog", () => ({ open, title, children }: { open: boolean; title: string; children: React.ReactNode }) => (
  open ? <div data-testid="modal"><div data-testid="modal-title">{title}</div>{children}</div> : null
));

jest.mock("@/features/user/components/ReviewForm/ReviewForm", () => ({ onSubmit }: { onSubmit: (data: { rating: number; reviewText: string }) => void }) => (
  <div data-testid="review-form"><button data-testid="submit-review" onClick={() => onSubmit({ rating: 5, reviewText: "Great!" })}>Submit</button></div>
));

const courseData = {
  id: "course-1",
  title: "React Fundamentals",
  category: { id: "cat-1", name: "Programming" },
  difficultyLevel: { id: "lvl-1", name: "Intermediate" },
  durationMinute: 120,
  overallRating: 4.5,
  thumbnailImgPath: "/img.jpg",
  status: true,
  description: "Learn React",
  createdByUser: { id: "u1", fullName: "John", email: "john@test.com", roles: [] },
  topics: [{ id: "topic-1", name: "JSX Basics", sequenceOrder: 1, durationMinute: 30, contents: [{ id: "c1", title: "Intro", contentType: { id: "video", name: "Video" }, videoUrl: "url", durationMinute: 10, sequenceOrder: 1 }] }],
  enrollmentCount: 100,
  assessmentTitle: "Final Test",
  totalMark: 100,
  passingMark: 60,
};

const assessmentData = { id: "asm-1", title: "Final Test", totalMark: 100, passingMark: 60, durationMinute: 30, course: { id: "course-1", title: "React", difficultyLevel: { id: "lvl-1", name: "Intermediate" } } };

describe("CourseOverview", () => {
  beforeEach(() => {
    jest.clearAllMocks();
    mockGetCourseOverview.mockResolvedValue(courseData);
    mockGetCourseContent.mockResolvedValue({ topics: [], thumbnailImg: "" });
    mockGetCourseProgress.mockResolvedValue(80);
    mockGetAssessmentOverview.mockResolvedValue(assessmentData);
    mockGetAssessmentAttempts.mockResolvedValue([]);
    mockSubmitReview.mockResolvedValue({});
    mockEnrollmentState.enrolledCourses = {};
    mockEnrollmentState.loading = false;
  });

  it("shows loading spinner initially", () => {
    mockGetCourseOverview.mockReturnValue(new Promise(() => {}));
    render(<CourseOverview />);
    expect(screen.getByTestId("spinner")).toBeInTheDocument();
  });

  it("shows error state when course fetch fails", async () => {
    mockGetCourseOverview.mockRejectedValue(new Error("fail"));
    render(<CourseOverview />);
    expect(await screen.findByText("Failed to load course details.")).toBeInTheDocument();
  });

  it("renders course title after loading", async () => {
    render(<CourseOverview />);
    expect(await screen.findByTestId("hero-title")).toHaveTextContent("React Fundamentals");
  });

  it("shows not enrolled by default", async () => {
    render(<CourseOverview />);
    expect(await screen.findByTestId("hero-enrolled")).toHaveTextContent("false");
  });

  it("shows enrolled when course is in enrolledCourses", async () => {
    mockEnrollmentState.enrolledCourses = { "course-1": { id: "enroll-1", courseId: "course-1", status: true } };
    render(<CourseOverview />);
    expect(await screen.findByTestId("hero-enrolled")).toHaveTextContent("true");
  });

  it("renders curriculum accordion with topics", async () => {
    render(<CourseOverview />);
    expect(await screen.findByTestId("accordion-topic")).toHaveTextContent("JSX Basics");
  });

  it("passes disabled prop to accordion", async () => {
    render(<CourseOverview />);
    const accordion = await screen.findByTestId("accordion");
    expect(accordion).toHaveAttribute("data-disabled", "true");
  });

  it("renders sidebar with navigation tabs", async () => {
    render(<CourseOverview />);
    expect(await screen.findByText("Overview")).toBeInTheDocument();
    expect(screen.getByText("Discussions")).toBeInTheDocument();
    expect(screen.getByText("Ratings")).toBeInTheDocument();
    expect(screen.getByText("Leaderboard")).toBeInTheDocument();
    expect(screen.getByText("History")).toBeInTheDocument();
  });

  it("switches to discussions tab", async () => {
    render(<CourseOverview />);
    await screen.findByTestId("hero-title");
    fireEvent.click(screen.getByText("Discussions"));
    expect(screen.getByTestId("discussions")).toBeInTheDocument();
  });

  it("switches to ratings tab", async () => {
    render(<CourseOverview />);
    await screen.findByTestId("hero-title");
    fireEvent.click(screen.getByText("Ratings"));
    expect(screen.getByTestId("ratings")).toBeInTheDocument();
  });

  it("switches to leaderboard tab and renders with courseId", async () => {
    render(<CourseOverview />);
    await screen.findByTestId("hero-title");
    fireEvent.click(screen.getByText("Leaderboard"));
    expect(screen.getByTestId("leaderboard")).toBeInTheDocument();
    expect(screen.getByTestId("leaderboard-course")).toHaveTextContent("course-1");
  });

  it("shows no-content when no topics exist", async () => {
    mockGetCourseOverview.mockResolvedValue({ ...courseData, topics: [] });
    render(<CourseOverview />);
    expect(await screen.findByTestId("nc-title")).toHaveTextContent("No curriculum");
  });

  it("opens review modal when rate button clicked", async () => {
    mockEnrollmentState.enrolledCourses = { "course-1": { id: "enroll-1", courseId: "course-1", status: true } };
    render(<CourseOverview />);
    await screen.findByTestId("hero-title");
    fireEvent.click(screen.getByTestId("rate-btn"));
    expect(screen.getByTestId("modal")).toBeInTheDocument();
    expect(screen.getByTestId("modal-title")).toHaveTextContent("Rate this Course");
  });

  it("submits review and refreshes ratings", async () => {
    mockEnrollmentState.enrolledCourses = { "course-1": { id: "enroll-1", courseId: "course-1", status: true } };
    render(<CourseOverview />);
    await screen.findByTestId("hero-title");
    fireEvent.click(screen.getByTestId("rate-btn"));
    fireEvent.click(screen.getByTestId("submit-review"));
    await act(async () => {});
    expect(mockSubmitReview).toHaveBeenCalledWith("course-1", { rating: 5, reviewText: "Great!" });
    expect(mockShowToast).toHaveBeenCalledWith("Review submitted successfully", "success");
  });

  it("shows error toast when review submission fails", async () => {
    mockSubmitReview.mockRejectedValue(new Error("fail"));
    mockEnrollmentState.enrolledCourses = { "course-1": { id: "enroll-1", courseId: "course-1", status: true } };
    render(<CourseOverview />);
    await screen.findByTestId("hero-title");
    fireEvent.click(screen.getByTestId("rate-btn"));
    fireEvent.click(screen.getByTestId("submit-review"));
    await act(async () => {});
    expect(mockShowToast).toHaveBeenCalledWith("fail", "error");
  });
});
