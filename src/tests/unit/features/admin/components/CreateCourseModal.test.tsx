import { render, screen, fireEvent } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";

const mockOnClose = jest.fn();
const mockOnSuccess = jest.fn();

jest.mock("@/features/admin/hooks/useReferenceData", () => ({
  useReferenceData: jest.fn(),
}));

import { useReferenceData } from "@/features/admin/hooks/useReferenceData";
const mockUseReferenceData = useReferenceData as jest.Mock;

jest.mock("@/shared/services/feedback/feedbackService", () => ({
  feedbackService: {
    showToast: jest.fn(),
    on: jest.fn(),
    off: jest.fn(),
    emit: jest.fn(),
    showConfirm: jest.fn(),
    dismissToast: jest.fn(),
  },
}));

import CreateCourseModal from "@/features/admin/components/CreateCourseModal/CreateCourseModal";

const defaultRefData = {
  categories: [{ id: "cat-1", name: "Frontend" }],
  subcategories: [{ id: "sub-1", name: "React" }],
  difficultyLevels: [{ id: "diff-1", name: "Beginner" }],
  contentTypes: [{ id: "ct-1", name: "Video" }],
  loading: false,
  error: null,
};

const renderComponent = (props = {}) =>
  render(
    <MemoryRouter>
      <CreateCourseModal
        open={true}
        onClose={mockOnClose}
        onSuccess={mockOnSuccess}
        {...props}
      />
    </MemoryRouter>
  );

describe("CreateCourseModal", () => {
  beforeEach(() => {
    jest.clearAllMocks();
    mockUseReferenceData.mockReturnValue(defaultRefData);
  });

  it("renders the modal with title", () => {
    renderComponent();
    expect(screen.getByText("Create New Course")).toBeInTheDocument();
  });

  it("renders edit title when editCourse is provided", () => {
    renderComponent({
      editCourse: {
        id: "course-1",
        title: "Existing Course",
        description: "Existing description",
        categoryId: "cat-1",
        subCategoryId: "sub-1",
        difficultyLevelId: "diff-1",
        durationHours: 2,
        thumbnailUrl: undefined,
        isDrafted: false,
        topics: [],
      },
    });
    expect(screen.getByText("Edit Course")).toBeInTheDocument();
  });

  it("renders loading state when reference data is loading", () => {
    mockUseReferenceData.mockReturnValue({ ...defaultRefData, loading: true });
    renderComponent();
    expect(screen.getByText("Loading reference data...")).toBeInTheDocument();
  });

  it("renders reference data error", () => {
    mockUseReferenceData.mockReturnValue({
      ...defaultRefData,
      error: "Failed to load",
    });
    renderComponent();
    expect(screen.getByText("Failed to load")).toBeInTheDocument();
  });

  it("renders course title input", () => {
    renderComponent();
    expect(screen.getByText(/Course Title/)).toBeInTheDocument();
    expect(screen.getByRole("textbox", { name: /course title/i })).toBeInTheDocument();
  });

  it("renders description input", () => {
    renderComponent();
    expect(screen.getByText("Description")).toBeInTheDocument();
  });

  it("renders category select", () => {
    renderComponent();
    expect(screen.getByText("Category")).toBeInTheDocument();
  });

  it("renders difficulty level select", () => {
    renderComponent();
    expect(screen.getByText("Difficulty Level")).toBeInTheDocument();
  });

  it("renders duration input", () => {
    renderComponent();
    expect(screen.getByText(/Duration \(hours\)/)).toBeInTheDocument();
  });

  it("renders Save as Draft and Publish buttons", () => {
    renderComponent();
    expect(screen.getByText("Save as Draft")).toBeInTheDocument();
    expect(screen.getByText("Publish Course")).toBeInTheDocument();
  });

  it("calls onClose when close button is clicked", () => {
    renderComponent();
    const closeBtn = screen.getAllByText("close")[0].closest("button");
    expect(closeBtn).toBeInTheDocument();
    fireEvent.click(closeBtn!);
    expect(mockOnClose).toHaveBeenCalledTimes(1);
  });

  it("renders topic section fields", () => {
    renderComponent();
    expect(screen.getByText("Add Content Meta Topic")).toBeInTheDocument();
  });
});
