import axios, { type AxiosRequestConfig } from "axios";
import { tokenService } from "../storage/tokenService";
import { feedbackService } from "../feedback";
import { authRefreshService } from "@/features/auth/services/authRefreshService";
import { env } from "@/core/config/env";

const apiClient = axios.create({
  baseURL: env.apiBaseUrl,
  headers: { "Content-Type": "application/json" },
});

apiClient.interceptors.request.use((config) => {
  const token = tokenService.getAccessToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  if (config.data instanceof FormData) {
    delete config.headers['Content-Type'];
  }
  return config;
});

apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config as AxiosRequestConfig & {
      _retry?: boolean;
    };

    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;
      try {
        await authRefreshService.refresh();
        const accessToken = tokenService.getAccessToken();
        if (accessToken && originalRequest.headers) {
          originalRequest.headers.Authorization = `Bearer ${accessToken}`;
        }
        return apiClient(originalRequest);
      } catch (refreshError) {
        tokenService.clearTokens();
        if (globalThis.location.pathname !== "/login") {
          globalThis.location.href = "/login";
        }
        return Promise.reject(refreshError);
      }
    }

    // Global error handler for 500 errors
    if (error.response?.status && error.response.status >= 500) {
      feedbackService.showToast(
        "Something went wrong. Please try again later.",
        "error",
      );
    }

    return Promise.reject(error);
  }
);

export default apiClient;
