import { normalizeRole, getUserRoles, hasRole, isAuthenticated } from "@/features/auth/utils/authUtils";
import { JWT_CLAIMS, ROLE_NORMALIZE } from "@/features/auth/constants";
import { tokenService } from "@/shared/services/storage/tokenService";

jest.mock("@/shared/services/storage/tokenService", () => ({
  tokenService: {
    getAccessToken: jest.fn(),
    getRefreshToken: jest.fn(),
  },
}));

beforeEach(() => {
  jest.clearAllMocks();
});

describe("normalizeRole", () => {
  it("strips ROLE_ prefix", () => {
    expect(normalizeRole("ROLE_ADMIN")).toBe("admin");
    expect(normalizeRole("role_admin")).toBe("admin");
  });

  it("strips role- prefix", () => {
    expect(normalizeRole("role-admin")).toBe("admin");
  });

  it("strips role_ prefix", () => {
    expect(normalizeRole("role_admin")).toBe("admin");
  });

  it("converts Administrator to admin", () => {
    expect(normalizeRole("Administrator")).toBe("admin");
    expect(normalizeRole("ROLE_ADMINISTRATOR")).toBe("admin");
  });

  it("trims whitespace and lowercases", () => {
    expect(normalizeRole("  USER ")).toBe("user");
  });

  it("passes through already normalized roles", () => {
    expect(normalizeRole("user")).toBe("user");
    expect(normalizeRole("student")).toBe("student");
  });
});

describe("getUserRoles", () => {
  it("returns empty array when no token", () => {
    (tokenService.getAccessToken as jest.Mock).mockReturnValue(null);
    expect(getUserRoles()).toEqual([]);
  });

  it("returns empty array for malformed token", () => {
    (tokenService.getAccessToken as jest.Mock).mockReturnValue("invalid-token");
    expect(getUserRoles()).toEqual([]);
  });

  it("extracts role from string MS claim (MS_ROLE)", () => {
    const payload = {
      [JWT_CLAIMS.MS_ROLE]: "ROLE_ADMIN",
    };
    const token = btoa(JSON.stringify(payload));
    (tokenService.getAccessToken as jest.Mock).mockReturnValue(`header.${token}.signature`);
    expect(getUserRoles()).toEqual(["admin"]);
  });

  it("extracts role from string ROLE claim", () => {
    const payload = { [JWT_CLAIMS.ROLE]: "ROLE_USER" };
    const token = btoa(JSON.stringify(payload));
    (tokenService.getAccessToken as jest.Mock).mockReturnValue(`x.${token}.z`);
    expect(getUserRoles()).toEqual(["user"]);
  });

  it("extracts roles from array ROLE claim", () => {
    const payload = { [JWT_CLAIMS.ROLES]: ["ROLE_ADMIN", "ROLE_USER"] };
    const token = btoa(JSON.stringify(payload));
    (tokenService.getAccessToken as jest.Mock).mockReturnValue(`x.${token}.z`);
    expect(getUserRoles()).toEqual(["admin", "user"]);
  });

  it("extracts roles from authorities array with object entries", () => {
    const payload = {
      [JWT_CLAIMS.AUTHORITIES]: [
        { [JWT_CLAIMS.AUTHORITY]: "ROLE_ADMIN" },
        { [JWT_CLAIMS.ROLE]: "ROLE_USER" },
      ],
    };
    const token = btoa(JSON.stringify(payload));
    (tokenService.getAccessToken as jest.Mock).mockReturnValue(`x.${token}.z`);
    expect(getUserRoles()).toEqual(["admin", "user"]);
  });

  it("uses fallback claim order", () => {
    const payload = { [JWT_CLAIMS.AUTHORITIES]: "ROLE_VIEWER" };
    const token = btoa(JSON.stringify(payload));
    (tokenService.getAccessToken as jest.Mock).mockReturnValue(`x.${token}.z`);
    expect(getUserRoles()).toEqual(["viewer"]);
  });
});

describe("hasRole", () => {
  it("returns true when user has the role", () => {
    const payload = { [JWT_CLAIMS.ROLE]: "ROLE_ADMIN" };
    const token = btoa(JSON.stringify(payload));
    (tokenService.getAccessToken as jest.Mock).mockReturnValue(`x.${token}.z`);
    expect(hasRole("admin")).toBe(true);
  });

  it("returns false when user does not have the role", () => {
    const payload = { [JWT_CLAIMS.ROLE]: "ROLE_USER" };
    const token = btoa(JSON.stringify(payload));
    (tokenService.getAccessToken as jest.Mock).mockReturnValue(`x.${token}.z`);
    expect(hasRole("admin")).toBe(false);
  });

  it("normalizes the input role for comparison", () => {
    const payload = { [JWT_CLAIMS.ROLE]: "ROLE_ADMIN" };
    const token = btoa(JSON.stringify(payload));
    (tokenService.getAccessToken as jest.Mock).mockReturnValue(`x.${token}.z`);
    expect(hasRole("ROLE_ADMIN")).toBe(true);
  });

  it("returns false when no token", () => {
    (tokenService.getAccessToken as jest.Mock).mockReturnValue(null);
    expect(hasRole("admin")).toBe(false);
  });
});

describe("isAuthenticated", () => {
  it("returns true when access token exists", () => {
    (tokenService.getAccessToken as jest.Mock).mockReturnValue("some-token");
    (tokenService.getRefreshToken as jest.Mock).mockReturnValue(null);
    expect(isAuthenticated()).toBe(true);
  });

  it("returns true when refresh token exists", () => {
    (tokenService.getAccessToken as jest.Mock).mockReturnValue(null);
    (tokenService.getRefreshToken as jest.Mock).mockReturnValue("refresh-token");
    expect(isAuthenticated()).toBe(true);
  });

  it("returns false when no tokens", () => {
    (tokenService.getAccessToken as jest.Mock).mockReturnValue(null);
    (tokenService.getRefreshToken as jest.Mock).mockReturnValue(null);
    expect(isAuthenticated()).toBe(false);
  });
});
