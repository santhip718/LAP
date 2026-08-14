import { useEffect, useState } from "react";
import Snackbar from "@mui/material/Snackbar";
import Alert from "@mui/material/Alert";
import { feedbackService } from "../../../services/feedback/feedbackService";
import type { ToastItem } from "../../../services/feedback/feedbackService";
import {
  CLICKAWAY_REASON,
  EVENT_NAMES,
  TOAST_ANCHOR_ORIGIN,
} from "./LapToast.constants";

export default function LapToast() {
  const [toast, setToast] = useState<ToastItem | null>(null);
  const [open, setOpen] = useState(false);

  useEffect(() => {
    const handleShow = (t: ToastItem) => {
      setToast(t);
      setOpen(true);
    };

    const handleDismiss = () => {
      setOpen(false);
    };

    feedbackService.on(EVENT_NAMES.SHOW, handleShow as (...args: unknown[]) => void);
    feedbackService.on(EVENT_NAMES.DISMISS, handleDismiss as (...args: unknown[]) => void);

    return () => {
      feedbackService.off(EVENT_NAMES.SHOW, handleShow as (...args: unknown[]) => void);
      feedbackService.off(EVENT_NAMES.DISMISS, handleDismiss as (...args: unknown[]) => void);
    };
  }, []);

  const handleClose = (_: Event | React.SyntheticEvent, reason?: string) => {
    if (reason === CLICKAWAY_REASON) return;
    setOpen(false);
  };

  if (!toast) return null;

  return (
    <Snackbar
      open={open}
      autoHideDuration={toast.duration}
      onClose={handleClose}
      anchorOrigin={TOAST_ANCHOR_ORIGIN}
    >
      <Alert
        onClose={handleClose}
        severity={toast.type}
        variant="filled"
        sx={{ width: "100%", minWidth: 280 }}
      >
        {toast.message}
      </Alert>
    </Snackbar>
  );
}
