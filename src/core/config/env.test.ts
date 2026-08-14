import { API_CONFIG } from "@/shared/constants/api";
import { APP_NAME } from "@/shared/constants/app";

export const env = {
  apiBaseUrl: API_CONFIG.BASE_URL,
  appName: APP_NAME,
  isDev: false,
  isProd: false,
};
