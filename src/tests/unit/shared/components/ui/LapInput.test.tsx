import "@testing-library/jest-dom";
import { render, screen, fireEvent } from "@testing-library/react";
import LapInput from "@/shared/components/ui/LapInput/LapInput";

describe("LapInput", () => {
  it("renders input element", () => {
    render(<LapInput name="test" />);
    expect(screen.getByRole("textbox")).toBeInTheDocument();
  });

  it("renders label when provided", () => {
    render(<LapInput name="email" label="Email" />);
    expect(screen.getByText("Email")).toBeInTheDocument();
    expect(screen.getByLabelText("Email")).toBeInTheDocument();
  });

  it("renders error message", () => {
    render(<LapInput name="test" error="Required" />);
    expect(screen.getByText("Required")).toBeInTheDocument();
  });

  it("renders right element", () => {
    render(<LapInput name="test" rightElement={<span>Clear</span>} />);
    expect(screen.getByText("Clear")).toBeInTheDocument();
  });

  it("forwards value and onChange", () => {
    const onChange = jest.fn();
    render(<LapInput name="test" onChange={onChange} />);
    fireEvent.change(screen.getByRole("textbox"), { target: { value: "abc" } });
    expect(onChange).toHaveBeenCalled();
  });

  it("applies search variant class", () => {
    render(<LapInput name="test" type="search" />);
    expect(screen.getByRole("textbox").className).toContain("input--search");
  });

  it("applies ghost variant class", () => {
    render(<LapInput name="test" type="ghost" />);
    expect(screen.getByRole("textbox").className).toContain("input--ghost");
  });

  it("forwards disabled prop", () => {
    render(<LapInput name="test" disabled />);
    expect(screen.getByRole("textbox")).toBeDisabled();
  });

  it("uses htmlType prop", () => {
    render(<LapInput name="pass" htmlType="password" />);
    expect(screen.getByDisplayValue("")).toBeInTheDocument();
  });
});
