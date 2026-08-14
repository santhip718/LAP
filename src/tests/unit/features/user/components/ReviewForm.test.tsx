import "@testing-library/jest-dom";
import { render, screen, fireEvent } from "@testing-library/react";
import ReviewForm from "@/features/user/components/ReviewForm/ReviewForm";

const mockRegister = jest.fn().mockReturnValue({});
const mockHandleSubmit = jest.fn(
  (cb: (data: { reviewText: string }) => void) =>
    (e?: { preventDefault: () => void }) => {
      e?.preventDefault();
      cb({ reviewText: "Great course!" });
    },
);

jest.mock("react-hook-form", () => ({
  useForm: () => ({
    register: mockRegister,
    handleSubmit: mockHandleSubmit,
    reset: jest.fn(),
    formState: { isSubmitting: false, errors: {} },
  }),
}));

describe("ReviewForm", () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  it("renders new review form title", () => {
    render(<ReviewForm onSubmit={jest.fn()} />);
    expect(
      screen.getByText("How would you describe your experience with this program?"),
    ).toBeInTheDocument();
  });

  it("renders edit review form title", () => {
    render(
      <ReviewForm
        onSubmit={jest.fn()}
        initialValues={{ rating: 4, reviewText: "Good" }}
      />,
    );
    expect(screen.getByText("Edit your review")).toBeInTheDocument();
  });

  it("renders Submit Feedback button for new review", () => {
    render(<ReviewForm onSubmit={jest.fn()} />);
    expect(screen.getByText("Submit Feedback")).toBeInTheDocument();
  });

  it("renders Save Changes button for edit review", () => {
    render(
      <ReviewForm
        onSubmit={jest.fn()}
        initialValues={{ rating: 4, reviewText: "Good" }}
      />,
    );
    expect(screen.getByText("Save Changes")).toBeInTheDocument();
  });

  it("renders Cancel button for edit review", () => {
    const onCancel = jest.fn();
    render(
      <ReviewForm
        onSubmit={jest.fn()}
        initialValues={{ rating: 4, reviewText: "Good" }}
        onCancel={onCancel}
      />,
    );
    expect(screen.getByText("Cancel")).toBeInTheDocument();
  });

  it("renders 5 star buttons", () => {
    render(<ReviewForm onSubmit={jest.fn()} />);
    const starButtons = screen.getAllByRole("button");
    expect(starButtons.length).toBeGreaterThanOrEqual(5);
  });

  it("calls onSubmit with rating and review text", async () => {
    const onSubmit = jest.fn();
    render(<ReviewForm onSubmit={onSubmit} />);

    const starBtn = screen.getAllByRole("button")[4];
    fireEvent.click(starBtn);

    const submitBtn = screen.getAllByRole("button")[5];
    fireEvent.click(submitBtn);

    expect(onSubmit).toHaveBeenCalledWith({
      rating: 5,
      reviewText: "Great course!",
    });
  });

  it("shows validation error if no rating selected on submit", () => {
    render(<ReviewForm onSubmit={jest.fn()} />);
    const submitBtn = screen.getAllByRole("button")[5];
    fireEvent.click(submitBtn);
    expect(screen.getByText("Please select a rating")).toBeInTheDocument();
  });
});
