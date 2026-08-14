import "@testing-library/jest-dom";
import { render, screen, fireEvent } from "@testing-library/react";
import ViewCourseContent from "@/features/user/pages/ViewCourseContent/ViewCourseContent";

const mockNavigate = jest.fn();
jest.mock("react-router-dom", () => ({
  useParams: () => ({ courseId: "course-1" }),
  useNavigate: () => mockNavigate,
}));

const mockGetCourseOverview = jest.fn();
const mockGetCourseProgress = jest.fn();
const mockGetCourseContent = jest.fn();
const mockGetAssessmentOverview = jest.fn();
const mockGetApiV1CourseContentId = jest.fn();
const mockPutApiV1CourseContentIdCompletionStatus = jest.fn();

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
  getAssessmentAttempts: jest.fn().mockResolvedValue([]),
}));

jest.mock("@/shared/services/api/config/axios", () => ({}));

jest.mock("@/shared/services/api/services/course-content/course-content", () => ({
  getCourseContent: () => ({
    getApiV1CourseContentId: (...args: unknown[]) => mockGetApiV1CourseContentId(...args),
    putApiV1CourseContentIdCompletionStatus: (...args: unknown[]) => mockPutApiV1CourseContentIdCompletionStatus(...args),
  }),
}));

jest.mock("@/shared/components/ui/LapSpinnerv1/LapSpinnerv1", () => () => <div data-testid="spinner">Loading...</div>);
jest.mock("@/features/user/components/CourseCanvas/CourseCanvas", () => ({ content }: { content: { title: string } | null }) => <div data-testid="canvas">{content?.title ?? "No content"}</div>);
jest.mock("@/shared/components/ui/LapSidebar/LapSidebar", () => ({ children }: { children: React.ReactNode }) => <div data-testid="sidebar">{children}</div>);
jest.mock("@/shared/components/layout/LapCourseLayout/LapCourseLayout", () => ({ children, sidebar }: { children: React.ReactNode; sidebar: React.ReactNode }) => (
  <div data-testid="layout"><div data-testid="layout-sidebar">{sidebar}</div><div data-testid="layout-content">{children}</div></div>
));
jest.mock("@/shared/components/ui/LapCurriculumAccordion/LapCurriculumAccordion", () => ({ topics }: { topics: Array<{ id: string }> }) => <div data-testid="accordion">{topics.map((t) => <span key={t.id} data-testid="accordion-topic">{t.id}</span>)}</div>);
jest.mock("@/shared/components/ui/LapAssessmentCard/LapAssessmentCard", () => ({ courseId }: { courseId: string }) => <div data-testid="assessment-card">{courseId}</div>);
jest.mock("@/features/user/components/AssessmentHistoryCard/AssessmentHistoryCard", () => () => null);

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
  topics: [{ id: "topic-1", name: "JSX Basics", sequenceOrder: 1, durationMinute: 30, contents: [{ id: "c1", title: "Intro to JSX", contentType: { id: "video", name: "Video" }, videoUrl: "url1", durationMinute: 10, sequenceOrder: 1 }] }],
  enrollmentCount: 100,
  assessmentTitle: "Final Test",
  totalMark: 100,
  passingMark: 60,
};

const assessmentData = { id: "asm-1", title: "Final Test", totalMark: 100, passingMark: 60, durationMinute: 30, course: { id: "course-1", title: "React", difficultyLevel: { id: "lvl-1", name: "Intermediate" } } };

const contentResponseDto = { data: { id: "c1", title: "Intro to JSX", video_url: "https://video.com/jsx", content_type: "Video", previous_content_id: null, next_content_id: "c2", is_completed: false, pdf_base64: null } };

const contentData = { topics: [{ id: "topic-1", name: "JSX Basics", isCompleted: false, contents: [{ id: "c1", title: "Intro to JSX", contentType: { id: "video", name: "Video" }, videoUrl: "url1", isCompleted: false, durationMinute: 10, sequenceOrder: 1 }] }], thumbnailImg: "" };

describe("ViewCourseContent", () => {
  beforeEach(() => {
    jest.clearAllMocks();
    mockGetCourseOverview.mockResolvedValue(courseData);
    mockGetCourseProgress.mockResolvedValue(80);
    mockGetCourseContent.mockResolvedValue(contentData);
    mockGetAssessmentOverview.mockResolvedValue(assessmentData);
    mockGetApiV1CourseContentId.mockResolvedValue(contentResponseDto);
    mockPutApiV1CourseContentIdCompletionStatus.mockResolvedValue({});
  });

  it("shows error banner when course fetch fails", async () => {
    mockGetCourseOverview.mockRejectedValue(new Error("fail"));
    render(<ViewCourseContent />);
    expect(await screen.findByText("Could not load course data. The curriculum may be incomplete.")).toBeInTheDocument();
  });

  it("renders course canvas after loading", async () => {
    render(<ViewCourseContent />);
    expect(await screen.findByTestId("canvas")).toBeInTheDocument();
  });

  it("renders content title from API response", async () => {
    render(<ViewCourseContent />);
    expect(await screen.findByText("Intro to JSX")).toBeInTheDocument();
  });

  it("renders accordion with topics", async () => {
    render(<ViewCourseContent />);
    expect(await screen.findByTestId("accordion")).toBeInTheDocument();
  });

  it("renders assessment card", async () => {
    render(<ViewCourseContent />);
    expect(await screen.findByTestId("assessment-card")).toBeInTheDocument();
  });

  it("renders navigation bar", async () => {
    render(<ViewCourseContent />);
    expect(await screen.findByText("Next")).toBeInTheDocument();
  });

  it("shows previous button disabled when no previous content", async () => {
    render(<ViewCourseContent />);
    await screen.findByText("Next");
    const prevBtn = screen.getByText("Previous").closest("button");
    expect(prevBtn).toBeDisabled();
  });

  it("shows next button enabled when next content exists", async () => {
    render(<ViewCourseContent />);
    await screen.findByText("Next");
    const nextBtn = screen.getByText("Next").closest("button");
    expect(nextBtn).not.toBeDisabled();
  });

  it("shows Mark as Completed button", async () => {
    render(<ViewCourseContent />);
    expect(await screen.findByText("Mark as Completed")).toBeInTheDocument();
  });

  it("shows sidebar status when no topics and no assessment", async () => {
    mockGetCourseOverview.mockResolvedValue({ ...courseData, topics: [], assessmentTitle: "" });
    mockGetAssessmentOverview.mockRejectedValue(new Error("none"));
    mockGetCourseContent.mockResolvedValue({ topics: [], thumbnailImg: "" });
    render(<ViewCourseContent />);
    expect(await screen.findByText("No curriculum content available.")).toBeInTheDocument();
  });

  it("shows loading sidebar status while loading", () => {
    mockGetCourseOverview.mockReturnValue(new Promise(() => {}));
    render(<ViewCourseContent />);
    expect(screen.getByText("Loading curriculum...")).toBeInTheDocument();
  });
});
