const DEFAULT_TTL = 5 * 60 * 1000;

class CacheService {
  private cache = new Map<string, { data: unknown; expiry: number }>();

  get<T>(key: string): T | undefined {
    const entry = this.cache.get(key);
    if (!entry) return undefined;
    if (Date.now() > entry.expiry) {
      this.cache.delete(key);
      return undefined;
    }
    return entry.data as T;
  }

  set<T>(key: string, data: T, ttlMs: number = DEFAULT_TTL): void {
    this.cache.set(key, { data, expiry: Date.now() + ttlMs });
  }

  clear(): void {
    this.cache.clear();
  }

  delete(key: string): void {
    this.cache.delete(key);
  }
}

export const cacheService = new CacheService();
export { DEFAULT_TTL };
