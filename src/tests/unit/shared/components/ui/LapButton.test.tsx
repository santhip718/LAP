import "@testing-library/jest-dom";
import { render, screen, fireEvent } from "@testing-library/react";
import LapButton from "@/shared/components/ui/LapButton/LapButton";

describe("LapButton", () => {
  it("renders children text", () => {
    render(<LapButton>Click me</LapButton>);
    expect(screen.getByText("Click me")).toBeInTheDocument();
  });

  it("applies default type class btn--primary", () => {
    render(<LapButton>Btn</LapButton>);
    const btn = screen.getByRole("button");
    expect(btn.className).toContain("btn--primary");
  });

  it("applies custom type class", () => {
    render(<LapButton type="secondary">Btn</LapButton>);
    expect(screen.getByRole("button").className).toContain("btn--secondary");
  });

  it("applies outline class", () => {
    render(<LapButton type="outline">Btn</LapButton>);
    expect(screen.getByRole("button").className).toContain("btn--outline");
  });

  it("applies ghost class", () => {
    render(<LapButton type="ghost">Btn</LapButton>);
    expect(screen.getByRole("button").className).toContain("btn--ghost");
  });

  it("renders icon when provided", () => {
    render(<LapButton icon={<span>🔍</span>}>Search</LapButton>);
    expect(screen.getByText("🔍")).toBeInTheDocument();
  });

  it("shows spinner when loading and hides text", () => {
    render(<LapButton loading>Submit</LapButton>);
    expect(screen.getByRole("button").className).toContain("btn--loading");
    expect(screen.getByText("Submit")).toBeInTheDocument();
    expect(screen.queryByText("Submit")).toBeInTheDocument();
  });

  it("does not render icon when loading", () => {
    render(
      <LapButton loading icon={<span>🔍</span>}>
        Search
      </LapButton>,
    );
    expect(screen.queryByText("🔍")).not.toBeInTheDocument();
  });

  it("disables button when loading", () => {
    render(<LapButton loading>Save</LapButton>);
    expect(screen.getByRole("button")).toBeDisabled();
  });

  it("disables button when disabled prop is true", () => {
    render(<LapButton disabled>Save</LapButton>);
    expect(screen.getByRole("button")).toBeDisabled();
  });

  it("calls onClick when clicked", () => {
    const onClick = jest.fn();
    render(<LapButton onClick={onClick}>Click</LapButton>);
    fireEvent.click(screen.getByRole("button"));
    expect(onClick).toHaveBeenCalled();
  });

  it("applies fullWidth class", () => {
    render(<LapButton fullWidth>Wide</LapButton>);
    expect(screen.getByRole("button").className).toContain("btn--full-width");
  });

  it("applies htmlType submit", () => {
    render(<LapButton htmlType="submit">Go</LapButton>);
    expect(screen.getByRole("button")).toHaveAttribute("type", "submit");
  });

  it("applies logout type class", () => {
    render(<LapButton type="logout">Logout</LapButton>);
    expect(screen.getByRole("button").className).toContain("btn--logout");
  });
});
