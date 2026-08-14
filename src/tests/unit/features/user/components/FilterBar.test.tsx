import "@testing-library/jest-dom";
import { render, screen, fireEvent } from "@testing-library/react";
import FilterBar from "@/features/user/components/FilterBar/FilterBar";

jest.mock("@/shared/services/referenceData", () => ({
  referenceDataService: {
    getCategories: jest.fn().mockResolvedValue([
      { id: "cat-1", name: "Frontend" },
      { id: "cat-2", name: "Backend" },
    ]),
    getDifficultyLevels: jest.fn().mockResolvedValue([
      { id: "lvl-1", name: "Beginner" },
      { id: "lvl-2", name: "Advanced" },
    ]),
  },
}));

jest.mock("@/shared/hooks/useDebounce", () => ({
  useDebounce: (value: string) => value,
}));

describe("FilterBar", () => {
  it("renders search input and selects", () => {
    render(<FilterBar onFilterChange={jest.fn()} />);
    expect(
      screen.getByPlaceholderText("Search courses..."),
    ).toBeInTheDocument();
    expect(screen.getByText("All Categories")).toBeInTheDocument();
    expect(screen.getByText("All Levels")).toBeInTheDocument();
  });

  it("calls onFilterChange when search input changes", () => {
    const onFilterChange = jest.fn();
    render(<FilterBar onFilterChange={onFilterChange} />);

    fireEvent.change(screen.getByPlaceholderText("Search courses..."), {
      target: { value: "react" },
    });

    expect(onFilterChange).toHaveBeenCalledWith(
      expect.objectContaining({ search: "react" }),
    );
  });

  it("shows clear button when filters are active and clears on click", () => {
    const onFilterChange = jest.fn();
    render(<FilterBar onFilterChange={onFilterChange} />);

    fireEvent.change(screen.getByPlaceholderText("Search courses..."), {
      target: { value: "react" },
    });

    expect(screen.getByText("Clear")).toBeInTheDocument();

    fireEvent.click(screen.getByText("Clear"));
    expect(onFilterChange).toHaveBeenLastCalledWith(
      expect.objectContaining({ search: undefined }),
    );
  });
});
