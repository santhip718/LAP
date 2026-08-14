import { render, screen } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import LapRouteErrorBoundary from "@/shared/components/feedback/LapErrorBoundary/LapRouteErrorBoundary";

describe("LapRouteErrorBoundary", () => {
  it("renders children when there is no error", () => {
    render(
      <LapRouteErrorBoundary>
        <div>Route content</div>
      </LapRouteErrorBoundary>
    );

    expect(screen.getByText("Route content")).toBeInTheDocument();
  });

  it("renders LapGenericErrorFallback on error", () => {
    const ErrorThrower = () => {
      throw new Error("Route error");
    };

    jest.spyOn(console, "error").mockImplementation(() => {});

    render(
      <MemoryRouter>
        <LapRouteErrorBoundary>
          <ErrorThrower />
        </LapRouteErrorBoundary>
      </MemoryRouter>
    );

    expect(screen.getByText("Oops! Something Went Wrong")).toBeInTheDocument();
    expect(screen.getByText("Reload Page")).toBeInTheDocument();

    jest.restoreAllMocks();
  });
});
