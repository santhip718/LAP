import { useCallback, useEffect, useRef, useState } from "react";
import { userService } from "../services/userService";
import type { UserProfile, UseUserProfileResult } from "../types";
import { userProfileStrings } from "../pages/UserProfile/constants";

export function useUserProfile(): UseUserProfileResult {
  const [profile, setProfile] = useState<UserProfile | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [uploading, setUploading] = useState(false);
  const isMountedRef = useRef(true);

  const fetchProfile = useCallback(async (silent = false) => {
    if (!silent) {
      setLoading(true);
    }
    setError(null);

    try {
      const result = await userService.getMyProfile();
      if (isMountedRef.current) {
        setProfile(result);
      }
    } catch (err) {
      console.error("Failed to load profile:", err);
      if (isMountedRef.current) {
        setError(userProfileStrings.loadProfileError);
        setProfile(null);
      }
    } finally {
      if (isMountedRef.current) {
        setLoading(false);
      }
    }
  }, []);

  const uploadImage = useCallback(async (file: File): Promise<string> => {
    setUploading(true);
    try {
      const imageUrl = await userService.uploadProfileImage(file);
      if (isMountedRef.current) {
        setProfile((prev) =>
          prev ? { ...prev, profileImage: imageUrl || prev.profileImage } : prev,
        );
      }
      return imageUrl;
    } catch (err) {
      console.error("Failed to upload image:", err);
      throw err;
    } finally {
      if (isMountedRef.current) {
        setUploading(false);
      }
    }
  }, []);

  useEffect(() => {
    isMountedRef.current = true;
    queueMicrotask(() => {
      void fetchProfile();
    });
    return () => {
      isMountedRef.current = false;
    };
  }, [fetchProfile]);

  return {
    profile,
    loading,
    error,
    refresh: fetchProfile,
    uploadImage,
    uploading,
  };
}
