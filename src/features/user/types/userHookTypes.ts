import type { UserListItem, UserProfile } from "./userServiceTypes";

export interface UseUserListResult {
  users: UserListItem[];
  total: number;
  page: number;
  pageSize: number;
  loading: boolean;
  error: string | null;
  refresh: () => void;
  setPage: (page: number) => void;
  setPageSize: (size: number) => void;
  setSearch: (search: string) => void;
  search: string;
  loadMore?: () => void;
  loadingMore?: boolean;
  hasMore?: boolean;
}

export interface UseUserProfileResult {
  profile: UserProfile | null;
  loading: boolean;
  error: string | null;
  refresh: () => void;
  uploadImage: (file: File) => Promise<string>;
  uploading: boolean;
}
