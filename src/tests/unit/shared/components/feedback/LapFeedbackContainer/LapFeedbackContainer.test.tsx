import { render, act, screen } from "@testing-library/react";
import LapFeedbackContainer from "@/shared/components/feedback/LapFeedbackContainer/LapFeedbackContainer";
import { feedbackService } from "@/shared/services/feedback/feedbackService";

describe("LapFeedbackContainer", () => {
  it("renders both LapToast and LapConfirmDialog", () => {
    render(<LapFeedbackContainer />);

    act(() => {
      feedbackService.showToast("Toast works", "info");
    });

    expect(screen.getByText("Toast works")).toBeInTheDocument();
  });

  it("renders confirm dialog when showConfirm is called", () => {
    render(<LapFeedbackContainer />);

    act(() => {
      feedbackService.showConfirm({
        title: "Dialog title",
        message: "Dialog message",
      });
    });

    expect(screen.getByText("Dialog title")).toBeInTheDocument();
    expect(screen.getByText("Dialog message")).toBeInTheDocument();
  });
});
