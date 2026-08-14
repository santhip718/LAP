import { render, screen, fireEvent } from "@testing-library/react";
import "@testing-library/jest-dom";
import LapCourseLayout from "@/shared/components/layout/LapCourseLayout/LapCourseLayout";

describe("LapCourseLayout", () => {
  const defaultProps = {
    sidebar: <div>Sidebar</div>,
    children: <div>Main Content</div>,
    isSidebarCollapsed: false,
    isMobileOpen: false,
    onMobileToggle: jest.fn(),
  };

  it("renders sidebar and main content", () => {
    render(<LapCourseLayout {...defaultProps} />);
    expect(screen.getByText("Sidebar")).toBeInTheDocument();
    expect(screen.getByText("Main Content")).toBeInTheDocument();
  });

  it("calls onMobileToggle when mobile toggle button is clicked", () => {
    render(<LapCourseLayout {...defaultProps} />);
    const toggleButton = screen.getByRole("button", { name: /open sidebar/i });
    fireEvent.click(toggleButton);
    expect(defaultProps.onMobileToggle).toHaveBeenCalledTimes(1);
  });
});
