import "@testing-library/jest-dom";
import { render, screen } from "@testing-library/react";
import CourseCanvas from "@/features/user/components/CourseCanvas/CourseCanvas";

describe("CourseCanvas", () => {
  it("shows placeholder when content is null", () => {
    render(<CourseCanvas content={null} />);
    expect(screen.getByText("Select a lesson to begin")).toBeInTheDocument();
    expect(screen.getByText("play_circle")).toBeInTheDocument();
  });

  it("shows no preview for unsupported content type", () => {
    render(
      <CourseCanvas
        content={{
          id: "c1",
          title: "Reading Material",
          contentType: { id: "ct3", name: "Article" as unknown as "Video" | "Pdf" },
          durationMinute: 10,
          sequenceOrder: 1,
        }}
      />,
    );
    expect(screen.getByText("No preview available")).toBeInTheDocument();
    expect(screen.getByText("description")).toBeInTheDocument();
  });

  it("renders YouTube iframe for video content", () => {
    render(
      <CourseCanvas
        content={{
          id: "c2",
          title: "Intro Video",
          contentType: { id: "ct1", name: "Video" },
          videoUrl: "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
          durationMinute: 5,
          sequenceOrder: 1,
        }}
      />,
    );
    const iframe = screen.getByTitle("Intro Video");
    expect(iframe).toBeInTheDocument();
    expect(iframe.tagName).toBe("IFRAME");
  });

  it("renders PDF iframe for PDF content with pdfBase64", () => {
    render(
      <CourseCanvas
        content={{
          id: "c3",
          title: "Course Notes",
          contentType: { id: "ct2", name: "Pdf" },
          pdfBase64: "data:application/pdf;base64,JVBERi0",
          durationMinute: 15,
          sequenceOrder: 2,
        }}
      />,
    );
    const iframe = screen.getByTitle("Course Notes");
    expect(iframe).toBeInTheDocument();
    expect(iframe).toHaveAttribute("src", "data:application/pdf;base64,JVBERi0");
  });

  it("renders PDF iframe for PDF content with pdfFilePath", () => {
    render(
      <CourseCanvas
        content={{
          id: "c4",
          title: "Slides",
          contentType: { id: "ct2", name: "Pdf" },
          pdfFilePath: "/files/slides.pdf",
          durationMinute: 10,
          sequenceOrder: 3,
        }}
      />,
    );
    const iframe = screen.getByTitle("Slides");
    expect(iframe).toHaveAttribute("src", "/files/slides.pdf");
  });

  it("converts youtu.be short URL to embed URL", () => {
    render(
      <CourseCanvas
        content={{
          id: "c5",
          title: "Short Link",
          contentType: { id: "ct1", name: "Video" },
          videoUrl: "https://youtu.be/abc123",
          durationMinute: 3,
          sequenceOrder: 4,
        }}
      />,
    );
    const iframe = screen.getByTitle("Short Link");
    expect(iframe).toHaveAttribute(
      "src",
      "https://www.youtube.com/embed/abc123",
    );
  });
});
