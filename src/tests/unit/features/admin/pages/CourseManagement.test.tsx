import { render, screen } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";

const mockNavigate = jest.fn();
jest.mock("react-router-dom", () => ({
  ...jest.requireActual("react-router-dom"),
  useNavigate: () => mockNavigate,
}));

const mockRefreshCourses = jest.fn();
jest.mock("@/features/admin/hooks/useAdminCourses", () => ({
  useAdminCourses: jest.fn(),
}));

import { useAdminCourses } from "@/features/admin/hooks/useAdminCourses";
const mockUseAdminCourses = useAdminCourses as jest.Mock;

jest.mock("@/shared/hooks/useDebounce", () => ({
  useDebounce: (value: string) => value,
}));

import CourseManagement from "@/features/admin/pages/CourseManagement/CourseManagement";

const defaultMock = {
  courses: [],
  totalCourses: 0,
  summary: {
    totalCourses: 0,
    publishedCourses: 0,
    draftCourses: 0,
    activeStudents: 0,
    totalEnrollments: 0,
  },
  loading: false,
  summaryLoading: false,
  loadingMore: false,
  error: null,
  summaryError: null,
  hasMore: false,
  refreshCourses: mockRefreshCourses,
};

const renderComponent = () =>
  render(
    <MemoryRouter>
      <CourseManagement />
    </MemoryRouter>
  );

describe("CourseManagement", () => {
  beforeEach(() => {
    jest.clearAllMocks();
    mockUseAdminCourses.mockReturnValue(defaultMock);
  });

  it("renders the page title", () => {
    renderComponent();
    expect(screen.getByText("Courses")).toBeInTheDocument();
  });

  it("renders Add Course button", () => {
    renderComponent();
    expect(screen.getByText("Add Course")).toBeInTheDocument();
  });

  it("renders stat cards", () => {
    mockUseAdminCourses.mockReturnValue({
      ...defaultMock,
      summary: { totalCourses: 10, publishedCourses: 7, draftCourses: 3, activeStudents: 50, totalEnrollments: 100 },
      totalCourses: 10,
    });
    renderComponent();
    expect(screen.getByText("10")).toBeInTheDocument();
  });

  it("renders loading state", () => {
    mockUseAdminCourses.mockReturnValue({
      ...defaultMock,
      loading: true,
    });
    renderComponent();
    expect(screen.getByText("Loading courses...")).toBeInTheDocument();
  });

  it("renders error state", () => {
    mockUseAdminCourses.mockReturnValue({
      ...defaultMock,
      error: "Failed to fetch courses",
    });
    renderComponent();
    expect(screen.getByText("Failed to fetch courses")).toBeInTheDocument();
  });

  it("renders course rows in the table", () => {
    mockUseAdminCourses.mockReturnValue({
      ...defaultMock,
      courses: [
        {
          id: "course-1",
          title: "React Basics",
          category: "Frontend",
          difficulty: "Beginner",
          durationMinute: 120,
          rating: 4.5,
          thumbnailUrl: undefined,
          isDrafted: false,
        },
      ],
    });
    renderComponent();
    expect(screen.getByText("React Basics")).toBeInTheDocument();
    expect(screen.getByText("Frontend")).toBeInTheDocument();
    expect(screen.getByText("Beginner")).toBeInTheDocument();
    expect(screen.getByText("2h")).toBeInTheDocument();
  });

  it("renders draft status badge for drafted courses", () => {
    mockUseAdminCourses.mockReturnValue({
      ...defaultMock,
      courses: [
        {
          id: "course-2",
          title: "Draft Course",
          category: "Backend",
          difficulty: "Intermediate",
          durationMinute: 60,
          rating: 0,
          thumbnailUrl: undefined,
          isDrafted: true,
        },
      ],
    });
    renderComponent();
    const draftBadges = screen.getAllByText("Draft");
    expect(draftBadges.length).toBe(1);
  });

  it("renders published status badge for published courses", () => {
    mockUseAdminCourses.mockReturnValue({
      ...defaultMock,
      courses: [
        {
          id: "course-3",
          title: "Live Course",
          category: "DevOps",
          difficulty: "Advanced",
          durationMinute: 180,
          rating: 4.0,
          thumbnailUrl: undefined,
          isDrafted: false,
        },
      ],
    });
    renderComponent();
    const publishedElements = screen.getAllByText("Published");
    expect(publishedElements.length).toBe(2);
  });

  it("shows active count badge", () => {
    mockUseAdminCourses.mockReturnValue({
      ...defaultMock,
      courses: [
        { id: "1", title: "C1", category: "A", difficulty: "Beginner", durationMinute: 30, rating: 3, isDrafted: false },
        { id: "2", title: "C2", category: "B", difficulty: "Intermediate", durationMinute: 45, rating: 4, isDrafted: true },
        { id: "3", title: "C3", category: "C", difficulty: "Expert", durationMinute: 60, rating: 5, isDrafted: false },
      ],
    });
    renderComponent();
    expect(screen.getByText("2 Active")).toBeInTheDocument();
  });

  it("renders search input", () => {
    renderComponent();
    expect(screen.getByPlaceholderText("Search courses")).toBeInTheDocument();
  });
});
