import "@testing-library/jest-dom";
import { render } from "@testing-library/react";
import LapSpinnerv1 from "@/shared/components/ui/LapSpinnerv1/LapSpinnerv1";

describe("LapSpinnerv1", () => {
  it("renders the book spinner overlay", () => {
    const { container } = render(<LapSpinnerv1 />);
    expect(container.querySelector(".lap-spinner-overlay")).toBeInTheDocument();
    expect(container.querySelector(".book")).toBeInTheDocument();
  });
});
