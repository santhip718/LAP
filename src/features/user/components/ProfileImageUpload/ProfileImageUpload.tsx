import { useState, useRef, useEffect, useCallback } from "react";
import Typography from "@mui/material/Typography";
import { feedbackService } from "../../../../shared/services/feedback/feedbackService";
import { userProfileStrings } from "../../pages/UserProfile/constants";
import type { ProfileImageUploadProps } from "../../types";
import "./ProfileImageUpload.css";

export default function ProfileImageUpload({
  currentImage,
  onUpload,
  uploading,
}: ProfileImageUploadProps) {
  const [preview, setPreview] = useState<string | null>(currentImage);
  const [dragOver, setDragOver] = useState(false);
  const inputRef = useRef<HTMLInputElement>(null);
  const blobRef = useRef<string | null>(null);

  const cleanupBlob = useCallback(() => {
    if (blobRef.current) {
      URL.revokeObjectURL(blobRef.current);
      blobRef.current = null;
    }
  }, []);

  useEffect(() => {
    setPreview(currentImage);
    cleanupBlob();
  }, [currentImage, cleanupBlob]);

  const handleFile = async (file: File) => {
    if (!file.type.startsWith(userProfileStrings.uploadConfig.typePrefix)) {
      feedbackService.showToast(userProfileStrings.upload.selectImage, "error");
      return;
    }
    if (file.size > userProfileStrings.uploadConfig.maxSize) {
      feedbackService.showToast(userProfileStrings.upload.imageTooLarge, "error");
      return;
    }

    const localPreview = URL.createObjectURL(file);
    cleanupBlob();
    blobRef.current = localPreview;
    setPreview(localPreview);

    try {
      await onUpload(file);
      feedbackService.showToast(userProfileStrings.uploadSuccess, "success");
    } catch {
      setPreview(currentImage);
      URL.revokeObjectURL(localPreview);
      blobRef.current = null;
      feedbackService.showToast(userProfileStrings.uploadError, "error");
    }
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) handleFile(file);
    if (inputRef.current) inputRef.current.value = "";
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    setDragOver(false);
    const file = e.dataTransfer.files[0];
    if (file) handleFile(file);
  };

  const getInitials = (name: string) => {
    return name
      .split(" ")
      .map((p) => p.charAt(0))
      .join("")
      .toUpperCase()
      .slice(0, 2);
  };

  return (
    <div
      className={`piu-container ${dragOver ? "piu-drag-over" : ""}`}
      onDragOver={(e) => { e.preventDefault(); setDragOver(true); }}
      onDragLeave={() => setDragOver(false)}
      onDrop={handleDrop}
    >
      <div className="piu-preview">
        {preview ? (
          <img src={preview} alt="Profile" className="piu-image" />
        ) : (
          <span className="material-symbols-outlined piu-placeholder">person</span>
        )}
        {uploading && (
          <div className="piu-overlay">
            <span className="material-symbols-outlined piu-spin">progress_activity</span>
          </div>
        )}
      </div>
      <button
        type="button"
        className="piu-upload-btn"
        onClick={() => inputRef.current?.click()}
        disabled={uploading}
      >
        <span className="material-symbols-outlined">photo_camera</span>
        {uploading ? userProfileStrings.upload.uploading : userProfileStrings.upload.changePhoto}
      </button>
      <Typography variant="caption" className="piu-hint">{userProfileStrings.upload.hint}</Typography>
      <input
        ref={inputRef}
        type="file"
        accept={userProfileStrings.uploadConfig.acceptedTypes}
        className="piu-input"
        onChange={handleInputChange}
      />
    </div>
  );
}
