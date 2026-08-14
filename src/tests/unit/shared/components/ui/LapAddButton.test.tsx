import "@testing-library/jest-dom";
import { render, screen, fireEvent } from "@testing-library/react";
import LapAddButton from "@/shared/components/ui/LapAddButton/LapAddButton";

describe("LapAddButton", () => {
  it("renders with default label and icon", () => {
    render(<LapAddButton />);
    expect(screen.getByText("Add")).toBeInTheDocument();
    expect(screen.getByText("add")).toBeInTheDocument();
  });

  it("renders custom label and icon", () => {
    render(<LapAddButton label="Create" icon="plus_one" />);
    expect(screen.getByText("Create")).toBeInTheDocument();
    expect(screen.getByText("plus_one")).toBeInTheDocument();
  });

  it("calls onClick when clicked", () => {
    const onClick = jest.fn();
    render(<LapAddButton onClick={onClick} />);
    fireEvent.click(screen.getByRole("button"));
    expect(onClick).toHaveBeenCalled();
  });
});
