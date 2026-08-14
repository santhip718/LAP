import { Component, type ReactNode } from "react";
import { Box, Button, Typography } from "@mui/material";
import { Refresh } from "@mui/icons-material";
import { logError } from "@/shared/utils/errorLogger";

interface Props {
  children: ReactNode;
  verbose?: boolean;
  fallback?: ReactNode;
  onReset?: () => void;
}

interface State {
  hasError: boolean;
  error: Error | null;
}

class LapErrorBoundary extends Component<Props, State> {
  constructor(props: Props) {
    super(props);
    this.state = { hasError: false, error: null };
  }

  static getDerivedStateFromError(error: Error): State {
    return { hasError: true, error };
  }

  componentDidCatch(error: Error, errorInfo: React.ErrorInfo) {
    logError(error, errorInfo.componentStack ?? undefined);
  }

  handleReset = () => {
    this.props.onReset?.();
    this.setState({ hasError: false, error: null });
  };

  render() {
    if (this.state.hasError) {
      if (this.props.fallback) {
        return this.props.fallback;
      }

      return (
        <Box
          sx={{
            display: "flex",
            flexDirection: "column",
            alignItems: "center",
            justifyContent: "center",
            minHeight: "50vh",
            px: { xs: 2, sm: 4 },
            py: 6,
            textAlign: "center",
          }}
        >
          <Typography
            variant="h5"
            gutterBottom
            color="error"
            sx={{ fontWeight: 600, fontSize: { xs: "1.25rem", sm: "1.5rem" } }}
          >
            Something went wrong.
          </Typography>
          <Typography
            variant="body2"
            color="text.secondary"
            sx={{ mb: 3, maxWidth: 480 }}
          >
            An unexpected error occurred. You can try again or reload the page.
          </Typography>
          <Button
            variant="contained"
            color="primary"
            onClick={this.handleReset}
            startIcon={<Refresh />}
            sx={{ textTransform: "none" }}
          >
            Try Again
          </Button>
          {this.props.verbose && this.state.error && (
            <Box
              component="pre"
              sx={{
                mt: 3,
                textAlign: "left",
                bgcolor: "grey.100",
                p: 2,
                borderRadius: 1,
                fontSize: "0.75rem",
                overflow: "auto",
                maxWidth: "100%",
                wordBreak: "break-word",
              }}
            >
              {this.state.error.toString()}
            </Box>
          )}
        </Box>
      );
    }

    return this.props.children;
  }
}

export default LapErrorBoundary;
