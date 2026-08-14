import { render, screen } from "@testing-library/react";
import "@testing-library/jest-dom";
import LapNavbar from "@/shared/components/layout/LapNavbar/LapNavbar";
import { BrowserRouter } from "react-router-dom";

// Mocking useAuth
jest.mock("@/core/providers/AuthProvider/useAuth", () => ({
  useAuth: () => ({
    isAuthenticated: false,
    isAdmin: false,
    isStudent: false,
    checkAuth: jest.fn(),
  }),
}));

// Mocking image import
jest.mock("@/assets/images/info-guide-logo.png", () => "logo.png");

// Mocking ThemeToggle
jest.mock("@/shared/components/ui/LapThemeToggle", () => ({
  LapThemeToggle: () => <div data-testid="theme-toggle" />,
}));

describe("LapNavbar", () => {
  it("renders navbar correctly", () => {
    render(
      <BrowserRouter>
        <LapNavbar />
      </BrowserRouter>
    );
    expect(screen.getByRole("banner")).toBeInTheDocument();
  });
});
