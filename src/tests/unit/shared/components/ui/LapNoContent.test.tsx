import "@testing-library/jest-dom";
import { render, screen } from "@testing-library/react";
import LapNoContent from "@/shared/components/ui/LapNoContent/LapNoContent";

describe("LapNoContent", () => {
  it("renders title and message", () => {
    render(<LapNoContent title="No items" message="Nothing to show." />);
    expect(screen.getByText("No items")).toBeInTheDocument();
    expect(screen.getByText("Nothing to show.")).toBeInTheDocument();
  });

  it("renders default icon", () => {
    render(<LapNoContent title="Empty" message="No data" />);
    expect(screen.getByText("inbox")).toBeInTheDocument();
  });

  it("renders custom icon", () => {
    render(<LapNoContent icon="search_off" title="Empty" message="No data" />);
    expect(screen.getByText("search_off")).toBeInTheDocument();
  });

  it("renders children", () => {
    render(
      <LapNoContent title="Empty" message="No data">
        <button>Retry</button>
      </LapNoContent>,
    );
    expect(screen.getByText("Retry")).toBeInTheDocument();
  });

  it("applies custom className", () => {
    const { container } = render(
      <LapNoContent title="Empty" message="No data" className="custom" />,
    );
    expect(container.querySelector(".lap-no-content.custom")).toBeInTheDocument();
  });
});
