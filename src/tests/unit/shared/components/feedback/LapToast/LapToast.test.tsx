import { render, act, screen } from "@testing-library/react";
import LapToast from "@/shared/components/feedback/LapToast/LapToast";
import { feedbackService } from "@/shared/services/feedback/feedbackService";

describe("LapToast", () => {
  it("renders nothing when no toast is shown", () => {
    const { container } = render(<LapToast />);
    expect(container.firstChild).toBeNull();
  });

  it("displays toast when feedbackService emits toast:show", () => {
    render(<LapToast />);

    act(() => {
      feedbackService.showToast("Test message", "success");
    });

    expect(screen.getByText("Test message")).toBeInTheDocument();
  });

  it("displays severe toast", () => {
    render(<LapToast />);

    act(() => {
      feedbackService.showToast("Warning!", "warning");
    });

    expect(screen.getByText("Warning!")).toBeInTheDocument();
  });

  it("can show multiple toasts sequentially", () => {
    render(<LapToast />);

    act(() => {
      feedbackService.showToast("First toast", "info");
    });

    expect(screen.getByText("First toast")).toBeInTheDocument();

    act(() => {
      feedbackService.showToast("Second toast", "success");
    });

    expect(screen.getByText("Second toast")).toBeInTheDocument();
  });
});
