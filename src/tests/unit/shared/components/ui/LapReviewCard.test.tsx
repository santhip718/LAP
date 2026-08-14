import "@testing-library/jest-dom";
import { render, screen, fireEvent } from "@testing-library/react";
import LapReviewCard from "@/shared/components/ui/LapReviewCard/LapReviewCard";

const baseReview = {
  id: "rev-1",
  user_full_name: "John Doe",
  rating: 4,
  review_text: "Great course!",
  date_created: "2025-06-15T10:00:00Z",
};

describe("LapReviewCard", () => {
  it("renders reviewer name", () => {
    render(<LapReviewCard review={baseReview} />);
    expect(screen.getByText("John Doe")).toBeInTheDocument();
  });

  it("shows Anonymous when no name", () => {
    render(<LapReviewCard review={{ ...baseReview, user_full_name: null }} />);
    expect(screen.getByText("Anonymous")).toBeInTheDocument();
  });

  it("renders initials avatar", () => {
    render(<LapReviewCard review={baseReview} />);
    const avatar = document.querySelector(".lap-review-card__avatar");
    expect(avatar).toHaveAttribute("data-initials", "JD");
  });

  it("renders star rating", () => {
    render(<LapReviewCard review={baseReview} />);
    const stars = document.querySelectorAll(".lap-review-card__star");
    expect(stars.length).toBe(5);
  });

  it("renders review text", () => {
    render(<LapReviewCard review={baseReview} />);
    expect(screen.getByText("Great course!")).toBeInTheDocument();
  });

  it("does not render text section when review_text is empty", () => {
    render(<LapReviewCard review={{ ...baseReview, review_text: "" }} />);
    expect(screen.queryByText("Great course!")).not.toBeInTheDocument();
  });

  it("renders formatted date", () => {
    render(<LapReviewCard review={baseReview} />);
    expect(screen.getByText("Jun 15, 2025")).toBeInTheDocument();
  });

  it("shows edit and delete buttons when isOwn is true", () => {
    render(<LapReviewCard review={baseReview} isOwn />);
    expect(screen.getByTitle("Edit review")).toBeInTheDocument();
    expect(screen.getByTitle("Delete review")).toBeInTheDocument();
  });

  it("does not show action buttons when isOwn is false", () => {
    render(<LapReviewCard review={baseReview} />);
    expect(screen.queryByTitle("Edit review")).not.toBeInTheDocument();
    expect(screen.queryByTitle("Delete review")).not.toBeInTheDocument();
  });

  it("shows You badge when isOwn is true", () => {
    render(<LapReviewCard review={baseReview} isOwn />);
    expect(screen.getByText("You")).toBeInTheDocument();
  });

  it("calls onEdit when edit button clicked", () => {
    const onEdit = jest.fn();
    render(<LapReviewCard review={baseReview} isOwn onEdit={onEdit} />);
    fireEvent.click(screen.getByTitle("Edit review"));
    expect(onEdit).toHaveBeenCalledWith(baseReview);
  });

  it("calls onDelete with review id when delete clicked", () => {
    const onDelete = jest.fn();
    render(<LapReviewCard review={baseReview} isOwn onDelete={onDelete} />);
    fireEvent.click(screen.getByTitle("Delete review"));
    expect(onDelete).toHaveBeenCalledWith("rev-1");
  });
});
