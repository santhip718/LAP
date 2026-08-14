import "@testing-library/jest-dom";
import { render, screen, fireEvent } from "@testing-library/react";
import CourseHero from "@/features/user/components/CourseHero/CourseHero";
import type { CourseDetail } from "@/features/user/services/courseDetailService";

const mockNavigate = jest.fn();
jest.mock("react-router-dom", () => ({
  useNavigate: () => mockNavigate,
}));

jest.mock("@/shared/components/ui/LapTooltip/LapTooltip", () => {
  return {
    __esModule: true,
    default: ({ text, ...props }: { text: string; [key: string]: unknown }) => {
      const { variant, className, maxLines, ...rest } = props;
      const Tag = variant === "h3" ? "h3" : variant === "h6" ? "h6" : "span";
      return <Tag className={className as string} {...rest}>{text}</Tag>;
    },
  };
});

const baseCourse: CourseDetail = {
  id: "course-1",
  title: "React Masterclass",
  description: "Learn React from scratch",
  thumbnailImgPath: "http://example.com/thumb.jpg",
  enrollmentCount: 1234,
  overallRating: 4.5,
  topics: [{
    id: "t1", name: "Intro", sequenceOrder: 1, metaSequenceOrder: 1, durationMinute: 30,
    contents: [{ id: "c1", title: "Video 1", contentType: { id: "ct1", name: "Video" }, durationMinute: 30, sequenceOrder: 1 }],
  }],
  category: { id: "cat-1", name: "Frontend" },
  difficultyLevel: { id: "lvl-1", name: "Beginner" },
  durationMinute: 600,
  status: true,
  createdByUser: { id: "u1", fullName: "John", email: "john@test.com", roles: ["admin"] },
  assessmentTitle: "Test",
  totalMark: 100,
  passingMark: 60,
};

const defaultProps = {
  course: baseCourse,
  durationLabel: "10h",
  isEnrolled: false,
  canResume: false,
  courseId: "course-1",
  onEnroll: jest.fn(),
  onRateClick: jest.fn(),
};

describe("CourseHero", () => {
  it("renders course title and description", () => {
    render(<CourseHero {...defaultProps} />);
    expect(screen.getByText("React Masterclass")).toBeInTheDocument();
    expect(screen.getByText("Learn react from scratch")).toBeInTheDocument();
  });

  it("shows enrollment count with students label", () => {
    render(<CourseHero {...defaultProps} />);
    expect(screen.getByText("1,234 students")).toBeInTheDocument();
  });

  it("shows duration", () => {
    render(<CourseHero {...defaultProps} />);
    expect(screen.getByText("10h total")).toBeInTheDocument();
  });

  it("shows rating", () => {
    render(<CourseHero {...defaultProps} />);
    expect(screen.getByText("4.5/5")).toBeInTheDocument();
  });

  it("shows Enroll Now for not enrolled", () => {
    render(<CourseHero {...defaultProps} />);
    expect(screen.getByText("Enroll Now")).toBeInTheDocument();
  });

  it("shows Resume Course when enrolled and can resume", () => {
    render(<CourseHero {...defaultProps} isEnrolled canResume />);
    expect(screen.getByText("Resume Course")).toBeInTheDocument();
  });

  it("shows Requested when enrolled but cannot resume", () => {
    render(<CourseHero {...defaultProps} isEnrolled canResume={false} />);
    expect(screen.getByText("Requested")).toBeInTheDocument();
  });

  it("shows Rate this Course button", () => {
    render(<CourseHero {...defaultProps} />);
    expect(screen.getByText("Rate this Course")).toBeInTheDocument();
  });

  it("calls onEnroll when Enroll Now clicked", () => {
    const onEnroll = jest.fn();
    render(<CourseHero {...defaultProps} onEnroll={onEnroll} />);
    fireEvent.click(screen.getByText("Enroll Now"));
    expect(onEnroll).toHaveBeenCalledWith("course-1");
  });

  it("calls onRateClick when rate button clicked", () => {
    const onRateClick = jest.fn();
    render(<CourseHero {...defaultProps} onRateClick={onRateClick} />);
    fireEvent.click(screen.getByText("Rate this Course"));
    expect(onRateClick).toHaveBeenCalled();
  });

  it("shows play icon on image", () => {
    render(<CourseHero {...defaultProps} />);
    expect(screen.getByText("play_circle")).toBeInTheDocument();
  });
});
