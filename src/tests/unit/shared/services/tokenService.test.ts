import { tokenService } from "@/shared/services/storage/tokenService";
import { STORAGE_KEYS } from "@/shared/constants/storage";

describe("tokenService", () => {
  beforeEach(() => {
    jest.restoreAllMocks();
    localStorage.clear();
  });

  describe("getAccessToken", () => {
    it("returns the access token from localStorage", () => {
      localStorage.setItem(STORAGE_KEYS.ACCESS_TOKEN, "test-at");
      expect(tokenService.getAccessToken()).toBe("test-at");
    });

    it("returns null when no access token stored", () => {
      expect(tokenService.getAccessToken()).toBeNull();
    });
  });

  describe("getRefreshToken", () => {
    it("returns the refresh token from localStorage", () => {
      localStorage.setItem(STORAGE_KEYS.REFRESH_TOKEN, "test-rt");
      expect(tokenService.getRefreshToken()).toBe("test-rt");
    });

    it("returns null when no refresh token stored", () => {
      expect(tokenService.getRefreshToken()).toBeNull();
    });
  });

  describe("setTokens", () => {
    it("stores both tokens in localStorage", () => {
      tokenService.setTokens("at-123", "rt-456");
      expect(localStorage.getItem(STORAGE_KEYS.ACCESS_TOKEN)).toBe("at-123");
      expect(localStorage.getItem(STORAGE_KEYS.REFRESH_TOKEN)).toBe("rt-456");
    });

    it("overwrites previously stored tokens", () => {
      localStorage.setItem(STORAGE_KEYS.ACCESS_TOKEN, "old-at");
      localStorage.setItem(STORAGE_KEYS.REFRESH_TOKEN, "old-rt");
      tokenService.setTokens("new-at", "new-rt");
      expect(localStorage.getItem(STORAGE_KEYS.ACCESS_TOKEN)).toBe("new-at");
      expect(localStorage.getItem(STORAGE_KEYS.REFRESH_TOKEN)).toBe("new-rt");
    });

    it("uses correct storage keys", () => {
      const setSpy = jest.spyOn(Storage.prototype, "setItem");
      tokenService.setTokens("at", "rt");
      expect(setSpy).toHaveBeenCalledWith(STORAGE_KEYS.ACCESS_TOKEN, "at");
      expect(setSpy).toHaveBeenCalledWith(STORAGE_KEYS.REFRESH_TOKEN, "rt");
    });
  });

  describe("clearTokens", () => {
    it("removes both tokens from localStorage", () => {
      localStorage.setItem(STORAGE_KEYS.ACCESS_TOKEN, "at");
      localStorage.setItem(STORAGE_KEYS.REFRESH_TOKEN, "rt");
      tokenService.clearTokens();
      expect(localStorage.getItem(STORAGE_KEYS.ACCESS_TOKEN)).toBeNull();
      expect(localStorage.getItem(STORAGE_KEYS.REFRESH_TOKEN)).toBeNull();
    });

    it("uses correct storage keys", () => {
      const removeSpy = jest.spyOn(Storage.prototype, "removeItem");
      tokenService.clearTokens();
      expect(removeSpy).toHaveBeenCalledWith(STORAGE_KEYS.ACCESS_TOKEN);
      expect(removeSpy).toHaveBeenCalledWith(STORAGE_KEYS.REFRESH_TOKEN);
    });

    it("does not throw when no tokens exist", () => {
      expect(() => tokenService.clearTokens()).not.toThrow();
    });
  });
});
