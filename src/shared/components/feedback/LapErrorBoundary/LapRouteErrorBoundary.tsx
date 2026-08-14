import LapErrorBoundary from "./LapErrorBoundary";
import LapGenericErrorFallback from "./LapGenericErrorFallback";
import type { LapRouteErrorBoundaryProps } from "@/shared/types/feedback.types";

export default function LapRouteErrorBoundary({ children }: LapRouteErrorBoundaryProps) {
  return (
    <LapErrorBoundary
      fallback={
        <LapGenericErrorFallback
          onReset={() => window.location.reload()}
        />
      }
    >
      {children}
    </LapErrorBoundary>
  );
}
