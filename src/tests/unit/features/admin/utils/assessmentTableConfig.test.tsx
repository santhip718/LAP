import "@testing-library/jest-dom/jest-globals";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import {
  getAssessmentStatus,
  assessmentFilterOptions,
  buildAssessmentColumns,
} from "@/features/admin/utils/assessmentTableConfig";
import type { AssessmentOverviewDto } from "@/shared/services/api/models/assessmentOverviewDto";

describe("getAssessmentStatus", () => {
  const base: AssessmentOverviewDto = {
    id: "1",
    title: "Test",
    total_mark: 100,
    passing_mark: 50,
    duration_minute: 60,
  };

  it("returns Draft when passing_mark is null", () => {
    expect(getAssessmentStatus({ ...base, passing_mark: undefined })).toBe("Draft");
  });

  it("returns Draft when total_mark is null", () => {
    expect(getAssessmentStatus({ ...base, total_mark: undefined })).toBe("Draft");
  });

  it("returns Inactive when total_mark is 0", () => {
    expect(getAssessmentStatus({ ...base, total_mark: 0 })).toBe("Inactive");
  });

  it("returns Inactive when duration_minute is null", () => {
    expect(getAssessmentStatus({ ...base, duration_minute: undefined })).toBe("Inactive");
  });

  it("returns Inactive when duration_minute is 0", () => {
    expect(getAssessmentStatus({ ...base, duration_minute: 0 })).toBe("Inactive");
  });

  it("returns Active when all conditions are met", () => {
    expect(getAssessmentStatus(base)).toBe("Active");
  });
});

describe("assessmentFilterOptions", () => {
  it("has four filter options", () => {
    expect(assessmentFilterOptions).toHaveLength(4);
  });

  it("includes all expected filter values", () => {
    const values = assessmentFilterOptions.map((o) => o.value);
    expect(values).toEqual(["all", "Active", "Draft", "Inactive"]);
  });
});

describe("buildAssessmentColumns", () => {
  const onDelete = jest.fn();
  const columns = buildAssessmentColumns(onDelete);

  it("returns 6 columns", () => {
    expect(columns).toHaveLength(6);
  });

  it("has correct column keys", () => {
    const keys = columns.map((c) => c.key);
    expect(keys).toEqual(["title", "course", "duration_minute", "passing_mark", "total_mark", "actions"]);
  });

  it("renders Assessment column", () => {
    render(<table><tbody><tr>{columns[0].render!("My Assessment", {} as AssessmentOverviewDto, 0)}</tr></tbody></table>);
    expect(screen.getByText("My Assessment")).toBeInTheDocument();
  });

  it("renders Untitled when assessment title is empty", () => {
    const { container } = render(<table><tbody><tr>{columns[0].render!("", {} as AssessmentOverviewDto, 0)}</tr></tbody></table>);
    expect(container.textContent).toMatch(/Untitled/);
  });

  it("renders Course column with course title", () => {
    const course = { title: "Math 101" };
    render(<table><tbody><tr>{columns[1].render!(course, {} as AssessmentOverviewDto, 0)}</tr></tbody></table>);
    expect(screen.getByText("Math 101")).toBeInTheDocument();
  });

  it("renders Course column with em-dash when no title", () => {
    const course = {};
    const { container } = render(<table><tbody><tr>{columns[1].render!(course, {} as AssessmentOverviewDto, 0)}</tr></tbody></table>);
    expect(container.textContent).toMatch(/—/);
  });

  it("renders Duration column with minutes", () => {
    render(<table><tbody><tr>{columns[2].render!(45, {} as AssessmentOverviewDto, 0)}</tr></tbody></table>);
    expect(screen.getByText("45 min")).toBeInTheDocument();
  });

  it("renders Duration column with em-dash when null", () => {
    const { container } = render(<table><tbody><tr>{columns[2].render!(null, {} as AssessmentOverviewDto, 0)}</tr></tbody></table>);
    expect(container.textContent).toMatch(/—/);
  });

  it("calls onDelete when delete button clicked", async () => {
    const user = userEvent.setup();
    const row: AssessmentOverviewDto = { id: "123", title: "Test" };
    const deleteColumns = buildAssessmentColumns(onDelete);
    render(<table><tbody><tr>{deleteColumns[5].render!("", row, 0)}</tr></tbody></table>);
    await user.click(screen.getByLabelText("Delete assessment"));
    expect(onDelete).toHaveBeenCalledWith(row);
  });
});
