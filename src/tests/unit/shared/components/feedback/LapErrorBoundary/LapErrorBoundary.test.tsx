import React from "react";
import { render, screen, fireEvent } from "@testing-library/react";
import LapErrorBoundary from "@/shared/components/feedback/LapErrorBoundary/LapErrorBoundary";

const ErrorThrower = ({ shouldThrow = false }: { shouldThrow?: boolean }) => {
  if (shouldThrow) {
    throw new Error("Test error");
  }
  return <div>Normal content</div>;
};

beforeEach(() => {
  jest.spyOn(console, "error").mockImplementation(() => {});
});

afterEach(() => {
  jest.restoreAllMocks();
});

describe("LapErrorBoundary", () => {
  it("renders children when there is no error", () => {
    render(
      <LapErrorBoundary>
        <div>Safe content</div>
      </LapErrorBoundary>
    );

    expect(screen.getByText("Safe content")).toBeInTheDocument();
  });

  it("renders default fallback when child throws", () => {
    render(
      <LapErrorBoundary>
        <ErrorThrower shouldThrow={true} />
      </LapErrorBoundary>
    );

    expect(screen.getByText("Something went wrong.")).toBeInTheDocument();
    expect(screen.getByText("Try Again")).toBeInTheDocument();
  });

  it("renders custom fallback when provided", () => {
    render(
      <LapErrorBoundary fallback={<div>Custom error UI</div>}>
        <ErrorThrower shouldThrow={true} />
      </LapErrorBoundary>
    );

    expect(screen.getByText("Custom error UI")).toBeInTheDocument();
    expect(screen.queryByText("Something went wrong.")).not.toBeInTheDocument();
  });

  it("shows error details in verbose mode", () => {
    render(
      <LapErrorBoundary verbose={true}>
        <ErrorThrower shouldThrow={true} />
      </LapErrorBoundary>
    );

    expect(screen.getByText(/Test error/)).toBeInTheDocument();
  });

  it("resets error state when Try Again is clicked", () => {
    function TestWrapper() {
      const [throwing, setThrowing] = React.useState(true);
      return (
        <div>
          <button data-testid="stop-throw" onClick={() => setThrowing(false)}>
            Stop throw
          </button>
          <LapErrorBoundary>
            <ErrorThrower shouldThrow={throwing} />
          </LapErrorBoundary>
        </div>
      );
    }

    render(<TestWrapper />);

    expect(screen.getByText("Something went wrong.")).toBeInTheDocument();

    fireEvent.click(screen.getByTestId("stop-throw"));

    fireEvent.click(screen.getByText("Try Again"));

    expect(screen.getByText("Normal content")).toBeInTheDocument();
  });

  it("calls onReset when Try Again is clicked", () => {
    const onReset = jest.fn();

    render(
      <LapErrorBoundary onReset={onReset}>
        <ErrorThrower shouldThrow={true} />
      </LapErrorBoundary>
    );

    fireEvent.click(screen.getByText("Try Again"));
    expect(onReset).toHaveBeenCalledTimes(1);
  });
});
