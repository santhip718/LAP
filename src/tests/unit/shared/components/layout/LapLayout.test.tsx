import { render, screen } from "@testing-library/react";
import "@testing-library/jest-dom";
import LapLayout from "@/shared/components/layout/LapLayout/LapLayout";

// Mocking react-router-dom
jest.mock("react-router-dom", () => ({
  ...jest.requireActual("react-router-dom"),
  Outlet: () => <div data-testid="outlet" />,
}));

// Mocking LapNavbar
jest.mock("@/shared/components/layout/LapNavbar/LapNavbar", () => () => <div data-testid="navbar" />);

describe("LapLayout", () => {
  it("renders navbar and outlet", () => {
    render(<LapLayout />);
    expect(screen.getByTestId("navbar")).toBeInTheDocument();
    expect(screen.getByTestId("outlet")).toBeInTheDocument();
  });
});
