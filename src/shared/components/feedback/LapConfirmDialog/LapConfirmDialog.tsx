import { useEffect, useState } from "react";
import Dialog from "@mui/material/Dialog";
import DialogTitle from "@mui/material/DialogTitle";
import DialogContent from "@mui/material/DialogContent";
import DialogContentText from "@mui/material/DialogContentText";
import DialogActions from "@mui/material/DialogActions";
import Button from "@mui/material/Button";
import { feedbackService } from "../../../services/feedback/feedbackService";
import type { ConfirmConfig } from "../../../services/feedback/feedbackService";
import type { DialogState } from "@/shared/types/feedback.types";
import {
  EVENT_NAME,
  DEFAULT_CANCEL_LABEL,
  DEFAULT_CONFIRM_LABEL,
  CONFIRM_BUTTON_COLOR,
  DIALOG_MAX_WIDTH,
} from "./LapConfirmDialog.constants";

export default function LapConfirmDialog() {
  const [state, setState] = useState<DialogState>({
    open: false,
    config: null,
    resolve: null,
  });

  useEffect(() => {
    const handleShow = (payload: {
      config: ConfirmConfig;
      resolve: (value: boolean) => void;
    }) => {
      setState({
        open: true,
        config: payload.config,
        resolve: payload.resolve,
      });
    };

    feedbackService.on(EVENT_NAME, handleShow as (...args: unknown[]) => void);
    return () => {
      feedbackService.off(EVENT_NAME, handleShow as (...args: unknown[]) => void);
    };
  }, []);

  const handleConfirm = () => {
    state.resolve?.(true);
    setState((prev) => ({ ...prev, open: false }));
  };

  const handleCancel = () => {
    state.resolve?.(false);
    setState((prev) => ({ ...prev, open: false }));
  };

  return (
    <Dialog
      open={state.open}
      onClose={handleCancel}
      maxWidth={DIALOG_MAX_WIDTH}
      fullWidth
    >
      {state.config && (
        <>
          <DialogTitle>{state.config.title}</DialogTitle>
          <DialogContent>
            <DialogContentText>{state.config.message}</DialogContentText>
          </DialogContent>
          <DialogActions>
            <Button onClick={handleCancel}>
              {state.config.cancelLabel ?? DEFAULT_CANCEL_LABEL}
            </Button>
            <Button onClick={handleConfirm} variant="contained" color={CONFIRM_BUTTON_COLOR}>
              {state.config.confirmLabel ?? DEFAULT_CONFIRM_LABEL}
            </Button>
          </DialogActions>
        </>
      )}
    </Dialog>
  );
}
