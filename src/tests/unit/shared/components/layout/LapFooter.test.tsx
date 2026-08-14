import { render, screen } from "@testing-library/react";
import "@testing-library/jest-dom";
import LapFooter from "@/shared/components/layout/LapFooter/LapFooter";

// Mocking the image import
jest.mock("@/assets/images/info-guide-logo.png", () => "logo.png");

describe("LapFooter", () => {
  it("renders footer content", () => {
    render(<LapFooter />);
    expect(screen.getByRole("contentinfo")).toBeInTheDocument();
  });
});
