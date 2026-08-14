import Dialog from "@mui/material/Dialog";
import DialogTitle from "@mui/material/DialogTitle";
import DialogContent from "@mui/material/DialogContent";
import IconButton from "@mui/material/IconButton";
import type { LapModalDialogProps } from "@/shared/types/feedback.types";

export default function LapModalDialog({
  open,
  onClose,
  title,
  subtitle,
  size = "sm",
  actions,
  children,
  maxWidth,
}: LapModalDialogProps) {
  return (
    <Dialog open={open} onClose={onClose} maxWidth={maxWidth ?? size} fullWidth>
      <DialogTitle sx={{ m: 0, p: 2, pr: 6 }}>
        {title}
        {subtitle && (
          <span style={{ display: "block", fontSize: "0.875rem", fontWeight: 400, marginTop: 4 }}>
            {subtitle}
          </span>
        )}
        <IconButton
          onClick={onClose}
          sx={{ position: "absolute", right: 8, top: 8 }}
        >
          <span className="material-symbols-outlined">close</span>
        </IconButton>
      </DialogTitle>
      <DialogContent dividers sx={{ p: 3 }}>
        {children}
      </DialogContent>
      {actions && (
        <div style={{ display: "flex", justifyContent: "flex-end", gap: 8, padding: "12px 24px" }}>
          {actions}
        </div>
      )}
    </Dialog>
  );
}
