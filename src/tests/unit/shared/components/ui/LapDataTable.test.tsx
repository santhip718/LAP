import "@testing-library/jest-dom";
import { render, screen, fireEvent } from "@testing-library/react";
import LapDataTable from "@/shared/components/ui/LapDataTable/LapDataTable";

interface CourseRow {
  id: string;
  title: string;
  duration: number;
  rating: number;
}

const columns = [
  { key: "title", label: "Title", sortable: true },
  { key: "duration", label: "Duration", sortable: true },
  { key: "rating", label: "Rating", sortable: false },
];

const data: CourseRow[] = [
  { id: "c1", title: "React Basics", duration: 120, rating: 4.5 },
  { id: "c2", title: "Advanced JS", duration: 180, rating: 4.8 },
  { id: "c3", title: "CSS Grid", duration: 90, rating: 4.2 },
];

describe("LapDataTable", () => {
  it("renders column headers", () => {
    render(<LapDataTable columns={columns} data={data} />);
    expect(screen.getByText("Title")).toBeInTheDocument();
    expect(screen.getByText("Duration")).toBeInTheDocument();
    expect(screen.getByText("Rating")).toBeInTheDocument();
  });

  it("renders data rows", () => {
    render(<LapDataTable columns={columns} data={data} />);
    expect(screen.getByText("React Basics")).toBeInTheDocument();
    expect(screen.getByText("Advanced JS")).toBeInTheDocument();
    expect(screen.getByText("CSS Grid")).toBeInTheDocument();
  });

  it("renders custom render cell", () => {
    const cols = [
      { key: "title", label: "Title", render: (v: unknown) => `★ ${v}` },
    ];
    render(<LapDataTable columns={cols} data={data} />);
    expect(screen.getByText("★ React Basics")).toBeInTheDocument();
  });

  it("shows empty state when no data", () => {
    render(<LapDataTable columns={columns} data={[]} />);
    expect(screen.getByText("No data available")).toBeInTheDocument();
  });

  it("sorts data when sortable header clicked", () => {
    render(<LapDataTable columns={columns} data={data} />);
    const titleHeader = screen.getByText("Title");
    fireEvent.click(titleHeader);
    const rows = screen.getAllByRole("row");
    expect(rows[1]).toHaveTextContent("Advanced JS");
    expect(rows[2]).toHaveTextContent("CSS Grid");
    expect(rows[3]).toHaveTextContent("React Basics");
  });

  it("reverses sort order on second click", () => {
    render(<LapDataTable columns={columns} data={data} />);
    const titleHeader = screen.getByText("Title");
    fireEvent.click(titleHeader);
    fireEvent.click(titleHeader);
    const rows = screen.getAllByRole("row");
    expect(rows[1]).toHaveTextContent("React Basics");
    expect(rows[3]).toHaveTextContent("Advanced JS");
  });

  it("paginates data with custom page size", () => {
    render(
      <LapDataTable columns={columns} data={data} pageSize={2} />,
    );
    expect(screen.getByText("React Basics")).toBeInTheDocument();
    expect(screen.getByText("Advanced JS")).toBeInTheDocument();
    expect(screen.queryByText("CSS Grid")).not.toBeInTheDocument();
  });

  it("navigates to next page on pagination", () => {
    render(
      <LapDataTable columns={columns} data={data} pageSize={2} />,
    );
    const nextBtn = document.querySelector(
      '[data-testid="KeyboardArrowRightIcon"]',
    );
    if (nextBtn) {
      fireEvent.click(nextBtn.closest("button")!);
      expect(screen.getByText("CSS Grid")).toBeInTheDocument();
    }
  });

  it("calls onRowClick when row clicked", () => {
    const onRowClick = jest.fn();
    render(
      <LapDataTable columns={columns} data={data} onRowClick={onRowClick} />,
    );
    fireEvent.click(screen.getByText("React Basics"));
    expect(onRowClick).toHaveBeenCalledWith(data[0]);
  });

  it("renders numeric sort correctly", () => {
    render(<LapDataTable columns={columns} data={data} />);
    const durationHeader = screen.getByText("Duration");
    fireEvent.click(durationHeader);
    const rows = screen.getAllByRole("row");
    expect(rows[1]).toHaveTextContent("90");
    expect(rows[3]).toHaveTextContent("180");
  });
});
