import { env } from './env';

export const appConfig = {
  api: {
    baseURL: env.apiBaseUrl,
  },
} as const;
