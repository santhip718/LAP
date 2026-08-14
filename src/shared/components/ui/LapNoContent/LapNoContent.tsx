import Typography from "@mui/material/Typography";
import type { LapNoContentProps } from "@/shared/types/ui.types";
import "./LapNoContent.css";

export default function LapNoContent({
  icon = "inbox",
  title,
  message,
  children,
  className,
}: LapNoContentProps) {
  return (
    <div className={`lap-no-content ${className ?? ""}`}>
      <span className="material-symbols-outlined lap-no-content-icon">{icon}</span>
      <Typography variant="h6" className="lap-no-content-title">{title}</Typography>
      <Typography variant="body2" className="lap-no-content-message">{message}</Typography>
      {children}
    </div>
  );
}
