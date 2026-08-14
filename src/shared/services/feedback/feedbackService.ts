export type ToastType = "success" | "error" | "info" | "warning";

export interface ToastItem {
  id: string;
  message: string;
  type: ToastType;
  duration: number;
}

export interface ConfirmConfig {
  title: string;
  message: string;
  confirmLabel?: string;
  cancelLabel?: string;
}

export interface ConfirmDialogState {
  open: boolean;
  config: ConfirmConfig | null;
  resolve: ((value: boolean) => void) | null;
}

type Listener = (...args: unknown[]) => void;

class FeedbackService {
  private toastId = 0;
  private events: Record<string, Listener[]> = {};

  on(event: string, listener: Listener) {
    if (!this.events[event]) {
      this.events[event] = [];
    }
    this.events[event].push(listener);
  }

  off(event: string, listener: Listener) {
    if (!this.events[event]) return;
    this.events[event] = this.events[event].filter((l) => l !== listener);
  }

  emit(event: string, ...args: unknown[]) {
    if (!this.events[event]) return;
    this.events[event].forEach((listener) => listener(...args));
  }

  showToast(message: string, type: ToastType = "info", duration = 3000) {
    const id = `toast-${++this.toastId}`;
    const toast: ToastItem = { id, message, type, duration };
    this.emit("toast:show", toast);
  }

  dismissToast(id: string) {
    this.emit("toast:dismiss", id);
  }

  showConfirm(config: ConfirmConfig): Promise<boolean> {
    return new Promise((resolve) => {
      this.emit("confirm:show", { config, resolve });
    });
  }
}

export const feedbackService = new FeedbackService();
