jest.mock("@/shared/services/storage/tokenService", () => ({
  tokenService: {
    getAccessToken: jest.fn(),
    getRefreshToken: jest.fn(),
  },
}));

import { tokenService } from "@/shared/services/storage/tokenService";
import { JWT_CLAIMS } from "@/features/auth/constants";
import {
  normalizeRole,
  getUserRoles,
  hasRole,
  isAuthenticated,
  getCurrentUser,
} from "@/features/auth/utils/authUtils";

const mockedGetAccessToken = tokenService.getAccessToken as jest.Mock;
const mockedGetRefreshToken = tokenService.getRefreshToken as jest.Mock;

const MS_NAMEIDENTIFIER = JWT_CLAIMS.MS_NAMEIDENTIFIER;
const MS_NAME = JWT_CLAIMS.MS_NAME;
const MS_EMAIL = JWT_CLAIMS.MS_EMAIL;
const MS_ROLE = JWT_CLAIMS.MS_ROLE;

function makeJwt(payload: Record<string, unknown>): string {
  const b64 = btoa(JSON.stringify(payload));
  return `header.${b64}.signature`;
}

beforeEach(() => {
  jest.clearAllMocks();
});

describe("normalizeRole", () => {
  it("trims and lowercases the role", () => {
    expect(normalizeRole("  ADMIN ")).toBe("admin");
  });

  it("strips ROLE_ prefix", () => {
    expect(normalizeRole("ROLE_Learner")).toBe("learner");
  });

  it("maps Administrator to admin", () => {
    expect(normalizeRole("Administrator")).toBe("admin");
  });

  it("handles already normalized input", () => {
    expect(normalizeRole("learner")).toBe("learner");
  });
});

describe("isAuthenticated", () => {
  it("returns true when access token exists", () => {
    mockedGetAccessToken.mockReturnValue("some-token");
    expect(isAuthenticated()).toBe(true);
  });

  it("returns true when only refresh token exists", () => {
    mockedGetAccessToken.mockReturnValue(null);
    mockedGetRefreshToken.mockReturnValue("refresh-token");
    expect(isAuthenticated()).toBe(true);
  });

  it("returns false when no tokens exist", () => {
    mockedGetAccessToken.mockReturnValue(null);
    mockedGetRefreshToken.mockReturnValue(null);
    expect(isAuthenticated()).toBe(false);
  });
});

describe("getUserRoles", () => {
  it("returns empty when no token", () => {
    mockedGetAccessToken.mockReturnValue(null);
    expect(getUserRoles()).toEqual([]);
  });

  it("parses a single role from MS claim", () => {
    mockedGetAccessToken.mockReturnValue(
      makeJwt({ [MS_ROLE]: "ROLE_Learner" }),
    );
    expect(getUserRoles()).toEqual(["learner"]);
  });

  it("parses a single role from short-form claim", () => {
    mockedGetAccessToken.mockReturnValue(makeJwt({ role: "ROLE_Learner" }));
    expect(getUserRoles()).toEqual(["learner"]);
  });

  it("parses multiple roles from array", () => {
    mockedGetAccessToken.mockReturnValue(
      makeJwt({ roles: ["ROLE_Learner", "ROLE_Instructor"] }),
    );
    expect(getUserRoles()).toEqual(["learner", "instructor"]);
  });

  it("returns empty for malformed token", () => {
    mockedGetAccessToken.mockReturnValue("invalid-token");
    expect(getUserRoles()).toEqual([]);
  });
});

describe("hasRole", () => {
  it("returns true when role exists", () => {
    mockedGetAccessToken.mockReturnValue(makeJwt({ [MS_ROLE]: "ROLE_Admin" }));
    expect(hasRole("admin")).toBe(true);
  });

  it("returns false when role does not exist", () => {
    mockedGetAccessToken.mockReturnValue(
      makeJwt({ [MS_ROLE]: "ROLE_Learner" }),
    );
    expect(hasRole("admin")).toBe(false);
  });
});

describe("getCurrentUser", () => {
  it("returns null when no token", () => {
    mockedGetAccessToken.mockReturnValue(null);
    expect(getCurrentUser()).toBeNull();
  });

  it("extracts user info from MS claims", () => {
    mockedGetAccessToken.mockReturnValue(
      makeJwt({
        [MS_NAMEIDENTIFIER]: "user-guid-123",
        [MS_NAME]: "John Doe",
        [MS_EMAIL]: "john@example.com",
        [MS_ROLE]: "ROLE_Learner",
      }),
    );
    expect(getCurrentUser()).toEqual({
      id: "user-guid-123",
      name: "John Doe",
      email: "john@example.com",
    });
  });

  it("falls back to sub when no nameidentifier exists", () => {
    mockedGetAccessToken.mockReturnValue(
      makeJwt({
        [MS_NAME]: "John",
        email: "john@example.com",
        sub: "fallback-id",
      }),
    );
    expect(getCurrentUser()?.id).toBe("fallback-id");
  });

  it("returns null for malformed token", () => {
    mockedGetAccessToken.mockReturnValue("bad-token");
    expect(getCurrentUser()).toBeNull();
  });
});
