import axios from "axios";
import { tokenService } from "@/shared/services/storage";
import { env } from "@/core/config/env";
import { getAuth } from "@/shared/services/api/services/auth/auth";

// Create a standalone instance for refresh to avoid circular dependency
const refreshInstance = axios.create({
  baseURL: env.apiBaseUrl,
  headers: { "Content-Type": "application/json" },
});

const authApi = getAuth(refreshInstance);

export const authRefreshService = {
  async refresh() {
    const refreshToken = tokenService.getRefreshToken();
    if (!refreshToken) throw new Error("No refresh token");

    const response = await authApi.postApiV1AuthRefresh({
      refresh_token: refreshToken,
    });

    const data = response.data as Record<string, string> | undefined;
    const accessToken = data?.accessToken ?? data?.access_token;
    const newRefresh = data?.refreshToken ?? data?.refresh_token;

    if (accessToken) {
      tokenService.setTokens(accessToken, newRefresh || refreshToken);
    }
    return data;
  },
};
