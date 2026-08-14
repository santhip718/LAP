import "@testing-library/jest-dom/jest-globals";
import { render, screen } from "@testing-library/react";
import StatCard from "@/features/admin/components/StatCard/StatCard";

describe("StatCard", () => {
  it("renders label and value", () => {
    render(<StatCard label="Total Users" value="1,284" />);
    expect(screen.getByText("Total Users")).toBeInTheDocument();
    expect(screen.getByText("1,284")).toBeInTheDocument();
  });

  it("renders trend when provided", () => {
    render(<StatCard label="Score" value={95} trend={{ text: "+5% increase" }} />);
    expect(screen.getByText("+5% increase")).toBeInTheDocument();
  });

  it("renders progress bar when progress is provided", () => {
    const { container } = render(<StatCard label="Progress" value="75%" progress={75} />);
    const fill = container.querySelector(".statcard-progress-fill");
    expect(fill).toBeInTheDocument();
    expect(fill).toHaveStyle("width: 75%");
  });

  it("clamps progress to 0–100", () => {
    const { container: high } = render(<StatCard label="Test" value="x" progress={150} />);
    expect(high.querySelector(".statcard-progress-fill")).toHaveStyle("width: 100%");

    const { container: low } = render(<StatCard label="Test" value="x" progress={-10} />);
    expect(low.querySelector(".statcard-progress-fill")).toHaveStyle("width: 0%");
  });

  it("does not render trend or progress when not provided", () => {
    const { container } = render(<StatCard label="Label" value="Val" />);
    expect(container.querySelector(".statcard-trend")).not.toBeInTheDocument();
    expect(container.querySelector(".statcard-progress-track")).not.toBeInTheDocument();
  });

  it("uses default trend icon for emerald color", () => {
    render(<StatCard label="Label" value="Val" trend={{ text: "Up", color: "emerald" }} />);
    expect(screen.getByText("trending_up")).toBeInTheDocument();
  });

  it("uses insights icon for secondary color trend", () => {
    render(<StatCard label="Label" value="Val" trend={{ text: "Down", color: "secondary" }} />);
    expect(screen.getByText("insights")).toBeInTheDocument();
  });
});
