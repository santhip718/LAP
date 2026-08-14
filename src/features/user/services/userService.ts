import apiClient from "../../../shared/services/api/apiClient";
import type { UserDetailDto } from "../../../shared/services/api/models/userDetailDto";
import type { UserProfileDto } from "../../../shared/services/api/models/userProfileDto";
import { getUser } from "../../../shared/services/api/services/user/user";

import type { UserSummaryDto } from "../../../shared/services/api/models/userSummaryDto";
import type { RefTermDto } from "../../../shared/services/api/models/refTermDto";
import { USER_ROLES } from "../../../shared/constants/roles";
import { STORAGE_KEYS } from "../../../shared/constants/storage";
import {
  getCurrentUserId,
  getCurrentUserEmail,
  getCurrentUserFullName,
  getCurrentUserPhone,
  getCurrentUserProfileImage,
  getUserRoles,
} from "../../auth/utils/authHelpers";
import type { UserListItem, UserListResult, UserProfile, UserDetail, EditUserRequest, RefTerm } from "../types/userServiceTypes";
import { FALLBACK_USER_NAME, FALLBACK_EMPTY, FALLBACK_NAME, DEFAULT_PAGE_SIZE, SEARCH_PAGE_SIZE, CONTENT_TYPE_MULTIPART } from "../utils/constants";

export type { UserListItem, UserListResult, UserProfile, UserDetail, EditUserRequest, RefTerm } from "../types/userServiceTypes";

const userApi = getUser(apiClient);

const mapUserSummary = (dto: UserSummaryDto): UserListItem => ({
  id: dto.id ?? "",
  fullName: dto.full_name ?? FALLBACK_USER_NAME,
  email: dto.email ?? "",
  roles: dto.roles ?? [],
});

const mapUserDetail = (dto: UserDetailDto): UserDetail => ({
  id: dto.id ?? "",
  fullName: dto.full_name ?? FALLBACK_USER_NAME,
  email: dto.email ?? "",
  mobileNumber: dto.mobile_number ?? "",
  designation: dto.designation?.name || FALLBACK_EMPTY,
  designationId: dto.designation?.id ?? "",
  gender: dto.gender?.name || FALLBACK_EMPTY,
  genderId: dto.gender?.id ?? "",
  currentTier: dto.current_tier?.name || FALLBACK_EMPTY,
  roles: dto.roles ?? [],
  dateCreated: dto.date_created ?? "",
  profileImage: dto.profile_image ?? null,
});

const mapUserProfile = (dto: UserDetailDto): UserProfile => ({
  id: dto.id ?? "",
  fullName: dto.full_name ?? FALLBACK_USER_NAME,
  email: dto.email ?? "",
  mobileNumber: dto.mobile_number ?? "",
  designation: dto.designation?.name || FALLBACK_EMPTY,
  designationId: dto.designation?.id ?? "",
  gender: dto.gender?.name || FALLBACK_EMPTY,
  genderId: dto.gender?.id ?? "",
  currentTier: dto.current_tier?.name || FALLBACK_EMPTY,
  roles: dto.roles ?? [],
  dateCreated: dto.date_created ?? "",
  profileImage: dto.profile_image ?? null,
});

const mapUserProfileDto = (dto: UserProfileDto): UserProfile => ({
  id: dto.id ?? "",
  fullName: dto.full_name ?? FALLBACK_USER_NAME,
  email: dto.email ?? "",
  mobileNumber: dto.mobile_number ?? "",
  designation: dto.designation?.name || FALLBACK_EMPTY,
  designationId: dto.designation?.id ?? "",
  gender: dto.gender?.name || FALLBACK_EMPTY,
  genderId: dto.gender?.id ?? "",
  currentTier: dto.current_tier?.name || FALLBACK_EMPTY,
  roles: dto.roles ?? [],
  dateCreated: dto.date_created ?? "",
  profileImage: dto.profile_image ?? null,
});

export const userService = {
  async getUserList(
    page = 1,
    pageSize = DEFAULT_PAGE_SIZE,
    search = "",
  ): Promise<UserListResult> {
    const params = new URLSearchParams();
    params.set("page", String(page));
    params.set("page_size", String(pageSize));
    if (search) params.set("search", search);

    const { data } = await apiClient.get<{
      data: UserSummaryDto[];
      total: number;
      page: number;
      page_size: number;
    }>(`/api/v1/user?${params.toString()}`);

    return {
      users: (data.data ?? []).map(mapUserSummary),
      total: data.total ?? 0,
      page: data.page ?? 1,
      pageSize: data.page_size ?? DEFAULT_PAGE_SIZE,
    };
  },

  async getUserDetail(userId: string): Promise<UserDetail> {
    const { data } = await apiClient.get<UserDetailDto>(
      `/api/v1/user/${userId}`,
    );
    return mapUserDetail(data);
  },

  async getMyProfile(): Promise<UserProfile> {
    const userId = getCurrentUserId();
    const roles = getUserRoles();
    const isStudent = roles.length === 0 || roles.includes(USER_ROLES.STUDENT);

    if (userId && isStudent) {
      try {
        const { data } = await userApi.getApiV1UserIdProfile(userId);
        if (data) {
          return mapUserProfileDto(data);
        }
      } catch (err) {
        console.error("Failed to fetch profile via profile endpoint:", err);
      }
    }

    if (userId) {
      try {
        const { data } = await apiClient.get<UserDetailDto>(
          `/api/v1/user/${userId}`,
        );
        if (data) {
          return mapUserProfile(data);
        }
      } catch (err) {
        console.error("Failed to fetch profile by userId:", err);
      }
    }

    const email = getCurrentUserEmail();
    if (email) {
      try {
        const params = new URLSearchParams({ page: "1", page_size: String(SEARCH_PAGE_SIZE), search: email });
        const { data } = await apiClient.get<{
          data: UserSummaryDto[];
        }>(`/api/v1/user?${params.toString()}`);
        const user = data?.data?.find((u) => u.email?.toLowerCase() === email.toLowerCase());
        if (user?.id) {
          if (isStudent) {
            try {
              const { data: profileData } = await userApi.getApiV1UserIdProfile(user.id);
              if (profileData) {
                return mapUserProfileDto(profileData);
              }
            } catch {
              // fall through to detail endpoint
            }
          }
          const { data: detail } = await apiClient.get<UserDetailDto>(
            `/api/v1/user/${user.id}`,
          );
          if (detail) {
            return mapUserProfile(detail);
          }
        }
      } catch (err) {
        console.error("Failed to fetch profile by email:", err);
      }
    }

    try {
      const rawProfile = localStorage.getItem(STORAGE_KEYS.PROFILE_DATA);
      if (rawProfile) {
        const profileData = JSON.parse(rawProfile) as Record<string, unknown>;
        const profileUserId = (profileData.userId ?? profileData.user_id ?? profileData.id ?? "") as string;
        if (profileUserId) {
          if (isStudent) {
            try {
              const { data: profData } = await userApi.getApiV1UserIdProfile(profileUserId);
              if (profData) {
                return mapUserProfileDto(profData);
              }
            } catch {
              // fall through
            }
          }
          const { data } = await apiClient.get<UserDetailDto>(
            `/api/v1/user/${profileUserId}`,
          );
          if (data) {
            return mapUserProfile(data);
          }
        }
      }
    } catch (err) {
      console.error("Failed to fetch profile by profile data userId:", err);
    }

    const name = getCurrentUserFullName();
    const jwtEmail = getCurrentUserEmail();
    const phone = getCurrentUserPhone();
    const picture = getCurrentUserProfileImage();
    const jwtUserId = getCurrentUserId();

    return {
      id: jwtUserId ?? "",
      fullName: name ?? FALLBACK_NAME,
      email: jwtEmail ?? "",
      mobileNumber: phone ?? "",
      designation: FALLBACK_EMPTY,
      designationId: "",
      gender: FALLBACK_EMPTY,
      genderId: "",
      currentTier: FALLBACK_EMPTY,
      roles: roles.length > 0 ? roles : [USER_ROLES.STUDENT],
      dateCreated: "",
      profileImage: picture ?? null,
    };
  },

  async updateUser(userId: string, input: EditUserRequest): Promise<void> {
    const body = {
      full_name: input.fullName,
      email: input.email,
      mobile_number: input.mobileNumber,
      designation_id: input.designationId,
      gender_id: input.genderId,
      roles: input.roles,
    } satisfies Record<string, unknown>;
    await apiClient.put(`/api/v1/user/${userId}`, body);
  },

  async deleteUser(userId: string): Promise<void> {
    await apiClient.delete(`/api/v1/user/${userId}`);
  },

  async uploadProfileImage(file: File): Promise<string> {
    const formData = new FormData();
    formData.append("file", file);
    const { data } = await apiClient.post<Record<string, unknown>>(
      "/api/v1/user/profile-image",
      formData,
      {
        headers: { "Content-Type": CONTENT_TYPE_MULTIPART },
      },
    );
    const url =
      (data.profile_image as string) ??
      (data.profileImage as string) ??
      (data.url as string) ??
      "";
    return url;
  },

  async getDesignations(): Promise<RefTerm[]> {
    const { data } = await apiClient.get<RefTermDto[]>(
      "/api/v1/reference-data/designation",
    );
    return (Array.isArray(data) ? data : []).map((d) => ({
      id: d.id ?? "",
      name: d.name ?? "",
    }));
  },

  async getGenders(): Promise<RefTerm[]> {
    const { data } = await apiClient.get<RefTermDto[]>(
      "/api/v1/reference-data/gender",
    );
    return (Array.isArray(data) ? data : []).map((d) => ({
      id: d.id ?? "",
      name: d.name ?? "",
    }));
  },
};
