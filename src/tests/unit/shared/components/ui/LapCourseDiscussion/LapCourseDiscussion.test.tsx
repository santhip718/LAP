import "@testing-library/jest-dom";
import { render, screen, fireEvent } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";

beforeEach(() => {
  Element.prototype.scrollIntoView = jest.fn();
});

jest.mock("@/shared/hooks/useForumMessages", () => ({
  useForumMessages: jest.fn(),
}));

import { useForumMessages } from "@/shared/hooks/useForumMessages";
const mockUseForumMessages = useForumMessages as jest.Mock;

const mockSendMessage = jest.fn().mockResolvedValue(undefined);
const mockRefresh = jest.fn();

const defaultMock = {
  messages: [],
  loading: false,
  error: null,
  refresh: mockRefresh,
  sendMessage: mockSendMessage,
  sending: false,
};

import LapCourseDiscussion from "@/shared/components/ui/LapCourseDiscussion/LapCourseDiscussion";

const renderComponent = () =>
  render(
    <MemoryRouter>
      <LapCourseDiscussion courseId="course-1" />
    </MemoryRouter>
  );

describe("LapCourseDiscussion", () => {
  beforeEach(() => {
    jest.clearAllMocks();
    mockUseForumMessages.mockReturnValue(defaultMock);
  });

  it("renders loading state", () => {
    mockUseForumMessages.mockReturnValue({ ...defaultMock, loading: true });
    renderComponent();
    expect(screen.getByText("Loading discussion...")).toBeInTheDocument();
  });

  it("renders error state with retry button", () => {
    mockUseForumMessages.mockReturnValue({
      ...defaultMock,
      error: "Failed to load messages",
    });
    renderComponent();
    expect(screen.getByText("Failed to load messages")).toBeInTheDocument();
    expect(screen.getByText("Retry")).toBeInTheDocument();
  });

  it("renders empty state", () => {
    renderComponent();
    expect(screen.getByText("No messages yet")).toBeInTheDocument();
    expect(
      screen.getByText("Start the discussion by sending the first message.")
    ).toBeInTheDocument();
  });

  it("renders messages list", () => {
    mockUseForumMessages.mockReturnValue({
      ...defaultMock,
      messages: [
        {
          id: "msg-1",
          courseId: "course-1",
          userId: "user-1",
          userFullName: "Alice",
          messageText: "Hello everyone!",
          dateCreated: new Date().toISOString(),
        },
        {
          id: "msg-2",
          courseId: "course-1",
          userId: "user-2",
          userFullName: "Bob",
          messageText: "Hi Alice!",
          dateCreated: new Date(Date.now() - 3600000).toISOString(),
        },
      ],
    });
    renderComponent();
    expect(screen.getByText("Hello everyone!")).toBeInTheDocument();
    expect(screen.getByText("Hi Alice!")).toBeInTheDocument();
    expect(screen.getByText("Alice")).toBeInTheDocument();
    expect(screen.getByText("Bob")).toBeInTheDocument();
  });

  it("renders textarea and send button", () => {
    renderComponent();
    expect(
      screen.getByPlaceholderText("Type your message...")
    ).toBeInTheDocument();
    expect(screen.getByLabelText("Send message")).toBeInTheDocument();
  });

  it("disables send button when textarea is empty", () => {
    renderComponent();
    const sendBtn = screen.getByLabelText("Send message");
    expect(sendBtn).toBeDisabled();
  });

  it("enables send button when textarea has text", () => {
    renderComponent();
    const textarea = screen.getByPlaceholderText("Type your message...");
    fireEvent.change(textarea, { target: { value: "Hello" } });
    const sendBtn = screen.getByLabelText("Send message");
    expect(sendBtn).not.toBeDisabled();
  });

  it("calls sendMessage when send button is clicked", () => {
    renderComponent();
    const textarea = screen.getByPlaceholderText("Type your message...");
    fireEvent.change(textarea, { target: { value: "Hello" } });
    fireEvent.click(screen.getByLabelText("Send message"));
    expect(mockSendMessage).toHaveBeenCalledWith("Hello");
  });

  it("calls sendMessage on Enter key press", () => {
    renderComponent();
    const textarea = screen.getByPlaceholderText("Type your message...");
    fireEvent.change(textarea, { target: { value: "Enter message" } });
    fireEvent.keyDown(textarea, { key: "Enter" });
    expect(mockSendMessage).toHaveBeenCalledWith("Enter message");
  });

  it("does not call sendMessage on Shift+Enter", () => {
    renderComponent();
    const textarea = screen.getByPlaceholderText("Type your message...");
    fireEvent.change(textarea, { target: { value: "Shift enter" } });
    fireEvent.keyDown(textarea, { key: "Enter", shiftKey: true });
    expect(mockSendMessage).not.toHaveBeenCalled();
  });

  it("disables inputs while sending", () => {
    mockUseForumMessages.mockReturnValue({
      ...defaultMock,
      sending: true,
    });
    renderComponent();
    const textarea = screen.getByPlaceholderText("Type your message...");
    expect(textarea).toBeDisabled();
    const sendBtn = screen.getByLabelText("Send message");
    expect(sendBtn).toBeDisabled();
  });

  it("renders hint text", () => {
    renderComponent();
    expect(
      screen.getByText("Press Enter to send, Shift+Enter for new line")
    ).toBeInTheDocument();
  });
});
