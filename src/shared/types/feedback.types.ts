import type { ReactNode } from "react";
import type { ConfirmConfig } from "@/shared/services/feedback/feedbackService";

export interface LapModalDialogProps {
  open: boolean;
  onClose: () => void;
  title: string;
  children: ReactNode;
  subtitle?: string;
  size?: "xs" | "sm" | "md" | "lg" | "xl";
  actions?: ReactNode;
  maxWidth?: "xs" | "sm" | "md" | "lg" | "xl";
}

export interface DialogState {
  open: boolean;
  config: ConfirmConfig | null;
  resolve: ((value: boolean) => void) | null;
}

export interface LapRouteErrorBoundaryProps {
  children: ReactNode;
}

export interface LapGenericErrorFallbackProps {
  error?: Error | null;
  onReset?: () => void;
  showDetails?: boolean;
}
