import Typography from "@mui/material/Typography";
import type { LapAddButtonProps } from "@/shared/types/ui.types";
import "./LapAddButton.css";

export default function LapAddButton({
  onClick,
  label = "Add",
  icon = "add",
}: LapAddButtonProps) {
  return (
    <button className="lap-addbtn" onClick={onClick}>
      <span className="material-symbols-outlined lap-addbtn-icon">{icon}</span>
      <Typography variant="body2" component="span">
        {label}
      </Typography>
    </button>
  );
}
