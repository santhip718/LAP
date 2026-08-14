export const JWT_CLAIMS = {
  MS_ROLE: "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
  MS_EMAIL: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
  MS_NAME: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
  MS_NAMEIDENTIFIER: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
  ROLE: "role",
  ROLES: "roles",
  AUTHORITIES: "authorities",
  AUTHORITY: "authority",
  NAME: "name",
} as const;

export const ROLE_NORMALIZE = {
  ROLE_PREFIX: /^role[_-]?/,
  ADMINISTRATOR: /^administrator$/,
  ADMIN: "admin",
  EMPTY: "",
} as const;
