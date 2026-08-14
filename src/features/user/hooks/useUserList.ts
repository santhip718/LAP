import { useCallback, useEffect, useRef, useState } from "react";
import { userService } from "../services/userService";
import type { UserListItem, UserListResult, UseUserListResult } from "../types";
import { DEFAULT_PAGE_SIZE, userListStrings } from "../utils/constants";

export function useUserList(): UseUserListResult {
  const [users, setUsers] = useState<UserListItem[]>([]);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);
  const [search, setSearch] = useState("");
  const [loading, setLoading] = useState(true);
  const [loadingMore, setLoadingMore] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const isMountedRef = useRef(true);

  const searchRef = useRef(search);
  useEffect(() => {
    searchRef.current = search;
  }, [search]);

  const fetchUsers = useCallback(async () => {
    setLoading(true);
    setError(null);
    setPage(1);

    try {
      const result: UserListResult = await userService.getUserList(1, pageSize, search);
      if (isMountedRef.current) {
        setUsers(result.users);
        setTotal(result.total);
      }
    } catch (err) {
      console.error("Failed to load users:", err);
      if (isMountedRef.current) {
        setError(userListStrings.error.loadFailed);
        setUsers([]);
        setTotal(0);
      }
    } finally {
      if (isMountedRef.current) {
        setLoading(false);
      }
    }
  }, [pageSize, search]);

  const loadMore = useCallback(async () => {
    if (loadingMore || loading || users.length >= total) return;
    setLoadingMore(true);

    const nextPage = page + 1;
    const currentSearch = searchRef.current;

    try {
      const result: UserListResult = await userService.getUserList(nextPage, pageSize, currentSearch);
      if (isMountedRef.current) {
        setUsers((prev) => [...prev, ...result.users]);
        setPage(nextPage);
        setTotal(result.total);
      }
    } catch (err) {
      console.error("Failed to load more users:", err);
    } finally {
      if (isMountedRef.current) {
        setLoadingMore(false);
      }
    }
  }, [loadingMore, loading, users.length, total, page, pageSize]);

  useEffect(() => {
    isMountedRef.current = true;
    queueMicrotask(() => {
      void fetchUsers();
    });
    return () => {
      isMountedRef.current = false;
    };
  }, [fetchUsers]);

  const hasMore = users.length < total;

  return {
    users,
    total,
    page,
    pageSize,
    loading,
    loadingMore,
    error,
    hasMore,
    loadMore,
    refresh: fetchUsers,
    setPage: (p: number) => setPage(p),
    setPageSize: (s: number) => {
      setPageSize(s);
      setPage(1);
    },
    setSearch: (s: string) => {
      setSearch(s);
      setPage(1);
    },
    search,
  };
}
