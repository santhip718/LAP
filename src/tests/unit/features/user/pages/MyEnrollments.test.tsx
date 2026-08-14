import "@testing-library/jest-dom";
import { render, screen, fireEvent } from "@testing-library/react";
import MyEnrollments from "@/features/user/pages/MyEnrollments/MyEnrollments";

const mockNavigate = jest.fn();
jest.mock("react-router-dom", () => ({ useNavigate: () => mockNavigate }));

const mockGetRecommendedCourses = jest.fn();
jest.mock("@/features/user/services/courseService", () => ({
  getRecommendedCourses: (...args: unknown[]) => mockGetRecommendedCourses(...args),
}));

const mockEnrollmentsState: { enrolledCourses: Record<string, unknown>; loading: boolean } = {
  enrolledCourses: {},
  loading: false,
};
jest.mock("@/core/providers/EnrollmentProvider", () => ({
  useEnrollment: () => mockEnrollmentsState,
}));

const mockCourseState: { courses: Array<Record<string, unknown>> } = { courses: [] };
jest.mock("@/core/providers/CourseProvider", () => ({
  useCourse: () => mockCourseState,
}));

jest.mock("@/shared/components/ui/LapSpinnerv1/LapSpinnerv1", () => () => <div data-testid="spinner">Loading...</div>);
jest.mock("@/features/user/components/EnrolledCourseCard/EnrolledCourseCard", () => ({ course }: { course: { title: string } }) => <div data-testid="enrolled-card">{course.title}</div>);
jest.mock("@/features/user/components/CourseCard/CourseCard", () => ({ course }: { course: { title: string } }) => <div data-testid="course-card">{course.title}</div>);

describe("MyEnrollments", () => {
  beforeEach(() => {
    jest.clearAllMocks();
    mockEnrollmentsState.enrolledCourses = {};
    mockEnrollmentsState.loading = false;
    mockCourseState.courses = [];
    mockGetRecommendedCourses.mockResolvedValue([]);
  });

  it("renders page title and subtitle", () => {
    render(<MyEnrollments />);
    expect(screen.getByText("My Enrollments")).toBeInTheDocument();
    expect(screen.getByText(/Track your learning progress/i)).toBeInTheDocument();
  });

  it("shows loading spinner when loading enrollments", () => {
    mockEnrollmentsState.loading = true;
    render(<MyEnrollments />);
    expect(screen.getByTestId("spinner")).toBeInTheDocument();
  });

  it("shows empty state when no enrollments", async () => {
    mockGetRecommendedCourses.mockResolvedValue([]);
    render(<MyEnrollments />);
    expect(await screen.findByText("No enrollments yet")).toBeInTheDocument();
  });

  it("renders browse button in empty state", async () => {
    mockGetRecommendedCourses.mockResolvedValue([]);
    render(<MyEnrollments />);
    const btn = await screen.findByText("Browse Courses");
    fireEvent.click(btn);
    expect(mockNavigate).toHaveBeenCalledWith("/discover");
  });

  it("renders enrolled courses", async () => {
    mockEnrollmentsState.enrolledCourses = { "course-1": { id: "enroll-1", courseId: "course-1", title: "React Basics", category: "Programming", enrolledOn: "2025-01-01", completedOn: null, progress: 50, status: true, thumbnail: "" } };
    mockGetRecommendedCourses.mockResolvedValue([]);
    render(<MyEnrollments />);
    expect(await screen.findByText("React Basics")).toBeInTheDocument();
  });

  it("renders recommended section when recommendations exist", async () => {
    mockGetRecommendedCourses.mockResolvedValue([{ id: "rec-1", title: "Advanced React", category: "Programming", duration: "10h", level: "Advanced", rating: "4.5", image: "", alt: "", isBestseller: false }]);
    render(<MyEnrollments />);
    expect(await screen.findByText("Recommended for You")).toBeInTheDocument();
    expect(screen.getByText("Advanced React")).toBeInTheDocument();
  });

  it("does not render recommended section when no recommendations", async () => {
    mockGetRecommendedCourses.mockResolvedValue([]);
    render(<MyEnrollments />);
    expect(screen.queryByText("Recommended for You")).not.toBeInTheDocument();
  });

  it("renders your courses section header", async () => {
    mockEnrollmentsState.enrolledCourses = { "course-1": { id: "enroll-1", courseId: "course-1", title: "React Basics", category: "Programming", enrolledOn: "2025-01-01", completedOn: null, progress: 50, status: true, thumbnail: "" } };
    mockGetRecommendedCourses.mockResolvedValue([]);
    render(<MyEnrollments />);
    expect(await screen.findByText("Your Courses")).toBeInTheDocument();
  });
});
