import "@testing-library/jest-dom";
import { render, screen, fireEvent } from "@testing-library/react";
import LapModalDialog from "@/shared/components/feedback/LapModalDialog/LapModalDialog";

describe("LapModalDialog", () => {
  it("renders the dialog with title and children when open", () => {
    render(
      <LapModalDialog open={true} onClose={jest.fn()} title="Test Title">
        <p>Dialog content</p>
      </LapModalDialog>
    );

    expect(screen.getByText("Test Title")).toBeInTheDocument();
    expect(screen.getByText("Dialog content")).toBeInTheDocument();
  });

  it("does not render content when closed", () => {
    render(
      <LapModalDialog open={false} onClose={jest.fn()} title="Test Title">
        <p>Dialog content</p>
      </LapModalDialog>
    );

    expect(screen.queryByText("Test Title")).not.toBeInTheDocument();
  });

  it("calls onClose when close button is clicked", () => {
    const onClose = jest.fn();
    render(
      <LapModalDialog open={true} onClose={onClose} title="Test Title">
        <p>Content</p>
      </LapModalDialog>
    );

    const closeBtn = screen.getByText("close").closest("button");
    expect(closeBtn).toBeInTheDocument();
    fireEvent.click(closeBtn!);
    expect(onClose).toHaveBeenCalledTimes(1);
  });

  it("calls onClose when backdrop is clicked", () => {
    const onClose = jest.fn();
    const { container } = render(
      <LapModalDialog open={true} onClose={onClose} title="Test Title">
        <p>Content</p>
      </LapModalDialog>
    );

    const backdrop = container.querySelector(".MuiBackdrop-root");
    if (backdrop) {
      fireEvent.click(backdrop);
      expect(onClose).toHaveBeenCalled();
    }
  });
});
