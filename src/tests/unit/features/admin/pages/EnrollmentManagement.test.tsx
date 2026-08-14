import { render, screen, fireEvent } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";

const mockRefresh = jest.fn();
jest.mock("@/features/admin/hooks/useEnrollments", () => ({
  useEnrollments: jest.fn(),
}));

import { useEnrollments } from "@/features/admin/hooks/useEnrollments";
const mockUseEnrollments = useEnrollments as jest.Mock;

jest.mock("@/features/admin/services/enrollmentService", () => ({
  enrollmentService: {
    acceptEnrollment: jest.fn().mockResolvedValue(undefined),
  },
}));

import EnrollmentManagement from "@/features/admin/pages/EnrollmentManagement/EnrollmentManagement";

const defaultMock = {
  enrollments: [],
  total: 0,
  loading: false,
  error: null,
  refreshing: false,
  refresh: mockRefresh,
};

const mockEnrollments = [
  {
    id: "enr-1",
    userId: "user-1",
    courseId: "course-1",
    courseTitle: "React Basics",
    userFullName: "Alice Smith",
    category: "Frontend",
    enrollmentStatus: false,
    enrolledOn: "2025-03-15T10:00:00Z",
  },
  {
    id: "enr-2",
    userId: "user-2",
    courseId: "course-1",
    courseTitle: "React Basics",
    userFullName: "Bob Jones",
    category: "Frontend",
    enrollmentStatus: false,
    enrolledOn: "2025-03-16T14:30:00Z",
  },
];

const renderComponent = () =>
  render(
    <MemoryRouter>
      <EnrollmentManagement />
    </MemoryRouter>
  );

describe("EnrollmentManagement", () => {
  beforeEach(() => {
    jest.clearAllMocks();
    mockUseEnrollments.mockReturnValue(defaultMock);
  });

  it("renders page title and subtitle", () => {
    renderComponent();
    expect(screen.getByText("Enrollments")).toBeInTheDocument();
    expect(
      screen.getByText("Review and manage pending enrollment requests.")
    ).toBeInTheDocument();
  });

  it("renders loading state", () => {
    mockUseEnrollments.mockReturnValue({ ...defaultMock, loading: true });
    renderComponent();
    expect(screen.getByText("Loading enrollments...")).toBeInTheDocument();
  });

  it("renders error state", () => {
    mockUseEnrollments.mockReturnValue({
      ...defaultMock,
      error: "Failed to load enrollments",
    });
    renderComponent();
    expect(screen.getByText("Failed to load enrollments")).toBeInTheDocument();
  });

  it("renders empty state when no pending enrollments", () => {
    renderComponent();
    expect(screen.getByText("No pending enrollments.")).toBeInTheDocument();
    expect(
      screen.getByText("There are no pending enrollment requests at this time.")
    ).toBeInTheDocument();
  });

  it("renders pending enrollments", () => {
    mockUseEnrollments.mockReturnValue({
      ...defaultMock,
      enrollments: mockEnrollments,
    });
    renderComponent();
    expect(screen.getByText("Alice Smith")).toBeInTheDocument();
    expect(screen.getByText("Bob Jones")).toBeInTheDocument();
    expect(screen.getAllByText("React Basics")[0]).toBeInTheDocument();
  });

  it("renders pending count badge", () => {
    mockUseEnrollments.mockReturnValue({
      ...defaultMock,
      enrollments: mockEnrollments,
    });
    renderComponent();
    expect(screen.getByText("2 pending")).toBeInTheDocument();
  });

  it("renders accept button for each enrollment", () => {
    mockUseEnrollments.mockReturnValue({
      ...defaultMock,
      enrollments: mockEnrollments,
    });
    renderComponent();
    const acceptButtons = screen.getAllByText("Accept");
    expect(acceptButtons).toHaveLength(2);
  });

  it("renders refresh button", () => {
    renderComponent();
    const refreshBtn = screen.getByLabelText("Refresh");
    expect(refreshBtn).toBeInTheDocument();
    fireEvent.click(refreshBtn);
    expect(mockRefresh).toHaveBeenCalledTimes(1);
  });

  it("formats enrollment date", () => {
    mockUseEnrollments.mockReturnValue({
      ...defaultMock,
      enrollments: [mockEnrollments[0]],
    });
    renderComponent();
    expect(screen.getByText("Mar 15, 2025")).toBeInTheDocument();
  });
});
