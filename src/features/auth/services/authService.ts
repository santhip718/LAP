import apiClient from "@/shared/services/api/apiClient";
import { getAuth } from "@/shared/services/api/services/auth/auth";
import { authRefreshService } from "./authRefreshService";

import { tokenService } from "@/shared/services/storage";
import { STORAGE_KEYS } from "@/shared/constants/storage";
import type { LoginPayload, RegisterPayload } from "@/features/auth/types";

const authApi = getAuth(apiClient);

function mapRegisterPayload(payload: RegisterPayload) {
  return {
    full_name: payload.fullName,
    email: payload.email,
    password: payload.password,
    mobile_number: payload.mobileNumber,
    designation_id: payload.designationId,
    gender_id: payload.genderId,
  };
}

function extractTokens(data: unknown): {
  accessToken: string;
  refreshToken: string;
} {
  const res = data as Record<string, string> | undefined;
  const accessToken = res?.accessToken ?? res?.access_token ?? "";
  const refreshToken = res?.refreshToken ?? res?.refresh_token ?? "";
  return { accessToken, refreshToken };
}

export const authService = {
  async login(payload: LoginPayload) {
    const { data } = await authApi.postApiV1AuthLogin({
      email: payload.email,
      password: payload.password,
    });
    const res = data as Record<string, string> | undefined;
    const accessToken = res?.accessToken ?? res?.access_token;
    const refreshToken = res?.refreshToken ?? res?.refresh_token;
    if (accessToken) {
      tokenService.setTokens(accessToken, refreshToken || "");
    }
    const userId = res?.userId ?? res?.user_id ?? res?.id ?? "";
    if (userId) {
      localStorage.setItem(STORAGE_KEYS.USER_ID, userId);
    }
    if (res) {
      localStorage.setItem(STORAGE_KEYS.PROFILE_DATA, JSON.stringify(res));
    }
    return data;
  },

  async register(payload: RegisterPayload) {
    const { data } = await authApi.postApiV1AuthRegister(
      mapRegisterPayload(payload),
    );
    return data;
  },

  async refresh() {
    return authRefreshService.refresh();
  },

  async logout() {
    const refreshToken = tokenService.getRefreshToken();
    try {
      if (refreshToken) {
        await authApi.postApiV1AuthLogout({
          refresh_token: refreshToken,
        });
      }
    } finally {
      tokenService.clearTokens();
      localStorage.removeItem(STORAGE_KEYS.USER_ID);
      localStorage.removeItem(STORAGE_KEYS.PROFILE_DATA);
      window.location.reload();
    }
  },
};
