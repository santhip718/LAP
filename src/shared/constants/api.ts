export const API_CONFIG = {
  BASE_URL: "http://localhost:5020",
  CONTENT_TYPE: "application/json",
} as const;

export const HTTP_STATUS = {
  UNAUTHORIZED: 401,
} as const;

export const AUTH_HEADER = {
  BEARER_PREFIX: "Bearer ",
} as const;
