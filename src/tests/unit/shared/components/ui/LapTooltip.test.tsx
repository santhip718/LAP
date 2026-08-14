import "@testing-library/jest-dom";
import { render, screen } from "@testing-library/react";
import LapTooltip from "@/shared/components/ui/LapTooltip/LapTooltip";

describe("LapTooltip", () => {
  it("renders text with capitalised first letter", () => {
    render(<LapTooltip text="hello world" />);
    expect(screen.getByText("Hello world")).toBeInTheDocument();
  });

  it("renders variant prop via MUI Typography", () => {
    render(<LapTooltip text="test" variant="h4" />);
    const el = screen.getByText("Test");
    expect(el.tagName).toBe("H4");
  });

  it("applies maxLines via sx prop", () => {
    render(<LapTooltip text="long text" maxLines={2} />);
    const el = screen.getByText("Long text");
    expect(el).toBeInTheDocument();
  });
});
