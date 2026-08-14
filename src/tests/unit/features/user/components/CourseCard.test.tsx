import "@testing-library/jest-dom";
import { render, screen, fireEvent } from "@testing-library/react";
import CourseCard from "@/features/user/components/CourseCard/CourseCard";

const mockNavigate = jest.fn();
jest.mock("react-router-dom", () => ({
  useNavigate: () => mockNavigate,
}));

jest.mock("@/shared/components/ui/LapTooltip/LapTooltip", () => {
  return {
    __esModule: true,
    default: ({ text, ...props }: { text: string; [key: string]: unknown }) => {
      const { variant, className, ...rest } = props;
      const Tag = variant === "h3" ? "h3" : variant === "h6" ? "h6" : "span";
      return <Tag className={className as string} {...rest}>{text}</Tag>;
    },
  };
});

const basicCourse = {
  id: "course-1",
  title: "React Basics",
  category: "Frontend",
  categoryId: "cat-1",
  duration: "10h",
  level: "Beginner",
  rating: "4.5",
  image: "http://example.com/img.jpg",
  alt: "React Basics thumbnail",
};

describe("CourseCard", () => {
  it("renders course info", () => {
    render(<CourseCard course={basicCourse} />);
    expect(screen.getByText("React Basics")).toBeInTheDocument();
    expect(screen.getByText("Frontend")).toBeInTheDocument();
    expect(screen.getByText("10h")).toBeInTheDocument();
    expect(screen.getByText("4.5")).toBeInTheDocument();
  });

  it("shows bestseller badge", () => {
    render(<CourseCard course={{ ...basicCourse, isBestseller: true }} />);
    expect(screen.getByText("Bestseller")).toBeInTheDocument();
  });

  it("shows fallback icon when no image", () => {
    render(<CourseCard course={{ ...basicCourse, image: "" }} />);
    expect(screen.getByText("school")).toBeInTheDocument();
  });

  it("shows enroll button by default", () => {
    render(<CourseCard course={basicCourse} />);
    expect(screen.getByText("Enroll +")).toBeInTheDocument();
  });

  it("shows resume button when enrolled and active", () => {
    render(
      <CourseCard
        course={basicCourse}
        enrollment={{ status: true, courseId: "course-1" } as never}
      />,
    );
    expect(screen.getByText("Resume")).toBeInTheDocument();
  });

  it("shows requested button when enrolled but not active", () => {
    render(
      <CourseCard
        course={basicCourse}
        enrollment={{ status: false, courseId: "course-1" } as never}
      />,
    );
    expect(screen.getByText("Requested")).toBeInTheDocument();
  });

  it("calls onEnroll when enroll button clicked", () => {
    const onEnroll = jest.fn();
    render(<CourseCard course={basicCourse} onEnroll={onEnroll} />);
    fireEvent.click(screen.getByText("Enroll +"));
    expect(onEnroll).toHaveBeenCalledWith("course-1");
  });
});
