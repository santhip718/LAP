import { tokenService } from "@/shared/services/storage/tokenService";
import { JWT_CLAIMS, ROLE_NORMALIZE } from "@/features/auth/constants";
import type { JwtPayload } from "@/features/auth/types";

function decodeToken(token: string): JwtPayload | null {
  try {
    const parts = token.split(".");
    if (parts.length !== 3) return null;
    const payload = parts[1];
    const decoded = atob(payload.replace(/-/g, "+").replace(/_/g, "/"));
    return JSON.parse(decoded) as JwtPayload;
  } catch {
    return null;
  }
}

export function normalizeRole(role: string): string {
  return role
    .trim()
    .toLowerCase()
    .replace(ROLE_NORMALIZE.ROLE_PREFIX, ROLE_NORMALIZE.EMPTY)
    .replace(ROLE_NORMALIZE.ADMINISTRATOR, ROLE_NORMALIZE.ADMIN);
}

export function getUserRoles(): string[] {
  const token = tokenService.getAccessToken();
  if (!token) return [];
  const payload = decodeToken(token);
  if (!payload) return [];

  const raw =
    payload[JWT_CLAIMS.MS_ROLE] ??
    payload[JWT_CLAIMS.ROLE] ??
    payload[JWT_CLAIMS.ROLES] ??
    payload[JWT_CLAIMS.AUTHORITIES];

  if (typeof raw === "string") return [normalizeRole(raw)];

  if (Array.isArray(raw)) {
    return raw
      .map((r: unknown) => {
        if (typeof r === "string") return normalizeRole(r);
        if (r && typeof r === "object") {
          const obj = r as Record<string, unknown>;
          const role =
            obj[JWT_CLAIMS.AUTHORITY] ??
            obj[JWT_CLAIMS.ROLE] ??
            obj[JWT_CLAIMS.NAME];
          return typeof role === "string"
            ? normalizeRole(role)
            : ROLE_NORMALIZE.EMPTY;
        }
        return "";
      })
      .filter(Boolean);
  }
  return [];
}

export function hasRole(role: string): boolean {
  return getUserRoles().includes(normalizeRole(role));
}

export function isAuthenticated(): boolean {
  return !!tokenService.getAccessToken() || !!tokenService.getRefreshToken();
}

export function getCurrentUser(): { id: string; name: string; email: string } | null {
  const token = tokenService.getAccessToken();
  if (!token) return null;
  const payload = decodeToken(token);
  if (!payload) return null;
  const name =
    (payload[JWT_CLAIMS.MS_NAME] as string) ??
    (payload[JWT_CLAIMS.NAME] as string) ??
    (payload.email as string) ??
    payload.sub ??
    "";
  const email =
    (payload[JWT_CLAIMS.MS_EMAIL] as string) ??
    (payload.email as string) ??
    "";
  const id =
    (payload[JWT_CLAIMS.MS_NAMEIDENTIFIER] as string) ??
    payload.sub ??
    email;
  return {
    id,
    name,
    email,
  };
}
