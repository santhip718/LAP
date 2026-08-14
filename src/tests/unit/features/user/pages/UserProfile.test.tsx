import "@testing-library/jest-dom";
import { render, screen, fireEvent } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import UserProfile from "@/features/user/pages/UserProfile/UserProfile";

jest.mock("@/features/user/hooks/useUserProfile", () => ({
  useUserProfile: jest.fn(),
}));

import { useUserProfile } from "@/features/user/hooks/useUserProfile";
const mockUseUserProfile = useUserProfile as jest.Mock;

const mockRefresh = jest.fn();
const mockUploadImage = jest.fn().mockResolvedValue("https://example.com/avatar.jpg");

const mockProfile = {
  id: "user-1",
  fullName: "John Doe",
  email: "john@example.com",
  mobileNumber: "+1234567890",
  designation: "Software Engineer",
  designationId: "des-1",
  gender: "Male",
  genderId: "gen-1",
  currentTier: "Gold",
  roles: ["Admin", "Instructor"],
  dateCreated: "2024-01-10T00:00:00Z",
  profileImage: null,
};

const defaultMock = {
  profile: mockProfile,
  loading: false,
  error: null,
  refresh: mockRefresh,
  uploadImage: mockUploadImage,
  uploading: false,
};

const renderComponent = () =>
  render(
    <MemoryRouter>
      <UserProfile />
    </MemoryRouter>
  );

describe("UserProfile", () => {
  beforeEach(() => {
    jest.clearAllMocks();
    mockUseUserProfile.mockReturnValue(defaultMock);
  });

  it("renders without crashing", () => {
    const { container } = renderComponent();
    expect(container.firstChild).toBeInTheDocument();
  });

  it("renders loading spinner", () => {
    mockUseUserProfile.mockReturnValue({ ...defaultMock, loading: true });
    const { container } = renderComponent();
    expect(container.querySelector(".lap-spinner-overlay")).toBeInTheDocument();
  });

  it("renders error state", () => {
    mockUseUserProfile.mockReturnValue({
      ...defaultMock,
      error: "Failed to load profile",
      profile: null,
    });
    renderComponent();
    expect(screen.getByText("Failed to load profile")).toBeInTheDocument();
  });

  it("renders page title", () => {
    renderComponent();
    expect(screen.getByText("My Profile")).toBeInTheDocument();
  });

  it("renders personal information section", () => {
    renderComponent();
    expect(screen.getByText("Personal Information")).toBeInTheDocument();
  });

  it("renders user full name and email", () => {
    renderComponent();
    expect(screen.getByText("John Doe")).toBeInTheDocument();
    expect(screen.getByText("john@example.com")).toBeInTheDocument();
  });

  it("renders mobile number", () => {
    renderComponent();
    expect(screen.getByText("+1234567890")).toBeInTheDocument();
  });

  it("renders work details section", () => {
    renderComponent();
    expect(screen.getByText("Work Details")).toBeInTheDocument();
  });

  it("renders designation and gender", () => {
    renderComponent();
    expect(screen.getByText("Software Engineer")).toBeInTheDocument();
    expect(screen.getByText("Male")).toBeInTheDocument();
  });

  it("renders current tier", () => {
    renderComponent();
    expect(screen.getByText("Gold")).toBeInTheDocument();
  });

  it("renders role badges", () => {
    renderComponent();
    expect(screen.getByText("Admin")).toBeInTheDocument();
    expect(screen.getByText("Instructor")).toBeInTheDocument();
  });

  it("renders retry button on error", () => {
    mockUseUserProfile.mockReturnValue({
      ...defaultMock,
      error: "API error",
      profile: null,
    });
    renderComponent();
    const retryBtn = screen.getByText("Retry");
    expect(retryBtn).toBeInTheDocument();
    fireEvent.click(retryBtn);
    expect(mockRefresh).toHaveBeenCalledTimes(1);
  });

  it("renders profile image upload section", () => {
    renderComponent();
    expect(screen.getByText("Personal Information")).toBeInTheDocument();
  });
});
