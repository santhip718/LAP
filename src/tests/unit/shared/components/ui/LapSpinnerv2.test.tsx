import "@testing-library/jest-dom";
import { render } from "@testing-library/react";
import LapSpinnerv2 from "@/shared/components/ui/LapSpinnerv2/LapSpinnerv2";

describe("LapSpinnerv2", () => {
  it("renders the loader overlay", () => {
    const { container } = render(<LapSpinnerv2 />);
    expect(container.querySelector(".lap-spinner-overlay")).toBeInTheDocument();
    expect(container.querySelector(".loader")).toBeInTheDocument();
  });
});
