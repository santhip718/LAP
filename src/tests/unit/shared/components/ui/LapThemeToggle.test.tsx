import "@testing-library/jest-dom";
import { render, screen, fireEvent } from "@testing-library/react";
import { LapThemeToggle } from "@/shared/components/ui/LapThemeToggle";

const mockToggleTheme = jest.fn();
const mockThemeState: { mode: string; toggleTheme: jest.Mock } = {
  mode: "light",
  toggleTheme: mockToggleTheme,
};

jest.mock("@/core/providers/ThemeProvider", () => ({
  useAppTheme: () => mockThemeState,
}));

describe("LapThemeToggle", () => {
  beforeEach(() => {
    jest.clearAllMocks();
    mockThemeState.mode = "light";
  });

  it("renders dark mode icon when theme is light", () => {
    render(<LapThemeToggle />);
    expect(screen.getByLabelText("Toggle theme")).toBeInTheDocument();
  });

  it("renders light mode icon when theme is dark", () => {
    mockThemeState.mode = "dark";
    render(<LapThemeToggle />);
    expect(screen.getByLabelText("Toggle theme")).toBeInTheDocument();
  });

  it("calls toggleTheme on click", () => {
    render(<LapThemeToggle />);
    fireEvent.click(screen.getByLabelText("Toggle theme"));
    expect(mockToggleTheme).toHaveBeenCalled();
  });

  it("applies custom className", () => {
    const { container } = render(<LapThemeToggle className="custom-toggle" />);
    expect(container.querySelector(".custom-toggle")).toBeInTheDocument();
  });
});
