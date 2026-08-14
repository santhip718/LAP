import { render, screen, fireEvent } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import LapGenericErrorFallback from "@/shared/components/feedback/LapErrorBoundary/LapGenericErrorFallback";

const renderWithRouter = (ui: React.ReactElement) =>
  render(<MemoryRouter>{ui}</MemoryRouter>);

describe("LapGenericErrorFallback", () => {
  it("renders fallback title and message", () => {
    renderWithRouter(<LapGenericErrorFallback />);

    expect(screen.getByText("Oops! Something Went Wrong")).toBeInTheDocument();
    expect(
      screen.getByText(/We encountered an unexpected error/)
    ).toBeInTheDocument();
  });

  it("does not show error details by default in test mode", () => {
    renderWithRouter(<LapGenericErrorFallback showDetails={false} />);

    expect(screen.queryByText("Error Details")).not.toBeInTheDocument();
  });

  it("shows error details when showDetails is true", () => {
    const error = new Error("Something broke");
    renderWithRouter(
      <LapGenericErrorFallback error={error} showDetails={true} />
    );

    expect(screen.getByText("Error Details")).toBeInTheDocument();
    expect(screen.getByText(/Something broke/)).toBeInTheDocument();
  });

  it("renders Reload Page and Go Home buttons", () => {
    renderWithRouter(<LapGenericErrorFallback />);

    expect(screen.getByText("Reload Page")).toBeInTheDocument();
    expect(screen.getByText("Go Home")).toBeInTheDocument();
  });

  it("renders Try Again button when onReset is provided", () => {
    renderWithRouter(
      <LapGenericErrorFallback onReset={jest.fn()} />
    );

    expect(screen.getByText("Try Again")).toBeInTheDocument();
  });

  it("does not render Try Again button when onReset is not provided", () => {
    renderWithRouter(<LapGenericErrorFallback />);

    expect(screen.queryByText("Try Again")).not.toBeInTheDocument();
  });

  it("calls onReset when Try Again is clicked", () => {
    const onReset = jest.fn();
    renderWithRouter(
      <LapGenericErrorFallback onReset={onReset} />
    );

    fireEvent.click(screen.getByText("Try Again"));
    expect(onReset).toHaveBeenCalledTimes(1);
  });

  it("renders Go Home button", () => {
    renderWithRouter(<LapGenericErrorFallback />);
    const goHomeBtn = screen.getByText("Go Home");
    const button = goHomeBtn.closest("button");
    expect(button).toBeInTheDocument();
    expect(button).not.toBeDisabled();
  });
});
