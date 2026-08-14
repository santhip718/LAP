import { API_CONFIG } from "@/shared/constants/api";
import { APP_NAME } from "@/shared/constants/app";

export const env = {
  apiBaseUrl: import.meta.env.VITE_API_BASE_URL ?? API_CONFIG.BASE_URL,
  appName: import.meta.env.VITE_APP_NAME ?? APP_NAME,
  isDev: import.meta.env.DEV,
  isProd: import.meta.env.PROD,
};
