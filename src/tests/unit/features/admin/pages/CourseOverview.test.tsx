import "@testing-library/jest-dom";
import { render, screen, fireEvent } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";

const mockNavigate = jest.fn();
const mockParams = { courseId: "course-1" };
jest.mock("react-router-dom", () => ({
  ...jest.requireActual("react-router-dom"),
  useNavigate: () => mockNavigate,
  useParams: () => mockParams,
}));

jest.mock("@/shared/utils/errorLogger", () => ({
  logError: jest.fn(),
}));

const mockRefresh = jest.fn();
jest.mock("@/features/admin/hooks/useCourseOverview", () => ({
  useCourseOverview: jest.fn(),
}));

import { useCourseOverview } from "@/features/admin/hooks/useCourseOverview";
const mockUseCourseOverview = useCourseOverview as jest.Mock;

import CourseOverview from "@/features/admin/pages/CourseOverview/CourseOverview";

const mockCourse = {
  id: "course-1",
  title: "React Fundamentals",
  description: "Learn React from scratch",
  category: "Frontend",
  subCategory: "Web",
  difficulty: "Beginner",
  durationMinute: 120,
  rating: 4.5,
  thumbnailUrl: undefined,
  isDrafted: false,
  createdBy: "John Doe",
  dateCreated: "2025-01-15T00:00:00Z",
  dateUpdated: "2025-06-01T00:00:00Z",
  enrollmentCount: 45,
  assessmentTitle: "Final Exam",
  totalMark: 100,
  passingMark: 60,
  topics: [
    {
      id: "topic-1",
      name: "Introduction",
      sequenceOrder: 1,
      durationMinute: 30,
      contents: [
        { id: "content-1", metaTopicId: "topic-1", sequenceOrder: 1, title: "What is React?" },
      ],
    },
  ],
};

const defaultMock = {
  course: mockCourse,
  loading: false,
  error: null,
  refresh: mockRefresh,
};

const renderComponent = () =>
  render(
    <MemoryRouter>
      <CourseOverview />
    </MemoryRouter>
  );

describe("CourseOverview", () => {
  beforeEach(() => {
    jest.clearAllMocks();
    mockUseCourseOverview.mockReturnValue(defaultMock);
  });

  it("renders loading spinner", () => {
    mockUseCourseOverview.mockReturnValue({ ...defaultMock, loading: true });
    const { container } = renderComponent();
    expect(container.querySelector(".lap-spinner-overlay")).toBeInTheDocument();
  });

  it("renders error state", () => {
    mockUseCourseOverview.mockReturnValue({
      ...defaultMock,
      error: "Course not found",
      course: null,
    });
    renderComponent();
    expect(screen.getByText("Course not found")).toBeInTheDocument();
  });

  it("renders course title and description", () => {
    renderComponent();
    expect(screen.getByText("React Fundamentals")).toBeInTheDocument();
    expect(screen.getByText("Learn React from scratch")).toBeInTheDocument();
  });

  it("renders course category and difficulty", () => {
    renderComponent();
    expect(screen.getAllByText("Frontend")[0]).toBeInTheDocument();
    expect(screen.getAllByText("Beginner")[0]).toBeInTheDocument();
  });

  it("renders duration and rating", () => {
    renderComponent();
    expect(screen.getByText("2h")).toBeInTheDocument();
  });

  it("rendert status badge", () => {
    renderComponent();
    expect(screen.getAllByText("Published")[0]).toBeInTheDocument();
  });

  it("renders draft badge for drafted courses", () => {
    mockUseCourseOverview.mockReturnValue({
      ...defaultMock,
      course: { ...mockCourse, isDrafted: true },
    });
    renderComponent();
    expect(screen.getAllByText("Draft")[0]).toBeInTheDocument();
  });

  it("renders course topics", () => {
    renderComponent();
    expect(screen.getByText("Introduction")).toBeInTheDocument();
    expect(screen.getByText("What is React?")).toBeInTheDocument();
  });

  it("renders created by and enrollment count", () => {
    renderComponent();
    expect(screen.getByText("John Doe")).toBeInTheDocument();
    expect(screen.getByText("45")).toBeInTheDocument();
  });

  it("renders assessment info", () => {
    renderComponent();
    expect(screen.getByText("Final Exam")).toBeInTheDocument();
    expect(screen.getByText("100")).toBeInTheDocument();
    expect(screen.getByText("60")).toBeInTheDocument();
  });


  it("navigates to discussion page when discussion tab is clicked", () => {
    renderComponent();
    fireEvent.click(screen.getByText("Discussion"));
    expect(mockNavigate).toHaveBeenCalledWith("/admin/courses/course-1/discussion");
  });

  it("renders discussion tab content", () => {
    render(
      <MemoryRouter initialEntries={["/admin/courses/course-1/discussion"]}>
        <CourseOverview />
      </MemoryRouter>
    );
    expect(screen.getByText("Course messages and announcements")).toBeInTheDocument();
  });
});
