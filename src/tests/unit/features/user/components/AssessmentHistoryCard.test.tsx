import "@testing-library/jest-dom";
import { render, screen, fireEvent } from "@testing-library/react";
import AssessmentHistoryCard from "@/features/user/components/AssessmentHistoryCard/AssessmentHistoryCard";

const baseItem = {
  assessment_history_id: "hist-1",
  assessment_id: "assess-1",
  assessment_title: "React Basics Quiz",
  course_id: "course-1",
  course_title: "React Fundamentals",
  score: 85,
  passed: true,
  attempted_on: "2025-06-15T10:30:00Z",
};

describe("AssessmentHistoryCard", () => {
  it("renders assessment title", () => {
    render(<AssessmentHistoryCard item={baseItem} />);
    expect(screen.getByText("React Basics Quiz")).toBeInTheDocument();
  });

  it("shows Passed status for passed assessments", () => {
    render(<AssessmentHistoryCard item={baseItem} />);
    expect(screen.getByText("Passed")).toBeInTheDocument();
    expect(screen.getByText("Passed")).toHaveClass("ah-status-passed");
  });

  it("shows Failed status for failed assessments", () => {
    render(<AssessmentHistoryCard item={{ ...baseItem, passed: false }} />);
    expect(screen.getByText("Failed")).toBeInTheDocument();
    expect(screen.getByText("Failed")).toHaveClass("ah-status-failed");
  });

  it("shows score", () => {
    render(<AssessmentHistoryCard item={baseItem} />);
    expect(screen.getByText("85")).toBeInTheDocument();
  });

  it("shows course title", () => {
    render(<AssessmentHistoryCard item={baseItem} />);
    expect(screen.getByText("React Fundamentals")).toBeInTheDocument();
  });

  it("shows fallback title when assessment_title is missing", () => {
    render(<AssessmentHistoryCard item={{ ...baseItem, assessment_title: undefined }} />);
    expect(screen.getByText("Untitled Assessment")).toBeInTheDocument();
  });

  it("shows N/A when attempted_on is missing", () => {
    render(<AssessmentHistoryCard item={{ ...baseItem, attempted_on: undefined }} />);
    expect(screen.getAllByText("N/A").length).toBeGreaterThanOrEqual(1);
  });

  it("calls onClick with course_id and assessment_id", () => {
    const onClick = jest.fn();
    render(<AssessmentHistoryCard item={baseItem} onClick={onClick} />);
    fireEvent.click(screen.getByText("React Basics Quiz").closest(".ah-card")!);
    expect(onClick).toHaveBeenCalledWith("course-1", "assess-1");
  });

  it("shows quiz icon", () => {
    render(<AssessmentHistoryCard item={baseItem} />);
    expect(screen.getByText("quiz")).toBeInTheDocument();
  });

  it("shows trophy icon for score", () => {
    render(<AssessmentHistoryCard item={baseItem} />);
    expect(screen.getAllByText("emoji_events").length).toBeGreaterThanOrEqual(1);
  });
});
