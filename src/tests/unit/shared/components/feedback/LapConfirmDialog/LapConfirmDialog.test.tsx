import { render, screen, fireEvent, act } from "@testing-library/react";
import LapConfirmDialog from "@/shared/components/feedback/LapConfirmDialog/LapConfirmDialog";
import { feedbackService } from "@/shared/services/feedback/feedbackService";

describe("LapConfirmDialog", () => {
  it("renders nothing by default", () => {
    const { container } = render(<LapConfirmDialog />);
    const dialog = container.querySelector(".MuiDialog-root");
    expect(dialog).not.toBeInTheDocument();
  });

  it("opens dialog when confirm:show event fires", () => {
    render(<LapConfirmDialog />);

    act(() => {
      feedbackService.showConfirm({
        title: "Delete?",
        message: "Are you sure?",
      });
    });

    expect(screen.getByText("Delete?")).toBeInTheDocument();
    expect(screen.getByText("Are you sure?")).toBeInTheDocument();
  });

  it("renders custom button labels when provided", () => {
    render(<LapConfirmDialog />);

    act(() => {
      feedbackService.showConfirm({
        title: "Custom",
        message: "Proceed?",
        confirmLabel: "Yes",
        cancelLabel: "No",
      });
    });

    expect(screen.getByText("Yes")).toBeInTheDocument();
    expect(screen.getByText("No")).toBeInTheDocument();
  });

  it("renders default button labels when not provided", () => {
    render(<LapConfirmDialog />);

    act(() => {
      feedbackService.showConfirm({
        title: "My Dialog",
        message: "Proceed?",
      });
    });

    expect(screen.getByText("My Dialog")).toBeInTheDocument();
    expect(screen.getByText("Cancel")).toBeInTheDocument();
    const confirmButtons = screen.getAllByText("Confirm");
    expect(confirmButtons.length).toBeGreaterThanOrEqual(1);
  });

  it("triggers confirm action when Confirm button is clicked", async () => {
    render(<LapConfirmDialog />);

    let result: boolean | undefined;
    act(() => {
      feedbackService.showConfirm({
        title: "Test",
        message: "Proceed?",
      }).then((res) => { result = res; });
    });

    expect(screen.getByText("Proceed?")).toBeInTheDocument();

    await act(async () => {
      fireEvent.click(screen.getByText("Confirm"));
    });

    expect(result).toBe(true);
  });
});
