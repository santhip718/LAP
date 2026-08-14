import { useNavigate } from "react-router-dom";
import {
  Box,
  Button,
  Container,
  Paper,
  Typography,
  Accordion,
  AccordionDetails,
  AccordionSummary,
} from "@mui/material";
import { HomeOutlined, RefreshOutlined, ExpandMoreOutlined } from "@mui/icons-material";
import type { LapGenericErrorFallbackProps } from "@/shared/types/feedback.types";
import "./LapGenericErrorFallback.css";

export default function LapGenericErrorFallback({
  error,
  onReset,
  showDetails = import.meta.env.DEV,
}: LapGenericErrorFallbackProps) {
  const navigate = useNavigate();

  const handleReload = () => {
    window.location.reload();
  };

  const handleGoHome = () => {
    navigate("/");
  };

  return (
    <Container maxWidth="md" sx={{ mt: { xs: 4, sm: 8 }, px: { xs: 1, sm: 2 } }}>
      <Paper elevation={3} sx={{ p: { xs: 2, sm: 4 }, textAlign: "center" }}>
        <Typography
          variant="h4"
          gutterBottom
          color="error"
          sx={{ fontSize: { xs: "1.5rem", sm: "2rem" }, fontWeight: 600 }}
        >
          Oops! Something Went Wrong
        </Typography>
        <Typography variant="body1" color="text.secondary" sx={{ mb: 3, maxWidth: 520, mx: "auto" }}>
          We encountered an unexpected error. Please try refreshing the page or
          contact support if the problem persists.
        </Typography>

        {showDetails && error && (
          <Accordion sx={{ mb: 3, textAlign: "left" }}>
            <AccordionSummary expandIcon={<ExpandMoreOutlined />}>
              <Typography variant="subtitle2" color="error">
                Error Details
              </Typography>
            </AccordionSummary>
            <AccordionDetails>
              <Box
                component="pre"
                className="lap-generic-error-details"
                sx={{
                  bgcolor: "grey.100",
                  p: 2,
                  borderRadius: 1,
                  overflow: "auto",
                  wordBreak: "break-word",
                  fontSize: "0.75rem",
                  maxHeight: 300,
                }}
              >
                {error.message}
                {"\n"}
                {error.stack}
              </Box>
            </AccordionDetails>
          </Accordion>
        )}

        <Box
          sx={{
            display: "flex",
            gap: { xs: 1, sm: 2 },
            justifyContent: "center",
            flexWrap: "wrap",
          }}
        >
          {onReset && (
            <Button
              variant="outlined"
              color="primary"
              onClick={onReset}
              startIcon={<RefreshOutlined />}
              sx={{ textTransform: "none", minWidth: 120 }}
            >
              Try Again
            </Button>
          )}
          <Button
            variant="contained"
            color="primary"
            onClick={handleReload}
            startIcon={<RefreshOutlined />}
            sx={{ textTransform: "none", minWidth: 120 }}
          >
            Reload Page
          </Button>
          <Button
            variant="outlined"
            color="secondary"
            onClick={handleGoHome}
            startIcon={<HomeOutlined />}
            sx={{ textTransform: "none", minWidth: 120 }}
          >
            Go Home
          </Button>
        </Box>
      </Paper>
    </Container>
  );
}
