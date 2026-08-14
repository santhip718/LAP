import "@testing-library/jest-dom";
import { render, screen, fireEvent } from "@testing-library/react";
import LapCurriculumAccordion from "@/shared/components/ui/LapCurriculumAccordion/LapCurriculumAccordion";
import type { LapContent, LapTopic } from "@/shared/types/ui.types";

const videoContent: LapContent = {
  id: "c1",
  title: "Intro to JSX",
  contentType: { id: "video", name: "Video" },
  videoUrl: "https://video.com/jsx",
  durationMinute: 10,
  sequenceOrder: 1,
  isCompleted: false,
};

const pdfContent: LapContent = {
  id: "c2",
  title: "React Handbook",
  contentType: { id: "pdf", name: "Pdf" },
  pdfFilePath: "/handbook.pdf",
  durationMinute: 0,
  sequenceOrder: 2,
  isCompleted: true,
};

const topics: LapTopic[] = [
  {
    id: "t1",
    name: "Getting Started",
    sequenceOrder: 1,
    metaSequenceOrder: 1,
    durationMinute: 30,
    contents: [videoContent, pdfContent],
    isCompleted: false,
  },
  {
    id: "t2",
    name: "Advanced Topics",
    sequenceOrder: 2,
    metaSequenceOrder: 2,
    durationMinute: 45,
    contents: [],
    isCompleted: true,
  },
];

describe("LapCurriculumAccordion", () => {
  it("renders topic names", () => {
    render(<LapCurriculumAccordion topics={topics} />);
    expect(screen.getByText("Getting Started")).toBeInTheDocument();
    expect(screen.getByText("Advanced Topics")).toBeInTheDocument();
  });

  it("renders content titles", () => {
    render(<LapCurriculumAccordion topics={topics} />);
    expect(screen.getByText("Intro to JSX")).toBeInTheDocument();
    expect(screen.getByText("React Handbook")).toBeInTheDocument();
  });

  it("shows video icon for video content", () => {
    const { container } = render(<LapCurriculumAccordion topics={topics} />);
    expect(container.querySelector(".lap-ca-icon-video")).toBeInTheDocument();
  });

  it("shows pdf icon for pdf content", () => {
    const { container } = render(<LapCurriculumAccordion topics={topics} />);
    expect(container.querySelector(".lap-ca-icon-pdf")).toBeInTheDocument();
  });

  it("renders topic duration in header", () => {
    render(<LapCurriculumAccordion topics={topics} />);
    expect(screen.getByText("30 min")).toBeInTheDocument();
    expect(screen.getByText("45 min")).toBeInTheDocument();
  });

  it("shows locked message for topic with no contents", () => {
    render(<LapCurriculumAccordion topics={topics} />);
    expect(
      screen.getByText("Locked until previous topic is complete."),
    ).toBeInTheDocument();
  });

  it("calls onContentClick when content row is clicked", () => {
    const onContentClick = jest.fn();
    render(
      <LapCurriculumAccordion topics={topics} onContentClick={onContentClick} />,
    );
    fireEvent.click(screen.getByText("Intro to JSX"));
    expect(onContentClick).toHaveBeenCalledWith(videoContent);
  });

  it("shows completion checkmark for completed content", () => {
    render(<LapCurriculumAccordion topics={topics} />);
    const checks = screen.getAllByText("check_circle");
    expect(checks.length).toBeGreaterThanOrEqual(1);
  });

  it("hides completion marks when showCompletion is false", () => {
    const { container } = render(
      <LapCurriculumAccordion topics={topics} showCompletion={false} />,
    );
    const topicCheck = container.querySelector(".lap-ca-num + span.material-symbols-outlined");
    expect(topicCheck).toBeNull();
  });

  it("renders topic sequence numbers", () => {
    render(<LapCurriculumAccordion topics={topics} />);
    expect(screen.getByText("1")).toBeInTheDocument();
    expect(screen.getByText("2")).toBeInTheDocument();
  });

  it("applies disabled CSS class when disabled", () => {
    const { container } = render(
      <LapCurriculumAccordion topics={topics} disabled />,
    );
    expect(container.querySelector(".lap-ca-disabled")).toBeInTheDocument();
  });

  it("does not render per-content duration when disabled", () => {
    render(<LapCurriculumAccordion topics={topics} disabled />);
    expect(screen.queryByText("10:00")).not.toBeInTheDocument();
    expect(screen.queryByText("1.2 MB")).not.toBeInTheDocument();
  });

  it("prevents content row click when disabled", () => {
    const onContentClick = jest.fn();
    render(
      <LapCurriculumAccordion
        topics={topics}
        onContentClick={onContentClick}
        disabled
      />,
    );
    fireEvent.click(screen.getByText("Intro to JSX"));
    expect(onContentClick).not.toHaveBeenCalled();
  });
});
