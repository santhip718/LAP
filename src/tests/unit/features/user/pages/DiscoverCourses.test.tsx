import "@testing-library/jest-dom";
import { render, screen } from "@testing-library/react";
import DiscoverCourses from "@/features/user/pages/DiscoverCourses/DiscoverCourses";

const mockLoadInitial = jest.fn();
const mockLoadMore = jest.fn();
const mockSetFilters = jest.fn();

const mockCourseState: Record<string, unknown> = {
  courses: [],
  allLoaded: false,
  loading: false,
  initialized: false,
  loadInitial: mockLoadInitial,
  loadMore: mockLoadMore,
  setFilters: mockSetFilters,
};

jest.mock("@/core/providers/CourseProvider", () => ({
  useCourse: () => mockCourseState,
}));

const mockEnroll = jest.fn();
const mockEnrollmentState = { enrolledCourses: {} as Record<string, unknown>, enroll: mockEnroll };
jest.mock("@/core/providers/EnrollmentProvider", () => ({
  useEnrollment: () => mockEnrollmentState,
}));

jest.mock("@/shared/components/ui/LapSpinnerv1/LapSpinnerv1", () => () => <div data-testid="spinner">Loading...</div>);

jest.mock("@/shared/components/ui/LapNoContent/LapNoContent", () => ({ title, message }: { title: string; message: string }) => (
  <div data-testid="no-content"><div data-testid="empty-title">{title}</div><div data-testid="empty-message">{message}</div></div>
));

jest.mock("@/features/user/components/CourseCard/CourseCard", () => ({ course }: { course: { id: string; title: string } }) => <div data-testid="course-card">{course.title}</div>);

jest.mock("@/features/user/components/FilterBar/FilterBar", () => ({ onFilterChange }: { onFilterChange: (filters: Record<string, string>) => void }) => (
  <div data-testid="filter-bar"><button data-testid="apply-filter" onClick={() => onFilterChange({ search: "react" })}>Filter</button></div>
));

describe("DiscoverCourses", () => {
  beforeEach(() => {
    jest.clearAllMocks();
    mockCourseState.courses = [];
    mockCourseState.allLoaded = false;
    mockCourseState.loading = false;
    mockCourseState.initialized = false;
  });

  it("calls loadInitial when not initialized", () => {
    render(<DiscoverCourses />);
    expect(mockLoadInitial).toHaveBeenCalled();
  });

  it("does not call loadInitial when already initialized", () => {
    mockCourseState.initialized = true;
    render(<DiscoverCourses />);
    expect(mockLoadInitial).not.toHaveBeenCalled();
  });

  it("renders hero title and subtitle", () => {
    render(<DiscoverCourses />);
    expect(screen.getByText("Expand your horizons")).toBeInTheDocument();
    expect(screen.getByText(/Explore thousands of courses/i)).toBeInTheDocument();
  });

  it("renders filter bar", () => {
    render(<DiscoverCourses />);
    expect(screen.getByTestId("filter-bar")).toBeInTheDocument();
  });

  it("shows no-content when no courses and not loading", () => {
    render(<DiscoverCourses />);
    expect(screen.getByTestId("no-content")).toBeInTheDocument();
    expect(screen.getByTestId("empty-title")).toHaveTextContent("No courses found");
  });

  it("shows spinner when loading and no courses", () => {
    mockCourseState.loading = true;
    render(<DiscoverCourses />);
    expect(screen.getByTestId("spinner")).toBeInTheDocument();
  });

  it("renders course cards", () => {
    mockCourseState.courses = [{ id: "c1", title: "React 101", category: "Programming", duration: "10h", level: "Beginner", rating: "4.5", image: "", alt: "", isBestseller: false }];
    mockCourseState.initialized = true;
    render(<DiscoverCourses />);
    expect(screen.getByText("React 101")).toBeInTheDocument();
  });

  it("renders sentinel when has courses and not all loaded", () => {
    mockCourseState.courses = [{ id: "c1", title: "React 101", category: "Programming", duration: "10h", level: "Beginner", rating: "4.5", image: "", alt: "", isBestseller: false }];
    mockCourseState.initialized = true;
    render(<DiscoverCourses />);
    expect(document.querySelector(".discover-sentinel")).toBeInTheDocument();
  });
});
