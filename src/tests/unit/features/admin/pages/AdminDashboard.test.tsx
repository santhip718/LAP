import "@testing-library/jest-dom";
import { render, screen, fireEvent } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";

const mockNavigate = jest.fn();
jest.mock("react-router-dom", () => ({
  ...jest.requireActual("react-router-dom"),
  useNavigate: () => mockNavigate,
}));

const mockRefresh = jest.fn();
const mockSetSearch = jest.fn();
jest.mock("@/features/user/hooks/useUserList", () => ({
  useUserList: jest.fn(),
}));

import { useUserList } from "@/features/user/hooks/useUserList";

const mockUseUserList = useUserList as jest.Mock;

const renderComponent = () =>
  render(
    <MemoryRouter>
      <AdminDashboard />
    </MemoryRouter>
  );

import AdminDashboard from "@/features/admin/pages/AdminDashboard/AdminDashboard";

describe("AdminDashboard", () => {
  beforeEach(() => {
    jest.clearAllMocks();
    mockUseUserList.mockReturnValue({
      users: [],
      total: 0,
      page: 1,
      pageSize: 20,
      loading: false,
      error: null,
      refresh: mockRefresh,
      setPage: jest.fn(),
      setPageSize: jest.fn(),
      setSearch: mockSetSearch,
      search: "",
      loadMore: jest.fn(),
      hasMore: false,
    });
  });

  it("renders the page title and subtitle", () => {
    renderComponent();
    expect(screen.getByText("Users")).toBeInTheDocument();
    expect(
      screen.getByText("Manage users, roles, and account details.")
    ).toBeInTheDocument();
  });

  it("renders Add User button", () => {
    renderComponent();
    expect(screen.getByText("Add User")).toBeInTheDocument();
  });

  it("renders loading state", () => {
    mockUseUserList.mockReturnValue({
      users: [],
      total: 0,
      page: 1,
      pageSize: 20,
      loading: true,
      error: null,
      refresh: mockRefresh,
      setPage: jest.fn(),
      setPageSize: jest.fn(),
      setSearch: mockSetSearch,
      search: "",
    });
    renderComponent();
    expect(screen.getByText("Loading users...")).toBeInTheDocument();
  });

  it("renders error state", () => {
    mockUseUserList.mockReturnValue({
      users: [],
      total: 0,
      page: 1,
      pageSize: 20,
      loading: false,
      error: "Failed to fetch users",
      refresh: mockRefresh,
      setPage: jest.fn(),
      setPageSize: jest.fn(),
      setSearch: mockSetSearch,
      search: "",
    });
    renderComponent();
    expect(screen.getByText("Failed to fetch users")).toBeInTheDocument();
  });

  it("renders users in the table", () => {
    mockUseUserList.mockReturnValue({
      users: [
        { id: "1", fullName: "Alice", email: "alice@test.com", roles: ["Admin"] },
        { id: "2", fullName: "Bob", email: "bob@test.com", roles: ["Student"] },
      ],
      total: 2,
      page: 1,
      pageSize: 20,
      loading: false,
      error: null,
      refresh: mockRefresh,
      setPage: jest.fn(),
      setPageSize: jest.fn(),
      setSearch: mockSetSearch,
      search: "",
    });
    renderComponent();
    expect(screen.getByText("Alice")).toBeInTheDocument();
    expect(screen.getByText("Bob")).toBeInTheDocument();
    expect(screen.getByText("alice@test.com")).toBeInTheDocument();
    expect(screen.getByText("bob@test.com")).toBeInTheDocument();
  });

  it("opens create user modal on Add User click", () => {
    renderComponent();
    fireEvent.click(screen.getByText("Add User"));
    expect(screen.getByText("Create User")).toBeInTheDocument();
  });

  it("calls refresh on refresh button click", () => {
    renderComponent();
    const refreshBtn = screen.getByText("refresh").closest("button");
    expect(refreshBtn).toBeInTheDocument();
    fireEvent.click(refreshBtn!);
    expect(mockRefresh).toHaveBeenCalledTimes(1);
  });

  it("updates search input and filters results", () => {
    mockUseUserList.mockReturnValue({
      users: [
        { id: "1", fullName: "Alice", email: "alice@test.com", roles: ["Admin"] },
        { id: "2", fullName: "Bob", email: "bob@test.com", roles: ["Student"] },
      ],
      total: 2,
      page: 1,
      pageSize: 20,
      loading: false,
      error: null,
      refresh: mockRefresh,
    });
    renderComponent();
    expect(screen.getByText("Alice")).toBeInTheDocument();
    expect(screen.getByText("Bob")).toBeInTheDocument();
    const searchInput = screen.getByPlaceholderText("Search by name or email");
    fireEvent.change(searchInput, { target: { value: "Alice" } });
    expect(searchInput).toHaveValue("Alice");
    expect(screen.getByText("Alice")).toBeInTheDocument();
    expect(screen.queryByText("Bob")).not.toBeInTheDocument();
  });
});
