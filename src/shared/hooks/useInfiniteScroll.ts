import { useEffect, useRef, useState, useCallback } from "react";

interface FetchModeOptions<T> {
  fetchFn: (page: number) => Promise<T[]>;
  initialPage?: number;
  items?: undefined;
  pageSize?: undefined;
  onLoadMore?: undefined;
  hasMore?: undefined;
}

interface ClientModeOptions<T> {
  fetchFn?: undefined;
  initialPage?: undefined;
  items: T[];
  pageSize: number;
  onLoadMore?: () => void;
  hasMore?: boolean;
}

type UseInfiniteScrollOptions<T> = FetchModeOptions<T> | ClientModeOptions<T>;

export function useInfiniteScroll<T>(options: UseInfiniteScrollOptions<T>) {
  const isClientMode = "items" in options;

  // Safely extract options for hooks (always run hooks unconditionally)
  const clientItems = isClientMode ? (options as ClientModeOptions<T>).items : [];
  const clientPageSize = isClientMode ? (options as ClientModeOptions<T>).pageSize : 10;
  const clientOnLoadMore = isClientMode ? (options as ClientModeOptions<T>).onLoadMore : undefined;
  const clientHasMore = isClientMode ? (options as ClientModeOptions<T>).hasMore : undefined;

  const fetchFn = !isClientMode ? (options as FetchModeOptions<T>).fetchFn : undefined;
  const initialPage = !isClientMode ? ((options as FetchModeOptions<T>).initialPage ?? 1) : 1;

  // Shared states
  const [items, setItems] = useState<T[]>([]);
  const [loading, setLoading] = useState(false);
  const [hasMore, setHasMore] = useState(true);
  const [visibleCount, setVisibleCount] = useState(clientPageSize);

  // Refs for checking fresh values without re-triggering effects
  const pageRef = useRef(initialPage);
  const loadingRef = useRef(false);
  const hasMoreRef = useRef(true);
  const fetchFnRef = useRef(fetchFn);
  const scrollContainerRef = useRef<HTMLElement | null>(null);
  const sentinelElementRef = useRef<HTMLElement | null>(null);
  const observerRef = useRef<IntersectionObserver | null>(null);

  // Keep refs in sync
  useEffect(() => {
    fetchFnRef.current = fetchFn;
  }, [fetchFn]);

  useEffect(() => {
    hasMoreRef.current = hasMore;
  }, [hasMore]);

  useEffect(() => {
    loadingRef.current = loading;
  }, [loading]);

  const onLoadMoreRef = useRef(clientOnLoadMore);
  useEffect(() => {
    onLoadMoreRef.current = clientOnLoadMore;
  }, [clientOnLoadMore]);

  // Sync client mode items/pageSize changes
  useEffect(() => {
    if (isClientMode) {
      setItems(clientItems);
      if (clientOnLoadMore) {
        // If server-side paginated, keep visibleCount matched with loaded items
        setVisibleCount(clientItems.length);
      } else {
        setVisibleCount(clientPageSize);
      }
      if (clientHasMore !== undefined) {
        setHasMore(clientHasMore);
      } else {
        setHasMore(clientPageSize < clientItems.length);
      }
    }
  }, [isClientMode, clientItems, clientPageSize, clientHasMore, clientOnLoadMore]);

  // Sync hasMore for client mode based on visibleCount
  useEffect(() => {
    if (isClientMode) {
      if (clientHasMore !== undefined) {
        setHasMore(clientHasMore);
      } else {
        setHasMore(visibleCount < items.length);
      }
    }
  }, [isClientMode, visibleCount, items.length, clientHasMore]);

  // Load more logic
  const loadMore = useCallback(async () => {
    if (isClientMode) {
      if (visibleCount >= items.length) {
        if (onLoadMoreRef.current) {
          onLoadMoreRef.current();
        } else {
          setHasMore(false);
        }
        return;
      }
      setVisibleCount((prev) => prev + clientPageSize);
      return;
    }

    // Fetch mode
    if (loadingRef.current || !hasMoreRef.current) return;
    loadingRef.current = true;
    setLoading(true);
    try {
      const data = await fetchFnRef.current!(pageRef.current);
      if (data.length === 0) {
        setHasMore(false);
      } else {
        setItems((prev) => [...prev, ...data]);
        pageRef.current += 1;
      }
    } catch {
      setHasMore(false);
    } finally {
      setLoading(false);
      loadingRef.current = false;
    }
  }, [isClientMode, visibleCount, items.length, clientPageSize]);

  // Keep loadMore ref updated to prevent stale closures in IntersectionObserver
  const loadMoreRef = useRef(loadMore);
  useEffect(() => {
    loadMoreRef.current = loadMore;
  }, [loadMore]);

  // Reconnect observer helper
  const reconnectObserver = useCallback(() => {
    if (observerRef.current) {
      observerRef.current.disconnect();
      observerRef.current = null;
    }

    const sentinel = sentinelElementRef.current;
    if (!sentinel || !hasMoreRef.current) return;

    const observer = new IntersectionObserver(
      (entries) => {
        if (entries[0].isIntersecting && hasMoreRef.current) {
          loadMoreRef.current();
        }
      },
      {
        root: scrollContainerRef.current || null,
        rootMargin: "200px",
        threshold: 0,
      }
    );

    observer.observe(sentinel);
    observerRef.current = observer;
  }, []);

  // Callback refs to watch mount/unmount of target elements
  const sentinelRef = useCallback((node: HTMLElement | null) => {
    sentinelElementRef.current = node;
    reconnectObserver();
  }, [reconnectObserver]);

  const setScrollContainerRef = useCallback((node: HTMLElement | null) => {
    scrollContainerRef.current = node;
    reconnectObserver();
  }, [reconnectObserver]);

  // Connect observer and handle cleanup
  useEffect(() => {
    reconnectObserver();
    return () => {
      if (observerRef.current) {
        observerRef.current.disconnect();
      }
    };
  }, [hasMore, reconnectObserver]);

  const reset = useCallback(() => {
    setItems(isClientMode ? clientItems : []);
    pageRef.current = initialPage;
    setHasMore(true);
    setLoading(false);
    loadingRef.current = false;
    if (isClientMode) {
      if (clientOnLoadMore) {
        setVisibleCount(clientItems.length);
      } else {
        setVisibleCount(clientPageSize);
      }
    }
  }, [isClientMode, clientItems, clientPageSize, initialPage, clientOnLoadMore]);

  const updateItem = useCallback(
    (predicate: (item: T) => boolean, updates: Partial<T>) => {
      setItems((prev) =>
        prev.map((item) => (predicate(item) ? { ...item, ...updates } : item)),
      );
    },
    [],
  );

  const displayedItems = isClientMode ? items.slice(0, visibleCount) : items;

  return {
    displayedItems,
    items,
    loading,
    hasMore,
    sentinelRef,
    setScrollContainerRef,
    reset,
    loadMore,
    updateItem,
  };
}
