import "@testing-library/jest-dom";
import { render, screen, fireEvent } from "@testing-library/react";
import LapSidebar from "@/shared/components/ui/LapSidebar/LapSidebar";

const course = {
  id: "c1",
  title: "React 101",
  category: { id: "cat-1", name: "Programming" },
  difficultyLevel: { id: "lvl-1", name: "Intermediate" },
  durationMinute: 120,
  overallRating: 4.5,
  thumbnailImgPath: "",
  status: true,
  description: "Test",
  createdByUser: { id: "u1", fullName: "John", email: "", roles: [] },
  topics: [],
  enrollmentCount: 0,
  assessmentTitle: "",
  totalMark: 0,
  passingMark: 0,
};

describe("LapSidebar", () => {
  const onToggleCollapse = jest.fn();
  const onMobileClose = jest.fn();

  beforeEach(() => {
    jest.clearAllMocks();
  });

  it("renders course title and category", () => {
    render(
      <LapSidebar
        course={course}
        onToggleCollapse={onToggleCollapse}
        isCollapsed={false}
        isMobileOpen={false}
        onMobileClose={onMobileClose}
      />,
    );
    expect(screen.getAllByText("Course Syllabus").length).toBeGreaterThanOrEqual(1);
    expect(screen.getByText("Programming")).toBeInTheDocument();
  });

  it("renders collapse toggle button", () => {
    render(
      <LapSidebar
        course={course}
        onToggleCollapse={onToggleCollapse}
        isCollapsed={false}
        isMobileOpen={false}
        onMobileClose={onMobileClose}
      />,
    );
    const toggle = screen.getByTitle("Collapse sidebar");
    expect(toggle).toBeInTheDocument();
  });

  it("calls onToggleCollapse when toggle clicked", () => {
    render(
      <LapSidebar
        course={course}
        onToggleCollapse={onToggleCollapse}
        isCollapsed={false}
        isMobileOpen={false}
        onMobileClose={onMobileClose}
      />,
    );
    fireEvent.click(screen.getByTitle("Collapse sidebar"));
    expect(onToggleCollapse).toHaveBeenCalled();
  });

  it("shows expand title when collapsed", () => {
    render(
      <LapSidebar
        course={course}
        onToggleCollapse={onToggleCollapse}
        isCollapsed
        isMobileOpen={false}
        onMobileClose={onMobileClose}
      />,
    );
    expect(screen.getByTitle("Expand sidebar")).toBeInTheDocument();
  });

  it("applies collapsed class when isCollapsed is true", () => {
    const { container } = render(
      <LapSidebar
        course={course}
        onToggleCollapse={onToggleCollapse}
        isCollapsed
        isMobileOpen={false}
        onMobileClose={onMobileClose}
      />,
    );
    expect(container.querySelector(".co-sidebar--collapsed")).toBeInTheDocument();
  });

  it("shows backdrop when mobile open", () => {
    const { container } = render(
      <LapSidebar
        course={course}
        onToggleCollapse={onToggleCollapse}
        isCollapsed={false}
        isMobileOpen
        onMobileClose={onMobileClose}
      />,
    );
    expect(container.querySelector(".co-sidebar-backdrop")).toBeInTheDocument();
  });

  it("closes mobile sidebar when backdrop clicked", () => {
    const { container } = render(
      <LapSidebar
        course={course}
        onToggleCollapse={onToggleCollapse}
        isCollapsed={false}
        isMobileOpen
        onMobileClose={onMobileClose}
      />,
    );
    fireEvent.click(container.querySelector(".co-sidebar-backdrop")!);
    expect(onMobileClose).toHaveBeenCalled();
  });

  it("renders children", () => {
    render(
      <LapSidebar
        course={course}
        onToggleCollapse={onToggleCollapse}
        isCollapsed={false}
        isMobileOpen={false}
        onMobileClose={onMobileClose}
      >
        <nav>Sidebar content</nav>
      </LapSidebar>,
    );
    expect(screen.getByText("Sidebar content")).toBeInTheDocument();
  });

  it("shows mobile-open class when isMobileOpen", () => {
    const { container } = render(
      <LapSidebar
        course={course}
        onToggleCollapse={onToggleCollapse}
        isCollapsed={false}
        isMobileOpen
        onMobileClose={onMobileClose}
      />,
    );
    expect(
      container.querySelector(".co-sidebar--mobile-open"),
    ).toBeInTheDocument();
  });
});
