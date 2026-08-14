import { tokenService } from "@/shared/services/storage/tokenService";
import { STORAGE_KEYS } from "@/shared/constants/storage";

interface JwtPayload {
  sub?: string;
  email?: string;
  role?: string | string[];
  roles?: string | string[];
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"?:
    | string
    | string[];
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"?: string;
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"?: string;
  [key: string]: unknown;
}

function decodeToken(token: string): JwtPayload | null {
  try {
    const parts = token.split(".");
    if (parts.length !== 3) return null;
    const payload = parts[1];
    const binary = atob(payload.replace(/-/g, "+").replace(/_/g, "/"));
    const bytes = new Uint8Array(binary.length);
    for (let i = 0; i < binary.length; i++) {
      bytes[i] = binary.charCodeAt(i);
    }
    const decoded = new TextDecoder().decode(bytes);
    return JSON.parse(decoded) as JwtPayload;
  } catch {
    return null;
  }
}

export function normalizeRole(role: string): string {
  return role
    .trim()
    .toLowerCase()
    .replace(/^role[_-]?/, "")
    .replace(/^administrator$/, "admin");
}

export function getUserRoles(): string[] {
  const token = tokenService.getAccessToken();
  if (!token) return [];
  const payload = decodeToken(token);
  if (!payload) return [];

  const raw =
    payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ??
    payload.role ??
    payload.roles ??
    payload.authorities ??
    payload.Role ??
    payload.Roles ??
    payload.user_role ??
    payload["urn:zitadel:iam:org:project:roles"];

  if (typeof raw === "string") return [normalizeRole(raw)];

  if (Array.isArray(raw)) {
    return raw
      .map((r: unknown) => {
        if (typeof r === "string") return normalizeRole(r);
        if (r && typeof r === "object") {
          const obj = r as Record<string, unknown>;
          const role = obj.authority ?? obj.role ?? obj.name;
          return typeof role === "string" ? normalizeRole(role) : "";
        }
        return "";
      })
      .filter(Boolean);
  }

  if (raw && typeof raw === "object") {
    const obj = raw as Record<string, unknown>;
    const keys = Object.keys(obj);
    if (keys.length > 0) {
      return keys
        .filter((key) => {
          const val = obj[key];
          return val === true || (typeof val === "string" && val.toLowerCase() === "true");
        })
        .map(normalizeRole);
    }
  }

  return [];
}

export function hasRole(role: string): boolean {
  return getUserRoles().includes(normalizeRole(role));
}

export function getCurrentUserId(): string | null {
  const storedUserId = localStorage.getItem(STORAGE_KEYS.USER_ID);
  if (storedUserId) return storedUserId;

  const payload = getDecodedPayload();
  if (!payload) return null;

  const raw =
    payload.sub ??
    payload.user_id ??
    payload.userId ??
    payload.uid ??
    payload.nameid ??
    payload.id ??
    (
      payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"]
    );

  if (typeof raw !== "string") return null;

  const atIndex = raw.indexOf("@");
  return atIndex > 0 ? raw.slice(0, atIndex) : raw;
}

export function getCurrentUserEmail(): string | null {
  const payload = getDecodedPayload();
  if (!payload) return null;
  return (
    payload.email ??
    payload.preferred_username ??
    payload.unique_name ??
    payload.upn ??
    (payload.emailAddress as string) ??
    payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"] ??
    null
  ) as string | null;
}

export function getCurrentUserFullName(): string | null {
  const payload = getDecodedPayload();
  if (!payload) return null;
  return (
    (payload.name as string) ??
    (payload.fullName as string) ??
    (payload.full_name as string) ??
    (payload.given_name as string) ??
    (payload.family_name as string) ??
    (payload.preferred_username as string) ??
    payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] ??
    null
  );
}

export function getCurrentUserPhone(): string | null {
  const payload = getDecodedPayload();
  if (!payload) return null;
  return (
    (payload.phone_number as string) ??
    (payload.phoneNumber as string) ??
    (payload.mobileNumber as string) ??
    (payload.mobile_number as string) ??
    payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/mobilephone"] ??
    null
  );
}

export function getCurrentUserProfileImage(): string | null {
  const payload = getDecodedPayload();
  if (!payload) return null;
  return (
    (payload.picture as string) ??
    (payload.profileImage as string) ??
    (payload.profile_image as string) ??
    null
  );
}

function getDecodedPayload(): JwtPayload | null {
  const token = tokenService.getAccessToken();
  if (!token) return null;
  return decodeToken(token);
}

export function isAuthenticated(): boolean {
  return !!tokenService.getAccessToken() || !!tokenService.getRefreshToken();
}
