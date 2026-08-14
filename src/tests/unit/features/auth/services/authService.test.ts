import { authService } from "@/features/auth/services/authService";
import { getAuth } from "@/shared/services/api/services/auth/auth";
import { tokenService } from "@/shared/services/storage/tokenService";

jest.mock("@/shared/services/api/services/auth/auth", () => {
  const api = {
    postApiV1AuthLogin: jest.fn(),
    postApiV1AuthRegister: jest.fn(),
    postApiV1AuthRefresh: jest.fn(),
    postApiV1AuthLogout: jest.fn(),
  };
  return { getAuth: jest.fn(() => api) };
});

jest.mock("@/shared/services/storage/tokenService", () => ({
  tokenService: {
    getAccessToken: jest.fn(),
    getRefreshToken: jest.fn(),
    setTokens: jest.fn(),
    clearTokens: jest.fn(),
  },
}));

const mockApi = (getAuth as jest.Mock)() as {
  postApiV1AuthLogin: jest.Mock;
  postApiV1AuthRegister: jest.Mock;
  postApiV1AuthRefresh: jest.Mock;
  postApiV1AuthLogout: jest.Mock;
};

beforeEach(() => {
  jest.clearAllMocks();
});

describe("authService.login", () => {
  it("calls login API with email and password", async () => {
    mockApi.postApiV1AuthLogin.mockResolvedValue({ data: { accessToken: "at", refreshToken: "rt" } });
    const result = await authService.login({ email: "a@b.com", password: "secret" });
    expect(mockApi.postApiV1AuthLogin).toHaveBeenCalledWith({ email: "a@b.com", password: "secret" });
    expect(tokenService.setTokens).toHaveBeenCalledWith("at", "rt");
    expect(result).toEqual({ accessToken: "at", refreshToken: "rt" });
  });

  it("handles snake_case response keys", async () => {
    mockApi.postApiV1AuthLogin.mockResolvedValue({ data: { access_token: "at", refresh_token: "rt" } });
    await authService.login({ email: "a@b.com", password: "s" });
    expect(tokenService.setTokens).toHaveBeenCalledWith("at", "rt");
  });

  it("does not call setTokens when no access token in response", async () => {
    mockApi.postApiV1AuthLogin.mockResolvedValue({ data: {} });
    await authService.login({ email: "a@b.com", password: "s" });
    expect(tokenService.setTokens).not.toHaveBeenCalled();
  });

  it("re-throws API errors", async () => {
    mockApi.postApiV1AuthLogin.mockRejectedValue(new Error("Unauthorized"));
    await expect(authService.login({ email: "a@b.com", password: "bad" })).rejects.toThrow("Unauthorized");
  });
});

describe("authService.register", () => {
  const payload = {
    fullName: "John Doe",
    email: "j@d.com",
    password: "secret123",
    mobileNumber: "1234567890",
    designationId: "des-1",
    genderId: "gen-1",
  };

  it("calls register API with mapped payload", async () => {
    mockApi.postApiV1AuthRegister.mockResolvedValue({ data: undefined });
    await authService.register(payload);
    expect(mockApi.postApiV1AuthRegister).toHaveBeenCalledWith({
      full_name: "John Doe",
      email: "j@d.com",
      password: "secret123",
      mobile_number: "1234567890",
      designation_id: "des-1",
      gender_id: "gen-1",
    });
  });

  it("re-throws API errors", async () => {
    mockApi.postApiV1AuthRegister.mockRejectedValue(new Error("Validation failed"));
    await expect(authService.register(payload)).rejects.toThrow("Validation failed");
  });
});

describe("authService.refresh", () => {
  it("throws when no refresh token", async () => {
    (tokenService.getRefreshToken as jest.Mock).mockReturnValue(null);
    await expect(authService.refresh()).rejects.toThrow("No refresh token");
  });

  it("calls refresh API and stores new tokens", async () => {
    (tokenService.getRefreshToken as jest.Mock).mockReturnValue("old-rt");
    mockApi.postApiV1AuthRefresh.mockResolvedValue({ data: { accessToken: "new-at", refreshToken: "new-rt" } });
    const result = await authService.refresh();
    expect(mockApi.postApiV1AuthRefresh).toHaveBeenCalledWith({ refresh_token: "old-rt" });
    expect(tokenService.setTokens).toHaveBeenCalledWith("new-at", "new-rt");
    expect(result).toEqual({ accessToken: "new-at", refreshToken: "new-rt" });
  });

  it("falls back to old refresh token when no new one returned", async () => {
    (tokenService.getRefreshToken as jest.Mock).mockReturnValue("old-rt");
    mockApi.postApiV1AuthRefresh.mockResolvedValue({ data: { accessToken: "new-at" } });
    await authService.refresh();
    expect(tokenService.setTokens).toHaveBeenCalledWith("new-at", "old-rt");
  });
});

describe("authService.logout", () => {
  it("calls logout API and clears tokens", async () => {
    (tokenService.getRefreshToken as jest.Mock).mockReturnValue("rt");
    mockApi.postApiV1AuthLogout.mockResolvedValue({ data: undefined });
    await authService.logout();
    expect(mockApi.postApiV1AuthLogout).toHaveBeenCalledWith({ refresh_token: "rt" });
    expect(tokenService.clearTokens).toHaveBeenCalled();
  });

  it("clears tokens even when logout API fails", async () => {
    (tokenService.getRefreshToken as jest.Mock).mockReturnValue("rt");
    mockApi.postApiV1AuthLogout.mockRejectedValue(new Error("Network error"));
    await expect(authService.logout()).rejects.toThrow("Network error");
    expect(tokenService.clearTokens).toHaveBeenCalled();
  });

  it("skips logout API when no refresh token", async () => {
    (tokenService.getRefreshToken as jest.Mock).mockReturnValue(null);
    await authService.logout();
    expect(mockApi.postApiV1AuthLogout).not.toHaveBeenCalled();
    expect(tokenService.clearTokens).toHaveBeenCalled();
  });
});
