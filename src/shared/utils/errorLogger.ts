export const logError = (error: Error, componentStack?: string): void => {
  if (import.meta.env.PROD) {
    console.error("ErrorBoundary caught an error:", {
      message: error.message,
      stack: error.stack,
      componentStack,
      timestamp: new Date().toISOString(),
      url: globalThis.location.href,
      userAgent: navigator.userAgent,
    });
  } else {
    console.error("[ErrorBoundary] Error caught in development:", {
      error,
      componentStack,
    });
  }
};
